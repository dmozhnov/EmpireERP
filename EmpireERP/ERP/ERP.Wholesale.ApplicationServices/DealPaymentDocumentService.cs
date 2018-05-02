using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Settings;
using ERP.Infrastructure.Repositories.Criteria;

namespace ERP.Wholesale.ApplicationServices
{
    public class DealPaymentDocumentService : IDealPaymentDocumentService
    {
        #region Поля

        private readonly IDealPaymentDocumentRepository dealPaymentDocumentRepository;
        private readonly IDealPaymentRepository dealPaymentRepository;
        private readonly IDealInitialBalanceCorrectionRepository dealInitialBalanceCorrectionRepository;
        private readonly IDealDebitInitialBalanceCorrectionRepository dealDebitInitialBalanceCorrectionRepository;
        private readonly IDealRepository dealRepository;
        private readonly IExpenditureWaybillRepository expenditureWaybillRepository;
        private readonly ISaleWaybillRepository saleWaybillRepository;
        private readonly ITeamRepository teamRepository;
        private readonly IUserRepository userRepository;

        private readonly IDealPaymentDocumentDistributionService dealPaymentDocumentDistributionService;
        private readonly IExpenditureWaybillService expenditureWaybillService;
        private readonly ISaleWaybillIndicatorService saleWaybillIndicatorService;
        private readonly IDealIndicatorService dealIndicatorService;
        private readonly IClientContractIndicatorService clientContractIndicatorService;

        #endregion

        #region Конструкторы

        public DealPaymentDocumentService(IDealPaymentDocumentDistributionService dealPaymentDocumentDistributionService,
            IExpenditureWaybillService expenditureWaybillService,
            ISaleWaybillIndicatorService saleWaybillIndicatorService, IDealIndicatorService dealIndicatorService,
            IDealPaymentDocumentRepository dealPaymentDocumentRepository, IDealPaymentRepository dealPaymentRepository,
            IDealInitialBalanceCorrectionRepository dealInitialBalanceCorrectionRepository,
            IDealDebitInitialBalanceCorrectionRepository dealDebitInitialBalanceCorrectionRepository,
            IExpenditureWaybillRepository expenditureWaybillRepository, ISaleWaybillRepository saleWaybillRepository, IDealRepository dealRepository, IClientContractIndicatorService clientContractIndicatorService,
            ITeamRepository teamRepository, IUserRepository userRepository)
        {
            this.dealPaymentDocumentDistributionService = dealPaymentDocumentDistributionService;
            this.expenditureWaybillService = expenditureWaybillService;
            this.saleWaybillIndicatorService = saleWaybillIndicatorService;
            this.dealIndicatorService = dealIndicatorService;

            this.dealPaymentDocumentRepository = dealPaymentDocumentRepository;
            this.dealPaymentRepository = dealPaymentRepository;
            this.dealInitialBalanceCorrectionRepository = dealInitialBalanceCorrectionRepository;
            this.dealDebitInitialBalanceCorrectionRepository = dealDebitInitialBalanceCorrectionRepository;
            this.dealRepository = dealRepository;
            this.expenditureWaybillRepository = expenditureWaybillRepository;
            this.saleWaybillRepository = saleWaybillRepository;
            this.teamRepository = teamRepository;
            this.userRepository = userRepository;
            this.clientContractIndicatorService = clientContractIndicatorService;
        }

        #endregion

        #region Методы

        #region Получение платежного документа по id с проверкой его существования

        /// <summary>
        /// Получение платежного документа по id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private DealPaymentDocument GetById(Guid id, Permission permission, User user)
        {
            var dealPaymentDocument = dealPaymentDocumentRepository.GetById(id);
            if (dealPaymentDocument == null)
            {
                return dealPaymentDocument;
            }

            // В зависимости от типа платежного документа выбираем распространение нужного права
            PermissionDistributionType type = user.GetPermissionDistributionType(permission);

            if (type == PermissionDistributionType.All)
            {
                return dealPaymentDocument;
            }
            else
            {
                var userTeamsContainDeal = user.Teams.SelectMany(x => x.Deals).Contains(dealPaymentDocument.Deal);

                if ((type == PermissionDistributionType.Personal && dealPaymentDocument.Deal.Curator == user && userTeamsContainDeal) ||
                    (type == PermissionDistributionType.Teams && userTeamsContainDeal))
                {
                    return dealPaymentDocument;
                }
            }

            return null;
        }

        /// <summary>
        /// Получение платежного документа по id с проверкой его существования
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private DealPaymentDocument CheckDealPaymentDocumentExistence(Guid id, Permission permission, User user, string message = "")
        {
            var dealPaymentDocument = GetById(id, permission, user);
            ValidationUtils.NotNull(dealPaymentDocument, String.IsNullOrEmpty(message) ? "Платежный документ не найден. Возможно, он был удален." : message);

            return dealPaymentDocument;
        }

        /// <summary>
        /// Получение оплаты от клиента по сделке с проверкой ее существования
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public DealPaymentFromClient CheckDealPaymentFromClientExistence(Guid id, User user, string message = "")
        {
            string errorMessage = "Оплата от клиента по сделке не найдена. Возможно, она была удалена.";
            var dealPaymentFromClient = CheckDealPaymentDocumentExistence(id, Permission.DealPayment_List_Details, user, errorMessage);
            ValidationUtils.Assert(dealPaymentFromClient.Is<DealPaymentFromClient>(), errorMessage);

            return dealPaymentFromClient.As<DealPaymentFromClient>();
        }

        /// <summary>
        /// Получение возврата оплаты клиенту по сделке с проверкой его существования
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public DealPaymentToClient CheckDealPaymentToClientExistence(Guid id, User user, string message = "")
        {
            string errorMessage = "Возврат оплаты клиенту по сделке не найден. Возможно, он был удален.";
            var dealPaymentToClient = CheckDealPaymentDocumentExistence(id, Permission.DealPayment_List_Details, user, errorMessage);
            ValidationUtils.Assert(dealPaymentToClient.Is<DealPaymentToClient>(), errorMessage);

            return dealPaymentToClient.As<DealPaymentToClient>();
        }

        /// <summary>
        /// Получение кредитовой корректировки сальдо по сделке с проверкой ее существования
        /// </summary>        
        public DealCreditInitialBalanceCorrection CheckDealCreditInitialBalanceCorrectionExistence(Guid id, User user, string message = "")
        {
            string errorMessage = "Кредитовая корректировка сальдо по сделке не найдена. Возможно, она была удалена.";
            var dealCreditInitialBalanceCorrection = CheckDealPaymentDocumentExistence(id, Permission.DealInitialBalanceCorrection_List_Details, user, errorMessage);
            ValidationUtils.Assert(dealCreditInitialBalanceCorrection.Is<DealCreditInitialBalanceCorrection>(), errorMessage);

            return dealCreditInitialBalanceCorrection.As<DealCreditInitialBalanceCorrection>();
        }

