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

namespace ERP.Wholesale.ApplicationServices
{
    public class ReturnFromClientWaybillService : BaseWaybillService<ReturnFromClientWaybill>, IReturnFromClientWaybillService
    {
        #region Поля

        private readonly ISettingRepository settingRepository;
        private readonly IReturnFromClientWaybillRepository returnFromClientWaybillRepository;
        private readonly IDealRepository dealRepository;
        private readonly ITeamRepository teamRepository;
        private readonly IStorageRepository storageRepository;
        private readonly IUserRepository userRepository;

        private readonly IArticlePriceService articlePriceService;
        private readonly IAcceptedSaleIndicatorService saleIndicatorService;
        private readonly IReturnFromClientService returnFromClientService;
        private readonly IFactualFinancialArticleMovementService factualFinancialArticleMovementService;
        private readonly IArticleMovementOperationCountService articleMovementOperationCountService;
        private readonly IArticleMovementService articleMovementService;
        private readonly IDealPaymentDocumentDistributionService dealPaymentDocumentDistributionService;
        private readonly IDealIndicatorService dealIndicatorService;
        private readonly IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService;

        private readonly IArticleRevaluationService articleRevaluationService;
        private readonly IArticleAvailabilityService articleAvailabilityService;

        #endregion

        #region Конструкторы

        public ReturnFromClientWaybillService(ISettingRepository settingRepository, IReturnFromClientWaybillRepository returnFromClientWaybillRepository,
            ITeamRepository teamRepository, IDealRepository dealRepository, IStorageRepository storageRepository, IUserRepository userRepository, IArticlePriceService articlePriceService,            
            IAcceptedSaleIndicatorService saleIndicatorService,
            IReturnFromClientService returnFromClientService, IFactualFinancialArticleMovementService factualFinancialArticleMovementService,
            IArticleMovementOperationCountService articleMovementOperationCountService,
            IArticleMovementService articleMovementService, IDealPaymentDocumentDistributionService dealPaymentDocumentDistributionService,
            IDealIndicatorService dealIndicatorService, IArticleRevaluationService articleRevaluationService, IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService,
            IArticleAvailabilityService articleAvailabilityService)
        {
            this.settingRepository = settingRepository;
            this.returnFromClientWaybillRepository = returnFromClientWaybillRepository;
            this.teamRepository = teamRepository;
            this.articlePriceService = articlePriceService;            
            this.saleIndicatorService = saleIndicatorService;
            this.returnFromClientService = returnFromClientService;
            this.factualFinancialArticleMovementService = factualFinancialArticleMovementService;
            this.articleMovementOperationCountService = articleMovementOperationCountService;
            this.articleMovementService = articleMovementService;
            
            this.articleRevaluationService = articleRevaluationService;
            this.dealPaymentDocumentDistributionService = dealPaymentDocumentDistributionService;
            this.dealIndicatorService = dealIndicatorService;
            this.dealRepository = dealRepository;
            this.storageRepository = storageRepository;
            this.userRepository = userRepository;
            this.expenditureWaybillIndicatorService = expenditureWaybillIndicatorService;

            this.articleAvailabilityService = articleAvailabilityService;
        }

        #endregion

        #region Методы

        #region Получение накладной по Id

        /// <summary>
        /// Получение накладной по коду с учетом прав пользователя
        /// </summary>
        /// <param name="id">Код накладной</param>
        /// <param name="user">Пользователь</param>        
        private ReturnFromClientWaybill GetById(Guid id, User user)
        {
            var type = user.GetPermissionDistributionType(Permission.ReturnFromClientWaybill_List_Details);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var waybill = returnFromClientWaybillRepository.GetById(id);

                if (waybill == null) return null;

                bool contains = user.Teams.SelectMany(x => x.Deals).Contains(waybill.Deal);

                if ((type == PermissionDistributionType.Personal && waybill.Deal.Curator == user && contains) || // все равно фильтруем еще и по командам
                    (type == PermissionDistributionType.Teams && contains) ||
                    type == PermissionDistributionType.All)
                {
                    return waybill;
                }

                return null;
            }
        }

        /// <summary>
        /// Получение накладной по id с проверкой ее существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override ReturnFromClientWaybill CheckWaybillExistence(Guid id, User user)
        {
            var returnFromClientWaybill = GetById(id, user);
            ValidationUtils.NotNull(returnFromClientWaybill, "Накладная возврата от клиента не найдена. Возможно, она была удалена.");

            return returnFromClientWaybill;
        }

