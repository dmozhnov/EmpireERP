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
    public class ExpenditureWaybillService : BaseOutgoingWaybillService<ExpenditureWaybill>, IExpenditureWaybillService
    {
        #region Поля

        private readonly ISettingRepository settingRepository;
        private readonly IExpenditureWaybillRepository expenditureWaybillRepository;
        private readonly ITeamRepository teamRepository;
        private readonly IDealRepository dealRepository;
        private readonly IStorageRepository storageRepository;
        private readonly IUserRepository userRepository;

        private readonly IArticlePriceService articlePriceService;
        private readonly IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService;
        private readonly IArticleMovementService articleMovementService;        
        private readonly IDealPaymentDocumentDistributionService dealPaymentDocumentDistributionService;
        private readonly IBlockingService blockingService;
        private readonly IArticleSaleService articleSaleService;
        private readonly IFactualFinancialArticleMovementService factualFinancialArticleMovementService;
        private readonly ITeamService teamService;
        private readonly IArticleMovementOperationCountService articleMovementOperationCountService;
        private readonly IDealService dealService;
        private readonly IDealIndicatorService dealIndicatorService;
        private readonly IClientContractIndicatorService clientContractIndicatorService;
        private readonly IReceiptWaybillService receiptWaybillService;
        private readonly IReturnFromClientWaybillRepository returnFromClientWaybillRepository;

        private readonly IArticleRevaluationService articleRevaluationService;

        #endregion

        #region Конструктор

        public ExpenditureWaybillService(ISettingRepository settingRepository, IExpenditureWaybillRepository expenditureWaybillRepository, ITeamRepository teamRepository, 
            IStorageRepository storageRepository, IUserRepository userRepository, IDealRepository dealRepository, IArticlePriceService articlePriceService,
            IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService, IArticleMovementService articleMovementService,            
            IDealPaymentDocumentDistributionService dealPaymentDocumentDistributionService,
            IBlockingService blockingService, IArticleSaleService articleSaleService,
            IFactualFinancialArticleMovementService factualFinancialArticleMovementService,
            IArticleMovementOperationCountService articleMovementOperationCountService, ITeamService teamService,
            IDealService dealService, IDealIndicatorService dealIndicatorService, IClientContractIndicatorService clientContractIndicatorService,
            IArticleAvailabilityService articleAvailabilityService, IReceiptWaybillService receiptWaybillService, IArticleRevaluationService articleRevaluationService,
            IReturnFromClientWaybillRepository returnFromClientWaybillRepository)
            : base(articleAvailabilityService)
        {
            this.settingRepository = settingRepository;
            this.expenditureWaybillRepository = expenditureWaybillRepository;
            this.teamRepository = teamRepository;
            this.dealRepository = dealRepository;
            this.storageRepository = storageRepository;
            this.userRepository = userRepository;
            this.articlePriceService = articlePriceService;
            this.expenditureWaybillIndicatorService = expenditureWaybillIndicatorService;
            this.articleMovementService = articleMovementService;            
            this.dealPaymentDocumentDistributionService = dealPaymentDocumentDistributionService;
            this.blockingService = blockingService;
            this.articleSaleService = articleSaleService;
            this.factualFinancialArticleMovementService = factualFinancialArticleMovementService;
            this.articleMovementOperationCountService = articleMovementOperationCountService;
            this.teamService = teamService;
            this.dealService = dealService;
            this.dealIndicatorService = dealIndicatorService;
            this.clientContractIndicatorService = clientContractIndicatorService;
            this.receiptWaybillService = receiptWaybillService;
            this.returnFromClientWaybillRepository = returnFromClientWaybillRepository;

            this.articleRevaluationService = articleRevaluationService;
        }

        #endregion

        #region Методы

        #region Получение накладной по Id

        /// <summary>
        /// Получение накладной по коду с учетом прав пользователя
        /// </summary>
        /// <param name="id">Код накладной</param>
        /// <param name="user">Пользователь</param>        
        private ExpenditureWaybill GetById(Guid id, User user)
        {
            var type = user.GetPermissionDistributionType(Permission.ExpenditureWaybill_List_Details);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var waybill = expenditureWaybillRepository.GetById(id);

                if (waybill == null) return null;

                // проверяем только командные сделки и не проверяем МХ
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
        public override ExpenditureWaybill CheckWaybillExistence(Guid id, User user)
        {
            var waybill = GetById(id, user);
            ValidationUtils.NotNull(waybill, "Накладная реализации товаров не найдена. Возможно, она была удалена.");

            return waybill;
        }

        /// <summary>
        /// Получение списка накладных по списку id с проверкой их существования
        /// </summary>
        /// <param name="idList">Список кодов накладных</param>
        /// <param name="user">Пользователь</param>
        /// <param name="message">Сообщение об ошибке (если хоть на одну нет прав или она не существует)</param>
        public IDictionary<Guid, ExpenditureWaybill> CheckWaybillExistence(IEnumerable<Guid> idList, User user, string message = "")
        {
            if (idList.Count() == 0)
            {
                return new Dictionary<Guid, ExpenditureWaybill>();
            }

            var dealSubQuery = dealIndicatorService.RestrictByUserPermissions(user, Permission.ExpenditureWaybill_List_Details);
            var result = dealSubQuery != null ?
                expenditureWaybillRepository.GetById(idList, dealSubQuery) :
                new Dictionary<Guid, ExpenditureWaybill>();

            ValidationUtils.Assert(idList.Distinct().Count() == result.Count,
                !String.IsNullOrEmpty(message) ? message : "Накладная реализации товаров не найдена. Возможно, она была удалена.");

            return result;
        }

        public ExpenditureWaybillRow GetRowById(Guid id)
        {
            return expenditureWaybillRepository.GetRowById(id);
        }

        #endregion

        #region Список накладных

        public IEnumerable<ExpenditureWaybill> GetFilteredList(object state, User user, ParameterString param = null)
        {
            if (param == null) param = new ParameterString("");

            Func<ISubCriteria<ExpenditureWaybill>, ISubCriteria<ExpenditureWaybill>> cond = null;

            switch (user.GetPermissionDistributionType(Permission.ExpenditureWaybill_List_Details))
            {
                case PermissionDistributionType.None:
                    return new List<ExpenditureWaybill>();

                case PermissionDistributionType.Personal:
                    cond = x => x.PropertyIn(y => y.Deal.Id, dealRepository.GetSubQueryForDealIdOnPersonalPermission(user.Id)).Select(y => y.Id);
                    break;

                case PermissionDistributionType.Teams:
                    cond = x => x.PropertyIn(y => y.Deal.Id, dealRepository.GetSubQueryForDealIdOnTeamPermission(user.Id)).Select(y => y.Id);
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            return expenditureWaybillRepository.GetFilteredList(state, param, cond: cond);
        }

        private ISubCriteria<ExpenditureWaybill> RestrictExpenditureWaybillByStorageAndUser( ISubCriteria<ExpenditureWaybill> crit, User user, 
            Permission storagePermission, Permission userPermission)
        {
            switch (user.GetPermissionDistributionType(storagePermission))
            {
                case PermissionDistributionType.Teams:
                    crit.PropertyIn(y => y.SenderStorage.Id, storageRepository.GetStorageSubQueryByTeamPermission(user.Id));
                    break;
            }
            switch (user.GetPermissionDistributionType(userPermission))
            {
                case PermissionDistributionType.Teams:
                    crit.PropertyIn(y => y.Curator.Id, userRepository.GetUserSubQueryByTeamPermission(user.Id));
                    break;
            }

            return crit;
        }

        public IEnumerable<ExpenditureWaybill> GetFilteredList(object state, User user, ParameterString param, Permission storagePermission, Permission userPermission)
        {
            if (param == null) param = new ParameterString("");

            Func<ISubCriteria<ExpenditureWaybill>, ISubCriteria<ExpenditureWaybill>> permission_cond = null;

            switch (user.GetPermissionDistributionType(Permission.ExpenditureWaybill_List_Details))
            {
                case PermissionDistributionType.None:
                    return new List<ExpenditureWaybill>();

                case PermissionDistributionType.Personal:
                    permission_cond = x =>
                    {
                        var sq = x.PropertyIn(y => y.Deal.Id, dealRepository.GetSubQueryForDealIdOnPersonalPermission(user.Id));
                        sq = RestrictExpenditureWaybillByStorageAndUser(sq, user, storagePermission, userPermission);

                        return sq.Select(y => y.Id);
                    };
                    break;

                case PermissionDistributionType.Teams:
                    permission_cond = x =>
                    {
                        var sq = x.PropertyIn(y => y.Deal.Id, dealRepository.GetSubQueryForDealIdOnTeamPermission(user.Id));
                        sq = RestrictExpenditureWaybillByStorageAndUser(sq, user, storagePermission, userPermission);

                        return sq.Select(y => y.Id);
                    };
                    break;

                case PermissionDistributionType.All:
                    permission_cond = x =>
                        {
                            x = RestrictExpenditureWaybillByStorageAndUser(x, user, storagePermission, userPermission);

                            return x.Select(y => y.Id);
                        };
                    break;
            }

            return expenditureWaybillRepository.GetFilteredList(state, param, cond: permission_cond);
        }

        /// <summary>
        /// Подзапрос для получения реализаций с учетом права на просмотр
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public IQueryable<ExpenditureWaybill> GetExpenditureWaybillByListPermission(User user)
        {
            IQueryable<ExpenditureWaybill> result = null;
            switch (user.GetPermissionDistributionType(Permission.ExpenditureWaybill_List_Details))
            {
                case PermissionDistributionType.None:
                    result = expenditureWaybillRepository.GetExpenditureWaybillByNonePermission();
                    break;

                case PermissionDistributionType.Personal:
                    result = expenditureWaybillRepository.GetExpenditureWaybillByPersonalPermission(user.Id);
                    break;

                case PermissionDistributionType.Teams:
                    result = expenditureWaybillRepository.GetExpenditureWaybillByTeamPermission(user.Id);
                    break;

                case PermissionDistributionType.All:
                    result = expenditureWaybillRepository.GetExpenditureWaybillByAllPermission();
                    break;
            }

            return result;
        }

        /// <summary>
        /// Получить список накладных реализации, дата отгрузки которых находится в диапазоне дат, по подзапросу сделок
        /// </summary>
        /// <param name="allDealSubQuery">Подкритерий, ограничивающий множество сделок (без учета видимости)</param>
        /// <param name="teamDealSubQuery">Подкритерий, ограничивающий множество сделок (командная видимость)</param>
        /// <param name="personalDealSubQuery">Подкритерий, ограничивающий множество сделок (персональная видимость)</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="user">Пользователь</param>
        private IDictionary<Guid, ExpenditureWaybill> GetShippedListInDateRangeByDealSubQuery(ISubQuery allDealSubQuery, ISubQuery teamDealSubQuery, ISubQuery personalDealSubQuery,
            IEnumerable<short> teamIdList, DateTime startDate, DateTime endDate, User user)
        {
            var dealSubQuery = dealIndicatorService.GetDealSubQueryByPermissionDistribution(allDealSubQuery, teamDealSubQuery, personalDealSubQuery,
                Permission.ExpenditureWaybill_List_Details, user);

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

            return dealSubQuery != null ? expenditureWaybillRepository.GetShippedListInDateRangeByDealSubQuery(startDate, endDate, dealSubQuery, teamIdList, teamSubQuery) :
                new Dictionary<Guid, ExpenditureWaybill>();
        }

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю накладных реализации, принадлежащих данным клиентам и командам,
        /// дата отгрузки которых находится в диапазоне дат
        /// </summary>
        /// <param name="clientIdList">Список кодов клиентов. Null - все клиенты</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="user">Пользователь</param>
        public IDictionary<Guid, ExpenditureWaybill> GetShippedListInDateRangeByClientAndTeamList(DateTime startDate, DateTime endDate, IList<int> clientIdList, IEnumerable<short> teamIdList, User user)
        {
            return GetShippedListInDateRangeByDealSubQuery(
                dealRepository.GetDealSubQueryOnAllPermissionByClient(clientIdList),
                dealRepository.GetDealSubQueryOnTeamPermissionByClientList(clientIdList, user.Id),
                dealRepository.GetDealSubQueryOnPersonalPermissionByClientList(clientIdList, user.Id),
                teamIdList, startDate, endDate, user);
        }

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю накладных реализации, принадлежащих данным организациям клиентов  и командам,
        /// дата отгрузки которых находится в диапазоне дат
        /// </summary>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиентов. Null - все организации клиентов</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="user">Пользователь</param>
        public IDictionary<Guid, ExpenditureWaybill> GetShippedListInDateRangeByClientOrganizationAndTeamList(DateTime startDate, DateTime endDate, IList<int> clientOrganizationIdList, IEnumerable<short> teamIdList, User user)
        {
            return GetShippedListInDateRangeByDealSubQuery(
                dealRepository.GetDealSubQueryOnAllPermissionByClientOrganizationList(clientOrganizationIdList),
                dealRepository.GetDealSubQueryOnTeamPermissionByClientOrganizationList(clientOrganizationIdList, user.Id),
                dealRepository.GetDealSubQueryOnPersonalPermissionByClientOrganizationList(clientOrganizationIdList, user.Id),
                teamIdList, startDate, endDate, user);
        }

        /// <summary>
        /// Получить список накладных реализации, дата отгрузки которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientList">Список клиентов</param>
        public IDictionary<Guid, ExpenditureWaybill> GetShippedListInDateRangeByClientList(DateTime startDate, DateTime endDate, IEnumerable<Client> clientList)
        {
            return expenditureWaybillRepository.GetShippedListInDateRangeByClientList(startDate, endDate, clientList.Select(x => x.Id).ToList());
        }

        /// <summary>
        /// Получить список накладных реализации, дата отгрузки которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationList">Список организаций клиента</param>
        public IDictionary<Guid, ExpenditureWaybill> GetShippedListInDateRangeByClientOrganizationList(DateTime startDate, DateTime endDate, IEnumerable<ClientOrganization> clientOrganizationList)
        {
            return expenditureWaybillRepository.GetShippedListInDateRangeByClientOrganizationList(startDate, endDate, clientOrganizationList.Select(x => x.Id).ToList());
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
        public IEnumerable<ExpenditureWaybill> GetList(ExpenditureWaybillLogicState logicState, IEnumerable<short> storageIdList, Permission storagePermission, 
            IEnumerable<int> curatorIdList, Permission curatorPermission, IEnumerable<int> clientIdList, Permission clientPermission, DateTime startDate, 
            DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate, User user)
        {
            ISubCriteria<Storage> storageSubQuery = null;
            ISubCriteria<User> curatorSubQuery = null;
            ISubCriteria<ExpenditureWaybill> expenditureWaybillSubQuery = null;

            switch (user.GetPermissionDistributionType(storagePermission))
            {
                case PermissionDistributionType.All:
                    storageSubQuery = storageRepository.GetStorageSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                    storageSubQuery = storageRepository.GetStorageSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    return new List<ExpenditureWaybill>();
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
                    return new List<ExpenditureWaybill>();
            }

            switch (user.GetPermissionDistributionType(Permission.ExpenditureWaybill_List_Details))
            {
                case PermissionDistributionType.All:
                    expenditureWaybillSubQuery = expenditureWaybillRepository.GetExpenditureWaybillSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                    expenditureWaybillSubQuery = expenditureWaybillRepository.GetExpenditureWaybillSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.Personal:
                    expenditureWaybillSubQuery = expenditureWaybillRepository.GetExpenditureWaybillSubQueryByPersonalPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    return new List<ExpenditureWaybill>();
            }

            switch (user.GetPermissionDistributionType(clientPermission))
            {
                case PermissionDistributionType.None:
                    return new List<ExpenditureWaybill>();
                case PermissionDistributionType.All:
                    break;
            }

            return expenditureWaybillRepository.GetList(logicState, expenditureWaybillSubQuery, storageIdList, storageSubQuery,
                curatorIdList, curatorSubQuery, clientIdList, startDate, endDate, pageNumber, dateType, priorToDate);
        }

        #endregion

        #region Список позиций

        /// <summary>
        /// Все реализации указанного товара в указанные сроки, у которых МХ-отправитель входит в указанный список
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="storageIds"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IEnumerable<ExpenditureWaybillRow> GetRows(int articleId, IEnumerable<short> storageIds, DateTime startDate, DateTime endDate, bool finallyMovedWaybillsOnly)
        {
            ISubQuery waybillSubQuery;

            if (finallyMovedWaybillsOnly)
            {
                waybillSubQuery = expenditureWaybillRepository.SubQuery<ExpenditureWaybill>()
               .OneOf(x => x.SenderStorage.Id, storageIds)
               .Where(x => x.ShippingDate >= startDate && x.ShippingDate <= endDate)
               .Select(x => x.Id);
            }
            else
            {
                waybillSubQuery = expenditureWaybillRepository.SubQuery<ExpenditureWaybill>()
                .OneOf(x => x.SenderStorage.Id, storageIds)
                .Where(x => x.AcceptanceDate >= startDate && x.AcceptanceDate <= endDate)
                .Select(x => x.Id);
            }

            return expenditureWaybillRepository.Query<ExpenditureWaybillRow>()
                .PropertyIn(x => x.SaleWaybill, waybillSubQuery)
                .Where(x => x.Article.Id == articleId)
                .ToList<ExpenditureWaybillRow>();
        }

        #endregion

        #region Добавление / редактирование

        /// <summary>
        /// Проверка номера накладной на уникальность
        /// </summary>
        /// <param name="number">Номер накладной</param>
        /// <param name="id">Код текущей накладной</param>
        /// <returns>Результат проверки</returns>
        private bool IsNumberUnique(string number, Guid id, DateTime documentDate, AccountOrganization accountOrganization)
        {
            return expenditureWaybillRepository.IsNumberUnique(number, id, documentDate, accountOrganization);
        }

        /// <summary>
        /// Проверка, видна ли накладная куратору
        /// </summary>
        /// <param name="expenditureWaybill">Накладная</param>
        /// <param name="curator">Куратор</param>
        public override void CheckPossibilityToViewDetailsByUser(ExpenditureWaybill expenditureWaybill, User curator)
        {
            var deals = curator.Teams.SelectMany(x => x.Deals);
            var result = false;

            switch (curator.GetPermissionDistributionType(Permission.ExpenditureWaybill_List_Details))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;
                case PermissionDistributionType.Personal:
                    result = deals.Contains(expenditureWaybill.Deal) && expenditureWaybill.Curator == curator;
                    break;
                case PermissionDistributionType.Teams:
                    result = deals.Contains(expenditureWaybill.Deal);
                    break;
                case PermissionDistributionType.All:
                    result = true;
                    break;
            }
            ValidationUtils.Assert(result, "Куратор накладной должен имееть право на ее просмотр.");
        }

        public Guid Save(ExpenditureWaybill expenditureWaybill)
        {
            ValidationUtils.NotNull(expenditureWaybill.SenderStorage, "Не указано место хранения отправителя.");
            ValidationUtils.NotNull(expenditureWaybill.Deal, "Не указана сделка по накладной реализации товаров.");

            // если номер генерируется автоматически
            if (expenditureWaybill.Number == "")
            {
                var lastDocumentNumbers = expenditureWaybill.Sender.GetLastDocumentNumbers(expenditureWaybill.Date.Year);
                var number = lastDocumentNumbers.ExpenditureWaybillLastNumber + 1;

                // пока не найдем уникальный номер
                while (!IsNumberUnique(number.ToString(), Guid.Empty, expenditureWaybill.Date, expenditureWaybill.Sender))
                {
                    number = number + 1;
                }

                expenditureWaybill.Number = number.ToString();
                lastDocumentNumbers.ExpenditureWaybillLastNumber = number;
            }
            else
            {
                ValidationUtils.Assert(IsNumberUnique(expenditureWaybill.Number, expenditureWaybill.Id, expenditureWaybill.Date, expenditureWaybill.Sender),
                String.Format("Накладная с номером {0} уже существует. Укажите другой номер.", expenditureWaybill.Number));
            }

            expenditureWaybillRepository.Save(expenditureWaybill);

            return expenditureWaybill.Id;
        }

        #endregion

        #region Расчет показателей

        /// <summary>
        /// Расчет основных показателей накладной
        /// </summary>                
        public ExpenditureWaybillMainIndicators GetMainIndicators(ExpenditureWaybill waybill, bool calculateSenderAccountingPriceSum = false,
            bool calculateSalePriceSum = false, bool calculatePaymentSum = false, bool calculatePaymentPercent = false, bool calculateDebtRemainder = false,
            bool calculateVatInfoList = false, bool calculateTotalDiscount = false, bool calculateProfit = false, bool calculateLostProfit = false,
            bool calculateTotalReturnedSum = false, bool calculateTotalReservedByReturnSum = false)
        {
            return expenditureWaybillIndicatorService.CalculateMainIndicators(waybill, calculateSenderAccountingPriceSum,
                calculateSalePriceSum, calculatePaymentSum, calculatePaymentPercent,
                calculateDebtRemainder, calculateVatInfoList, calculateTotalDiscount, calculateProfit, calculateLostProfit, calculateTotalReturnedSum, calculateTotalReservedByReturnSum);
        }

        /// <summary>
        /// Расчет основных показателей для списка позиций накладной
        /// </summary>
        public IDictionary<Guid, ExpenditureWaybillRowMainIndicators> GetMainRowsIndicators(ExpenditureWaybill waybill, User user, bool calculateSalePrice = false, bool calculateValueAddedTaxSum = false)
        {
            return expenditureWaybillIndicatorService.CalculateRowsMainIndicators(waybill, allowToViewAccPrices: user.HasPermissionToViewStorageAccountingPrices(waybill.SenderStorage),
            calculateSalePrice: calculateSalePrice, calculateValueAddedTaxSum: calculateValueAddedTaxSum);
        }

        /// <summary>
        /// Расчет основных показателей для позиции накладной
        /// </summary>
        public ExpenditureWaybillRowMainIndicators GetMainRowIndicators(ExpenditureWaybillRow waybillRow, User user,
            bool calculateSalePrice = false, bool calculateValueAddedTaxSum = false)
        {
            return expenditureWaybillIndicatorService.CalculateRowMainIndicators(waybillRow, user.HasPermissionToViewStorageAccountingPrices(waybillRow.ExpenditureWaybill.SenderStorage),
            calculateSalePrice: calculateSalePrice, calculateValueAddedTaxSum: calculateValueAddedTaxSum)[waybillRow.Id];
        }

        /// <summary>
        /// Расчет суммы отпускных цен для накладной
        /// </summary>
        /// <param name="waybill"></param>
        /// <returns></returns>
        public decimal CalculateSalePriceSum(ExpenditureWaybill waybill)
        {
            return expenditureWaybillIndicatorService.CalculateSalePriceSum(waybill);
        }

        /// <summary>
        /// Получение отпускной цены для позиции накладной
        /// </summary>        
        public decimal? CalculateRowSalePrice(ExpenditureWaybillRow waybillRow)
        {
            return expenditureWaybillIndicatorService.CalculateRowSalePrice(waybillRow);
        }

        #endregion

        #region Подготовка / Отменить готовность к проводке

        /// <summary>
        /// Подготовка накладной к проводке
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="user">Пользователь</param>
        public void PrepareToAccept(ExpenditureWaybill waybill, User user)
        {
            CheckPossibilityToPrepareToAccept(waybill, user);

            waybill.PrepareToAccept();
        }

        /// <summary>
        /// Отмена готовности накладной к проводке
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="user">Пользователь</param>
        public void CancelReadinessToAccept(ExpenditureWaybill waybill, User user)
        {
            CheckPossibilityToCancelReadinessToAccept(waybill, user);

            waybill.CancelReadinessToAccept();
        }

        #endregion

        #region Проводка / отмена проводки

        /// <summary>
        /// Проводка накладной
        /// </summary>
        /// <param name="waybill"></param>
        public void Accept(ExpenditureWaybill waybill, User acceptedBy, DateTime currentDateTime)
        {
            CheckPossibilityToAccept(waybill, acceptedBy);

            AcceptCommon(waybill, currentDateTime, acceptedBy, currentDateTime);    // даты совпадают
        }

        /// <summary>
        /// Проводка накладной задним числом
        /// </summary>
        /// <param name="waybill">Накладная реализации</param>
        /// <param name="acceptanceDate">Дата, на которую нужно провести накладную</param>
        /// <param name="currentDateTime">Текущая дата</param>
        public void AcceptRetroactively(ExpenditureWaybill waybill, DateTime acceptanceDate, User acceptedBy, DateTime currentDateTime)
        {
            CheckPossibilityToAcceptRetroactively(waybill, acceptedBy);

            ValidationUtils.Assert(acceptanceDate >= waybill.Date, "Дата проводки накладной не может быть меньше даты накладной.");
            ValidationUtils.Assert(acceptanceDate < currentDateTime, "Время проводки накладной должно быть меньше текущего времени.");

            AcceptCommon(waybill, acceptanceDate, acceptedBy, currentDateTime);

            expenditureWaybillRepository.Save(waybill);

            // создание связей и пересчет проведенной переоценки (актуально только для проводки задним числом)
            articleRevaluationService.ExpenditureWaybillAccepted(waybill);
        }

        /// <summary>
        /// Общая часть операции проводки
        /// </summary>        
        private void AcceptCommon(ExpenditureWaybill waybill, DateTime acceptanceDate, User acceptedBy, DateTime currentDateTime)
        {
            // регулярная проверка - не появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime); // именно на данный момент
            
            // получение текущих позиций реестров цен
            var senderPriceLists = articlePriceService.GetArticleAccountingPrices(waybill.SenderStorage.Id,
                expenditureWaybillRepository.GetArticlesSubquery(waybill.Id), acceptanceDate);

            // проводим накладную
            waybill.Accept(senderPriceLists, UseReadyToAcceptState, acceptedBy, acceptanceDate);

            // резервирование товаров при проводке
            var reservationInfoList = articleMovementService.AcceptArticles(waybill);

            expenditureWaybillRepository.Save(waybill); // Flush

            // Пересчет показателей исходящего проведенного наличия
            articleAvailabilityService.ExpenditureWaybillAccepted(waybill,
                articleMovementService.GetIncomingWaybillRowForOutgoingWaybillRow(expenditureWaybillRepository.GetRowsSubQuery(waybill.Id)));

            // пересчет показателя реализации
            articleSaleService.ExpenditureWaybillAccepted(waybill);

            // Засчитываем возможный аванс по накладной. Если она имеет нулевую сумму, сразу ставится признак полной оплаты.
            // На данный момент неоплаченный остаток по накладной равен сумме накладной в ОЦ
            dealPaymentDocumentDistributionService.PaySaleWaybillOnAccept(waybill, currentDateTime);

            if (waybill.IsPrepayment)
            {
                blockingService.CheckForBlocking(BlockingDependentOperation.AcceptPrePaymentExpenditureWaybill, waybill.Deal, waybill);
            }
            else
            {
                blockingService.CheckForBlocking(BlockingDependentOperation.AcceptPostPaymentExpenditureWaybill, waybill.Deal, waybill);
            }
        }

        /// <summary>
        /// Отмена проводки накладной
        /// </summary>
        /// <param name="waybill"></param>
        public void CancelAcceptance(ExpenditureWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - не появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);
            
            CheckPossibilityToCancelAcceptance(waybill, user);

            // отмена резервирования товара при проводке
            var reservationInfoList = articleMovementService.CancelArticleAcceptance(waybill);

            // Пересчет показателей входящего проведенного наличия
            articleAvailabilityService.ExpenditureWaybillAcceptanceCanceled(waybill, reservationInfoList,
                articleMovementService.GetIncomingWaybillRowForOutgoingWaybillRow(expenditureWaybillRepository.GetRowsSubQuery(waybill.Id)));

            // пересчет показателя реализации
            articleSaleService.ExpenditureWaybillAcceptanceCancelled(waybill);

            waybill.CancelAcceptance(UseReadyToAcceptState);

            dealPaymentDocumentDistributionService.CancelSaleWaybillPaymentDistribution(waybill);

            expenditureWaybillRepository.Save(waybill);

            // удаление связей и пересчет проведенной переоценки
            articleRevaluationService.ExpenditureWaybillAcceptanceCancelled(waybill);
        }
        #endregion

        #region Отгрузка / отмена отгрузки

        /// <summary>
        /// Отгрузка накладной
        /// </summary>
        /// <param name="waybill"></param>
        public void Ship(ExpenditureWaybill waybill, User shippedBy, DateTime currentDateTime)
        {
            // регулярная проверка - не появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);
            
            CheckPossibilityToShip(waybill, shippedBy);

            waybill.Ship(shippedBy, currentDateTime);

            articleMovementService.FinallyMoveAcceptedArticles(waybill);

            // Пересчет показателей проведенного исходящего и точного наличия
            articleAvailabilityService.ExpenditureWaybillShipped(waybill);

            // пересчет показателя реализации
            articleSaleService.ExpenditureWaybillShipped(waybill);
            
            // пересчет финансового показателя
            factualFinancialArticleMovementService.ExpenditureWaybillShipped(waybill);

            // пересчитываем счетчики количеств операций
            articleMovementOperationCountService.WaybillFinalized(waybill);

            if (!waybill.IsPrepayment && !waybill.IsFullyPaid)
            {
                blockingService.CheckForBlocking(BlockingDependentOperation.ShipUnpaidPostPaymentExpenditureWaybill, waybill.Deal, waybill);
            }

            // уменьшение показателя точной переоценки
            articleRevaluationService.ExpenditureWaybillFinalized(waybill);
        }

        /// <summary>
        /// Отгрузка накладной задним числом
        /// </summary>
        /// <param name="waybill">Накладная реализации</param>
        /// <param name="shippingDate">Дата, от которой нужно отгрузить накладную</param>
        /// <param name="currentDateTime">Текущая дата</param>
        public void ShipRetroactively(ExpenditureWaybill waybill, DateTime shippingDate, User shippedBy, DateTime currentDateTime)
        {
            CheckPossibilityToShipRetroactively(waybill, shippedBy);

            ValidationUtils.Assert(shippingDate < currentDateTime, "Время отгрузки накладной должно быть меньше текущего времени.");
            ValidationUtils.Assert(shippingDate > waybill.AcceptanceDate, 
                String.Format("Время отгрузки накладной должно быть больше времени ее проводки ({0}).", waybill.AcceptanceDate.Value.ToFullDateTimeString()));

            // при отгрузке накладной задним числом с предоплатой нужно, чтобы дата отгрузки была больше максимальной даты разнесения, а накладная была 100% оплачена.
            if (waybill.IsPrepayment)
            {
                // 100% оплата проверяется в методе CheckPossibilityToShip
                ValidationUtils.Assert(shippingDate > waybill.Distributions.Max(x => x.DistributionDate),
                    String.Format("Невозможно отгрузить накладную с предоплатой, т.к. на {0} недостаточно средств для оплаты. Доступно всего {1} руб.",
                    shippingDate.ToFullDateTimeString(), waybill.Distributions.Where(x => x.DistributionDate < shippingDate).Sum(x => x.Sum).ForDisplay(ValueDisplayType.Money)));
            }

            Ship(waybill, shippedBy, shippingDate);
        }

        /// <summary>
        /// Отмена отгрузки накладной
        /// </summary>
        /// <param name="waybill"></param>
        public void CancelShipping(ExpenditureWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - не появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);
            
            CheckPossibilityToCancelShipping(waybill, user);

            // Пересчет показателей проведенного исходящего и точного наличия
            articleAvailabilityService.ExpenditureWaybillShippingCanceled(waybill);

            // пересчет показателя реализации
            articleSaleService.ExpenditureWaybillShippingCancelled(waybill);

            // пересчет финансового показателя
            factualFinancialArticleMovementService.ExpenditureWaybillShippingCancelled(waybill);

            // пересчитываем счетчики количеств операций
            articleMovementOperationCountService.WaybillFinalizationCancelled(waybill);

            // увеличение показателя точной переоценки
            articleRevaluationService.ExpenditureWaybillFinalizationCancelled(waybill);

            waybill.CancelShipping();

            // помечаем товар как проведенный
            articleMovementService.CancelArticleFinalMoving(waybill);
        }

        #endregion

        #region Удаление накладной

        /// <summary>
        /// Удаление накладной
        /// </summary>
        /// <param name="waybill"></param>
        public void Delete(ExpenditureWaybill waybill, User user)
        {
            CheckPossibilityToDelete(waybill, user);

            // удаляем связи с установленными вручную источниками
            articleMovementService.ResetManualSources(waybill);

            expenditureWaybillRepository.Delete(waybill);
        }

        #endregion

        #region Добавление / удаление позиций

        /// <summary>
        /// Добавление позиции в накладную
        /// </summary>
        public virtual void AddRow(ExpenditureWaybill waybill, ExpenditureWaybillRow row, User user)
        {
            AddRowLocal(waybill, row, null, user);
        }

        /// <summary>
        /// Добавление позиции в накладную с ручным указанием источников
        /// </summary>
        public virtual void AddRow(ExpenditureWaybill waybill, ExpenditureWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
        {
            AddRowLocal(waybill, row, sourceDistributionInfo, user);
        }

        private void AddRowLocal(ExpenditureWaybill waybill, ExpenditureWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
        {
            CheckPossibilityToEdit(waybill, user);

            waybill.AddRow(row);

            expenditureWaybillRepository.Save(waybill);

            if (sourceDistributionInfo != null)
            {
                articleMovementService.SetManualSources(row, sourceDistributionInfo);
            }

            CheckBlockingOnRowAdding(waybill);
        }

        public void SaveRow(ExpenditureWaybill expenditureWaybill, ExpenditureWaybillRow expenditureWaybillRow, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
        {
            SaveRowLocal(expenditureWaybill, expenditureWaybillRow, sourceDistributionInfo, user);
        }

        public void SaveRow(ExpenditureWaybill expenditureWaybill, ExpenditureWaybillRow expenditureWaybillRow, User user)
        {
            SaveRowLocal(expenditureWaybill, expenditureWaybillRow, null, user);
        }

        private void SaveRowLocal(ExpenditureWaybill expenditureWaybill, ExpenditureWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
        {
            CheckPossibilityToEdit(expenditureWaybill, user);

            if (row.IsUsingManualSource)
            {
                articleMovementService.ResetManualSources(row);
            }

            if (sourceDistributionInfo != null)
            {
                articleMovementService.SetManualSources(row, sourceDistributionInfo);
            }

            expenditureWaybillRepository.Save(expenditureWaybill);

            CheckBlockingOnRowAdding(expenditureWaybill);
        }

        public void DeleteRow(ExpenditureWaybill waybill, ExpenditureWaybillRow row, User user)
        {
            CheckPossibilityToDeleteRow(row, user);

            if (row.IsUsingManualSource)
            {
                articleMovementService.ResetManualSources(row);
            }

            waybill.DeleteRow(row);
        }

        /// <summary>
        /// Упрощенное добавление позиции в накладную
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="article">Товар</param>
        /// <param name="count">Кол-во резервируемого товара</param>
        /// <param name="user">Пользователь</param>        
        public void AddRowSimply(ExpenditureWaybill waybill, Article article, decimal count, User user)
        {
            CheckPossibilityToEdit(waybill, user);

            CheckBlockingOnRowAdding(waybill);

            // распределяем кол-во товара по партиям
            var countDistributionInfo = DistributeCountByBatches(article, waybill.SenderStorage, waybill.Sender, count);

            var batchList = receiptWaybillService.GetRows(countDistributionInfo.Keys);
            foreach (var item in countDistributionInfo)
            {
                var batch = batchList[item.Key];
                var row = new ExpenditureWaybillRow(batch, item.Value, waybill.ValueAddedTax);

                AddRow(waybill, row, user);
            }
        }

        /// <summary>
        /// Проверка блокировок при добавлении/сохранении позиции накладной
        /// </summary>        
        private void CheckBlockingOnRowAdding(ExpenditureWaybill waybill)
        {
            if (waybill.IsPrepayment)
            {
                blockingService.CheckForBlocking(BlockingDependentOperation.SavePrePaymentExpenditureWaybillRow, waybill.Deal, waybill);
            }
            else
            {
                blockingService.CheckForBlocking(BlockingDependentOperation.SavePostPaymentExpenditureWaybillRow, waybill.Deal, waybill);
            }
        }

        #endregion
        
        #region Установка квоты

        /// <summary>
        /// Назначить накладной реализации товаров указанную квоту.
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров.</param>
        /// <param name="dealQuota">Квота.</param>
        /// <param name="user">Пользователь, выполняющий операцию.</param>
        public void SetDealQuota(ExpenditureWaybill waybill, DealQuota dealQuota, User user)
        {
            CheckPossibilityToEdit(waybill, user);

            if (waybill.Quota != dealQuota)
            {
                waybill.Quota = dealQuota;
            }

            waybill.IsPrepayment = dealQuota.IsPrepayment;
        }

        #endregion

        #region Проверки на возможность совершения операций

        #region Настройки аккаунта

        /// <summary>
        /// Разрешение использовать статус "Готово к проводке"
        /// </summary>
        private bool UseReadyToAcceptState
        {
            get { return settingRepository.Get().UseReadyToAcceptStateForExpenditureWaybill; }
        }

        #endregion

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(ExpenditureWaybill waybill, User user, Permission permission,
            bool checkPermissionByDeals = true, bool checkPermissionByStorages = true)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                    result = waybill.Deal.Curator == user &&
                        ((checkPermissionByStorages && user.Teams.SelectMany(x => x.Storages).Contains(waybill.SenderStorage)) || !checkPermissionByStorages) &&
                        ((checkPermissionByDeals && user.Teams.SelectMany(x => x.Deals).Contains(waybill.Deal)) || !checkPermissionByDeals); // свои + командные
                    break;

                case PermissionDistributionType.Teams:
                    result = ((checkPermissionByStorages && user.Teams.SelectMany(x => x.Storages).Contains(waybill.SenderStorage)) || !checkPermissionByStorages) &&
                        ((checkPermissionByDeals && user.Teams.SelectMany(x => x.Deals).Contains(waybill.Deal)) || !checkPermissionByDeals);
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(ExpenditureWaybill waybill, User user, Permission permission,
            bool checkPermissionByDeals = true, bool checkPermissionByStorages = true)
        {
            if (!IsPermissionToPerformOperation(waybill, user, permission, checkPermissionByDeals, checkPermissionByStorages))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #endregion

        #region Просмотр деталей

        public bool IsPossibilityToViewDetails(ExpenditureWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToViewDetails(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewDetails(ExpenditureWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_List_Details, checkPermissionByStorages: false);
        }

        #endregion

        #region Создание накладной реализации товаров

        public bool IsPossibilityToCreate(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToCreate(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreate(Deal deal, User user)
        {
            // права
            user.CheckPermission(Permission.ExpenditureWaybill_Create_Edit);

            dealService.CheckPossibilityToCreateExpenditureWaybill(deal, user);

            blockingService.CheckForBlocking(BlockingDependentOperation.CreateExpenditureWaybill, deal, null);
        }

        #endregion

        #region Редактирование

        public bool IsPossibilityToEdit(ExpenditureWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToEdit, waybill, user);
        }

        public void CheckPossibilityToEdit(ExpenditureWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Create_Edit);

            // сущность
            waybill.CheckPossibilityToEdit();
        }

        #endregion

        #region Редактирование даты накладной

        public bool IsPossibilityToChangeDate(ExpenditureWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToChangeDate, waybill, user);
        }

        public void CheckPossibilityToChangeDate(ExpenditureWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Date_Change);

            // сущность
            waybill.CheckPossibilityToChangeDate();
        }

        #endregion

        #region Подготовка к проводке

        public bool IsPossibilityToPrepareToAccept(ExpenditureWaybill waybill, User user, bool onlyPermission = false)
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

        public void CheckPossibilityToPrepareToAccept(ExpenditureWaybill waybill, User user, bool onlyPermission = false)
        {
            // настройки аккаунта
            ValidationUtils.Assert(UseReadyToAcceptState, "Невозможно выполнить операцию, т.к. опция подготовки накладной к проводке отключена в настройках аккаунта.");

            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Create_Edit);

            // Сделано для корректного отображения кнопки на форме
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

        public bool IsPossibilityToCancelReadinessToAccept(ExpenditureWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToCancelReadinessToAccept, waybill, user);            
        }

        public void CheckPossibilityToCancelReadinessToAccept(ExpenditureWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Create_Edit);

            // сущность
            waybill.CheckPossibilityToCancelReadinessToAccept();
        }

        #endregion

        #region Проводка

        public bool IsPossibilityToAccept(ExpenditureWaybill waybill, User user, bool onlyPermission = false)
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

        public void CheckPossibilityToAccept(ExpenditureWaybill waybill, User user, bool onlyPermission = false)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Accept_Deal_List, checkPermissionByDeals: true, checkPermissionByStorages: false);
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Accept_Storage_List, checkPermissionByDeals: false, checkPermissionByStorages: true);

            // Сделано для корректного отображения кнопки на форме
            if (!onlyPermission)
            {
                // сущность
                waybill.CheckPossibilityToAccept(UseReadyToAcceptState);
            }
            else
            {
                // При проверке прав необходимо убедиться, что "следующий шаг" для накладной проводка.
                ValidationUtils.Assert(UseReadyToAcceptState ? waybill.IsReadyToAccept : waybill.IsNew,
                    String.Format("Невозможно провести накладную из состояния «{0}».", waybill.State.GetDisplayName()));
            }
        }

        #endregion

        #region Проводка задним числом

        public bool IsPossibilityToAcceptRetroactively(ExpenditureWaybill waybill, User user, bool onlyPermission = false)
        {
            try
            {
                CheckPossibilityToAcceptRetroactively(waybill, user, onlyPermission);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAcceptRetroactively(ExpenditureWaybill waybill, User user, bool onlyPermission = false)
        {
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Accept_Retroactively);
            CheckPossibilityToAccept(waybill, user, onlyPermission);
        }

        #endregion

        #region Отмена проводки

        public bool IsPossibilityToCancelAcceptance(ExpenditureWaybill waybill, User user)
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

        public void CheckPossibilityToCancelAcceptance(ExpenditureWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Cancel_Acceptance_Deal_List, checkPermissionByDeals: true, checkPermissionByStorages: false);
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Cancel_Acceptance_Storage_List, checkPermissionByDeals: false, checkPermissionByStorages: true);

            // сущность
            waybill.CheckPossibilityToCancelAcceptance();
        }

        #endregion

        #region Отгрузка

        public bool IsPossibilityToShip(ExpenditureWaybill waybill, User user, bool onlyPermission = false)
        {
            try
            {
                CheckPossibilityToShip(waybill, user, onlyPermission);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToShip(ExpenditureWaybill waybill, User user, bool onlyPermission = false)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Ship_Deal_List, checkPermissionByDeals: true, checkPermissionByStorages: false);
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Ship_Storage_List, checkPermissionByDeals: false, checkPermissionByStorages: true);

            if (!onlyPermission)
            {
                // сущность
                waybill.CheckPossibilityToShip();
            }
            else
            {
                ValidationUtils.Assert(waybill.State == ExpenditureWaybillState.ReadyToShip,
                    String.Format("Невозможно отгрузить товар по накладной со статусом «{0}».", waybill.State.GetDisplayName()));
            }
        }

        #endregion

        #region Отгрузка задним числом

        public bool IsPossibilityToShipRetroactively(ExpenditureWaybill waybill, User user, bool onlyPermission = false)
        {
            try
            {
                CheckPossibilityToShipRetroactively(waybill, user, onlyPermission);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToShipRetroactively(ExpenditureWaybill waybill, User user, bool onlyPermission = false)
        {
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Ship_Retroactively);
            CheckPossibilityToShip(waybill, user, onlyPermission);
        }

        #endregion

        #region Отмена отгрузки

        public bool IsPossibilityToCancelShipping(ExpenditureWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToCancelShipping(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCancelShipping(ExpenditureWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Cancel_Shipping_Deal_List, checkPermissionByDeals: true, checkPermissionByStorages: false);
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Cancel_Shipping_Storage_List, checkPermissionByDeals: false, checkPermissionByStorages: true);

            // сущность
            waybill.CheckPossibilityToCancelShipping();

            // проверка наличия возвратов по реализации
            ValidationUtils.Assert(!returnFromClientWaybillRepository.AreReturnFromClientWaybillRowsForSaleWaybillRows(expenditureWaybillRepository.GetRowsSubQuery(waybill.Id)),
                "Невозможно отменить отгрузку реализации, т.к. по ней имеются возвраты.");
        }

        #endregion

        #region Удаление

        public bool IsPossibilityToDelete(ExpenditureWaybill waybill, User user)
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

        public void CheckPossibilityToDelete(ExpenditureWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Delete_Row_Delete);

            // сущность
            waybill.CheckPossibilityToDelete();
        }

        #endregion

        #region Удаление позиции

        public bool IsPossibilityToDeleteRow(ExpenditureWaybillRow row, User user)
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

        public void CheckPossibilityToDeleteRow(ExpenditureWaybillRow row, User user)
        {
            // права
            CheckPermissionToPerformOperation(row.ExpenditureWaybill, user, Permission.ExpenditureWaybill_Delete_Row_Delete);

            // сущность
            row.ExpenditureWaybill.CheckPossibilityToDeleteRow();
            row.CheckPossibilityToDelete();
        }

        #endregion

        #region Просмотр суммы возвратов

        public bool IsPossibilityToViewReturnsFromClient(ExpenditureWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToViewReturnsFromClient(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewReturnsFromClient(ExpenditureWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReturnFromClientWaybill_List_Details, checkPermissionByStorages: false);
        }

        #endregion

        #region Просмотр суммы оплат и сумм по ним

        public bool IsPossibilityToViewPayments(ExpenditureWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToViewPayments(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewPayments(ExpenditureWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.DealPayment_List_Details, checkPermissionByStorages: false);
        }

        #endregion

        #region Возможность печатать формы документов

        public bool IsPossibilityToPrintForms(ExpenditureWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintForms, waybill, user);
        }

        public void CheckPossibilityToPrintForms(ExpenditureWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_List_Details, checkPermissionByStorages: false);

            // сущность
            ValidationUtils.Assert(waybill.IsAccepted, "Невозможно распечатать форму, т.к. накладная еще не проведена.");
        }

        #region Печать форм в отпускных ценах

        public bool IsPossibilityToPrintFormInSalePrices(ExpenditureWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintFormInSalePrices, waybill, user);
        }

        public void CheckPossibilityToPrintFormInSalePrices(ExpenditureWaybill waybill, User user)
        {
            CheckPossibilityToPrintForms(waybill, user);

            ValidationUtils.Assert(user.HasPermissionToViewStorageAccountingPrices(waybill.SenderStorage),
                "Недостаточно прав для просмотра отпускных цен.");
        }
        #endregion

        #region Печать форм в ЗЦ

        public bool IsPossibilityToPrintFormInPurchaseCosts(ExpenditureWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintFormInPurchaseCosts, waybill, user);
        }

        public void CheckPossibilityToPrintFormInPurchaseCosts(ExpenditureWaybill waybill, User user)
        {
            CheckPossibilityToPrintForms(waybill, user);

            ValidationUtils.Assert(user.HasPermission(Permission.PurchaseCost_View_ForEverywhere),
                "Недостаточно прав для просмотра закупочных цен.");
        }
        #endregion

        #endregion

        #region Редактирование команды, к которой относится накладная

        public bool IsPossibilityToEditTeam(ExpenditureWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToEditTeam, waybill, user);            
        }

        public void CheckPossibilityToEditTeam(ExpenditureWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Create_Edit, checkPermissionByStorages: false);

            // редактировать команду может только куратор накладной
            ValidationUtils.Assert(waybill.Curator == user, 
                "Изменить команду, к которой относится накладная реализации, может только куратор накладной.");

            // сущность
            waybill.CheckPossibilityToEditTeam();
        }

        #endregion

        #region Смена куратора

        public override void CheckPossibilityToChangeCurator(ExpenditureWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ExpenditureWaybill_Curator_Change);

            // сущность
            waybill.CheckPossibilityToChangeCurator();
        }
        #endregion

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получение статусов позиций накладной
        /// </summary>
        /// <param name="waybill"></param>
        /// <returns></returns>
        public IDictionary<Guid, OutgoingWaybillRowState> GetRowStates(ExpenditureWaybill waybill)
        {
            var result = new Dictionary<Guid, OutgoingWaybillRowState>();

            IEnumerable<ArticleBatchAvailabilityShortInfo> articleBatchAvailability = null;

            // если накладная не проведена, то для позиций без ручного указания источников 
            // необходимо найти точное наличие
            if (!waybill.IsAccepted)
            {
                // создаем подзапрос для партий для позиций без ручного указания источников
                var rowWithoutManualSourceBatchSubQuery = expenditureWaybillRepository.GetRowWithoutManualSourceBatchSubquery(waybill.Id);

                // получаем доступное для резервирования кол-во по партиям
                articleBatchAvailability = articleAvailabilityService.GetAvailableToReserveFromExactArticleAvailability(
                    rowWithoutManualSourceBatchSubQuery, waybill.SenderStorage.Id, waybill.Sender.Id, DateTime.Now);
            }

            // вычисляем статусы всех позиций
            foreach (var row in waybill.Rows.Select(x => x as ExpenditureWaybillRow))
            {
                var outgoingWaybillRowState = OutgoingWaybillRowState.ArticlePending;

                // если накладная проведена или по позиции известны источники - то статус уже известен
                if (waybill.IsAccepted || row.IsUsingManualSource)
                {
                    outgoingWaybillRowState = row.OutgoingWaybillRowState;
                }
                // если накладная не проведена и источники для позиции не указаны
                else
                {
                    // находим точное наличие по партии
                    var availability = articleBatchAvailability.Where(x => x.BatchId == row.ReceiptWaybillRow.Id).FirstOrDefault();

                    outgoingWaybillRowState = (availability != null && availability.Count >= row.SellingCount) ?
                        OutgoingWaybillRowState.ReadyToArticleMovement : OutgoingWaybillRowState.ArticlePending;
                }

                result.Add(row.Id, outgoingWaybillRowState);
            }

            return result;
        }

        #endregion

        #endregion
    }
}