        /// <summary>
        /// Получение дебетовой корректировки сальдо по сделке с проверкой ее существования
        /// </summary>        
        public DealDebitInitialBalanceCorrection CheckDealDebitInitialBalanceCorrectionExistence(Guid id, User user, string message = "")
        {
            string errorMessage = !String.IsNullOrEmpty(message) ? message : "Дебетовая корректировка сальдо по сделке не найдена. Возможно, она была удалена.";
            var dealDebitInitialBalanceCorrection = CheckDealPaymentDocumentExistence(id, Permission.DealInitialBalanceCorrection_List_Details, user, errorMessage);
            ValidationUtils.Assert(dealDebitInitialBalanceCorrection.Is<DealDebitInitialBalanceCorrection>(), errorMessage);

            return dealDebitInitialBalanceCorrection.As<DealDebitInitialBalanceCorrection>();
        }

        /// <summary>
        /// Получение списка дебетовых корректировок сальдо по сделке с проверкой их существования
        /// </summary>        
        private IDictionary<Guid, DealDebitInitialBalanceCorrection> CheckDealDebitInitialBalanceCorrectionExistence(IEnumerable<Guid> idList, User user, string message = "")
        {
            if (idList.Count() == 0)
            {
                return new Dictionary<Guid, DealDebitInitialBalanceCorrection>();
            }

            var dealSubQuery = dealIndicatorService.RestrictByUserPermissions(user, Permission.DealInitialBalanceCorrection_List_Details);
            var result = dealSubQuery != null ?
                dealDebitInitialBalanceCorrectionRepository.GetById(idList, dealSubQuery) :
                new Dictionary<Guid, DealDebitInitialBalanceCorrection>();

            ValidationUtils.Assert(idList.Distinct().Count() == result.Count,
                !String.IsNullOrEmpty(message) ? message : "Дебетовая корректировка сальдо по сделке не найдена. Возможно, она была удалена.");

            return result;
        }

        /// <summary>
        /// Получить список оплат и возвратов оплат, дата которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientList">Список клиентов</param>
        public IDictionary<Guid, DealPayment> GetDealPaymentListInDateRangeByClientList(DateTime startDate, DateTime endDate, IEnumerable<Client> clientList)
        {
            return dealPaymentRepository.GetListInDateRangeByClientList(startDate, endDate, clientList.Select(x => x.Id).ToList(), null);
        }

        /// <summary>
        /// Получить список оплат и возвратов оплат, дата которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationList">Список организаций клиента</param>
        public IDictionary<Guid, DealPayment> GetDealPaymentListInDateRangeByClientOrganizationList(DateTime startDate, DateTime endDate,
            IEnumerable<ClientOrganization> clientOrganizationList)
        {
            return dealPaymentRepository.GetListInDateRangeByClientOrganizationList(startDate, endDate, clientOrganizationList.Select(x => x.Id).ToList(), null);
        }

        /// <summary>
        /// Получить список корректировок сальдо, дата которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientList">Список клиентов</param>
        public IDictionary<Guid, DealInitialBalanceCorrection> GetDealInitialBalanceCorrectionListInDateRangeByClientList(DateTime startDate, DateTime endDate,
            IEnumerable<Client> clientList)
        {
            return dealInitialBalanceCorrectionRepository.GetListInDateRangeByClientList(startDate, endDate, clientList.Select(x => x.Id).ToList());
        }

        /// <summary>
        /// Получить список корректировок сальдо, дата которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationList">Список организаций клиента</param>
        public IDictionary<Guid, DealInitialBalanceCorrection> GetDealInitialBalanceCorrectionListInDateRangeByClientOrganizationList(DateTime startDate,
            DateTime endDate, IEnumerable<ClientOrganization> clientOrganizationList)
        {
            return dealInitialBalanceCorrectionRepository.GetListInDateRangeByClientOrganizationList(startDate, endDate, clientOrganizationList.Select(x => x.Id).ToList());
        }

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю оплат и возвратов оплат, принадлежащих данным клиентам и командам,
        /// дата которых находится в диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов. Null - все клиенты</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="user">Пользователь</param>
        public IDictionary<Guid, DealPayment> GetDealPaymentListInDateRangeByClientAndTeamList(DateTime startDate, DateTime endDate, IEnumerable<int> clientIdList, 
            IEnumerable<short> teamIdList, User user)
        {
            return GetDealPaymentListInDateRangeByDealSubQuery(
                dealRepository.GetDealSubQueryOnAllPermissionByClient(clientIdList),
                dealRepository.GetDealSubQueryOnTeamPermissionByClientList(clientIdList, user.Id),
                dealRepository.GetDealSubQueryOnPersonalPermissionByClientList(clientIdList, user.Id),
                teamIdList, null, startDate, endDate, user);
        }

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю оплат и возвратов оплат, принадлежащих данным организациям клиентов и командам,
        /// дата которых находится в диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиентов. Null - все организации клиентов</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="user">Пользователь</param>
        public IDictionary<Guid, DealPayment> GetDealPaymentListInDateRangeByClientOrganizationAndTeamList(DateTime startDate, DateTime endDate, IEnumerable<int> clientOrganizationIdList,
            IEnumerable<short> teamIdList, User user)
        {
            return GetDealPaymentListInDateRangeByDealSubQuery(
                dealRepository.GetDealSubQueryOnAllPermissionByClientOrganizationList(clientOrganizationIdList),
                dealRepository.GetDealSubQueryOnTeamPermissionByClientOrganizationList(clientOrganizationIdList, user.Id),
                dealRepository.GetDealSubQueryOnPersonalPermissionByClientOrganizationList(clientOrganizationIdList, user.Id),
                teamIdList, null, startDate, endDate, user);
        }

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю оплат и возвратов оплат, принадлежащих данным клиентам, собственным ораганизациям, командам и пользователям,
        /// дата которых находится в диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов. Null - все клиенты</param>
        /// <param name="accountOrganizationIdList">Список кодов собственных организаций. Null - все собственные организации</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="userIdList">Список кодов пользователей. Null - все пользователи</param>
        /// <param name="user">Пользователь</param>
        public IDictionary<Guid, DealPayment> GetDealPaymentListInDateRangeByClientAndTeamAndAccountOrganizationList(DateTime startDate, DateTime endDate, IEnumerable<int> clientIdList,
            IEnumerable<int> accountOrganizationIdList, IEnumerable<short> teamIdList, IEnumerable<int> userIdList, User user)
        {
            return GetDealPaymentListInDateRangeByDealSubQuery(
                dealRepository.GetDealSubQueryOnAllPermissionByClientAndAccountOrganizationList(clientIdList, accountOrganizationIdList),
                dealRepository.GetDealSubQueryOnTeamPermissionByClientAndAccountOrganizationList(clientIdList, accountOrganizationIdList, user.Id),
                dealRepository.GetDealSubQueryOnPersonalPermissionByClientAndAccountOrganizationList(clientIdList, accountOrganizationIdList, user.Id),
                teamIdList, userIdList, startDate, endDate, user);
        }