        public ReturnFromClientWaybillRow CheckWaybillRowExistence(Guid id)
        {
            var returnFromClientWaybillRow = returnFromClientWaybillRepository.GetRowById(id);
            ValidationUtils.NotNull(returnFromClientWaybillRow, "Позиция накладной возврата от клиента не найдена. Возможно, она была удалена.");

            return returnFromClientWaybillRow;
        }

        #endregion

        #region Получение позиций

        /// <summary>
        /// Все возвраты указанного товара в указанные сроки, у которых МХ-отправитель входит в указанный список
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="storageIds"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IEnumerable<ReturnFromClientWaybillRow> GetRows(int articleId, IEnumerable<short> storageIds, DateTime startDate, DateTime endDate, bool finallyMovedWaybillsOnly)
        {
            ISubQuery waybillSubQuery;

            if (finallyMovedWaybillsOnly)
            {
                waybillSubQuery = returnFromClientWaybillRepository.SubQuery<ReturnFromClientWaybill>()
                    .OneOf(x => x.RecipientStorage.Id, storageIds)
                    .Where(x => x.ReceiptDate >= startDate && x.ReceiptDate <= endDate)
                    .Select(x => x.Id);
            }
            else
            {
                waybillSubQuery = returnFromClientWaybillRepository.SubQuery<ReturnFromClientWaybill>()
                    .OneOf(x => x.RecipientStorage.Id, storageIds)
                    .Where(x => x.AcceptanceDate >= startDate && x.AcceptanceDate <= endDate)
                    .Select(x => x.Id);
            }

            return returnFromClientWaybillRepository.Query<ReturnFromClientWaybillRow>()
                .PropertyIn(x => x.ReturnFromClientWaybill, waybillSubQuery)
                .Where(x => x.Article.Id == articleId)
                .ToList<ReturnFromClientWaybillRow>();
        }

        #endregion

        #region Список накладных

