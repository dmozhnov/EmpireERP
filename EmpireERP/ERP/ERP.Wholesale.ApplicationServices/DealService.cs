using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Settings;

namespace ERP.Wholesale.ApplicationServices
{
    public class DealService : IDealService
    {
        #region Поля

        private readonly IDealRepository dealRepository;
        private readonly IClientContractRepository clientContractRepository;
        private readonly ITaskRepository taskRepository;

        private readonly IDealIndicatorService dealIndicatorService;
        private readonly IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService;
        private readonly IStorageService storageService;
        private readonly IClientContractIndicatorService clientContractIndicatorService;

        #endregion

        #region Конструкторы

        public DealService(IDealRepository dealRepository, IClientContractRepository clientContractRepository, ITaskRepository taskRepository,
            IDealIndicatorService dealIndicatorService, IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService,
            IStorageService storageService, IClientContractIndicatorService clientContractIndicatorService)
        {
            this.dealRepository = dealRepository;
            this.clientContractRepository = clientContractRepository;
            this.taskRepository = taskRepository;

            this.dealIndicatorService = dealIndicatorService;
            this.expenditureWaybillIndicatorService = expenditureWaybillIndicatorService;
            this.storageService = storageService;
            this.clientContractIndicatorService = clientContractIndicatorService;
        }

        #endregion

        #region Методы

        #region Получение сделки

