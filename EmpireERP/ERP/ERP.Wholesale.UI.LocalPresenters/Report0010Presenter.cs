using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Report.Report0010;
using OfficeOpenXml;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class Report0010Presenter : BaseReportPresenter, IReport0010Presenter
    {
        #region Внутренние классы

        /// <summary>
        /// Тип группировки в отчете
        /// </summary>
        private enum GroupingType
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
            Team = 4,

            /// <summary>
            /// По пользователям
            /// </summary>
            [EnumDisplayName("Пользователь")]
            User = 5,

            /// <summary>
            /// По собственной организации
            /// </summary>
            [EnumDisplayName("Собственная организация")]
            AccountOrganization = 6
        }

        private class DealPaymentInfo
        {
            /// <summary>
            /// Дата документа
            /// </summary>
            public DateTime Date { get; set; }

            /// <summary>
            /// Тип + номер документа
            /// </summary>
            public string PaymentDocument { get; set; }

            /// <summary>
            /// Форма оплаты
            /// </summary>
            public DealPaymentForm PaymentForm { get; set; }

            /// <summary>
            /// Тип платежного документа
            /// </summary>
            public DealPaymentDocumentType Type { get; set; }

            public int ClientId { get; set; }
            public string ClientName { get; set; }

            public int ClientOrganizationId { get; set; }
            public string ClientOrganizationName { get; set; }

            public int ClientContractId { get; set; }
            public string ClientContractName { get; set; }

            public int AccountOrganizationId { get; set; }
            public string AccountOrganizationName { get; set; }

            public short TeamId { get; set; }
            public string TeamName { get; set; }

            public int UserId { get; set; }
            public string UserName { get; set; }
                       
            /// <summary>
            /// Сумма платежа
            /// </summary>
            public decimal PaymentSum { get; set; }

            /// <summary>
            /// Разнесенная сумма платежа
            /// </summary>
            public decimal DistributedSum { get; set; }

            /// <summary>
            /// Разнесенная сумма на накладные
            /// </summary>
            public decimal DistributedToSaleWaybillPaymentSum { get; set; }

            /// <summary>
            /// Разнесенная сумма на корректировки сальдо
            /// </summary>
            public decimal DistributedToBalanceCorrectionPaymentSum { get; set; }

            /// <summary>
            /// Возвращено из данной оплаты
            /// </summary>
            public decimal PaymentToClientSum { get; set; }

            /// <summary>
            /// Неразнесенная сумма платежа
            /// </summary>
            public decimal UndistributedSum { get; set; }
        }

        #endregion

        #region Поля
                
        private readonly IClientService clientService;
        private readonly ITeamService teamService;
        private readonly IDealPaymentDocumentService dealPaymentDocumentService;
        private readonly IAccountOrganizationService accountOrganizationService;
                
        #endregion

        #region Конструктор

        public Report0010Presenter(IUnitOfWorkFactory unitOfWorkFactory, IUserService userService, IClientService clientService, 
            IAccountOrganizationService accountOrganizationService, ITeamService teamService, IDealPaymentDocumentService dealPaymentDocumentService) :
            base(unitOfWorkFactory, userService)
        {            
            this.clientService = clientService;
            this.accountOrganizationService = accountOrganizationService;
            this.teamService = teamService;
            this.dealPaymentDocumentService = dealPaymentDocumentService;
        }

        #endregion

        #region Методы

        #region  Настройка отчета

        public Report0010SettingsViewModel Report0010Settings(string backUrl, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                CheckReportPermissions(user);

                var currentDate = DateTimeUtils.GetCurrentDateTime();

                var model = new Report0010SettingsViewModel()
                {
                    BackURL = backUrl,

                    StartDate = new DateTime(currentDate.Year, currentDate.Month, 1).ToShortDateString(),
                    EndDate = currentDate.ToShortDateString(),

                    GroupByCollection = ComboBoxBuilder.GetComboBoxItemList<GroupingType>(addEmptyItem: false, sort: false),
                    ClientList = clientService.GetList(user).OrderBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name),
                    AccountOrganizationList = accountOrganizationService.GetList().OrderBy(x => x.ShortName).ToDictionary(x => x.Id.ToString(), x => x.ShortName),
                    TeamList = teamService.GetList(user, Permission.Team_List_Details).OrderBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name),
                    UserList = userService.GetList(user).OrderBy(x => x.DisplayName).ToDictionary(x => x.Id.ToString(), x => x.DisplayName),

                    SeparateByDealPaymentForm = "1",

                    ShowClientSummary = "1",                    
                    ShowClientOrganizationSummary = "1",
                    ShowAccountOrganizationSummary = "1",
                    ShowClientContractSummary = "1",
                    ShowTeamSummary = "1",
                    ShowUserSummary = "1",

                    ShowDetailsTable = "1",
                    ShowDistributedAndUndistributedSums = "1",
                    ShowDistributionDetails = "1",

                    GroupByCollectionIDs = ""
                };

                return model;
            }
        }

        #endregion

        #region Построение отчета

        /// <summary>
        /// Построение отчета
        /// </summary>
        /// <param name="settings">Модель настроек отчета</param>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <returns></returns>
        public Report0010ViewModel Report0010(Report0010SettingsViewModel settings, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                #region Чтение основных параметров отчета

                ValidationUtils.NotNull(settings, "Входной параметр задан неверно.");
                var user = userService.CheckUserExistence(currentUser.Id);

                CheckReportPermissions(user);

                var currentDate = DateTimeUtils.GetCurrentDateTime();

                DateTime startDate, endDate;
                base.ParseDatePeriod(settings.StartDate, settings.EndDate, currentDate, out startDate, out endDate);

                var showClientSummary = ValidationUtils.TryGetBool(settings.ShowClientSummary);
                var showClientOrganizationSummary = ValidationUtils.TryGetBool(settings.ShowClientOrganizationSummary);
                var showAccountOrganizationSummary = ValidationUtils.TryGetBool(settings.ShowAccountOrganizationSummary);
                var showClientContractSummary = ValidationUtils.TryGetBool(settings.ShowClientContractSummary);
                var showTeamSummary = ValidationUtils.TryGetBool(settings.ShowTeamSummary);
                var showUserSummary = ValidationUtils.TryGetBool(settings.ShowUserSummary);
                
                var showDetailsTable = ValidationUtils.TryGetBool(settings.ShowDetailsTable);
                var separateByDealPaymentForm = ValidationUtils.TryGetBool(settings.SeparateByDealPaymentForm);
                var showDistributedAndUndistributedSums = ValidationUtils.TryGetBool(settings.ShowDistributedAndUndistributedSums);
                var showDistributionDetails = ValidationUtils.TryGetBool(settings.ShowDistributionDetails);

                ValidationUtils.Assert(showClientSummary || showClientOrganizationSummary || showAccountOrganizationSummary || showClientContractSummary ||
                    showTeamSummary || showUserSummary || showDetailsTable, "Необходимо выбрать хотя бы одну таблицу.");

                ValidationUtils.Assert(showDetailsTable || !(separateByDealPaymentForm || showDistributedAndUndistributedSums || showDistributionDetails),
                    "Необходимо установить в «Да» параметр «Развернутая информация с документами оплат».");

                ValidationUtils.Assert(showDistributedAndUndistributedSums || !showDistributionDetails,
                    "Необходимо установить в «Да» параметр «Выводить столбцы «Разнесено в сумме» и «Неразнесенный остаток».");

                // Флаги выбора всех сущностей для отчета                
                var allClients = settings.AllClients == "1";
                var allAccountOrganizations = settings.AllAccountOrganizations == "1";
                var allTeams = settings.AllTeams == "1";
                var allUsers = settings.AllUsers == "1";

                // порядок группировки
                var groupFields = ParseGroupingString(settings.GroupByCollectionIDs);
                var maxGroupLevel = groupFields.Count();

                IEnumerable<int> clientIDs = null;
                IEnumerable<int> accountOrganizationIds = null;
                IEnumerable<short> teamIDs = null;
                IEnumerable<int> userIDs = null;

                if (!allClients)
                {
                    ValidationUtils.Assert(!String.IsNullOrEmpty(settings.ClientIDs), "Не выбрано ни одного клиента.");
                    clientIDs = settings.ClientIDs.Split('_').Select(x => ValidationUtils.TryGetInt(x));
                }

                if (!allAccountOrganizations) 
                {
                    ValidationUtils.Assert(!String.IsNullOrEmpty(settings.AccountOrganizationIDs), "Не выбрано ни одной собственной организации.");
                    accountOrganizationIds = settings.AccountOrganizationIDs.Split('_').Select(x => ValidationUtils.TryGetInt(x));
                }
                if (!allTeams)
                {
                    ValidationUtils.Assert(!String.IsNullOrEmpty(settings.TeamIDs), "Не выбрано ни одной команды.");
                    teamIDs = settings.TeamIDs.Split('_').Select(x => ValidationUtils.TryGetShort(x));
                }
                if (!allUsers)
                {
                    ValidationUtils.Assert(!String.IsNullOrEmpty(settings.UserIDs), "Не выбрано ни одного пользователя.");
                    userIDs = settings.UserIDs.Split('_').Select(x => ValidationUtils.TryGetInt(x));
                }

                #endregion

                var model = new Report0010ViewModel()
                {
                    Settings = settings,

                    CreatedBy = user.DisplayName,
                    CreationDate = currentDate.ToFullDateTimeString(),
                    ReportName = "Отчет «Принятые платежи»"
                };

                #region Подгрузка данных для отчета и заполнение основной модели данных отчета

                // подгрузка данных
                var loadedDealPayments = dealPaymentDocumentService.GetDealPaymentListInDateRangeByClientAndTeamAndAccountOrganizationList(startDate, endDate, clientIDs, accountOrganizationIds, teamIDs, userIDs, user).Values;

                // заполнение основной модели данных отчета
                var dealPaymentInfo = loadedDealPayments.Select(x => new DealPaymentInfo() {
                    Date = x.Date,
                    PaymentDocument = (x.Type == DealPaymentDocumentType.DealPaymentFromClient ? "Оплата" : "Возврат оплаты") + " №"  + x.PaymentDocumentNumber,
                    PaymentForm = x.DealPaymentForm,
                    Type = x.Type,
                    ClientId = x.Deal.Client.Id,
                    ClientName = x.Deal.Client.Name,
                    ClientOrganizationId = x.Deal.Contract.ContractorOrganization.Id,
                    ClientOrganizationName = x.Deal.Contract.ContractorOrganization.ShortName,
                    ClientContractId = x.Deal.Contract.Id,
                    ClientContractName = x.Deal.Contract.FullName,
                    AccountOrganizationId = x.Deal.Contract.AccountOrganization.Id,
                    AccountOrganizationName = x.Deal.Contract.AccountOrganization.ShortName,
                    TeamId = x.Team.Id,
                    TeamName = x.Team.Name,
                    UserId = x.User.Id,
                    UserName = x.User.DisplayName,
                    PaymentSum = (x.Type == DealPaymentDocumentType.DealPaymentFromClient ? x.Sum : -x.Sum),
                    DistributedSum = (x.Type == DealPaymentDocumentType.DealPaymentFromClient ? x.DistributedSum : -x.DistributedSum),
                    DistributedToSaleWaybillPaymentSum = x.DistributedToSaleWaybillSum,
                    DistributedToBalanceCorrectionPaymentSum = (x.Type == DealPaymentDocumentType.DealPaymentFromClient ? x.InitialBalancePaymentSum : -x.InitialBalancePaymentSum),
                    PaymentToClientSum = (x.Type == DealPaymentDocumentType.DealPaymentFromClient ? x.PaymentToClientSum : 0),
                    UndistributedSum = x.UndistributedSum
                });

                #endregion

                #region Заполнение сводных таблиц

                if (showAccountOrganizationSummary)
                {
                    model.AccountOrganizationSummary.Rows = GroupDataForSummaryTable(dealPaymentInfo, x => x.AccountOrganizationId, x => x.AccountOrganizationName);
                }
                if (showClientSummary)
                {
                    model.ClientSummary.Rows = GroupDataForSummaryTable(dealPaymentInfo, x => x.ClientId, x => x.ClientName);
                }
                if (showClientOrganizationSummary)
                {
                    model.ClientOrganizationSummary.Rows = GroupDataForSummaryTable(dealPaymentInfo, x => x.ClientOrganizationId, x => x.ClientOrganizationName);
                }
                if (showClientContractSummary)
                {
                    model.ClientContractSummary.Rows = GroupDataForSummaryTable(dealPaymentInfo, x => x.ClientContractId, x => x.ClientContractName);
                }
                if (showTeamSummary)
                {
                    model.TeamSummary.Rows = GroupDataForSummaryTable(dealPaymentInfo, x => x.TeamId, x => x.TeamName);
                }
                if (showUserSummary)
                {
                    model.UserSummary.Rows = GroupDataForSummaryTable(dealPaymentInfo, x => x.UserId, x => x.UserName);
                }
                
                #endregion

                #region Заполнение развернутых таблиц

                if (showDetailsTable)
                {
                    // если разделяем по форме оплаты
                    if (separateByDealPaymentForm)
                    {
                        // оплаты наличными
                        model.CashPaymentsDetailsTable.Settings = settings;
                        model.CashPaymentsDetailsTable.Title = "Развернутая информация по оплатам наличными";
                        model.CashPaymentsDetailsTable.ShowPaymentForm = false;
                        model.CashPaymentsDetailsTable.Rows = FillDetailsTable(dealPaymentInfo.Where(x => x.PaymentForm == DealPaymentForm.Cash), groupFields);
                        
                        // оплаты безналичными
                        model.CashlessPaymentsDetailsTable.Settings = settings;
                        model.CashlessPaymentsDetailsTable.Title = "Развернутая информация по оплатам безналичными";
                        model.CashlessPaymentsDetailsTable.ShowPaymentForm = false;
                        model.CashlessPaymentsDetailsTable.Rows = FillDetailsTable(dealPaymentInfo.Where(x => x.PaymentForm == DealPaymentForm.Cashless), groupFields);

                        // оплаты безналичными
                        model.ThirdPartyCashlessPaymentsDetailsTable.Settings = settings;
                        model.ThirdPartyCashlessPaymentsDetailsTable.Title = "Развернутая информация по оплатам безналичными через третье лицо";
                        model.ThirdPartyCashlessPaymentsDetailsTable.ShowPaymentForm = false;
                        model.ThirdPartyCashlessPaymentsDetailsTable.Rows = FillDetailsTable(dealPaymentInfo.Where(x => x.PaymentForm == DealPaymentForm.ThirdPartyCashless), groupFields);
                    }
                    else
                    {
                        model.AllPaymentsDetailsTable.Settings = settings;
                        model.AllPaymentsDetailsTable.Title = "Развернутая информация по оплатам";
                        model.AllPaymentsDetailsTable.ShowPaymentForm = true;
                        model.AllPaymentsDetailsTable.Rows = FillDetailsTable(dealPaymentInfo, groupFields);
                    }
                }
                
                #endregion

                return model;
            }
        }

        #endregion

        #region Вспомогательные методы

        private void CheckReportPermissions(User user)
        {
            user.CheckPermission(Permission.Report0010_View);
            user.CheckPermission(Permission.DealPayment_List_Details);
        }

        /// <summary>
        /// Разобрать строку с кодами группировок
        /// </summary>
        /// <param name="groupingString">Строка</param>
        private IEnumerable<GroupingType> ParseGroupingString(string groupingString)
        {
            var result = new List<GroupingType>();
            if (!String.IsNullOrEmpty(groupingString))
            {
                var splitResult = groupingString.Split('_').ToList();
                ValidationUtils.Assert(splitResult.Count == splitResult.Distinct().Count(),
                    "В таблице «Порядок группировки информации» заданы повторяющиеся элементы.");

                foreach (var item in splitResult)
                {
                    var value = ValidationUtils.TryGetEnum<GroupingType>(item);                                                            
                    result.Add(value);
                }
            }

            return result;
        }

        #region Заполнение сводных таблиц
        
        /// <summary>
        /// Группировка данных для сводных таблиц
        /// </summary>
        /// <param name="dealPaymentInfo">Информация о платежах</param>
        /// <param name="groupKey">Ключ для группировки</param>
        private IEnumerable<Report0010SummaryTableRowViewModel> GroupDataForSummaryTable(IEnumerable<DealPaymentInfo> dealPaymentInfo,
            Func<DealPaymentInfo, int> groupKey, Func<DealPaymentInfo, string> nameField)
        {
            return dealPaymentInfo.GroupBy(x => groupKey(x)).Select(x => new Report0010SummaryTableRowViewModel()
            {
                Name = nameField(x.First()),

                DealPaymentFromClientSum = x.Where(y => y.Type == DealPaymentDocumentType.DealPaymentFromClient).Sum(z => z.PaymentSum),
                DealPaymentFromClientCashSum = x.Where(y => y.Type == DealPaymentDocumentType.DealPaymentFromClient && y.PaymentForm == DealPaymentForm.Cash).Sum(z => z.PaymentSum),
                DealPaymentFromClientCashlessSum = x.Where(y => y.Type == DealPaymentDocumentType.DealPaymentFromClient && y.PaymentForm == DealPaymentForm.Cashless).Sum(z => z.PaymentSum),

                DealPaymentToClientSum = x.Where(y => y.Type == DealPaymentDocumentType.DealPaymentToClient).Sum(z => z.PaymentSum),
                DealPaymentToClientCashSum = x.Where(y => y.Type == DealPaymentDocumentType.DealPaymentToClient && y.PaymentForm == DealPaymentForm.Cash).Sum(z => z.PaymentSum),
                DealPaymentToClientCashlessSum = x.Where(y => y.Type == DealPaymentDocumentType.DealPaymentToClient && y.PaymentForm == DealPaymentForm.Cashless).Sum(z => z.PaymentSum),
            }).OrderBy(x => x.Name);
        } 
        #endregion

        #region Заполнение развернутых таблиц

        /// <summary>
        /// Заполнить модель для детализированной таблицы
        /// </summary>
        private IEnumerable<Report0010DetailsTableRowViewModel> FillDetailsTable(IEnumerable<DealPaymentInfo> dealPaymentInfo, IEnumerable<GroupingType> groupFields)
        {
            var detailTableRows = new List<Report0010DetailsTableRowViewModel>();

            GroupByDetailsTableRow(dealPaymentInfo, detailTableRows, groupFields, 1);

            return detailTableRows;
        }

        /// <summary>
        /// Сделать группировки. Рекурсивный спуск
        /// </summary>
        /// <param name="dealPaymentInfo">Данные о платежах</param>
        /// <param name="detailTableRows">Строки детализированный таблицы</param>
        /// <param name="groupFields">Список группировок, которые необходимо произвести</param>
        /// <param name="groupLevel">Уровень текущей группировки</param>
        private void GroupByDetailsTableRow(IEnumerable<DealPaymentInfo> dealPaymentInfo, IList<Report0010DetailsTableRowViewModel> detailTableRows,
            IEnumerable<GroupingType> groupFields, int groupLevel)
        {
            //Выход из рекурсии, прекращения группировок и заполнение обычными строками с товаром
            if (groupFields.Count() < groupLevel)
            {
                FillDetailsTableRows(dealPaymentInfo, detailTableRows);
                return;
            }

            //Выбор текущей группировки
            switch (groupFields.ElementAt(groupLevel - 1))
            {
                case GroupingType.Client:
                    DetailsTableRowsGrouping<Client>(dealPaymentInfo, detailTableRows, x => x.ClientId, x => x.ClientName, "Клиент", groupFields, groupLevel);
                    break;

                case GroupingType.ClientOrganization:
                    DetailsTableRowsGrouping<ClientOrganization>(dealPaymentInfo, detailTableRows, x => x.ClientOrganizationId, x => x.ClientOrganizationName, "Организация клиента", groupFields, groupLevel);
                    break;

                case GroupingType.ClientContract:
                    DetailsTableRowsGrouping<ClientContract>(dealPaymentInfo, detailTableRows, x => x.ClientContractId, x => x.ClientContractName, "Договор", groupFields, groupLevel);
                    break;

                case GroupingType.Team:
                    DetailsTableRowsGrouping<Team>(dealPaymentInfo, detailTableRows, x => x.TeamId, x => x.TeamName, "Команда", groupFields, groupLevel);
                    break;

                case GroupingType.User:
                    DetailsTableRowsGrouping<User>(dealPaymentInfo, detailTableRows, x => x.UserId, x => x.UserName, "Пользователь", groupFields, groupLevel);
                    break;

                case GroupingType.AccountOrganization:
                    DetailsTableRowsGrouping<AccountOrganization>(dealPaymentInfo, detailTableRows, x => x.AccountOrganizationId, x => x.AccountOrganizationName, "Собственная организация", groupFields, groupLevel);
                    break;

                default:
                    throw new Exception("Неизвестный тип группировки.");
            }
        }

        /// <summary>
        /// Заполнение строк развернутой таблицы
        /// </summary>
        /// <param name="dealPaymentInfo"></param>
        /// <param name="detailTableRows"></param>
        private void FillDetailsTableRows(IEnumerable<DealPaymentInfo> dealPaymentInfo, IList<Report0010DetailsTableRowViewModel> detailTableRows)
        {
            foreach (var payment in dealPaymentInfo.OrderBy(x => x.Date))
            {
                var row = new Report0010DetailsTableRowViewModel()
                {
                    Date = payment.Date.ToShortDateString(),
                    PaymentDocument = payment.PaymentDocument,
                    PaymentFormName = payment.PaymentForm.GetDisplayName(),
                    PaymentSum = payment.PaymentSum,
                    DistributedSum = payment.DistributedSum,
                    DistributedToSaleWaybillPaymentSum = (payment.Type == DealPaymentDocumentType.DealPaymentFromClient ? payment.DistributedToSaleWaybillPaymentSum : (decimal?)null),
                    DistributedToBalanceCorrectionPaymentSum = payment.DistributedToBalanceCorrectionPaymentSum,
                    PaymentToClientSum = (payment.Type == DealPaymentDocumentType.DealPaymentFromClient ? payment.PaymentToClientSum : (decimal?)null),
                    UndistributedSum = (payment.Type == DealPaymentDocumentType.DealPaymentFromClient ? payment.UndistributedSum : (decimal?)null)
                };

                detailTableRows.Add(row);
            }
        }

        private void DetailsTableRowsGrouping<T>(IEnumerable<DealPaymentInfo> dealPaymentInfo, IList<Report0010DetailsTableRowViewModel> detailTableRows,
              Func<DealPaymentInfo, int> groupingObject, Func<DealPaymentInfo, string> name, string title, IEnumerable<GroupingType> groupFields, int groupLevel)
        {
            foreach (var group in dealPaymentInfo.GroupBy(x => groupingObject(x)).OrderBy(x => name(x.First())))
            {
                detailTableRows.Add(new Report0010DetailsTableRowViewModel()
                {
                    IsGroup = true,
                    GroupTitle = String.Format("{0}: {1}", title, name(group.First())),
                    GroupLevel = groupLevel,

                    PaymentSum = group.Sum(x => x.PaymentSum),
                    DistributedSum = group.Sum(x => x.DistributedSum),
                    DistributedToSaleWaybillPaymentSum = group.Sum(x => x.DistributedToSaleWaybillPaymentSum),
                    DistributedToBalanceCorrectionPaymentSum = group.Sum(x => x.DistributedToBalanceCorrectionPaymentSum),
                    PaymentToClientSum = group.Sum(x => x.PaymentToClientSum),
                    UndistributedSum = group.Sum(x => x.UndistributedSum) 

                }); // Добавляем заголовок группы

                // группируем данные далее
                GroupByDetailsTableRow(group, detailTableRows, groupFields, groupLevel + 1);
            }
        }

        #endregion

        #endregion

        #region Выгрузка в Excel
        /// <summary>
        /// Экспорт отчета в Excel
        /// </summary>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="currentUser">Текущий пользователь</param>
        public byte[] Report0010ExportToExcel(Report0010SettingsViewModel settings, UserInfo currentUser)
        {
            var viewModel = Report0010(settings, currentUser);

            string reportHeader = viewModel.ReportName + "\r\nза период с  " + viewModel.Settings.StartDate + " по " + viewModel.Settings.EndDate;
            string sign = "Форма: Report0010 \r\nАвтор: " + viewModel.CreatedBy + "\r\nСоставлен: " + viewModel.CreationDate;
            int detailsTableColumnCount = 0;
            using (ExcelPackage pck = new ExcelPackage())
            {
                if (viewModel.Settings.ShowAccountOrganizationSummary == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по собственным организациям");
                    FillSummaryTable(sheet, 7, viewModel.AccountOrganizationSummary, sheet.PrintHeader(7, reportHeader, sign, "Сводная информация по собственным организациям:", 1));
                }
                if (viewModel.Settings.ShowClientSummary == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по клиентам");
                    FillSummaryTable(sheet, 7, viewModel.ClientSummary, sheet.PrintHeader(7, reportHeader, sign, "Сводная информация по клиентам:", 1));
                }
                if (viewModel.Settings.ShowClientOrganizationSummary == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по организациям клиентов");
                    FillSummaryTable(sheet, 7, viewModel.ClientOrganizationSummary, sheet.PrintHeader(7, reportHeader, sign, "Сводная информация по организациям клиентов:", 1));
                }
                if (viewModel.Settings.ShowClientContractSummary == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по договорам с клиентами");
                    FillSummaryTable(sheet, 7, viewModel.ClientContractSummary, sheet.PrintHeader(7, reportHeader, sign, "Сводная информация по договорам с клиентами:", 1));
                }
                if (viewModel.Settings.ShowTeamSummary == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по командам");
                    FillSummaryTable(sheet, 7, viewModel.TeamSummary, sheet.PrintHeader(7, reportHeader, sign, "Сводная информация по командам:", 1));
                }
                if (viewModel.Settings.ShowUserSummary == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по пользователям");
                    FillSummaryTable(sheet, 7, viewModel.UserSummary, sheet.PrintHeader(7, reportHeader, sign, "Сводная информация по пользователям:", 1));
                }
                if (viewModel.Settings.ShowDetailsTable == "1")
                {
                    if (viewModel.Settings.SeparateByDealPaymentForm == "1")
                    {
                        detailsTableColumnCount = GetColumnCount(viewModel.CashPaymentsDetailsTable);
                        ExcelWorksheet sheetCashPayments = pck.Workbook.Worksheets.Add("Развернутая по оплатам наличными");
                        FillDetailsTable(sheetCashPayments, detailsTableColumnCount, viewModel.CashPaymentsDetailsTable, sheetCashPayments.PrintHeader(detailsTableColumnCount, reportHeader, sign, viewModel.CashPaymentsDetailsTable.Title + ":", 1));
                        ExcelWorksheet sheetCashlessPayments = pck.Workbook.Worksheets.Add("Развернутая по оплатам безналичными");
                        FillDetailsTable(sheetCashlessPayments, detailsTableColumnCount, viewModel.CashlessPaymentsDetailsTable, sheetCashlessPayments.PrintHeader(detailsTableColumnCount, reportHeader, sign, viewModel.CashlessPaymentsDetailsTable.Title + ":", 1));
                        ExcelWorksheet sheetThirdPartyCashlessPayments = pck.Workbook.Worksheets.Add("Развернутая по оплатам через третье лицо");
                        FillDetailsTable(sheetThirdPartyCashlessPayments, detailsTableColumnCount, viewModel.ThirdPartyCashlessPaymentsDetailsTable, sheetThirdPartyCashlessPayments.PrintHeader(detailsTableColumnCount, reportHeader, sign, viewModel.ThirdPartyCashlessPaymentsDetailsTable.Title + ":", 1));
                    }
                    else
                    {
                        detailsTableColumnCount = GetColumnCount(viewModel.AllPaymentsDetailsTable);
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Развернутая по оплатам");
                        FillDetailsTable(sheet, detailsTableColumnCount, viewModel.AllPaymentsDetailsTable, sheet.PrintHeader(detailsTableColumnCount, reportHeader, sign, viewModel.AllPaymentsDetailsTable.Title + ":", 1));
                    }
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
        /// Формирует сводную таблицу 
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillSummaryTable(ExcelWorksheet workSheet, int columns, Report0010SummaryTableViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010SummaryTableRowViewModel>(x => x.Name));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010SummaryTableRowViewModel>(x => x.DealPaymentFromClientSumString));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010SummaryTableRowViewModel>(x => x.DealPaymentFromClientCashSumString));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010SummaryTableRowViewModel>(x => x.DealPaymentFromClientCashlessSumString));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010SummaryTableRowViewModel>(x => x.DealPaymentToClientSumString));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010SummaryTableRowViewModel>(x => x.DealPaymentToClientCashSumString));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010SummaryTableRowViewModel>(x => x.DealPaymentToClientCashlessSumString));
            currentCol = 1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки
            bool flag = false;
            foreach (var row in viewModel.Rows)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle());

                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Name).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DealPaymentFromClientSumString, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DealPaymentFromClientCashSumString, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DealPaymentFromClientCashlessSumString, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DealPaymentToClientSumString, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DealPaymentToClientCashSumString, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DealPaymentToClientCashlessSumString, ValueDisplayType.Money);
                currentCol = 1;
                currentRow++;
                flag = !flag;
            }
            if (viewModel.Rows.Count() == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().ApplyStyle(ExcelUtils.GetTableEvenRowStyle())
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center).SetFormattedValue("Нет данных");
                currentRow++;
            }
            #endregion

            #region Итого
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого:");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalDealPaymentFromClientSumString, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalDealPaymentFromClientCashSumString, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalDealPaymentFromClientCashlessSumString, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalDealPaymentToClientSumString, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalDealPaymentToClientCashSumString, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalDealPaymentToClientCashlessSumString, ValueDisplayType.Money);
            currentCol = 1;
            currentRow++;
            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        /// <summary>
        /// Формирует развернутую таблицу 
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillDetailsTable(ExcelWorksheet workSheet, int columns, Report0010DetailsTableViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Подсчет колонок для объединения
            int groupColSpan = 3;

            if (!viewModel.ShowPaymentForm)
            {
                groupColSpan--;
            }
            #endregion

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010DetailsTableRowViewModel>(x => x.Date));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010DetailsTableRowViewModel>(x => x.PaymentDocument));
            currentCol++;

            if (viewModel.ShowPaymentForm)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010DetailsTableRowViewModel>(x => x.PaymentFormName));
                currentCol++;
            }

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010DetailsTableRowViewModel>(x => x.PaymentSumString));
            currentCol++;

            if (viewModel.Settings.ShowDistributedAndUndistributedSums == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010DetailsTableRowViewModel>(x => x.DistributedSumString));
                currentCol++;
            }

            if (viewModel.Settings.ShowDistributionDetails == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010DetailsTableRowViewModel>(x => x.DistributedToSaleWaybillPaymentSumString));
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010DetailsTableRowViewModel>(x => x.DistributedToBalanceCorrectionPaymentSumString));
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010DetailsTableRowViewModel>(x => x.PaymentToClientSumString));
                currentCol++;
            }

            if (viewModel.Settings.ShowDistributedAndUndistributedSums == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0010DetailsTableRowViewModel>(x => x.UndistributedSumString));
                currentCol++;
            }

            currentCol = 1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки
            bool flag = false;
            foreach (var row in viewModel.Rows)
            {
                if (row.IsGroup)
                {
                    flag = false;
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle());

                    workSheet.Cells[currentRow, currentCol, currentRow, groupColSpan].MergeRange().SetFormattedValue(row.GroupTitle)
                        .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, indent: row.GroupLevel);
                    currentCol += groupColSpan;

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PaymentSumString, ValueDisplayType.Money);
                    currentCol++;

                    if (viewModel.Settings.ShowDistributedAndUndistributedSums == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DistributedSumString, ValueDisplayType.Money);
                        currentCol++;
                    }

                    if (viewModel.Settings.ShowDistributionDetails == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DistributedToSaleWaybillPaymentSumString, ValueDisplayType.Money);
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DistributedToBalanceCorrectionPaymentSumString, ValueDisplayType.Money);
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PaymentToClientSumString, ValueDisplayType.Money);
                        currentCol++;
                    }

                    if (viewModel.Settings.ShowDistributedAndUndistributedSums == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.UndistributedSumString, ValueDisplayType.Money);
                        currentCol++;
                    }
                }
                else
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Date).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PaymentDocument).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;

                    if (viewModel.ShowPaymentForm)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PaymentFormName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PaymentSumString, ValueDisplayType.Money);
                    currentCol++;

                    if (viewModel.Settings.ShowDistributedAndUndistributedSums == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DistributedSumString, ValueDisplayType.Money);
                        currentCol++;
                    }

                    if (viewModel.Settings.ShowDistributionDetails == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DistributedToSaleWaybillPaymentSumString, ValueDisplayType.Money);
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DistributedToBalanceCorrectionPaymentSumString, ValueDisplayType.Money);
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PaymentToClientSumString, ValueDisplayType.Money);
                        currentCol++;
                    }

                    if (viewModel.Settings.ShowDistributedAndUndistributedSums == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.UndistributedSumString, ValueDisplayType.Money);
                        currentCol++;
                    }
                    flag = !flag;
                }

                currentCol = 1;
                currentRow++;
            }
            if (!viewModel.Rows.Any())
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().ApplyStyle(ExcelUtils.GetTableEvenRowStyle())
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center).SetFormattedValue("Нет данных");
                currentRow++;
            }
            #endregion

            #region Итого
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

            workSheet.Cells[currentRow, currentCol, currentRow, groupColSpan].MergeRange().SetFormattedValue("Итого:")
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
            currentCol += groupColSpan;

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalPaymentSumString, ValueDisplayType.Money);
            currentCol++;

            if (viewModel.Settings.ShowDistributedAndUndistributedSums == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalDistributedSumString, ValueDisplayType.Money);
                currentCol++;
            }

            if (viewModel.Settings.ShowDistributionDetails == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalDistributedToSaleWaybillPaymentSumString, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalDistributedToBalanceCorrectionPaymentSumString, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalPaymentToClientSumString, ValueDisplayType.Money);
                currentCol++;
            }

            if (viewModel.Settings.ShowDistributedAndUndistributedSums == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalUndistributedSumString, ValueDisplayType.Money);
                currentCol++;
            }

            currentCol = 1;
            currentRow++;
            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        /// <summary>
        /// Подсчет колонок в детализированной таблице
        /// </summary>
        private int GetColumnCount(Report0010DetailsTableViewModel viewModel)
        {
            int count = 3;
            if (viewModel.ShowPaymentForm)
            {
                count++;
            }
            if (viewModel.Settings.ShowDistributedAndUndistributedSums == "1")
            {
                count++;
            }
            if (viewModel.Settings.ShowDistributionDetails == "1")
            {
                count += 3;
            }
            if (viewModel.Settings.ShowDistributedAndUndistributedSums == "1")
            {
                count++;
            }
            return count;
        } 
        #endregion

        #endregion
    }
}