        public IEnumerable<ReturnFromClientWaybill> GetFilteredList(object state, User user, ParameterString param = null)
        {
            if (param == null) param = new ParameterString("");

            Func<ISubCriteria<ReturnFromClientWaybill>, ISubCriteria<ReturnFromClientWaybill>> cond = null;

            switch (user.GetPermissionDistributionType(Permission.ReturnFromClientWaybill_List_Details))
            {
                case PermissionDistributionType.None:
                    return new List<ReturnFromClientWaybill>();

                case PermissionDistributionType.Personal:
                    cond = x =>
                        {
                            AddArticleRestriction(param, x);

                            return x.PropertyIn(y => y.Deal.Id, dealRepository.GetSubQueryForDealIdOnPersonalPermission(user.Id)).Select(y => y.Id);
                        };
                    break;

                case PermissionDistributionType.Teams:
                    cond = x =>
                    {
                        AddArticleRestriction(param, x);

                        return x.PropertyIn(y => y.Deal.Id, dealRepository.GetSubQueryForDealIdOnTeamPermission(user.Id)).Select(y => y.Id);
                    };
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            return returnFromClientWaybillRepository.GetFilteredList(state, param, cond: cond);
        }

        /// <summary>
        /// Добавление ограничения по товару
        /// </summary>        
        private void AddArticleRestriction(ParameterString param, ISubCriteria<ReturnFromClientWaybill> x)
        {
            if (param.Keys.Contains("Article"))
            {
                if (!String.IsNullOrEmpty((param["Article"].Value as List<string>)[0]))
                {
                    var articleId = ValidationUtils.TryGetInt((param["Article"].Value as List<string>)[0]);

                    x.Restriction<ReturnFromClientWaybillRow>(y => y.Rows)
                        .Where(y => y.Article.Id == articleId);
                }
                param.Delete("Article");
            }
        }

        /// <summary>
        /// Получить список накладных возврата товара от клиента, дата принятия которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientList">Список клиентов</param>
        public IDictionary<Guid, ReturnFromClientWaybill> GetReceiptedListInDateRangeByClientList(DateTime startDate, DateTime endDate, IList<Client> clientList)
        {
            return returnFromClientWaybillRepository.GetReceiptedListInDateRangeByClientList(startDate, endDate, clientList.Select(x => x.Id).ToList());
        }

        /// <summary>
        /// Получить список накладных возврата товара от клиента, дата принятия которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationList">Список организаций клиента</param>
        public IDictionary<Guid, ReturnFromClientWaybill> GetReceiptedListInDateRangeByClientOrganizationList(DateTime startDate, DateTime endDate,
            IEnumerable<ClientOrganization> clientOrganizationList)
        {
            return returnFromClientWaybillRepository.GetReceiptedListInDateRangeByClientOrganizationList(startDate, endDate, clientOrganizationList.Select(x => x.Id).ToList());
        }

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю накладных возврата товара от клиента, принадлежащих данным клиентам и командам,
        /// дата проводки которых находится в диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов. Null - все клиенты</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="user">Пользователь</param>
        public IDictionary<Guid, ReturnFromClientWaybill> GetListInDateRangeByClientAndTeamList(DateTime startDate, DateTime endDate, IList<int> clientIdList, IEnumerable<short> teamIdList, User user)
        {
            return GetReceiptedListInDateRangeByDealSubQuery(
                dealRepository.GetDealSubQueryOnAllPermissionByClient(clientIdList),
                dealRepository.GetDealSubQueryOnTeamPermissionByClientList(clientIdList, user.Id),
                dealRepository.GetDealSubQueryOnPersonalPermissionByClientList(clientIdList, user.Id),
                teamIdList, startDate, endDate, user);
        }

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю накладных возврата товара от клиента, принадлежащих данным организациям клиентов и командам,
        /// дата проводки которых находится в диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиентов. Null - все организации клиентов</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="user">Пользователь</param>
        public IDictionary<Guid, ReturnFromClientWaybill> GetListInDateRangeByClientOrganizationAndTeamList(DateTime startDate, DateTime endDate, IList<int> clientOrganizationIdList, IEnumerable<short> teamIdList, User user)
        {
            return GetReceiptedListInDateRangeByDealSubQuery(
                dealRepository.GetDealSubQueryOnAllPermissionByClientOrganizationList(clientOrganizationIdList),
                dealRepository.GetDealSubQueryOnTeamPermissionByClientOrganizationList(clientOrganizationIdList, user.Id),
                dealRepository.GetDealSubQueryOnPersonalPermissionByClientOrganizationList(clientOrganizationIdList, user.Id),
                teamIdList, startDate, endDate, user);
        }

        /// <summary>
        /// Получить список накладных возврата товара от клиента, дата принятия которых находится в диапазоне дат, по подзапросу сделок и списку команд
        /// </summary>
        /// <param name="allDealSubQuery">Подкритерий, ограничивающий множество сделок (без учета видимости)</param>
        /// <param name="teamDealSubQuery">Подкритерий, ограничивающий множество сделок (командная видимость)</param>
        /// <param name="personalDealSubQuery">Подкритерий, ограничивающий множество сделок (персональная видимость)</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="user">Пользователь</param>
        private IDictionary<Guid, ReturnFromClientWaybill> GetReceiptedListInDateRangeByDealSubQuery(ISubQuery allDealSubQuery, ISubQuery teamDealSubQuery, ISubQuery personalDealSubQuery, 
            IEnumerable<short> teamIdList, DateTime startDate, DateTime endDate, User user)
        {
            var dealSubQuery = dealIndicatorService.GetDealSubQueryByPermissionDistribution(allDealSubQuery, teamDealSubQuery, personalDealSubQuery,
                Permission.ReturnFromClientWaybill_List_Details, user);
            
            ISubCriteria<Team> teamSubQuery = null;
            switch (user.GetPermissionDistributionType(Permission.Team_List_Details))
            {
                case PermissionDistributionType.All:
                    teamSubQuery = teamRepository.GetTeamSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                case PermissionDistributionType.Personal:
                    teamSubQuery = teamRepository.GetTeamSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    teamSubQuery = teamRepository.GetTeamSubQueryByNonePermission();
                    break;
            }

            return dealSubQuery != null ? returnFromClientWaybillRepository.GetReceiptedListInDateRangeByDealSubQuery(startDate, endDate, dealSubQuery, teamIdList, teamSubQuery) :
                new Dictionary<Guid, ReturnFromClientWaybill>();
        }


        /// <summary>
        /// Получение списка накладных
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="storageIdList">Список кодов мест хранения</param>
        /// <param name="storagePermission">Право, которым определяются доступные места хранения</param>
        /// <param name="curatorIdList">Список кодов кураторов</param>
        /// <param name="curatorPermission">Право, которым определяются доступные пользователи</param>
        /// <param name="clientIdList">Список кодов клиентов</param>
        /// <param name="clientPermission">Право, которым определяются доступные клиенты</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="pageNumber">Номер страницы, первая 1.</param>
        /// <param name="dateType">Тип даты</param>
        /// <param name="priorToDate">Параметр "До даты"</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Список накладных</returns>
        public IEnumerable<ReturnFromClientWaybill> GetList(ReturnFromClientWaybillLogicState logicState, IEnumerable<short> storageIdList, Permission storagePermission,
            IEnumerable<int> curatorIdList, Permission curatorPermission, IEnumerable<int> clientIdList, Permission clientPermission, DateTime startDate,
            DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate, User user)
        {
            ISubCriteria<Storage> storageSubQuery = null;
            ISubCriteria<User> curatorSubQuery = null;
            ISubCriteria<ReturnFromClientWaybill> returnFromClientWaybillSubQuery = null;

            switch (user.GetPermissionDistributionType(storagePermission))
            {
                case PermissionDistributionType.All:
                    storageSubQuery = storageRepository.GetStorageSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                    storageSubQuery = storageRepository.GetStorageSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    return new List<ReturnFromClientWaybill>();
            }

            switch (user.GetPermissionDistributionType(curatorPermission))
            {
                case PermissionDistributionType.All:
                    curatorSubQuery = userRepository.GetUserSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                    curatorSubQuery = userRepository.GetUserSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    return new List<ReturnFromClientWaybill>();
            }

            switch (user.GetPermissionDistributionType(Permission.ReturnFromClientWaybill_List_Details))
            {
                case PermissionDistributionType.All:
                    returnFromClientWaybillSubQuery = returnFromClientWaybillRepository.GetReturnFromClientWaybillSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                    returnFromClientWaybillSubQuery = returnFromClientWaybillRepository.GetReturnFromClientWaybillSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.Personal:
                    returnFromClientWaybillSubQuery = returnFromClientWaybillRepository.GetReturnFromClientWaybillSubQueryByPersonalPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    return new List<ReturnFromClientWaybill>();
            }

            switch (user.GetPermissionDistributionType(clientPermission))
            {
                case PermissionDistributionType.None:
                    return new List<ReturnFromClientWaybill>();
                case PermissionDistributionType.All:
                    break;
            }

            return returnFromClientWaybillRepository.GetList(logicState, returnFromClientWaybillSubQuery, storageIdList, storageSubQuery,
                curatorIdList, curatorSubQuery, clientIdList, startDate, endDate, pageNumber, dateType,  priorToDate);
        }

        #endregion

        #region Добавление / редактирование

        /// <summary>
        /// Проверка, видна ли накладная куратору
        /// </summary>
        /// <param name="returnFromClientWaybill">Накладная</param>
        /// <param name="curator">Куратор</param>
        public override void CheckPossibilityToViewDetailsByUser(ReturnFromClientWaybill returnFromClientWaybill, User curator)
        {
            var deals = curator.Teams.SelectMany(x => x.Deals);
            var result = false;

            switch (curator.GetPermissionDistributionType(Permission.ReturnFromClientWaybill_List_Details))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                    result = deals.Contains(returnFromClientWaybill.Deal) && returnFromClientWaybill.Curator == curator;
                    break;

                case PermissionDistributionType.Teams:
                    result = deals.Contains(returnFromClientWaybill.Deal);
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }
            ValidationUtils.Assert(result, "Куратор накладной должен имееть право на ее просмотр.");
        }

