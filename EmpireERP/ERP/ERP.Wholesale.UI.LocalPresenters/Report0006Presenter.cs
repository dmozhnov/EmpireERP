using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Report.Report0006;
using OfficeOpenXml;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class Report0006Presenter : BaseReportPresenter, IReport0006Presenter
    {
        #region Классы

        /// <summary>
        /// Класс с информацией о договоре (название клиента, полное название договора, номер договора,
        /// название договора, краткие имена организаций договора)
        /// </summary>
        private class ClientContractInfo
        {
            /// <summary>
            /// Название клиента
            /// </summary>
            public string ClientName { get; protected set; }

            /// <summary>
            /// Полное название договора
            /// </summary>
            public string ClientContractFullName { get; protected set; }

            /// <summary>
            /// Номер договора
            /// </summary>
            public string ClientContractNumber { get; protected set; }

            /// <summary>
            /// Название договора
            /// </summary>
            public string ClientContractName { get; protected set; }

            /// <summary>
            /// Краткое название собственной организации
            /// </summary>
            public string AccountOrganizationShortName { get; protected set; }

            /// <summary>
            /// Краткое название организации клиента
            /// </summary>
            public string ClientOrganizationShortName { get; protected set; }

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="clientName">Название клиента</param>
            /// <param name="clientContractFullName">Полное название договора</param>
            /// <param name="clientContractNumber">Номер договора</param>
            /// <param name="clientContractName">Название договора</param>
            /// <param name="accountOrganizationShortName">AccountOrganizationShortName</param>
            /// <param name="clientOrganizationShortName">ClientOrganizationShortName</param>
            public ClientContractInfo(string clientName, string clientContractFullName, string clientContractNumber, string clientContractName,
                string accountOrganizationShortName, string clientOrganizationShortName)
            {
                ClientName = clientName;
                ClientContractFullName = clientContractFullName;
                ClientContractNumber = clientContractNumber;
                ClientContractName = clientContractName;
                AccountOrganizationShortName = accountOrganizationShortName;
                ClientOrganizationShortName = clientOrganizationShortName;
            }
        }

        /// <summary>
        /// Класс с информацией о документе по балансу сделки (для конвертации в строку отчета / печатной формы и сортировки)
        /// </summary>
        private class BalanceDocumentFullInfo
        {
            /// <summary>
            /// Дата создания
            /// </summary>
            public DateTime CreationDate { get; protected set; }

            /// <summary>
            /// Дата документа / проводки
            /// </summary>
            public DateTime Date { get; protected set; }

            /// <summary>
            /// Строка с описанием документа
            /// </summary>
            public string Name { get; protected set; }

            /// <summary>
            /// Строка с дополнительной информацией (вторая строка названия)
            /// </summary>
            public string AdditionalInfo1 { get; set; }

            /// <summary>
            /// Строка с дополнительной информацией (третья строка названия)
            /// </summary>
            public string AdditionalInfo2 { get; set; }

            /// <summary>
            /// Дебет
            /// </summary>
            public decimal Debit { get; protected set; }

            /// <summary>
            /// Кредит
            /// </summary>
            public decimal Credit { get; protected set; }

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="creationDate">Дата создания</param>
            /// <param name="date">Дата документа / проводки</param>
            /// <param name="name">Строка с описанием документа</param>
            /// <param name="sum">Сумма</param>
            /// <param name="isDebit">Попадает ли сумма в столбец "Дебит"</param>
            public BalanceDocumentFullInfo(DateTime creationDate, DateTime date, decimal sum, bool isDebit, string name, IList<string> additionalInfo = null)
            {
                CreationDate = creationDate;
                Date = date.Date;
                Name = name;
                AdditionalInfo1 = additionalInfo != null && additionalInfo.Count() > 0 ? additionalInfo[0] : "";
                AdditionalInfo2 = additionalInfo != null && additionalInfo.Count() > 1 ? additionalInfo[1] : "";
                if (isDebit)
                {
                    Debit = sum;
                }
                else
                {
                    Credit = sum;
                }
            }
        }

        /// <summary>
        /// Тип группировки в 6 отчете
        /// </summary>
        private enum GroupingType : byte
        {
            /// <summary>
            /// По клиентам
            /// </summary>
            [EnumDisplayName("Клиент")]
            Client = 1,

            /// <summary>
            /// По организациям клиентов
            /// </summary>
            [EnumDisplayName("Организация клиента")]
            ClientOrganization = 2,

            /// <summary>
            /// По договорам
            /// </summary>
            [EnumDisplayName("Договор")]
            ClientContract = 3,

            /// <summary>
            /// По командам
            /// </summary>
            [EnumDisplayName("Команда")]
            Team = 4
        }

        /// <summary>
        /// Класс с информацией, необходимой для расчета таблиц 4 и 5
        /// </summary>
        private class Report0006Info
        {
            /// <summary>
            /// Вычислять ли таблицу 4 ("Общая информация по взаиморасчетам")
            /// </summary>
            public bool ShowBalanceDocumentSummary { get; private set; }

            /// <summary>
            /// Вычислять ли таблицу 5 ("Развернутая информация по документам учета")
            /// </summary>
            public bool ShowBalanceDocumentFullInfo { get; private set; }

            /// <summary>
            /// Модель отчета
            /// </summary>
            public Report0006ViewModel Model { get; private set; }

            /// <summary>
            /// Начальная дата построения отчета
            /// </summary>
            public DateTime StartDate { get; private set; }

            /// <summary>
            /// Конечная дата построения отчета
            /// </summary>
            public DateTime EndDate { get; private set; }

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="showBalanceDocumentSummary">Вычислять ли таблицу 4 ("Общая информация по взаиморасчетам")</param>
            /// <param name="showBalanceDocumentFullInfo">Вычислять ли таблицу 5 ("Развернутая информация по документам учета")</param>
            /// <param name="model">Модель отчета</param>
            /// <param name="startDate">Начальная дата построения отчета</param>
            /// <param name="endDate">Конечная дата построения отчета</param>
            public Report0006Info(bool showBalanceDocumentSummary, bool showBalanceDocumentFullInfo, Report0006ViewModel model, DateTime startDate, DateTime endDate)
            {
                ShowBalanceDocumentSummary = showBalanceDocumentSummary;
                ShowBalanceDocumentFullInfo = showBalanceDocumentFullInfo;
                Model = model;
                StartDate = startDate;
                EndDate = endDate;
            }
        }

        #endregion

        #region Поля
                
        private readonly IClientService clientService;
        private readonly IDealIndicatorService dealIndicatorService;
        private readonly IExpenditureWaybillService expenditureWaybillService;
        private readonly IReturnFromClientWaybillService returnFromClientWaybillService;
        private readonly IDealPaymentDocumentService dealPaymentDocumentService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly IClientOrganizationService clientOrganizationService;
        private readonly ITeamService teamService;
        private readonly IClientContractService clientContractService;
        private readonly IDealService dealService;

        #endregion

        #region Конструкторы

        public Report0006Presenter(IUnitOfWorkFactory unitOfWorkFactory, IClientService clientService, IDealIndicatorService dealIndicatorService,
            IExpenditureWaybillService expenditureWaybillService, IReturnFromClientWaybillService returnFromClientWaybillService, IAccountOrganizationService accountOrganizationService,
            IClientOrganizationService clientOrganizationService, IDealPaymentDocumentService dealPaymentDocumentService, IUserService userService,
            ITeamService teamService, IClientContractService clientContractService, IDealService dealService)
            :base(unitOfWorkFactory, userService)
        {
            this.clientService = clientService;
            this.dealIndicatorService = dealIndicatorService;
            this.expenditureWaybillService = expenditureWaybillService;
            this.returnFromClientWaybillService = returnFromClientWaybillService;
            this.accountOrganizationService = accountOrganizationService;
            this.clientOrganizationService = clientOrganizationService;
            this.dealPaymentDocumentService = dealPaymentDocumentService;
            this.teamService = teamService;
            this.clientContractService = clientContractService;
            this.dealService = dealService;
        }

        #endregion

        #region Методы

        #region Настройки отчета / печатной формы

        /// <summary>
        /// Создание настроек 6 отчета
        /// </summary>
        /// <param name="currentUser">Пользователь</param>
        /// <param name="backURL">Адрес возврата</param>
        public Report0006SettingsViewModel Report0006Settings(string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var model = new Report0006SettingsViewModel()
                {
                    BackURL = backURL,
                    StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDateString(),
                    EndDate = DateTime.Now.Date.ToShortDateString()
                };

                user.CheckPermission(Permission.Report0006_View);

                model.ShowClientSummary = "1";
                model.ShowClientOrganizationSummary = "1";
                model.ShowClientContractSummary = "1";
                model.ShowBalanceDocumentSummary = "1";
                model.ShowBalanceDocumentFullInfo = "1";
                model.IncludeExpenditureWaybillsAndReturnFromClientWaybills = "1";
                model.IncludeDealPayments = "1";
                model.IncludeDealInitialBalanceCorrections = "1";
                model.CreateByClient = "1";

                model.GroupByCollection = ComboBoxBuilder.GetComboBoxItemList<GroupingType>(addEmptyItem: false, sort: false);
                model.ClientList = clientService.GetList(user).OrderBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name);
                model.ClientOrganizationList = clientOrganizationService.GetList(user).OrderBy(x => x.ShortName).ToDictionary(x => x.Id.ToString(), x => x.ShortName);
                model.TeamList = teamService.GetList(user, Permission.Team_List_Details).OrderBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name);

                return model;
            }
        }

        /// <summary>
        /// Создание настроек печатной формы
        /// </summary>
        /// <param name="clientId">Код клиента (если строится из деталей клиента)</param>
        /// <param name="clientOrganizationId">Код организации клиента (если строится из деталей организации клиента)</param>
        /// <param name="currentUser">Пользователь</param>
        public Report0006PrintingFormSettingsViewModel Report0006PrintingFormSettings(int? clientId, int? clientOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var model = new Report0006PrintingFormSettingsViewModel()
                {
                    StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDateString(),
                    EndDate = DateTime.Now.Date.ToShortDateString(),
                    PrintingFormClientId = clientId.HasValue ? clientId.ToString() : "",
                    PrintingFormClientOrganizationId = clientOrganizationId.HasValue ? clientOrganizationId.ToString() : ""
                };

                ValidationUtils.Assert(clientId.HasValue || clientOrganizationId.HasValue, "Должен быть указан клиент или организация клиента.");

                user.CheckPermission(Permission.Deal_Balance_View);
                user.CheckPermission(clientOrganizationId.HasValue ? Permission.ClientOrganization_List_Details : Permission.Client_List_Details);

                return model;
            }
        }

        #endregion

        #region Report0006 Построение отчета

        public Report0006ViewModel Report0006(Report0006SettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                #region Чтение основных параметров отчета

                var currentDate = DateTimeUtils.GetCurrentDateTime();
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Report0006_View);

                DateTime startDate, endDate;
                base.ParseDatePeriod(settings.StartDate, settings.EndDate, currentDate, out startDate, out endDate);

                bool createByClient = ValidationUtils.TryGetBool(settings.CreateByClient);
                bool showClientSummary = ValidationUtils.TryGetBool(settings.ShowClientSummary);
                bool showClientOrganizationSummary = ValidationUtils.TryGetBool(settings.ShowClientOrganizationSummary);
                bool showClientContractSummary = ValidationUtils.TryGetBool(settings.ShowClientContractSummary);
                bool showBalanceDocumentSummary = ValidationUtils.TryGetBool(settings.ShowBalanceDocumentSummary);
                bool showBalanceDocumentFullInfo = ValidationUtils.TryGetBool(settings.ShowBalanceDocumentFullInfo);
                bool includeExpenditureWaybillsAndReturnFromClientWaybills = ValidationUtils.TryGetBool(settings.IncludeExpenditureWaybillsAndReturnFromClientWaybills);
                bool includeDealPayments = ValidationUtils.TryGetBool(settings.IncludeDealPayments);
                bool includeDealInitialBalanceCorrections = ValidationUtils.TryGetBool(settings.IncludeDealInitialBalanceCorrections);
                bool allClients = settings.AllClients == "1";
                bool allClientOrganizations = settings.AllClientOrganizations == "1";
                bool allTeams = settings.AllTeams == "1";

                var groupingList = ParseGroupingString(settings.GroupByCollectionIDs);

                ValidationUtils.Assert(showClientSummary || showClientOrganizationSummary || showClientContractSummary ||
                    showBalanceDocumentSummary || showBalanceDocumentFullInfo,
                    "Необходимо выбрать хотя бы одну таблицу.");
                ValidationUtils.Assert(includeExpenditureWaybillsAndReturnFromClientWaybills || includeDealPayments || includeDealInitialBalanceCorrections,
                    "Необходимо учитывать хотя бы один вид документов.");

                if (showClientSummary || showClientContractSummary || groupingList.Contains(GroupingType.Client))
                {
                    user.CheckPermission(Permission.Client_List_Details);
                }
                if (showClientOrganizationSummary || showClientContractSummary || groupingList.Contains(GroupingType.ClientOrganization))
                {
                    user.CheckPermission(Permission.ClientOrganization_List_Details);
                }

                var model = new Report0006ViewModel()
                {
                    ReportName = "Отчет по взаиморасчетам с " + (createByClient ? "клиентами" : "организациями клиентов"),
                    CreatedBy = currentUser.DisplayName,
                    DateTime = currentDate.ToString(),
                    Date = currentDate.ToShortDateString(),
                    Settings = settings
                };

                #endregion

                #region Получение списка выбранных для отчета сущностей (клиентов или организаций клиентов, команд)

                IList<int> clientIDs = null;
                IList<int> clientOrganizationIDs = null;
                IList<short> teamIDs = null;
                if (createByClient)
                {
                    if (!allClients)
                    {
                        ValidationUtils.Assert(!String.IsNullOrEmpty(settings.ClientIDs),
                            "Не выбрано ни одного клиента.");
                        clientIDs = settings.ClientIDs.Split('_').Select(x => ValidationUtils.TryGetInt(x)).ToList();
                    }

                }
                else
                {
                    if (!allClientOrganizations)
                    {
                        ValidationUtils.Assert(!String.IsNullOrEmpty(settings.ClientOrganizationIDs),
                            "Не выбрано ни одной организации клиента.");
                        clientOrganizationIDs = settings.ClientOrganizationIDs.Split('_').Select(x => ValidationUtils.TryGetInt(x)).ToList();
                    }
                }

                if (!allTeams)
                {
                    ValidationUtils.Assert(!String.IsNullOrEmpty(settings.TeamIDs),
                           "Не выбрано ни одной команды.");
                    teamIDs = settings.TeamIDs.Split('_').Select(x => ValidationUtils.TryGetShort(x)).ToList();
                }

                #endregion

                #region Самая емкая по времени обращения к базе часть - расчет начального сальдо и подгрузка сущностей для отчета

                // Рассчитывать начальное сальдо по сделкам надо при видимости любой из 4 таблиц (кроме договоров)
                bool calculateBalance = showClientSummary || showClientOrganizationSummary || showBalanceDocumentSummary || showBalanceDocumentFullInfo;

                // При сборке "по живым" данным надо подгружать сущности и для конечного сальдо, а не только для таблицы "Развернутая информация по документам учета",
                // то есть при видимости любой из 4 таблиц (кроме договоров), но только если выбрана опция "Учитывать" для данного вида документов
                bool loadExpenditureWaybillListAndReturnFromClientWaybillList = includeExpenditureWaybillsAndReturnFromClientWaybills &&
                    (showClientSummary || showClientOrganizationSummary || showBalanceDocumentSummary || showBalanceDocumentFullInfo);
                bool loadDealPaymentList = includeDealPayments &&
                    (showClientSummary || showClientOrganizationSummary || showBalanceDocumentSummary || showBalanceDocumentFullInfo);
                bool loadDealInitialBalanceCorrectionList = includeDealInitialBalanceCorrections &&
                    (showClientSummary || showClientOrganizationSummary || showBalanceDocumentSummary || showBalanceDocumentFullInfo);

                IList<InitialBalanceInfo> startingBalanceInfo = new List<InitialBalanceInfo>();
                IDictionary<Guid, ExpenditureWaybill> expenditureWaybillList = new Dictionary<Guid, ExpenditureWaybill>();
                IDictionary<Guid, ReturnFromClientWaybill> returnFromClientWaybillList = new Dictionary<Guid, ReturnFromClientWaybill>();
                IDictionary<Guid, DealPayment> dealPaymentList = new Dictionary<Guid, DealPayment>();
                IDictionary<Guid, DealInitialBalanceCorrection> dealInitialBalanceCorrectionList = new Dictionary<Guid, DealInitialBalanceCorrection>();

                if (createByClient)
                {
                    if (calculateBalance)
                    {
                        // Начальное сальдо по сделкам
                        startingBalanceInfo = dealIndicatorService.CalculateBalanceOnDateByClientAndTeamList(clientIDs, teamIDs, startDate, user,
                             includeExpenditureWaybillsAndReturnFromClientWaybills, includeDealPayments, includeDealInitialBalanceCorrections);
                    }

                    if (loadExpenditureWaybillListAndReturnFromClientWaybillList)
                    {
                        expenditureWaybillList = expenditureWaybillService.GetShippedListInDateRangeByClientAndTeamList(startDate, endDate, clientIDs, teamIDs, user);
                        returnFromClientWaybillList = returnFromClientWaybillService.GetListInDateRangeByClientAndTeamList(startDate, endDate, clientIDs, teamIDs, user);
                    }

                    if (loadDealPaymentList)
                    {
                        dealPaymentList = dealPaymentDocumentService.GetDealPaymentListInDateRangeByClientAndTeamList(startDate, endDate, clientIDs, teamIDs, user);
                    }

                    if (loadDealInitialBalanceCorrectionList)
                    {
                        dealInitialBalanceCorrectionList = dealPaymentDocumentService.GetDealInitialBalanceCorrectionListInDateRangeByClientAndTeamList(startDate, endDate, clientIDs, teamIDs, user);
                    }
                }
                else
                {
                    if (calculateBalance)
                    {
                        // Начальное сальдо по сделкам
                        startingBalanceInfo = dealIndicatorService.CalculateBalanceOnDateByClientOrganizationAndTeamList(clientOrganizationIDs, teamIDs, startDate, user,
                            includeExpenditureWaybillsAndReturnFromClientWaybills, includeDealPayments, includeDealInitialBalanceCorrections);
                    }

                    if (loadExpenditureWaybillListAndReturnFromClientWaybillList)
                    {
                        expenditureWaybillList = expenditureWaybillService.GetShippedListInDateRangeByClientOrganizationAndTeamList(startDate, endDate, clientOrganizationIDs, teamIDs, user);
                        returnFromClientWaybillList = returnFromClientWaybillService.GetListInDateRangeByClientOrganizationAndTeamList(startDate, endDate, clientOrganizationIDs, teamIDs, user);
                    }

                    if (loadDealPaymentList)
                    {
                        dealPaymentList = dealPaymentDocumentService.GetDealPaymentListInDateRangeByClientOrganizationAndTeamList(startDate, endDate, clientOrganizationIDs, teamIDs, user);
                    }

                    if (loadDealInitialBalanceCorrectionList)
                    {
                        dealInitialBalanceCorrectionList = dealPaymentDocumentService.GetDealInitialBalanceCorrectionListInDateRangeByClientOrganizationAndTeamList(startDate, endDate, clientOrganizationIDs, teamIDs, user);
                    }
                }

                #endregion

                #region Вычисляем дебет, кредит за период и конечное сальдо по сделкам

                IList<InitialBalanceInfo> dealDebitInfo = new List<InitialBalanceInfo>();
                IList<InitialBalanceInfo> dealCreditInfo = new List<InitialBalanceInfo>();
                IList<InitialBalanceInfo> dealEndingBalanceInfo = new List<InitialBalanceInfo>();

                if (calculateBalance)
                {
                    foreach (var expenditureWaybill in expenditureWaybillList.Values)
                    {
                        AddValue(dealDebitInfo, expenditureWaybill.Deal.Contract.AccountOrganization.Id, expenditureWaybill.Deal.Client.Id,
                            expenditureWaybill.Deal.Contract.ContractorOrganization.Id, expenditureWaybill.Deal.Contract.Id, expenditureWaybill.Team.Id,
                            expenditureWaybill.SalePriceSum);
                    }

                    foreach (var returnFromClientWaybill in returnFromClientWaybillList.Values)
                    {
                        AddValue(dealCreditInfo, returnFromClientWaybill.Deal.Contract.AccountOrganization.Id, returnFromClientWaybill.Deal.Client.Id,
                            returnFromClientWaybill.Deal.Contract.ContractorOrganization.Id, returnFromClientWaybill.Deal.Contract.Id,
                            returnFromClientWaybill.Team.Id, returnFromClientWaybill.SalePriceSum);
                    }

                    foreach (var dealPayment in dealPaymentList.Values)
                    {
                        if (dealPayment.Type == DealPaymentDocumentType.DealPaymentFromClient)
                        {
                            AddValue(dealCreditInfo, dealPayment.Deal.Contract.AccountOrganization.Id, dealPayment.Deal.Client.Id, dealPayment.Deal.Contract.ContractorOrganization.Id,
                                dealPayment.Deal.Contract.Id, dealPayment.Team.Id, dealPayment.Sum);
                        }
                        else
                        {
                            AddValue(dealDebitInfo, dealPayment.Deal.Contract.AccountOrganization.Id, dealPayment.Deal.Client.Id, dealPayment.Deal.Contract.ContractorOrganization.Id,
                                dealPayment.Deal.Contract.Id, dealPayment.Team.Id, dealPayment.Sum);
                        }
                    }

                    foreach (var dealInitialBalanceCorrection in dealInitialBalanceCorrectionList.Values)
                    {
                        if (dealInitialBalanceCorrection.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection)
                        {
                            AddValue(dealDebitInfo, dealInitialBalanceCorrection.Deal.Contract.AccountOrganization.Id, dealInitialBalanceCorrection.Deal.Client.Id,
                                dealInitialBalanceCorrection.Deal.Contract.ContractorOrganization.Id, dealInitialBalanceCorrection.Deal.Contract.Id,
                                dealInitialBalanceCorrection.Team.Id, dealInitialBalanceCorrection.Sum);
                        }
                        else
                        {
                            AddValue(dealCreditInfo, dealInitialBalanceCorrection.Deal.Contract.AccountOrganization.Id, dealInitialBalanceCorrection.Deal.Client.Id,
                                dealInitialBalanceCorrection.Deal.Contract.ContractorOrganization.Id, dealInitialBalanceCorrection.Deal.Contract.Id,
                                dealInitialBalanceCorrection.Team.Id, dealInitialBalanceCorrection.Sum);
                        }
                    }

                    AddValues(dealEndingBalanceInfo, dealDebitInfo, 1);
                    AddValues(dealEndingBalanceInfo, dealCreditInfo, -1);
                }

                #endregion

                #region Получаем списки всех необходимых сущностей

                IEnumerable<Deal> deals = new List<Deal>();
                IEnumerable<Client> clients = new List<Client>();
                IEnumerable<ClientOrganization> clientOrganizations = new List<ClientOrganization>();
                IEnumerable<ClientContract> contracts = new List<ClientContract>();
                IEnumerable<Team> teams = new List<Team>();

                if (showClientContractSummary || groupingList.Contains(GroupingType.ClientContract))
                {
                    contracts = clientContractService.GetList(startDate, endDate, user);

                    var endingBalanceDict = dealEndingBalanceInfo.GroupBy(x => x.ContractId).ToDictionary(x => x.Key);
                    var startBalanceDict = startingBalanceInfo.GroupBy(x => x.ContractId).ToDictionary(x => x.Key);
                    contracts = contracts.Where(y =>
                        (endingBalanceDict.Keys.Contains(y.Id) && endingBalanceDict[y.Id].Sum(x => x.Sum) != 0) ||
                        (startBalanceDict.Keys.Contains(y.Id) && startBalanceDict[y.Id].Sum(x => x.Sum) != 0));

                    deals = dealService.GetListByActiveContract(startDate, endDate, user).Where(x => contracts.Contains(x.Contract));
                }

                if (createByClient)
                {
                    if (showClientContractSummary || showClientSummary || groupingList.Contains(GroupingType.Client) || showClientContractSummary ||
                        showClientOrganizationSummary || groupingList.Contains(GroupingType.ClientOrganization))
                    {
                        var endingBalanceDict = dealEndingBalanceInfo.GroupBy(x => x.ClientId).ToDictionary(x => x.Key);
                        var startBalanceDict = startingBalanceInfo.GroupBy(x => x.ClientId).ToDictionary(x => x.Key);

                        if (allClients)
                        {
                            clients = clientService.GetList(user).Where(y => (endingBalanceDict.Keys.Contains(y.Id) && endingBalanceDict[y.Id].Sum(x => x.Sum) != 0) ||
                                (startBalanceDict.Keys.Contains(y.Id) && startBalanceDict[y.Id].Sum(x => x.Sum) != 0));
                        }
                        else
                        {
                            clients = clientService.CheckClientsExistence(clientIDs, user);
                        }

                        if (showClientContractSummary || showClientOrganizationSummary || groupingList.Contains(GroupingType.ClientOrganization))
                        {
                            clientOrganizations = clients.SelectMany(x => x.Organizations.Select(y => y.As<ClientOrganization>())).Distinct();
                        }
                    }
                }
                else
                {
                    if (showClientContractSummary || showClientOrganizationSummary || groupingList.Contains(GroupingType.ClientOrganization) ||
                        showClientContractSummary || showClientSummary || groupingList.Contains(GroupingType.Client))
                    {
                        var endingBalanceDict = dealEndingBalanceInfo.GroupBy(x => x.ClientOrganizationId).ToDictionary(x => x.Key);
                        var startBalanceDict = startingBalanceInfo.GroupBy(x => x.ClientOrganizationId).ToDictionary(x => x.Key);

                        if (!createByClient)
                        {
                            if (allClientOrganizations)
                            {
                                clientOrganizations = clientOrganizationService.GetList(user).Where(y => (endingBalanceDict.Keys.Contains(y.Id) && endingBalanceDict[y.Id].Sum(x => x.Sum) != 0) ||
                                    (startBalanceDict.Keys.Contains(y.Id) && startBalanceDict[y.Id].Sum(x => x.Sum) != 0));
                            }
                            else
                            {
                                clientOrganizations = clientOrganizationService.CheckClientOrganizationsExistence(clientOrganizationIDs, user);
                            }
                        }

                        if (showClientContractSummary || showClientSummary || groupingList.Contains(GroupingType.Client))
                        {
                            clients = clientOrganizations.SelectMany(x => x.Contractors.Select(y => y.As<Client>())).Distinct();
                        }
                    }
                }

                // Подгружаем команды только если задана группировка по ним
                if (groupingList.Contains(GroupingType.Team))
                {
                    teams = teamService.GetList(user, Permission.Team_List_Details);
                }

                #endregion

                #region Заполняем таблицу 1 "Сводная информация по клиентам"

                if (showClientSummary)
                {
                    FillClientSummaryTable(model, clients, startingBalanceInfo, dealDebitInfo, dealCreditInfo);
                }

                #endregion

                #region Заполняем таблицу 2 "Сводная информация по организациям клиентов"

                if (showClientOrganizationSummary)
                {
                    FillClientOrganizationSummaryTable(model, clientOrganizations, startingBalanceInfo, dealDebitInfo, dealCreditInfo);
                }

                #endregion

                #region Заполняем таблицу 3 "Список открытых договоров в период"

                if (showClientContractSummary)
                {
                    // Словарь информации о договорах (ключ - код договора, код клиента)
                    var clientContractInfoList = new Dictionary<Tuple<short, int>, ClientContractInfo>();
                    var endingBalanceDict = dealEndingBalanceInfo.GroupBy(x => x.ContractId).ToDictionary(x => x.Key);
                    var startBalanceDict = startingBalanceInfo.GroupBy(x => x.ContractId).ToDictionary(x => x.Key);
                    var dealDict = deals.GroupBy(x => x.Contract).ToDictionary(x => x.Key);

                    foreach (var contract in contracts.OrderBy(x => x.StartDate).ThenBy(x => x.Number).ThenBy(x => x.Name))
                    {
                        if ((!endingBalanceDict.Keys.Contains(contract.Id) || endingBalanceDict[contract.Id].Sum(x => x.Sum) == 0) &&
                            (!startBalanceDict.Keys.Contains(contract.Id) || startBalanceDict[contract.Id].Sum(x => x.Sum) == 0))
                        {
                            continue;
                        }

                        var dealList = dealDict[contract]; //deals.Where(x => x.Contract == contract);
                        foreach (var deal in dealList)
                        {
                            var key = new Tuple<short, int>(deal.Contract.Id, deal.Client.Id);
                            if (!clientContractInfoList.ContainsKey(key))
                            {
                                clientContractInfoList.Add(key, new ClientContractInfo(
                                    deal.Client.Name,
                                    deal.Contract.FullName,
                                    deal.Contract.Number,
                                    deal.Contract.Name,
                                    deal.Contract.AccountOrganization.ShortName,
                                    deal.Contract.ContractorOrganization.ShortName));
                            }
                        }
                    }

                    foreach (var item in clientContractInfoList
                        .OrderBy(x => x.Value.ClientName)
                        .ThenBy(x => x.Value.ClientContractNumber)
                        .ThenBy(x => x.Value.ClientContractName))
                    {
                        model.ClientContractSummary.Add(new Report0006ContractItemViewModel(
                            item.Value.ClientName,
                            item.Value.ClientContractFullName,
                            item.Value.AccountOrganizationShortName,
                            item.Value.ClientOrganizationShortName));
                    }
                }

                #endregion

                #region Заполняем таблицы 4 и 5 (общая информация по взаиморасчетам и развернутая информация по документам учета)

                if (showBalanceDocumentSummary || showBalanceDocumentFullInfo)
                {
                    var report0006info = new Report0006Info(showBalanceDocumentSummary, showBalanceDocumentFullInfo, model, startDate, endDate);

                    // Неиспользованные пока группировки. Отсюда будет удаляться каждый раз первая, и произойдет группировка по ней
                    var keysToUse = groupingList.ToList();

                    // Уже полученные имена объектов группировки (клиенты, организации, договора). В конец будет добавляться очередное имя сущности.
                    // Например, если группируем по "клиент, договор", в первом уровне вложенности добавится имя клиента, на втором - имя договора,
                    // и в списке будет 2 строки. После 1-го использования строки здесь заменяются на null
                    var keyNames = new List<string>();

                    // Группируем сделки (метод рекурсивно вызывает сам себя, убирая элементы из keysToUse и добавляя названия в keyNames,
                    // а после исчерпания группировок вызывает метод ProcessDeals())
                    GroupDeals(expenditureWaybillList.Values, returnFromClientWaybillList.Values, dealPaymentList.Values, dealInitialBalanceCorrectionList.Values,
                        clients, clientOrganizations, contracts, teams, startingBalanceInfo, keysToUse, keyNames, report0006info);
                }

                #endregion

                return model;
            }
        }

        /// <summary>
        /// Добавление значения в список
        /// </summary>
        private void AddValue(IList<InitialBalanceInfo> list, int accountOrganizationId, int clientId, int clientOrganizationId,
            short contractId, short teamId, decimal value)
        {
            var item = list.Where(x => x.AccountOrganizationId == accountOrganizationId && x.ClientId == clientId && x.ClientOrganizationId == clientOrganizationId &&
                x.ContractId == contractId && x.TeamId == teamId)
                .FirstOrDefault();
            if (item != null)
            {
                item.Sum += value;
            }
            else
            {
                list.Add(new InitialBalanceInfo(accountOrganizationId, clientId, clientOrganizationId, contractId, teamId, value));
            }
        }

        /// <summary>
        /// Добавить в список суммы из списка с заданным знаком
        /// </summary>
        private void AddValues(IList<InitialBalanceInfo> sumList, IList<InitialBalanceInfo> valuesList, decimal sign)
        {
            foreach (var value in valuesList)
            {
                var item = sumList.Where(x => x.AccountOrganizationId == value.AccountOrganizationId && x.ClientId == value.ClientId &&
                    x.ClientOrganizationId == value.ClientOrganizationId && x.ContractId == value.ContractId && x.TeamId == value.TeamId)
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
        /// Заполнить таблицу "Сводная информация по организациям клиентов"
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dealStartingBalanceInfo"></param>
        /// <param name="dealDebitInfo"></param>
        /// <param name="dealCreditInfo"></param>
        private void FillClientOrganizationSummaryTable(Report0006ViewModel model, IEnumerable<ClientOrganization> clientOrganizations,
            IList<InitialBalanceInfo> dealStartingBalanceInfo,
            IList<InitialBalanceInfo> dealDebitInfo, IList<InitialBalanceInfo> dealCreditInfo)
        {
            var dealStartingBalanceInfoByClientOrganization = dealStartingBalanceInfo.GroupBy(x => x.ClientOrganizationId).ToDictionary(x => x.Key);
            var dealDebitInfoByClientOrganization = dealDebitInfo.GroupBy(x => x.ClientOrganizationId).ToDictionary(x => x.Key);
            var dealCreditInfoByClientOrganization = dealCreditInfo.GroupBy(x => x.ClientOrganizationId).ToDictionary(x => x.Key);

            foreach (var clientOrganization in clientOrganizations)
            {
                decimal debit = dealDebitInfoByClientOrganization.Keys.Contains(clientOrganization.Id) ?
                    dealDebitInfoByClientOrganization[clientOrganization.Id].Sum(x => x.Sum) : 0M;
                decimal credit = dealCreditInfoByClientOrganization.Keys.Contains(clientOrganization.Id) ?
                    dealCreditInfoByClientOrganization[clientOrganization.Id].Sum(x => x.Sum) : 0M;
                decimal startingBalance = dealStartingBalanceInfoByClientOrganization.Keys.Contains(clientOrganization.Id) ?
                    dealStartingBalanceInfoByClientOrganization[clientOrganization.Id].Sum(x => x.Sum) : 0M;
                //проверяем не пустая ли строка получится в таблице
                if (!(debit == 0 && credit == 0 && startingBalance == 0))
                {
                    model.ClientOrganizationSummary.Add(new Report0006BalanceByPeriodItemViewModel(
                        clientOrganization.ShortName,
                        startingBalance,
                        debit,
                        credit,
                        GetEndingBalance(startingBalance, debit, credit)));
                }
            }

            model.ClientOrganizationSummary = model.ClientOrganizationSummary.OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Заполнить таблицу "Сводная информация по клиентам"
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dealStartingBalanceInfo">Коллекция первоначального сальдо</param>
        /// <param name="dealDebitInfo">Коллекция дебита</param>
        /// <param name="dealCreditInfo">Коллекция кредита</param>
        private void FillClientSummaryTable(Report0006ViewModel model, IEnumerable<Client> clients, IList<InitialBalanceInfo> dealStartingBalanceInfo,
            IList<InitialBalanceInfo> dealDebitInfo, IList<InitialBalanceInfo> dealCreditInfo)
        {
            var dealStartingBalanceInfoByClient = dealStartingBalanceInfo.GroupBy(x => x.ClientId).ToDictionary(x => x.Key);
            var dealDebitInfoByClient = dealDebitInfo.GroupBy(x => x.ClientId).ToDictionary(x => x.Key);
            var dealCreditInfoByClient = dealCreditInfo.GroupBy(x => x.ClientId).ToDictionary(x => x.Key);

            foreach (var client in clients)
            {
                decimal debit = dealDebitInfoByClient.Keys.Contains(client.Id) ?
                    dealDebitInfoByClient[client.Id].Sum(x => x.Sum) : 0M;
                decimal credit = dealCreditInfoByClient.Keys.Contains(client.Id) ?
                    dealCreditInfoByClient[client.Id].Sum(x => x.Sum) : 0M;
                decimal startingBalance = dealStartingBalanceInfoByClient.Keys.Contains(client.Id) ?
                    dealStartingBalanceInfoByClient[client.Id].Sum(x => x.Sum) : 0M;

                if (!(debit == 0 && credit == 0 && startingBalance == 0))
                {
                    model.ClientSummary.Add(new Report0006BalanceByPeriodItemViewModel(
                        client.Name,
                        startingBalance,
                        debit,
                        credit,
                        GetEndingBalance(startingBalance, debit, credit)));
                }
            }
            model.ClientSummary = model.ClientSummary.OrderBy(x => x.Name).ToList();
        }

        private decimal GetEndingBalance(decimal startingBalance, decimal debit, decimal credit)
        {
            return startingBalance + debit - credit;
        }

        /// <summary>
        /// Метод для группировки сделок. Если полей для группировки уже нет, вызывает метод ProcessDeals. Если поля для группировки
        /// остались, делает группировку по первому полю и рекурсивно вызывает сам себя. Сложность должна быть 3n при тройной группировке n сделок,
        /// если у простой группировки сложность n
        /// </summary>
        /// <param name="dealList">Коллекция сделок</param>
        /// <param name="keysToUse">Неиспользованные пока группировки. Отсюда будет удаляться каждый раз первая, и произойдет группировка по ней</param>
        /// <param name="keyNames">Названия сущностей ключа группировки (если ключ = "клиент, договор", это имена клиента и договора)</param>
        /// <param name="report0006info">Данные отчета</param>
        private bool GroupDeals(IEnumerable<ExpenditureWaybill> expenditureWaybillList, IEnumerable<ReturnFromClientWaybill> returnFromClientWaybillList,
            IEnumerable<DealPayment> dealPaymentList, IEnumerable<DealInitialBalanceCorrection> dealInitialBalanceCorrectionList,
            IEnumerable<Client> clients, IEnumerable<ClientOrganization> clientOrganizations, IEnumerable<ClientContract> contracts, IEnumerable<Team> teams,
            IEnumerable<InitialBalanceInfo> startingBalanceInfo, IList<GroupingType> keysToUse, IList<string> keyNames, Report0006Info report0006info)
        {
            bool isGroupOut = false;

            // Если полей для группировки нет
            if (keysToUse.Count == 0)
            {
                return ProcessDeals(expenditureWaybillList, returnFromClientWaybillList, dealPaymentList, dealInitialBalanceCorrectionList, startingBalanceInfo,
                     keyNames, report0006info);
            }

            // Делаем копию списка группировок, удаляя первую
            var keyToUse = keysToUse.First();
            var newKeysToUse = keysToUse.Skip(1).ToList();

            // По первой (удаленной) группировке из списка делаем группировку и рекурсивно вызываем себя
            switch (keyToUse)
            {
                case GroupingType.Client:
                    {
                        var keyNamesCopy = keyNames.ToList();
                        var expenditureGroup = expenditureWaybillList.GroupBy(x => x.Deal.Client).ToDictionary(x => x.Key);
                        var returnFromClientGroup = returnFromClientWaybillList.GroupBy(x => x.Deal.Client).ToDictionary(x => x.Key);
                        var dealPaymentGroup = dealPaymentList.GroupBy(x => x.Deal.Client).ToDictionary(x => x.Key);
                        var dealInitialBalanceCorrectionGroup = dealInitialBalanceCorrectionList.GroupBy(x => x.Deal.Client).ToDictionary(x => x.Key);
                        var startingBalanceInfoGroup = startingBalanceInfo.GroupBy(x => x.ClientId).ToDictionary(x => x.Key);

                        var deals = new List<Deal>();
                        deals.AddRange(expenditureWaybillList.Select(x => x.Deal));
                        deals.AddRange(returnFromClientWaybillList.Select(x => x.Deal));
                        deals.AddRange(dealPaymentList.Select(x => x.Deal));
                        deals.AddRange(dealInitialBalanceCorrectionList.Select(x => x.Deal));

                        var contractList = new List<dynamic>();
                        contractList.AddRange(deals.Distinct().Select(x => new { ClientId = x.Client.Id, ContractId = x.Contract.Id }));
                        contractList.AddRange(startingBalanceInfo.Select(x => new { ClientId = x.ClientId, ContractId = x.ContractId }));

                        var t = contractList.GroupBy(x => (int)x.ClientId, x => (int)x.ContractId);
                        var _contracts = t.ToDictionary(x => x.Key, x => contracts.Where(y => x.Contains(y.Id)));


                        foreach (var client in clients.OrderBy(x => x.Name))
                        {
                            var newKeyNames = keyNamesCopy.ToList();
                            newKeyNames.Add(client.Name);
                            var isRowOut = GroupDeals(
                                expenditureGroup.Keys.Contains(client) ?
                                    (IEnumerable<ExpenditureWaybill>)expenditureGroup[client] :
                                    new List<ExpenditureWaybill>(),
                                returnFromClientGroup.Keys.Contains(client) ?
                                    (IEnumerable<ReturnFromClientWaybill>)returnFromClientGroup[client] :
                                    new List<ReturnFromClientWaybill>(),
                                dealPaymentGroup.Keys.Contains(client) ?
                                    (IEnumerable<DealPayment>)dealPaymentGroup[client] :
                                    new List<DealPayment>(),
                                dealInitialBalanceCorrectionGroup.Keys.Contains(client) ?
                                    (IEnumerable<DealInitialBalanceCorrection>)dealInitialBalanceCorrectionGroup[client] :
                                    new List<DealInitialBalanceCorrection>(),
                                clients,
                                clientOrganizations.Where(x => client.Organizations.Any(y => y.Id == x.Id)),
                                _contracts.Keys.Contains(client.Id) ? _contracts[client.Id] : new List<ClientContract>(),
                                teams,
                                startingBalanceInfoGroup.Keys.Contains(client.Id) ?
                                    (IEnumerable<InitialBalanceInfo>)startingBalanceInfoGroup[client.Id] :
                                    new List<InitialBalanceInfo>(),
                                newKeysToUse, newKeyNames, report0006info);

                            if (isRowOut)
                            {
                                // Удаляем все использованные названия сущностей, заменяя их на null (ProcessDeals не будет для таких создавать строки)
                                keyNamesCopy = EmptyStringList(keyNamesCopy);
                            }
                            isGroupOut = isGroupOut || isRowOut;
                        }
                    }
                    break;
                case GroupingType.ClientOrganization:
                    {
                        var keyNamesCopy = keyNames.ToList();
                        var expenditureGroup = expenditureWaybillList.GroupBy(x => x.Deal.Contract.ContractorOrganization).ToDictionary(x => x.Key);
                        var returnFromClientGroup = returnFromClientWaybillList.GroupBy(x => x.Deal.Contract.ContractorOrganization).ToDictionary(x => x.Key);
                        var dealPaymentGroup = dealPaymentList.GroupBy(x => x.Deal.Contract.ContractorOrganization).ToDictionary(x => x.Key);
                        var dealInitialBalanceCorrectionGroup = dealInitialBalanceCorrectionList.GroupBy(x => x.Deal.Contract.ContractorOrganization).ToDictionary(x => x.Key);
                        var startingBalanceInfoGroup = startingBalanceInfo.GroupBy(x => x.ClientOrganizationId).ToDictionary(x => x.Key);

                        foreach (var clientOrganization in clientOrganizations.OrderBy(x => x.ShortName))
                        {
                            var newKeyNames = keyNamesCopy.ToList();
                            newKeyNames.Add(clientOrganization.ShortName);
                            var isRowOut = GroupDeals(
                                expenditureGroup.Keys.Contains(clientOrganization) ?
                                    (IEnumerable<ExpenditureWaybill>)expenditureGroup[clientOrganization] :
                                    new List<ExpenditureWaybill>(),
                                returnFromClientGroup.Keys.Contains(clientOrganization) ?
                                    (IEnumerable<ReturnFromClientWaybill>)returnFromClientGroup[clientOrganization] :
                                    new List<ReturnFromClientWaybill>(),
                                dealPaymentGroup.Keys.Contains(clientOrganization) ?
                                    (IEnumerable<DealPayment>)dealPaymentGroup[clientOrganization] :
                                    new List<DealPayment>(),
                                dealInitialBalanceCorrectionGroup.Keys.Contains(clientOrganization) ?
                                    (IEnumerable<DealInitialBalanceCorrection>)dealInitialBalanceCorrectionGroup[clientOrganization] :
                                    new List<DealInitialBalanceCorrection>(),
                                clients.Where(x => x.Organizations.Any(y => y.Id == clientOrganization.Id)),
                                clientOrganizations,
                                contracts.Where(x => clientOrganization.Contracts.Contains(x)),
                                teams,
                                startingBalanceInfoGroup.Keys.Contains(clientOrganization.Id) ?
                                    (IEnumerable<InitialBalanceInfo>)startingBalanceInfoGroup[clientOrganization.Id] :
                                    new List<InitialBalanceInfo>(),
                                newKeysToUse, newKeyNames, report0006info);

                            if (isRowOut)
                            {
                                // Удаляем все использованные названия сущностей, заменяя их на null (ProcessDeals не будет для таких создавать строки)
                                keyNamesCopy = EmptyStringList(keyNamesCopy);
                            }
                            isGroupOut = isGroupOut || isRowOut;
                        }
                    }
                    break;
                case GroupingType.ClientContract:
                    {
                        var keyNamesCopy = keyNames.ToList();
                        var expenditureGroup = expenditureWaybillList.GroupBy(x => x.Deal.Contract).ToDictionary(x => x.Key);
                        var returnFromClientGroup = returnFromClientWaybillList.GroupBy(x => x.Deal.Contract).ToDictionary(x => x.Key);
                        var dealPaymentGroup = dealPaymentList.GroupBy(x => x.Deal.Contract).ToDictionary(x => x.Key);
                        var dealInitialBalanceCorrectionGroup = dealInitialBalanceCorrectionList.GroupBy(x => x.Deal.Contract).ToDictionary(x => x.Key);
                        var startingBalanceInfoGroup = startingBalanceInfo.GroupBy(x => x.ContractId).ToDictionary(x => x.Key);

                        foreach (var contract in contracts.OrderBy(x => x.Number).ThenBy(x => x.Name))
                        {
                            var newKeyNames = keyNamesCopy.ToList();
                            newKeyNames.Add(contract.FullName);
                            var isRowOut = GroupDeals(
                                expenditureGroup.Keys.Contains(contract) ?
                                    (IEnumerable<ExpenditureWaybill>)expenditureGroup[contract] :
                                    new List<ExpenditureWaybill>(),
                                returnFromClientGroup.Keys.Contains(contract) ?
                                    (IEnumerable<ReturnFromClientWaybill>)returnFromClientGroup[contract] :
                                    new List<ReturnFromClientWaybill>(),
                                dealPaymentGroup.Keys.Contains(contract) ?
                                    (IEnumerable<DealPayment>)dealPaymentGroup[contract] :
                                    new List<DealPayment>(),
                                dealInitialBalanceCorrectionGroup.Keys.Contains(contract) ?
                                    (IEnumerable<DealInitialBalanceCorrection>)dealInitialBalanceCorrectionGroup[contract] :
                                    new List<DealInitialBalanceCorrection>(),
                                clients, clientOrganizations, contracts, teams,
                                startingBalanceInfoGroup.Keys.Contains(contract.Id) ?
                                    (IEnumerable<InitialBalanceInfo>)startingBalanceInfoGroup[contract.Id] :
                                    new List<InitialBalanceInfo>(),
                                newKeysToUse, newKeyNames, report0006info);

                            if (isRowOut)
                            {
                                // Удаляем все использованные названия сущностей, заменяя их на null (ProcessDeals не будет для таких создавать строки)
                                keyNamesCopy = EmptyStringList(keyNamesCopy);
                            }
                            isGroupOut = isGroupOut || isRowOut;
                        }
                    }
                    break;
                case GroupingType.Team:
                    {
                        var keyNamesCopy = keyNames.ToList();
                        var expenditureGroup = expenditureWaybillList.GroupBy(x => x.Team).ToDictionary(x => x.Key);
                        var returnFromClientGroup = returnFromClientWaybillList.GroupBy(x => x.Team).ToDictionary(x => x.Key);
                        var dealPaymentGroup = dealPaymentList.GroupBy(x => x.Team).ToDictionary(x => x.Key);
                        var dealInitialBalanceCorrectionGroup = dealInitialBalanceCorrectionList.GroupBy(x => x.Team).ToDictionary(x => x.Key);
                        var startingBalanceGroup = startingBalanceInfo.GroupBy(x => x.TeamId).ToDictionary(x => x.Key);


                        var swByTeam = expenditureWaybillList.GroupBy(x => x.Team).ToDictionary(x => x.Key);
                        var rfcwbyTeam = returnFromClientWaybillList.GroupBy(x => x.Team).ToDictionary(x => x.Key);
                        var dplByTeam = dealPaymentList.GroupBy(x => x.Team).ToDictionary(x => x.Key);
                        var dibcByTeam = dealInitialBalanceCorrectionList.GroupBy(x => x.Team).ToDictionary(x => x.Key);

                        var _contracts = teams.ToDictionary(x => x, y =>
                        {
                            var result = new List<ClientContract>();
                            result.AddRange(swByTeam.Keys.Contains(y) ? swByTeam[y].Select(x => x.Deal.Contract) : new List<ClientContract>());
                            result.AddRange(rfcwbyTeam.Keys.Contains(y) ? rfcwbyTeam[y].Select(x => x.Deal.Contract) : new List<ClientContract>());
                            result.AddRange(dplByTeam.Keys.Contains(y) ? dplByTeam[y].Select(x => x.Deal.Contract) : new List<ClientContract>());
                            result.AddRange(dibcByTeam.Keys.Contains(y) ? dibcByTeam[y].Select(x => x.Deal.Contract) : new List<ClientContract>());
                            result.AddRange(startingBalanceGroup.Keys.Contains(y.Id) ?
                                contracts.Where(z => startingBalanceGroup[y.Id].GroupBy(x => x.ContractId).Where(x => x.Sum(x2 => x2.Sum) != 0).Select(x => x.Key).Contains(z.Id)) :
                                new List<ClientContract>());

                            return result.Distinct();
                        });
                        var _client = teams.ToDictionary(x => x, y =>
                        {
                            var result = new List<Client>();
                            result.AddRange(swByTeam.Keys.Contains(y) ? swByTeam[y].Select(x => x.Deal.Client) : new List<Client>());
                            result.AddRange(rfcwbyTeam.Keys.Contains(y) ? rfcwbyTeam[y].Select(x => x.Deal.Client) : new List<Client>());
                            result.AddRange(dplByTeam.Keys.Contains(y) ? dplByTeam[y].Select(x => x.Deal.Client) : new List<Client>());
                            result.AddRange(dibcByTeam.Keys.Contains(y) ? dibcByTeam[y].Select(x => x.Deal.Client) : new List<Client>());
                            result.AddRange(startingBalanceGroup.Keys.Contains(y.Id) ?
                                clients.Where(x => startingBalanceGroup[y.Id].Select(z => z.ClientId).Contains(x.Id)) :
                                new List<Client>());

                            return result.Distinct();
                        });
                        var _clientOrganization = teams.ToDictionary(x => x, y =>
                        {
                            var result = new List<ClientOrganization>();
                            result.AddRange(swByTeam.Keys.Contains(y) ? swByTeam[y].Select(x => x.Deal.Contract.ContractorOrganization.As<ClientOrganization>()) : new List<ClientOrganization>());
                            result.AddRange(rfcwbyTeam.Keys.Contains(y) ? rfcwbyTeam[y].Select(x => x.Deal.Contract.ContractorOrganization.As<ClientOrganization>()) : new List<ClientOrganization>());
                            result.AddRange(dplByTeam.Keys.Contains(y) ? dplByTeam[y].Select(x => x.Deal.Contract.ContractorOrganization.As<ClientOrganization>()) : new List<ClientOrganization>());
                            result.AddRange(dibcByTeam.Keys.Contains(y) ? dibcByTeam[y].Select(x => x.Deal.Contract.ContractorOrganization.As<ClientOrganization>()) : new List<ClientOrganization>());
                            result.AddRange(startingBalanceGroup.Keys.Contains(y.Id) ?
                                clientOrganizations.Where(x => startingBalanceGroup[y.Id].Select(z => z.ClientOrganizationId).Contains(x.Id)) :
                                new List<ClientOrganization>());

                            return result.Distinct();
                        });

                        foreach (var team in teams.OrderBy(x => x.Name))
                        {
                            var newKeyNames = keyNamesCopy.ToList();
                            newKeyNames.Add(team.Name);
                            var isRowOut = GroupDeals(
                                expenditureGroup.Keys.Contains(team) ?
                                    (IEnumerable<ExpenditureWaybill>)expenditureGroup[team] :
                                    new List<ExpenditureWaybill>(),
                                returnFromClientGroup.Keys.Contains(team) ?
                                    (IEnumerable<ReturnFromClientWaybill>)returnFromClientGroup[team] :
                                    new List<ReturnFromClientWaybill>(),
                                dealPaymentGroup.Keys.Contains(team) ?
                                    (IEnumerable<DealPayment>)dealPaymentGroup[team] :
                                    new List<DealPayment>(),
                                dealInitialBalanceCorrectionGroup.Keys.Contains(team) ?
                                    (IEnumerable<DealInitialBalanceCorrection>)dealInitialBalanceCorrectionGroup[team] :
                                    new List<DealInitialBalanceCorrection>(),
                                _client[team],
                                _clientOrganization[team],
                                _contracts[team],
                                teams,
                                startingBalanceGroup.Keys.Contains(team.Id) ?
                                    (IEnumerable<InitialBalanceInfo>)startingBalanceGroup[team.Id] :
                                    new List<InitialBalanceInfo>(),
                                newKeysToUse, newKeyNames, report0006info);
                            if (isRowOut)
                            {
                                // Удаляем все использованные названия сущностей, заменяя их на null (ProcessDeals не будет для таких создавать строки)
                                keyNamesCopy = EmptyStringList(keyNamesCopy);
                            }
                            isGroupOut = isGroupOut || isRowOut;
                        }
                    }
                    break;
                default:
                    throw new Exception("Неизвестный тип группировки в отчете.");
            };

            return isGroupOut;
        }

        /// <summary>
        /// Обработать группу сделок (представляющую собой минимальную единицу группировки) и добавить ее в модель отчета
        /// </summary>
        /// <param name="dealList">Коллекция сделок</param>
        /// <param name="keyNames">Названия сущностей ключа группировки (если ключ = "клиент, договор", это имена клиента и договора).
        /// null в списке означает, что эту строку выводить не надо (она уже была выведена в предыдущей группировке)</param>
        /// <param name="report0006info">Данные отчета</param>
        private bool ProcessDeals(IEnumerable<ExpenditureWaybill> expenditureWaybillList, IEnumerable<ReturnFromClientWaybill> returnFromClientWaybillList,
            IEnumerable<DealPayment> dealPaymentList, IEnumerable<DealInitialBalanceCorrection> dealInitialBalanceCorrectionList,
            IEnumerable<InitialBalanceInfo> startingBalanceInfo, IList<string> keyNames, Report0006Info report0006info)
        {
            var balanceInfoByPeriod = CalculateDealBalanceInfoByPeriod(expenditureWaybillList, returnFromClientWaybillList, dealPaymentList,
                dealInitialBalanceCorrectionList, startingBalanceInfo, report0006info);

            //Если сальдо на начало отчета нулевое и никаких взаиморосчетов за период отчета не было, то не добавляем строки в отчет
            if (balanceInfoByPeriod.StartingBalance == 0 &&
                balanceInfoByPeriod.EndingBalance == 0 &&
                balanceInfoByPeriod.TotalCredit == 0 &&
                balanceInfoByPeriod.TotalDebit == 0)
            {
                return false;
            }

            // Добавляем в отчет строки
            var groupingLevel = 1;
            foreach (var name in keyNames)
            {
                if (name != null)
                {
                    if (report0006info.ShowBalanceDocumentSummary)
                    {
                        report0006info.Model.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(groupingLevel, name));
                    }

                    if (report0006info.ShowBalanceDocumentFullInfo)
                    {
                        report0006info.Model.BalanceDocumentFullInfo.Add(new Report0006BalanceItemViewModel(groupingLevel, name));
                    }
                }

                groupingLevel++;
            }

            //Заполняем таблицу 4 ("Общая информация по взаиморасчетам")
            if (report0006info.ShowBalanceDocumentSummary)
            {
                AddBlockToBalanceDocumentSummaryTable(report0006info, balanceInfoByPeriod);
            }

            //Заполняем таблицу 5 ("Развернутая информация по документам учета")

            if (report0006info.ShowBalanceDocumentFullInfo)
            {
                AddBlockToBalanceDocumentFullInfoTable(expenditureWaybillList, returnFromClientWaybillList, dealPaymentList,
                dealInitialBalanceCorrectionList, report0006info, balanceInfoByPeriod);
            }

            return true;
        }

        /// <summary>
        /// Добавить блок информации (о клиенте | организации | договоре | ...) в таблицу "Развернутая информация по документам учета"
        /// </summary>
        /// <param name="dealList">Коллекция сделок</param>
        /// <param name="report0006info">Данные отчета</param>
        /// <param name="balanceInfoByPeriod">Информация о сальдо по сделке (группе сделок) за период</param>
        private void AddBlockToBalanceDocumentFullInfoTable(IEnumerable<ExpenditureWaybill> expenditureWaybillList, IEnumerable<ReturnFromClientWaybill> returnFromClientWaybillList,
            IEnumerable<DealPayment> dealPaymentList, IEnumerable<DealInitialBalanceCorrection> dealInitialBalanceCorrectionList, Report0006Info report0006info,
            DealBalanceInfoByPeriod balanceInfoByPeriod)
        {
            var balanceDocumentFullInfoList = new List<BalanceDocumentFullInfo>();

            FillExpenditureWaybillFullInfo(expenditureWaybillList, report0006info.StartDate, report0006info.EndDate, balanceDocumentFullInfoList);
            FillReturnFromClientWaybillFullInfo(returnFromClientWaybillList, report0006info.StartDate, report0006info.EndDate, balanceDocumentFullInfoList);
            FillDealPaymentFullInfo(dealPaymentList, report0006info.StartDate, report0006info.EndDate, balanceDocumentFullInfoList);
            FillDealInitialBalanceCorrectionFullInfo(dealInitialBalanceCorrectionList, report0006info.StartDate, report0006info.EndDate, balanceDocumentFullInfoList);

            FillBalanceDocumentFullInfo(balanceDocumentFullInfoList, balanceInfoByPeriod, report0006info.StartDate, report0006info.EndDate,
                report0006info.Model.BalanceDocumentFullInfo);
        }

        /// <summary>
        /// Добавить блок информации (о клиенте | организации | договоре | ...) в таблицу "Общая информация по взаиморасчетам"
        /// </summary>
        /// <param name="report0006info">Данные отчета</param>
        /// <param name="balanceInfoByPeriod">Информация о сальдо по сделке (группе сделок) за период</param>
        private void AddBlockToBalanceDocumentSummaryTable(Report0006Info report0006info, DealBalanceInfoByPeriod balanceInfoByPeriod)
        {
            int rowNumber = 0;
            report0006info.Model.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber, balanceInfoByPeriod.StartingBalance,
                String.Format("Сальдо на {0}", report0006info.StartDate.ToShortDateString()), isHeader: true));
            report0006info.Model.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber,
                balanceInfoByPeriod.SaleWaybillSum, balanceInfoByPeriod.ReturnFromClientWaybillSum, "Отгрузка (возвраты) товаров"));
            report0006info.Model.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber,
                balanceInfoByPeriod.DealPaymentToClientCashSum, balanceInfoByPeriod.DealPaymentFromClientCashSum, "Оплаты за наличный расчет"));
            report0006info.Model.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber,
                balanceInfoByPeriod.DealPaymentToClientCashlessSum, balanceInfoByPeriod.DealPaymentFromClientCashlessSum, "Оплаты за безналичный расчет"));
            report0006info.Model.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber,
                balanceInfoByPeriod.DealPaymentToClientThirdPartyCashlessSum, balanceInfoByPeriod.DealPaymentFromClientThirdPartyCashlessSum, "Оплаты, принятые от третих лиц"));
            report0006info.Model.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber,
                balanceInfoByPeriod.DealDebitInitialBalanceCorrectionSum, balanceInfoByPeriod.DealCreditInitialBalanceCorrectionSum, "Корректировки сальдо"));
            report0006info.Model.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber,
                balanceInfoByPeriod.TotalDebit, balanceInfoByPeriod.TotalCredit, "Итого обороты за период", isHeader: true, forceZeroes: true));
            report0006info.Model.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber,
                balanceInfoByPeriod.EndingBalance, String.Format("Сальдо на {0}", report0006info.EndDate.ToShortDateString()), isHeader: true));
        }

        /// <summary>
        /// Вычисляем начальный, конечный баланс по собственной организации, общие обороты за период и суммы по каждому виду оборота по списку сделок
        /// </summary>
        /// <param name="dealList">Коллекция сделок</param>
        /// <param name="report0006info">Данные отчета за период</param>
        /// <returns>Информация о сальдо по группе сделок за период</returns>
        private DealBalanceInfoByPeriod CalculateDealBalanceInfoByPeriod(IEnumerable<ExpenditureWaybill> expenditureWaybillList, IEnumerable<ReturnFromClientWaybill> returnFromClientWaybillList,
            IEnumerable<DealPayment> dealPaymentList, IEnumerable<DealInitialBalanceCorrection> dealInitialBalanceCorrectionList,
            IEnumerable<InitialBalanceInfo> startingBalanceInfo, Report0006Info report0006info)
        {
            var balanceInfoByPeriod = new DealBalanceInfoByPeriod();

            balanceInfoByPeriod.StartingBalance += startingBalanceInfo.Sum(x => x.Sum);
            balanceInfoByPeriod.SaleWaybillSum += expenditureWaybillList.Sum(x => x.SalePriceSum);
            balanceInfoByPeriod.ReturnFromClientWaybillSum += returnFromClientWaybillList.Sum(x => x.SalePriceSum);
            balanceInfoByPeriod.DealPaymentFromClientCashlessSum += dealPaymentList
                .Where(x => x.Type == DealPaymentDocumentType.DealPaymentFromClient && x.DealPaymentForm == DealPaymentForm.Cashless)
                .Sum(x => x.Sum);
            balanceInfoByPeriod.DealPaymentFromClientCashSum += dealPaymentList
                .Where(x => x.Type == DealPaymentDocumentType.DealPaymentFromClient && x.DealPaymentForm == DealPaymentForm.Cash)
                .Sum(x => x.Sum);
            balanceInfoByPeriod.DealPaymentFromClientThirdPartyCashlessSum += dealPaymentList
                .Where(x => x.Type == DealPaymentDocumentType.DealPaymentFromClient && x.DealPaymentForm == DealPaymentForm.ThirdPartyCashless)
                .Sum(x => x.Sum);
            balanceInfoByPeriod.DealPaymentToClientCashlessSum += dealPaymentList
                .Where(x => x.Type == DealPaymentDocumentType.DealPaymentToClient && x.DealPaymentForm == DealPaymentForm.Cashless)
                .Sum(x => x.Sum);
            balanceInfoByPeriod.DealPaymentToClientCashSum += dealPaymentList
                .Where(x => x.Type == DealPaymentDocumentType.DealPaymentToClient && x.DealPaymentForm == DealPaymentForm.Cash)
                .Sum(x => x.Sum);
            balanceInfoByPeriod.DealPaymentToClientThirdPartyCashlessSum += dealPaymentList
                .Where(x => x.Type == DealPaymentDocumentType.DealPaymentToClient && x.DealPaymentForm == DealPaymentForm.ThirdPartyCashless)
                .Sum(x => x.Sum);
            balanceInfoByPeriod.DealCreditInitialBalanceCorrectionSum += dealInitialBalanceCorrectionList
                .Where(x => x.Type == DealPaymentDocumentType.DealCreditInitialBalanceCorrection)
                .Sum(x => x.Sum);
            balanceInfoByPeriod.DealDebitInitialBalanceCorrectionSum += dealInitialBalanceCorrectionList
                .Where(x => x.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection)
                .Sum(x => x.Sum);

            return balanceInfoByPeriod;
        }

        /// <summary>
        /// "Очистить" список строк, вернув список, который содержит столько null, сколько было элементов
        /// </summary>
        /// <param name="list">Исходный список строк</param>
        /// <returns>Список из null</returns>
        private List<string> EmptyStringList(IList<string> list)
        {
            var result = new List<string>();
            var capacity = list.Count;
            for (int i = 0; i < capacity; i++)
            {
                result.Add(null);
            }

            return result;
        }

        #endregion

        #region Выгрузка в Excel
        /// <summary>
        /// Экспорт отчета в Excel
        /// </summary>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="currentUser">Текущий пользователь</param>
        public byte[] Report0006ExportToExcel(Report0006SettingsViewModel settings, UserInfo currentUser)
        {
            var viewModel = Report0006(settings, currentUser);

            string reportHeader = viewModel.ReportName + " \r\nпо состоянию на " + viewModel.Date + "\r\nза период с " + viewModel.Settings.StartDate + " по " + viewModel.Settings.EndDate;
            string sign = "Форма: Report0006" + "\r\nАвтор: " + viewModel.CreatedBy + "\r\nСоставлен: " + viewModel.DateTime;
            
            using (ExcelPackage pck = new ExcelPackage())
            {
                if (viewModel.Settings.ShowClientSummary == "1" || viewModel.Settings.ShowClientOrganizationSummary == "1")
                {
                    if (viewModel.Settings.ShowClientSummary == "1")
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по клиентам");
                        FillClientSummaryTable(sheet, 5, viewModel, sheet.PrintHeader(5, reportHeader, sign, "Сводная информация по клиентам:", 1));
                    }

                    if (viewModel.Settings.ShowClientOrganizationSummary == "1")
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по организациям клиентов");
                        FillClientOrganizationSummaryTable(sheet, 5, viewModel, sheet.PrintHeader(5, reportHeader, sign, "Сводная информация по организациям клиентов:", 1));
                    }
                }
    
                if (viewModel.Settings.ShowClientContractSummary == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Список открытых договоров");
                    FillClientContractSummaryTable(sheet, 4, viewModel, sheet.PrintHeader(4, reportHeader, sign, "Список открытых договоров в период:", 1));
                }
    
                if (viewModel.Settings.ShowBalanceDocumentSummary == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Общая информация по взаиморасчетам");
                    FillBalanceDocumentSummaryTable(sheet, 4, viewModel, sheet.PrintHeader(4, reportHeader, sign, "Общая информация по взаиморасчетам:", 1));
                }
    
                if (viewModel.Settings.ShowBalanceDocumentFullInfo == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Развернутая информация по документам учета");
                    FillBalanceDocumentFullInfoTable(sheet, 4, viewModel, sheet.PrintHeader(4, reportHeader, sign, "Развернутая информация по документам учета:", 1));
                }


                if (pck.Workbook.Worksheets.Any())
                {
                    pck.Workbook.Worksheets[1].View.TabSelected = true;
                }

                //Возвращаем файл
                return pck.GetAsByteArray();
            }
        }


        /// <summary>
        /// Формирует таблицу с данными финансового отчета 
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillClientSummaryTable(ExcelWorksheet workSheet, int columns, Report0006ViewModel viewModel, int startRow)
        { 
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
             //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Клиент");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Нач. сальдо");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Дебет");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Кредит");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Кон. сальдо");
            currentCol=1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки
            bool flag = false;
            foreach (var item in viewModel.ClientSummary)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle(): ExcelUtils.GetTableUnEvenRowStyle());
                
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Name).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.StartingBalance, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Debit, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Credit, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.EndingBalance, ValueDisplayType.Money);
                currentCol = 1;
                currentRow++;
                flag = !flag;
            }
            if (viewModel.ClientSummary.Count == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).MergeRange();
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("нет данных");
                currentCol = 1;
                currentRow++;
            }

            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(100);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        /// <summary>
        /// Формирует таблицу с данными финансового отчета 
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillClientOrganizationSummaryTable(ExcelWorksheet workSheet, int columns, Report0006ViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Организация клиента");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Нач. сальдо");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Дебет");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Кредит");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Кон. сальдо");
            currentCol = 1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки
            bool flag = false;
            foreach (var item in viewModel.ClientOrganizationSummary)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle());

                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Name).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.StartingBalance, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Debit, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Credit, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.EndingBalance, ValueDisplayType.Money);
                currentCol = 1;
                currentRow++;
                flag = !flag;
            }
            if (viewModel.ClientOrganizationSummary.Count == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).MergeRange();
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("нет данных");
                currentCol = 1;
                currentRow++;
            }

            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(100);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        /// <summary>
        /// Формирует таблицу с данными финансового отчета 
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillClientContractSummaryTable(ExcelWorksheet workSheet, int columns, Report0006ViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Клиент");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Договор");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Одна сторона");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Вторая сторона");
            currentCol = 1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки
            bool flag = false;
            foreach (var item in viewModel.ClientContractSummary)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle());

                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ClientName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ClientContractFullName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.AccountOrganizationShortName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ClientOrganizationShortName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                currentCol = 1;
                currentRow++;
                flag = !flag;
            }
            if (viewModel.ClientContractSummary.Count == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).MergeRange();
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("нет данных");
                currentCol = 1;
                currentRow++;
            }

            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(100);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        /// <summary>
        /// Формирует таблицу с данными финансового отчета 
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillBalanceDocumentSummaryTable(ExcelWorksheet workSheet, int columns, Report0006ViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("№ п/п");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Наименование операции");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Дебет");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Кредит");
            currentCol = 1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки
            bool flag = false;
            foreach (var item in viewModel.BalanceDocumentSummary)
            {
                if (item.IsGroupHeader) {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle())
                        .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                }
                else if (item.IsHeader) {
                        workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableArticleSubTotalRowStyle())
                            .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right, fontStyle: FontStyle.Bold);
                    }
                    else
                    {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ?ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle())
                            .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    }

                if (!item.IsGroupHeader)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                }
                   
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Name).ChangeRangeStyle(indent: item.GroupHeaderLevel);
                currentCol++;

                if (!item.IsGroupHeader)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Debit, ValueDisplayType.Money).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right); ;
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Credit, ValueDisplayType.Money).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right); ;
                    currentCol++;
                }
                currentCol = 1;
                currentRow++;
                flag = !flag;
            }
            if (viewModel.BalanceDocumentSummary.Count == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).MergeRange();
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("нет данных");
                currentCol = 1;
                currentRow++;
            }

            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(100);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        /// <summary>
        /// Формирует таблицу с данными финансового отчета 
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillBalanceDocumentFullInfoTable(ExcelWorksheet workSheet, int columns, Report0006ViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("№ п/п");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Наименование операции");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Дебет");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Кредит");
            currentCol = 1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки
            bool flag = false;
            foreach (var item in viewModel.BalanceDocumentFullInfo)
            {
                if (item.IsGroupHeader)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle())
                        .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                }
                else if (item.IsHeader)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableArticleSubTotalRowStyle())
                        .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right, fontStyle: FontStyle.Bold);
                }
                else
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle())
                        .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                }

                if (!item.IsGroupHeader)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                }

                workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Name).ChangeRangeStyle(indent: item.GroupHeaderLevel);
                currentCol++;

                if (!item.IsGroupHeader)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Debit, ValueDisplayType.Money).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right); ;
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Credit, ValueDisplayType.Money).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right); ;
                    currentCol++;
                }
                currentCol = 1;
                currentRow++;
                flag = !flag;
            }
            if (viewModel.BalanceDocumentFullInfo.Count == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).MergeRange();
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("нет данных");
                currentCol = 1;
                currentRow++;
            }

            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(100);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        #endregion

        #region Report0006 Построение печатной формы

        /// <summary>
        /// Построение печатной формы "Акт сверки взаиморасчетов"
        /// </summary>
        /// <param name="settings">Настройки</param>
        /// <param name="currentUser">Пользователь</param>
        public Report0006PrintingFormListViewModel Report0006PrintingForm(Report0006PrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var currentDate = DateTimeUtils.GetCurrentDateTime();
                var user = userService.CheckUserExistence(currentUser.Id);

                var isPrintingFormByClient = String.IsNullOrEmpty(settings.PrintingFormClientOrganizationId);
                user.CheckPermission(isPrintingFormByClient ? Permission.Client_List_Details : Permission.ClientOrganization_List_Details);
                user.CheckPermission(Permission.Deal_Balance_View);
                user.CheckPermission(Permission.ExpenditureWaybill_List_Details);
                user.CheckPermission(Permission.ReturnFromClientWaybill_List_Details);
                user.CheckPermission(Permission.DealPayment_List_Details);
                user.CheckPermission(Permission.DealInitialBalanceCorrection_List_Details);

                DateTime startDate, endDate;
                base.ParseDatePeriod(settings.StartDate, settings.EndDate, currentDate, out startDate, out endDate);

                var model = new Report0006PrintingFormListViewModel
                {
                    Date = currentDate.ToShortDateString(),
                    Settings = settings
                };

                Client client = null;
                ClientOrganization clientOrganization = null;
                // Список {AccountOrganizationId, ClientId, ClientOrganizationId, ContractId, TeamId, Баланс}
                IList<InitialBalanceInfo> startingBalanceInfo;
                IDictionary<Guid, ExpenditureWaybill> expenditureWaybillList;
                IDictionary<Guid, ReturnFromClientWaybill> returnFromClientWaybillList;
                IDictionary<Guid, DealPayment> dealPaymentList;
                IDictionary<Guid, DealInitialBalanceCorrection> dealInitialBalanceCorrectionList;
                if (isPrintingFormByClient)
                {
                    client = clientService.CheckClientExistence(ValidationUtils.TryGetInt(settings.PrintingFormClientId), user);
                    model.ClientOrClientOrganizationName = client.Name;

                    // Проверяем, есть ли сделки вне области видимости
                    ValidationUtils.Assert(!dealIndicatorService.AreAnyRestrictedDeals(client,
                        new List<Permission> { Permission.Deal_Balance_View, Permission.ExpenditureWaybill_List_Details, Permission.ReturnFromClientWaybill_List_Details,
                        Permission.DealPayment_List_Details, Permission.DealInitialBalanceCorrection_List_Details },
                        user),
                        "Существуют сделки, на которые недостаточно прав. Невозможно напечатать форму.");

                    startingBalanceInfo = dealIndicatorService.CalculateBalanceOnDateByClient(client, startDate, user);

                    expenditureWaybillList = expenditureWaybillService.GetShippedListInDateRangeByClientList(startDate, endDate, new List<Client> { client });
                    returnFromClientWaybillList = returnFromClientWaybillService.GetReceiptedListInDateRangeByClientList(startDate, endDate, new List<Client> { client });
                    dealPaymentList = dealPaymentDocumentService.GetDealPaymentListInDateRangeByClientList(startDate, endDate, new List<Client> { client });
                    dealInitialBalanceCorrectionList = dealPaymentDocumentService.GetDealInitialBalanceCorrectionListInDateRangeByClientList(startDate, endDate, new List<Client> { client });
                }
                else
                {
                    clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(ValidationUtils.TryGetInt(settings.PrintingFormClientOrganizationId), user);
                    model.ClientOrClientOrganizationName = clientOrganization.ShortName;

                    // Проверяем, есть ли сделки вне области видимости
                    ValidationUtils.Assert(!dealIndicatorService.AreAnyRestrictedDeals(clientOrganization,
                        new List<Permission> { Permission.Deal_Balance_View, Permission.ExpenditureWaybill_List_Details, Permission.ReturnFromClientWaybill_List_Details,
                        Permission.DealPayment_List_Details, Permission.DealInitialBalanceCorrection_List_Details },
                        user),
                        "Существуют сделки, на которые недостаточно прав. Невозможно напечатать форму.");

                    startingBalanceInfo = dealIndicatorService.CalculateBalanceOnDateByClientOrganization(clientOrganization, startDate, user);

                    expenditureWaybillList = expenditureWaybillService.GetShippedListInDateRangeByClientOrganizationList(startDate, endDate, new List<ClientOrganization> { clientOrganization });
                    returnFromClientWaybillList = returnFromClientWaybillService.GetReceiptedListInDateRangeByClientOrganizationList(startDate, endDate, new List<ClientOrganization> { clientOrganization });
                    dealPaymentList = dealPaymentDocumentService.GetDealPaymentListInDateRangeByClientOrganizationList(startDate, endDate, new List<ClientOrganization> { clientOrganization });
                    dealInitialBalanceCorrectionList = dealPaymentDocumentService.GetDealInitialBalanceCorrectionListInDateRangeByClientOrganizationList(startDate, endDate, new List<ClientOrganization> { clientOrganization });
                }

                var accountOrganizationIdList = new List<int>();
                accountOrganizationIdList.AddRange(startingBalanceInfo.Select(x => x.AccountOrganizationId));
                accountOrganizationIdList.AddRange(expenditureWaybillList.Values.Select(x => x.Deal.Contract.AccountOrganization.Id));
                accountOrganizationIdList.AddRange(returnFromClientWaybillList.Values.Select(x => x.Deal.Contract.AccountOrganization.Id));
                accountOrganizationIdList.AddRange(dealPaymentList.Values.Select(x => x.Deal.Contract.AccountOrganization.Id));
                accountOrganizationIdList.AddRange(dealInitialBalanceCorrectionList.Values.Select(x => x.Deal.Contract.AccountOrganization.Id));

                // Подгружаем все необходимые собственные организации
                var accountOrganizations = accountOrganizationService.GetList(accountOrganizationIdList.Distinct());

                var startingBalanceInfoGroup = startingBalanceInfo.GroupBy(x => x.AccountOrganizationId).ToDictionary(x => x.Key);

                // Создаем модели отчетов по каждой собственной организации
                foreach (var accountOrganization in accountOrganizations.Values)
                {
                    var isJuridicalPerson = accountOrganization.EconomicAgent.Is<JuridicalPerson>();
                    var juridicalPerson = accountOrganization.EconomicAgent.As<JuridicalPerson>();
                    var physicalPerson = accountOrganization.EconomicAgent.As<PhysicalPerson>();

                    var formModel = new Report0006PrintingFormViewModel()
                    {
                        AccountOrganizationName = accountOrganization.ShortName,
                        IsJuridicalPerson = isJuridicalPerson,
                        DirectorPost = isJuridicalPerson ? juridicalPerson.DirectorPost : "",
                        DirectorName = isJuridicalPerson ? juridicalPerson.DirectorName : "",
                        MainBookkeeperName = isJuridicalPerson ? juridicalPerson.MainBookkeeperName : "",
                        OwnerName = isJuridicalPerson ? "" : physicalPerson.OwnerName
                    };

                    #region Вычисляем начальный, конечный баланс по собственной организации, общие обороты за период и суммы по каждому виду оборота

                    var balanceInfoByPeriod = new DealBalanceInfoByPeriod();
                    balanceInfoByPeriod.StartingBalance = startingBalanceInfoGroup.Keys.Contains(accountOrganization.Id) ?
                        startingBalanceInfoGroup[accountOrganization.Id].Sum(x => x.Sum) : 0M;
                    balanceInfoByPeriod.DealCreditInitialBalanceCorrectionSum = dealInitialBalanceCorrectionList
                        .Where(x => x.Value.Deal.Contract.AccountOrganization == accountOrganization)
                        .Where(x => x.Value.Type == DealPaymentDocumentType.DealCreditInitialBalanceCorrection)
                        .Sum(x => x.Value.Sum);
                    balanceInfoByPeriod.DealDebitInitialBalanceCorrectionSum = dealInitialBalanceCorrectionList
                        .Where(x => x.Value.Deal.Contract.AccountOrganization == accountOrganization)
                        .Where(x => x.Value.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection)
                        .Sum(x => x.Value.Sum);
                    balanceInfoByPeriod.DealPaymentFromClientCashlessSum = dealPaymentList
                        .Where(x => x.Value.Deal.Contract.AccountOrganization == accountOrganization)
                        .Where(x => x.Value.Type == DealPaymentDocumentType.DealPaymentFromClient && x.Value.DealPaymentForm == DealPaymentForm.Cashless)
                        .Sum(x => x.Value.Sum);
                    balanceInfoByPeriod.DealPaymentFromClientCashSum = dealPaymentList
                        .Where(x => x.Value.Deal.Contract.AccountOrganization == accountOrganization)
                        .Where(x => x.Value.Type == DealPaymentDocumentType.DealPaymentFromClient && x.Value.DealPaymentForm == DealPaymentForm.Cash)
                        .Sum(x => x.Value.Sum);
                    balanceInfoByPeriod.DealPaymentFromClientThirdPartyCashlessSum = dealPaymentList
                        .Where(x => x.Value.Deal.Contract.AccountOrganization == accountOrganization)
                        .Where(x => x.Value.Type == DealPaymentDocumentType.DealPaymentFromClient && x.Value.DealPaymentForm == DealPaymentForm.ThirdPartyCashless)
                        .Sum(x => x.Value.Sum);
                    balanceInfoByPeriod.DealPaymentToClientCashlessSum = dealPaymentList
                        .Where(x => x.Value.Deal.Contract.AccountOrganization == accountOrganization)
                        .Where(x => x.Value.Type == DealPaymentDocumentType.DealPaymentToClient && x.Value.DealPaymentForm == DealPaymentForm.Cashless)
                        .Sum(x => x.Value.Sum);
                    balanceInfoByPeriod.DealPaymentToClientCashSum = dealPaymentList
                        .Where(x => x.Value.Deal.Contract.AccountOrganization == accountOrganization)
                        .Where(x => x.Value.Type == DealPaymentDocumentType.DealPaymentToClient && x.Value.DealPaymentForm == DealPaymentForm.Cash)
                        .Sum(x => x.Value.Sum);
                    balanceInfoByPeriod.DealPaymentToClientThirdPartyCashlessSum = dealPaymentList
                        .Where(x => x.Value.Deal.Contract.AccountOrganization == accountOrganization)
                        .Where(x => x.Value.Type == DealPaymentDocumentType.DealPaymentToClient && x.Value.DealPaymentForm == DealPaymentForm.ThirdPartyCashless)
                        .Sum(x => x.Value.Sum);
                    balanceInfoByPeriod.ReturnFromClientWaybillSum = returnFromClientWaybillList
                        .Where(x => x.Value.Deal.Contract.AccountOrganization == accountOrganization)
                        .Sum(x => x.Value.SalePriceSum);
                    balanceInfoByPeriod.SaleWaybillSum = expenditureWaybillList
                        .Where(x => x.Value.Deal.Contract.AccountOrganization == accountOrganization)
                        .Sum(x => x.Value.SalePriceSum);

                    #endregion

                    #region Заполняем таблицу 1 ("Состояние взаимных расчетов по данным учета")

                    int rowNumber = 0;
                    formModel.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber, balanceInfoByPeriod.StartingBalance,
                        String.Format("Сальдо на {0}", startDate.ToShortDateString()), isHeader: true));
                    formModel.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber,
                        balanceInfoByPeriod.SaleWaybillSum, balanceInfoByPeriod.ReturnFromClientWaybillSum, "Отгрузка (возвраты) товаров"));
                    formModel.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber,
                        balanceInfoByPeriod.DealPaymentToClientCashSum, balanceInfoByPeriod.DealPaymentFromClientCashSum, "Оплаты за наличный расчет"));
                    formModel.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber,
                        balanceInfoByPeriod.DealPaymentToClientCashlessSum, balanceInfoByPeriod.DealPaymentFromClientCashlessSum, "Оплаты за безналичный расчет"));
                    formModel.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber,
                        balanceInfoByPeriod.DealPaymentToClientThirdPartyCashlessSum, balanceInfoByPeriod.DealPaymentFromClientThirdPartyCashlessSum, "Оплаты, принятые от третих лиц"));
                    formModel.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber,
                        balanceInfoByPeriod.DealDebitInitialBalanceCorrectionSum, balanceInfoByPeriod.DealCreditInitialBalanceCorrectionSum, "Корректировки сальдо"));
                    formModel.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber,
                        balanceInfoByPeriod.TotalDebit, balanceInfoByPeriod.TotalCredit, "Итого обороты за период", isHeader: true, forceZeroes: true));
                    formModel.BalanceDocumentSummary.Add(new Report0006BalanceItemViewModel(++rowNumber,
                        balanceInfoByPeriod.EndingBalance, String.Format("Сальдо на {0}", endDate.ToShortDateString()), isHeader: true));

                    #endregion

                    #region Заполняем таблицу 2 ("Развернутая информация по документам учета")

                    var balanceDocumentFullInfoList = new List<BalanceDocumentFullInfo>();

                    FillExpenditureWaybillFullInfo(expenditureWaybillList.Values.Where(x => x.Deal.Contract.AccountOrganization == accountOrganization), startDate, endDate, balanceDocumentFullInfoList);
                    FillReturnFromClientWaybillFullInfo(returnFromClientWaybillList.Values.Where(x => x.Deal.Contract.AccountOrganization == accountOrganization), startDate, endDate, balanceDocumentFullInfoList);
                    FillDealPaymentFullInfo(dealPaymentList.Values.Where(x => x.Deal.Contract.AccountOrganization == accountOrganization), startDate, endDate, balanceDocumentFullInfoList);
                    FillDealInitialBalanceCorrectionFullInfo(dealInitialBalanceCorrectionList.Values.Where(x => x.Deal.Contract.AccountOrganization == accountOrganization), startDate, endDate, balanceDocumentFullInfoList);

                    FillBalanceDocumentFullInfo(balanceDocumentFullInfoList, balanceInfoByPeriod, startDate, endDate, formModel.BalanceDocumentFullInfo);

                    #endregion

                    model.Report0006PrintingFormList.Add(formModel);
                }

                return model;
            }
        }

        #endregion

        #region Методы обработки данных
               

        /// <summary>
        /// Разобрать строку с кодами группировок
        /// </summary>
        /// <param name="groupingString">Строка</param>
        private IList<GroupingType> ParseGroupingString(string groupingString)
        {
            bool isLastGrouping = false;

            var result = new List<GroupingType>();
            if (!String.IsNullOrEmpty(groupingString))
            {
                var splitResult = groupingString.Split('_').ToList();
                ValidationUtils.Assert(splitResult.Count == splitResult.Distinct().Count(),
                    @"В таблице ""Порядок группировки информации"" заданы повторяющиеся элементы.");

                foreach (var item in splitResult)
                {
                    var value = (GroupingType)ValidationUtils.TryGetByte(item);
                    ValidationUtils.Assert(Enum.IsDefined(typeof(GroupingType), value), "Неизвестный тип группировки.");

                    ValidationUtils.Assert(!isLastGrouping, "Запрещается добавлять группировки после группировки по договору.");
                    result.Add(value);

                    // Запрещается добавлять группировки после группировки по договору
                    if (value == GroupingType.ClientContract)
                    {
                        isLastGrouping = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Заполнить полную информацию о накладных реализации
        /// </summary>
        /// <param name="expenditureWaybillList">Список накладных</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="balanceDocumentFullInfoList">Коллекция внутренних объектов для информации о накладных</param>
        private void FillExpenditureWaybillFullInfo(IEnumerable<ExpenditureWaybill> expenditureWaybillList, DateTime startDate, DateTime endDate,
            IList<BalanceDocumentFullInfo> balanceDocumentFullInfoList)
        {
            foreach (var expenditureWaybill in expenditureWaybillList)
            {
                var additionalInfoList = new List<string>();

                if (expenditureWaybill.AcceptanceDate.Value >= startDate && expenditureWaybill.AcceptanceDate.Value <= endDate)
                {
                    additionalInfoList.Add(String.Format("Проведено: {0}", expenditureWaybill.AcceptanceDate.Value.ToShortDateString()));
                }
                if (expenditureWaybill.ShippingDate.HasValue && expenditureWaybill.ShippingDate.Value >= startDate && expenditureWaybill.ShippingDate.Value <= endDate)
                {
                    additionalInfoList.Add(String.Format("Отгружено: {0}", expenditureWaybill.ShippingDate.Value.ToShortDateString()));
                }

                balanceDocumentFullInfoList.Add(new BalanceDocumentFullInfo(expenditureWaybill.CreationDate,
                    expenditureWaybill.AcceptanceDate.Value, expenditureWaybill.SalePriceSum, true,
                    String.Format("Реализация товаров {0} по договору {1}", expenditureWaybill.Name, expenditureWaybill.Deal.Contract.FullName),
                    additionalInfoList));
            }
        }

        /// <summary>
        /// Заполнить полную информацию о накладных возврата товаров
        /// </summary>
        /// <param name="returnFromClientWaybillList">Список накладных возврата</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="balanceDocumentFullInfoList">Коллекция внутренних объектов для информации о накладных</param>
        private void FillReturnFromClientWaybillFullInfo(IEnumerable<ReturnFromClientWaybill> returnFromClientWaybillList,
            DateTime startDate, DateTime endDate, IList<BalanceDocumentFullInfo> balanceDocumentFullInfoList)
        {
            foreach (var returnFromClientWaybill in returnFromClientWaybillList)
            {
                var additionalInfoList = new List<string>();

                if (returnFromClientWaybill.AcceptanceDate.Value >= startDate && returnFromClientWaybill.AcceptanceDate.Value <= endDate)
                {
                    additionalInfoList.Add(String.Format("Проведен: {0}", returnFromClientWaybill.AcceptanceDate.Value.ToShortDateString()));
                }
                if (returnFromClientWaybill.ReceiptDate.HasValue &&
                    returnFromClientWaybill.ReceiptDate.Value >= startDate && returnFromClientWaybill.ReceiptDate.Value <= endDate)
                {
                    additionalInfoList.Add(String.Format("Принят: {0}", returnFromClientWaybill.ReceiptDate.Value.ToShortDateString()));
                }

                balanceDocumentFullInfoList.Add(new BalanceDocumentFullInfo(returnFromClientWaybill.CreationDate,
                    returnFromClientWaybill.AcceptanceDate.Value, returnFromClientWaybill.SalePriceSum, false,
                    String.Format("Возврат товаров {0} по договору {1}", returnFromClientWaybill.Name, returnFromClientWaybill.Deal.Contract.FullName),
                    additionalInfoList));
            }
        }

        /// <summary>
        /// Заполнить полную информацию об оплатах и возвратах оплат
        /// </summary>
        /// <param name="dealPaymentList">Список оплат и возвратов оплат</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="balanceDocumentFullInfoList">Коллекция внутренних объектов для информации об оплатах и возвратах оплат</param>
        private void FillDealPaymentFullInfo(IEnumerable<DealPayment> dealPaymentList,
            DateTime startDate, DateTime endDate, IList<BalanceDocumentFullInfo> balanceDocumentFullInfoList)
        {
            foreach (var dealPayment in dealPaymentList)
            {
                string type = dealPayment.Type == DealPaymentDocumentType.DealPaymentFromClient ?
                    "Оплата" : "Возврат оплаты";

                string paymentForm;
                switch (dealPayment.DealPaymentForm)
                {
                    case DealPaymentForm.Cash:
                        paymentForm = "наличный расчет";
                        break;
                    case DealPaymentForm.Cashless:
                        paymentForm = "безналичный расчет";
                        break;
                    case DealPaymentForm.ThirdPartyCashless:
                        paymentForm = dealPayment.Type == DealPaymentDocumentType.DealPaymentFromClient ?
                            "принята от третьего лица" : "передан третьему лицу";
                        break;
                    default:
                        throw new Exception("Неизвестная форма оплаты по сделке.");
                };

                balanceDocumentFullInfoList.Add(new BalanceDocumentFullInfo(dealPayment.CreationDate,
                    dealPayment.Date, dealPayment.Sum, dealPayment.Type == DealPaymentDocumentType.DealPaymentToClient,
                    String.Format("{0} № {1} от {2} по договору {3} ({4})", type, dealPayment.PaymentDocumentNumber,
                        dealPayment.Date.ToShortDateString(), dealPayment.Deal.Contract.FullName,
                        paymentForm)));
            }
        }

        /// <summary>
        /// Заполнить полную информацию о корректировках сальдо
        /// </summary>
        /// <param name="dealInitialBalanceCorrectionList">Список корректировок сальдо</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="balanceDocumentFullInfoList">Коллекция внутренних объектов для информации о корректировках сальдо</param>
        private void FillDealInitialBalanceCorrectionFullInfo(IEnumerable<DealInitialBalanceCorrection> dealInitialBalanceCorrectionList,
            DateTime startDate, DateTime endDate, IList<BalanceDocumentFullInfo> balanceDocumentFullInfoList)
        {
            foreach (var dealInitialBalanceCorrection in dealInitialBalanceCorrectionList)
            {
                balanceDocumentFullInfoList.Add(new BalanceDocumentFullInfo(dealInitialBalanceCorrection.CreationDate,
                    dealInitialBalanceCorrection.Date, dealInitialBalanceCorrection.Sum,
                    dealInitialBalanceCorrection.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection,
                    String.Format("Корректировки сальдо от {0} по причине: {1}", dealInitialBalanceCorrection.Date.ToShortDateString(),
                        dealInitialBalanceCorrection.CorrectionReason)));
            }
        }

        /// <summary>
        /// Заполнить полную информацию о документах учета в модели отчета / печатной формы
        /// </summary>
        /// <param name="balanceDocumentFullInfoList">Коллекция внутренних классов с информацией о документах учета</param>
        /// <param name="balanceInfoByPeriod">Вычисленная информация о сальдо на период</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="balanceDocumentFullInfo">Коллекция ViewModels</param>
        private void FillBalanceDocumentFullInfo(IList<BalanceDocumentFullInfo> balanceDocumentFullInfoList, DealBalanceInfoByPeriod balanceInfoByPeriod,
            DateTime startDate, DateTime endDate, IList<Report0006BalanceItemViewModel> balanceDocumentFullInfo)
        {
            int rowNumber = 0;
            balanceDocumentFullInfo.Add(new Report0006BalanceItemViewModel(++rowNumber,
                balanceInfoByPeriod.StartingBalance, String.Format("Сальдо на {0}", startDate.ToShortDateString()), isHeader: true));

            foreach (var balanceDocumentFullInfoItem in balanceDocumentFullInfoList.OrderBy(x => x.Date).ThenBy(x => x.CreationDate))
            {
                balanceDocumentFullInfo.Add(new Report0006BalanceItemViewModel(++rowNumber,
                    balanceDocumentFullInfoItem.Debit, balanceDocumentFullInfoItem.Credit, balanceDocumentFullInfoItem.Name,
                    balanceDocumentFullInfoItem.AdditionalInfo1, balanceDocumentFullInfoItem.AdditionalInfo2));
            }

            balanceDocumentFullInfo.Add(new Report0006BalanceItemViewModel(++rowNumber,
                balanceInfoByPeriod.TotalDebit, balanceInfoByPeriod.TotalCredit, "Итого обороты за период", isHeader: true, forceZeroes: true));
            balanceDocumentFullInfo.Add(new Report0006BalanceItemViewModel(++rowNumber,
                balanceInfoByPeriod.EndingBalance, String.Format("Сальдо на {0}", endDate.ToShortDateString()), isHeader: true));
        }

        #endregion

        #endregion
    }
}