        /// <summary>
        /// Получить список оплат и возвратов оплат, дата проводки которых находится в диапазоне дат, по подзапросу сделок
        /// </summary>
        /// <param name="allDealSubQuery">Подкритерий, ограничивающий множество сделок (без учета видимости)</param>
        /// <param name="teamDealSubQuery">Подкритерий, ограничивающий множество сделок (командная видимость)</param>
        /// <param name="personalDealSubQuery">Подкритерий, ограничивающий множество сделок (персональная видимость)</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="userIdList">Список кодов пользователей. Null - все пользователи</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="user">Пользователь</param>
        private IDictionary<Guid, DealPayment> GetDealPaymentListInDateRangeByDealSubQuery(ISubQuery allDealSubQuery, ISubQuery teamDealSubQuery, ISubQuery personalDealSubQuery,
            IEnumerable<short> teamIdList, IEnumerable<int> userIdList, DateTime startDate, DateTime endDate, User user)
        {
            var dealSubQuery = dealIndicatorService.GetDealSubQueryByPermissionDistribution(allDealSubQuery, teamDealSubQuery, personalDealSubQuery,
                Permission.DealPayment_List_Details, user);

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

            ISubCriteria<User> userSubQuery = null;
            switch (user.GetPermissionDistributionType(Permission.User_List_Details))
            {
                case PermissionDistributionType.All:
                    userSubQuery = userRepository.GetUserSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                case PermissionDistributionType.Personal:
                    userSubQuery = userRepository.GetUserSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    userSubQuery = userRepository.GetUserSubQueryByNonePermission();
                    break;
            }

            return dealSubQuery != null ? dealPaymentRepository.GetListInDateRangeByDealSubQuery(startDate, endDate, dealSubQuery, teamIdList, teamSubQuery, userIdList, userSubQuery) :
                new Dictionary<Guid, DealPayment>();
        }

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю корректировок сальдо, принадлежащих данным клиентам и командам,
        /// дата проводки которых находится в диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов. Null - все клиенты</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="user">Пользователь</param>
        public IDictionary<Guid, DealInitialBalanceCorrection> GetDealInitialBalanceCorrectionListInDateRangeByClientAndTeamList(DateTime startDate, DateTime endDate, 
            IEnumerable<int> clientIdList, IEnumerable<short> teamIdList, User user)
        {
            return GetDealInitialBalanceCorrectionListInDateRangeByDealSubQuery(
                dealRepository.GetDealSubQueryOnAllPermissionByClient(clientIdList),
                dealRepository.GetDealSubQueryOnTeamPermissionByClientList(clientIdList, user.Id),
                dealRepository.GetDealSubQueryOnPersonalPermissionByClientList(clientIdList, user.Id),
                teamIdList, startDate, endDate, user);
        }

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю корректировок сальдо, принадлежащих данным организациям клиентов и командам,
        /// дата проводки которых находится в диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиентов. Null - все организации клиентов</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="user">Пользователь</param>
        public IDictionary<Guid, DealInitialBalanceCorrection> GetDealInitialBalanceCorrectionListInDateRangeByClientOrganizationAndTeamList(DateTime startDate, DateTime endDate,
            IEnumerable<int> clientOrganizationIdList, IEnumerable<short> teamIdList, User user)
        {
            return GetDealInitialBalanceCorrectionListInDateRangeByDealSubQuery(
                dealRepository.GetDealSubQueryOnAllPermissionByClientOrganizationList(clientOrganizationIdList),
                dealRepository.GetDealSubQueryOnTeamPermissionByClientOrganizationList(clientOrganizationIdList, user.Id),
                dealRepository.GetDealSubQueryOnPersonalPermissionByClientOrganizationList(clientOrganizationIdList, user.Id),
                teamIdList, startDate, endDate, user);
        }

        /// <summary>
        /// Получить список корректировок сальдо, дата проводки которых находится в диапазоне дат, по подзапросу сделок
        /// </summary>
        /// <param name="allDealSubQuery">Подкритерий, ограничивающий множество сделок (без учета видимости)</param>
        /// <param name="teamDealSubQuery">Подкритерий, ограничивающий множество сделок (командная видимость)</param>
        /// <param name="personalDealSubQuery">Подкритерий, ограничивающий множество сделок (персональная видимость)</param>
        /// <param name="teamIdList">Список кодов команд, корректировки по которым нужно учесть. null - учитываются все</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="user">Пользователь</param>
        private IDictionary<Guid, DealInitialBalanceCorrection> GetDealInitialBalanceCorrectionListInDateRangeByDealSubQuery(ISubQuery allDealSubQuery, ISubQuery teamDealSubQuery, 
            ISubQuery personalDealSubQuery, IEnumerable<short> teamIdList, DateTime startDate, DateTime endDate, User user)
        {
            var dealSubQuery = dealIndicatorService.GetDealSubQueryByPermissionDistribution(allDealSubQuery, teamDealSubQuery, personalDealSubQuery,
                Permission.DealInitialBalanceCorrection_List_Details, user);

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

            return dealSubQuery != null ? dealInitialBalanceCorrectionRepository.GetListInDateRangeByDealSubQuery(startDate, endDate, dealSubQuery, teamIdList, teamSubQuery) :
                new Dictionary<Guid, DealInitialBalanceCorrection>();
        }

        #endregion

        #region Список

        /// <summary>
        /// Получение отфильтрованного списка оплат по сделке
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public IEnumerable<DealPayment> GetDealPaymentFilteredList(object state, User user)
        {
            return GetDealPaymentFilteredList(state, new ParameterString(""), user);
        }

