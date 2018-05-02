using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Report.Report0007;
using OfficeOpenXml;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class Report0007Presenter : IReport0007Presenter
    {
        #region Доп. классы/перечисления

        /// <summary>
        /// Тип группировки в 0007 отчете
        /// </summary>
        private enum GroupingType : byte
        {
            /// <summary>
            /// По местам хранения
            /// </summary>
            [EnumDisplayName("Место хранения")]
            ByStorage = 1,

            /// <summary>
            /// По командам
            /// </summary>
            [EnumDisplayName("Команда")]
            ByTeam = 2,

            /// <summary>
            /// По пользователям
            /// </summary>
            [EnumDisplayName("Пользователь")]
            ByUser = 3,

            /// <summary>
            /// По клиентам
            /// </summary>
            [EnumDisplayName("Клиент")]
            ByClient = 4,

            /// <summary>
            /// По организациям клиентов
            /// </summary>
            [EnumDisplayName("Организация клиента")]
            ByClientOrganization = 5,

            /// <summary>
            /// По собственной организации
            /// </summary>
            [EnumDisplayName("Собственная организация")]
            ByAccountOrganization = 6
        }

        /// <summary>
        /// Простая строка таблицы отчета
        /// </summary>
        private class ReportTableRow
        {
            public string Name { get; set; }
            public decimal ReserveSum { get; set; }
            public decimal DebtSum { get; set; }
            public decimal DelayDebtSum { get; set; }
        }

        /// <summary>
        /// Строка таблицы отчета для МХ
        /// </summary>
        private class StorageReportTableRow : ReportTableRow
        {
            public StorageType Type { get; set; }
        }

        /// <summary>
        /// Расширенная строка таблицы отчета с выводом неразнесенной оплаты и просрочки
        /// </summary>
        private class ReportExtendedTableRow : ReportTableRow
        {
            public decimal UndistributionPaymentSum { get; set; }
            public TimeSpan? DelayPaymentDays { get; set; }
        }

        /// <summary>
        /// Ключ для сортировки строк
        /// </summary>
        /// <typeparam name="T">Тип сущности</typeparam>
        /// <typeparam name="TId">Тип кода сущности</typeparam>
        private class RowKeyForGroupping<T, TId> where TId: struct
        {
            /// <summary>
            /// Код сущности
            /// </summary>
            public TId Id { get; set; }

            /// <summary>
            /// Название сущности
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Сущность. Нужно для последующей сортировки
            /// </summary>
            public T Entity { get; set; }

            #region Перегрузка операций сравнения 
            //Требуется для группировок

            public static bool operator ==(RowKeyForGroupping<T, TId> left, RowKeyForGroupping<T, TId> right)
            {
                return left.Id.Equals(right.Id);
            }

            public static bool operator !=(RowKeyForGroupping<T, TId> left, RowKeyForGroupping<T, TId> right)
            {
                return !(left == right);
            }

            public override bool Equals(object obj)
            {
                if (obj is RowKeyForGroupping<T, TId>)
                {
                    return this == (obj as RowKeyForGroupping<T, TId>);
                }

                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return Convert.ToInt32(Id);
            }

            #endregion
        }

        /// <summary>
        /// Строка развернутой таблицы отчета
        /// </summary>
        private class ReportExpenditureWaybillTableRow
        {
            #region Ключи для группировки

            public RowKeyForGroupping<Storage, short> Storage { get; set; }
            public RowKeyForGroupping<Team, int> Team { get; set; }
            public RowKeyForGroupping<User, int> User { get; set; }
            public RowKeyForGroupping<Client, int> Client { get; set; }
            public RowKeyForGroupping<ClientOrganization, int> ClientOrganization { get; set; }
            public RowKeyForGroupping<AccountOrganization, int> AccountOrganization { get; set; }

            #endregion

            /// <summary>
            /// Дата реализации
            /// </summary>
            public DateTime Date { get; set; }

            /// <summary>
            /// Номер реализации
            /// </summary>
            public string Number { get; set; }

            /// <summary>
            /// Сумма реализации
            /// </summary>
            public decimal SaleSum { get; set; }

            /// <summary>
            /// Сумма задолженности по реализации
            /// </summary>
            public decimal DebtSum { get; set; }

            /// <summary>
            /// Дата проводки реализации
            /// </summary>
            public DateTime AcceptanceDate { get; set; }

            /// <summary>
            /// Дата отгрузки реализации
            /// </summary>
            public DateTime ShippingDate { get; set; }

            /// <summary>
            /// Дата оплаты
            /// </summary>
            public DateTime? PaymentDate { get; set; }

            /// <summary>
            /// Отсрочка оплаты
            /// </summary>
            public int? PostPaymentDays { get; set; }

            /// <summary>
            /// Просрочка
            /// </summary>
            public TimeSpan? DelayPaymentDays { get; set; }

            /// <summary>
            /// Конструктор
            /// </summary>
            public ReportExpenditureWaybillTableRow()
            {
                Storage = new RowKeyForGroupping<Storage, short>();
                Team = new RowKeyForGroupping<Team, int>();
                User = new RowKeyForGroupping<User, int>();
                Client = new RowKeyForGroupping<Client, int>();
                ClientOrganization = new RowKeyForGroupping<ClientOrganization, int>();
                AccountOrganization = new RowKeyForGroupping<AccountOrganization, int>();
            }
        }

        /// <summary>
        ///  Класс, хранящий данные отчета
        /// </summary>
        private class Report0007Model
        {
            /// <summary>
            /// Признак вывода только просроченных задолжностей для развернутой таблицы
            /// </summary>
            public bool ShowOnlyDelayDebt { get; set; }

            #region Признаки вывода таблиц

            /// <summary>
            /// Признак вывода таблицы по местам хранения
            /// </summary>
            public bool ShowStorageTable { get; set; }

            /// <summary>
            /// Признак вывода таблицы по собственным организациям
            /// </summary>
            public bool ShowAccountOrganizationTable { get; set; }

            /// <summary>
            /// Признак вывода таблицы по клиентам
            /// </summary>
            public bool ShowClientTable { get; set; }

            /// <summary>
            /// Признак вывода таблицы по организациям клиентов
            /// </summary>
            public bool ShowClientOrganizationTable { get; set; }

            /// <summary>
            /// Признак вывода таблицы по сделкам
            /// </summary>
            public bool ShowDealTable { get; set; }

            /// <summary>
            /// Признак вывода таблицы по командам
            /// </summary>
            public bool ShowTeamTable { get; set; }

            /// <summary>
            /// Признак вывода таблицы по пользователям
            /// </summary>
            public bool ShowUserTable { get; set; }

            /// <summary>
            /// Признак вывода таблицы по реализациям
            /// </summary>
            public bool ShowExpenditureWaybillTable { get; set; }

            #endregion

            #region Данные для вывода таблиц

            /// <summary>
            /// Таблица мест хранения
            /// </summary>
            public DynamicDictionary<int, StorageReportTableRow> Storages { get; private set; }

            /// <summary>
            /// Таблица собственных организаций
            /// </summary>
            public DynamicDictionary<int, ReportTableRow> AccountOrganizations { get; private set; }

            /// <summary>
            /// Таблица клиентов
            /// </summary>
            public DynamicDictionary<int, ReportExtendedTableRow> Clients { get; private set; }

            /// <summary>
            /// Таблица организаций клиентов
            /// </summary>
            public DynamicDictionary<int, ReportExtendedTableRow> ClientOrganizations { get; private set; }

            /// <summary>
            /// Таблица сделок
            /// </summary>
            public DynamicDictionary<int, ReportExtendedTableRow> Deals { get; private set; }

            /// <summary>
            /// Таблица команд
            /// </summary>
            public DynamicDictionary<int, ReportTableRow> Teams { get; private set; }

            /// <summary>
            /// Таблица пользователей
            /// </summary>
            public DynamicDictionary<int, ReportTableRow> Users { get; private set; }

            /// <summary>
            /// Развернутая таблица по реализациям
            /// </summary>
            public DynamicDictionary<Guid, ReportExpenditureWaybillTableRow> ExpenditureWaybills { get; private set; }

            #region Словари для исключения повторной обработки сделок

            /// <summary>
            /// Коды обработанных сделок для таблицы клиентов
            /// </summary>
            public DynamicDictionary<int, List<int>> ProcessedDealsForClient { get; private set; }

            /// <summary>
            /// Коды обработанных сделок для таблицы организаций клиентов
            /// </summary>
            public DynamicDictionary<int, List<int>> ProcessedDealsForClientOrgnization { get; private set; }

            /// <summary>
            /// Коды обработанных сделок для таблицы сделок
            /// </summary>
            public IList<int> ProcessedDeals { get; private set; }

            #endregion

            #endregion

            /// <summary>
            /// Порядок группировки по полям
            /// </summary>
            public List<int> GroupFields { get; set; }

            /// <summary>
            /// Конструктор
            /// </summary>
            public Report0007Model()
            {
                Storages = new DynamicDictionary<int, StorageReportTableRow>();
                AccountOrganizations = new DynamicDictionary<int, ReportTableRow>();
                Clients = new DynamicDictionary<int, ReportExtendedTableRow>();
                ClientOrganizations = new DynamicDictionary<int, ReportExtendedTableRow>();
                Deals = new DynamicDictionary<int, ReportExtendedTableRow>();
                Teams = new DynamicDictionary<int, ReportTableRow>();
                Users = new DynamicDictionary<int, ReportTableRow>();
                ExpenditureWaybills = new DynamicDictionary<Guid, ReportExpenditureWaybillTableRow>();

                GroupFields = new List<int>();

                ProcessedDealsForClient = new DynamicDictionary<int, List<int>>();
                ProcessedDealsForClientOrgnization = new DynamicDictionary<int, List<int>>();
                ProcessedDeals = new List<int>();
            }
        }

        #endregion

        #region Поля

        private const int OtherEntityId = -1;   // Идентификатор для невидимых сущностей (Они попадут в строку «Прочие»)

        private const string OtherDealName = "<Прочие сделки>"; // Имя «невидимых» сделок
        private const string OtherTeamName = "<Прочие команды>";    // Имя «невидимых» команд


        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IUserService userService;
        private readonly IStorageService storageService;
        private readonly IClientService clientService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly IDealService dealService;
        private readonly ITeamService teamService;
        private readonly IExpenditureWaybillService expenditureWaybillService;
        private readonly IReceiptedReturnFromClientIndicatorService receiptedReturnFromClientIndicatorService;

        #endregion

        #region Конструктор

        public Report0007Presenter(IUnitOfWorkFactory unitOfWorkFactory, IUserService userService, IStorageService storageService, IClientService clientService,
            IAccountOrganizationService accountOrganizationService, IDealService dealService, ITeamService teamService, 
            IExpenditureWaybillService expenditureWaybillService, IReceiptedReturnFromClientIndicatorService receiptedReturnFromClientIndicatorService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.userService = userService;
            this.storageService = storageService;
            this.clientService = clientService;
            this.accountOrganizationService = accountOrganizationService;
            this.dealService = dealService;
            this.teamService = teamService;
            this.expenditureWaybillService = expenditureWaybillService;
            this.receiptedReturnFromClientIndicatorService = receiptedReturnFromClientIndicatorService;
        }

        #endregion

        #region Методы

        #region Общие методы
        /// <summary>
        /// Проверка минимальных прав для отчета
        /// </summary>
        /// <param name="user">Текущий пользователь</param>
        private void CheckMinPermissionForReport(User user)
        {
            // Проверяем минимальный набор прав для построения отчета и генерируем понятные для пользователя сообщения о нехватке прав
            user.CheckPermission(Permission.Report0007_View);
            ValidationUtils.Assert(user.HasPermission(Permission.Report0007_Storage_List), "Невозможно построить отчет, т.к. отсутствует право на места хранения в отчете.");
            ValidationUtils.Assert(user.HasPermission(Permission.Storage_List_Details), "Невозможно построить отчет, т.к. отсутствует право на просмотр списка мест хранения.");
            ValidationUtils.Assert(user.HasPermission(Permission.Client_List_Details), "Невозможно построить отчет, т.к. отсутствует право на просмотр списка клиентов.");
            ValidationUtils.Assert(user.HasPermission(Permission.User_List_Details), "Невозможно построить отчет, т.к. отсутствует право на просмотр списка пользователей.");
            ValidationUtils.Assert(user.HasPermission(Permission.ExpenditureWaybill_List_Details), "Невозможно построить отчет, т.к. отсутствует право на просмотр списка накладных реализации.");
        }

        #endregion

        #region Настройка отчета

        /// <summary>
        /// Настройка отчета
        /// </summary>
        /// <param name="backUrl">Адрес возврата</param>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <returns></returns>
        public Report0007SettingsViewModel Report0007Settings(string backUrl, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDate = DateTimeUtils.GetCurrentDateTime();

                CheckMinPermissionForReport(user);   //Проверка прав на построение отчета

                // Получаем флаги разрешений на просмотр для необязательных сущностей. 
                // Если право отсутствует, то соответствующая таблица будет исключена из отчета.

                var allowToCheckClientOrganizationTable = user.HasPermission(Permission.ClientOrganization_List_Details);
                var allowToCheckDealTable = user.HasPermission(Permission.Deal_List_Details);
                var allowToCheckTeamTable = user.HasPermission(Permission.Team_List_Details);

                // Группировки

                var groupList = new List<SelectListItem>();

                // собственной организации
                groupList.Add(new SelectListItem() { Text = GroupingType.ByAccountOrganization.GetDisplayName(), Value = GroupingType.ByAccountOrganization.ValueToString() });
                // МХ
                groupList.Add(new SelectListItem() { Text = GroupingType.ByStorage.GetDisplayName(), Value = GroupingType.ByStorage.ValueToString() });
                // Команды
                if (allowToCheckTeamTable)
                { groupList.Add(new SelectListItem() { Text = GroupingType.ByTeam.GetDisplayName(), Value = GroupingType.ByTeam.ValueToString() }); }
                // Пользователи
                groupList.Add(new SelectListItem() { Text = GroupingType.ByUser.GetDisplayName(), Value = GroupingType.ByUser.ValueToString() });
                // Клиенты
                groupList.Add(new SelectListItem() { Text = GroupingType.ByClient.GetDisplayName(), Value = GroupingType.ByClient.ValueToString() });
                // Организации клиентов
                if (allowToCheckClientOrganizationTable)
                { groupList.Add(new SelectListItem() { Text = GroupingType.ByClientOrganization.GetDisplayName(), Value = GroupingType.ByClientOrganization.ValueToString() }); }

                return new Report0007SettingsViewModel()
                {
                    BackURL = backUrl,
                    Date = currentDate.ToShortDateString(),
                    AllowToChangeDataTime = user.HasPermission(Permission.Report0007_Date_Change),

                    GroupByCollection = groupList,
                    ClientList = clientService.GetList(user).OrderBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name),
                    StorageList = storageService.GetList(user, Permission.Report0007_Storage_List).OrderBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name),
                    AccountOrganizationList = accountOrganizationService.GetList().OrderBy(x => x.ShortName).ToDictionary(x => x.Id.ToString(), x => x.ShortName),
                    UserList = userService.GetList(user).OrderBy(x => x.DisplayName).ToDictionary(x => x.Id.ToString(), x => x.DisplayName),

                    ShowOnlyDelayDebt = "0",
                    ShowAccountOrganizationTable = "1",

                    AllowCheckClientOrganizationTable = allowToCheckClientOrganizationTable,
                    ShowClientOrganizationTable = allowToCheckClientOrganizationTable ? "1" : "0",    // Если права нет, то исключаем таблицу из числа выводимых

                    ShowClientTable = "1",

                    AllowCheckDealTable = allowToCheckDealTable,
                    ShowDealTable = allowToCheckDealTable ? "1" : "0",     // Если права нет, то исключаем таблицу из числа выводимых

                    ShowExpenditureWaybillTable = "1",
                    ShowStorageTable = "1",

                    AllowCheckTeamTable = allowToCheckTeamTable,
                    ShowTeamTable = allowToCheckTeamTable ? "1" : "0",     // Если права нет, то исключаем таблицу из числа выводимых

                    ShowUserTable = "1"
                };
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
        public Report0007ViewModel Report0007(Report0007SettingsViewModel settings, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                ValidationUtils.NotNull(settings, "Входной параметр задан неверно.");
                var user = userService.CheckUserExistence(currentUser.Id);

                CheckMinPermissionForReport(user);   //Проверка прав на построение отчета

                DateTime date;
                if (user.HasPermission(Permission.Report0007_Date_Change))  // Проверяем права на изменение даты построения отчета
                {
                    // Право изменять дату построения отчета есть, получаем и проверяем дату.
                    date = ValidationUtils.TryGetDate(settings.Date).SetHoursMinutesAndSeconds(23, 59, 59); // Получаем дату построения отчета
                    ValidationUtils.Assert(date.Date <= DateTimeUtils.GetCurrentDateTime().Date, "Невозможно построить отчет за будущий период.");
                }
                else
                {
                    // Права изменять дату построения отчета нет, выставляем текущую дату.
                    date = DateTimeUtils.GetCurrentDateTime().SetHoursMinutesAndSeconds(23, 59, 59);
                }

                // Коды выбранных для отчета сущностей
                IEnumerable<short> storageIds = null;
                IEnumerable<int> clientIds = null;
                IEnumerable<int> accountOrganizationIds = null;
                IEnumerable<int> userIds = null;

                // Флаги выбора всех сущностей для отчета
                var allStorages = settings.AllStorages == "1";
                var allClients = settings.AllClients == "1";
                var allAccountOrganizations = settings.AllAccountOrganizations == "1";
                var allUsers = settings.AllUsers == "1";

                // Если выбраны не все сущности, то получаем коллекцию кодов выбранных сущностей
                if (!allStorages) { storageIds = GetShortIdList(settings, x => x.StorageIDs); }
                if (!allClients) { clientIds = GetIntIdList(settings, x => x.ClientIDs); }
                if (!allAccountOrganizations) { accountOrganizationIds = GetIntIdList(settings, x => x.AccountOrganizationIDs); }
                if (!allUsers) { userIds = GetIntIdList(settings, x => x.UserIDs); }

                var dataModel = new Report0007Model();  // Модель данных отчета
                var viewModel = new Report0007ViewModel()   // Модель представления отчета
                {
                    CreationData = DateTimeUtils.GetCurrentDateTime().ToFullDateTimeString(),
                    Date = date.ToFullDateTimeString(),
                    CreatedBy = user.DisplayName
                };

                #region Читаем порядок группировок

                if (!String.IsNullOrEmpty(settings.GroupByCollectionIDs))
                {
                    foreach (var val in settings.GroupByCollectionIDs.Split('_'))
                    {
                        var value = ValidationUtils.TryGetInt(val);   // Получаем код поля группировки
                        dataModel.GroupFields.Add(value);   // Добавляем его в коллекцию

                        var enumValue = ValidationUtils.TryGetEnum<GroupingType>(val, "Неверный код группировки.");   // Приводим код к значению перечисления
                        switch (enumValue)  // Проверяем права видимости полей, по которым будем группировать
                        {
                            case GroupingType.ByClient: // т.к. входит в минимальный набор прав для построения отчета, то необходимости проверять наличие права нет.
                                break;
                            case GroupingType.ByClientOrganization:
                                ValidationUtils.Assert(user.HasPermission(Permission.ClientOrganization_List_Details),
                                    "Невозможно виполнить группировку по организациям клиентов, т.к. отсутствуют права на просмотр списка и деталей организаций клиента.");
                                break;
                            case GroupingType.ByStorage:  // т.к. входит в минимальный набор прав для построения отчета, то необходимости проверять наличие права нет.
                                break;
                            case GroupingType.ByTeam:
                                ValidationUtils.Assert(user.HasPermission(Permission.Team_List_Details),
                                    "Невозможно виполнить группировку по командам, т.к. отсутствуют права на просмотр списка и деталей команд.");
                                break;
                            case GroupingType.ByUser:   // т.к. входит в минимальный набор прав для построения отчета, то необходимости проверять наличие права нет.
                                break;
                            case GroupingType.ByAccountOrganization:    //На просмотр собственных организаций права есть всегда
                                break;
                        }
                    }
                }

                #endregion

                // Получаем флаги вывода таблиц

                dataModel.ShowStorageTable = settings.ShowStorageTable == "1";
                dataModel.ShowAccountOrganizationTable = settings.ShowAccountOrganizationTable == "1";
                dataModel.ShowClientTable = settings.ShowClientTable == "1";
                dataModel.ShowClientOrganizationTable = settings.ShowClientOrganizationTable == "1";
                dataModel.ShowDealTable = settings.ShowDealTable == "1";
                dataModel.ShowTeamTable = settings.ShowTeamTable == "1";
                dataModel.ShowExpenditureWaybillTable = settings.ShowExpenditureWaybillTable == "1";
                dataModel.ShowUserTable = settings.ShowUserTable == "1";
                dataModel.ShowOnlyDelayDebt = settings.ShowOnlyDelayDebt == "1";

                #region Проверяем права на вывод таблиц

                if (dataModel.ShowClientOrganizationTable)
                {
                    ValidationUtils.Assert(user.HasPermission(Permission.ClientOrganization_List_Details),
                        "Невозможно вывести сводную таблицу по организациям клиентов, т.к. отствует разрешение права на просмотр списка и деталей организаций клиентов.");
                }
                if (dataModel.ShowDealTable)
                {
                    ValidationUtils.Assert(user.HasPermission(Permission.Deal_List_Details),
                        "Невозможно вывести сводную таблицу по сделкам, т.к. отствует разрешение права на просмотр списка и деталей сделок.");
                }
                if (dataModel.ShowTeamTable)
                {
                    ValidationUtils.Assert(user.HasPermission(Permission.Team_List_Details),
                        "Невозможно вывести сводную таблицу по командам, т.к. отствует разрешение права на просмотр списка и деталей команд.");
                }

                #endregion

                // Формирование отчета
                FillInReportByExpenditureWaybill(dataModel, storageIds, allStorages, clientIds, allClients, accountOrganizationIds, allAccountOrganizations,
                    userIds, allUsers, date, user);

                #region Заполнение представления

                // Устанавливаем признак вывода таблиц

                viewModel.ShowStorageTable = dataModel.ShowStorageTable;
                viewModel.ShowAccountOrganizationTable = dataModel.ShowAccountOrganizationTable;
                viewModel.ShowClientTable = dataModel.ShowClientTable;
                viewModel.ShowClientOrganizationTable = dataModel.ShowClientOrganizationTable;
                viewModel.ShowDealTable = dataModel.ShowDealTable;
                viewModel.ShowTeamTable = dataModel.ShowTeamTable;
                viewModel.ShowExpenditureWaybillTable = dataModel.ShowExpenditureWaybillTable;
                viewModel.ShowUserTable = dataModel.ShowUserTable;

                // Заполняем модель для каждой выводимой таблицы

                if (dataModel.ShowStorageTable)
                {
                    FullInStorageTableViewModel(dataModel, viewModel);
                }
                if (dataModel.ShowAccountOrganizationTable)
                {
                    FullInAccountOrganizationTableViewModel(dataModel, viewModel);
                }
                if (dataModel.ShowClientTable)
                {
                    FullInClientTableViewModel(dataModel, viewModel);
                }
                if (dataModel.ShowClientOrganizationTable)
                {
                    FullInClientOrganizationTableViewModel(dataModel, viewModel);
                }
                if (dataModel.ShowDealTable)
                {
                    FullInDealTableViewModel(dataModel, viewModel);
                }
                if (dataModel.ShowTeamTable)
                {
                    FullInTeamTableViewModel(dataModel, viewModel);
                }
                if (dataModel.ShowUserTable)
                {
                    FullInUserTableViewModel(dataModel, viewModel);
                }
                if (dataModel.ShowExpenditureWaybillTable)
                {
                    FullInExpenditureWaybillTableViewModel(dataModel, viewModel);
                }

                #endregion

                return viewModel;
            }
        }

        /// <summary>
        /// Получение ограничений для получения накладных реализации
        /// </summary>
        /// <param name="storageIds">Коды выбранных МХ</param>
        /// <param name="allStorages">Признак выбора всех МХ</param>
        /// <param name="clientIds">Коды выбранных клиентов</param>
        /// <param name="allClients">Признак выбора всех клиентов</param>
        /// <param name="accountOrganizationIds">Коды выбранных собственных организаций</param>
        /// <param name="allAccountOrganizations">Признак выбора всех собственных организаций</param>
        /// <param name="userIds">Коды выбранных пользователей</param>
        /// <param name="allUsers">Признак выбора всех пользователей</param>
        /// <param name="currentDate">Дата построения отчета</param>
        /// <returns></returns>
        private ParameterString GetParameterStringForExpenditureWaybills(IEnumerable<short> storageIds, bool allStorages, IEnumerable<int> clientIds, bool allClients,
            IEnumerable<int> accountOrganizationIds, bool allAccountOrganizations, IEnumerable<int> userIds, bool allUsers, DateTime currentDate )
        {
            var ps = new ParameterString("");

            ps.Add("IsFullyPaid", ParameterStringItem.OperationType.Eq, "false");   // Выбираем только не полностью оплаченные
            ps.Add("AcceptanceDate", ParameterStringItem.OperationType.Le, currentDate.ToFullDateTimeString());  // Берем только проведенные до указанной даты включительно

            if (!allClients)
            {
                ps.Add("Deal.Client.Id", ParameterStringItem.OperationType.OneOf, clientIds.Select(x => x.ToString()));
            }
            if (!allStorages)
            {
                ps.Add("SenderStorage.Id", ParameterStringItem.OperationType.OneOf, storageIds.Select(x => x.ToString()));
            }
            if (!allAccountOrganizations)
            {
                ps.Add("Deal.Contract.AccountOrganization.Id", ParameterStringItem.OperationType.OneOf, accountOrganizationIds.Select(x => x.ToString()));
            }
            if (!allUsers)
            {
                ps.Add("Curator.Id", ParameterStringItem.OperationType.OneOf, userIds.Select(x => x.ToString()));
            }

            return ps;
        }

        /// <summary>
        /// Построение отчета по реализациям
        /// </summary>
        /// <param name="model">Модель данных отчета</param>
        /// <param name="storageIds">Коды выбранных МХ</param>
        /// <param name="allStorages">Признак выбора всех МХ</param>
        /// <param name="clientIds">Коды выбранных клиентов</param>
        /// <param name="allClients">Признак выбора всех клиентов</param>
        /// <param name="accountOrganizationIds">Коды выбранных собственных организаций</param>
        /// <param name="allAccountOrganizations">Признак выбора всех собственных организаций</param>
        /// <param name="userIds">Коды выбранных пользователей</param>
        /// <param name="allUsers">Признак выбора всех пользователей</param>
        /// <param name="currentDate">Дата построения отчета</param>
        /// <param name="user">Пользователь</param>
        private void FillInReportByExpenditureWaybill(Report0007Model model, IEnumerable<short> storageIds, bool allStorages, IEnumerable<int> clientIds, bool allClients,
            IEnumerable<int> accountOrganizationIds, bool allAccountOrganizations, IEnumerable<int> userIds, bool allUsers, DateTime currentDate, User user)
        {
            var state = new GridState() { CurrentPage = 1, PageSize = 500 };
            var ps = GetParameterStringForExpenditureWaybills(storageIds, allStorages, clientIds, allClients, accountOrganizationIds, allAccountOrganizations, userIds, allUsers, currentDate);

            var exList = expenditureWaybillService.GetFilteredList(state, user, ps, Permission.Report0007_Storage_List, Permission.User_List_Details);    // Получаем первую «страницу» реализаций

            var pageCount = state.TotalRow / state.PageSize + (state.TotalRow % state.PageSize > 0 ? 1 : 0);    //Подсчитываем общее количество таких «страниц»
            for (int page = 1; page <= pageCount; page++)   //Цикл прохода по всем старницам
            {
                state.CurrentPage = page;   //Выставляем номер страницы
                if (page != 1)  // Если страница не первая, то 
                {
                    exList = expenditureWaybillService.GetFilteredList(state, user, ps,Permission.Report0007_Storage_List, Permission.User_List_Details);    // Подгружаем ее
                }// иначе первая старница уже загружена до начала цикла

                var returns = receiptedReturnFromClientIndicatorService.GetReturnsOnSale(currentDate, exList);  // Получаем возвраты по реализациям

                // Заполняем таблицы отчета по странице реализаций

                if (model.ShowStorageTable)
                { GetStorageTable(model, exList, returns, currentDate, user); }
                if (model.ShowAccountOrganizationTable)
                { GetAccountOrganizationTable(model, exList, returns, currentDate, user); }
                if (model.ShowClientTable)
                { GetClientTable(model, exList, returns, currentDate, user); }
                if (model.ShowClientOrganizationTable)
                { GetClientOrganizationTable(model, exList, returns, currentDate, user); }
                if (model.ShowDealTable)
                { GetDealTable(model, exList, returns, currentDate, user); }
                if (model.ShowTeamTable)
                { GetTeamTable(model, exList, returns, currentDate, user); }
                if (model.ShowUserTable)
                { GetUserTable(model, exList, returns, currentDate, user); }
                if (model.ShowExpenditureWaybillTable)
                { GetExpenditureWaybillTable(model, exList, returns, currentDate, user); }
            }
        }

        #region Вспомогательные методы

        /// <summary>
        /// Получение коллекции выбранных кодов
        /// </summary>
        private IEnumerable<int> GetIntIdList(Report0007SettingsViewModel model, Func<Report0007SettingsViewModel, string> getIdList)
        {
            IList<int> list = new List<int>();
            var str = getIdList(model);

            if (!String.IsNullOrEmpty(str))
            {
                var ids = str.Split('_');

                foreach (var id in ids)
                {
                    list.Add(ValidationUtils.TryGetInt(id));
                }
            }

            return list;
        }

        /// <summary>
        /// Получение коллекции выбранных кодов
        /// </summary>
        private IEnumerable<short> GetShortIdList(Report0007SettingsViewModel model, Func<Report0007SettingsViewModel, string> getIdList)
        {
            IList<short> list = new List<short>();
            var str = getIdList(model);

            if (!String.IsNullOrEmpty(str))
            {
                var ids = str.Split('_');

                foreach (var id in ids)
                {
                    list.Add(ValidationUtils.TryGetShort(id));
                }
            }

            return list;
        }

        /// <summary>
        /// Заполнение строки таблицы отчета данными по коллекции реализаций
        /// </summary>
        /// <param name="exList">Коллекция реализаций</param>
        /// <param name="row">Строка таблицы</param>
        /// <param name="returns">Возвраты по реализациям</param>
        /// <param name="currentDate">Дата построения отчета</param>
        /// <param name="user">Пользователь</param>
        private void FillInTableRowByExpenditureWaybills(IEnumerable<ExpenditureWaybill> exList, ReportTableRow row,
            DynamicDictionary<Guid, List<ReceiptedReturnFromClientIndicator>> returns, DateTime currentDate, User user)
        {
            foreach (var exWaybill in exList)
            {
                FillInTableRowByExpenditureWaybill(exWaybill, row, returns[exWaybill.Id], currentDate);
            }
        }

        /// <summary>
        /// Заполнение строки таблицы отчета данными по коллекции реализаций
        /// </summary>
        /// <param name="exList">Коллекция реализаций</param>
        /// <param name="row">Строка таблицы</param>
        /// <param name="returns">Возвраты по реализациям</param>
        /// <param name="currentDate">Дата построения отчета</param>
        /// <param name="user">Пользователь</param>
        private void FillInTableRowByExpenditureWaybills(IEnumerable<ExpenditureWaybill> exList, ReportExtendedTableRow row,
            DynamicDictionary<Guid, List<ReceiptedReturnFromClientIndicator>> returns, DateTime currentDate, User user)
        {
            foreach (var exWaybill in exList)
            {
                var deadline = FillInTableRowByExpenditureWaybill(exWaybill, row, returns[exWaybill.Id], currentDate);
                if (deadline != null)
                {
                    var delta = currentDate - deadline;

                    if (row.DelayPaymentDays == null || row.DelayPaymentDays < delta)
                    {
                        row.DelayPaymentDays = delta;
                    }
                }
            }
        }

        /// <summary>
        /// Заполнение строки таблицы отчета по накладной реализации
        /// </summary>
        /// <param name="exWaybill">Накладная реализации</param>
        /// <param name="row">Строка таблицы отчета</param>
        /// <param name="returns">Возвраты по реализации</param>
        /// <param name="currentDate">Дата отчета</param>
        /// <returns></returns>
        private DateTime? FillInTableRowByExpenditureWaybill(ExpenditureWaybill exWaybill, ReportTableRow row,
           List<ReceiptedReturnFromClientIndicator> returns, DateTime currentDate)
        {
            DateTime? deadline = null;
            var pSum = exWaybill.Distributions.Where(x => x.DistributionDate <= currentDate).Sum(x => x.Sum);
            var returnSum = returns.Sum(x => x.SalePriceSum);
            var debt = exWaybill.SalePriceSum - pSum - returnSum;

            if (exWaybill.ShippingDate == null || exWaybill.ShippingDate > currentDate)
            {
                row.ReserveSum += debt;
            }
            else
            {
                row.DebtSum += debt;
            }
            if (exWaybill.IsShipped && exWaybill.Quota.PostPaymentDays.Value > 0)
            {
                deadline = exWaybill.ShippingDate.Value.Date.AddDays(exWaybill.Quota.PostPaymentDays.Value);
                if (deadline < currentDate.Date)
                {
                    row.DelayDebtSum += debt;
                }
            }

            return deadline;
        }

        /// <summary>
        /// Заполнение нераспределенной суммы оплат
        /// </summary>
        /// <param name="row">Строка таблицы для заполнения</param>
        /// <param name="deal">Сделка, по которой посчитать сумму</param>
        /// <param name="currentDate">Дата, на которую посчитать сумму</param>
        private void FillInPaymentInfo(ReportExtendedTableRow row, Deal deal, DateTime currentDate)
        {
            FillInPaymentInfo(row, new List<Deal> { deal }, currentDate);
        }

        /// <summary>
        /// Заполнение нераспределенной суммы оплат
        /// </summary>
        /// <param name="row">Строка таблицы для заполнения</param>
        /// <param name="deals">Сделки, по которым посчитать сумму</param>
        /// <param name="currentDate">Дата, на которую посчитать сумму</param>
        private void FillInPaymentInfo(ReportExtendedTableRow row, IEnumerable<Deal> deals, DateTime currentDate)
        {
            row.UndistributionPaymentSum += GetUndistributionPaymentSum(deals.SelectMany(x => x.DealPaymentDocuments), currentDate);
        }

        /// <summary>
        /// Получение неразнесенной суммы оплат по платежным документам на дату
        /// </summary>
        /// <param name="documentList">Платежные документы</param>
        /// <param name="currentDate">Дата, на которую считается наразнесенная сумма</param>
        /// <returns>Неразнесенная сумма оплат</returns>
        private decimal GetUndistributionPaymentSum(IEnumerable<DealPaymentDocument> documentList, DateTime currentDate)
        {
            decimal paymentSum, creditCorrectionSum;    //Суммы денежных средств
            decimal distributedPaymentSum, distributedCreditCorrectionSum;  //Суммы разнесенных денежных средств

            paymentSum = creditCorrectionSum = distributedPaymentSum = distributedCreditCorrectionSum = 0;

            foreach (var p in documentList)
            {
                if (p.Date <= currentDate)
                {
                    switch (p.Type)
                    {
                        case DealPaymentDocumentType.DealPaymentFromClient:
                            paymentSum += p.Sum;    // Подсчитываем сумму оплат
                            distributedPaymentSum += p.Distributions.Where(x => x.DistributionDate <= currentDate).Sum(x => x.Sum); // Подсчитываем разнесенную сумму оплат
                            break;

                        case DealPaymentDocumentType.DealCreditInitialBalanceCorrection:
                            creditCorrectionSum += p.Sum;   //Подсчитываем сумму корректировок
                            distributedCreditCorrectionSum += p.Distributions.Where(x => x.DistributionDate <= currentDate).Sum(x => x.Sum);    //Подсчитываем разнесенную сумму корректировок
                            break;
                    }
                }
            }

            return paymentSum - distributedPaymentSum + creditCorrectionSum - distributedCreditCorrectionSum;   // Возвращаем сумму нераспределенных оплат
        }

        #endregion

        #region Места хранения

        /// <summary>
        /// Формирование таблицы по местам хранения
        /// </summary>
        /// <param name="model">Модель данных отчета</param>
        /// <param name="exList">Реализации</param>
        /// <param name="returns">Возвраты по реализациям</param>
        /// <param name="currentDate">Дата отчета</param>
        /// <param name="user">Пользователь</param>
        private void GetStorageTable(Report0007Model model, IEnumerable<ExpenditureWaybill> exList,
            DynamicDictionary<Guid, List<ReceiptedReturnFromClientIndicator>> returns, DateTime currentDate, User user)
        {
            // Группируем по МХ
            foreach (var group in exList.GroupBy(x => x.SenderStorage))
            {
                var row = model.Storages[group.Key.Id]; //Получаем строку таблицы для текущего пользователя

                //Заполняем ее
                FillInTableRowByExpenditureWaybills(group, row, returns, currentDate, user);
                row.Name = group.Key.Name;  // Сохраняем название МХ
                row.Type = group.Key.Type;  // Сохраняем тип МХ (для сортировки)
            }
        }

        /// <summary>
        /// Заполнение модели
        /// </summary>
        /// <param name="dataModel">Модель данных отчета</param>
        /// <param name="viewModel">Модель представления</param>
        private void FullInStorageTableViewModel(Report0007Model dataModel, Report0007ViewModel viewModel)
        {
            foreach (var row in dataModel.Storages.Values.OrderBy(x => x.Type).ThenBy(x => x.Name))
            {
                viewModel.Storages.Items.Add(new Report0007SummaryTableItemViewModel()
                {
                    Name = row.Name,
                    ReserveSum = row.ReserveSum,
                    DebtSum = row.DebtSum,
                    DelayDebtSum = row.DelayDebtSum
                });
            }
            viewModel.Storages.TableTitle = "Информация по МХ";
            viewModel.Storages.FirstColumnName = "Место хранения";
        }

        #endregion

        #region Собственные организации

        /// <summary>
        /// Формирование таблицы по собственным организациям
        /// </summary>
        /// <param name="model">Модель данных отчета</param>
        /// <param name="exList">Реализации</param>
        /// <param name="returns">Возвраты по реализациям</param>
        /// <param name="currentDate">Дата отчета</param>
        /// <param name="user">Пользователь</param>
        private void GetAccountOrganizationTable(Report0007Model model, IEnumerable<ExpenditureWaybill> exList,
            DynamicDictionary<Guid, List<ReceiptedReturnFromClientIndicator>> returns, DateTime currentDate, User user)
        {
            // Группируем по собственным организациям
            foreach (var group in exList.GroupBy(x => x.Deal.Contract.AccountOrganization))
            {
                var row = model.AccountOrganizations[group.Key.Id];  //Получаем строку таблицы для текущей собственной организации

                //Заполняем ее
                FillInTableRowByExpenditureWaybills(group, row, returns, currentDate, user);
                row.Name = group.Key.ShortName; //Сохраняем название
            }
        }

        /// <summary>
        /// Заполнение модели
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="viewModel"></param>
        private void FullInAccountOrganizationTableViewModel(Report0007Model dataModel, Report0007ViewModel viewModel)
        {
            foreach (var row in dataModel.AccountOrganizations.Values.OrderBy(x => x.Name))
            {
                viewModel.AccountOrganizations.Items.Add(new Report0007SummaryTableItemViewModel()
                {
                    Name = row.Name,
                    ReserveSum = row.ReserveSum,
                    DebtSum = row.DebtSum,
                    DelayDebtSum = row.DelayDebtSum
                });
            }
            viewModel.AccountOrganizations.TableTitle = "Информация по собственным организациям";
            viewModel.AccountOrganizations.FirstColumnName = "Название";
        }

        #endregion

        #region Клиенты

        /// <summary>
        /// Формирование таблицы по клиентам
        /// </summary>
        /// <param name="model">Модель данных отчета</param>
        /// <param name="exList">Реализации</param>
        /// <param name="returns">Возвраты по реализациям</param>
        /// <param name="currentDate">Дата отчета</param>
        /// <param name="user">Пользователь</param>
        private void GetClientTable(Report0007Model model, IEnumerable<ExpenditureWaybill> exList,
            DynamicDictionary<Guid, List<ReceiptedReturnFromClientIndicator>> returns, DateTime currentDate, User user)
        {
            // Группируем реализации по клиентам
            foreach (var group in exList.GroupBy(x => x.Deal.Client))
            {
                var row = model.Clients[group.Key.Id];  //Получаем строку таблицы по текущему клиенту

                FillInTableRowByExpenditureWaybills(group, row, returns, currentDate, user);    // Заполняем ее данными по реализациям

                // выбираем из реализаций уникальные сделки, которые еще не были обработанны
                var deals = group.Select(x => x.Deal).Distinct().Where(x => !model.ProcessedDealsForClient[group.Key.Id].Contains(x.Id));
                FillInPaymentInfo(row, deals, currentDate); // Заполняем строку данными по оплатам
                model.ProcessedDealsForClient[group.Key.Id].AddRange(deals.Select(x => x.Id));  //Сохраняем коды обработанных сделок

                row.Name = group.Key.Name;  //Сохраняем имя клиента
            }
        }

        /// <summary>
        /// Заполнение модели
        /// </summary>
        /// <param name="dataModel">Модель данных</param>
        /// <param name="viewModel">Модель представления</param>
        private void FullInClientTableViewModel(Report0007Model dataModel, Report0007ViewModel viewModel)
        {
            foreach (var row in dataModel.Clients.Values.OrderBy(x => x.Name))
            {
                viewModel.Clients.Items.Add(new Report0007SummaryTableItemWithExtendFieldsViewModel()
                {
                    Name = row.Name,
                    ReserveSum = row.ReserveSum,
                    DebtSum = row.DebtSum,
                    DelayDebtSum = row.DelayDebtSum,
                    UndistributionPaymentSum = row.UndistributionPaymentSum,
                    DelayPaymentDays = (row.DelayPaymentDays != null ? row.DelayPaymentDays.Value.Days : (int?)null).ForDisplay()
                });
            }
            viewModel.Clients.TableTitle = "Информация по клентам";
            viewModel.Clients.FirstColumnName = "Клиент";
        }

        #endregion

        #region Организации клиентов

        /// <summary>
        /// Формирование таблицы по органзациям клиентов
        /// </summary>
        /// <param name="model">Модель данных отчета</param>
        /// <param name="exList">Реализации</param>
        /// <param name="returns">Возвраты по реализациям</param>
        /// <param name="currentDate">Дата отчета</param>
        /// <param name="user">Пользователь</param>
        private void GetClientOrganizationTable(Report0007Model model, IEnumerable<ExpenditureWaybill> exList,
            DynamicDictionary<Guid, List<ReceiptedReturnFromClientIndicator>> returns, DateTime currentDate, User user)
        {
            // Группируем реализации по организациям клиента
            foreach (var group in exList.GroupBy(x => x.Deal.Contract.ContractorOrganization))
            {
                var row = model.ClientOrganizations[group.Key.Id];  //Получаем строку таблицы по текущей организации клиента.

                FillInTableRowByExpenditureWaybills(group, row, returns, currentDate, user);    // Заполняем ее данными по реализациям

                // выбираем из реализаций уникальные сделки, которые еще не были обработаны
                var deals = group.Select(x => x.Deal).Distinct().Where(x => !model.ProcessedDealsForClientOrgnization[group.Key.Id].Contains(x.Id));
                FillInPaymentInfo(row, deals, currentDate); // Заполняем строку данными по оплатам
                model.ProcessedDealsForClientOrgnization[group.Key.Id].AddRange(deals.Select(x => x.Id));   //Сохраняем коды обработанных сделок

                row.Name = group.Key.ShortName; //Сохраняем название организации клиента
            }
        }

        /// <summary>
        /// Заполнение модели
        /// </summary>
        /// <param name="dataModel">Модель данных</param>
        /// <param name="viewModel">Модель представления</param>
        private void FullInClientOrganizationTableViewModel(Report0007Model dataModel, Report0007ViewModel viewModel)
        {
            foreach (var row in dataModel.ClientOrganizations.Values.OrderBy(x => x.Name))
            {
                viewModel.ClientOrganizations.Items.Add(new Report0007SummaryTableItemWithExtendFieldsViewModel()
                {
                    Name = row.Name,
                    ReserveSum = row.ReserveSum,
                    DebtSum = row.DebtSum,
                    DelayDebtSum = row.DelayDebtSum,
                    UndistributionPaymentSum = row.UndistributionPaymentSum,
                    DelayPaymentDays = (row.DelayPaymentDays != null ? row.DelayPaymentDays.Value.Days : (int?)null).ForDisplay()
                });
            }
            viewModel.ClientOrganizations.TableTitle = "Информация по организациям клентов";
            viewModel.ClientOrganizations.FirstColumnName = "Название";
        }

        #endregion

        #region Сделки

        /// <summary>
        /// Формирование таблицы по сделкам
        /// </summary>
        /// <param name="model">Модель данных отчета</param>
        /// <param name="exList">Реализации</param>
        /// <param name="returns">Возвраты по реализациям</param>
        /// <param name="currentDate">Дата отчета</param>
        /// <param name="user">Пользователь</param>
        private void GetDealTable(Report0007Model model, IEnumerable<ExpenditureWaybill> exList, DynamicDictionary<Guid, List<ReceiptedReturnFromClientIndicator>> returns,
            DateTime currentDate, User user)
        {
            // группируем реализации по сделке
            foreach (var group in exList.GroupBy(x => x.Deal))
            {
                // Получаем строку таблицы для текущей сделки. Если текущая сделка не видна пользователю, то используем код «невидимой» сущности.
                var row = model.Deals[dealService.IsPossibilityToViewDetails(group.Key, user) ? group.Key.Id : OtherEntityId];

                FillInTableRowByExpenditureWaybills(group, row, returns, currentDate, user);    //Заполняем строку данными по реализациям

                if (!model.ProcessedDeals.Contains(group.Key.Id))   //Если сделка еще не обрабатывалась, то ...
                {
                    // ... заполняем строку данными по оплатам
                    FillInPaymentInfo(row, group.Key, currentDate);
                    model.ProcessedDeals.Add(group.Key.Id); //И сохранеям код сделки в списке обработанных
                }

                // Сохраняем название сделки. Если она не видна пользователю, то подставляем название «невидимой» сделки
                row.Name = dealService.IsPossibilityToViewDetails(group.Key, user) ? group.Key.Name : OtherDealName;
            }
        }

        /// <summary>
        /// Заполнение модели
        /// </summary>
        /// <param name="dataModel">Модель данных</param>
        /// <param name="viewModel">Модель представления</param>
        private void FullInDealTableViewModel(Report0007Model dataModel, Report0007ViewModel viewModel)
        {
            foreach (var row in dataModel.Deals.Values.OrderBy(x => x.Name))
            {
                viewModel.Deals.Items.Add(new Report0007SummaryTableItemWithExtendFieldsViewModel()
                {
                    Name = row.Name,
                    ReserveSum = row.ReserveSum,
                    DebtSum = row.DebtSum,
                    DelayDebtSum = row.DelayDebtSum,
                    UndistributionPaymentSum = row.UndistributionPaymentSum,
                    DelayPaymentDays = (row.DelayPaymentDays != null ? row.DelayPaymentDays.Value.Days : (int?)null).ForDisplay()
                });
            }
            viewModel.Deals.TableTitle = "Информация по сделкам";
            viewModel.Deals.FirstColumnName = "Сделка";
        }

        #endregion

        #region Команды

        /// <summary>
        /// Формирование таблицы по командам
        /// </summary>
        /// <param name="model">Модель данных отчета</param>
        /// <param name="exList">Реализации</param>
        /// <param name="returns">Возвраты по реализациям</param>
        /// <param name="currentDate">Дата отчета</param>
        /// <param name="user">Пользователь</param>
        private void GetTeamTable(Report0007Model model, IEnumerable<ExpenditureWaybill> exList, DynamicDictionary<Guid, List<ReceiptedReturnFromClientIndicator>> returns,
            DateTime currentDate, User user)
        {
            foreach (var group in exList.GroupBy(x => x.Team))
            {
                int id; //Код команды для обработки
                string name;    //Отображаемое название команды
                    
                if (teamService.IsPossibilityToViewDetails(group.Key, user))    // Может ли пользователь ее видеть?
                {
                    // Да, может.
                    id = group.Key.Id;
                    name = group.Key.Name;
                }
                else
                {
                    // Нет, не может. Относим ее к группе «прочих»
                    id = OtherEntityId;
                    name = OtherTeamName;
                }                
                
                var row = model.Teams[id];  // Получаем строку таблицы по коду команды

                FillInTableRowByExpenditureWaybills(group, row, returns, currentDate, user);    //Заполняем ее данными
                row.Name = name;    // сохраняем отображаемое название команды
            }
        }

        /// <summary>
        /// Заполнение модели
        /// </summary>
        /// <param name="dataModel">Модель данных</param>
        /// <param name="viewModel">Модель представления</param>
        private void FullInTeamTableViewModel(Report0007Model dataModel, Report0007ViewModel viewModel)
        {
            foreach (var row in dataModel.Teams.Values.OrderBy(x => x.Name))
            {
                viewModel.Teams.Items.Add(new Report0007SummaryTableItemViewModel()
                {
                    Name = row.Name,
                    ReserveSum = row.ReserveSum,
                    DebtSum = row.DebtSum,
                    DelayDebtSum = row.DelayDebtSum
                });
            }
            viewModel.Teams.TableTitle = "Информация по командам";
            viewModel.Teams.FirstColumnName = "Команда";
        }

        #endregion

        #region Пользователи

        /// <summary>
        /// Формирование таблицы по пользователям
        /// </summary>
        /// <param name="model">Модель данных отчета</param>
        /// <param name="exList">Реализации</param>
        /// <param name="returns">Возвраты по реализациям</param>
        /// <param name="currentDate">Дата отчета</param>
        /// <param name="user">Пользователь</param>
        private void GetUserTable(Report0007Model model, IEnumerable<ExpenditureWaybill> exList, DynamicDictionary<Guid, List<ReceiptedReturnFromClientIndicator>> returns,
           DateTime currentDate, User user)
        {
            // Группируем реализации по пользователям
            foreach (var group in exList.GroupBy(x => x.Curator))
            {
                var row = model.Users[group.Key.Id];    // Получаем строку таблицы по коду текущего клиента

                FillInTableRowByExpenditureWaybills(group, row, returns, currentDate, user);    // заполняем ее данными
                row.Name = group.Key.DisplayName;   //Сохраняем имя пользователя
            }
        }

        /// <summary>
        /// Заполнение модели
        /// </summary>
        /// <param name="dataModel">Модель данных</param>
        /// <param name="viewModel">Модель представления</param>
        private void FullInUserTableViewModel(Report0007Model dataModel, Report0007ViewModel viewModel)
        {
            foreach (var row in dataModel.Users.Values.OrderBy(x => x.Name))
            {
                viewModel.Users.Items.Add(new Report0007SummaryTableItemViewModel()
                {
                    Name = row.Name,
                    ReserveSum = row.ReserveSum,
                    DebtSum = row.DebtSum,
                    DelayDebtSum = row.DelayDebtSum
                });
            }
            viewModel.Users.TableTitle = "Информация по пользователям";
            viewModel.Users.FirstColumnName = "Пользователь";
        }

        #endregion

        #region Реализации (Развернутая таблица)

        #region Формирование данных

        /// <summary>
        /// Формирование таблицы по пользователям
        /// </summary>
        /// <param name="model">Модель данных отчета</param>
        /// <param name="exList">Реализации</param>
        /// <param name="returns">Возвраты по реализациям</param>
        /// <param name="currentDate">Дата отчета</param>
        /// <param name="user">Пользователь</param>
        private void GetExpenditureWaybillTable(Report0007Model model, IEnumerable<ExpenditureWaybill> exList,
            DynamicDictionary<Guid, List<ReceiptedReturnFromClientIndicator>> returns, DateTime currentDate, User user)
        {
            // Обрабатываем только отгруженные реализации в порядке их дат
            foreach (var exWaybill in exList.Where(x => x.ShippingDate != null && x.ShippingDate <= currentDate).OrderBy(x => x.Date))
            {
                var pSum = exWaybill.Distributions.Where(x => x.DistributionDate <= currentDate).Sum(x => x.Sum);   // Сумма оплаты по реализации
                var returnSum = returns[exWaybill.Id].Sum(x => x.SalePriceSum); //Сумма возвратов по реализации
                var postPaymetDays = exWaybill.Quota.PostPaymentDays;   // Отсрочка оплаты в днях

                // Вычисляем крайнюю дату оплаты. Если отсрочка не предоставлялась, то null.
                // Хотя просрочить оплату реализации, по которой нет отсрочки нельзя.
                var paymentDate = exWaybill.ShippingDate != null && postPaymetDays > 0 ?
                    exWaybill.ShippingDate.Value.AddDays(postPaymetDays.Value) : (DateTime?)null;

                if (!model.ShowOnlyDelayDebt || (paymentDate != null && currentDate > paymentDate))  // Если необходимо вывести только просроченные, то проверяем факт просрочки.
                {
                    // Задолженность просрочена, выводим ее.

                    var row = model.ExpenditureWaybills[exWaybill.Id];

                    row.Date = exWaybill.Date;
                    row.Number = exWaybill.Number;
                    row.SaleSum = exWaybill.SalePriceSum - returnSum;
                    row.DebtSum = row.SaleSum - pSum;
                    row.AcceptanceDate = exWaybill.AcceptanceDate.Value;
                    row.ShippingDate = exWaybill.ShippingDate.Value;
                    row.PaymentDate = paymentDate;
                    row.DelayPaymentDays = currentDate - row.PaymentDate;
                    row.PostPaymentDays = postPaymetDays;

                    // Выставляем признаки для группировки
                    row.Storage.Id = exWaybill.SenderStorage.Id;
                    row.Storage.Name = exWaybill.SenderStorage.Name;
                    row.Storage.Entity = exWaybill.SenderStorage;

                    
                    if (teamService.IsPossibilityToViewDetails(exWaybill.Team, user))   // команда видна пользователю
                    {
                        // то выставляем ее настоящие код и навзание
                        row.Team.Id = exWaybill.Team.Id;
                        row.Team.Name = exWaybill.Team.Name;
                        row.Team.Entity = exWaybill.Team;
                    }
                    else
                    {
                        // иначе подменяем их на «прочие»
                        row.Team.Id = OtherEntityId;
                        row.Team.Name = OtherTeamName;
                    }

                    row.User.Id = exWaybill.Curator.Id;
                    row.User.Name = exWaybill.Curator.DisplayName;
                    row.User.Entity = exWaybill.Curator;

                    row.Client.Id = exWaybill.Deal.Client.Id;
                    row.Client.Name = exWaybill.Deal.Client.Name;
                    row.Client.Entity = exWaybill.Deal.Client;

                    row.ClientOrganization.Id = exWaybill.Deal.Contract.ContractorOrganization.Id;
                    row.ClientOrganization.Name = exWaybill.Deal.Contract.ContractorOrganization.ShortName;
                    row.ClientOrganization.Entity = exWaybill.Deal.Contract.ContractorOrganization.As<ClientOrganization>();

                    row.AccountOrganization.Id = exWaybill.Deal.Contract.AccountOrganization.Id;
                    row.AccountOrganization.Name = exWaybill.Deal.Contract.AccountOrganization.ShortName;
                    row.AccountOrganization.Entity = exWaybill.Deal.Contract.AccountOrganization;
                }
            }
        }

        #endregion

        #region Формирование данных представления

        /// <summary>
        /// Заполнение модели
        /// </summary>
        /// <param name="dataModel">Модель данных</param>
        /// <param name="viewModel">Модель представления</param>
        private void FullInExpenditureWaybillTableViewModel(Report0007Model dataModel, Report0007ViewModel viewModel)
        {
            GroupByExpenditureWaybillTableRow(dataModel.ExpenditureWaybills.Values, dataModel, viewModel, 1); // Запускаем рекурсивную группировку
        }

        /// <summary>
        /// Заполнение представления для группы строк реализаций
        /// </summary>
        /// <param name="list">Список строк развернутой таблицы реализаций</param>
        /// <param name="viewModel">Модель представления</param>
        private void FullInExpenditureWaybillTableRow(IEnumerable<ReportExpenditureWaybillTableRow> list, Report0007ViewModel viewModel)
        {
            foreach (var row in list.OrderBy(x => x.Date))
            {
                viewModel.ExpenditureWaybillTable.Add(new Report0007ExpenditureWaybillItemViewModel()
                {
                    AcceptanceDate = row.AcceptanceDate.ToShortDateString(),
                    Date = row.Date.ToShortDateString(),
                    DebtSum = row.DebtSum,
                    DelayPaymentDays = row.DelayPaymentDays!= null? row.DelayPaymentDays.Value.Days.ForDisplay(): "---",
                    Number = row.Number,
                    PaymentDate = row.PaymentDate.ForDisplay(),
                    PostPaymentDays = row.PostPaymentDays > 0? row.PostPaymentDays.ForDisplay(): "---",
                    SaleSum = row.SaleSum,
                    ShippingDate = row.ShippingDate.ToShortDateString()
                });
            }
        }

        /// <summary>
        /// Рекурсивная группировка реализаций
        /// </summary>
        /// <param name="list">Список строк развернутой таблицы реализаций</param>
        /// <param name="dataModel">Модель данных</param>
        /// <param name="viewModel">Модель представления</param>
        /// <param name="groupLevel">Уровень текущей группы</param>
        private void GroupByExpenditureWaybillTableRow(IEnumerable<ReportExpenditureWaybillTableRow> list, Report0007Model dataModel, Report0007ViewModel viewModel, int groupLevel)
        {
            if (dataModel.GroupFields.Count >= groupLevel)  //Если уровень группы не меньше количества полей для группировки, то
            {
                switch (dataModel.GroupFields[groupLevel - 1])  // Выполняем группировку по указанному полю
                {
                    case (byte)GroupingType.ByStorage:
                        GroupByStorage(list, dataModel, viewModel, groupLevel);
                        break;
                    case (byte)GroupingType.ByTeam:
                        GroupByTeam(list, dataModel, viewModel, groupLevel);
                        break;
                    case (byte)GroupingType.ByUser:
                        GroupByUser(list, dataModel, viewModel, groupLevel);
                        break;
                    case (byte)GroupingType.ByClient:
                        GroupByClient(list, dataModel, viewModel, groupLevel);
                        break;
                    case (byte)GroupingType.ByClientOrganization:
                        GroupByClientOrganization(list, dataModel, viewModel, groupLevel);
                        break;
                    case (byte)GroupingType.ByAccountOrganization:
                        GroupByAccountOrganization(list, dataModel, viewModel, groupLevel);
                        break;
                    default:
                        throw new Exception("Неизвестная группировка данных.");
                }
            }
            else
            {
                // иначе данные сгруппированы, выводим строки таблицы
                FullInExpenditureWaybillTableRow(list, viewModel);
            }
        }

        #region Методы группировки строк таблицы реализаций

        /// <summary>
        /// Группировка по МХ
        /// </summary>
        /// <param name="list">Список строк развернутой таблицы реализаций</param>
        /// <param name="dataModel">Модель данных</param>
        /// <param name="viewModel">Модель представления</param>
        /// <param name="groupLevel">Уровень группировки</param>
        private void GroupByStorage(IEnumerable<ReportExpenditureWaybillTableRow> list, Report0007Model dataModel, Report0007ViewModel viewModel, int groupLevel)
        {
            foreach (var group in list.GroupBy(x => x.Storage).OrderBy(x => x.Key.Entity.Type).ThenBy(x => x.Key.Name))
            {
                viewModel.ExpenditureWaybillTable.Add(new Report0007ExpenditureWaybillItemViewModel()
                {
                    IsGroup = true,
                    GroupTitle = String.Format("Место хранения: {0}", group.Key.Name),
                    GroupLevel = groupLevel
                }); // Добавляем заголовок группы

                GroupByExpenditureWaybillTableRow(group, dataModel, viewModel, groupLevel + 1); // группируем данные далее
            }
        }

        /// <summary>
        /// Группировка по командам
        /// </summary>
        /// <param name="list">Список строк развернутой таблицы реализаций</param>
        /// <param name="dataModel">Модель данных</param>
        /// <param name="viewModel">Модель представления</param>
        /// <param name="groupLevel">Уровень группировки</param>
        private void GroupByTeam(IEnumerable<ReportExpenditureWaybillTableRow> list, Report0007Model dataModel, Report0007ViewModel viewModel, int groupLevel)
        {
            foreach (var group in list.GroupBy(x => x.Team).OrderBy(x => x.Key.Name))
            {
                viewModel.ExpenditureWaybillTable.Add(new Report0007ExpenditureWaybillItemViewModel()
                {
                    IsGroup = true,
                    GroupTitle = String.Format("Команда: {0}", group.Key.Name),
                    GroupLevel = groupLevel
                }); // Добавляем заголовок группы

                GroupByExpenditureWaybillTableRow(group, dataModel, viewModel, groupLevel + 1); // группируем данные далее
            }
        }

        /// <summary>
        /// Группировка по пользователям
        /// </summary>
        /// <param name="list">Список строк развернутой таблицы реализаций</param>
        /// <param name="dataModel">Модель данных</param>
        /// <param name="viewModel">Модель представления</param>
        /// <param name="groupLevel">Уровень группировки</param>
        private void GroupByUser(IEnumerable<ReportExpenditureWaybillTableRow> list, Report0007Model dataModel, Report0007ViewModel viewModel, int groupLevel)
        {
            foreach (var group in list.GroupBy(x => x.User).OrderBy(x => x.Key.Name))
            {
                viewModel.ExpenditureWaybillTable.Add(new Report0007ExpenditureWaybillItemViewModel()
                {
                    IsGroup = true,
                    GroupTitle = String.Format("Пользователь: {0}", group.Key.Name),
                    GroupLevel = groupLevel
                }); // Добавляем заголовок группы

                GroupByExpenditureWaybillTableRow(group, dataModel, viewModel, groupLevel + 1); // группируем данные далее
            }
        }

        /// <summary>
        /// Группировка по клиентам
        /// </summary>
        /// <param name="list">Список строк развернутой таблицы реализаций</param>
        /// <param name="dataModel">Модель данных</param>
        /// <param name="viewModel">Модель представления</param>
        /// <param name="groupLevel">Уровень группировки</param>
        private void GroupByClient(IEnumerable<ReportExpenditureWaybillTableRow> list, Report0007Model dataModel, Report0007ViewModel viewModel, int groupLevel)
        {
            foreach (var group in list.GroupBy(x => x.Client).OrderBy(x => x.Key.Name))
            {
                viewModel.ExpenditureWaybillTable.Add(new Report0007ExpenditureWaybillItemViewModel()
                {
                    IsGroup = true,
                    GroupTitle = String.Format("Клиент: {0}", group.Key.Name),
                    GroupLevel = groupLevel
                }); // Добавляем заголовок группы

                GroupByExpenditureWaybillTableRow(group, dataModel, viewModel, groupLevel + 1); // группируем данные далее
            }
        }

        /// <summary>
        /// Группировка по организациям клиентов
        /// </summary>
        /// <param name="list">Список строк развернутой таблицы реализаций</param>
        /// <param name="dataModel">Модель данных</param>
        /// <param name="viewModel">Модель представления</param>
        /// <param name="groupLevel">Уровень группировки</param>
        private void GroupByClientOrganization(IEnumerable<ReportExpenditureWaybillTableRow> list, Report0007Model dataModel, Report0007ViewModel viewModel, int groupLevel)
        {
            foreach (var group in list.GroupBy(x => x.ClientOrganization).OrderBy(x => x.Key.Name))
            {
                viewModel.ExpenditureWaybillTable.Add(new Report0007ExpenditureWaybillItemViewModel()
                {
                    IsGroup = true,
                    GroupTitle = String.Format("Организация клиента: {0}", group.Key.Name),
                    GroupLevel = groupLevel
                }); // Добавляем заголовок группы

                GroupByExpenditureWaybillTableRow(group, dataModel, viewModel, groupLevel + 1); // группируем данные далее
            }
        }

        /// <summary>
        /// Группировка по собственным организациям
        /// </summary>
        /// <param name="list">Список строк развернутой таблицы реализаций</param>
        /// <param name="dataModel">Модель данных</param>
        /// <param name="viewModel">Модель представления</param>
        /// <param name="groupLevel">Уровень группировки</param>
        private void GroupByAccountOrganization(IEnumerable<ReportExpenditureWaybillTableRow> list, Report0007Model dataModel, Report0007ViewModel viewModel, int groupLevel)
        {
            foreach (var group in list.GroupBy(x => x.AccountOrganization).OrderBy(x => x.Key.Name))
            {
                viewModel.ExpenditureWaybillTable.Add(new Report0007ExpenditureWaybillItemViewModel()
                {
                    IsGroup = true,
                    GroupTitle = String.Format("Собственная организация: {0}", group.Key.Name),
                    GroupLevel = groupLevel
                }); // Добавляем заголовок группы

                GroupByExpenditureWaybillTableRow(group, dataModel, viewModel, groupLevel + 1); // группируем данные далее
            }
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #region выгрузка в Excel
        
        /// <summary>
        /// Экспорт отчета в Excel
        /// </summary>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="currentUser">Текущий пользователь</param>
        public byte[] Report0007ExportToExcel(Report0007SettingsViewModel settings, UserInfo currentUser)
        {
            var viewModel = Report0007(settings, currentUser);

            string reportHeader = "Отчет «Взаиморасчеты по реализациям» \r\nна дату " + viewModel.Date;
            string sign = "Форма: Report0007" + "\r\nАвтор: " + viewModel.CreatedBy + "\r\nСоставлен: " + viewModel.CreationData;
            
            using (ExcelPackage pck = new ExcelPackage())
            {
                if (viewModel.ShowStorageTable)
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Информация по МХ");
                    FillSummaryTable(sheet, 4, viewModel.Storages, sheet.PrintHeader(4, reportHeader, sign, viewModel.Storages.TableTitle + ":", 1));
                }
                if (viewModel.ShowAccountOrganizationTable)
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Информация по собственным организациям");
                    FillSummaryTable(sheet, 4, viewModel.AccountOrganizations, sheet.PrintHeader(4, reportHeader, sign, viewModel.AccountOrganizations.TableTitle + ":", 1));
                }
                if (viewModel.ShowClientTable)
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Информация по клентам");
                    FillSummaryTableWithExtendFields(sheet, 6, viewModel.Clients, sheet.PrintHeader(6, reportHeader, sign, viewModel.Clients.TableTitle + ":", 1));
                }
                if (viewModel.ShowClientOrganizationTable)
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Информация по организациям клентов");
                    FillSummaryTableWithExtendFields(sheet, 6, viewModel.ClientOrganizations, sheet.PrintHeader(6, reportHeader, sign, viewModel.ClientOrganizations.TableTitle + ":", 1));
                }
                if (viewModel.ShowDealTable)
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Информация по сделкам");
                    FillSummaryTableWithExtendFields(sheet, 6, viewModel.Deals, sheet.PrintHeader(6, reportHeader, sign, viewModel.Deals.TableTitle + ":", 1));
                }
                if (viewModel.ShowTeamTable)
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Информация по командам");
                    FillSummaryTable(sheet, 4, viewModel.Teams, sheet.PrintHeader(4, reportHeader, sign, viewModel.Teams.TableTitle + ":", 1));
                }
                if (viewModel.ShowUserTable)
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Информация по пользователям");
                    FillSummaryTable(sheet, 4, viewModel.Users, sheet.PrintHeader(4, reportHeader, sign, viewModel.Users.TableTitle + ":", 1));
                }
                if (viewModel.ShowExpenditureWaybillTable)
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Развернутая реализациям");
                    FillDetailsTable(sheet, 9, viewModel, sheet.PrintHeader(9, reportHeader, sign, "Развернутая таблица по реализациям:", 1));
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
        /// Формирует сводную таблицу с 4 колонками
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillSummaryTable(ExcelWorksheet workSheet, int columns, Report0007SummaryTableViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.FirstColumnName);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007SummaryTableItemViewModel>(x => x.ReserveSum));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007SummaryTableItemViewModel>(x => x.DebtSum));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007SummaryTableItemViewModel>(x => x.DelayDebtSum));
            currentCol = 1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion
            
            #region Строки
            bool flag = false;
            foreach (var row in viewModel.Items)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle());

                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Name).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ReserveSum, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DebtSum, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DelayDebtSum, ValueDisplayType.Money);
                currentCol = 1;
                currentRow++;
                flag = !flag;
            }
            if (viewModel.Items.Count == 0)
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
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ReserveSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.DebtSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.DelayDebtSumTotal, ValueDisplayType.Money);
            currentCol = 1;
            currentRow++;
            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        /// <summary>
        /// Формирует сводную таблицу с 6 колонками
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillSummaryTableWithExtendFields(ExcelWorksheet workSheet, int columns, Report0007SummaryTableWithExtendFieldsViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.FirstColumnName);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007SummaryTableItemWithExtendFieldsViewModel>(x => x.ReserveSum));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007SummaryTableItemWithExtendFieldsViewModel>(x => x.DebtSum));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007SummaryTableItemWithExtendFieldsViewModel>(x => x.DelayDebtSum));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007SummaryTableItemWithExtendFieldsViewModel>(x => x.UndistributionPaymentSum));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007SummaryTableItemWithExtendFieldsViewModel>(x => x.DelayPaymentDays));
            currentCol = 1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки
            bool flag = false;
            foreach (var row in viewModel.Items)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle());

                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Name).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ReserveSum, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DebtSum, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DelayDebtSum, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.UndistributionPaymentSum, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DelayPaymentDays, ValueDisplayType.Money);
                currentCol = 1;
                currentRow++;
                flag = !flag;
            }
            if (viewModel.Items.Count == 0)
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
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ReserveSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.DebtSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.DelayDebtSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.UndistributionPaymentSumTotal, ValueDisplayType.Money);
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
        private int FillDetailsTable(ExcelWorksheet workSheet, int columns, Report0007ViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007ExpenditureWaybillItemViewModel>(x => x.Date));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007ExpenditureWaybillItemViewModel>(x => x.Number));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007ExpenditureWaybillItemViewModel>(x => x.SaleSum));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007ExpenditureWaybillItemViewModel>(x => x.DebtSum));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007ExpenditureWaybillItemViewModel>(x => x.AcceptanceDate));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007ExpenditureWaybillItemViewModel>(x => x.ShippingDate));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007ExpenditureWaybillItemViewModel>(x => x.PaymentDate));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007ExpenditureWaybillItemViewModel>(x => x.PostPaymentDays));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0007ExpenditureWaybillItemViewModel>(x => x.DelayPaymentDays));
            currentCol = 1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки
            bool flag = false;

            foreach (var row in viewModel.ExpenditureWaybillTable)
            {
                if (row.IsGroup)
                {
                    flag = false;
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle())
                        .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, indent: row.GroupLevel).SetFormattedValue(row.GroupTitle);
                    currentRow++;
                }
                else
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Date).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SaleSum, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DebtSum, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AcceptanceDate).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ShippingDate).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PaymentDate).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PostPaymentDays);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.DelayPaymentDays);
                    currentCol = 1;
                    currentRow++;
                    flag = !flag;
                }
            }
            if (viewModel.ExpenditureWaybillTable.Count == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().ApplyStyle(ExcelUtils.GetTableEvenRowStyle())
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center).SetFormattedValue("Нет данных");
                currentRow++;
            }

            #endregion

            #region Итого
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 1].MergeRange().SetFormattedValue("Итого:").ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
            currentCol += 2;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.SaleSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.DebtSumTotal, ValueDisplayType.Money);
            currentCol = 1;
            currentRow++;
            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }
        #endregion

        #endregion
    }
}