        private Deal GetById(int id, User user, Permission permission)
        {
            var type = user.GetPermissionDistributionType(permission);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var deal = dealRepository.GetById(id);

                if (type == PermissionDistributionType.All)
                {
                    return deal;
                }
                else
                {
                    var contains = user.Teams.SelectMany(x => x.Deals).Contains(deal);

                    if ((type == PermissionDistributionType.Personal && deal.Curator == user && contains) ||
                        (type == PermissionDistributionType.Teams && contains))
                    {
                        return deal;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Получение сделки по id с проверкой ее существования
        /// </summary>
        /// <param name="id">Код сделки</param>
        /// <returns>Сделка</returns>
        public Deal CheckDealExistence(int id, User user, string message = "")
        {
            return CheckDealExistence(id, user, Permission.Deal_List_Details, message);
        }

        public Deal CheckDealExistence(int id, User user, Permission permission, string message = "")
        {
            var deal = GetById(id, user, permission);

            ValidationUtils.NotNull(deal, String.IsNullOrEmpty(message) ? "Сделка не найдена. Возможно, она была удалена." : message);

            return deal;
        }

        /// <summary>
        /// Получение квоты по сделке по id с проверкой ее существования
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="id">Код квоты</param>
        /// <returns>Квота</returns>
        public DealQuota CheckDealQuotaExistence(Deal deal, int id)
        {
            var quota = deal.Quotas.FirstOrDefault(x => x.Id == id);

            ValidationUtils.NotNull(quota, "Квота не найдена. Возможно, она была удалена.");

            return quota;
        }

        // TODO: пока так
        /// <summary>
        /// Фильтрация сделок для пользователя
        /// </summary>
        /// <param name="list"></param>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public IEnumerable<Deal> FilterByUser(IEnumerable<Deal> list, User user, Permission permission)
        {
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<Deal>();

                case PermissionDistributionType.Personal:
                    return user.Teams.SelectMany(x => x.Deals.Where(y => y.Curator == user)).Intersect(list).Distinct();

                case PermissionDistributionType.Teams:
                    return user.Teams.SelectMany(x => x.Deals).Intersect(list).Distinct();

                case PermissionDistributionType.All:
                    return list;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Получение словаря сделок по списку их идентификаторов
        /// </summary>
        /// <param name="listId">Список идетификаторов</param>
        /// <returns></returns>
        public IDictionary<int, Deal> GetList(IList<int> listId)
        {
            return dealRepository.GetList(listId);
        }

        /// <summary>
        /// Получение списка видимых сделок по активным договорам за указанный период
        /// </summary>
        /// <param name="startDate">Начальная дата интервала</param>
        /// <param name="endDate">Конечная дата интервала</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Список сделок</returns>
        public IEnumerable<Deal> GetListByActiveContract(DateTime startDate, DateTime endDate, User user)
        {
            ISubCriteria<Deal> subQuery = null;
            switch (user.GetPermissionDistributionType(Permission.Deal_List_Details))
            {
                case PermissionDistributionType.None:
                    return new List<Deal>();
                case PermissionDistributionType.Personal:
                    subQuery = dealRepository.GetSubQueryForDealIdOnPersonalPermission(user.Id);
                    break;
                case PermissionDistributionType.Teams:
                    subQuery = dealRepository.GetSubQueryForDealIdOnTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.All:
                    subQuery = dealRepository.GetSubQueryForDealIdOnAllPermission();
                    break;
            }

            return dealRepository.GetListByActiveContract(startDate, endDate, subQuery);
        }

        #endregion

        #region Список

        public IEnumerable<Deal> GetFilteredList(object state, ParameterString param, User user, Permission permission)
        {
            Func<ISubCriteria<Deal>, ISubCriteria<Deal>> cond = null;
            
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<Deal>();

                case PermissionDistributionType.Personal:
                    cond = x => x.PropertyIn(y => y.Id, dealRepository.GetSubQueryForDealIdOnPersonalPermission(user.Id)).Select(y => y.Id);
                    break;

                case PermissionDistributionType.Teams:
                    cond = x => x.PropertyIn(y => y.Id, dealRepository.GetSubQueryForDealIdOnTeamPermission(user.Id)).Select(y => y.Id);
                    break;

                case PermissionDistributionType.All:
                    break;
            }
            
            return dealRepository.GetFilteredList(state, param, cond: cond);
        }

        public IEnumerable<Deal> GetFilteredList(object state, ParameterString param, User user)
        {
            if (param == null) param = new ParameterString("");

            return dealRepository.GetFilteredList(state, param);
        }
        
        #endregion

        #region Сохранение

        public int Save(Deal deal, User user)
        {
            CheckDealNameUniqueness(deal);
            // признак необходимости добавления сделки в команды текущего пользователя
            bool needAddToTeams = false;
            if (deal.Id == 0)
            {
                needAddToTeams = true;
            }

            dealRepository.Save(deal);

            // при добавлении сделки добавляем ее во все команды пользователя
            if (needAddToTeams)
            {
                foreach (var team in user.Teams)
                {
                    team.AddDeal(deal);
                }
            }

            return deal.Id;
        }

        /// <summary>
        /// Проверка имени сделки на уникальность
        /// </summary>
        /// <param name="deal">Сделка</param>
        public void CheckDealNameUniqueness(Deal deal)
        {
            var isUnique = dealRepository.Query<Deal>().Where(x => x.Name == deal.Name && x.Client.Id == deal.Client.Id && x.Id != deal.Id).Count() == 0;
            if (!isUnique)
            {
                throw new Exception(String.Format("Сделка с названием «{0}» уже существует у данного клиента.", deal.Name));
            }
        }

        #endregion

        #region Выставление договора с клиентом

        /// <summary>
        /// Назначить сделке указанный договор с клиентом.
        /// </summary>
        /// <param name="deal">Сделка.</param>
        /// <param name="contract">Договор с клиентом.</param>
        /// <param name="user">Пользователь, выполняющий операцию.</param>
        public void SetContract(Deal deal, ClientContract contract, User user)
        {
            if (deal.Contract == null)
            {
                CheckPossibilityToAddContract(deal, user);
            }
            else
            {
                CheckPossibilityToChangeContract(deal, user);
            }

            if (deal.Contract != contract)
            {
                CheckForMaxCashPaymentSumExceeding(deal, contract);

                var oldContract = deal.Contract;

                deal.Contract = contract;

                dealRepository.Save(deal);

                if (oldContract != null)
                {
                    if (clientContractRepository.IsUsedBySingleDeal(oldContract, deal)) //коммита еще не было, поэтому в БД у сделки еще старый договор
                    {
                        clientContractRepository.Delete(oldContract);
                    }
                }
            }
        }

        /// <summary>
        /// Проверка на превышение суммы оплат наличкой по сделке с учетом нового договора
        /// </summary>
        private void CheckForMaxCashPaymentSumExceeding(Deal deal, ClientContract clientContract)
        {
            var dealCashPaymentSumExceed = clientContractIndicatorService.CalculateCashPaymentLimitExcessByPaymentsFromClient(clientContract) + deal.CashDealPaymentSum;

            ValidationUtils.Assert(dealCashPaymentSumExceed <= 0,
               String.Format("Данная операция невозможна, так как максимально допустимая сумма наличных оплат от клиента по договору будет превышена на {0} р.",
               dealCashPaymentSumExceed.ForDisplay(ValueDisplayType.Money)));
        }

        #endregion

        #region Смена этапа сделки

        /// <summary>
        /// Переход на следующий этап
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="user">Пользователь</param>
        public void MoveToNextStage(Deal deal, User user)
        {
            CheckPossibilityToMoveToNextStage(deal, user);

            // меняем этап сделки
            deal.MoveToNextStage();
        }

        /// <summary>
        /// Переход на предыдущий этап
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="user">Пользователь</param>
        public void MoveToPreviousStage(Deal deal, User user)
        {
            CheckPossibilityToMoveToPreviousStage(deal, user);

            // меняем этап сделки
            deal.MoveToPreviousStage();
        }

        /// <summary>
        /// Переход на этап «Неуспешное закрытие»
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="user">Пользователь</param>
        public void MoveToUnsuccessfulClosingStage(Deal deal, User user)
        {
            CheckPossibilityToMoveToUnsuccessfulClosingStage(deal, user);

            // меняем этап сделки
            deal.MoveToUnsuccessfulClosingStage();
        }

        /// <summary>
        /// Переход на этап «Поиск принимающего решения»
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="user">Пользователь</param>
        public void MoveToDecisionMakerSearchStage(Deal deal, User user)
        {
            CheckPossibilityToMoveToDecisionMakerSearchStage(deal, user);

            // меняем этап сделки
            deal.MoveToDecisionMakerSearchStage();
        }

        /// <summary>
        /// Проверить возможность закрытия сделки (при переходе на этапы 7.1, 7.2)
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="dealStage">Новый этап сделки</param>
        private void CheckPossibilityToCloseDeal(Deal deal, DealStage dealStage, User user)
        {
            // Получаем все реализации по сделке
            var sales = dealRepository.Query<SaleWaybill>().Where(x => x.Deal.Id == deal.Id).ToList<SaleWaybill>();

            // проверяем наличие неоплаченных реализаций
            if (sales.Where(x => x.IsFullyPaid == false).Count() != 0)
            {
                throw new Exception("Невозможно закрыть сделку, в которой имеются неоплаченные накладные реализации.");
            }

            ValidationUtils.Assert(!dealRepository.IsAnyUndistributedDealPaymentDocument(deal.Id),
                "Невозможно закрыть сделку, в которой имеются неразнесенные оплаты или корректировки сальдо.");

            // проверяем наличие неотгруженных накладных реализации
            var expenditureWaybills = sales.Where(x => x.Is<ExpenditureWaybill>()).ToList().ConvertAll(x => x.As<ExpenditureWaybill>());
            if (expenditureWaybills.Where(x => x.State != ExpenditureWaybillState.ShippedBySender).Count() != 0)
            {
                throw new Exception("Невозможно закрыть сделку, в которой имеются неотгруженные накладные реализации.");
            }

            // проверяем наличие непринятых возвратов
            var unreceiptedReturnFromClientCount = dealRepository.Query<ReturnFromClientWaybill>()
                .Where(x => x.Deal.Id == deal.Id && x.State != ReturnFromClientWaybillState.Receipted)
                .Count();
            ValidationUtils.Assert(unreceiptedReturnFromClientCount == 0, "Невозможно закрыть сделку, в которой имеются непринятые возвраты.");

            ValidationUtils.Assert(CalculateMainIndicators(deal, calculateBalance: true).Balance == 0M,
                "Невозможно закрыть сделку, сальдо по которой отлично от нуля.");

            // Проверка задач
            var taskCount = taskRepository.GetOpenTaskCountForDeal(deal.Id);
            ValidationUtils.Assert(taskCount == 0, "Невозможно закрыть сделку, в которой имеются незакрытые мероприятия и задачи.");
        }

        #endregion

        #region Расчет показателей для главных деталей

        /// <summary>
        /// Расчет показателей для сделки
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="calculateSaleSum">Рассчитывать сумму реализации</param>
        /// <param name="calculateShippingPendingSaleSum">Рассчитывать сумму реализации по неотгруженным накладным</param>
        /// <param name="calculateBalance">Рассчитывать сальдо</param>
        /// <param name="calculatePaymentDelayPeriod">Рассчитывать период просрочки</param>
        /// <param name="calculatePaymentDelaySum">Рассчитывать сумму просрочки</param>
        /// <param name="calculateReturnedFromClientSum">Рассчитывать сумму принятых возвратов</param>
        /// <param name="calculateReservedByReturnFromClientSum">Рассчитывать сумму оформленных возвратов</param>    
        /// <param name="calculateCorrectedInitialBalance">Рассчитывать сумму корректировок сальдо</param>
        public DealMainIndicators CalculateMainIndicators(Deal deal, bool calculateSaleSum = false, bool calculateShippingPendingSaleSum = false,
            bool calculateBalance = false, bool calculatePaymentDelayPeriod = false,
            bool calculatePaymentDelaySum = false, bool calculateReturnedFromClientSum = false, bool calculateReservedByReturnFromClientSum = false,
            bool calculateInitialBalance = false)
        {
            return dealIndicatorService.CalculateMainIndicators(deal, null ,calculateSaleSum, calculateShippingPendingSaleSum,
                calculateBalance, calculatePaymentDelayPeriod, calculatePaymentDelaySum, calculateReturnedFromClientSum,
                calculateReservedByReturnFromClientSum, calculateInitialBalance);
        }

        /// <summary>
        /// Расчет остатка кредитного лимита по отгруженным накладным реализации для данной сделки.
        /// Накладная реализации должна иметь квоту с постоплатой (т.е. с отсрочкой платежа).
        /// Если же накладная реализации имеет квоту с предоплатой, currentCreditLimitSum будет равно null.
        /// Если накладная реализации имеет квоту с безлимитным кредитом, currentCreditLimitSum будет равно 0.
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="currentCreditLimitSum">Текущее значение кредитного лимита (0 - бесконечный кредит)</param>
        /// <returns>Остаток кредитного лимита</returns>
        public List<KeyValuePair<SaleWaybill, decimal>> CalculatePostPaymentShippedCreditLimitRemainder(Deal deal, out List<KeyValuePair<SaleWaybill, decimal?>> currentCreditLimitSum)
        {
            return dealIndicatorService.CalculatePostPaymentShippedCreditLimitRemainder(deal, out currentCreditLimitSum);
        }

        /// <summary>
        /// Расчет суммы реализации
        /// </summary>
        /// <param name="deal">Сделка</param>
        public decimal CalculateSaleSum(Deal deal, User user)
        {
            if (IsPossibilityToViewSales(deal, user))
            {
                return dealIndicatorService.CalculateSaleSum(deal);
            }

            return 0M;
        }

        #endregion

        #region Удаление квоты

        public void RemoveQuota(Deal deal, DealQuota quota, User user)
        {
            CheckPossibilityToRemoveQuota(deal, quota, user);

            deal.RemoveQuota(quota, false);

        }

        #endregion

        #region Добавление квот

        public void AddQuota(Deal deal, DealQuota dealQuota, User user)
        {
            CheckPossibilityToAddQuota(deal, user);

            deal.AddQuota(dealQuota);
        }

        public void AddQuotas(Deal deal, IEnumerable<DealQuota> dealQuotas, User user)
        {
            CheckPossibilityToAddQuota(deal, user);

            foreach (var quota in dealQuotas.Except(deal.Quotas))
            {
                deal.AddQuota(quota);
            }
        }

        #endregion

        #region Получение максимальной возможной оплаты наличными средствами

        /// <summary>
        /// Получение максимально возможной оплаты наличными средствами по сделке
        /// </summary>
        /// <param name="deal">Сделка (null - получить максимально возможную сумму оплаты по любой сделке, из настроек)</param>
        /// <returns></returns>
        public decimal GetMaxPossibleCashPaymentSum(Deal deal)
        {
            var cashPaymentSumRemainder = AppSettings.MaxCashPaymentSum - (deal != null ? deal.CashDealPaymentSum : 0M);

            return cashPaymentSumRemainder > 0 ? cashPaymentSumRemainder : 0;
        }

        #endregion

        #region Проверки на возможность выполнения операций

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(Deal deal, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                    result = deal.Curator == user && user.Teams.SelectMany(x => x.Deals).Contains(deal);
                    break;

                case PermissionDistributionType.Teams:
                    result = user.Teams.SelectMany(x => x.Deals).Contains(deal);
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(Deal deal, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(deal, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #endregion

        #region Редактирование

        public bool IsPossibilityToEdit(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToEdit(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEdit(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.Deal_Create_Edit);

            // сущность
            deal.CheckPossibilityToEdit();
        }

        #endregion

        #region Смена этапа

        #region Возможность сменить этап (проверяется только право, для появления модальной формы смены этапа)

        /// <summary>
        /// Возможность сменить этап (проверяется только право, для появления модальной формы смены этапа)
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="user">Пользователь</param>
        public bool IsPossibilityToChangeStage(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToChangeStage(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Возможность сменить этап (проверяется только право, для появления модальной формы смены этапа)
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="user">Пользователь</param>
        public void CheckPossibilityToChangeStage(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.Deal_Stage_Change);
        }

        #endregion

        #region Переход на следующий этап

        public bool IsPossibilityToMoveToNextStage(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToMoveToNextStage(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToMoveToNextStage(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.Deal_Stage_Change);

            // сущность
            deal.CheckPossibilityToMoveToNextStage();

            // запросы к БД
            switch (deal.NextStage.Value)
            {
                // 7.1
                case DealStage.SuccessfullyClosed:
                    CheckPossibilityToCloseDeal(deal, deal.NextStage.Value, user);
                    break;
            }
        }

        #endregion

        #region Переход на предыдущий этап

        public bool IsPossibilityToMoveToPreviousStage(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToMoveToPreviousStage(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToMoveToPreviousStage(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.Deal_Stage_Change);

            // сущность
            deal.CheckPossibilityToMoveToPreviousStage();

            // запросы к БД
            switch (deal.Stage)
            {
                // 5
                case DealStage.ContractExecution:
                    // В предпродажное состояние можно перейти, только если нет ни одной продажной сущности: оплат, накладных реализации
                    var expenditureWaybillCount = dealRepository.Query<ExpenditureWaybill>().Where(x => x.Deal.Id == deal.Id).Count();
                    ValidationUtils.Assert(expenditureWaybillCount == 0,
                        String.Format("Невозможно перейти на этап «{0}», так как по сделке существуют накладные реализации товаров.", deal.PreviousStage.GetDisplayName()));

                    var returnFromClientWaybillCount = dealRepository.Query<ReturnFromClientWaybill>().Where(x => x.Deal.Id == deal.Id).Count();
                    ValidationUtils.Assert(returnFromClientWaybillCount == 0,
                        String.Format("Невозможно перейти на этап «{0}», так как по сделке существуют накладные возврата товара.", deal.PreviousStage.GetDisplayName()));

                    ValidationUtils.Assert(!deal.DealPaymentDocuments.Any(),
                        String.Format("Невозможно перейти на этап «{0}», так как по сделке существуют оплаты или корректировки сальдо.", deal.PreviousStage.GetDisplayName()));
                    break;
            }
        }

        #endregion

        #region Переход на этап "Неуспешное закрытие"

        public bool IsPossibilityToMoveToUnsuccessfulClosingStage(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToMoveToUnsuccessfulClosingStage(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToMoveToUnsuccessfulClosingStage(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.Deal_Stage_Change);

            // сущность
            deal.CheckPossibilityToMoveToUnsuccessfulClosingStage();

            // запросы к БД
            switch (deal.UnsuccessfulClosingStage.Value)
            {
                // 7.2
                case DealStage.ContractAbrogated:
                    CheckPossibilityToCloseDeal(deal, deal.UnsuccessfulClosingStage.Value, user);
                    break;
            }
        }

        #endregion

        #region Переход на этап "Поиск принимающего решения"

        public bool IsPossibilityToMoveToDecisionMakerSearchStage(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToMoveToDecisionMakerSearchStage(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToMoveToDecisionMakerSearchStage(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.Deal_Stage_Change);

            // сущность
            deal.CheckPossibilityToMoveToDecisionMakerSearchStage();
        }

        #endregion

        #endregion

        #region Создание договора из сделки

        public bool IsPossibilityToCreateContractFromDeal(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToCreateContractFromDeal(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateContractFromDeal(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.ClientContract_Create);
        }

        #endregion

        #region Добавление договора

        public bool IsPossibilityToAddContract(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToAddContract(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual void CheckPossibilityToAddContract(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.Deal_Contract_Set);

            // сущность
            deal.CheckPossibilityToAddContract();
        }

        #endregion

        #region Редактирование договора

        public bool IsPossibilityToChangeContract(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToChangeContract(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual void CheckPossibilityToChangeContract(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.Deal_Contract_Set);

            // сущность
            deal.CheckPossibilityToChangeContract();
        }

        /// <summary>
        /// Можно ли изменять организации, указанные в договоре
        /// </summary>
        /// <param name="deal"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsPossibilityToEditOrganization(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToEditOrganization(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditOrganization(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.Deal_Contract_Set);

            // сущность
            deal.CheckPossibilityToEditOrganization();

            ValidationUtils.Assert(!dealRepository.IsAnySale(deal.Id), "Невозможно редактировать организации в договоре сделки, по которой уже есть накладные реализации.");
        }

        #endregion

        #region Установка договора (добавление или изменение)

        public bool IsPossibilityToSetContract(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToSetContract(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual void CheckPossibilityToSetContract(Deal deal, User user)
        {
            if (deal.Contract == null)
            {
                CheckPossibilityToAddContract(deal, user);
            }
            else
            {
                CheckPossibilityToChangeContract(deal, user);
            }
        }

        #endregion

        #region Просмотр деталей

        public bool IsPossibilityToViewDetails(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToViewDetails(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewDetails(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.Deal_List_Details);
        }

        #endregion

        #region Просмотр списка и суммы оплат

        public bool IsPossibilityToViewDealPayments(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToViewPayments(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewPayments(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.DealPayment_List_Details);
        }

        #endregion

        #region Просмотр списка квот

        public bool IsPossibilityToViewDealQuotaList(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToViewDealQuotaList(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewDealQuotaList(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.Deal_Quota_List);
        }

        #endregion

        #region Добавление квоты

        public bool IsPossibilityToAddQuota(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToAddQuota(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAddQuota(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.Deal_Quota_Add);

            // сущность
            deal.CheckPossibilityToAddQuota();
        }

        #endregion

        #region Удаление квоты

        public bool IsPossibilityToRemoveQuota(Deal deal, DealQuota quota, User user)
        {
            try
            {
                CheckPossibilityToRemoveQuota(deal, quota, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToRemoveQuota(Deal deal, DealQuota quota, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.Deal_Quota_Remove);

            // сущность
            deal.CheckPossibilityToRemoveQuota(quota);
        }

        #endregion

        #region Просмотр списка и суммы реализации

        public bool IsPossibilityToViewSales(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToViewSales(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewSales(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.ExpenditureWaybill_List_Details);
        }

        #endregion

        #region Добавление накладной реализации товаров

        public bool IsPossibilityToCreateExpenditureWaybill(Deal deal, User user, bool checkLogic = true)
        {
            try
            {
                CheckPossibilityToCreateExpenditureWaybill(deal, user, checkLogic);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateExpenditureWaybill(Deal deal, User user, bool checkLogic = true)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.ExpenditureWaybill_Create_Edit);

            if (checkLogic)
            {
                // сущность
                deal.CheckPossibilityToCreateExpenditureWaybill();
            }
        }

        #endregion

        #region Просмотр списка и суммы возвратов

        public bool IsPossibilityToViewReturnsFromClient(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToViewReturnsFromClient(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewReturnsFromClient(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.ReturnFromClientWaybill_List_Details);
        }

        #endregion

        #region Добавление накладной возврата от клиента

        public bool IsPossibilityToCreateReturnFromClientWaybill(Deal deal, User user, bool checkLogic = true)
        {
            try
            {
                CheckPossibilityToCreateReturnFromClientWaybill(deal, user, checkLogic);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateReturnFromClientWaybill(Deal deal, User user, bool checkLogic = true)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.ReturnFromClientWaybill_Create_Edit);

            if (checkLogic)
            {
                // сущность
                deal.CheckPossibilityToCreateReturnFromClientWaybill();
            }
        }

        #endregion

        #region Просмотр суммы общего сальдо

        public bool IsPossibilityToViewBalance(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToViewBalance(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewBalance(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.Deal_Balance_View);
        }

        #endregion

        #endregion

        #region Получение связанных данных

        /// <summary>
        /// Получение списка всех мест хранения, относящихся к данной сделке с учетом прав
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Storage> GetStorageList(Deal deal, User user)
        {
            // получаем список доступных мест хранения на основании прав
            var storageList = storageService.GetList(user, Permission.ExpenditureWaybill_Create_Edit);

            // оставляем только те места хранения, которые связаны с собственной организацией и договора по сделке
            storageList = storageList.Intersect(deal.Contract.AccountOrganization.Storages);

            return storageList.OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name);
        }

        #endregion

        #region Получение списика доступных для выбора команд

        /// <summary>
        /// Получение перечня команд по реализациям сделки (без учета их видимости)
        /// </summary>
        /// <param name="deal">Сделка, по реализациям которой будет сформирован список команд</param>
        /// <returns>Список команд</returns>
        public IEnumerable<Team> GetTeamListFromSales(Deal deal)
        {
            return dealRepository.GetTeamListFromSales(deal.Id);
        }

        /// <summary>
        /// Получение списка команд для документов сделки
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Список команд</returns>
        public IEnumerable<Team> GetTeamListForDealDocumentByDeal(Deal deal, User user)
        {
            return dealRepository.GetTeamListForDealDocumentByDeal(deal.Id, user.Id);
        }

        /// <summary>
        /// Получение списка команд для документа сделки (команды пользователя + команды, что могут видеть сделку)
        /// </summary>
        /// <param name="clientOrganization">Организация клиента</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Список команд</returns>
        public IEnumerable<Team> GetTeamListForDealDocumentByClientOrganization(ClientOrganization clientOrganization, User user)
        {
            return dealRepository.GetTeamListForDealDocumentByClientOrganization(clientOrganization.Id, user.Id);
        }

        #endregion

        #endregion
    }
}