        public Guid Save(ReturnFromClientWaybill returnFromClientWaybill)
        {
            // если номер генерируется автоматически
            if (returnFromClientWaybill.Number == "")
            {
                var lastDocumentNumbers = returnFromClientWaybill.Recipient.GetLastDocumentNumbers(returnFromClientWaybill.Date.Year);
                var number = lastDocumentNumbers.ReturnFromClientWaybillLastNumber + 1;

                // пока не найдем уникальный номер
                while (!IsNumberUnique(number.ToString(), Guid.Empty, returnFromClientWaybill.Date, returnFromClientWaybill.Recipient))
                {
                    number = number + 1;
                }

                returnFromClientWaybill.Number = number.ToString();
                lastDocumentNumbers.ReturnFromClientWaybillLastNumber = number;
            }
            else
            {
                ValidationUtils.Assert(IsNumberUnique(returnFromClientWaybill.Number, returnFromClientWaybill.Id, returnFromClientWaybill.Date, returnFromClientWaybill.Recipient),
                String.Format("Накладная с номером {0} уже существует. Укажите другой номер.", returnFromClientWaybill.Number));
            }

            //Проверяем имеет ли указанная команда реализации в указанной сделке
            var teamList = dealRepository.GetTeamListFromSales(returnFromClientWaybill.Deal.Id);
            ValidationUtils.Assert(teamList.Contains(returnFromClientWaybill.Team), 
                String.Format("Команда «{0}» не имеет реализаций в сделке «{1}».", returnFromClientWaybill.Team.Name, returnFromClientWaybill.Deal.Name));

            returnFromClientWaybillRepository.Save(returnFromClientWaybill);

            return returnFromClientWaybill.Id;
        }