        /// <summary>
        /// Получение отфильтрованного списка оплат по сделке
        /// </summary>
        /// <param name="state"></param>
        /// <param name="param"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public IEnumerable<DealPayment> GetDealPaymentFilteredList(object state, ParameterString param, User user)
        {
            Func<ISubCriteria<DealPayment>, ISubCriteria<DealPayment>> cond = null;
            
            switch (user.GetPermissionDistributionType(Permission.DealPayment_List_Details))
            {
                case PermissionDistributionType.None:
                    return new List<DealPayment>();

                case PermissionDistributionType.Personal:
                    cond = x => x.PropertyIn(y => y.Deal.Id, dealRepository.GetSubQueryForDealIdOnPersonalPermission(user.Id)).Select(y => y.Id);
                    break;

                case PermissionDistributionType.Teams:
                    cond = x => x.PropertyIn(y => y.Deal.Id, dealRepository.GetSubQueryForDealIdOnTeamPermission(user.Id)).Select(y => y.Id);
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            return dealPaymentRepository.GetFilteredList(state, param, cond: cond);
        }

        /// <summary>
        /// Получение отфильтрованного списка оплат по сделке (без проверки прав)
        /// </summary>        
        public IEnumerable<DealPayment> GetDealPaymentFilteredList(object state, ParameterString param)
        {
            return dealPaymentRepository.GetFilteredList(state, param);
        }

        /// <summary>
        /// Получение отфильтрованного списка корректировок сальдо по сделке
        /// </summary>        
        public IEnumerable<DealInitialBalanceCorrection> GetDealInitialBalanceCorrectionFilteredList(object state, User user)
        {
            return GetDealInitialBalanceCorrectionFilteredList(state, new ParameterString(""), user);
        }

        /// <summary>
        /// Получение отфильтрованного списка корректировок сальдо по сделке
        /// </summary>        
        public IEnumerable<DealInitialBalanceCorrection> GetDealInitialBalanceCorrectionFilteredList(object state, ParameterString param, User user)
        {
            Func<ISubCriteria<DealInitialBalanceCorrection>, ISubCriteria<DealInitialBalanceCorrection>> cond = null;
            
            switch (user.GetPermissionDistributionType(Permission.DealInitialBalanceCorrection_List_Details))
            {
                case PermissionDistributionType.None:
                    return new List<DealInitialBalanceCorrection>();

                case PermissionDistributionType.Personal:
                    cond = x => x.PropertyIn(y => y.Deal.Id, dealRepository.GetSubQueryForDealIdOnPersonalPermission(user.Id)).Select(y => y.Id);
                    break;

                case PermissionDistributionType.Teams:
                    cond = x => x.PropertyIn(y => y.Deal.Id, dealRepository.GetSubQueryForDealIdOnTeamPermission(user.Id)).Select(y => y.Id);
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            return dealInitialBalanceCorrectionRepository.GetFilteredList(state, param, cond: cond);
        }
               

        /// <summary>
        /// Получить список не полностью оплаченных дебетовых корректировок сальдо по списку сделок
        /// (отсортированные по дате, затем по дате создания по возрастанию)
        /// </summary>
        /// <param name="dealList">Список сделок</param>
        /// <param name="team">Команда</param>
        /// <returns>Список дебетовых корректировок сальдо</returns>
        public IEnumerable<DealDebitInitialBalanceCorrection> GetDealDebitInitialBalanceCorrectionListForDealPaymentDocumentDistribution(IEnumerable<Deal> dealList, Team team)
        {
            return dealDebitInitialBalanceCorrectionRepository.GetDealDebitInitialBalanceCorrectionListForDealPaymentDocumentDistribution(dealList.Select(x => x.Id), team.Id);
        }

        #endregion

        #region Создание и разнесение

        /// <summary>
        /// Создание и разнесение оплаты от клиента по организации клиента. Реально создается по одной оплате по каждой сделке с одинаковыми параметрами, кроме сумм
        /// </summary>
        /// <param name="clientOrganization">Организация клиента, по которой разносится оплата</param>
        /// <param name="team">Команда, для которой создается оплата</param>
        /// <param name="paymentDocumentNumber">Номер платежного документа (параметр оплаты)</param>
        /// <param name="date">Дата (параметр оплаты)</param>
        /// <param name="sum">Сумма оплаты</param>
        /// <param name="dealPaymentForm">Форма оплаты (параметр оплаты)</param>
        /// <param name="dealPaymentDocumentDistributionInfoList">Информация о разнесении создаваемой оплаты. Оплата должна быть разнесена полностью</param>
        /// <param name="createdBy">Пользователь, вносящий оплату в систему</param>
        /// <param name="takenBy">Пользователь, принявший оплату</param>
        /// <param name="currentDate">Дата операции</param>
        public void CreateClientOrganizationPaymentFromClient(ClientOrganization clientOrganization, Team team, string paymentDocumentNumber, DateTime date,
            decimal sum, DealPaymentForm dealPaymentForm, IEnumerable<DealPaymentDocumentDistributionInfo> dealPaymentDocumentDistributionInfoList,
            User createdBy, User takenBy, DateTime currentDate)
        {
            decimal undistributedSumForOrganization = sum;

            foreach (Deal deal in dealPaymentDocumentDistributionInfoList.Select(x => x.Deal).Distinct())
            {
                // Выбираем информацию обо всех накладных по данной сделке
                var concreteDealPaymentDocumentDistributionInfoList = dealPaymentDocumentDistributionInfoList.Where(x => x.Deal == deal);

                decimal sumByDeal = concreteDealPaymentDocumentDistributionInfoList.Sum(x => x.Sum);

                var dealPaymentFromClient = new DealPaymentFromClient(team, takenBy, paymentDocumentNumber, date, sumByDeal, dealPaymentForm, currentDate);
                
                CheckPossibilityToCreateDealPaymentFromClient(deal, createdBy, dealPaymentFromClient);
                
                deal.AddDealPaymentDocument(dealPaymentFromClient);

                dealPaymentDocumentRepository.Save(dealPaymentFromClient);

                // Разносим оплату по одной сделке
                dealPaymentDocumentDistributionService.DistributeDealPaymentFromClient(dealPaymentFromClient, concreteDealPaymentDocumentDistributionInfoList, currentDate);

                undistributedSumForOrganization -= sumByDeal;
            }

            ValidationUtils.Assert(undistributedSumForOrganization <= 0,
                String.Format("При разнесении оплаты по организации клиента невозможно оставить неразнесенный остаток. Неразнесенная сумма оплаты: {0} р.",
                undistributedSumForOrganization.ForDisplay(ValueDisplayType.Money)));

            ValidationUtils.Assert(undistributedSumForOrganization >= 0,
                String.Format("Сумма для разнесения по организации клиента ({0} р.) превышает сумму оплаты ({1} р.).",
                (sum - undistributedSumForOrganization).ForDisplay(ValueDisplayType.Money), sum.ForDisplay(ValueDisplayType.Money)));
        }

        /// <summary>
        /// Создание и разнесение оплаты от клиента по сделке
        /// </summary>
        /// <param name="deal">Сделка, по которой разносится оплата</param>
        /// <param name="team">Команда, для которой создается оплата от клиента</param>
        /// <param name="paymentDocumentNumber">Номер платежного документа (параметр оплаты)</param>
        /// <param name="date">Дата (параметр оплаты)</param>
        /// <param name="sum">Сумма оплаты</param>
        /// <param name="dealPaymentForm">Форма оплаты (параметр оплаты)</param>
        /// <param name="dealPaymentDocumentDistributionInfoList">Информация о разнесении создаваемой оплаты</param>
        /// <param name="createdBy">Пользователь, вносящий оплату в систему</param>
        /// <param name="takenBy">Пользователь, принявший оплату</param>
        /// <param name="currentDate">Дата операции</param>
        public void CreateDealPaymentFromClient(Deal deal, Team team, string paymentDocumentNumber, DateTime date, decimal sum, DealPaymentForm dealPaymentForm,
            IEnumerable<DealPaymentDocumentDistributionInfo> dealPaymentDocumentDistributionInfoList, User createdBy, User takenBy, DateTime currentDate)
        {
            var dealPaymentFromClient = new DealPaymentFromClient(team, takenBy, paymentDocumentNumber, date, sum, dealPaymentForm, currentDate);
            CheckPossibilityToCreateDealPaymentFromClient(deal, createdBy, dealPaymentFromClient);
            deal.AddDealPaymentDocument(dealPaymentFromClient);

            dealPaymentDocumentDistributionService.DistributeDealPaymentFromClient(dealPaymentFromClient,
                dealPaymentDocumentDistributionInfoList, currentDate);
        }

        /// <summary>
        /// Создание и разнесение возврата оплаты от клиента по сделке
        /// </summary>
        /// <param name="deal">Сделка, по которой разносится оплата</param>
        /// <param name="team">Команда, для которой создается оплата</param>
        /// <param name="paymentDocumentNumber">Номер платежного документа (параметр оплаты)</param>
        /// <param name="date">Дата (параметр оплаты)</param>
        /// <param name="sum">Сумма оплаты</param>
        /// <param name="dealPaymentForm">Форма оплаты (параметр оплаты)</param>
        /// <param name="createdBy">Пользователь, вносящий оплату в систему</param>
        /// <param name="returnedBy">Пользователь, вернувший оплату</param>
        /// <param name="currentDate">Дата операции</param>
        public void CreateDealPaymentToClient(Deal deal, Team team, string paymentDocumentNumber, DateTime date, decimal sum, DealPaymentForm dealPaymentForm,
            User createdBy, User returnedBy, DateTime currentDate)
        {
            CheckPossibilityToCreateDealPaymentToClient(deal, createdBy);

            var indicators = dealIndicatorService.CalculateMainIndicators(deal, team, calculateBalance: true);
            if (-indicators.Balance < sum)
            {
                // Если мы ничего не должны клиенту, то выводим 0. Иначе выводим сумму нашего долга перед клиентом
                var debtSum = Math.Max(0M, -indicators.Balance);
                throw new Exception(String.Format("Сумма возврата оплаты клиенту не может быть больше задолженности перед клиентом по команде ({0})",
                    debtSum.ForDisplay(ValueDisplayType.Money) + " р."));
            }

            var dealPaymentToClient = new DealPaymentToClient(team, returnedBy, paymentDocumentNumber, date, sum, dealPaymentForm, currentDate);
            deal.AddDealPaymentDocument(dealPaymentToClient);

            dealPaymentDocumentDistributionService.DistributeDealPaymentToClient(dealPaymentToClient, currentDate);
        }

        /// <summary>
        /// Создание и разнесение кредитовой корректировки сальдо по сделке
        /// </summary>
        /// <param name="deal">Сделка, по которой разносится кредитовая корректировка сальдо</param>
        /// <param name="team">Команда, для которой создается корректировка сальдо</param>
        /// <param name="correctionReason">Причина корректировки (параметр корректировки сальдо)</param>
        /// <param name="date">Дата (параметр корректировки сальдо)</param>
        /// <param name="sum">Сумма корректировки сальдо</param>
        /// <param name="dealPaymentDocumentDistributionInfoList">Информация о разнесении создаваемой корректировки сальдо</param>
        /// <param name="createdBy">Пользователь, вносящий корректировку сальдо в систему</param>
        /// <param name="takenBy">Пользователь, принявший корректировку сальдо</param>
        /// <param name="currentDate">Дата операции</param>
        public void CreateDealCreditInitialBalanceCorrection(Deal deal, Team team, string correctionReason, DateTime date, decimal sum,
            IEnumerable<DealPaymentDocumentDistributionInfo> dealPaymentDocumentDistributionInfoList, User createdBy, User takenBy, DateTime currentDate)
        {
            CheckPossibilityToCreateDealCreditInitialBalanceCorrection(deal, createdBy);

            var dealCreditInitialBalanceCorrection = new DealCreditInitialBalanceCorrection(team, takenBy, correctionReason, date, sum, currentDate);
            deal.AddDealPaymentDocument(dealCreditInitialBalanceCorrection);

            dealPaymentDocumentDistributionService.DistributeDealCreditInitialBalanceCorrection(dealCreditInitialBalanceCorrection,
                dealPaymentDocumentDistributionInfoList, currentDate);
        }

        /// <summary>
        /// Создание и разнесение дебетовой корректировки сальдо по сделке
        /// </summary>
        /// <param name="deal">Сделка, по которой разносится дебетовая корректировка сальдо</param>
        /// <param name="team">Команда, для которой создается корректировка сальдо</param>
        /// <param name="correctionReason">Причина корректировки (параметр корректировки сальдо)</param>
        /// <param name="date">Дата (параметр корректировки сальдо)</param>
        /// <param name="sum">Сумма корректировки сальдо</param>
        /// <param name="createdBy">Пользователь, вносящий корректировку сальдо в систему</param>
        /// <param name="returnedBy">Пользователь, вернувший корректировку сальдо</param>
        /// <param name="currentDate">Дата операции</param>
        public void CreateDealDebitInitialBalanceCorrection(Deal deal, Team team, string correctionReason, DateTime date, decimal sum,
            User createdBy, User returnedBy, DateTime currentDate)
        {
            CheckPossibilityToCreateDealDebitInitialBalanceCorrection(deal, createdBy);

            var dealDebitInitialBalanceCorrection = new DealDebitInitialBalanceCorrection(team, returnedBy, correctionReason, date, sum, currentDate);
            deal.AddDealPaymentDocument(dealDebitInitialBalanceCorrection);

            dealPaymentDocumentDistributionService.DistributeDealDebitInitialBalanceCorrection(dealDebitInitialBalanceCorrection, currentDate);
        }

        #endregion

        #region Переразнесение

        /// <summary>
        /// Переразнесение оплаты от клиента по сделке
        /// </summary>
        /// <param name="dealPaymentFromClient">Оплата от клиента</param>
        /// <param name="dealPaymentDocumentDistributionInfoList">Информация о разнесении оплаты</param>
        /// <param name="user">Пользователь</param>
        /// <param name="currentDate">Дата операции</param>
        public void RedistributeDealPaymentFromClient(DealPaymentFromClient dealPaymentFromClient,
            IEnumerable<DealPaymentDocumentDistributionInfo> dealPaymentDocumentDistributionInfoList, User user, DateTime currentDate)
        {
            CheckPossibilityToRedistribute(dealPaymentFromClient, user);

            dealPaymentDocumentDistributionService.DistributeDealPaymentFromClient(dealPaymentFromClient,
                dealPaymentDocumentDistributionInfoList, currentDate);
        }

        /// <summary>
        /// Переразнесение кредитовой корректировки сальдо по сделке
        /// </summary>
        /// <param name="dealCreditInitialBalanceCorrection">Кредитовая корректировка сальдо</param>
        /// <param name="dealPaymentDocumentDistributionInfoList">Информация о разнесении корректировки</param>
        /// <param name="user">Пользователь</param>
        /// <param name="currentDate">Дата операции</param>
        public void RedistributeDealCreditInitialBalanceCorrection(DealCreditInitialBalanceCorrection dealCreditInitialBalanceCorrection,
            IEnumerable<DealPaymentDocumentDistributionInfo> dealPaymentDocumentDistributionInfoList, User user, DateTime currentDate)
        {
            CheckPossibilityToRedistribute(dealCreditInitialBalanceCorrection, user);

            dealPaymentDocumentDistributionService.DistributeDealCreditInitialBalanceCorrection(dealCreditInitialBalanceCorrection,
                dealPaymentDocumentDistributionInfoList, currentDate);
        }
        
        #endregion

        #region Удаление

        /// <summary>
        /// Удаление оплаты от клиента
        /// </summary>
        /// <param name="dealPaymentFromClient"></param>
        /// <param name="user"></param>
        public void Delete(DealPaymentFromClient dealPaymentFromClient, User user, DateTime currentDate)
        {
            CheckPossibilityToDelete(dealPaymentFromClient, user);

            // фиксируем список накладных реализации, связанных с данным платежным документом
            var saleWaybillList = GetSaleWaybillListByDealPaymentDocument(dealPaymentFromClient);

            DeleteDealPaymentDocumentDistributionList(dealPaymentDocumentRepository
                .GetDealPaymentDocumentDistributionListForSourceDealPaymentDocument(dealPaymentFromClient.Id));

            dealPaymentFromClient.Deal.DeleteDealPaymentDocument(dealPaymentFromClient, currentDate);

            // сохраняем удаление разнесений в БД
            dealPaymentDocumentRepository.Save(dealPaymentFromClient);

            UpdateSaleWaybillFullyPaidAttribute(saleWaybillList);
        }

        /// <summary>
        /// Удаление возврата оплаты клиенту
        /// </summary>
        /// <param name="dealPaymentToClient"></param>
        /// <param name="user"></param>
        public void Delete(DealPaymentToClient dealPaymentToClient, User user, DateTime currentDate)
        {
            CheckPossibilityToDelete(dealPaymentToClient, user);

            DeleteDealPaymentDocumentDistributionList(dealPaymentDocumentRepository
                .GetDealPaymentDocumentDistributionListForDestinationDealPaymentDocument(dealPaymentToClient.Id));

            dealPaymentToClient.Deal.DeleteDealPaymentDocument(dealPaymentToClient, currentDate);
        }

        /// <summary>
        /// Удаление кредитовой корректировки сальдо
        /// </summary>        
        public void Delete(DealCreditInitialBalanceCorrection dealCreditInitialBalanceCorrection, User user, DateTime currentDate)
        {
            CheckPossibilityToDelete(dealCreditInitialBalanceCorrection, user);

            // фиксируем список накладных реализации, связанных с данным платежным документом
            var saleWaybillList = GetSaleWaybillListByDealPaymentDocument(dealCreditInitialBalanceCorrection);

            DeleteDealPaymentDocumentDistributionList(dealPaymentDocumentRepository
                .GetDealPaymentDocumentDistributionListForSourceDealPaymentDocument(dealCreditInitialBalanceCorrection.Id));

            dealCreditInitialBalanceCorrection.Deal.DeleteDealPaymentDocument(dealCreditInitialBalanceCorrection, currentDate);

            // сохраняем удаление разнесений в БД
            dealPaymentDocumentRepository.Save(dealCreditInitialBalanceCorrection);

            UpdateSaleWaybillFullyPaidAttribute(saleWaybillList);
        }

        /// <summary>
        /// Удаление дебетовой корректировки сальдо
        /// </summary>        
        public void Delete(DealDebitInitialBalanceCorrection dealDebitInitialBalanceCorrection, User user, DateTime currentDate)
        {
            CheckPossibilityToDelete(dealDebitInitialBalanceCorrection, user);

            DeleteDealPaymentDocumentDistributionList(dealPaymentDocumentRepository
                .GetDealPaymentDocumentDistributionListForDestinationDealPaymentDocument(dealDebitInitialBalanceCorrection.Id));

            dealDebitInitialBalanceCorrection.Deal.DeleteDealPaymentDocument(dealDebitInitialBalanceCorrection, currentDate);
        }

        /// <summary>
        /// Обновление флага полной оплаты для накладных реализации
        /// </summary>
        /// <param name="dealPaymentDocument"></param>
        private void UpdateSaleWaybillFullyPaidAttribute(IEnumerable<SaleWaybill> saleWaybillList)
        {
            foreach (var saleWaybill in saleWaybillList)
            {
                var salePriceSumWithoutReturns = saleWaybill.As<ExpenditureWaybill>().SalePriceSum - 
                    saleWaybillIndicatorService.GetTotalReturnedSumForSaleWaybill(saleWaybill);

                // сумма оплат по накладной реализации за минусом суммы возвратов
                var paymentSumWithoutReturns = saleWaybillRepository.CalculatePaymentSum(saleWaybill.Id);

                saleWaybill.IsFullyPaid = paymentSumWithoutReturns >= salePriceSumWithoutReturns;
            }
        }

        /// <summary>
        /// Получение списка накладных реализации, на которые разнесен данный платежный документ 
        /// </summary>
        private IEnumerable<SaleWaybill> GetSaleWaybillListByDealPaymentDocument(DealPaymentDocument dealPaymentDocument)
        {
            return dealPaymentDocument.Distributions.Where(x => x.Is<DealPaymentDocumentDistributionToSaleWaybill>())
                .Select(x => x.As<DealPaymentDocumentDistributionToSaleWaybill>().SaleWaybill).ToList();
        }

        /// <summary>
        /// Удаление списка разнесений платежных документов из всех сущностей, где они фигурируют
        /// </summary>
        /// <param name="dealPaymentDocumentDistributionList"></param>
        private void DeleteDealPaymentDocumentDistributionList(IEnumerable<DealPaymentDocumentDistribution> dealPaymentDocumentDistributionList)
        {
            // начинаем с удаления разнесения с максимальной суммой, т.е. отрицательные разнесения удаляем последними
            foreach (var dealPaymentDocumentDistribution in dealPaymentDocumentDistributionList.OrderByDescending(x => x.Sum))
            {
                // Вызываемый виртуальный метод удаляет разнесение из коллекций обоих сущностей
                dealPaymentDocumentDistribution.SourceDealPaymentDocument.RemoveDealPaymentDocumentDistribution(dealPaymentDocumentDistribution);
            }
        }

        #endregion

        #region Разбор строки с информацией о разнесении оплаты или корректировки сальдо

        /// <summary>
        /// Преобразовать информацию о разнесении оплаты или корректировки сальдо из строкового во внутренний формат.
        /// При этом рассчитать неоплаченные остатки всех накладных реализации
        /// </summary>
        /// <param name="distributionInfo">Строка с информацией о разнесении оплаты или корректировки сальдо</param>
        /// <returns>Список элементов с внутренней информацией об оплате или корректировке сальдо (в виде сущностей)</returns>
        public IEnumerable<DealPaymentDocumentDistributionInfo> ParseDealPaymentDocumentDistributionInfo(string distributionInfo, User user)
        {
            var result = new List<DealPaymentDocumentDistributionInfo>();

            var entityInfoList = distributionInfo.Split(';');
            for (int i = 0; i < entityInfoList.Length - 1; i++)
            {
                string entityInfo = entityInfoList[i];

                var entityInfoParams = entityInfo.Split(new char[] { '=', '_' });

                DealPaymentDocumentDistributionInfo dealPaymentDocumentDistributionInfo = new DealPaymentDocumentDistributionInfo();
                dealPaymentDocumentDistributionInfo.Id = ValidationUtils.TryGetGuid(entityInfoParams[0]);
                dealPaymentDocumentDistributionInfo.OrdinalNumber = ValidationUtils.TryGetInt(entityInfoParams[1]);
                dealPaymentDocumentDistributionInfo.Sum = Math.Round(ValidationUtils.TryGetDecimal(entityInfoParams[2]), 2);

                // Тип документа: 1 - накладная реализации товаров, 2 - дебетовая корректировка сальдо сделки
                var type = (DealPaymentDocumentDistributionType)ValidationUtils.TryGetByte(entityInfoParams[3]);
                ValidationUtils.Assert(Enum.IsDefined(typeof(DealPaymentDocumentDistributionType), type), "Неизвестный тип документа для разнесения.");
                dealPaymentDocumentDistributionInfo.Type = type;

                result.Add(dealPaymentDocumentDistributionInfo);
            }

            // Загружаем накладные реализации товаров (с проверкой видимости для данного пользователя)
            var expenditureWaybillDistributionInfoList = result.Where(x => x.Type == DealPaymentDocumentDistributionType.ExpenditureWaybill);
            var expenditureWaybillDictionary = expenditureWaybillService.CheckWaybillExistence(
                expenditureWaybillDistributionInfoList.Select(x => x.Id), user);

            // Рассчитываем неоплаченные остатки по накладным реализации (когда будет несколько типов, передавать сюда слияние списков)
            var debtRemainderList = saleWaybillIndicatorService.CalculateDebtRemainderList(expenditureWaybillDictionary.Values);
            ValidationUtils.Assert(debtRemainderList.Count == expenditureWaybillDistributionInfoList.Count(),
                "Не для всех накладных реализации удалось рассчитать неоплаченный остаток.");

            // Записываем прочитанные накладные и их неоплаченные остатки в информацию о разнесении
            foreach (var expenditureWaybillDistributionInfo in expenditureWaybillDistributionInfoList)
            {
                expenditureWaybillDistributionInfo.SaleWaybill = expenditureWaybillDictionary[expenditureWaybillDistributionInfo.Id];
                expenditureWaybillDistributionInfo.SaleWaybillDebtRemainder = debtRemainderList[expenditureWaybillDistributionInfo.SaleWaybill.Id];
            }

            // Загружаем дебетовые корректировки сальдо (с проверкой видимости для данного пользователя)
            var dealDebitInitialBalanceCorrectionDistributionInfoList = result.Where(x => x.Type == DealPaymentDocumentDistributionType.DealDebitInitialBalanceCorrection);
            var dealDebitInitialBalanceCorrectionDictionary = CheckDealDebitInitialBalanceCorrectionExistence(
                dealDebitInitialBalanceCorrectionDistributionInfoList.Select(x => x.Id), user);

            // Записываем прочитанные дебетовые корректировки сальдо в информацию о разнесении
            foreach (var dealDebitInitialBalanceCorrectionDistributionInfo in dealDebitInitialBalanceCorrectionDistributionInfoList)
            {
                dealDebitInitialBalanceCorrectionDistributionInfo.DealDebitInitialBalanceCorrection =
                    dealDebitInitialBalanceCorrectionDictionary[dealDebitInitialBalanceCorrectionDistributionInfo.Id];
            }

            return result;
        }

        #endregion

        #region Подгрузка платежных документов (для уменьшения количества запросов)

        /// <summary>
        /// Подгрузить все коллекции разнесений для списка платежных документов
        /// </summary>
        /// <param name="dealPaymentDocumentList">Список платежных документов</param>
        public void LoadDealPaymentDocumentDistributions(IEnumerable<DealPaymentDocument> dealPaymentDocumentList)
        {
            dealPaymentDocumentRepository.LoadDealPaymentDocumentDistributions(dealPaymentDocumentList);
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

        private void CheckPermissionToPerformOperation(DealPaymentDocument dealPaymentDocument, User user, Permission permission)
        {
            CheckPermissionToPerformOperation(dealPaymentDocument.Deal, user, permission);
        }

        #endregion        

        #region Просмотр платежных документов

        #region Просмотр корректировок

        public bool IsPossibilityToViewDealInitialBalanceCorrections(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToViewDealInitialBalanceCorrections(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CheckPossibilityToViewDealInitialBalanceCorrections(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.DealInitialBalanceCorrection_List_Details);
        }

        #endregion

        #endregion

        #region Создание платежных документов

        #region Добавление оплаты от клиента

        public bool IsPossibilityToCreateDealPaymentFromClient(Deal deal, User user, DealPaymentFromClient dealPaymentFromClient = null)
        {
            try
            {
                CheckPossibilityToCreateDealPaymentFromClient(deal, user, dealPaymentFromClient);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateDealPaymentFromClient(Deal deal, User user, DealPaymentFromClient dealPaymentFromClient = null)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.DealPaymentFromClient_Create_Edit);

            if (dealPaymentFromClient != null && dealPaymentFromClient.DealPaymentForm == DealPaymentForm.Cash)
            {
                var maxCashExcess = clientContractIndicatorService.CalculateCashPaymentLimitExcessByPaymentsFromClient(deal.Contract) + dealPaymentFromClient.Sum;

                ValidationUtils.Assert(maxCashExcess <= 0,
                    String.Format("Невозможно добавить новую оплату, так как максимально допустимая сумма наличных расчетов ({0} р.) по договору «{1}» превышена на {2} р.",
                    AppSettings.MaxCashPaymentSum.ForDisplay(ValueDisplayType.Money), deal.Contract.FullName, maxCashExcess.ForDisplay(ValueDisplayType.Money)));
            }

            // сущность
            deal.CheckPossibilityToCreateDealPaymentFromClient();
        }

        #endregion

        #region Добавление возврата оплаты клиенту

        public bool IsPossibilityToCreateDealPaymentToClient(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToCreateDealPaymentToClient(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateDealPaymentToClient(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.DealPaymentToClient_Create);

            // сущность
            deal.CheckPossibilityToCreateDealPaymentToClient();
        }

        #endregion

        #region Смена пользователя, принявшего оплату от клиента

        public bool IsPossibilityToChangeTakenByInDealPaymentFromClient(DealPaymentFromClient dealPaymentFromClient, User user)
        {
            try
            {
                CheckPossibilityToChangeTakenByInDealPaymentFromClient(dealPaymentFromClient, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToChangeTakenByInDealPaymentFromClient(DealPaymentFromClient dealPaymentFromClient, User user)
        {
            // права
            CheckPermissionToPerformOperation(dealPaymentFromClient.Deal, user, Permission.DealPayment_User_Change);
        }

        #endregion

        #region Смена пользователя, вернувшего оплату клиенту

        public bool IsPossibilityToChangeReturnedByInDealPaymentToClient(DealPaymentToClient dealPaymentToClient, User user)
        {
            try
            {
                CheckPossibilityToChangeReturnedByInDealPaymentToClient(dealPaymentToClient, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToChangeReturnedByInDealPaymentToClient(DealPaymentToClient dealPaymentToClient, User user)
        {
            // права
            CheckPermissionToPerformOperation(dealPaymentToClient.Deal, user, Permission.DealPayment_User_Change);
        }

        #endregion

        #region Добавление кредитовой корректировки сальдо

        public bool IsPossibilityToCreateDealCreditInitialBalanceCorrection(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToCreateDealCreditInitialBalanceCorrection(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateDealCreditInitialBalanceCorrection(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.DealCreditInitialBalanceCorrection_Create_Edit);

            // сущность
            deal.CheckPossibilityToCreateDealCreditInitialBalanceCorrection();
        }

        #endregion

        #region Добавление дебетовой корректировки сальдо

        public bool IsPossibilityToCreateDealDebitInitialBalanceCorrection(Deal deal, User user)
        {
            try
            {
                CheckPossibilityToCreateDealDebitInitialBalanceCorrection(deal, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateDealDebitInitialBalanceCorrection(Deal deal, User user)
        {
            // права
            CheckPermissionToPerformOperation(deal, user, Permission.DealDebitInitialBalanceCorrection_Create);

            // сущность
            deal.CheckPossibilityToCreateDealDebitInitialBalanceCorrection();
        }

        #endregion

        #endregion

        #region Перераспределение платежных документов

        public bool IsPossibilityToRedistribute(DealPaymentFromClient dealPaymentFromClient, User user)
        {
            try
            {
                CheckPossibilityToRedistribute(dealPaymentFromClient, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToRedistribute(DealPaymentFromClient dealPaymentFromClient, User user)
        {
            // права
            CheckPermissionToPerformOperation(dealPaymentFromClient, user, Permission.DealPaymentFromClient_Create_Edit);

            dealPaymentFromClient.CheckPossibilityToRedistribute();
        }

        #region Перераспределение корректировок сальдо по сделке

        public bool IsPossibilityToRedistribute(DealCreditInitialBalanceCorrection dealCreditInitialBalanceCorrection, User user)
        {
            try
            {
                CheckPossibilityToRedistribute(dealCreditInitialBalanceCorrection, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToRedistribute(DealCreditInitialBalanceCorrection dealCreditInitialBalanceCorrection, User user)
        {
            // права
            CheckPermissionToPerformOperation(dealCreditInitialBalanceCorrection, user, Permission.DealCreditInitialBalanceCorrection_Create_Edit);

            // сущность
            dealCreditInitialBalanceCorrection.CheckPossibilityToRedistribute();
        }

        #endregion

        #endregion

        #region Удаление платежных документов

        #region Удаление оплаты от клиента

        public bool IsPossibilityToDelete(DealPaymentFromClient dealPaymentFromClient, User user)
        {
            try
            {
                CheckPossibilityToDelete(dealPaymentFromClient, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CheckPossibilityToDelete(DealPaymentFromClient dealPaymentFromClient, User user)
        {
            // права
            CheckPermissionToPerformOperation(dealPaymentFromClient, user, Permission.DealPaymentFromClient_Delete);

            // сущность
            dealPaymentFromClient.Deal.CheckPossibilityToDeleteDealPaymentFromClient();
            dealPaymentFromClient.CheckPossibilityToDelete();
        }

        #endregion

        #region Удаление возврата оплаты клиенту

        public bool IsPossibilityToDelete(DealPaymentToClient dealPaymentToClient, User user)
        {
            try
            {
                CheckPossibilityToDelete(dealPaymentToClient, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CheckPossibilityToDelete(DealPaymentToClient dealPaymentToClient, User user)
        {
            // права
            CheckPermissionToPerformOperation(dealPaymentToClient, user, Permission.DealPaymentToClient_Delete);

            // сущность
            dealPaymentToClient.Deal.CheckPossibilityToDeleteDealPaymentToClient();
            dealPaymentToClient.CheckPossibilityToDelete();
        }

        #endregion

        #region Удаление кредитовой корректировки сальдо по сделке

        public bool IsPossibilityToDelete(DealCreditInitialBalanceCorrection dealCreditInitialBalanceCorrection, User user)
        {
            try
            {
                CheckPossibilityToDelete(dealCreditInitialBalanceCorrection, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CheckPossibilityToDelete(DealCreditInitialBalanceCorrection dealCreditInitialBalanceCorrection, User user)
        {
            // права
            CheckPermissionToPerformOperation(dealCreditInitialBalanceCorrection, user, Permission.DealCreditInitialBalanceCorrection_Delete);

            // сущность
            dealCreditInitialBalanceCorrection.Deal.CheckPossibilityToDeleteDealCreditInitialBalanceCorrection();
            dealCreditInitialBalanceCorrection.CheckPossibilityToDelete();
        }

        #endregion

        #region Удаление дебетовой корректировки сальдо по сделке

        public bool IsPossibilityToDelete(DealDebitInitialBalanceCorrection dealDebitInitialBalanceCorrection, User user)
        {
            try
            {
                CheckPossibilityToDelete(dealDebitInitialBalanceCorrection, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CheckPossibilityToDelete(DealDebitInitialBalanceCorrection dealDebitInitialBalanceCorrection, User user)
        {
            // права
            CheckPermissionToPerformOperation(dealDebitInitialBalanceCorrection, user, Permission.DealDebitInitialBalanceCorrection_Delete);

            // сущность
            dealDebitInitialBalanceCorrection.Deal.CheckPossibilityToDeleteDealDebitInitialBalanceCorrection();
            dealDebitInitialBalanceCorrection.CheckPossibilityToDelete();
        }

        #endregion

        #region Удаление корректировки (общего типа) сальдо по сделке

        public bool IsPossibilityToDelete(DealInitialBalanceCorrection correction, User user)
        {
            if (correction.Is<DealDebitInitialBalanceCorrection>())
            {
                return IsPossibilityToDelete(correction.As<DealDebitInitialBalanceCorrection>(), user);
            }

            if (correction.Is<DealCreditInitialBalanceCorrection>())
            {
                return IsPossibilityToDelete(correction.As<DealCreditInitialBalanceCorrection>(), user);
            }

            throw new Exception("Неизвестный тип корректировки сальдо.");
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }
}