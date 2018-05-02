using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class DealIndicatorService : IDealIndicatorService
    {
        #region Поля

        private readonly IDealRepository dealRepository;
        private readonly IClientRepository clientRepository;
        private readonly IClientContractRepository clientContractRepository;
        private readonly IAccountOrganizationRepository accountOrganizationRepository;
        private readonly IClientOrganizationRepository clientOrganizationRepository;
        private readonly IEconomicAgentRepository economicAgentRepository;
        private readonly IExpenditureWaybillRepository expenditureWaybillRepository;
        private readonly IReturnFromClientWaybillRepository returnFromClientWaybillRepository;
        private readonly IDealPaymentDocumentRepository dealPaymentDocumentRepository;
        private readonly ITeamRepository teamRepository;

        private readonly ISaleWaybillIndicatorService saleWaybillIndicatorService;
        private readonly IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService;
        private readonly IAcceptedSaleIndicatorService acceptedSaleIndicatorService;
        private readonly IPermissionDistributionService permissionDistributionService;

        #endregion

        #region Конструктор

        public DealIndicatorService(IDealRepository dealRepository, IExpenditureWaybillRepository expenditureWaybillRepository,
            IReturnFromClientWaybillRepository returnFromClientWaybillRepository, IDealPaymentDocumentRepository dealPaymentDocumentRepository,
            IClientRepository clientRepository, IClientContractRepository clientContractRepository, IAccountOrganizationRepository accountOrganizationRepository,
            IClientOrganizationRepository clientOrganizationRepository, IEconomicAgentRepository economicAgentRepository, ISaleWaybillIndicatorService saleWaybillIndicatorService,
            IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService, IAcceptedSaleIndicatorService acceptedSaleIndicatorService,
            IPermissionDistributionService permissionDistributionService, ITeamRepository teamRepository)
        {
            this.dealRepository = dealRepository;
            this.expenditureWaybillRepository = expenditureWaybillRepository;
            this.clientRepository = clientRepository;
            this.clientContractRepository = clientContractRepository;
            this.accountOrganizationRepository = accountOrganizationRepository;
            this.clientOrganizationRepository = clientOrganizationRepository;
            this.economicAgentRepository = economicAgentRepository;
            this.returnFromClientWaybillRepository = returnFromClientWaybillRepository;
            this.dealPaymentDocumentRepository = dealPaymentDocumentRepository;
            this.teamRepository = teamRepository;

            this.saleWaybillIndicatorService = saleWaybillIndicatorService;
            this.expenditureWaybillIndicatorService = expenditureWaybillIndicatorService;
            this.acceptedSaleIndicatorService = acceptedSaleIndicatorService;
            this.permissionDistributionService = permissionDistributionService;
        }
        
        #endregion

        #region Методы

        #region Расчет показателей для разнесения оплат и блокировок
        
        /// <summary>
        /// Расчет остатка кредитного лимита по всем постоплатным накладным реализации для данной сделки.
        /// Накладная реализации должна иметь квоту с постоплатой (т.е. с отсрочкой платежа).
        /// Если же накладная реализации имеет квоту с предоплатой, currentCreditLimitSum будет равно null.
        /// Если квота накладной реализации имеет безлимитный кредит, currentCreditLimitSum будет равно 0.
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="currentCreditLimitSum">Текущее значение кредитного лимита (0 - безлимитный кредит)</param>
        /// <returns>Остаток кредитного лимита</returns>
        public decimal CalculateCreditLimitRemainder(Deal deal, out decimal? currentCreditLimitSum, SaleWaybill transientSaleWaybill)
        {
            var quota = transientSaleWaybill.Quota;
            if (quota.IsPrepayment)
            {
                currentCreditLimitSum = null;

                return 0;
            }

            currentCreditLimitSum = quota.CreditLimitSum;
            if (currentCreditLimitSum == 0) // безлимитный кредит
            {
                return 0;
            }

            decimal totalDebtRemainder = 0;

            Guid saleWaybillToExcludeId = transientSaleWaybill != null ? transientSaleWaybill.Id : Guid.Empty;

            var sales = dealRepository.Query<SaleWaybill>()
                .Where(x => x.Deal.Id == deal.Id && x.IsPrepayment == false && x.IsFullyPaid == false && x.Id != saleWaybillToExcludeId && x.AcceptanceDate != null)
                .ToList<SaleWaybill>();
            
            foreach (SaleWaybill saleWaybill in sales)
            {
                totalDebtRemainder += saleWaybillIndicatorService.CalculateDebtRemainder(saleWaybill);
            }

            if (transientSaleWaybill != null)
            {
                totalDebtRemainder += saleWaybillIndicatorService.CalculateDebtRemainder(transientSaleWaybill);
            }

            //К кредитному лимиту прибавляем неразнесенные оплаты и вычитаем нераспределенное начальное сальдо клиента.
            var maxSaleSum = currentCreditLimitSum.Value + deal.UndistributedDealPaymentFromClientSum - deal.UnpaidDebtToInitialBalance;

            return maxSaleSum - totalDebtRemainder; 
        }

        /// <summary>
        /// Расчет остатка кредитного лимита по отгруженным постоплатным накладным реализации для данной сделки.
        /// Накладная реализации должна иметь квоту с постоплатой (т.е. с отсрочкой платежа).
        /// Если же накладная реализации имеет квоту с предоплатой, currentCreditLimitSum будет равно null.
        /// Если накладная реализации имеет квоту с безлимитным кредитом, currentCreditLimitSum будет равно 0.
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="currentCreditLimitSum">Текущее значение кредитного лимита (0 - бесконечный кредит)</param>
        /// <returns>Остаток кредитного лимита</returns>
        public List<KeyValuePair<SaleWaybill, decimal>> CalculatePostPaymentShippedCreditLimitRemainder(Deal deal, out List<KeyValuePair<SaleWaybill, decimal?>> currentCreditLimitSumList)
        {
            var creditLimitRemainderList = new List<KeyValuePair<SaleWaybill, decimal>>();
            currentCreditLimitSumList = new List<KeyValuePair<SaleWaybill, decimal?>>();

            var sales = dealRepository.Query<SaleWaybill>()
                .Where(x => x.Deal.Id == deal.Id && x.IsPrepayment == false && x.IsFullyPaid == false && x.AcceptanceDate != null).ToList<SaleWaybill>();

            decimal totalShippedDebtRemainder = 0;
            foreach (SaleWaybill saleWaybill in sales)
            {
                if (saleWaybill.IsShipped)
                {
                    totalShippedDebtRemainder += saleWaybillIndicatorService.CalculateDebtRemainder(saleWaybill);
                }
            }

            decimal? currentCreditLimitSum;

            foreach (SaleWaybill sale in sales)
            {
                if (sale.Quota.IsPrepayment)
                {
                    currentCreditLimitSumList.Add(new KeyValuePair<SaleWaybill, decimal?>(sale, null));
                    creditLimitRemainderList.Add(new KeyValuePair<SaleWaybill, decimal>(sale, 0));
                    continue;
                }

                currentCreditLimitSum = sale.Quota.CreditLimitSum;
                currentCreditLimitSumList.Add(new KeyValuePair<SaleWaybill, decimal?>(sale, currentCreditLimitSum));
                if (currentCreditLimitSum == 0) // безлимитный кредит                 
                {
                    creditLimitRemainderList.Add(new KeyValuePair<SaleWaybill, decimal>(sale, 0));
                    continue;
                }

                creditLimitRemainderList.Add(new KeyValuePair<SaleWaybill, decimal>(sale, sale.Quota.CreditLimitSum.Value - totalShippedDebtRemainder));
            }

            return creditLimitRemainderList;
        }
        
        #endregion

        #region Расчет показателей для сервиса сделок уровня приложения

        /// <summary>
        /// Расчет основных показателей
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="team">Команда, по которой рассчитать показатели. null - рассчитать по всем командам.</param>
        /// <param name="calculateSaleSum">Рассчитывать сумму реализации</param>
        /// <param name="calculateShippingPendingSaleSum">Рассчитывать сумму реализации по неотгруженным накладным</param>
        /// <param name="calculateBalance">Рассчитывать сальдо</param>
        /// <param name="calculatePaymentDelayPeriod">Рассчитывать период просрочки</param>
        /// <param name="calculatePaymentDelaySum">Рассчитывать сумму просрочки</param>
        /// <param name="calculateReturnedFromClientSum">Рассчитывать сумму принятых возвратов</param>
        /// <param name="calculateReservedByReturnFromClientSum">Рассчитывать сумму оформленных возвратов</param>
        /// <param name="calculateInitialBalance">Рассчитывать сумму корректировок сальдо</param>
        public DealMainIndicators CalculateMainIndicators(Deal deal, Team team = null, bool calculateSaleSum = false, bool calculateShippingPendingSaleSum = false,
            bool calculateBalance = false, bool calculatePaymentDelayPeriod = false,
            bool calculatePaymentDelaySum = false, bool calculateReturnedFromClientSum = false, bool calculateReservedByReturnFromClientSum = false,
            bool calculateInitialBalance = false)
        {
            var result = new DealMainIndicators();

            decimal saleSum = 0M, shippingPendingSaleSum = 0M, paymentDelaySum = 0M, returnedFromClientSum = 0M, reservedByReturnFromClientSum = 0M;
            int paymentDelayPeriod = 0;

            var sales = dealRepository.Query<SaleWaybill>()
                .Where(x => x.Deal.Id == deal.Id && x.AcceptanceDate != null)
                .Where(x => team == null || x.Team == team)
                .ToList<SaleWaybill>();

            foreach (SaleWaybill saleWaybill in sales)
            {
                if (saleWaybill.Is<ExpenditureWaybill>())
                {
                    var expenditureWaybill = saleWaybill.As<ExpenditureWaybill>();

                    if (calculateSaleSum || calculateShippingPendingSaleSum || calculateBalance)
                    {
                        decimal waybillSaleSum = expenditureWaybillIndicatorService.CalculateSalePriceSum(expenditureWaybill);

                        saleSum += waybillSaleSum;
                        if (!expenditureWaybill.IsShipped)
                        {
                            shippingPendingSaleSum += waybillSaleSum;
                        }
                    }

                    if (calculatePaymentDelayPeriod || calculatePaymentDelaySum)
                    {
                        int currentPaymentDelay = expenditureWaybill.PaymentDelay;
                        if (currentPaymentDelay > 0)
                        {
                            paymentDelayPeriod = Math.Max(currentPaymentDelay, paymentDelayPeriod);

                            if (calculatePaymentDelaySum)
                            {
                                paymentDelaySum += saleWaybillIndicatorService.CalculateDebtRemainder(expenditureWaybill);
                            }
                        }
                    }

                    if (calculateReturnedFromClientSum || calculateBalance)
                    {
                        returnedFromClientSum += saleWaybillIndicatorService.GetTotalReturnedSumForSaleWaybill(expenditureWaybill);                        
                    }

                    if (calculateReservedByReturnFromClientSum)
                    {
                        reservedByReturnFromClientSum += saleWaybillIndicatorService.GetTotalReservedByReturnSumForSaleWaybill(expenditureWaybill);
                    }
                }
            }

            if (calculateSaleSum) { result.SaleSum = saleSum; }

            if (calculateShippingPendingSaleSum) { result.ShippingPendingSaleSum = shippingPendingSaleSum; }

            if (calculatePaymentDelaySum) { result.PaymentDelaySum = paymentDelaySum; }

            if (calculatePaymentDelayPeriod) { result.PaymentDelayPeriod = paymentDelayPeriod; }

            if (calculateReturnedFromClientSum) { result.ReturnedFromClientSum = returnedFromClientSum; }

            if (calculateReservedByReturnFromClientSum) { result.ReservedByReturnFromClientSum = reservedByReturnFromClientSum; }

            if (calculateBalance)
            {
                if (team == null)
                {
                    result.Balance = saleSum - returnedFromClientSum - deal.DealPaymentSum + deal.InitialBalance;
                }
                else
                {
                    result.Balance = saleSum - returnedFromClientSum - deal.DealPaymentSumForTeam(team) + deal.InitialBalanceForTeam(team);
                }
            }

            if (calculateInitialBalance)
            {
                result.InitialBalance = deal.InitialBalance;
            }

            return result;
        }

        /// <summary>
        /// Расчет суммы реализации
        /// </summary>
        /// <param name="deal">Сделка</param>
        public decimal CalculateSaleSum(Deal deal)
        {
            return CalculateMainIndicators(deal, calculateSaleSum: true).SaleSum;
        }

        /// <summary>
        /// Расчет текущего периода просрочки
        /// </summary>
        /// <param name="deal">Сделка</param>
        public int CalculatePaymentDelayPeriod(Deal deal)
        {
            return CalculateMainIndicators(deal, calculatePaymentDelayPeriod: true).PaymentDelayPeriod;            
        }

        #endregion

        #region Получение подкритерия сделок, видимых юзером для конкретного права

        /// <summary>
        /// Получение подкритерия по сделке, ограничивающего ее видимостью данного юзера для данного права
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="permission">Право, для которого берутся сделки</param>
        /// <returns>null, если пользователь не имеет права; иначе подкритерий возможных сделок</returns>
        public ISubCriteria<Deal> RestrictByUserPermissions(User user, Permission permission)
        {
            return RestrictByUserPermissions(user, user.GetPermissionDistributionType(permission), permission.GetDisplayName());
        }

        /// <summary>
        /// Получение подкритерия по сделке, ограничивающего ее видимостью данного юзера для данного права
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="type">Распространение права</param>
        /// <param name="permissionDisplayName">Наименование права</param>
        /// <returns>null, если пользователь не имеет права; иначе подкритерий возможных сделок</returns>
        private ISubCriteria<Deal> RestrictByUserPermissions(User user, PermissionDistributionType type, string permissionDisplayName ="")
        {
            switch (type)
            {
                case PermissionDistributionType.None:
                    return null;

                case PermissionDistributionType.Personal:
                    return dealRepository.GetSubQueryForDealIdOnPersonalPermission(user.Id);

                case PermissionDistributionType.Teams:
                    return dealRepository.GetSubQueryForDealIdOnTeamPermission(user.Id);

                case PermissionDistributionType.All:
                    return dealRepository.GetSubQueryForDealIdOnAllPermission();

                default:
                    throw new Exception(String.Format("Неизвестное распространение права «{0}».", permissionDisplayName));
            }
        }

        #endregion

        #region Проверка, нет ли невидимых для пользователя сделок по клиенту или организации клиента

        /// <summary>
        /// Проверка, есть ли невидимые для пользователя сделки по клиенту
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        public bool AreAnyRestrictedDeals(Client client, IEnumerable<Permission> permissionList, User user)
        {
            // Подсчитываем количества сделок, видимых пользователю с правом "все", "только командные" и "только свои"
            var allDealCount = dealRepository.GetDealCountOnAllPermissionByClient(user.Id, client.Id);
            var teamDealCount = dealRepository.GetDealCountOnTeamPermissionByClient(user.Id, client.Id);
            var personalDealCount = dealRepository.GetDealCountOnPersonalPermissionByClient(user.Id, client.Id);

            return AreAnyRestrictedDeals(allDealCount, teamDealCount, personalDealCount, permissionList, user);
        }

        /// <summary>
        /// Проверка, есть ли невидимые для пользователя сделки по организации клиента
        /// </summary>
        /// <param name="clientOrganization"></param>
        /// <param name="user"></param>
        public bool AreAnyRestrictedDeals(ClientOrganization clientOrganization, IEnumerable<Permission> permissionList, User user)
        {
            // Подсчитываем количества сделок, видимых пользователю с правом "все", "только командные" и "только свои"
            var allDealCount = dealRepository.GetDealCountOnAllPermissionByClientOrganization(user.Id, clientOrganization.Id);
            var teamDealCount = dealRepository.GetDealCountOnTeamPermissionByClientOrganization(user.Id, clientOrganization.Id);
            var personalDealCount = dealRepository.GetDealCountOnPersonalPermissionByClientOrganization(user.Id, clientOrganization.Id);

            return AreAnyRestrictedDeals(allDealCount, teamDealCount, personalDealCount, permissionList, user);
        }

        /// <summary>
        /// Проверка, есть ли невидимые для пользователя сделки по списку прав
        /// </summary>
        /// <param name="allDealCount">Количество сделок для области видимости "Все"</param>
        /// <param name="teamDealCount">Количество сделок для области видимости "Только командные"</param>
        /// <param name="personalDealCount">Количество сделок для области видимости "Только свои"</param>
        /// <param name="permissionList">Список прав</param>
        /// <param name="user">Пользователь</param>
        /// <returns>true, если невидимые сделки существуют или пользователь имеет хоть по одному праву распространение "Нет"</returns>
        private bool AreAnyRestrictedDeals(int allDealCount, int teamDealCount, int personalDealCount, IEnumerable<Permission> permissionList, User user)
        {
            // В зависимости от минимального распространения проверяем соответствующие количества сделок
            switch (permissionDistributionService.GetMinPermission(permissionList, user))
            {
                case PermissionDistributionType.None: // Если распространение "Нет", генерируем ошибку
                    return true;
                case PermissionDistributionType.Personal:
                    return personalDealCount < allDealCount || teamDealCount < allDealCount;
                case PermissionDistributionType.Teams:
                    return teamDealCount < allDealCount;
                case PermissionDistributionType.All:
                    return false;
                default:
                    throw new Exception("Неизвестное распространение права.");
            };
        }

        #endregion

        #region Расчет сальдо списком

        /// <summary>
        /// Рассчитать список балансов по сделкам, принадлежащим данному клиенту
        /// (для печатной формы - все сделки, все виды документов)
        /// </summary>
        public IList<InitialBalanceInfo> CalculateBalanceOnDateByClient(Client client, DateTime date, User user)
        {
            return CalculateBalanceOnDate(null, new List<int> { client.Id }, null, date, user, usePermission: false);
        }

        /// <summary>
        /// Рассчитать список балансов, принадлежащим данным клиентам и командам
        /// (для отчета - видимые по подкритерию сделки)
        /// </summary>
        /// <param name="clientIdList">Список кодов клиентов. Null - все клиенты</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        public IList<InitialBalanceInfo> CalculateBalanceOnDateByClientAndTeamList(IList<int> clientIdList, IEnumerable<short> teamIdList, DateTime date,
            User user, bool includeExpenditureWaybillsAndReturnFromClientWaybills, bool includeDealPayments, bool includeDealInitialBalanceCorrections)
        {
            return CalculateBalanceOnDate(teamIdList, clientIdList, null, date, user, includeExpenditureWaybillsAndReturnFromClientWaybills,
                includeDealPayments, includeDealInitialBalanceCorrections);
        }

        /// <summary>
        /// Рассчитать список балансов по сделкам, принадлежащим данной организации клиента, сгруппированный по сделкам, на дату
        /// (для печатной формы - все сделки, все виды документов)
        /// </summary>
        public IList<InitialBalanceInfo> CalculateBalanceOnDateByClientOrganization(ClientOrganization clientOrganization, DateTime date, User user)
        {
            return CalculateBalanceOnDate(null, null, new List<int> { clientOrganization.Id }, date, user, usePermission: false);
        }

        /// <summary>
        /// Рассчитать список балансов по сделкам, принадлежащим данным организациям клиента и командам, сгруппированный по сделкам, на дату
        /// (для отчета - видимые по подкритерию сделки)
        /// </summary>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиента. Null - все организации клиента</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <returns>Список {AccountOrganizationId, ClientId, ClientOrganizationId, ContractId, TeamId, Сумма реализаций}</returns>
        public IList<InitialBalanceInfo> CalculateBalanceOnDateByClientOrganizationAndTeamList(IList<int> clientOrganizationIdList, IEnumerable<short> teamIdList, DateTime date,
            User user, bool includeExpenditureWaybillsAndReturnFromClientWaybills, bool includeDealPayments, bool includeDealInitialBalanceCorrections)
        {
            return CalculateBalanceOnDate(teamIdList, null, clientOrganizationIdList, date, user, includeExpenditureWaybillsAndReturnFromClientWaybills,
                includeDealPayments, includeDealInitialBalanceCorrections);
        }

        /// <summary>
        /// Рассчитать список сальдо по документам сделки
        /// </summary>
        /// <param name="teamIdList">Коллекция кодов команд, для которых нужно рассчитать сальдо</param>
        /// <param name="startDate">Начальная дата, на которую надо рассчитать сальдо</param>
        /// <param name="user">Пользователь, видимостью которого ограничивать множества документов. Null - не ограничивать видимостью</param>
        /// <param name="includeExpenditureWaybillsAndReturnFromClientWaybills">Включать ли реализации и возвраты</param>
        /// <param name="includeDealPayments">Включать ли оплаты и возвраты оплат</param>
        /// <param name="includeDealInitialBalanceCorrections">Включать ли корректировки сальдо</param>
        /// <param name="usePermission">Учитывать ли видимость сущностей</param>
        ///<returns>Список {AccountOrganizationId, ClientId, ClientOrganizationId, ContractId, TeamId, Сумма реализаций}</returns>
        private IList<InitialBalanceInfo> CalculateBalanceOnDate(IEnumerable<short> teamIdList, IEnumerable<int> clientIdList,
            IEnumerable<int> clientOrganizationIdList, DateTime startDate, User user, bool includeExpenditureWaybillsAndReturnFromClientWaybills = true,
            bool includeDealPayments = true, bool includeDealInitialBalanceCorrections = true, bool usePermission = true)
        {
            var result = new List<InitialBalanceInfo>();

            #region Суммы по сущностям

            IQueryable<Team> teamSubQuery = null;
            switch (user.GetPermissionDistributionType(Permission.Team_List_Details))
            {
                case PermissionDistributionType.All:
                    teamSubQuery = teamRepository.GetTeamListByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                case PermissionDistributionType.Personal:
                    teamSubQuery = teamRepository.GetTeamListByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    teamSubQuery = teamRepository.GetTeamListByNonePermission();
                    break;
            }


            if (includeExpenditureWaybillsAndReturnFromClientWaybills)
            {
                var expenditureWaybillSubQuery = usePermission ?
                    GetDealSubQueryByPermissionDistribution(
                    expenditureWaybillRepository.GetExpenditureWaybillByAllPermission(),
                    expenditureWaybillRepository.GetExpenditureWaybillByTeamPermission(user.Id),
                    expenditureWaybillRepository.GetExpenditureWaybillByPersonalPermission(user.Id),
                    Permission.ExpenditureWaybill_List_Details, user) :
                    expenditureWaybillRepository.GetExpenditureWaybillByAllPermission();

                if (expenditureWaybillSubQuery != null)
                {
                    var saleWaybillSum = expenditureWaybillRepository.GetShippedSumOnDate(startDate, expenditureWaybillSubQuery, teamSubQuery, teamIdList, 
                        clientIdList, clientOrganizationIdList);
                    AddValuesToDealDictionary(result, saleWaybillSum, Decimal.One);
                }

                var returnFromClientWaybillDealSubQuery = usePermission ?
                    GetDealSubQueryByPermissionDistribution(
                    returnFromClientWaybillRepository.GetReturnFromClientWaybillByAllPermission(),
                    returnFromClientWaybillRepository.GetReturnFromClientWaybillByTeamPermission(user.Id),
                    returnFromClientWaybillRepository.GetReturnFromClientWaybillByPersonalPermission(user.Id),
                    Permission.ReturnFromClientWaybill_List_Details, user) :
                    returnFromClientWaybillRepository.GetReturnFromClientWaybillByAllPermission();

                if (returnFromClientWaybillDealSubQuery != null)
                {
                    var returnFromClientWaybillSum = returnFromClientWaybillRepository.GetReceiptedSumOnDate(startDate, returnFromClientWaybillDealSubQuery, 
                        teamSubQuery, teamIdList, clientIdList, clientOrganizationIdList);
                    AddValuesToDealDictionary(result, returnFromClientWaybillSum, Decimal.MinusOne);
                }

            }

            if (includeDealPayments)
            {
                var dealPaymentDealSubQuery = usePermission ?
                    GetDealSubQueryByPermissionDistribution(
                    dealPaymentDocumentRepository.GetDealPaymentDocumentByAllPermission(),
                    dealPaymentDocumentRepository.GetDealPaymentDocumentByTeamPermission(user.Id),
                    dealPaymentDocumentRepository.GetDealPaymentDocumentByPersonalPermission(user.Id),
                    Permission.DealPayment_List_Details, user) :
                    dealPaymentDocumentRepository.GetDealPaymentDocumentByAllPermission();

                if (dealPaymentDealSubQuery != null)
                {
                    var dealPaymentFromClientSum = dealPaymentDocumentRepository.GetDealPaymentFromClientSumOnDate(startDate, dealPaymentDealSubQuery, teamSubQuery, 
                        teamIdList, clientIdList, clientOrganizationIdList);
                    AddValuesToDealDictionary(result, dealPaymentFromClientSum, Decimal.MinusOne);

                    var dealPaymentToClientSum = dealPaymentDocumentRepository.GetDealPaymentToClientSumOnDate(startDate, dealPaymentDealSubQuery, teamSubQuery, 
                        teamIdList, clientIdList, clientOrganizationIdList);
                    AddValuesToDealDictionary(result, dealPaymentToClientSum, Decimal.One);
                }
            }

            if (includeDealInitialBalanceCorrections)
            {
                var dealDebitInitialBalanceCorrectionDealSubQuery = usePermission ?
                    GetDealSubQueryByPermissionDistribution(
                    dealPaymentDocumentRepository.GetDealPaymentDocumentByAllPermission(),
                    dealPaymentDocumentRepository.GetDealPaymentDocumentByTeamPermission(user.Id),
                    dealPaymentDocumentRepository.GetDealPaymentDocumentByPersonalPermission(user.Id),
                    Permission.DealInitialBalanceCorrection_List_Details, user) :
                    dealPaymentDocumentRepository.GetDealPaymentDocumentByAllPermission();

                if (dealDebitInitialBalanceCorrectionDealSubQuery != null)
                {
                    var dealDebitInitialBalanceCorrectionSum = dealPaymentDocumentRepository.GetDealDebitInitialBalanceCorrectionSumOnDate(startDate,
                        dealDebitInitialBalanceCorrectionDealSubQuery, teamSubQuery, teamIdList, clientIdList, clientOrganizationIdList);
                    AddValuesToDealDictionary(result, dealDebitInitialBalanceCorrectionSum, Decimal.One);

                    var dealCreditInitialBalanceCorrectionSum = dealPaymentDocumentRepository.GetDealCreditInitialBalanceCorrectionSumOnDate(startDate,
                        dealDebitInitialBalanceCorrectionDealSubQuery, teamSubQuery, teamIdList, clientIdList, clientOrganizationIdList);
                    AddValuesToDealDictionary(result, dealCreditInitialBalanceCorrectionSum, Decimal.MinusOne);
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// Добавить в список суммы из списка с заданным знаком
        /// </summary>
        private void AddValuesToDealDictionary(IList<InitialBalanceInfo> sumList, IList<InitialBalanceInfo> valuesList, decimal sign)
        {
            foreach(var value in valuesList)
            {
                var item = sumList.Where(x => x.AccountOrganizationId == value.AccountOrganizationId && x.ClientId== value.ClientId && 
                    x.ClientOrganizationId== value.ClientOrganizationId&& x.ContractId== value.ContractId&& x.TeamId== value.TeamId)
                    .FirstOrDefault();
                if (item != null)
                {
                    item.Sum += sign * value.Sum;
                }
                else
                {
                    sumList.Add(new InitialBalanceInfo(value.AccountOrganizationId, value.ClientId, value.ClientOrganizationId, value.ContractId, value.TeamId, sign * value.Sum));
                }
            }
        }

        /// <summary>
        /// Получить нужный подзапрос в зависимости от распространения права (null - права нет)
        /// </summary>
        /// <param name="allDealSubQuery">Подкритерий, ограничивающий множество сделок (без учета видимости)</param>
        /// <param name="teamDealSubQuery">Подкритерий, ограничивающий множество сделок (командная видимость)</param>
        /// <param name="personalDealSubQuery">Подкритерий, ограничивающий множество сделок (персональная видимость)</param>
        /// <param name="permission">Право</param>
        /// <param name="user">Пользователь, видимостью которого ограничивать множества документов. Null - не ограничивать видимостью</param>
        public ISubQuery GetDealSubQueryByPermissionDistribution(ISubQuery allDealSubQuery, ISubQuery teamDealSubQuery, ISubQuery personalDealSubQuery,
            Permission permission, User user)
        {
            return GetDealSubQueryByPermissionDistribution(allDealSubQuery, teamDealSubQuery, personalDealSubQuery,
                user != null ? user.GetPermissionDistributionType(permission) : PermissionDistributionType.All, user);
        }

        /// <summary>
        /// Получить нужный подзапрос в зависимости от распространения права (null - права нет)
        /// </summary>
        /// <param name="permission">Право</param>
        /// <param name="user">Пользователь, видимостью которого ограничивать множества документов. Null - не ограничивать видимостью</param>
        public IQueryable<TValue> GetDealSubQueryByPermissionDistribution<TValue>(IQueryable<TValue> allSubQuery, IQueryable<TValue> teamSubQuery, IQueryable<TValue> personalSubQuery,
            Permission permission, User user)
        {
            return GetDealSubQueryByPermissionDistribution(allSubQuery, teamSubQuery, personalSubQuery,
                user != null ? user.GetPermissionDistributionType(permission) : PermissionDistributionType.All, user);
        }

        /// <summary>
        /// Получить нужный подзапрос в зависимости от распространения права (null - права нет)
        /// </summary>
        /// <param name="allDealSubQuery">Подкритерий, ограничивающий множество сделок (без учета видимости)</param>
        /// <param name="teamDealSubQuery">Подкритерий, ограничивающий множество сделок (командная видимость)</param>
        /// <param name="personalDealSubQuery">Подкритерий, ограничивающий множество сделок (персональная видимость)</param>
        /// <param name="permissionDistribution">Распространение права</param>
        /// <param name="user">Пользователь, видимостью которого ограничивать множества документов. Null - не ограничивать видимостью</param>
        private TValue GetDealSubQueryByPermissionDistribution<TValue>(TValue allDealSubQuery, TValue teamDealSubQuery, TValue personalDealSubQuery,
            PermissionDistributionType permissionDistribution, User user) where TValue: class
        {
            // Если пользователь не указан, ограничивать видимость не надо
            if (user == null)
            {
                return allDealSubQuery;
            }

            switch (permissionDistribution)
            {
                case PermissionDistributionType.All:
                    return allDealSubQuery;
                case PermissionDistributionType.Teams:
                    return teamDealSubQuery;
                case PermissionDistributionType.Personal:
                    return personalDealSubQuery;
                case PermissionDistributionType.None:
                    return null;
                default:
                    throw new Exception("Неизвестный тип распространения права.");
            };
        }

        #endregion

        #endregion
    }
}