        /// <summary>
        /// Проверка номера накладной на уникальность
        /// </summary>
        /// <param name="number">Номер накладной</param>
        /// <param name="id">Код текущей накладной</param>
        /// <returns>Результат проверки</returns>
        private bool IsNumberUnique(string number, Guid id, DateTime documentDate, AccountOrganization accountOrganization)
        {
            return returnFromClientWaybillRepository.IsNumberUnique(number, id, documentDate, accountOrganization);
        }

        #endregion

        #region Добавление / удаление позиций

        /// <summary>
        /// Добавление позиции в накладную
        /// </summary>
        public virtual void AddRow(ReturnFromClientWaybill waybill, ReturnFromClientWaybillRow row)
        {
            waybill.AddRow(row);

            returnFromClientWaybillRepository.Save(waybill);
        }

        public void DeleteRow(ReturnFromClientWaybill returnFromClientWaybill, ReturnFromClientWaybillRow returnFromClientWaybillRow, User user)
        {
            CheckPossibilityToDeleteRow(returnFromClientWaybillRow, user);

            returnFromClientWaybill.DeleteRow(returnFromClientWaybillRow);

            returnFromClientWaybillRepository.Save(returnFromClientWaybill);
        }

        #endregion

        #region Подготовка / Отменить готовность к проводке

        /// <summary>
        /// Подготовка накладной к проводке
        /// </summary>
        /// <param name="waybill">Накладная возврата</param>
        /// <param name="user">Пользователь</param>
        public void PrepareToAccept(ReturnFromClientWaybill waybill, User user)
        {
            CheckPossibilityToPrepareToAccept(waybill, user);

            waybill.PrepareToAccept();
        }

        /// <summary>
        /// Отменить готовность к проводке
        /// </summary>
        /// <param name="waybill">Накладная возврата</param>
        /// <param name="user">Пользователь</param>
        public void CancelReadinessToAccept(ReturnFromClientWaybill waybill, User user)
        {
            CheckPossibilityToCancelReadinessToAccept(waybill, user);

            waybill.CancelReadinessToAccept();
        }

        #endregion

        #region Проводка / отмена проводки

        /// <summary>
        /// Проводка накладной
        /// </summary>
        /// <param name="waybill">Накладная</param>
        public void Accept(ReturnFromClientWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            CheckPossibilityToAccept(waybill, user);

            // получаем учетные цены для товаров в накладной
            var senderPriceLists = articlePriceService.GetArticleAccountingPrices(waybill.RecipientStorage.Id,
                returnFromClientWaybillRepository.GetArticlesSubquery(waybill.Id), currentDateTime);

            // проводим накладную
            waybill.Accept(senderPriceLists, UseReadyToAcceptState, user, currentDateTime);

            returnFromClientWaybillRepository.Save(waybill);

            // Пересчет показателей входящего проведенного наличия
            articleAvailabilityService.ReturnFromClientWaybillAccepted(waybill);

            // резервирование товара для возвратов и пересчет показателей возвратов
            returnFromClientService.ReturnFromClientWaybillAccepted(waybill);
        }

        /// <summary>
        /// Отмена проводки накладной
        /// </summary>
        /// <param name="waybill">Накладная</param>
        public void CancelAcceptance(ReturnFromClientWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - не появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            CheckPossibilityToCancelAcceptance(waybill, user);

            // отмена резервирования товара для возвратов и пересчет показателей возвратов
            returnFromClientService.ReturnFromClientWaybillAcceptanceCancelled(waybill);

            // Пересчет показателей входящего проведенного наличия
            articleAvailabilityService.ReturnFromClientWaybillAcceptanceCanceled(waybill);
            
            waybill.CancelAcceptance(UseReadyToAcceptState);

            returnFromClientWaybillRepository.Save(waybill);

            // удаление связей и пересчет проведенной переоценки
            articleRevaluationService.ReturnFromClientWaybillAcceptanceCancelled(waybill);
        }
        #endregion

        #region Приемка / отмена приемки

        /// <summary>
        /// Обновление признака полной оплаты для списка реализаций
        /// </summary>
        /// <param name="returnFromClientWaybill">Накладная возврата</param>
        private void UpdateFullyPaidPropertyForSales(ReturnFromClientWaybill returnFromClientWaybill)
        {
            var waybillList = returnFromClientWaybill.Rows.Select(x => x.SaleWaybillRow.SaleWaybill)
                .Distinct()
                .Where(x => x.Is<ExpenditureWaybill>())
                .Select(x => x.As<ExpenditureWaybill>());

            foreach (var waybill in waybillList)
            {
                var ind = expenditureWaybillIndicatorService.CalculateMainIndicators(waybill, calculateDebtRemainder: true);
                waybill.IsFullyPaid = ind.DebtRemainder == 0;
            }
        }

