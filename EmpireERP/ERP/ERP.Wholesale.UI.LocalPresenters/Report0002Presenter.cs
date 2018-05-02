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
using ERP.Wholesale.Domain.Misc.Report.Report0002;
using ERP.Wholesale.Domain.Repositories.Report;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Report.Report0002;
using OfficeOpenXml;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class Report0002Presenter : BaseReportPresenter, IReport0002Presenter
    {
        #region Дополнительные классы

        /// <summary>
        /// Разрешения на просмотр для одного МХ
        /// </summary>
        private class ViewPermissions
        {
            /// <summary>
            /// Разрешение на просмотр ЗЦ
            /// </summary>
            public bool AllowToViewPurchaseCost { get; set; }
        }

        /// <summary>
        /// Опции формирования отчета
        /// </summary>
        private class ReportSettings
        {
            #region Свойства

            /// <summary>
            /// Вывод в ЗЦ
            /// </summary>
            public bool InPurchaseCost { get; set; }

            /// <summary>
            /// Вывод в УЦ
            /// </summary>
            public bool InAccountingPrice { get; set; }

            /// <summary>
            /// Вывод в ОЦ
            /// </summary>
            public bool InSalePrice { get; set; }

            /// <summary>
            /// Расчет средних цен
            /// </summary>
            public bool CalculateAveragePrice { get; set; }

            /// <summary>
            /// Расчет наценки
            /// </summary>
            public bool CalculateMarkup { get; set; }

            /// <summary>
            /// Учитываются ли возвраты
            /// </summary>
            public bool WithReturnFromClient { get; set; }

            /// <summary>
            /// Признак разделения партий
            /// </summary>
            public bool IsDevideByBatch { get; set; }

            /// <summary>
            /// Признак вывода МХ в столбцах
            /// </summary>
            public bool AreStoragesInColumns { get; set; }

            /// <summary>
            /// Вывод дополнительных столбцов
            /// </summary>
            public bool ShowAdditionColumns { get; set; }

            /// <summary>
            /// Поля для группировки
            /// </summary>
            public List<GroupingType> GroupFields { get; set; }

            /// <summary>
            ///  Вывод развернутой таблицы в сокращенном виде
            /// </summary>
            public bool ShowShortDetailsTable { get; set; }

            /// <summary>
            /// Показывать количество проданного товара?
            /// </summary>
            public bool ShowSoldArticleCount { get; set; }

            /// <summary>
            /// Права на просмотр
            /// </summary>
            public DynamicDictionary<short, ViewPermissions> ViewPermissions { get; private set; }

            #endregion

            #region Конструктор

            /// <summary>
            /// Конструктор
            /// </summary>
            public ReportSettings()
            {
                GroupFields = new List<GroupingType>();
                ViewPermissions = new DynamicDictionary<short, ViewPermissions>();
            }

            #endregion
        }

        /// <summary>
        /// Суммарная инфорация по группе позиций отчета
        /// </summary>
        private class ReportGroupRowSummaryInfo
        {
            /// <summary>
            /// Сумма реализации в ЗЦ
            /// </summary>
            public decimal SoldArticlesInPurchaseCostSum { get; set; }

            /// <summary>
            /// Сумма возвратов в ЗЦ
            /// </summary>
            public decimal ReturnArticlesInPurchaseCostSum { get; set; }

            /// <summary>
            /// Сумма реализаций в ОЦ
            /// </summary>
            public decimal SoldArticlesInSalePriceSum { get; set; }

            /// <summary>
            /// Сумма возвратов в ОЦ
            /// </summary>
            public decimal ReturnArticlesInSalePriceSum { get; set; }

            /// <summary>
            /// Сумма реализаций в УЦ
            /// </summary>
            public decimal SoldArticlesInAccountingPriceSum { get; set; }

            /// <summary>
            /// Сумма возвратов в УЦ
            /// </summary>
            public decimal ReturnedArticlesInAccountingPriceSum { get; set; }

            /// <summary>
            /// Сумма наценки
            /// </summary>
            public decimal MarkupSum { get; set; }

            /// <summary>
            /// Количество проданных товаров
            /// </summary>
            public decimal SoldCount { get; set; }

            /// <summary>
            /// Количество возвращенных товаров
            /// </summary>
            public decimal ReturnedCount { get; set; }

            /// <summary>
            /// Детализация суммы реализации по МХ. Словарь [код МХ][Сумма реализаций в ОЦ]
            /// </summary>
            public DynamicDictionary<short, ReportSummaryInfoByStorages> SummaryInfoByStorages { get; private set; }

            /// <summary>
            /// Конструктор
            /// </summary>
            public ReportGroupRowSummaryInfo()
            {
                SummaryInfoByStorages = new DynamicDictionary<short, ReportSummaryInfoByStorages>();
            }
        }

        /// <summary>
        /// Суммарная информация по реализациям по МХ
        /// </summary>
        private class ReportSummaryInfoByStorages
        {
            /// <summary>
            /// Сумма в ОЦ
            /// </summary>
            public decimal SalePriceSum { get; set; }

            /// <summary>
            /// Сумма в ЗЦ
            /// </summary>
            public decimal PurchaseCostSum { get; set; }

            /// <summary>
            /// Сумма в УЦ
            /// </summary>
            public decimal AccountingPriceSum { get; set; }

            /// <summary>
            /// Количество проданных товаров
            /// </summary>
            public decimal SoldCount { get; set; }

            /// <summary>
            /// Остаток товаров на МХ
            /// </summary>
            public decimal AvailableArticleCount { get; set; }

            /// <summary>
            /// Количество возвращенных товаров
            /// </summary>
            public decimal ReturnedCount { get; set; }
        }

        #endregion

        #region Поля

        private readonly IStorageService storageService;
        private readonly IArticleGroupService articleGroupService;
        private readonly IClientService clientService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly IReport0002Repository report0002Repository;

        /// <summary>
        /// Доступные типы группировок
        /// </summary>
        private enum GroupingType
        {
            /// <summary>
            /// По МХ
            /// </summary>
            [EnumDisplayName("Место хранения")]
            ByStorage = 1,

            /// <summary>
            /// По собственной организации
            /// </summary>
            [EnumDisplayName("Собственная организация")]
            ByAccountOrganization = 2,

            /// <summary>
            /// По команде
            /// </summary>
            [EnumDisplayName("Команда")]
            ByTeam = 3,

            /// <summary>
            /// По пользователю
            /// </summary>
            [EnumDisplayName("Пользователь")]
            ByUser = 4,

            /// <summary>
            /// По клиенту
            /// </summary>
            [EnumDisplayName("Клиент")]
            ByClient = 5,

            /// <summary>
            /// Организация клиента
            /// </summary>
            [EnumDisplayName("Организация клиента")]
            ByClientOrganization = 6,

            /// <summary>
            /// По поставщику / производителю
            /// </summary>
            [EnumDisplayName("Поставщик/производитель")]
            ByContractor = 7,

            /// <summary>
            /// По группе товара
            /// </summary>
            [EnumDisplayName("Группа/подгруппа")]
            ByArticleGroup = 8
        };

        #endregion

        #region Конструкторы

        public Report0002Presenter(IUnitOfWorkFactory unitOfWorkFactory, IStorageService storageService,
            IArticleGroupService articleGroupService, IClientService clientService,
            IAccountOrganizationService accountOrganizationService, IUserService userService,
            IReport0002Repository report0002Repository): base(unitOfWorkFactory, userService)
        {
            this.storageService = storageService;
            this.articleGroupService = articleGroupService;
            this.clientService = clientService;
            this.accountOrganizationService = accountOrganizationService;

            this.report0002Repository = report0002Repository;
        }

        #endregion

        #region Методы

        #region Настройки отчета

        public Report0002SettingsViewModel Report0002Settings(string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Report0002_View);

                var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

                var model = new Report0002SettingsViewModel()
                {
                    BackURL = backURL,
                    Storages = GetStorageSelectorLocal(true, user), // По умолчанию УЦ выводятся
                    ArticleGroups = articleGroupService.GetList().Where(x => x.Parent != null).OrderBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name),
                    Clients = clientService.GetList(user).OrderBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name),
                    Users = userService.GetList(user, Permission.Report0002_User_List).OrderBy(x => x.DisplayName).ToDictionary(x => x.Id.ToString(), x => x.DisplayName),
                    AccountOrganizations = accountOrganizationService.GetList().OrderBy(x => x.ShortName).ToDictionary(x => x.Id.ToString(), x => x.ShortName),
                    GroupByCollection = ComboBoxBuilder.GetComboBoxItemList<GroupingType>(false, false) // Получаем список значений перечисления
                        .Where(x => x.Value != GroupingType.ByArticleGroup.ValueToString()),    // Отбрасываем группировку по группам товаров, т.к. она делается по умолчанию
                    AllowToViewPurchaseCost = allowToViewPurchaseCost,
                    CreateByArticleGroup = "1",
                    ArticleGroupName = "Выберите наименование группы товаров"
                };

                // если у пользователя нет прав на просмотр закупочных цен - не выводим их 
                if (!allowToViewPurchaseCost)
                {
                    model.CalculateMarkup = "0";
                    model.InPurchaseCost = "0";
                }

                return model;
            }
        }

        /// <summary>
        /// Получение списка МХ для построения отчета
        /// </summary>
        /// <param name="inAccountingPrice">Признак вывода УЦ</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель списка МХ для выбора</returns>
        public Report0002StorageSelectorViewModel GetStorageSelector(string inAccountingPrice, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetStorageSelectorLocal(ValidationUtils.TryGetBool(inAccountingPrice), user);
            }
        }

        /// <summary>
        /// Получение списка МХ для построения отчета
        /// </summary>
        /// <param name="inAccountingPrice">Признак вывода УЦ</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель списка МХ для выбора</returns>
        private Report0002StorageSelectorViewModel GetStorageSelectorLocal(bool inAccountingPrice, User user)
        {
            // МХ по правам на отчет
            IEnumerable<Storage> storageList = storageService.GetList(user, Permission.Report0002_Storage_List)
                .OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name);
            
			// Если нужно вывести УЦ и пользователь не может видеть их на некомандных МХ, то ...
            if (inAccountingPrice && !user.HasPermission(Permission.AccountingPrice_NotCommandStorage_View))
            {
                // ... огранициваем видимые МХ командными
                storageList = storageList.Intersect(user.Teams.SelectMany(x => x.Storages).Distinct());
            }

            return new Report0002StorageSelectorViewModel() { Storages = storageList.ToDictionary(x => x.Id.ToString(), x => x.Name) };
        }

        #endregion

        #region Report0002 Построение отчета

        /// <summary>
        /// Построение отчета 0002
        /// </summary>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель отчета</returns>
        public Report0002ViewModel Report0002(Report0002SettingsViewModel settings, UserInfo currentUser)
        {
            ReportSettings reportSettings;

            return Report0002(settings, currentUser, out reportSettings);
        }

        /// <summary>
        /// Построение отчета 0002
        /// </summary>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="currentUser">Пользователь</param>
        /// <param name="reportSettings">Распарсенные настройки отчета</param>
        /// <returns>Модель отчета</returns>
        private Report0002ViewModel Report0002(Report0002SettingsViewModel settings, UserInfo currentUser, out ReportSettings reportSettings)
        {
            /*using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {*/
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Report0002_View);

                reportSettings = new ReportSettings()
                {
                    AreStoragesInColumns = ValidationUtils.TryGetBool(settings.StoragesInColumns),
                    CalculateAveragePrice = ValidationUtils.TryGetBool(settings.InAvgPrice),
                    CalculateMarkup = ValidationUtils.TryGetBool(settings.CalculateMarkup),
                    InAccountingPrice = ValidationUtils.TryGetBool(settings.InAccountingPrice),
                    InPurchaseCost = ValidationUtils.TryGetBool(settings.InPurchaseCost),
                    InSalePrice = ValidationUtils.TryGetBool(settings.InSalePrice),
                    IsDevideByBatch = ValidationUtils.TryGetBool(settings.DevideByBatch),
                    WithReturnFromClient = ValidationUtils.TryGetBool(settings.WithReturnFromClient),
                    ShowAdditionColumns = ValidationUtils.TryGetBool(settings.ShowAdditionColumns),
                    ShowShortDetailsTable = ValidationUtils.TryGetBool(settings.ShowShortDetailsTable),
                    ShowSoldArticleCount = ValidationUtils.TryGetBool(settings.ShowSoldArticleCount)
                };

                DateTime startDate, endDate;
                ParseDatePeriod(settings.StartDate, settings.EndDate, DateTimeUtils.GetCurrentDateTime(), out startDate, out endDate);

                var model = new Report0002ViewModel()
                {
                    StartDate = startDate.ToShortDateString(),
                    EndDate = endDate.ToShortDateString(),
                    CreatedBy = user.DisplayName,
                    AreStoragesInColumns = reportSettings.AreStoragesInColumns,

                    ShowStorageSummaryTable = ValidationUtils.TryGetBool(settings.ShowStorageTable),
                    ShowAccountOrganizationSummaryTable = ValidationUtils.TryGetBool(settings.ShowAccountOrganizationTable),
                    ShowClientSummaryTable = ValidationUtils.TryGetBool(settings.ShowClientTable),
                    ShowClientOrganizationSummaryTable = ValidationUtils.TryGetBool(settings.ShowClientOrganizationTable),
                    ShowArticleGroupSummaryTable = ValidationUtils.TryGetBool(settings.ShowArticleGroupTable),
                    ShowTeamSummaryTable = ValidationUtils.TryGetBool(settings.ShowTeamTable),
                    ShowUserSummaryTable = ValidationUtils.TryGetBool(settings.ShowUserTable),
                    ShowContractorSummaryTable = ValidationUtils.TryGetBool(settings.ShowProviderAndProducerTable),
                    ShowDetailsTable = ValidationUtils.TryGetBool(settings.ShowDetailsTable),
                    ShowShortDetailsTable = ValidationUtils.TryGetBool(settings.ShowShortDetailsTable)
                };

                #region Проверки

                ValidationUtils.Assert(!(String.IsNullOrEmpty(settings.StorageIDs) && settings.AllStorages != "1"),
                    "Не выбрано ни одного места хранения.");

                if (settings.CreateByArticleGroup == "1")
                {
                    ValidationUtils.Assert(!String.IsNullOrEmpty(settings.ArticleGroupsIDs) || settings.AllArticleGroups == "1", "Не выбрано ни одной группы товаров.");
                }
                else
                {
                    ValidationUtils.Assert(!String.IsNullOrEmpty(settings.ArticlesIDs), "Не выбрано ни одного товара.");

                    var articleIDs = settings.ArticlesIDs.Split('_').Select(x => ValidationUtils.TryGetInt(x)).Distinct();
                    ValidationUtils.Assert(articleIDs.Count() <= 100, "Максимально допустимое количество товаров равно 100.");
                }

                ValidationUtils.Assert(!(String.IsNullOrEmpty(settings.ClientsIDs) && settings.AllClients != "1"),
                    "Не выбрано ни одного клиента.");
                ValidationUtils.Assert(!(String.IsNullOrEmpty(settings.UsersIDs) && settings.AllUsers != "1"),
                    "Не выбрано ни одного пользователя.");
                ValidationUtils.Assert(!(String.IsNullOrEmpty(settings.AccountOrganizationsIDs) && settings.AllAccountOrganizations != "1"),
                   "Не выбрано ни одной собственной организации.");
                ValidationUtils.Assert(!(settings.WaybillStateId != "0" && settings.WaybillStateId != "1"),
                    "Неверное значение состояния накладной.");

                //Проверяем, чтобы хотя бы одна таблица была видима
                ValidateTablesVisibility(settings);

                //Если не показывать развернутую таблицу, то и "Средние суммы" ,"Разбить на партии" и "Выводить развернутую таблицу в сокращенном виде" должны быть равны "Нет"
                ValidateSettingsDetailsTableConsistency(settings.ShowDetailsTable, settings.DevideByBatch, settings.InAvgPrice,settings.ShowShortDetailsTable);


                #endregion

                #region Обработка группировок

                if (!String.IsNullOrEmpty(settings.GroupByCollectionIDs))
                {
                    foreach (var item in settings.GroupByCollectionIDs.Split('_'))
                    {
                        var result = ValidationUtils.TryGetEnum<GroupingType>(item);

                        if (!(reportSettings.AreStoragesInColumns && result == GroupingType.ByStorage || reportSettings.GroupFields.Contains(result)))
                        {
                            reportSettings.GroupFields.Add(result);
                        }
                    }
                }

                //В рамках существующего механизма просто добавим всегда дефолтно нужную группировку по группе товара
                if (!reportSettings.GroupFields.Contains(GroupingType.ByArticleGroup))
                {
                    reportSettings.GroupFields.Add(GroupingType.ByArticleGroup);
                }

                #endregion

                #region Получение данных

                var flatTable = report0002Repository.GetData(
                    startDate, endDate,
                    ValidationUtils.TryGetBool(settings.WaybillStateId),
                    reportSettings.IsDevideByBatch,
                    reportSettings.AreStoragesInColumns,
                    reportSettings.InAccountingPrice,
                    reportSettings.WithReturnFromClient,
                    !ValidationUtils.TryGetBool(settings.ReturnFromClientType),
                    settings.StorageIDs ?? "", !String.IsNullOrEmpty(settings.AllStorages),
                    settings.CreateByArticleGroup == "1", settings.ArticleGroupsIDs ?? "", 
                    !String.IsNullOrEmpty(settings.AllArticleGroups),settings.ArticlesIDs ?? "",
                    settings.ClientsIDs ?? "", !String.IsNullOrEmpty(settings.AllClients),
                    settings.UsersIDs ?? "", !String.IsNullOrEmpty(settings.AllUsers),
                    settings.AccountOrganizationsIDs ?? "", !String.IsNullOrEmpty(settings.AllAccountOrganizations),
                    user.Id);

                // Производим инициализацию разрешений на просмотр
                InitViewPermission(reportSettings, flatTable.Select(x => x.StorageId).Distinct(), user);

                #endregion

                #region Заполнение таблиц

                if (model.ShowAccountOrganizationSummaryTable)
                {
                    FillUpAccountOrganizationSummaryTable(model.AccountOrganizationSummaryTable, flatTable, reportSettings);
                }
                if (model.ShowArticleGroupSummaryTable)
                {
                    FillUpArticleGroupSummaryTable(model.ArticleGroupSummaryTable, flatTable, reportSettings);
                }
                if (model.ShowClientSummaryTable)
                {
                    FillUpClientSummaryTable(model.ClientSummaryTable, flatTable, reportSettings);
                }
                if (model.ShowClientOrganizationSummaryTable)
                {
                    FillUpClientOrganizationSummaryTable(model.ClientOrganizationSummaryTable, flatTable, reportSettings);
                }
                if (model.ShowStorageSummaryTable)
                {
                    FillUpStoragesSummaryTable(model.StorageSummaryTable, flatTable, reportSettings);
                }
                if (model.ShowTeamSummaryTable)
                {
                    FillUpTeamSummaryTable(model.TeamSummaryTable, flatTable, reportSettings);
                }
                if (model.ShowUserSummaryTable)
                {
                    FillUpUserSummaryTable(model.UserSummaryTable, flatTable, reportSettings);
                }
                if (model.ShowContractorSummaryTable)
                {
                    FillUpContractorSummaryTable(model.ContractorSummaryTable, flatTable, reportSettings);
                }
                if (model.ShowDetailsTable)
                {
                    FillUpDetailsTable(model.DetailsTable, flatTable, reportSettings);
                }

                #endregion

                return model;
            //}
        }

        /// <summary>
        /// Иницализация разрешений на просмотр
        /// </summary>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="storageIdList">Список Мх отчета</param>
        /// <param name="user">Пользователь, строящий отчет</param>
        private void InitViewPermission(ReportSettings settings, IEnumerable<short> storageIdList, User user)
        {
            // разрешение на просмотр ЗЦ везде
            var allowToViewPurchaseCostEverywhere = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);
            if (!allowToViewPurchaseCostEverywhere)
            {
                settings.CalculateMarkup = false;
                settings.InPurchaseCost = false;
            }

            // Цикл прохода по всем МХ
            foreach (var storageId in storageIdList)
            {
                var obj = settings.ViewPermissions[storageId];
                obj.AllowToViewPurchaseCost = allowToViewPurchaseCostEverywhere;
            }
        }

        #region Формирование сводных таблиц

        /// <summary>
        /// Заполнение модели сводной таблицы
        /// </summary>
        /// <param name="model">Модель сводной таблицы</param>
        /// <param name="flatTable">Плоская таблица данных отчета</param>
        /// <param name="getGroupKey">Лямбда для получения ключа группировки</param>
        /// <param name="getGroupSort">Лямбда для сортировки групп</param>
        /// <param name="getGroupHeader">Лямбда для получения заголовка</param>
        /// <param name="settings">Настройки отчета</param>
        private void FillUpSummaryTableModel<TValue>(Report0002_SummaryTableViewModel model, IEnumerable<Report0002RowDataModel> flatTable,
            Func<Report0002RowDataModel, TValue> getGroupKey,
            Func<IEnumerable<IGrouping<TValue, Report0002RowDataModel>>, IOrderedEnumerable<IGrouping<TValue, Report0002RowDataModel>>> getGroupSort,
            Func<Report0002RowDataModel, string> getGroupHeader, ReportSettings settings)
        {
            decimal totalResultAccountingPriceSum, totalReturnsAccountingPriceSum, totalResultPurchasePriceSum, totalResultSalePriceSum,
                totalReturnsSalePriceSum, totalMarkupSum, totalReturnsPurchaseCostSum, totalExpenditureAccountingPriceSum,
                totalExpenditurePurchasePriceSum, totalExpenditureSalePriceSum, totalSoldCount;

            totalResultAccountingPriceSum = totalReturnsAccountingPriceSum = totalResultPurchasePriceSum = totalResultSalePriceSum =
                totalReturnsSalePriceSum = totalMarkupSum = totalReturnsPurchaseCostSum = totalExpenditureAccountingPriceSum =
                totalExpenditurePurchasePriceSum = totalExpenditureSalePriceSum = totalSoldCount = 0;

            foreach (var group in getGroupSort(flatTable.GroupBy(getGroupKey)))
            {
                var groupWithoutReturns = group.Where(x => !x.IsReturn).ToList();
                var groupWithoutExpenditures = group.Where(x => x.IsReturn).ToList();

                // Суммы возвратов без реализаций
                decimal? returnAccountingPriceSum = null, returnPurchasePriceSum = null, returnSalePriceSum = 0;

                var row = new Report0002_SummaryTableItemViewModel();

                row.Name = getGroupHeader(group.First());

                // Суммы реализаций без возвратов
                decimal? expenditureAccountingPriceSum = null;
                if (settings.InAccountingPrice)
                {
                    expenditureAccountingPriceSum = groupWithoutReturns.Sum(x => x.AccountingPriceSum);
                }
                decimal? expenditurePurchasePriceSum = null;
                if (settings.InPurchaseCost || settings.CalculateMarkup)
                {
                    expenditurePurchasePriceSum = groupWithoutReturns.Sum(x => x.PurchaseCostSum * (
                        settings.ViewPermissions[x.StorageId].AllowToViewPurchaseCost ? 1 : 0));    // Обнуляем суммы, на которые нет прав
                }
                decimal? expenditureSalePriceSum = (settings.InSalePrice || settings.CalculateMarkup) ? groupWithoutReturns.Sum(x => x.SalePriceSum) : (decimal?)null;

                // Суммы возвратов без реализаций
                if (settings.InAccountingPrice)
                {
                    // Если возвраты учитываются, то сумма в УЦ должна быть отлична от null
                    returnAccountingPriceSum = groupWithoutExpenditures.Sum(x => x.AccountingPriceSum );
                }
                if (settings.InPurchaseCost || settings.CalculateMarkup)
                {
                    returnPurchasePriceSum = groupWithoutExpenditures.Sum(x => x.PurchaseCostSum * (
                        settings.ViewPermissions[x.StorageId].AllowToViewPurchaseCost ? 1 : 0));    // Обнуляем суммы, на которые нет прав
                }
                returnSalePriceSum = (settings.InSalePrice || settings.CalculateMarkup) ? groupWithoutExpenditures.Sum(x => x.SalePriceSum) : (decimal?)null;

                var resultAccountingPriceSum = expenditureAccountingPriceSum - returnAccountingPriceSum;

                if (settings.InAccountingPrice)
                {
                    row.ExpenditureAccountingPriceSum = expenditureAccountingPriceSum.ForDisplay(ValueDisplayType.Money);
                    row.ResultAccountingPriceSum = resultAccountingPriceSum.ForDisplay(ValueDisplayType.Money);
                    row.ReturnsAccountingPriceSum = returnAccountingPriceSum.ForDisplay(ValueDisplayType.Money);
                }

                totalExpenditureAccountingPriceSum += expenditureAccountingPriceSum ?? 0;
                totalResultAccountingPriceSum += resultAccountingPriceSum ?? 0;
                totalReturnsAccountingPriceSum += returnAccountingPriceSum ?? 0;

                var resultPurchasePriceSum = expenditurePurchasePriceSum - returnPurchasePriceSum;

                if (settings.InPurchaseCost)
                {
                    row.ExpenditurePurchasePriceSum = expenditurePurchasePriceSum.ForDisplay(ValueDisplayType.Money);
                    row.ResultPurchasePriceSum = resultPurchasePriceSum.ForDisplay(ValueDisplayType.Money);
                    row.ReturnsPurchaseCostSum = returnPurchasePriceSum.ForDisplay(ValueDisplayType.Money);

                }

                totalExpenditurePurchasePriceSum += expenditurePurchasePriceSum ?? 0;
                totalResultPurchasePriceSum += resultPurchasePriceSum ?? 0;
                totalReturnsPurchaseCostSum += returnPurchasePriceSum ?? 0;

                var resultSalePriceSum = expenditureSalePriceSum - returnSalePriceSum;
                if (settings.InSalePrice)
                {
                    row.ExpenditureSalePriceSum = expenditureSalePriceSum.ForDisplay(ValueDisplayType.Money);
                    row.ResultSalePriceSum = resultSalePriceSum.ForDisplay(ValueDisplayType.Money);
                    row.ReturnsSalePriceSum = returnSalePriceSum.ForDisplay(ValueDisplayType.Money);
                }

                totalExpenditureSalePriceSum += expenditureSalePriceSum ?? 0;
                totalResultSalePriceSum += resultSalePriceSum ?? 0;
                totalReturnsSalePriceSum += returnSalePriceSum ?? 0;

                if (settings.CalculateMarkup)
                {
                    var markupSum = (resultSalePriceSum - resultPurchasePriceSum);
                    var markupPercentage = (resultPurchasePriceSum.HasValue && resultPurchasePriceSum != 0) ? 
                        (markupSum / resultPurchasePriceSum) * 100 : (decimal?)null;
                    
                    row.MarkupSum = markupSum.ForDisplay(ValueDisplayType.Money);
                    row.MarkupPercentage = markupPercentage.ForDisplay(ValueDisplayType.Money);

                    totalMarkupSum += markupSum ?? 0;
                }

                decimal? soldCount = (settings.ShowSoldArticleCount) ? groupWithoutReturns.Sum(x => x.Count) : (decimal?)null;

                if (settings.ShowSoldArticleCount)
                {
                    row.SoldArticleCount = soldCount.ForDisplay(ValueDisplayType.Default);
                }

                totalSoldCount += soldCount ?? 0;

                model.Items.Add(row);
            }

            model.MarkupTotal = totalMarkupSum.ForDisplay(ValueDisplayType.Money);
            model.ResultTotalSumInAccountPrice = totalResultAccountingPriceSum.ForDisplay(ValueDisplayType.Money);
            model.ResultTotalSumInPurchasePrice = totalResultPurchasePriceSum.ForDisplay(ValueDisplayType.Money);
            model.ResultTotalSumInSalePrice = totalResultSalePriceSum.ForDisplay(ValueDisplayType.Money);
            model.ReturnsTotalSumInAccountPrice = totalReturnsAccountingPriceSum.ForDisplay(ValueDisplayType.Money);
            model.ReturnsTotalSumInPurchasePrice = totalReturnsPurchaseCostSum.ForDisplay(ValueDisplayType.Money);
            model.ReturnsTotalSumInSalePrice = totalReturnsSalePriceSum.ForDisplay(ValueDisplayType.Money);
            model.ExpenditureTotalSumInAccountPrice = totalExpenditureAccountingPriceSum.ForDisplay(ValueDisplayType.Money);
            model.ExpenditureTotalSumInPurchasePrice = totalExpenditurePurchasePriceSum.ForDisplay(ValueDisplayType.Money);
            model.ExpenditureTotalSumInSalePrice = totalExpenditureSalePriceSum.ForDisplay(ValueDisplayType.Money);
            model.SoldArticleCount = totalSoldCount.ForDisplay(ValueDisplayType.Default);

            model.InAccountingPrice = settings.InAccountingPrice;
            model.InPurchaseCost = settings.InPurchaseCost;
            model.InSalePrice = settings.InSalePrice;
            model.UseReturns = settings.WithReturnFromClient;
            model.CalculateMarkup = settings.CalculateMarkup;
            model.ShowSoldArticleCount = settings.ShowSoldArticleCount;
        }

        /// <summary>
        /// Получение сводной таблицы по МХ
        /// </summary>
        /// <param name="model">Модель отчета</param>
        /// <param name="flatTable">Плоская таблица данных отчета</param>
        /// <param name="settings">Настройки отчета</param>
        private void FillUpStoragesSummaryTable(Report0002_SummaryTableViewModel model, IEnumerable<Report0002RowDataModel> flatTable, ReportSettings settings)
        {
            model.Name = "Место хранения";
            FillUpSummaryTableModel(model, flatTable,
                x => x.StorageId,
                x => x.OrderBy(y => y.First().StorageTypeId).ThenBy(y => y.First().StorageName),
                x => x.StorageName,
                settings);
        }

        /// <summary>
        /// Получение сводной таблицы по организациям аккаунта
        /// </summary>
        /// <param name="model">Модель отчета</param>
        /// <param name="flatTable">Плоская таблица данных отчета</param>
        /// <param name="settings">Настройки отчета</param>
        private void FillUpAccountOrganizationSummaryTable(Report0002_SummaryTableViewModel model, IEnumerable<Report0002RowDataModel> flatTable, ReportSettings settings)
        {
            model.Name = "Организация";
            FillUpSummaryTableModel(model, flatTable,
                 x => x.AccountOrganizationId,
                 x => x.OrderBy(y => y.First().AccountOrganizationName),
                 x => x.AccountOrganizationName,
                 settings);
        }

        /// <summary>
        /// Получение сводной таблицы по клиентам
        /// </summary>
        /// <param name="model">Модель отчета</param>
        /// <param name="flatTable">Плоская таблица данных отчета</param>
        /// <param name="settings">Настройки отчета</param>
        private void FillUpClientSummaryTable(Report0002_SummaryTableViewModel model, IEnumerable<Report0002RowDataModel> flatTable, ReportSettings settings)
        {
            model.Name = "Клиент";
            FillUpSummaryTableModel(model, flatTable,
                 x => x.ClientId,
                 x => x.OrderBy(y => y.First().ClientName),
                 x => x.ClientName,
                 settings);
        }

        /// <summary>
        /// Получение сводной таблицы по организациям клиента
        /// </summary>
        /// <param name="model">Модель отчета</param>
        /// <param name="flatTable">Плоская таблица данных отчета</param>
        /// <param name="settings">Настройки отчета</param>
        private void FillUpClientOrganizationSummaryTable(Report0002_SummaryTableViewModel model, IEnumerable<Report0002RowDataModel> flatTable, ReportSettings settings)
        {
            model.Name = "Организация клиента";
            FillUpSummaryTableModel(model, flatTable,
                 x => x.ClientOrganizationId,
                 x => x.OrderBy(y => y.First().ClientOrganizationName),
                 x => x.ClientOrganizationName,
                 settings);
        }

        /// <summary>
        /// Получение сводной таблицы по группам товаров
        /// </summary>
        /// <param name="model">Модель отчета</param>
        /// <param name="flatTable">Плоская таблица данных отчета</param>
        /// <param name="settings">Настройки отчета</param>
        private void FillUpArticleGroupSummaryTable(Report0002_SummaryTableViewModel model, IEnumerable<Report0002RowDataModel> flatTable, ReportSettings settings)
        {
            model.Name = "Группа товаров";
            FillUpSummaryTableModel(model, flatTable,
                 x => x.ArticleGroupId,
                 x => x.OrderBy(y => y.First().ArticleGroupName),
                 x => x.ArticleGroupName,
                 settings);
        }

        /// <summary>
        /// Получение сводной таблицы по командам
        /// </summary>
        /// <param name="model">Модель отчета</param>
        /// <param name="flatTable">Плоская таблица данных отчета</param>
        /// <param name="settings">Настройки отчета</param>
        private void FillUpTeamSummaryTable(Report0002_SummaryTableViewModel model, IEnumerable<Report0002RowDataModel> flatTable, ReportSettings settings)
        {
            model.Name = "Команда";
            FillUpSummaryTableModel(model, flatTable,
                 x => x.TeamId,
                 x => x.OrderBy(y => y.First().TeamName),
                 x => x.TeamName,
                 settings);
        }

        /// <summary>
        /// Получение сводной таблицы по пользователеям
        /// </summary>
        /// <param name="model">Модель отчета</param>
        /// <param name="flatTable">Плоская таблица данных отчета</param>
        /// <param name="settings">Настройки отчета</param>
        private void FillUpUserSummaryTable(Report0002_SummaryTableViewModel model, IEnumerable<Report0002RowDataModel> flatTable, ReportSettings settings)
        {
            model.Name = "Пользователь";
            FillUpSummaryTableModel(model, flatTable,
                x => x.UserId,
                x => x.OrderBy(y => y.First().UserName),
                x => x.UserName,
                settings);
        }

        /// <summary>
        /// Получение сводной таблицы по поставщикам и производителям
        /// </summary>
        /// <param name="model">Модель отчета</param>
        /// <param name="flatTable">Плоская таблица данных отчета</param>
        /// <param name="settings">Настройки отчета</param>
        private void FillUpContractorSummaryTable(Report0002_SummaryTableViewModel model, IEnumerable<Report0002RowDataModel> flatTable, ReportSettings settings)
        {
            model.Name = "Поставщик / производитель";
            FillUpSummaryTableModel(model, flatTable,
                x => x.ProducerId,
                x => x.OrderBy(y => y.First().ProducerName),
                x => x.ProducerName,
                settings);
        }

        #endregion

        #region Формирование развернутой таблицы

        /// <summary>
        /// Заполнение детализированной таблицы
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="flatTable">Плоская таблица данных отчета</param>
        /// <param name="settings">Настройки отчета</param>
        private void FillUpDetailsTable(Report0002_DetailsTableViewModel model, IEnumerable<Report0002RowDataModel> flatTable,
            ReportSettings settings)
        {
            model.GroupCount = settings.GroupFields.Count;  // Выставляем количество группировок
            model.Storages = flatTable.GroupBy(x => x.StorageId).Select(x => new Report0002_StorageInfoItem
                {
                    Id = x.Key,
                    Name = x.First().StorageName,
                    TypeId = x.First().StorageTypeId
                })
                .OrderBy(x => x.TypeId).ThenBy(x => x.Name).ToList();   // Формируем список МХ отчета

            // группировка строк плоской таблицы и заполнение модели таблицы отчета
            var summaryInfo = GroupDetailsTableRows(model, flatTable, settings, 0);

            // Заполнение строки "Итого"
            model.ExpenditureTotalSumInAccountPriceSum = summaryInfo.SoldArticlesInAccountingPriceSum.ForDisplay(ValueDisplayType.Money);
            model.ExpenditureTotalSumInPurchasePriceSum = summaryInfo.SoldArticlesInPurchaseCostSum.ForDisplay(ValueDisplayType.Money);
            model.ExpenditureTotalSumInSalePriceSum = summaryInfo.SoldArticlesInSalePriceSum.ForDisplay(ValueDisplayType.Money);
            model.ReturnsTotalSumInAccountPriceSum = summaryInfo.ReturnedArticlesInAccountingPriceSum.ForDisplay(ValueDisplayType.Money);
            model.ReturnsTotalSumInPurchasePriceSum = summaryInfo.ReturnArticlesInPurchaseCostSum.ForDisplay(ValueDisplayType.Money);
            model.ReturnsTotalSumInSalePriceSum = summaryInfo.ReturnArticlesInSalePriceSum.ForDisplay(ValueDisplayType.Money);
            model.ResultTotalSumInSalePriceSum = (summaryInfo.SoldArticlesInSalePriceSum - summaryInfo.ReturnArticlesInSalePriceSum).ForDisplay(ValueDisplayType.Money);
            model.TotalMarkup = summaryInfo.MarkupSum.ForDisplay(ValueDisplayType.Money);
            model.TotalSoldCount = summaryInfo.SoldCount.ForDisplay(ValueDisplayType.Default);
            model.TotalReturnedCount = summaryInfo.ReturnedCount.ForDisplay(ValueDisplayType.Default);
           
            // Формируем "Итого" по каждому МХ для группы
            foreach (var item in summaryInfo.SummaryInfoByStorages)
            {
                model.SeparationByStorages[item.Key] = new Report0002_SeparationDetailsTableRowByStoragesViewModel()
                {
                    SalePriceSum = item.Value.SalePriceSum.ForDisplay(ValueDisplayType.Money),
                    PurchaseCostSum = item.Value.PurchaseCostSum.ForDisplay(ValueDisplayType.Money),
                    StoredAccountingPriceSum = item.Value.AccountingPriceSum.ForDisplay(ValueDisplayType.Money),
                    SoldCount = item.Value.SoldCount.ForDisplay(ValueDisplayType.Default),
                    AvailableArticleCount = item.Value.AvailableArticleCount.ForDisplay(ValueDisplayType.Default),
                    ReturnedCount = item.Value.ReturnedCount.ForDisplay(ValueDisplayType.Default)
                };
            }

            // Заполняем признаки отображения столбцов
            model.InAccountingPrice = settings.InAccountingPrice;
            model.InPurchaseCost = settings.InPurchaseCost;
            model.InSalePrice = settings.InSalePrice;
            model.UseReturns = settings.WithReturnFromClient;
            model.CalculateMarkup = settings.CalculateMarkup;
            model.IsDevideByBatch = settings.IsDevideByBatch;
            model.CalculateAveragePrice = settings.CalculateAveragePrice;
            model.ShowAdditionColumns = settings.ShowAdditionColumns;
        }

        /// <summary>
        /// Группировка строк по очередному ключу
        /// </summary>
        /// <param name="model">Модель таблицы</param>
        /// <param name="flatTable">Плоская таблица данных отчета</param>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="groupByFieldIndex">Индекс применяемой группировки</param>
        /// <returns>Информация по "Итого" для группы</returns>
        private ReportGroupRowSummaryInfo GroupDetailsTableRows(Report0002_DetailsTableViewModel model, IEnumerable<Report0002RowDataModel> flatTable,
            ReportSettings settings, int groupByFieldIndex)
        {
            ReportGroupRowSummaryInfo summaryInfo;

            // Если группировки закончились, ...
            if (settings.GroupFields.Count == groupByFieldIndex)
            {
                // ... то заполняем модель
                summaryInfo = FillUpDetailsTableByGroupRow(model, flatTable, settings);
            }
            else
            {
                // Иначе группируем
                switch (settings.GroupFields[groupByFieldIndex])
                {
                    case GroupingType.ByStorage:
                        summaryInfo = GroupDetailsTableRowByField(model, flatTable, settings, groupByFieldIndex,
                            x => x.StorageId, x => x.StorageName);
                        break;
                    case GroupingType.ByAccountOrganization:
                        summaryInfo = GroupDetailsTableRowByField(model, flatTable, settings, groupByFieldIndex,
                            x => x.AccountOrganizationId, x => x.AccountOrganizationName);
                        break;
                    case GroupingType.ByTeam:
                        summaryInfo = GroupDetailsTableRowByField(model, flatTable, settings, groupByFieldIndex,
                            x => x.TeamId, x => x.TeamName);
                        break;
                    case GroupingType.ByUser:
                        summaryInfo = GroupDetailsTableRowByField(model, flatTable, settings, groupByFieldIndex,
                            x => x.UserId, x => x.UserName);
                        break;
                    case GroupingType.ByClient:
                        summaryInfo = GroupDetailsTableRowByField(model, flatTable, settings, groupByFieldIndex,
                            x => x.ClientId, x => x.ClientName);
                        break;
                    case GroupingType.ByClientOrganization:
                        summaryInfo = GroupDetailsTableRowByField(model, flatTable, settings, groupByFieldIndex,
                            x => x.ClientOrganizationId, x => x.ClientOrganizationName);
                        break;
                    case GroupingType.ByContractor:
                        summaryInfo = GroupDetailsTableRowByField(model, flatTable, settings, groupByFieldIndex,
                            x => x.ProducerId, x => x.ProducerName);
                        break;
                    case GroupingType.ByArticleGroup:
                        summaryInfo = GroupDetailsTableRowByField(model, flatTable, settings, groupByFieldIndex,
                            x => x.ArticleGroupId, x => x.ArticleGroupName);
                        break;
                    default:
                        throw new Exception("Неизвестный тип группировки.");
                }
            }

            return summaryInfo;
        }

        /// <summary>
        /// Группировка строк по заданному ключу
        /// </summary>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <param name="model">Модель таблицы</param>
        /// <param name="flatTable">Плоская таблица данных отчета</param>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="groupByFieldIndex">Индекс применяемой группировки</param>
        /// <param name="groupKey">Лямбда выражение, получения ключа группировки</param>
        /// <param name="groupHeader">Лямбда выражение получения заголовка группировки</param>
        /// <returns>Информация по "Итого" для группы</returns>
        private ReportGroupRowSummaryInfo GroupDetailsTableRowByField<TKey>(Report0002_DetailsTableViewModel model, IEnumerable<Report0002RowDataModel> flatTable,
            ReportSettings settings, int groupByFieldIndex, Func<Report0002RowDataModel, TKey> groupKey,
            Func<Report0002RowDataModel, string> groupHeader)
        {
            var groupsList = flatTable.GroupBy(groupKey);
            // Список "Итого" по дочерним группам
            var summaryInfoList = new List<ReportGroupRowSummaryInfo>();

            foreach (var group in groupsList.OrderBy(x => groupHeader(x.First())))
            {
                var groupHeaderRow = new Report0002_DetailsTableItemViewModel();

                model.Items.Add(groupHeaderRow);    // т.к. строка заголовка группы должна быть выше ее позиций.
                // Заполняем таблицу позициями группы и получаем "Итого" по ним
                var summaryInfo = GroupDetailsTableRows(model, group, settings, groupByFieldIndex + 1);
                // Заполняем "шапку" группы
                groupHeaderRow.IsHeaderGroup = true;
                groupHeaderRow.HeaderName = settings.GroupFields[groupByFieldIndex].GetDisplayName();
                groupHeaderRow.HeaderValue = groupHeader(group.First());
                groupHeaderRow.HeaderOffset = groupByFieldIndex;
                groupHeaderRow.HeaderPurchaseSum = summaryInfo.SoldArticlesInPurchaseCostSum.ForDisplay(ValueDisplayType.Money);
                groupHeaderRow.HeaderAccountingSum = summaryInfo.SoldArticlesInAccountingPriceSum.ForDisplay(ValueDisplayType.Money);
                groupHeaderRow.HeaderSaleSum = summaryInfo.SoldArticlesInSalePriceSum.ForDisplay(ValueDisplayType.Money);
                groupHeaderRow.HeaderReturnPurchaseSum = summaryInfo.ReturnArticlesInPurchaseCostSum.ForDisplay(ValueDisplayType.Money);
                groupHeaderRow.HeaderReturnAccountingSum = summaryInfo.ReturnedArticlesInAccountingPriceSum.ForDisplay(ValueDisplayType.Money);
                groupHeaderRow.HeaderReturnSaleSum = summaryInfo.ReturnArticlesInSalePriceSum.ForDisplay(ValueDisplayType.Money);
                groupHeaderRow.HeaderSaleWithoutReturnSum = (summaryInfo.SoldArticlesInSalePriceSum - summaryInfo.ReturnArticlesInSalePriceSum).ForDisplay(ValueDisplayType.Money);
                groupHeaderRow.HeaderMarkupSum = summaryInfo.MarkupSum.ForDisplay(ValueDisplayType.Money);
                groupHeaderRow.HeaderSoldCount = summaryInfo.SoldCount.ForDisplay(ValueDisplayType.Default);
                groupHeaderRow.HeaderReturnedCount = summaryInfo.ReturnedCount.ForDisplay(ValueDisplayType.Default);
                // Формируем "Итого" по каждому МХ для группы
                foreach (var item in summaryInfo.SummaryInfoByStorages)
                {
                    groupHeaderRow.SeparationByStorages[item.Key] = new Report0002_SeparationDetailsTableRowByStoragesViewModel()
                    {
                        SalePriceSum = item.Value.SalePriceSum.ForDisplay(ValueDisplayType.Money),
                        PurchaseCostSum = item.Value.PurchaseCostSum.ForDisplay(ValueDisplayType.Money),
                        StoredAccountingPriceSum = item.Value.AccountingPriceSum.ForDisplay(ValueDisplayType.Money),
                        SoldCount = item.Value.SoldCount.ForDisplay(ValueDisplayType.Default),
                        AvailableArticleCount = item.Value.AvailableArticleCount.ForDisplay(ValueDisplayType.Default),
                        ReturnedCount = item.Value.ReturnedCount.ForDisplay(ValueDisplayType.Default)
                    };
                }
                summaryInfoList.Add(summaryInfo);
            }

            return GetSummaryInfoForParentGroup(summaryInfoList);
        }

        /// <summary>
        /// Получение данных "Итого" для родительской группы
        /// </summary>
        /// <param name="infoList">Список данных по дочерним группам</param>
        /// <returns>Информация по "Итого" для родителькой группы</returns>
        private ReportGroupRowSummaryInfo GetSummaryInfoForParentGroup(IEnumerable<ReportGroupRowSummaryInfo> infoList)
        {
            var summaryInfo = new ReportGroupRowSummaryInfo();
            // Цикл агрегации по данным дочерних групп
            foreach (var info in infoList)
            {
                summaryInfo.ReturnedArticlesInAccountingPriceSum += info.ReturnedArticlesInAccountingPriceSum;
                summaryInfo.ReturnArticlesInPurchaseCostSum += info.ReturnArticlesInPurchaseCostSum;
                summaryInfo.ReturnArticlesInSalePriceSum += info.ReturnArticlesInSalePriceSum;
                summaryInfo.SoldArticlesInAccountingPriceSum += info.SoldArticlesInAccountingPriceSum;
                summaryInfo.SoldArticlesInPurchaseCostSum += info.SoldArticlesInPurchaseCostSum;
                summaryInfo.SoldArticlesInSalePriceSum += info.SoldArticlesInSalePriceSum;
                summaryInfo.MarkupSum += info.MarkupSum;
                summaryInfo.SoldCount += info.SoldCount;
                summaryInfo.ReturnedCount += info.ReturnedCount;

                // Агрегируем данные детализаций по МХ
                foreach (var item in info.SummaryInfoByStorages)
                {
                    var value = summaryInfo.SummaryInfoByStorages[item.Key];
                    value.SalePriceSum += item.Value.SalePriceSum;
                    value.PurchaseCostSum = value.PurchaseCostSum + item.Value.PurchaseCostSum;
                    value.AccountingPriceSum = value.AccountingPriceSum + item.Value.AccountingPriceSum;
                    value.SoldCount += item.Value.SoldCount;
                    value.AvailableArticleCount += item.Value.AvailableArticleCount;
                    value.ReturnedCount += item.Value.ReturnedCount;
                }
            }

            return summaryInfo;
        }

        /// <summary>
        /// Заполение детализированной таблицы
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="rows">Строки плоской таблицы данных, принадлежащих одной группе</param>
        /// <param name="settings">Настройки отчета</param>
        /// <returns>Информация по "Итого" для группы</returns>
        private ReportGroupRowSummaryInfo FillUpDetailsTableByGroupRow(Report0002_DetailsTableViewModel model, IEnumerable<Report0002RowDataModel> rows,
            ReportSettings settings)
        {
            var summaryInfoList = new List<ReportGroupRowSummaryInfo>();
            // Нужно ли разделять партии?
            if (settings.IsDevideByBatch)
            {
                //Да, нужно. Группируем по состаному ключу
                var groupRows = rows.GroupBy(x => new { x.ArticleName, Batch = x.BatchId }).OrderBy(x => x.Key.ArticleName).ThenBy(x => x.First().BatchNumber);
                // Вызов заполнения таблицы для каждой группы
                foreach (var group in groupRows.OrderBy(x => x.First().ArticleId).ThenBy(x => x.Key.ArticleName))
                {
                    summaryInfoList.Add(FillUpDetailsTableRow(model, group, settings));
                }
            }
            else
            {
                // Нет, не нужно. Группируем по товарам
                var groupRows = rows.GroupBy(x => x.ArticleName).OrderBy(x => x.Key);
                // Вызов заполнения таблицы для каждой группы
                foreach (var group in groupRows.OrderBy(x => x.First().ArticleId).ThenBy(x => x.Key))
                {
                    summaryInfoList.Add(FillUpDetailsTableRow(model, group, settings));
                }
            }

            return GetSummaryInfoForParentGroup(summaryInfoList);
        }

        /// <summary>
        /// Заполнения строки детализированной таблицы
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="rows">Строки плоской таблицы данных, на основании которых заполняется строка детализированной таблицы</param>
        /// <param name="settings">Настройки отчета</param>
        /// <returns>Информация по "Итого" для группы</returns>
        private ReportGroupRowSummaryInfo FillUpDetailsTableRow(Report0002_DetailsTableViewModel model,
            IEnumerable<Report0002RowDataModel> rows, ReportSettings settings)
        {
            var rowsWithoutReturns = rows.Where(x => !x.IsReturn).ToList();
            var rowsWithoutExpenditures = rows.Where(x => x.IsReturn).ToList();

            // признак наличия невидимых по правам ЗЦ
            var hasDeniedToViewPurchaseCost = rows.Any(x => !settings.ViewPermissions[x.StorageId].AllowToViewPurchaseCost);

            var firstRow = rows.First();
            // Количество товара
            var soldArticleCount = rowsWithoutReturns.Sum(x => x.Count);
            var returnArticlesCount = rowsWithoutExpenditures.Sum(x => x.Count);
            // Сумма товара в ЗЦ
            decimal? soldArticlesInPurchaseCostSum = null, returnArticlesInPurchaseCostSum = null, expenditurePurchaseCostSumWithReturns = null;
            if (!hasDeniedToViewPurchaseCost)
            {
                soldArticlesInPurchaseCostSum = rowsWithoutReturns.Sum(x => x.PurchaseCostSum);
                returnArticlesInPurchaseCostSum = rowsWithoutExpenditures.Sum(x => x.PurchaseCostSum);
                expenditurePurchaseCostSumWithReturns = soldArticlesInPurchaseCostSum - returnArticlesInPurchaseCostSum;    // Сумма реализации в ЗЦ с учетом возвратов
            }
            // Сумма товара в ОЦ
            var soldArticlesInSalePriceSum = rowsWithoutReturns.Sum(x => x.SalePriceSum);
            var returnArticlesInSalePriceSum = rowsWithoutExpenditures.Sum(x => x.SalePriceSum);

            decimal? soldArticlesInAccountingPriceSum = null, returnedArticlesInAccountingPriceSum = null;
            // Сумма в учетных ценах
            soldArticlesInAccountingPriceSum = rowsWithoutReturns.Sum(x => x.AccountingPriceSum);
            returnedArticlesInAccountingPriceSum = rowsWithoutExpenditures.Sum(x => x.AccountingPriceSum);

            var markUpSum = (soldArticlesInSalePriceSum - returnArticlesInSalePriceSum) - (soldArticlesInPurchaseCostSum - returnArticlesInPurchaseCostSum);
            var markUpPercentage = (expenditurePurchaseCostSumWithReturns != null && expenditurePurchaseCostSumWithReturns != 0) ?
                (decimal?)((markUpSum * 100) / Math.Abs(expenditurePurchaseCostSumWithReturns.Value)) : 0;

            var soldArticleCountWithReturns = soldArticleCount - returnArticlesCount;
            decimal? avgPurchaseCostSum = null;
            if (!hasDeniedToViewPurchaseCost)
            {
                avgPurchaseCostSum = soldArticleCount != 0 ? (soldArticlesInPurchaseCostSum / soldArticleCount) : 0;
            }
            var modelRow = new Report0002_DetailsTableItemViewModel()
            {
                AccountingPriceSum = soldArticlesInAccountingPriceSum.ForDisplay(ValueDisplayType.Money),
                ArticleId = firstRow.ArticleId.ToString(),
                ArticleName = firstRow.ArticleName,
                ArticleNumber = firstRow.ArticleNumber,
                AvgPurchaseCostSum = avgPurchaseCostSum.ForDisplay(ValueDisplayType.Money),
                AvgSalePriceSum = soldArticleCount != 0 ?
                    (soldArticlesInSalePriceSum / soldArticleCount).ForDisplay(ValueDisplayType.Money) : "0",
                BatchName = String.Format("№ {0} от {1}", firstRow.BatchNumber, firstRow.BatchDate.ToShortDateString()),
                CustomsDeclarationNumber = firstRow.CustomsDeclarationNumber,
                IsHeaderGroup = false,
                PackSize = firstRow.PackSize.ForDisplay(),
                ProductionCountryName = firstRow.CountryName,
                PurchaseCostSum = soldArticlesInPurchaseCostSum.ForDisplay(ValueDisplayType.Money),
                ReturnAccountingPriceSum = returnedArticlesInAccountingPriceSum.ForDisplay(ValueDisplayType.Money),
                ReturnedCount = returnArticlesCount.ForDisplay(),
                ReturnPurchaseCostSum = returnArticlesInPurchaseCostSum.ForDisplay(),
                ReturnSalePriceSum = returnArticlesInSalePriceSum.ForDisplay(),
                SalePriceSum = soldArticlesInSalePriceSum.ForDisplay(),
                SoldCount = soldArticleCount.ForDisplay(),
                ResultSalePriceSum = (soldArticlesInSalePriceSum - returnArticlesInSalePriceSum).ForDisplay(ValueDisplayType.Money),
                MarkupSum = markUpSum.ForDisplay(ValueDisplayType.Money),
                MarkupPercent = markUpPercentage.ForDisplay(ValueDisplayType.Percent)
            };
            // Формируем данные "Итого" по группе
            var summaryInfo = new ReportGroupRowSummaryInfo()
            {
                SoldArticlesInPurchaseCostSum = soldArticlesInPurchaseCostSum ?? 0,
                ReturnArticlesInPurchaseCostSum = returnArticlesInPurchaseCostSum ?? 0,
                SoldArticlesInSalePriceSum = soldArticlesInSalePriceSum,
                ReturnArticlesInSalePriceSum = returnArticlesInSalePriceSum,
                SoldArticlesInAccountingPriceSum = soldArticlesInAccountingPriceSum ?? 0,
                ReturnedArticlesInAccountingPriceSum = returnedArticlesInAccountingPriceSum ?? 0,
                MarkupSum = markUpSum ?? 0,
                SoldCount = soldArticleCount,
                ReturnedCount = returnArticlesCount
            };

            // Если МХ выводятся в столбцах, то ...
            if (settings.AreStoragesInColumns)
            {
                // ... заполняем дополнительные поля детализации по МХ
                FillUpSeparationByStoragesForGroupRows(modelRow, rows, settings, summaryInfo);
            }

            ///Записывать , если это не сокращенная развернутая таблица  отчета
            if (!settings.ShowShortDetailsTable)
            {
                model.Items.Add(modelRow);
            }

            return summaryInfo;
        }

        /// <summary>
        /// Получение данных распределния реализаций по МХ
        /// </summary>
        /// <param name="model">Модель строки таблицы отчета</param>
        /// <param name="rows">Строки плоской таблицы, агрегируемые в одну позицию таблицы отчета</param>
        /// <param name="settings">Настройк иотчета</param>
        /// <param name="summaryInfo">Информация по "Итого" для группы</param>
        private void FillUpSeparationByStoragesForGroupRows(Report0002_DetailsTableItemViewModel model, IEnumerable<Report0002RowDataModel> rows,
            ReportSettings settings, ReportGroupRowSummaryInfo summaryInfo)
        {
            var saleList = rows.Where(x => !x.IsReturn).ToList();   // Получаем реализации без возвратов

            var returnDictionary = rows.Where(x => x.IsReturn).GroupBy(x => x.StorageId).ToDictionary(x => x.Key);

            // Цикл по всем МХ
            foreach (var group in saleList.GroupBy(x => x.StorageId))
            {
                var salePriceSum = group.Sum(x => x.SalePriceSum);   // Сумма реализации в ОЦ 
                decimal? purchaseCostSum = (settings.ViewPermissions[group.Key].AllowToViewPurchaseCost) ?
                    group.Sum(x => x.PurchaseCostSum) : // Сумма реализаций в ЗЦ
                    (decimal?)null;
                var soldCount = group.Sum(x => x.Count);    // реализованное количество
                
                var firstRow = group.First();
                var availabilityCount = firstRow.ArticleAvailabilityCount;
                var accountingPriceSum = firstRow.ArticleAvailabilityAccountingPrice * availabilityCount;
                var storedAccountingPrice = firstRow.ArticleAvailabilityAccountingPrice;

                var returnCount = returnDictionary.Where(x => x.Key == group.Key).Select(x => x.Value.Sum(s => s.Count)).Sum(); 

                model.SeparationByStorages.Add(group.Key, new Report0002_SeparationDetailsTableRowByStoragesViewModel()
                {
                    AvailableArticleCount = availabilityCount.ForDisplay(),
                    PurchaseCostSum = purchaseCostSum.ForDisplay(ValueDisplayType.Money),
                    SalePriceSum = (salePriceSum >= 0 ? salePriceSum : 0).ForDisplay(ValueDisplayType.Money),
                    SoldCount = soldCount.ForDisplay(),
                    StoredAccountingPrice = storedAccountingPrice.ForDisplay(),
                    StoredAccountingPriceSum = accountingPriceSum.ForDisplay(ValueDisplayType.Money),
                    ReturnedCount = returnCount.ForDisplay()
                });
                // Подстчитваем "Итого" по МХ для группы
                var value = summaryInfo.SummaryInfoByStorages[group.Key];
                value.SalePriceSum = salePriceSum;
                value.AccountingPriceSum = accountingPriceSum ?? 0;
                value.PurchaseCostSum = purchaseCostSum ?? 0;
                value.SoldCount = soldCount;
                value.AvailableArticleCount = availabilityCount ?? 0;
                value.ReturnedCount = returnCount;
            }
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Проверяем чтобы хотя бы одна таблица была видима
        /// </summary>
        private void ValidateTablesVisibility(Report0002SettingsViewModel settings)
        {
            if (!ValidationUtils.TryGetBool(settings.ShowAccountOrganizationTable) &&
                !ValidationUtils.TryGetBool(settings.ShowArticleGroupTable) &&
                !ValidationUtils.TryGetBool(settings.ShowClientOrganizationTable) &&
                !ValidationUtils.TryGetBool(settings.ShowClientTable) &&
                !ValidationUtils.TryGetBool(settings.ShowDetailsTable) &&
                !ValidationUtils.TryGetBool(settings.ShowProviderAndProducerTable) &&
                !ValidationUtils.TryGetBool(settings.ShowStorageTable) &&
                !ValidationUtils.TryGetBool(settings.ShowTeamTable) &&
                !ValidationUtils.TryGetBool(settings.ShowUserTable))
            {
                throw new Exception("Не выбрано ни одной таблицы.");
            }
        }

        private void ValidateSettingsDetailsTableConsistency(string showDetailsTable, string devideByBatch, string inAvgPrice, string showShortDetailsTable)
        {
            bool isShowDetailsTable = ValidationUtils.TryGetBool(showDetailsTable);
            bool isDevideByBatch = ValidationUtils.TryGetBool(devideByBatch);
            bool isInAvgPrice = ValidationUtils.TryGetBool(inAvgPrice);
            bool isShowShortDetailsTable = ValidationUtils.TryGetBool(showShortDetailsTable);

            if (!isShowDetailsTable)
            {
                ValidationUtils.Assert(!(isDevideByBatch || isInAvgPrice || isShowShortDetailsTable), "Параметры «Разделить партии товаров»,«Вывод средних сумм» и " +
                    "«Выводить развернутую таблицу в сокращенном виде» могут быть равны «Да» только если параметр «Выводить развернутую таблицу» равен «Да».");
            }
        }

        #endregion

        #endregion

        #region Выгрузка в Excel

        /// <summary>
        /// Выгрузка отчета в Excel
        /// </summary>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="currentUser">Информация о пользователе</param>
        /// <returns>Массив байт файла excel</returns>
        public byte[] Report0002ExportToExcel(Report0002SettingsViewModel settings, UserInfo currentUser)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                ReportSettings reportSettings;  // настройки отчета
                var viewModel = Report0002(settings, currentUser, out reportSettings);
                string reportHeader = "Отчет о реализации товаров \r\nза период с " + settings.StartDate + " по " + settings.EndDate;
                string sign = "Форма: Report0002." + (Int32.Parse(settings.StoragesInColumns) + 1).ToString() + "\r\nАвтор: " + viewModel.CreatedBy + "\r\nСоставлен: " + DateTime.Now.ToString();
                int summaryTableColumnsCount = 0;
                int detailsTableColumnsCount = 0;
                GetColumnCount(viewModel, reportSettings, out summaryTableColumnsCount, out detailsTableColumnsCount);

                if (!reportSettings.AreStoragesInColumns)
                {
                    if (ValidationUtils.TryGetBool(settings.ShowStorageTable))
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по МХ");
                        FillSummaryExcelTable(sheet, summaryTableColumnsCount, viewModel.StorageSummaryTable,
                            sheet.PrintHeader(summaryTableColumnsCount, reportHeader, sign, "Сводная информация по выбранным МХ:", 1));
                    }
                    if (ValidationUtils.TryGetBool(settings.ShowAccountOrganizationTable))
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по собственным организациям");
                        FillSummaryExcelTable(sheet, summaryTableColumnsCount, viewModel.AccountOrganizationSummaryTable,
                            sheet.PrintHeader(summaryTableColumnsCount, reportHeader, sign, "Сводная информация по собственным организациям:", 1));
                    }
                    if (ValidationUtils.TryGetBool(settings.ShowClientTable))
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по клиентам");
                        FillSummaryExcelTable(sheet, summaryTableColumnsCount, viewModel.ClientSummaryTable,
                            sheet.PrintHeader(summaryTableColumnsCount, reportHeader, sign, "Сводная информация по клиентам:", 1));
                    }
                    if (ValidationUtils.TryGetBool(settings.ShowClientOrganizationTable))
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по организациям клиентов");
                        FillSummaryExcelTable(sheet, summaryTableColumnsCount, viewModel.ClientOrganizationSummaryTable,
                            sheet.PrintHeader(summaryTableColumnsCount, reportHeader, sign, "Сводная информация по организациям клиентов:", 1));
                    }
                    if (ValidationUtils.TryGetBool(settings.ShowArticleGroupTable))
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по группам товаров");
                        FillSummaryExcelTable(sheet, summaryTableColumnsCount, viewModel.ArticleGroupSummaryTable,
                            sheet.PrintHeader(summaryTableColumnsCount, reportHeader, sign, "Сводная информация по выбраным группам товаров:", 1));
                    }
                    if (ValidationUtils.TryGetBool(settings.ShowTeamTable))
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по командам");
                        FillSummaryExcelTable(sheet, summaryTableColumnsCount, viewModel.TeamSummaryTable,
                            sheet.PrintHeader(summaryTableColumnsCount, reportHeader, sign, "Сводная информация по командам:", 1));
                    }
                    if (ValidationUtils.TryGetBool(settings.ShowUserTable))
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по пользователям");
                        FillSummaryExcelTable(sheet, summaryTableColumnsCount, viewModel.UserSummaryTable,
                            sheet.PrintHeader(summaryTableColumnsCount, reportHeader, sign, "Сводная информация по пользователям:", 1));
                    }
                    if (ValidationUtils.TryGetBool(settings.ShowProviderAndProducerTable))
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по поставщикам");
                        FillSummaryExcelTable(sheet, summaryTableColumnsCount, viewModel.ContractorSummaryTable,
                            sheet.PrintHeader(summaryTableColumnsCount, reportHeader, sign, "Сводная информация по поставщикам и производителям:", 1));
                    }

                    if (ValidationUtils.TryGetBool(settings.ShowDetailsTable))
                    {
                        if (ValidationUtils.TryGetBool(settings.ShowShortDetailsTable))
                        {
                            ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сокращенная развернутая информация");
                            FillDetailExcelTable(sheet, detailsTableColumnsCount, viewModel.DetailsTable,
                                sheet.PrintHeader(detailsTableColumnsCount, reportHeader, sign, "Сокращенная развернутая информация:", 1));
                        }
                        else
                        {
                            ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Развернутая информация");
                            FillDetailExcelTable(sheet, detailsTableColumnsCount, viewModel.DetailsTable,
                                sheet.PrintHeader(detailsTableColumnsCount, reportHeader, sign, "Развернутая информация:", 1));
                        }
                    }
                }
                else
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Развернутая информация");
                    FillDetail2ExcelTable(sheet, detailsTableColumnsCount, viewModel.DetailsTable,
                        sheet.PrintHeader(detailsTableColumnsCount, reportHeader, sign, "Развернутая информация:", 1));
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
        private int FillSummaryExcelTable(ExcelWorksheet workSheet, int columns, Report0002_SummaryTableViewModel viewModel, int startRow)
        {
            int count = 0;  //Сколько подколонок (УЦ, ЗЦ, ОЦ) используется
            
            if (viewModel.InPurchaseCost) { count++; }
            if (viewModel.InAccountingPrice) { count++; }
            if (viewModel.InSalePrice) { count++; }

            //Указатель на текущую строку
            int currentRow = startRow;
            //Указатель на текущий столбец
            int currentCol = 1;

            #region Шапка таблицы
            //Устанавливаем стиль для всех ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow + 1, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            //Название
            workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue(viewModel.Name);

            
            currentCol = 2;
            if (viewModel.ShowSoldArticleCount)
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Количество");
                currentCol++;
            }

            //Если используются возвраты то выводим столбцы "Сумма реализации" и "сумма возврата"
            if (viewModel.UseReturns)
            {
                if (count > 0)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + count - 1].MergeRange().SetFormattedValue("Сумма реализаций");
                    currentCol += count;
                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + count - 1].MergeRange().SetFormattedValue("Сумма возвратов");
                    currentCol += count;
                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + count - 1].MergeRange().SetFormattedValue("Cумма реализаций \r\nс учетом возвратов").ChangeRangeStyle(textWrap: true);
                    workSheet.Row(currentRow).Height = 29.25;
                }
            }
            else
            {
                if (count > 0)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + count - 1].MergeRange().SetFormattedValue("Сумма реализаций");
                }
            }

            if (count > 0)
            {
               currentCol += count;
            }

            if (viewModel.CalculateMarkup)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 1].MergeRange().SetFormattedValue("Наценка");
            }


            currentCol = 2;
            if (viewModel.ShowSoldArticleCount) { currentCol++; }
            currentRow++;

            if (viewModel.UseReturns)
            {
                if (viewModel.InPurchaseCost)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue("ЗЦ");
                    workSheet.Cells[currentRow, currentCol + count].SetFormattedValue("ЗЦ");
                    currentCol++;
                }
                if (viewModel.InAccountingPrice)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue("УЦ");
                    workSheet.Cells[currentRow, currentCol + count].SetFormattedValue("УЦ");
                    currentCol++;
                }
                if (viewModel.InSalePrice)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue("ОЦ");
                    workSheet.Cells[currentRow, currentCol + count].SetFormattedValue("ОЦ");
                    currentCol++;
                }
                currentCol += count;
            }

            if (viewModel.InPurchaseCost)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("ЗЦ");
                currentCol++;
            }
            if (viewModel.InAccountingPrice)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("УЦ");
                currentCol++;
            }
            if (viewModel.InSalePrice)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("ОЦ");
                currentCol++;
            }

            if (viewModel.CalculateMarkup)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("%");
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("Сумма");
                currentCol++;
            }
            currentCol = 1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки таблицы
            //Заполняем строки таблицы

            //Флаг четности строки
            bool flag = true;

            if (viewModel.Items != null && viewModel.Items.Any())
            {
                #region Строки с данными
                //Заполняем строки
                foreach (var item in viewModel.Items)
                {
                    //Устанавливаем стиль для строки
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableUnEvenRowStyle() : ExcelUtils.GetTableEvenRowStyle());

                    //для первого столбца отдельный стиль
                    workSheet.Cells[currentRow, currentCol].ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, textWrap: true);

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Name);
                    currentCol++;

                    if (viewModel.ShowSoldArticleCount)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.SoldArticleCount,ValueDisplayType.PackCount);
                        currentCol++;
                    }

                    if (viewModel.UseReturns)
                    {
                        if (viewModel.InPurchaseCost)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ExpenditurePurchasePriceSum, ValueDisplayType.Money);
                            workSheet.Cells[currentRow, currentCol + count].SetFormattedValue(item.ReturnsPurchaseCostSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                        if (viewModel.InAccountingPrice)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ExpenditureAccountingPriceSum, ValueDisplayType.Money);
                            workSheet.Cells[currentRow, currentCol + count].SetFormattedValue(item.ReturnsAccountingPriceSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                        if (viewModel.InSalePrice)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ExpenditureSalePriceSum, ValueDisplayType.Money);
                            workSheet.Cells[currentRow, currentCol + count].SetFormattedValue(item.ReturnsSalePriceSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                        currentCol += count;
                    }

                    if (viewModel.InPurchaseCost)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ResultPurchasePriceSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.InAccountingPrice)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ResultAccountingPriceSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.InSalePrice)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ResultSalePriceSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.CalculateMarkup)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.MarkupPercentage, ValueDisplayType.Percent);
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.MarkupSum, ValueDisplayType.Money);
                        currentCol++;
                    }

                    currentCol = 1;
                    currentRow++;
                    flag = !flag;
                }
                #endregion
            }
            else
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle())
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous).MergeRange().SetFormattedValue("Нет данных");
                currentRow++;
            }

            #region Итого
            //Заполняем Итого
            if (viewModel.InPurchaseCost || viewModel.InAccountingPrice || viewModel.InSalePrice || viewModel.ShowSoldArticleCount)
            {
                //Устанавливаем стиль для "Итого"
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

                //Заполняем данные
                workSheet.Cells[currentRow, currentCol].ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);

                workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого:").ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
                currentCol++;

                if (viewModel.ShowSoldArticleCount)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.SoldArticleCount, ValueDisplayType.PackCount);
                    currentCol++;
                }

                if (viewModel.UseReturns)
                {
                    if (viewModel.InPurchaseCost)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ExpenditureTotalSumInPurchasePrice, ValueDisplayType.Money);

                        workSheet.Cells[currentRow, currentCol + count].SetFormattedValue(viewModel.ReturnsTotalSumInPurchasePrice, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.InAccountingPrice)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ExpenditureTotalSumInAccountPrice, ValueDisplayType.Money);
                        workSheet.Cells[currentRow, currentCol + count].SetFormattedValue(viewModel.ReturnsTotalSumInAccountPrice, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.InSalePrice)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ExpenditureTotalSumInSalePrice, ValueDisplayType.Money);
                        workSheet.Cells[currentRow, currentCol + count].SetFormattedValue(viewModel.ReturnsTotalSumInSalePrice, ValueDisplayType.Money);
                        currentCol++;
                    }
                    currentCol += count;
                }

                if (viewModel.InPurchaseCost)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ResultTotalSumInPurchasePrice, ValueDisplayType.Money);
                    currentCol++;
                }
                if (viewModel.InAccountingPrice)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ResultTotalSumInAccountPrice, ValueDisplayType.Money);
                    currentCol++;
                }
                if (viewModel.InSalePrice)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ResultTotalSumInSalePrice, ValueDisplayType.Money);
                    currentCol++;
                }
                if (viewModel.CalculateMarkup)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue("");
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.MarkupTotal, ValueDisplayType.Money);
                    currentCol++;
                }
                currentRow += 2;
            }
            #endregion
            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Column(1).Width = 50;
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        /// <summary>
        /// Формирует развернутую таблицу 
        /// </summary>
        /// <param name="workSheet">Лист Excel </param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные</param>
        /// <param name="startRow">Начальная строка</param>
        private int FillDetailExcelTable(ExcelWorksheet workSheet, int columns, Report0002_DetailsTableViewModel viewModel, int startRow)
        {
            var baseColspan = 3; // Количество колонок которые необходимо объединить чьтобы поместить название группы
            int currentRow = startRow;
            int currentCol = 1;

            #region Подсчет количества базовых колонок
            if (viewModel.IsDevideByBatch)
                baseColspan++;
            
                      
            #endregion

            #region шапка отчета
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow + 1, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 2].MergeRange().SetFormattedValue("Товар");

            workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Код");
            currentCol++;
            workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Артикул");
            currentCol++;
            workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Наименование");
            currentCol++;

            if (viewModel.IsDevideByBatch)
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Партия");
                currentCol++;
            }

            workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Кол-во");
            currentCol++;

            if (viewModel.CalculateAveragePrice)
            {
                if (viewModel.InPurchaseCost)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Средняя ЗЦ");
                    currentCol++;
                }
                if (viewModel.InSalePrice)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Средняя ОЦ");
                    currentCol++;
                }
            }
            if (viewModel.InPurchaseCost)
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Сумма ЗЦ");
                currentCol++;
            }
            if (viewModel.InAccountingPrice)
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Сумма УЦ");
                currentCol++;
            }
            if (viewModel.InSalePrice)
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Сумма ОЦ");
                currentCol++;
            }
            if (viewModel.UseReturns)
            {
                int count = 0;
                if (viewModel.InPurchaseCost) count++;
                if (viewModel.InAccountingPrice) count++;
                if (viewModel.InSalePrice) count += 2;

                workSheet.Cells[currentRow, currentCol, currentRow, currentCol + count].MergeRange().SetFormattedValue("Возвраты");
                workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Кол-во");
                currentCol++;
                if (viewModel.InPurchaseCost)
                {
                    workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Сумма ЗЦ");
                    currentCol++;
                }
                if (viewModel.InAccountingPrice)
                {
                    workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Сумма УЦ");
                    currentCol++;
                }
                if (viewModel.InSalePrice)
                {
                    workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Сумма ОЦ");
                    currentCol++;
                    workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Сумма реализации \r\nс учетом возвратов").ChangeRangeStyle(textWrap: true);
                    workSheet.Column(currentCol).Width = 18;
                    currentCol++;
                }
            }
            if (viewModel.CalculateMarkup)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 1].MergeRange().SetFormattedValue("Наценка");
                workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("%");
                currentCol++;
                workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Сумма");
                currentCol++;
            }
            if (viewModel.ShowAdditionColumns)
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Кол-во в упаковке");
                currentCol++;
                if (viewModel.IsDevideByBatch)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Страна производства");
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("ГТД");
                    currentCol++;
                }
            }

            currentRow += 2;
            currentCol = 1;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            bool flag = false;

            #region Строки таблицы
            foreach (var row in viewModel.Items)
            {
                if (!row.IsHeaderGroup)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableUnEvenRowStyle() : ExcelUtils.GetTableEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol + 1, currentRow, currentCol + 2].ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ArticleId);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ArticleNumber);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ArticleName);
                    currentCol++;

                    if (viewModel.IsDevideByBatch)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.BatchName);
                        currentCol++;
                    }
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SoldCount, ValueDisplayType.PackCount);
                    currentCol++;
                    if (viewModel.CalculateAveragePrice)
                    {
                        if (viewModel.InPurchaseCost)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AvgPurchaseCostSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                        if (viewModel.InSalePrice)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AvgSalePriceSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                    }
                    if (viewModel.InPurchaseCost)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCostSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.InAccountingPrice)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.InSalePrice)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SalePriceSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.UseReturns)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ReturnedCount, ValueDisplayType.PackCount);
                        currentCol++;
                        if (viewModel.InPurchaseCost)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ReturnPurchaseCostSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                        if (viewModel.InAccountingPrice)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ReturnAccountingPriceSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                        if (viewModel.InSalePrice)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ReturnSalePriceSum, ValueDisplayType.Money);
                            currentCol++;
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SalePriceSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                    }
                    if (viewModel.CalculateMarkup)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.MarkupPercent, ValueDisplayType.Percent);
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.MarkupSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.ShowAdditionColumns)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PackSize, ValueDisplayType.PackCount);
                        currentCol++;
                        if (viewModel.IsDevideByBatch)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ProductionCountryName);
                            currentCol++;
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.CustomsDeclarationNumber);
                            currentCol++;
                        }
                    }
                    currentRow++;
                    currentCol = 1;
                }
                else
                {
                    if (row.HeaderOffset == viewModel.GroupCount - 1)
                        workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableArticleSubTotalRowStyle());
                    else
                        workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle());
                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + baseColspan - 1].MergeRange().ChangeRangeStyle(indent: (row.HeaderOffset + 1) * 2
                        , horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderName + ": " + row.HeaderValue);
                    currentCol += baseColspan;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderSoldCount,ValueDisplayType.PackCount);
                    currentCol++;
                    if (viewModel.CalculateAveragePrice)
                    {
                        if (viewModel.InPurchaseCost) currentCol++;
                        if (viewModel.InSalePrice) currentCol++;
                    }
                    if (viewModel.InPurchaseCost)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderPurchaseSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.InAccountingPrice)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderAccountingSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.InSalePrice)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderSaleSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.UseReturns)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderReturnedCount, ValueDisplayType.PackCount);
                        currentCol++;
                        if (viewModel.InPurchaseCost)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderReturnPurchaseSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                        if (viewModel.InAccountingPrice)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderReturnAccountingSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                        if (viewModel.InSalePrice)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderReturnSaleSum, ValueDisplayType.Money);
                            currentCol++;
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderSaleWithoutReturnSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                    }
                    if (viewModel.CalculateMarkup)
                    {
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderMarkupSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    currentRow++;
                    currentCol = 1;
                }
                flag = !flag;
            }
            if (viewModel.Items.Count() == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center)
                    .SetFormattedValue("Нет данных");
                currentRow++;
            }
            #endregion

            #region Итого

            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());
            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + baseColspan - 1].MergeRange();

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого:").ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
            currentCol += baseColspan;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalSoldCount, ValueDisplayType.PackCount);
            currentCol++;
            if (viewModel.CalculateAveragePrice)
            {
                if (viewModel.InPurchaseCost) currentCol++;
                if (viewModel.InSalePrice) currentCol++;
            }
            if (viewModel.InPurchaseCost)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ExpenditureTotalSumInPurchasePriceSum, ValueDisplayType.Money);
                currentCol++;
            }
            if (viewModel.InAccountingPrice)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ExpenditureTotalSumInAccountPriceSum, ValueDisplayType.Money);
                currentCol++;
            }
            if (viewModel.InSalePrice)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ExpenditureTotalSumInSalePriceSum, ValueDisplayType.Money);
                currentCol++;
            }
            if (viewModel.UseReturns)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalReturnedCount, ValueDisplayType.PackCount);
                currentCol++;
                if (viewModel.InPurchaseCost)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ReturnsTotalSumInPurchasePriceSum, ValueDisplayType.Money);
                    currentCol++;
                }
                if (viewModel.InAccountingPrice)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ReturnsTotalSumInAccountPriceSum, ValueDisplayType.Money);
                    currentCol++;
                }
                if (viewModel.InSalePrice)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ReturnsTotalSumInSalePriceSum, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ResultTotalSumInSalePriceSum, ValueDisplayType.Money);
                    currentCol++;
                }
            }
            if (viewModel.CalculateMarkup)
            {
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalMarkup, ValueDisplayType.Money);
                currentCol++;
            }
            currentRow++;
            currentCol = 1;
            
            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));
            currentRow++;

            return currentRow;
        }

        /// <summary>
        /// Формирует развернутую таблицу с МХ в столбцах
        /// </summary>
        /// <param name="workSheet">Лист Excel </param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные</param>
        /// <param name="startRow">Начальная строка</param>
        private int FillDetail2ExcelTable(ExcelWorksheet workSheet, int columns, Report0002_DetailsTableViewModel viewModel, int startRow)
        {
            var baseColspan = 3;
            int currentRow = startRow;
            int currentCol = 1;
            #region Подсчет количества базовых колонок
            if (viewModel.IsDevideByBatch)
                baseColspan++;
            
            #endregion

            #region шапка отчета
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow + 2, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 2].MergeRange().SetFormattedValue("Товар");

            workSheet.Cells[currentRow + 1, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Код");
            currentCol++;
            workSheet.Cells[currentRow + 1, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Артикул");
            currentCol++;
            workSheet.Cells[currentRow + 1, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Наименование");
            currentCol++;

            if (viewModel.IsDevideByBatch)
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Партия");
                currentCol++;
            }

            workSheet.Cells[currentRow, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Кол-во");
            currentCol++;

            if (viewModel.CalculateAveragePrice)
            {
                if (viewModel.InPurchaseCost)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Средняя ЗЦ");
                    currentCol++;
                }
                if (viewModel.InSalePrice)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Средняя ОЦ");
                    currentCol++;
                }
            }
            if (viewModel.InPurchaseCost)
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Сумма ЗЦ");
                currentCol++;
            }
            if (viewModel.InAccountingPrice)
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Сумма УЦ");
                currentCol++;
            }
            if (viewModel.InSalePrice)
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Сумма ОЦ");
                currentCol++;
            }
            if (viewModel.ShowAdditionColumns)
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Кол-во в упаковке");
                currentCol++;
                if (viewModel.IsDevideByBatch)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Страна производства");
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("ГТД");
                    currentCol++;
                }
            }
            if (viewModel.UseReturns)
            {
                int count = 0;
                if (viewModel.InPurchaseCost) count++;
                if (viewModel.InAccountingPrice) count++;
                if (viewModel.InSalePrice) count += 2;

                workSheet.Cells[currentRow, currentCol, currentRow, currentCol + count].MergeRange().SetFormattedValue("Возвраты");
                workSheet.Cells[currentRow + 1, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Кол-во");
                currentCol++;
                if (viewModel.InPurchaseCost)
                {
                    workSheet.Cells[currentRow + 1, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Сумма ЗЦ");
                    currentCol++;
                }
                if (viewModel.InAccountingPrice)
                {
                    workSheet.Cells[currentRow + 1, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Сумма УЦ");
                    currentCol++;
                }
                if (viewModel.InSalePrice)
                {
                    workSheet.Cells[currentRow + 1, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Сумма ОЦ");
                    currentCol++;
                    workSheet.Cells[currentRow + 1, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Сумма реализации \r\nс учетом возвратов").ChangeRangeStyle(textWrap: true);
                    currentCol++;
                }
            }
            if (viewModel.CalculateMarkup)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 1].MergeRange().SetFormattedValue("Наценка");
                workSheet.Cells[currentRow + 1, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("%");
                currentCol++;
                workSheet.Cells[currentRow + 1, currentCol, currentRow + 2, currentCol].MergeRange().SetFormattedValue("Сумма");
                currentCol++;
            }

            foreach (var storage in viewModel.Storages)
            {
                int count1 = 0;
                if (viewModel.InSalePrice) count1++;
                if (viewModel.InPurchaseCost) count1++;
                
                int count2 = 1;
                if (viewModel.InAccountingPrice) count2 += 2;

                workSheet.Cells[currentRow, currentCol, currentRow, currentCol + count1 + count2].MergeRange().SetFormattedValue(storage.Name);
                workSheet.Cells[currentRow + 1, currentCol, currentRow + 1, currentCol + count1].MergeRange().SetFormattedValue("Реализация");
                workSheet.Cells[currentRow + 1, currentCol + count1 + 1, currentRow + 1, currentCol + count1 + count2].MergeRange().SetFormattedValue("Остаток");
                workSheet.Cells[currentRow + 2, currentCol].SetFormattedValue("Кол-во");
                currentCol++;
                if (viewModel.InSalePrice)
                {
                    workSheet.Cells[currentRow + 2, currentCol].SetFormattedValue("Сумма ОЦ");
                    currentCol++;
                }
                if (viewModel.InPurchaseCost)
                {
                    workSheet.Cells[currentRow + 2, currentCol].SetFormattedValue("Сумма ЗЦ");
                    currentCol++;
                }
                workSheet.Cells[currentRow + 2, currentCol].SetFormattedValue("Кол-во");
                currentCol++;
                if (viewModel.InAccountingPrice)
                {
                    workSheet.Cells[currentRow + 2, currentCol].SetFormattedValue("УЦ");
                    currentCol++;
                    workSheet.Cells[currentRow + 2, currentCol].SetFormattedValue("Сумма УЦ");
                    currentCol++;
                }
            }

            currentRow += 3;
            currentCol = 1;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            bool flag = false;

            #region Строки таблицы
            foreach (var row in viewModel.Items)
            {
                if (!row.IsHeaderGroup)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableUnEvenRowStyle() : ExcelUtils.GetTableEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol + 1, currentRow, currentCol + 2].ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ArticleId);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ArticleNumber);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ArticleName);
                    currentCol++;

                    if (viewModel.IsDevideByBatch)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.BatchName);
                        currentCol++;
                    }
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SoldCount, ValueDisplayType.PackCount);
                    currentCol++;
                    if (viewModel.CalculateAveragePrice)
                    {
                        if (viewModel.InPurchaseCost)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AvgPurchaseCostSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                        if (viewModel.InSalePrice)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AvgSalePriceSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                    }
                    if (viewModel.InPurchaseCost)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCostSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.InAccountingPrice)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.InSalePrice)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SalePriceSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.ShowAdditionColumns)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PackSize, ValueDisplayType.PackCount);
                        currentCol++;
                        if (viewModel.IsDevideByBatch)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ProductionCountryName);
                            currentCol++;
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.CustomsDeclarationNumber);
                            currentCol++;
                        }
                    }
                    if (viewModel.UseReturns)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ReturnedCount, ValueDisplayType.PackCount);
                        currentCol++;
                        if (viewModel.InPurchaseCost)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ReturnPurchaseCostSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                        if (viewModel.InAccountingPrice)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ReturnAccountingPriceSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                        if (viewModel.InSalePrice)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ReturnSalePriceSum, ValueDisplayType.Money);
                            currentCol++;
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SalePriceSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                    }
                    if (viewModel.CalculateMarkup)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.MarkupPercent, ValueDisplayType.Percent);
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.MarkupSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    foreach (var storage in viewModel.Storages)
                    {
                        if (row.SeparationByStorages.ContainsKey(storage.Id))
                        {
                            var subitem = row.SeparationByStorages[storage.Id];
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.SoldCount, ValueDisplayType.PackCount);
                            currentCol++;
                            if (viewModel.InSalePrice)
                            {
                                workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.SalePriceSum, ValueDisplayType.Money);
                                currentCol++;
                            }
                            if (viewModel.InPurchaseCost)
                            {
                                workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.PurchaseCostSum, ValueDisplayType.Money);
                                currentCol++;
                            }
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.AvailableArticleCount, ValueDisplayType.PackCount);
                            currentCol++;
                            if (viewModel.InAccountingPrice)
                            {
                                workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.StoredAccountingPrice, ValueDisplayType.Money);
                                currentCol++;
                                workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.StoredAccountingPriceSum, ValueDisplayType.Money);
                                currentCol++;
                            }
                        }
                        else
                        {
                            currentCol += 2;
                            if (viewModel.InSalePrice) currentCol++;
                            if (viewModel.InPurchaseCost) currentCol++;
                            if (viewModel.InAccountingPrice) currentCol += 2;
                        }
                    }
                    currentRow++;
                    currentCol = 1;
                }
                else
                {
                    if (row.HeaderOffset == viewModel.GroupCount - 1)
                        workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableArticleSubTotalRowStyle());
                    else
                        workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle());

                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + baseColspan - 1].MergeRange().ChangeRangeStyle(indent: (row.HeaderOffset + 1) * 3, 
                        horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderName + ": " + row.HeaderValue);
                    currentCol += baseColspan;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderSoldCount,ValueDisplayType.PackCount);
                    currentCol++;
                    if (viewModel.CalculateAveragePrice)
                    {
                        if (viewModel.InPurchaseCost) currentCol++;
                        if (viewModel.InSalePrice) currentCol++;
                    }
                    if (viewModel.InPurchaseCost)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderPurchaseSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.InAccountingPrice)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderAccountingSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.InSalePrice)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderSaleSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.ShowAdditionColumns)
                    {
                        currentCol++;
                        if (viewModel.IsDevideByBatch)
                        {
                            currentCol++;
                            currentCol++;
                        }
                    }
                    if (viewModel.UseReturns)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderReturnedCount, ValueDisplayType.PackCount);
                        currentCol++;
                        if (viewModel.InPurchaseCost)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderReturnPurchaseSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                        if (viewModel.InAccountingPrice)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderReturnAccountingSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                        if (viewModel.InSalePrice)
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderReturnSaleSum, ValueDisplayType.Money);
                            currentCol++;
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderSaleWithoutReturnSum, ValueDisplayType.Money);
                            currentCol++;
                        }
                    }
                    if (viewModel.CalculateMarkup)
                    {
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.HeaderMarkupSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    foreach (var storage in viewModel.Storages)
                    {
                        if (row.SeparationByStorages.ContainsKey(storage.Id))
                        {
                            var subitem = row.SeparationByStorages[storage.Id];

                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.SoldCount, ValueDisplayType.PackCount);
                            currentCol++;
                            if (viewModel.InSalePrice)
                            {
                                workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.SalePriceSum, ValueDisplayType.Money);
                                currentCol++;
                            }
                            if (viewModel.InPurchaseCost)
                            {
                                workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.PurchaseCostSum, ValueDisplayType.Money);
                                currentCol++;
                            }
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.AvailableArticleCount, ValueDisplayType.PackCount);
                            currentCol++;
                            if (viewModel.InAccountingPrice)
                            {
                                currentCol++;
                                workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.StoredAccountingPriceSum, ValueDisplayType.Money);
                                currentCol++;
                            }
                        }
                        else
                        {
                            currentCol++;
                            if (viewModel.InSalePrice)
                            {
                                workSheet.Cells[currentRow, currentCol].SetFormattedValue("");
                                currentCol++;
                            }
                            if (viewModel.InPurchaseCost)
                            {
                                workSheet.Cells[currentRow, currentCol].SetFormattedValue("");
                                currentCol++;
                            }
                            currentCol++;
                            if (viewModel.InAccountingPrice)
                            {
                                currentCol++;
                                workSheet.Cells[currentRow, currentCol].SetFormattedValue("");
                                currentCol++;
                            }
                        }
                    }
                    currentRow++;
                    currentCol = 1;
                }
                flag = !flag;
            }
            if (viewModel.Items.Count() == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center)
                    .SetFormattedValue("Нет данных");
                currentRow++;
            }
            #endregion

            #region Итого
            
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());
            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + baseColspan - 1].MergeRange();

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого:").ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
            currentCol += baseColspan;

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalSoldCount, ValueDisplayType.PackCount);
            currentCol++;
            if (viewModel.CalculateAveragePrice)
            {
                if (viewModel.InPurchaseCost)
                {
                    currentCol++;
                }
                if (viewModel.InSalePrice)
                {
                    currentCol++;
                }
            }
            if (viewModel.InPurchaseCost)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ExpenditureTotalSumInPurchasePriceSum, ValueDisplayType.Money);
                currentCol++;
            }
            if (viewModel.InAccountingPrice)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ExpenditureTotalSumInAccountPriceSum, ValueDisplayType.Money);
                currentCol++;
            }
            if (viewModel.InSalePrice)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ExpenditureTotalSumInSalePriceSum, ValueDisplayType.Money);
                currentCol++;
            }
            if (viewModel.ShowAdditionColumns)
            {
                currentCol++;
                if (viewModel.IsDevideByBatch)
                {
                    currentCol++;
                    currentCol++;
                }
            }
            if (viewModel.UseReturns)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalReturnedCount, ValueDisplayType.PackCount);
                currentCol++;
                if (viewModel.InPurchaseCost)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ReturnsTotalSumInPurchasePriceSum, ValueDisplayType.Money);
                    currentCol++;
                }
                if (viewModel.InAccountingPrice)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ReturnsTotalSumInAccountPriceSum, ValueDisplayType.Money);
                    currentCol++;
                }
                if (viewModel.InSalePrice)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ReturnsTotalSumInSalePriceSum, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ResultTotalSumInSalePriceSum, ValueDisplayType.Money);
                    currentCol++;
                }
            }
            if (viewModel.CalculateMarkup)
            {
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalMarkup, ValueDisplayType.Money);
                currentCol++;
            }

            foreach (var storage in viewModel.Storages)
            {
                if (viewModel.SeparationByStorages.ContainsKey(storage.Id))
                {
                    var subitem = viewModel.SeparationByStorages[storage.Id];

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.SoldCount, ValueDisplayType.PackCount);
                    currentCol++;
                    if (viewModel.InSalePrice)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.SalePriceSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.InPurchaseCost)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.PurchaseCostSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.AvailableArticleCount, ValueDisplayType.PackCount);
                    currentCol ++;
                    if (viewModel.InAccountingPrice)
                    {
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.StoredAccountingPriceSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                }
                else
                {
                    currentCol++;
                    if (viewModel.InSalePrice)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(0, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.InPurchaseCost)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(0, ValueDisplayType.Money);
                        currentCol++;
                    }
                    currentCol++;
                    if (viewModel.InAccountingPrice)
                    {
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(0, ValueDisplayType.Money);
                        currentCol++;
                    }
                }
            }
            currentRow++;
            currentCol = 1;
            
            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));
            currentRow++;

            return currentRow;
        }

        /// <summary>
        /// Подсчет количества столбцов в таблицах отчета
        /// </summary>
        /// <param name="viewModel">Данные</param>
        /// <param name="reportSettings">Настройки отчета</param>
        /// <param name="summaryTableColumns">Количество строк в сводных таблицах</param>
        /// <param name="detailsTableColumns">Количество строк в детализированной таблице</param>
        private void GetColumnCount(Report0002ViewModel viewModel, ReportSettings reportSettings, out int summaryTableColumns, out int detailsTableColumns)
        {
            int summaryTableColumnsCount = 2;
            int detailsTableColumnsCount = 4;

            //Подсчет для детализированной таблицы
            if (reportSettings.AreStoragesInColumns)
            {
                if (reportSettings.IsDevideByBatch)
                {
                    detailsTableColumnsCount++;
                }
                if (reportSettings.CalculateAveragePrice)
                {
                    if (reportSettings.InPurchaseCost) detailsTableColumnsCount++;
                    if (reportSettings.InSalePrice) detailsTableColumnsCount++;
                }
                if (reportSettings.InPurchaseCost) detailsTableColumnsCount++;
                if (reportSettings.InAccountingPrice) detailsTableColumnsCount++;
                if (reportSettings.InSalePrice) detailsTableColumnsCount++;
                if (reportSettings.WithReturnFromClient)
                {
                    detailsTableColumnsCount++;
                    if (reportSettings.InPurchaseCost) detailsTableColumnsCount++;
                    if (reportSettings.InAccountingPrice) detailsTableColumnsCount++;
                    if (reportSettings.InSalePrice) detailsTableColumnsCount += 2;
                }
                if (reportSettings.CalculateMarkup) detailsTableColumnsCount += 2;
                if (reportSettings.ShowAdditionColumns)
                {
                    detailsTableColumnsCount++;
                    if (reportSettings.IsDevideByBatch)
                    {
                        detailsTableColumnsCount += 2;
                    }
                }
                int count = 1; //Количество столбцов для вывода МХ
                if (reportSettings.InSalePrice) count++;
                if (reportSettings.InPurchaseCost) count++;
                count++;
                if (reportSettings.InAccountingPrice) count += 2;

                detailsTableColumnsCount += viewModel.DetailsTable.Storages.Count * count;
            }
            else
            {
                if (reportSettings.IsDevideByBatch)
                {
                    detailsTableColumnsCount++;
                }
                if (reportSettings.CalculateAveragePrice)
                {
                    if (reportSettings.InPurchaseCost) detailsTableColumnsCount++;
                    if (reportSettings.InSalePrice) detailsTableColumnsCount++;
                }
                if (reportSettings.InPurchaseCost) detailsTableColumnsCount++;
                if (reportSettings.InAccountingPrice) detailsTableColumnsCount++;
                if (reportSettings.InSalePrice) detailsTableColumnsCount++;
                if (reportSettings.WithReturnFromClient)
                {
                    detailsTableColumnsCount++;
                    if (reportSettings.InPurchaseCost) detailsTableColumnsCount++;
                    if (reportSettings.InAccountingPrice) detailsTableColumnsCount++;
                    if (reportSettings.InSalePrice) detailsTableColumnsCount += 2;
                }
                if (reportSettings.CalculateMarkup) detailsTableColumnsCount += 2;
                if (reportSettings.ShowAdditionColumns)
                {
                    detailsTableColumnsCount++;
                    if (reportSettings.IsDevideByBatch)
                    {
                        detailsTableColumnsCount += 2;
                    }
                }
            }

            //Подсчет для Сводных таблиц
            int subColumns = 0;
            if (reportSettings.InPurchaseCost) { subColumns++; }
            if (reportSettings.InAccountingPrice) { subColumns++; }
            if (reportSettings.InSalePrice) { subColumns++; }
            if (reportSettings.WithReturnFromClient)
            {
                if (subColumns > 0)
                    summaryTableColumnsCount += subColumns * 3 - 1;
            }
            else
            {
                if (subColumns > 0)
                    summaryTableColumnsCount += subColumns - 1;
            }
            if (reportSettings.CalculateMarkup) { summaryTableColumnsCount += 2; }
            if (reportSettings.ShowSoldArticleCount) { summaryTableColumnsCount++; }
            if (subColumns == 0) { summaryTableColumnsCount--; }
            summaryTableColumns = summaryTableColumnsCount;
            detailsTableColumns = detailsTableColumnsCount;
        }

        #endregion

        #endregion

    }
}