        /// <summary>
        /// Приемка накладной
        /// </summary>
        /// <param name="waybill">Накладная</param>
        public void Receipt(ReturnFromClientWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            CheckPossibilityToReceipt(waybill, user);

            waybill.Receipt(user, currentDateTime);

            returnFromClientWaybillRepository.Save(waybill);

            // Пересчет показателей входящего проведенного и точного наличия
            articleAvailabilityService.ReturnFromClientWaybillReceipted(waybill);

            // пересчет показателей возвратов от клиента
            returnFromClientService.ReturnFromClientWaybillFinalized(waybill);

            // пересчет финансового показателя
            factualFinancialArticleMovementService.ReturnFromClientWaybillReceipted(waybill);

            // расчет переоценок по принятым позициям
            articleRevaluationService.ReturnFromClientWaybillFinalized(waybill);

            // Пересчитываем счетчики количеств операций
            articleMovementOperationCountService.WaybillFinalized(waybill);
            
            returnFromClientWaybillRepository.Save(waybill);    //Т.к. для метода ReturnPaymentToSales необходимы строки накладной возврата.

            dealPaymentDocumentDistributionService.ReturnPaymentToSales(waybill, currentDateTime);  //Возвращаем оплаты

            returnFromClientWaybillRepository.Save(waybill);    //для того, чтобы сохранились изменения в оплатах

            //Обновляем признак полной оплаты для реализаций
            UpdateFullyPaidPropertyForSales(waybill);

            returnFromClientWaybillRepository.Save(waybill);

            articleMovementService.UpdateOutgoingWaybillsStates(waybill, waybill.ReceiptDate);
        }

        /// <summary>
        /// Отмена приемки накладной
        /// </summary>
        /// <param name="waybill">Накладная</param>
        public void CancelReceipt(ReturnFromClientWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            CheckPossibilityToCancelReceipt(waybill, user);

            // Пересчет показателей входящего проведенного и точного наличия
            articleAvailabilityService.ReturnFromClientWaybillReceiptCanceled(waybill,
                articleMovementService.GetOutgoingWaybillRows(returnFromClientWaybillRepository.GetRowsSubQuery(waybill.Id)));

            // пересчет показателей возвратов от клиента
            returnFromClientService.ReturnFromClientWaybillFinalizationCancelled(waybill);

            // пересчет показателей переоценки
            articleRevaluationService.ReturnFromClientWaybillReceiptCancelled(waybill);

            // пересчет финансового показателя
            factualFinancialArticleMovementService.ReturnFromClientWaybillReceiptCancelled(waybill);

            // Пересчитываем счетчики количеств операций
            articleMovementOperationCountService.WaybillFinalizationCancelled(waybill);

            var receiptDate = waybill.ReceiptDate.Value;    //Сохраняем дату принятия для индикаторов
            waybill.CancelReceipt();

            returnFromClientWaybillRepository.Save(waybill);

            // Отменяем возврат оплаты по возвращенным позициям
            dealPaymentDocumentDistributionService.CancelPaymentReturnToSales(waybill, receiptDate);

            returnFromClientWaybillRepository.Save(waybill);    //для того, чтобы сохранились изменения в оплатах

            // Обновляем признак полной оплаты для реализаций
            UpdateFullyPaidPropertyForSales(waybill);

            returnFromClientWaybillRepository.Save(waybill);

            articleMovementService.UpdateOutgoingWaybillsStates(waybill, waybill.ReceiptDate);
        }

        #endregion

        #region Удаление накладной

        /// <summary>
        /// Удаление накладной
        /// </summary>
        /// <param name="waybill">Накладная</param>
        public void Delete(ReturnFromClientWaybill waybill, User user)
        {
            CheckPossibilityToDelete(waybill, user);

            waybill.DeletionDate = DateTime.Now;
            returnFromClientWaybillRepository.Delete(waybill);
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Группировка строк накладной, чтобы для тех позиций, которые сделаны по одной партии, но из разных реализаций, записывалось не несколько индикаторов, а один, 
        /// но с суммой возвращаемых количеств по этим позициям
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private IEnumerable<dynamic> DistinctWaybillRows(IEnumerable<ReturnFromClientWaybillRow> rows)
        {
            return rows.GroupBy(x => new { x.Article, x.ReceiptWaybillRow, x.PurchaseCost },
                (key, group) => new
                {
                    Article = key.Article,
                    ReceiptWaybillRow = key.ReceiptWaybillRow,
                    PurchaseCost = key.PurchaseCost,
                    ReturnCount = group.Sum(x => x.ReturnCount),
                    AcceptedCount = group.Sum(x => x.AcceptedCount)
                });
        }


        #endregion

        #region Проверки на возможность совершения операций

        #region Настройки аккаунта

        /// <summary>
        /// Разрешение использовать статус «Готово к отгрузке»
        /// </summary>
        private bool UseReadyToAcceptState
        {
            get { return settingRepository.Get().UseReadyToAcceptStateForReturnFromClientWaybill; }
        }

        #endregion

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperationByDeal(ReturnFromClientWaybill waybill, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                    result = waybill.Deal.Curator == user &&
                        user.Teams.SelectMany(x => x.Deals).Contains(waybill.Deal); // свои + командные
                    break;

                case PermissionDistributionType.Teams:
                    result = user.Teams.SelectMany(x => x.Deals).Contains(waybill.Deal);
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private bool IsPermissionToPerformOperationByStorage(ReturnFromClientWaybill waybill, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                case PermissionDistributionType.Teams:
                    result = user.Teams.SelectMany(x => x.Storages).Contains(waybill.RecipientStorage);
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperationByDeal(ReturnFromClientWaybill waybill, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperationByDeal(waybill, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        private void CheckPermissionToPerformOperationByStorage(ReturnFromClientWaybill waybill, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperationByStorage(waybill, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #endregion

        #region Редактирование

        public bool IsPossibilityToEdit(ReturnFromClientWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToEdit(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEdit(ReturnFromClientWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperationByDeal(waybill, user, Permission.ReturnFromClientWaybill_Create_Edit);

            // сущность
            waybill.CheckPossibilityToEdit();
        }

        #endregion

        #region Удаление

        public bool IsPossibilityToDelete(ReturnFromClientWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToDelete(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDelete(ReturnFromClientWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperationByDeal(waybill, user, Permission.ReturnFromClientWaybill_Delete_Row_Delete);

            // сущность
            waybill.CheckPossibilityToDelete();
        }

        #endregion

        #region Удаление позиции

        public bool IsPossibilityToDeleteRow(ReturnFromClientWaybillRow row, User user)
        {
            try
            {
                CheckPossibilityToDeleteRow(row, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeleteRow(ReturnFromClientWaybillRow row, User user)
        {
            // права
            CheckPermissionToPerformOperationByDeal(row.ReturnFromClientWaybill, user, Permission.ReturnFromClientWaybill_Delete_Row_Delete);

            // сущность
            row.CheckPossibilityToDelete();
        }
        #endregion

        #region Подготовка к проводке

        public bool IsPossibilityToPrepareToAccept(ReturnFromClientWaybill waybill, User user, bool onlyPermission = false)
        {
            try
            {
                CheckPossibilityToPrepareToAccept(waybill, user, onlyPermission);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToPrepareToAccept(ReturnFromClientWaybill waybill, User user, bool onlyPermission = false)
        {
            // настроки аккаунта
            ValidationUtils.Assert(UseReadyToAcceptState, "Невозможно выполнить операцию, т.к. опция подготовки накладной к проводке отключена в настройках аккаунта.");

            // права
            CheckPermissionToPerformOperationByDeal(waybill, user, Permission.ReturnFromClientWaybill_Create_Edit);

            // Сделано для корретного отображения кнопки на форме
            if (!onlyPermission)
            {
                // сущность
                waybill.CheckPossibilityToPrepareToAccept();
            }
            else
            {
                // При проверке прав необходимо убедиться, что "следующий шаг" для накладной подготовка к проводке.
                ValidationUtils.Assert(waybill.IsDraft, String.Format("Невозможно подготовить к проводке накладную со статусом «{0}».", waybill.State.GetDisplayName()));
            }
        }

        #endregion

        #region Отменить готовность к проводке

        public bool IsPossibilityToCancelReadinessToAccept(ReturnFromClientWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToCancelReadinessToAccept(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCancelReadinessToAccept(ReturnFromClientWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperationByDeal(waybill, user, Permission.ReturnFromClientWaybill_Create_Edit);

            // сущность
            waybill.CheckPossibilityToCancelReadinessToAccept();
        }

        #endregion

        #region Проводка

        public bool IsPossibilityToAccept(ReturnFromClientWaybill waybill, User user, bool onlyPermission = false)
        {
            try
            {
                CheckPossibilityToAccept(waybill, user, onlyPermission);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAccept(ReturnFromClientWaybill waybill, User user, bool onlyPermission = false)
        {
            // права
            CheckPermissionToPerformOperationByDeal(waybill, user, Permission.ReturnFromClientWaybill_Accept_Deal_List);
            CheckPermissionToPerformOperationByStorage(waybill, user, Permission.ReturnFromClientWaybill_Accept_Storage_List);

            if (!onlyPermission)
            {
                // сущность
                waybill.CheckPossibilityToAccept(UseReadyToAcceptState);
            }
            else
            {
                ValidationUtils.Assert(UseReadyToAcceptState ? waybill.IsReadyToAccept : waybill.IsNew,
                    String.Format("Невозможно провести накладную со статусом «{0}».", waybill.State.GetDisplayName()));
            }
        }

        #endregion

        #region Отмена проводки

        public bool IsPossibilityToCancelAcceptance(ReturnFromClientWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToCancelAcceptance(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCancelAcceptance(ReturnFromClientWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperationByDeal(waybill, user, Permission.ReturnFromClientWaybill_Acceptance_Cancel_Deal_List);
            CheckPermissionToPerformOperationByStorage(waybill, user, Permission.ReturnFromClientWaybill_Acceptance_Cancel_Storage_List);

            // сущность
            waybill.CheckPossibilityToCancelAcceptance();
        }

        #endregion

        #region Приемка

        public bool IsPossibilityToReceipt(ReturnFromClientWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToReceipt(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToReceipt(ReturnFromClientWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperationByDeal(waybill, user, Permission.ReturnFromClientWaybill_Receipt_Deal_List);
            CheckPermissionToPerformOperationByStorage(waybill, user, Permission.ReturnFromClientWaybill_Receipt_Storage_List);

            // сущность
            waybill.CheckPossibilityToReceipt();
        }

        #endregion

        #region Отмена приемки

        public bool IsPossibilityToCancelReceipt(ReturnFromClientWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToCancelReceipt(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCancelReceipt(ReturnFromClientWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperationByDeal(waybill, user, Permission.ReturnFromClientWaybill_Receipt_Cancel_Deal_List);
            CheckPermissionToPerformOperationByStorage(waybill, user, Permission.ReturnFromClientWaybill_Receipt_Cancel_Storage_List);

            // сущность
            waybill.CheckPossibilityToCancelReceipt();
        }

        #endregion

        #region Возможность редактировать команду

        public bool IsPossibilityToEditTeam(ReturnFromClientWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToEditTeam(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditTeam(ReturnFromClientWaybill waybill, User user)
        {
            // Проверяем возможность редактировать возврат
            CheckPossibilityToEdit(waybill, user);

            // сущность
            waybill.CheckPossibilityToEditTeam();
        }

        #endregion

        #region Возможность печатать формы документов

        public bool IsPossibilityToPrintForms(ReturnFromClientWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintForms, waybill, user);
        }

        public void CheckPossibilityToPrintForms(ReturnFromClientWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperationByDeal(waybill, user, Permission.ReturnFromClientWaybill_List_Details);

            // сущность
            ValidationUtils.Assert(waybill.IsAccepted, "Невозможно распечатать форму, т.к. накладная еще не проведена.");
        }

        #region Печать форм в отпускных ценах

        public bool IsPossibilityToPrintFormInSalePrices(ReturnFromClientWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintFormInSalePrices, waybill, user);
        }

        public void CheckPossibilityToPrintFormInSalePrices(ReturnFromClientWaybill waybill, User user)
        {
            CheckPossibilityToPrintForms(waybill, user);

            ValidationUtils.Assert(user.HasPermissionToViewStorageAccountingPrices(waybill.RecipientStorage),
                "Недостаточно прав для просмотра отпускных цен.");
        }
        #endregion

        #region Печать форм в ЗЦ

        public bool IsPossibilityToPrintFormInPurchaseCosts(ReturnFromClientWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintFormInPurchaseCosts, waybill, user);
        }

        public void CheckPossibilityToPrintFormInPurchaseCosts(ReturnFromClientWaybill waybill, User user)
        {
            CheckPossibilityToPrintForms(waybill, user);

            ValidationUtils.Assert(user.HasPermission(Permission.PurchaseCost_View_ForEverywhere),
                "Недостаточно прав для просмотра закупочных цен.");
        }
        #endregion

        #endregion

        #region Смена куратора

        public override void CheckPossibilityToChangeCurator(ReturnFromClientWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperationByDeal(waybill, user, Permission.ReturnFromClientWaybill_Curator_Change);
            CheckPermissionToPerformOperationByStorage(waybill, user, Permission.ReturnFromClientWaybill_Curator_Change);

            //сущность
            waybill.CheckPossibilityToChangeCurator();
        }
        #endregion

        #endregion

        #endregion
    }
}