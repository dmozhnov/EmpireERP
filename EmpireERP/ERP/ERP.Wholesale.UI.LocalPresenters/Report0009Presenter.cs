using System;
using System.Collections.Generic;
using System.Data;
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
using ERP.Wholesale.UI.ViewModels.Report.Report0009;
using OfficeOpenXml;

namespace ERP.Wholesale.UI.LocalPresenters
{
    /// <summary>
    /// Отчет по поставкам
    /// </summary>
    public class Report0009Presenter : IReport0009Presenter
    {
        #region Внутрение типы данных

        /// <summary>
        /// Проверенные настройки
        /// </summary>
        private class ValidatedSettings
        {
            /// <summary>
            /// Обратный адрес
            /// </summary>
            public string BackURL { get; set; }

            /// <summary>
            /// Дата начала отчета
            /// </summary>
            public DateTime StartDate { get; set; }

            /// <summary>
            /// Дата конца отчета
            /// </summary>
            public DateTime EndDate { get; set; }

            /// <summary>
            /// Дата документа, по которой документ должен попадать в отчет
            /// </summary>
            public Report0009DateType UsedDateType { get; set; }

            #region Списки
            /// <summary>
            /// Строка кодов выбранных мест хранения
            /// </summary>
            public string StorageIds { get; set; }
            public bool AllStorages { get; set; }

            /// <summary>
            /// Строка кодов выбранных групп товаров
            /// </summary>
            public string ArticleGroupsIds { get; set; }
            public bool AllArticleGroups { get; set; }

            /// <summary>
            /// Строка кодов поставщиков
            /// </summary>
            public string ProvidersIds { get; set; }
            public bool AllProviders { get; set; }

            /// <summary>
            /// Строка кодов пользователей
            /// </summary>
            public string UsersIds { get; set; }
            public bool AllUsers { get; set; }

            /// <summary>
            /// Строка кодов выбранных группировок информации
            /// </summary>     
            public string GroupByCollectionIds { get; set; }

            /// <summary>
            /// Есть права на просмотр закупочных цен?
            /// </summary>
            public bool AllowToViewPurchaseCosts { get; set; }

            #endregion

            #region Группа настроек "Печать таблиц"

            /// <summary>
            /// Вывод развернутой таблицы
            /// </summary>
            public bool ShowDetailsTable { get; set; }

            /// <summary>
            /// Вывод развернутой таблицы c расхождениями
            /// </summary>
            public bool ShowDetailReceiptWaybillRowsWithDivergencesTable { get; set; }

            /// <summary>
            /// Вывести приходы по местам хранения
            /// </summary>
            public bool ShowStorageTable { get; set; }

            /// <summary>
            /// Вывести приходы по  организациям
            /// </summary>
            public bool ShowOrganizationTable { get; set; }

            /// <summary>
            /// Вывести приходы по  группе товаров
            /// </summary>
            public bool ShowArticleGroupTable { get; set; }

            /// <summary>
            /// Вывести приходы по поставщикам
            /// </summary>
            public bool ShowProviderTable { get; set; }

            /// <summary>
            /// Вывести приходы по  организациям поставщиков
            /// </summary>
            public bool ShowProviderOrganizationTable { get; set; }

            /// <summary>
            /// Вывести приходы по пользователям
            /// </summary>
            public bool ShowUserTable { get; set; }

            #endregion

            #region Группа  настроек "Вывод цен"

            /// <summary>
            /// Вывести закупочные цены
            /// </summary>
            public bool InPurchaseCost { get; set; }

            /// <summary>
            /// Вывести учетные цены
            /// </summary>
            public bool InRecipientWaybillAccountingPrice { get; set; }

            /// <summary>
            /// Вывести текущие учетные цены
            /// </summary>
            public bool InCurrentAccountingPrice { get; set; }


            #endregion

            #region Группа дополнительных настроек

            /// <summary>
            /// Вывести партии
            /// </summary>
            public bool ShowBatch { get; set; }

            /// <summary>
            /// Вывести кол-во в упаковке
            /// </summary>
            public bool ShowCountArticleInPack { get; set; }

            /// <summary>
            /// Вывести страну производства
            /// </summary>
            public bool ShowCountryOfProduction { get; set; }

            /// <summary>
            /// Вывести фабрику-изготовитель
            /// </summary>
            public bool ShowManufacturer { get; set; }

            /// <summary>
            /// Вывести ГТД
            /// </summary>
            public bool ShowCustomsDeclarationNumber { get; set; }

            /// <summary>
            /// Посчитать прибыль
            /// </summary>
            public bool CalculateMarkup { get; set; }

            #endregion
        }

        /// <summary>
        /// Строка детализированной таблицы
        /// </summary>
        private class Report0009DataModel
        {
            #region Поля
         
            /// <summary>
            /// Товар
            /// </summary>
            public Article Article { get; set; }

            /// <summary>
            /// МХ
            /// </summary>
            public Storage Storage { get; set; }

            /// <summary>
            /// Название партии
            /// </summary>
            public string BatchName { get; set; }

            /// <summary>
            /// Количество товара
            /// </summary>
            public decimal Count  { get; set; }

            /// <summary>
            /// Количество  в упаковоках
            /// </summary>
            public decimal PackCount 
            {
                get
                {
                    return Article.PackSize != 0 ? Count / Article.PackSize : 0;
                }
            }

            /// <summary>
            /// Текущая учетная цена
            /// </summary>
            public decimal? CurrentAccountingPrice { get; set; }

            /// <summary>
            /// Сумма в текущих учетных ценах
            /// </summary>
            public decimal? CurrentAccountingPriceSum
            {
                get
                {
                    return CurrentAccountingPrice * Count;
                }
            }

            /// <summary>
            /// Закупочная цена
            /// </summary>
            public decimal? PurchaseCost { get; set; }

            /// <summary>
            /// Сумма в закупочных ценах
            /// </summary>
            public  decimal? PurchaseCostSum
            {
                get
                {
                    return purchaseCostSum ?? Count * PurchaseCost;
                }
                set
                {
                    purchaseCostSum = value;
                }
            }
            private decimal? purchaseCostSum;

            /// <summary>
            /// Сумма в учетных ценах прихода
            /// </summary>
            public decimal? RecipientWaybillAccountingPriceSum
            {
                get
                {
                    return recipientWaybillAccountingPriceSum ?? RecipientWaybillAccountingPrice * Count;
                }
                set
                {
                    recipientWaybillAccountingPriceSum = value;
                }
            }
            private decimal? recipientWaybillAccountingPriceSum;

            /// <summary>
            /// Учетная цена прихода
            /// </summary>
            public decimal? RecipientWaybillAccountingPrice { get; set; }
            
            /// <summary>
            /// Наценка
            /// </summary>
            public decimal? CurrentMarkup
            {
                get
                {
                    return CurrentAccountingPriceSum - PurchaseCostSum;
                }
            }

            /// <summary>
            /// Номер ГТД (грузовая таможенная декларация)
            /// </summary>
            public  string CustomsDeclarationNumber { get; set; }

            /// <summary>
            /// Страна производства
            /// </summary>
            public Country ProductionCountry { get; set; }

            /// <summary>
            /// Фабрика-изготовитель
            /// </summary>
            public Manufacturer Manufacturer { get; set; }
      
            /// <summary>
            /// Поставщик
            /// </summary>
            public Provider Provider { get; set; }

            /// <summary>
            /// Организация поставщика
            /// </summary>
            public ContractorOrganization ProviderOrganization { get; set; }

            /// <summary>
            /// Договор с поставщиком
            /// </summary>
            public ProviderContract ProviderContract { get; set; }

            /// <summary>
            /// Организация - приемщик
            /// </summary>
            public AccountOrganization AccountOrganization { get; set; }

            /// <summary>
            /// Куратор накладной
            /// </summary>
            public User Curator { get; set; }

            #endregion
        }

        /// <summary>
        /// Тип даты, по которой документ должен попадать в отчет
        /// </summary>
        private enum Report0009DateType
        {
            /// <summary>
            /// Дата документа
            /// </summary>
            [EnumDisplayName("Дата документа")]
            DocumentDate = 1,

            /// <summary>
            /// Дата проводки
            /// </summary>
            [EnumDisplayName("Дата проводки")]
            AcceptanceDate = 2,

            /// <summary>
            /// Дата приемки
            /// </summary>
            [EnumDisplayName("Дата окончательной приемки")]
            ApprovementDate = 3,

            /// <summary>
            /// Дата документа и дата окончательной приемки
            /// </summary>
            [EnumDisplayName("Дата документа и дата приемки")]
            DocumentAndApprovementDate = 4,
        }

        /// <summary>
        /// Делегат для функции сервиса
        /// </summary>
        private delegate IEnumerable<ReceiptWaybillRow> GetReceiptWaybillRows(DateTime startDate, DateTime endDate, IEnumerable<short> storageIds, 
            IEnumerable<short> articleGroupIds, IEnumerable<int> providerIds, IEnumerable<int> userIds, int batchSize, int pageNumber);

        /// <summary>
        /// Тип группировки
        /// </summary>
        private enum GroupingType
        {
            /// <summary>
            /// По поставщикам
            /// </summary>
            [EnumDisplayName("Поставщик")]
            ByProvider = 1,
            
            /// <summary>
            /// По оранизации поставщика
            /// </summary>
            [EnumDisplayName("Организация поставщика")]
            ByProviderOrganization = 2,
        
            /// <summary>
            /// По договору
            /// </summary>
            [EnumDisplayName("Договор с поставщиком")]
            ByContract = 3,
        
            /// <summary>
            /// По МХ
            /// </summary>
            [EnumDisplayName("Место хранения - приемщик")]
            ByStorage = 4,
        
            /// <summary>
            /// По организации приемщика
            /// </summary>
            [EnumDisplayName("Организация приемщик")]
            ByAccountOrganization = 5,

            /// <summary>
            /// По куратору накладной
            /// </summary>
            [EnumDisplayName("Пользователь (куратор накладной)")]
            ByCurator = 6
        }

        #endregion

        #region Поля

        private readonly IUserService userService;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IProviderService providerService;
        private readonly IStorageService storageService;
        private readonly IArticleGroupService articleGroupService;
        private readonly IReceiptWaybillService receiptWaybillService;
        private readonly IArticlePriceService articlePriceService;

        /// <summary>
        /// Количество вытягиваемых из репозитория позиций накладных за один раз
        /// </summary>
        private const int batchSize = 500;

        #endregion

        #region Конструкторы

        public Report0009Presenter(IUnitOfWorkFactory unitOfWorkFactory, IUserService userService, IProviderService providerService,
            IStorageService storageService, IArticleGroupService articleGroupService, IReceiptWaybillService receiptWaybillService,
            IArticlePriceService articlePriceService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.userService = userService;
            this.providerService = providerService;
            this.storageService = storageService;
            this.articleGroupService = articleGroupService;
            this.receiptWaybillService = receiptWaybillService;
            this.articlePriceService = articlePriceService;
        }

        #endregion

        #region Методы

        #region Настройки отчета

        ///<summary>
        /// Настройка отчета
        /// </summary>
        public Report0009SettingsViewModel Report0009Settings(string backUrl, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                
                user.CheckPermission(Permission.Report0009_View);
                user.CheckPermission(Permission.ReceiptWaybill_List_Details);

                var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForReceipt);

                var dateTypeList = new List<Report0009DateType>() { Report0009DateType.DocumentDate, Report0009DateType.AcceptanceDate, Report0009DateType.ApprovementDate, Report0009DateType.DocumentAndApprovementDate };
                

                var model = new Report0009SettingsViewModel()
                {
                    BackURL = backUrl,

                    Storages = storageService.GetList(user, Permission.Report0009_Storage_List).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name),
                    ArticleGroups = articleGroupService.GetList().Where(x => x.Parent != null).OrderBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name),
                    Providers = providerService.GetList(user).OrderBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name),
                    Users = userService.GetList(user, Permission.Report0009_User_List).OrderBy(x => x.DisplayName).ToDictionary(x => x.Id.ToString(), x => x.DisplayName),
                    GroupByCollection = ComboBoxBuilder.GetComboBoxItemList<GroupingType>(false, false),
                    DateTypeList = ComboBoxBuilder.GetComboBoxItemList<Report0009DateType>(dateTypeList, x => x.GetDisplayName(), x => x.ValueToString(), true, false),
                    AllowToViewPurchaseCost = allowToViewPurchaseCost
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

        #endregion

        #region Построение отчета

        public Report0009ViewModel Report0009(Report0009SettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);                
                user.CheckPermission(Permission.Report0009_View);
                
                var allowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForReceipt);

                // если у пользователя нет прав на просмотр закупочных цен - не выводим их 
                if (!allowToViewPurchaseCosts)
                {
                    settings.CalculateMarkup = "0";
                    settings.InPurchaseCost = "0";
                }

                var viewModel = new Report0009ViewModel()
                {
                    Settings = settings,
                    CreatedBy = currentUser.DisplayName,
                    AllowToViewPurchaseCosts = allowToViewPurchaseCosts,
                    CreationDate = DateTimeUtils.GetCurrentDateTime().ToFullDateTimeString(), 
                };

                ValidatedSettings validatedSettings = GetValidatedSettings(settings);
                validatedSettings.AllowToViewPurchaseCosts = allowToViewPurchaseCosts;

                viewModel.DateTypeName = validatedSettings.UsedDateType.GetDisplayName();

                // устанавливаем последнюю секунду указанной даты
                validatedSettings.EndDate = validatedSettings.EndDate.AddHours(23).AddMinutes(59).AddSeconds(59);

                #region Получение списка кодов

                //получаем список кодов МХ
                IEnumerable<short> storageIDs = validatedSettings.AllStorages ? storageService.GetList(user, Permission.Report0009_Storage_List).Select(x => x.Id)
                                                                              : StringUtils.GetShortIdList(validatedSettings.StorageIds);

                // получаем список кодов групп товаров
                IEnumerable<short> articleGroupIDs = validatedSettings.AllArticleGroups ? null
                                                                                        : StringUtils.GetShortIdList(validatedSettings.ArticleGroupsIds);

                // получаем список кодов пользователей
                IEnumerable<int> userIDs = validatedSettings.AllUsers ? userService.GetList(user, Permission.Report0009_User_List).Select(x => x.Id)
                                                                      : StringUtils.GetIntIdList(validatedSettings.UsersIds);

                // получаем список кодов поставщиков
                IEnumerable<int> providerIDs = validatedSettings.AllProviders ? null
                                                                              : StringUtils.GetIntIdList(validatedSettings.ProvidersIds);

                #endregion

                //данные для группировок
                var groupFields = GetGroupFields(validatedSettings, user);
                var maxGroupLevel = groupFields.Count();
                
                #region Получение данных и заполнение развернутой и сводных таблиц

                //Если показываем детализированную таблицу или одну из сводных, то берем данные
                if (validatedSettings.ShowDetailsTable ||
                    validatedSettings.ShowArticleGroupTable ||
                    validatedSettings.ShowOrganizationTable ||
                    validatedSettings.ShowProviderOrganizationTable ||
                    validatedSettings.ShowProviderTable ||
                    validatedSettings.ShowStorageTable ||
                    validatedSettings.ShowUserTable)
                {
                    //получаем данные
                    var data = GetData(validatedSettings, storageIDs, articleGroupIDs, userIDs, providerIDs, user);

                    //Заполняем сводные таблицы
                    FillSummaryTables(data, validatedSettings, viewModel);

                    //Заполняем развернутую таблицу
                    if (validatedSettings.ShowDetailsTable)
                    {
                        viewModel.ReceiptWaybillRowDetailTable.Rows = FillDetailsTable(data, validatedSettings, groupFields);
                        viewModel.ReceiptWaybillRowDetailTable.Settings = settings;
                        viewModel.ReceiptWaybillRowDetailTable.AllowToViewPurchaseCosts = allowToViewPurchaseCosts;
                        viewModel.ReceiptWaybillRowDetailTable.Title = "Развернутая таблица по поставкам";
                    }

                }

                #endregion

                #region Получение данных и заполнение развернутой таблицы с расхождениями

                if (validatedSettings.ShowDetailReceiptWaybillRowsWithDivergencesTable)
                {
                    IEnumerable<Report0009DataModel> dataForTableWithDivergences;
                    string title;

                    //Для развернутой таблицы с расхождениями отдельно получаем данные
                    switch (validatedSettings.UsedDateType)
                    {
                        case Report0009DateType.DocumentDate:
                            dataForTableWithDivergences = GetDataFromWaybillRows(Report0009RowType.RowsDivergentByDate, validatedSettings, storageIDs, articleGroupIDs, userIDs, providerIDs, user);
                            title = "Развернутая таблица по поставкам с расхождениями с датой документа в указанном диапазоне";
                            break;
                        case Report0009DateType.AcceptanceDate:
                            dataForTableWithDivergences = GetDataFromWaybillRows(Report0009RowType.RowsDivergentByAcceptanceDate, validatedSettings, storageIDs, articleGroupIDs, userIDs, providerIDs, user);
                            title = "Развернутая таблица по поставкам с расхождениями с датой проводки в указанном диапазоне";
                            break;
                        default:
                            title = "Развернутая таблица по поставкам с расхождениями";
                            dataForTableWithDivergences = new List<Report0009DataModel>();
                            break;
                    } 

                    viewModel.ReceiptWaybillRowWithDivergencesAfterReceiptDetailTable.Rows = FillDetailsTable(dataForTableWithDivergences, validatedSettings, groupFields);
                    viewModel.ReceiptWaybillRowWithDivergencesAfterReceiptDetailTable.Settings = settings;
                    viewModel.ReceiptWaybillRowWithDivergencesAfterReceiptDetailTable.AllowToViewPurchaseCosts = allowToViewPurchaseCosts;
                    viewModel.ReceiptWaybillRowWithDivergencesAfterReceiptDetailTable.Title = title;
                }

                #endregion
                
                return viewModel;
            }
        }

        #region Валидация настроек

        private ValidatedSettings GetValidatedSettings(Report0009SettingsViewModel settings)
        {
            var result = new ValidatedSettings();

            result.UsedDateType = ValidationUtils.TryGetEnum<Report0009DateType>(settings.DateTypeId, "Неверный тип даты");

            #region Проверка дат

            result.StartDate = ValidationUtils.TryGetDate(settings.StartDate);
            result.EndDate = ValidationUtils.TryGetDate(settings.EndDate);

            ValidationUtils.Assert(result.EndDate >= result.StartDate, "Дата начала периода для отчета должна быть меньше либо равна дате конца.");

            #endregion

            #region Проверка списков

            result.AllStorages = ValidationUtils.IsTrue(settings.AllStorages);
            ValidationUtils.Assert(!String.IsNullOrEmpty(settings.StorageIDs) || result.AllStorages,
                                    "Не выбрано ни одного места хранения.");
            result.StorageIds = settings.StorageIDs;

            result.AllArticleGroups = ValidationUtils.IsTrue(settings.AllArticleGroups);
            ValidationUtils.Assert(!String.IsNullOrEmpty(settings.ArticleGroupsIDs) || result.AllArticleGroups,
                                    "Не выбрано ни одной группы товаров.");
            result.ArticleGroupsIds = settings.ArticleGroupsIDs;

            result.AllProviders = ValidationUtils.IsTrue(settings.AllProviders);
            ValidationUtils.Assert(!String.IsNullOrEmpty(settings.ProvidersIDs) || result.AllProviders,
                                    "Не выбрано ни одного поставщика.");
            result.ProvidersIds = settings.ProvidersIDs;

            result.AllUsers = ValidationUtils.IsTrue(settings.AllUsers);
            ValidationUtils.Assert(!String.IsNullOrEmpty(settings.UsersIDs) || ValidationUtils.TryGetBool(settings.AllUsers),
                                    "Не выбрано ни одного пользователя.");
            result.UsersIds = settings.UsersIDs;

            result.GroupByCollectionIds = settings.GroupByCollectionIDs;

            #endregion

            #region Проверка настроек отображения таблиц

            result.ShowDetailsTable = ValidationUtils.TryGetBool(settings.ShowDetailsTable);
            result.ShowDetailReceiptWaybillRowsWithDivergencesTable = ValidationUtils.TryGetBool(settings.ShowDetailReceiptWaybillRowsWithDivergencesTable);

            result.ShowStorageTable = ValidationUtils.TryGetBool(settings.ShowStorageTable);
            result.ShowOrganizationTable = ValidationUtils.TryGetBool(settings.ShowOrganizationTable);
            result.ShowArticleGroupTable = ValidationUtils.TryGetBool(settings.ShowArticleGroupTable);
            result.ShowProviderTable = ValidationUtils.TryGetBool(settings.ShowProviderTable);
            result.ShowProviderOrganizationTable = ValidationUtils.TryGetBool(settings.ShowProviderOrganizationTable);
            result.ShowUserTable = ValidationUtils.TryGetBool(settings.ShowUserTable);

            //Проверяем, чтобы хотя бы одна таблица была видима
            ValidateTablesVisibility(result);

            #endregion

            #region Проверка настроек отображения цен

            result.InPurchaseCost = ValidationUtils.TryGetBool(settings.InPurchaseCost);
            result.InRecipientWaybillAccountingPrice = ValidationUtils.TryGetBool(settings.InRecipientWaybillAccountingPrice);
            result.InCurrentAccountingPrice = ValidationUtils.TryGetBool(settings.InCurrentAccountingPrice);

            #endregion

            #region Проверка доп. настроек

            result.ShowBatch = ValidationUtils.TryGetBool(settings.ShowBatch);
            result.ShowCountArticleInPack = ValidationUtils.TryGetBool(settings.ShowCountArticleInPack);
            result.ShowCountryOfProduction = ValidationUtils.TryGetBool(settings.ShowCountryOfProduction);
            result.ShowManufacturer = ValidationUtils.TryGetBool(settings.ShowManufacturer);
            result.ShowCustomsDeclarationNumber = ValidationUtils.TryGetBool(settings.ShowCustomsDeclarationNumber);
            result.CalculateMarkup = ValidationUtils.TryGetBool(settings.CalculateMarkup);

            #endregion

            //Проверяем согласованость настроек
            ValidateSettingsDetailsTableConsistency(result);

            return result;
        }

        /// <summary>
        /// Проверяем чтобы хотя бы одна таблица была видима
        /// </summary>
        private void ValidateTablesVisibility(ValidatedSettings result)
        {
            if (!result.ShowDetailsTable &&
                !result.ShowDetailReceiptWaybillRowsWithDivergencesTable &&
                !result.ShowStorageTable &&
                !result.ShowOrganizationTable &&
                !result.ShowArticleGroupTable &&
                !result.ShowProviderTable &&
                !result.ShowProviderOrganizationTable &&
                !result.ShowUserTable)
            {
                throw new Exception("Не выбрано ни одной таблицы.");
            }
        }

        /// <summary>
        /// Проверить корректность всех зависимых настроек
        /// </summary>
        private void ValidateSettingsDetailsTableConsistency(ValidatedSettings result)
        {
            //если не показываем детализированные таблицы, то и цены с прибылью не показываем
            if (!result.ShowDetailsTable && !result.ShowDetailReceiptWaybillRowsWithDivergencesTable)
            {
                ValidationUtils.Assert(!result.InCurrentAccountingPrice,
                    "Параметр «В текущих учетных ценах» может быть равным «Да» только если параметр «Выводить развернутую таблицу» или параметр «Выводить развернутую таблицу с расхождениями»  равен «Да».");
                ValidationUtils.Assert(!result.InPurchaseCost,
                    "Параметр «Выводить партии товаров» может быть равным «Да» только если параметр «Выводить развернутую таблицу» или параметр «Выводить развернутую таблицу с расхождениями»  равен «Да».");
                ValidationUtils.Assert(!result.ShowCountArticleInPack,
                   "Параметр «Выводить кол-во в упаковке» может быть равным «Да» только если параметр «Выводить развернутую таблицу» или параметр «Выводить развернутую таблицу с расхождениями»  равен «Да».");

                ValidationUtils.Assert(!result.InRecipientWaybillAccountingPrice,
                    "Параметр «В учетных ценах прихода» может быть равным «Да» только если параметр «Выводить партии товаров»  равен «Да».");
                ValidationUtils.Assert(!result.InPurchaseCost,
                    "Параметр «В закупочных ценах» может быть равным «Да» только если параметр «Выводить партии товаров»  равен «Да».");
                ValidationUtils.Assert(!result.CalculateMarkup,
                    "Параметр «Посчитать текущую прибыль» может быть равным «Да» только если параметр «Выводить партии товаров»  равен «Да».");


                ValidationUtils.Assert(!result.ShowCountryOfProduction,
                    "Параметр «Выводить страну производства» может быть равным «Да» только если параметр «Выводить партии товаров»  равен «Да».");
                ValidationUtils.Assert(!result.ShowManufacturer,
                    "Параметр «Выводить фабрику-изготовителя» может быть равным «Да» только если параметр «Выводить партии товаров»  равен «Да».");
                ValidationUtils.Assert(!result.ShowCustomsDeclarationNumber,
                    "Параметр «Посчитать текущую прибыль» может быть равным «Да» только если параметр «Выводить партии товаров»  равен «Да».");
            }
            //Если выбраны даты «Дата окончательной приемки» и «Дата документа и дата окончательной приемки», то параметр «Выводить развернутую таблицу с расхождениями» не может быть true
            if (result.UsedDateType.ContainsIn(Report0009DateType.ApprovementDate, Report0009DateType.DocumentAndApprovementDate))
            {
                ValidationUtils.Assert(!result.ShowDetailReceiptWaybillRowsWithDivergencesTable, "Параметр «Выводить развернутую таблицу с расхождениями» не может быть равен «Да» при выбранном типе даты.");
            }
        }

        #endregion

        #region Получение данных

        /// <summary>
        /// Получить данные отчета
        /// </summary>
        private IEnumerable<Report0009DataModel> GetData(ValidatedSettings validatedSettings, IEnumerable<short> storageIDs, IEnumerable<short> articleGroupIDs,
            IEnumerable<int> userIDs, IEnumerable<int> providerIDs, User user)
        {
            IEnumerable<Report0009DataModel> articlesFromReceiptStorage;

            switch (validatedSettings.UsedDateType)
            {
                case Report0009DateType.DocumentDate:
                    articlesFromReceiptStorage = GetDataFromWaybillRows(Report0009RowType.RowsByDate, validatedSettings, storageIDs, articleGroupIDs, userIDs, providerIDs, user);
                    break;
                case Report0009DateType.AcceptanceDate:
                    articlesFromReceiptStorage = GetDataFromWaybillRows(Report0009RowType.RowsByAcceptenceDate, validatedSettings, storageIDs, articleGroupIDs, userIDs, providerIDs, user);
                    break;
                case Report0009DateType.ApprovementDate:
                    articlesFromReceiptStorage = GetDataFromWaybillRows(Report0009RowType.RowsByApprovementDate, validatedSettings, storageIDs, articleGroupIDs, userIDs, providerIDs, user);
                    break;
                case Report0009DateType.DocumentAndApprovementDate:
                    articlesFromReceiptStorage = GetDataFromWaybillRows(Report0009RowType.RowsByDateAndApprovementDate, validatedSettings, storageIDs, articleGroupIDs, userIDs, providerIDs, user);
                    break;
                default:
                    throw new Exception("Неизвестный тип даты");
            }

            return articlesFromReceiptStorage;
        }

        /// <summary>
        /// Получить данные из партий прихода
        /// </summary>
        private IEnumerable<Report0009DataModel> GetDataFromWaybillRows(Report0009RowType waybillRowType, ValidatedSettings validatedSettings,
            IEnumerable<short> storageIDs, IEnumerable<short> articleGroupIDs, IEnumerable<int> userIDs, IEnumerable<int> providerIDs, User user)
        {
            var detailsTableRows = new List<Report0009DataModel>();
            IEnumerable<ReceiptWaybillRow> waybillRows;
            int pageNumber = 0;

            //получаем запросами партии
            do
            {
                waybillRows = receiptWaybillService.GetRowsForReport0009(waybillRowType, validatedSettings.StartDate, validatedSettings.EndDate, storageIDs, articleGroupIDs, 
                    providerIDs, userIDs, ++pageNumber, batchSize);

                GetDetailsTableRows(detailsTableRows, waybillRows, validatedSettings, user);
            }
            while (waybillRows.Count() >= batchSize);

            return detailsTableRows;
        }

        /// <summary>
        /// Выбрать данные
        /// </summary>
        private void GetDetailsTableRows(List<Report0009DataModel> detailsTableRows, IEnumerable<ReceiptWaybillRow> waybillRows, ValidatedSettings settings, User user)
        {   
            DynamicDictionary<short, DynamicDictionary<int, decimal?>> articlesPrice = null;

            articlesPrice = articlePriceService.GetAccountingPrice(waybillRows.GroupBy(x => x.ReceiptWaybill.ReceiptStorage).Select(x => x.Key.Id),
                                                                        waybillRows.Select(x => x.Article.Id), DateTime.Now);

            foreach (var waybillRow in waybillRows)
            {
                var allowToViewStorageAccountingPrices = user.HasPermissionToViewStorageAccountingPrices(waybillRow.ReceiptWaybill.ReceiptStorage);
                var allowToViewStoragePurchasePrices = user.HasPermissionToViewStoragePurchasePrices(waybillRow.ReceiptWaybill.ReceiptStorage, waybillRow.ReceiptWaybill.Curator);

                var detailsTableRow = new Report0009DataModel()
                {
                    Article = waybillRow.Article,
                    BatchName = waybillRow.BatchName,
                    Count = waybillRow.CurrentCount,
                    PurchaseCost = allowToViewStoragePurchasePrices ? (decimal?)waybillRow.PurchaseCost : null,
                    RecipientWaybillAccountingPrice = waybillRow.RecipientArticleAccountingPrice == null || !allowToViewStorageAccountingPrices ? null
                                                                                : (decimal?)waybillRow.RecipientArticleAccountingPrice.AccountingPrice,
                    Storage = waybillRow.ReceiptWaybill.ReceiptStorage,
                    CurrentAccountingPrice = allowToViewStorageAccountingPrices ? articlesPrice[waybillRow.ReceiptWaybill.ReceiptStorage.Id][waybillRow.Article.Id] : null,
                    Manufacturer = waybillRow.Manufacturer,
                    ProductionCountry = waybillRow.ProductionCountry,
                    CustomsDeclarationNumber = waybillRow.CustomsDeclarationNumber,
                    AccountOrganization = waybillRow.ReceiptWaybill.AccountOrganization,
                    Curator = waybillRow.ReceiptWaybill.Curator,
                    ProviderContract = waybillRow.ReceiptWaybill.ProviderContract,
                    Provider = waybillRow.ReceiptWaybill.Provider,
                    ProviderOrganization = waybillRow.ReceiptWaybill.Contract.ContractorOrganization
                };
        
                detailsTableRows.Add(detailsTableRow);
            }
        }
        
        #endregion

        #region Построение представления

        #region Развернутые таблицы

        /// <summary>
        /// Заполнить модель для детализированной таблицы.
        /// </summary>
        private IEnumerable<Report0009DetailTableItemViewModel> FillDetailsTable(IEnumerable<Report0009DataModel> data, ValidatedSettings validatedSettings, List<int> groupFields)
        {
            var detailTable = new List<Report0009DetailTableItemViewModel>();
            GroupByDetailsTableRow(data, validatedSettings, detailTable, groupFields, 1);
            return detailTable;
        }

        #region Группировки

        /// <summary>
        /// Сделать группировки. Рекурсивный спуск.
        /// </summary>
        /// <param name="list">Данные для строк таблицы</param>
        /// <param name="groupFields">Список группировок, которые необходимо произвести</param>
        /// <param name="groupLevel">Уровень текущей группировки</param>
        private void GroupByDetailsTableRow(IEnumerable<Report0009DataModel> list, ValidatedSettings settings, IList<Report0009DetailTableItemViewModel> viewModel,
            List<int> groupFields, int groupLevel)
        {
            //Выход из рекурсии, прекращения группировок и заполнение обычными строками с товаром
            if (groupFields.Count < groupLevel)
            {
                FillDetailsTableRows(list, viewModel, settings);
                return;
            }
            
            //Выбор текущей группировки
            switch (groupFields[groupLevel - 1])
            {
                case (byte)GroupingType.ByProvider:
                    DetailsTableRowsGrouping<Provider>(list, settings, viewModel, x => x.Provider, x => x.Provider.Name, "Поставщик", groupFields, groupLevel);
                    break;
                case (byte)GroupingType.ByProviderOrganization:
                    DetailsTableRowsGrouping<ContractorOrganization>(list, settings, viewModel, x => x.ProviderOrganization, x => x.ProviderOrganization.ShortName, 
                        "Организация поставщика", groupFields, groupLevel);
                    break;
                case (byte)GroupingType.ByContract:
                    DetailsTableRowsGrouping<ProviderContract>(list, settings, viewModel, x => x.ProviderContract, x => x.ProviderContract.Name, 
                        "Договор с поставщиком", groupFields, groupLevel);
                    break;
                case (byte)GroupingType.ByStorage:
                    DetailsTableRowsGrouping<Storage>(list, settings, viewModel, x => x.Storage, x => x.Storage.Name, "Место хранения - приемщик", groupFields, groupLevel);
                    break;
                case (byte)GroupingType.ByAccountOrganization:
                    DetailsTableRowsGrouping<AccountOrganization>(list, settings, viewModel, x => x.AccountOrganization, x => x.AccountOrganization.ShortName, 
                        "Организация - приемщик", groupFields, groupLevel);
                    break;
                case (byte)GroupingType.ByCurator:
                    DetailsTableRowsGrouping<User>(list, settings, viewModel, x => x.Curator, x => x.Curator.DisplayName, "Куратор накладной", groupFields, groupLevel);
                    break;
                default:
                    throw new Exception("Неизвестная группировка данных.");
            }
        }

        /// <summary>
        /// Заполнить модель-представление строками с товаром
        /// </summary>
        private void FillDetailsTableRows(IEnumerable<Report0009DataModel> list, IList<Report0009DetailTableItemViewModel> viewModel, ValidatedSettings settings)
        {
            if (settings.ShowBatch)
                FillDetailsTableRowWithBatch(list, viewModel, settings);
            else
                FillDetailsTableRowsWithoutBatch(list, viewModel, settings);
        }

        /// <summary>
        /// Заполняем представление строками с партиями
        /// </summary>
        private static void FillDetailsTableRowWithBatch(IEnumerable<Report0009DataModel> list, IList<Report0009DetailTableItemViewModel> viewModel, ValidatedSettings settings)
        {
            foreach (var row in list)
            {
                var tableRow = new Report0009DetailTableItemViewModel()
                {
                    ArticleCount = row.Count.ForDisplay(),
                    ArticleName = row.Article.ShortName,
                    ArticleNumber = row.Article.Number,
                    ArticleId = row.Article.Id.ToString(),
                    CountArticleInPack = settings.ShowCountArticleInPack ? row.PackCount.ForDisplay(ValueDisplayType.PackCount) : "",
                    CurrentAccountingPrice = settings.InCurrentAccountingPrice ? row.CurrentAccountingPrice.ForDisplay(ValueDisplayType.Money) : "",
                    BatchName = row.BatchName,
                    CountryOfProduction = settings.ShowCountryOfProduction ? row.ProductionCountry.Name : "",
                    CustomsDeclarationNumber = settings.ShowCustomsDeclarationNumber ? row.CustomsDeclarationNumber : "",
                    Manufacturer = settings.ShowManufacturer ? row.Manufacturer.Name : "",
                    Markup = settings.AllowToViewPurchaseCosts && settings.CalculateMarkup ? row.CurrentMarkup.ForDisplay(ValueDisplayType.Money) : "",
                    PurchaseCost = settings.AllowToViewPurchaseCosts && settings.InPurchaseCost ? row.PurchaseCost.ForDisplay(ValueDisplayType.Money) : "",
                    RecipientWaybillAccountingPrice = settings.InRecipientWaybillAccountingPrice ? row.RecipientWaybillAccountingPrice.ForDisplay(ValueDisplayType.Money) : ""
                };

                viewModel.Add(tableRow);
            }
        }

        /// <summary>
        /// Заполняем представление строками без партий
        /// </summary>
        private static void FillDetailsTableRowsWithoutBatch(IEnumerable<Report0009DataModel> list, IList<Report0009DetailTableItemViewModel> viewModel,
            ValidatedSettings settings)
        {
            foreach (var articleFromStorage in list.GroupBy(x => new { x.Article, x.Storage }))
            {
                var tableRow = new Report0009DetailTableItemViewModel()
                {
                    ArticleCount = articleFromStorage.Sum(x => x.Count).ForDisplay(),
                    ArticleName = articleFromStorage.Key.Article.ShortName,
                    ArticleNumber = articleFromStorage.Key.Article.Number,
                    ArticleId = articleFromStorage.Key.Article.Id.ToString(),
                    CountArticleInPack = settings.ShowCountArticleInPack ? articleFromStorage.Sum(x => x.PackCount).ForDisplay(ValueDisplayType.PackCount) : "",
                    CurrentAccountingPrice = settings.InCurrentAccountingPrice ? articleFromStorage.Sum(x => x.CurrentAccountingPrice).ForDisplay(ValueDisplayType.Money) : ""
                };

                viewModel.Add(tableRow);
            }
        }


        private void DetailsTableRowsGrouping<T>(IEnumerable<Report0009DataModel> list, ValidatedSettings settings, IList<Report0009DetailTableItemViewModel> viewModel,
              Func<Report0009DataModel, T> groupingObject, Func<Report0009DataModel, string> name, string title, List<int> groupFields, int groupLevel)
        {
            foreach (var group in list.GroupBy(x => groupingObject(x)).OrderBy(x => name(x.First())))
            {
                viewModel.Add(new Report0009DetailTableItemViewModel()
                {
                    IsGroup = true,
                    GroupTitle = String.Format("{0}: {1}", title, name(group.First())),
                    GroupLevel = groupLevel
                }); // Добавляем заголовок группы

                // группируем данные далее
                GroupByDetailsTableRow(group, settings, viewModel, groupFields, groupLevel + 1);
            }
        }


        private List<int> GetGroupFields(ValidatedSettings settings, User user)
        {
            var result = new List<int>();
            if (String.IsNullOrEmpty(settings.GroupByCollectionIds))
                return result;

            foreach (var val in settings.GroupByCollectionIds.Split('_'))
            {
                var value = ValidationUtils.TryGetInt(val);
                var enumValue = ValidationUtils.TryGetEnum<GroupingType>(val, "Неверный код группировки.");

                //проверяем права на группировки
                switch (enumValue)
                {
                    case GroupingType.ByProvider:
                        ValidationUtils.Assert(user.HasPermission(Permission.Provider_List_Details),
                            "Недостаточно прав на выполнение операции «Группировка по поставщику.»");
                        break;
                    case GroupingType.ByProviderOrganization:
                        ValidationUtils.Assert(user.HasPermission(Permission.ProviderOrganization_List_Details),
                            "Недостаточно прав на выполнение операции «Группировка по организации поставщика».");
                        break;
                    case GroupingType.ByContract:
                        //Отдельного права на просмотр договора нету, но он относится к деталям организации
                        ValidationUtils.Assert(user.HasPermission(Permission.ProviderOrganization_List_Details),
                            "Недостаточно прав на выполнение операции «Группировка по договору с поставщиком».");
                        break;
                    case GroupingType.ByStorage://Уже проверили при вытягивании данных
                        break;
                    case GroupingType.ByAccountOrganization://На просмотр собственных организаций права есть всегда
                        break;
                    case GroupingType.ByCurator://Уже проверили при вытягивании данных
                        break;
                    default:
                        break;
                }

                result.Add(value);
            }

            return result;

        }

        #endregion

        #endregion

        #region Сводные таблицы

        private void FillSummaryTables(IEnumerable<Report0009DataModel> data, ValidatedSettings settings, Report0009ViewModel viewModel)
        {

            if(settings.ShowOrganizationTable)
            {
                viewModel.AccountOrganizationSummaryTable = FillSummaryTable<AccountOrganization>(data, x => x.AccountOrganization, 
                                                                                x => x.AccountOrganization.ShortName, settings);
                viewModel.AccountOrganizationSummaryTable.TableTitle = "Сводная таблица по организациям";
                viewModel.AccountOrganizationSummaryTable.NameColumnTitle = "Организация";
            }

            if(settings.ShowProviderOrganizationTable)
            {
                viewModel.ProviderOrganizationSummaryTable = FillSummaryTable<ContractorOrganization>(data, x => x.ProviderOrganization,
                                                                                x => x.ProviderOrganization.ShortName, settings);
                viewModel.ProviderOrganizationSummaryTable.TableTitle = "Сводная таблица по организациям поставщиков";
                viewModel.ProviderOrganizationSummaryTable.NameColumnTitle = "Организация поставщика";
            }

            if(settings.ShowProviderTable)
            {
                viewModel.ProviderSummaryTable = FillSummaryTable<Provider>(data, x => x.Provider,
                                                                x => x.Provider.Name, settings);
                viewModel.ProviderSummaryTable.TableTitle = "Сводная таблица по поставщикам";
                viewModel.ProviderSummaryTable.NameColumnTitle = "Поставщик";
            }

            if(settings.ShowStorageTable)
            {
                viewModel.StorageSummaryTable = FillSummaryTable<Storage>(data, x => x.Storage,
                                                x => x.Storage.Name, settings);
                viewModel.StorageSummaryTable.TableTitle = "Сводная таблица по МХ";
                viewModel.StorageSummaryTable.NameColumnTitle = "Место хранения";
            }

            if(settings.ShowUserTable)
            {
                viewModel.UserSummaryTable = FillSummaryTable<User>(data, x => x.Curator,
                                                x => x.Curator.DisplayName, settings);
                viewModel.UserSummaryTable.TableTitle = "Сводная таблица по кураторам накладных";
                viewModel.UserSummaryTable.NameColumnTitle = "Куратор накладной";
            }

            if(settings.ShowArticleGroupTable)
            {
                viewModel.ArticleGroupSummaryTable = FillSummaryTable<ArticleGroup>(data, x => x.Article.ArticleGroup,
                                                x => x.Article.ArticleGroup.Name, settings);
                viewModel.ArticleGroupSummaryTable.TableTitle = "Сводная таблица по группам товаров";
                viewModel.ArticleGroupSummaryTable.NameColumnTitle = "Группа товара";
            }

        }

        private Report0009SummaryTableViewModel FillSummaryTable<T>(IEnumerable<Report0009DataModel> data, 
            Func<Report0009DataModel, T> groupingObject, Func<Report0009DataModel, string> name, ValidatedSettings settings)
        {           
            var result = new Report0009SummaryTableViewModel();
            var summaryTableRows = new List<Report0009SummaryTableItemViewModel>();
            
            decimal? purchaseCostSumTotal = 0;
            decimal? recipientWaybillAccountingPriceSumTotal = 0;
            decimal? currentAccountingPriceSumTotal = 0;
            decimal? markupSumTotal = 0;

            foreach (var row in data.GroupBy(x => groupingObject(x)))
            {
                //если у нас вообще нет текущих УЦ, то нет и прибыли, в обоих столбцах выводим прочерки
                var thereIsNoCurrentAccountingPrice = row.All(x => !x.CurrentAccountingPriceSum.HasValue);

                var thereIsNoCurrentPurchasePrice = row.All(x => !x.PurchaseCostSum.HasValue);

                //производим вычисления только если это необходимо
                var purchaseCostSum = !thereIsNoCurrentPurchasePrice ? (decimal?)row.Sum(x => x.PurchaseCostSum) : null;
                var recipientWaybillAccountingPriceSum = !thereIsNoCurrentAccountingPrice ? (decimal?) row.Sum(x => x.RecipientWaybillAccountingPriceSum ?? 0) : null;
                var currentAccountingPriceSum =  !thereIsNoCurrentAccountingPrice ? (decimal?)row.Sum(x => x.CurrentAccountingPriceSum ?? 0) : null;
                var markupSum = !thereIsNoCurrentAccountingPrice && !thereIsNoCurrentPurchasePrice ? (decimal?)row.Sum(x => x.CurrentMarkup) : null;

                summaryTableRows.Add(new Report0009SummaryTableItemViewModel()
                {
                    Name = name(row.First()),
                    PurchaseCostSum = purchaseCostSum.ForDisplay(ValueDisplayType.Money),
                    RecipientWaybillAccountingPriceSum = recipientWaybillAccountingPriceSum.ForDisplay(ValueDisplayType.Money),
                    CurrentAccountingPriceSum = currentAccountingPriceSum.ForDisplay(ValueDisplayType.Money),
                    MarkupSum = markupSum.ForDisplay(ValueDisplayType.Money)
                });

                purchaseCostSumTotal += purchaseCostSum != null ? purchaseCostSum : 0;
                recipientWaybillAccountingPriceSumTotal += recipientWaybillAccountingPriceSum != null ? recipientWaybillAccountingPriceSum : 0;
                currentAccountingPriceSumTotal += currentAccountingPriceSum != null ? currentAccountingPriceSum : 0;
                markupSumTotal += markupSum != null ? markupSum : 0;
            }

            result.Rows = summaryTableRows;
            result.AllowToViewPurchaseCost = settings.AllowToViewPurchaseCosts;
            result.ShowMarkup = settings.CalculateMarkup;
            result.InPurchaseCost = settings.InPurchaseCost;
            result.InRecipientWaybillAccountingPrice = settings.InRecipientWaybillAccountingPrice;
            result.InCurrentAccountingPrice = settings.InCurrentAccountingPrice;

            result.PurchaseCostSumTotal = purchaseCostSumTotal.ForDisplay(ValueDisplayType.Money);
            result.RecipientWaybillAccountingPriceSumTotal = recipientWaybillAccountingPriceSumTotal.ForDisplay(ValueDisplayType.Money);
            result.CurrentAccountingPriceSumTotal = currentAccountingPriceSumTotal.ForDisplay(ValueDisplayType.Money);
            result.MarkupSumTotal = markupSumTotal.ForDisplay(ValueDisplayType.Money);

            return result;
        }

        #endregion
        
        #endregion

        #endregion

        #region Выгрузка в Excel
        /// <summary>
        /// Экспорт отчета в Excel
        /// </summary>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="currentUser">Текущий пользователь</param>
        public byte[] Report0009ExportToExcel(Report0009SettingsViewModel settings, UserInfo currentUser)
        {
            var viewModel = Report0009(settings, currentUser);

            string reportHeader = "Отчет «Поставки»\r\nза период с " + viewModel.Settings.StartDate + " по " + viewModel.Settings.EndDate;
            string sign = "Форма: Report0009 \r\nАвтор: " + viewModel.CreatedBy + "\r\nСоставлен: " + viewModel.CreationDate;
            string subheader = "В диапазон попадает: " + viewModel.DateTypeName;
            int detailsTableColumnCount = 0;
            int summaryTableColumnCount = 0;
            GetColumnCount(viewModel.Settings, out detailsTableColumnCount, out summaryTableColumnCount);

            using (ExcelPackage pck = new ExcelPackage())
            {
                if (viewModel.Settings.ShowStorageTable == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по МХ");
                    FillSummaryTable(sheet, summaryTableColumnCount, viewModel.StorageSummaryTable, 
                        sheet.PrintHeader(summaryTableColumnCount, reportHeader, sign, viewModel.StorageSummaryTable.TableTitle + ":", 1, subheader));
                }
        
                if (viewModel.Settings.ShowOrganizationTable == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по собственным организациям");
                    FillSummaryTable(sheet, summaryTableColumnCount, viewModel.AccountOrganizationSummaryTable, 
                        sheet.PrintHeader(summaryTableColumnCount, reportHeader, sign, viewModel.AccountOrganizationSummaryTable.TableTitle + ":", 1, subheader));
                }
       
                if (viewModel.Settings.ShowArticleGroupTable == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по группам товаров");
                    FillSummaryTable(sheet, summaryTableColumnCount, viewModel.ArticleGroupSummaryTable, 
                        sheet.PrintHeader(summaryTableColumnCount, reportHeader, sign, viewModel.ArticleGroupSummaryTable.TableTitle + ":", 1, subheader));
                }
        
                if (viewModel.Settings.ShowProviderTable == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по поставщикам");
                    FillSummaryTable(sheet, summaryTableColumnCount, viewModel.ProviderSummaryTable, 
                        sheet.PrintHeader(summaryTableColumnCount, reportHeader, sign, viewModel.ProviderSummaryTable.TableTitle + ":", 1, subheader));
                }
        
                if (viewModel.Settings.ShowProviderOrganizationTable == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по организациям поставщиков");
                    FillSummaryTable(sheet, summaryTableColumnCount, viewModel.ProviderOrganizationSummaryTable, 
                        sheet.PrintHeader(summaryTableColumnCount, reportHeader, sign, viewModel.ProviderOrganizationSummaryTable.TableTitle + ":", 1, subheader));
                }
        
                if (viewModel.Settings.ShowUserTable == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по куратором");
                    FillSummaryTable(sheet, summaryTableColumnCount, viewModel.UserSummaryTable, 
                        sheet.PrintHeader(summaryTableColumnCount, reportHeader, sign, viewModel.UserSummaryTable.TableTitle + ":", 1, subheader));
                }

                if (viewModel.Settings.ShowDetailsTable == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Развернутая по приходам");
                    FillDetailsTable(sheet, detailsTableColumnCount, viewModel.ReceiptWaybillRowDetailTable, 
                        sheet.PrintHeader(detailsTableColumnCount, reportHeader, sign, viewModel.ReceiptWaybillRowDetailTable.Title + ":", 1, subheader));
                } 
        
                if (viewModel.Settings.ShowDetailReceiptWaybillRowsWithDivergencesTable == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Развернутая по приходам c расхождениями");
                    FillDetailsTable(sheet, detailsTableColumnCount, viewModel.ReceiptWaybillRowWithDivergencesAfterReceiptDetailTable, 
                        sheet.PrintHeader(detailsTableColumnCount, reportHeader, sign, viewModel.ReceiptWaybillRowWithDivergencesAfterReceiptDetailTable.Title + ":", 1, subheader));
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
        private int FillSummaryTable(ExcelWorksheet workSheet, int columns, Report0009SummaryTableViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.NameColumnTitle);
            currentCol++;
            if (viewModel.InPurchaseCost)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009SummaryTableItemViewModel>(x => x.PurchaseCostSum));
                currentCol++;
            }
            if (viewModel.InRecipientWaybillAccountingPrice)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009SummaryTableItemViewModel>(x => x.RecipientWaybillAccountingPriceSum));
                currentCol++;
            }
            if (viewModel.InCurrentAccountingPrice)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009SummaryTableItemViewModel>(x => x.CurrentAccountingPriceSum));
                currentCol++;
            }
            if (viewModel.ShowMarkup)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009SummaryTableItemViewModel>(x => x.MarkupSum));
            }
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
                if (viewModel.InPurchaseCost)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCostSum, ValueDisplayType.Money);
                    currentCol++;
                }
                if (viewModel.InRecipientWaybillAccountingPrice)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.RecipientWaybillAccountingPriceSum, ValueDisplayType.Money);
                    currentCol++;
                }
                if (viewModel.InCurrentAccountingPrice)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.CurrentAccountingPriceSum, ValueDisplayType.Money);
                    currentCol++;
                }
                if (viewModel.ShowMarkup)
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.MarkupSum, ValueDisplayType.Money);
                    currentCol++;
                }
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

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого по столбцу:");
            currentCol++;
            if (viewModel.InPurchaseCost)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.PurchaseCostSumTotal, ValueDisplayType.Money);
                currentCol++;
            }
            if (viewModel.InRecipientWaybillAccountingPrice)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.RecipientWaybillAccountingPriceSumTotal, ValueDisplayType.Money);
                currentCol++;
            }
            if (viewModel.InCurrentAccountingPrice)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.CurrentAccountingPriceSumTotal, ValueDisplayType.Money);
                currentCol++;
            }
            if (viewModel.ShowMarkup)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.MarkupSumTotal, ValueDisplayType.Money);
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
        /// Формирует развернутую таблицу 
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillDetailsTable(ExcelWorksheet workSheet, int columns, Report0009DetailTableViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow + 1, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 2].MergeRange().SetFormattedValue("Товар");
            workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009DetailTableItemViewModel>(x => x.ArticleId));
            currentCol++;
            workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009DetailTableItemViewModel>(x => x.ArticleNumber));
            currentCol++;
            workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009DetailTableItemViewModel>(x => x.ArticleName));
            currentCol++;

            workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009DetailTableItemViewModel>(x => x.ArticleCount));
            currentCol++;

            if (viewModel.Settings.ShowBatch == "1")
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009DetailTableItemViewModel>(x => x.BatchName));
                currentCol++;
            }

            if (viewModel.Settings.ShowCountArticleInPack == "1")
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009DetailTableItemViewModel>(x => x.CountArticleInPack));
                currentCol++;
            }

            if (viewModel.Settings.InPurchaseCost == "1")
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009DetailTableItemViewModel>(x => x.PurchaseCost));
                currentCol++;
            }
                    
            if (viewModel.Settings.InRecipientWaybillAccountingPrice == "1")
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009DetailTableItemViewModel>(x => x.RecipientWaybillAccountingPrice));
                currentCol++;
            }
                    
            if (viewModel.Settings.InCurrentAccountingPrice == "1")
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009DetailTableItemViewModel>(x => x.CurrentAccountingPrice));
                currentCol++;
            }
                    
            if (viewModel.Settings.CalculateMarkup == "1")
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009DetailTableItemViewModel>(x => x.Markup));
                currentCol++;
            }
                    
            if (viewModel.Settings.ShowManufacturer == "1")
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009DetailTableItemViewModel>(x => x.Manufacturer));
                currentCol++;
            }
                    
            if (viewModel.Settings.ShowCountryOfProduction == "1")
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009DetailTableItemViewModel>(x => x.CountryOfProduction));
                currentCol++;
            }
                    
            if (viewModel.Settings.ShowCustomsDeclarationNumber == "1")
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0009DetailTableItemViewModel>(x => x.CustomsDeclarationNumber));
                currentCol++;
            }
            currentCol = 1;
            currentRow += 2;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки
            bool flag = false;
            foreach (var row in viewModel.Rows)
            {
                if (row.IsGroup)
                {
                    flag = false;
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().SetFormattedValue(row.GroupTitle, ExcelUtils.GetTableSubTotalRowStyle())
                        .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, indent: row.GroupLevel);
                }
                else
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ArticleId).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ArticleNumber).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ArticleName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ArticleCount, ValueDisplayType.PackCount);
                    currentCol++;

                    if (viewModel.Settings.ShowBatch == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.BatchName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                            
                    if (viewModel.Settings.ShowCountArticleInPack == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.CountArticleInPack, ValueDisplayType.PackCount);
                        currentCol++;
                    }

                    if (viewModel.Settings.InPurchaseCost == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCost, ValueDisplayType.Money);
                        currentCol++;
                    }
                    
                    if (viewModel.Settings.InRecipientWaybillAccountingPrice == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.RecipientWaybillAccountingPrice, ValueDisplayType.Money);
                        currentCol++;
                    }
                    
                    if (viewModel.Settings.InCurrentAccountingPrice == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.CurrentAccountingPrice, ValueDisplayType.Money);
                        currentCol++;
                    }
                    
                    if (viewModel.Settings.CalculateMarkup == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Markup, ValueDisplayType.Money);
                        currentCol++;
                    }
                    
                    if (viewModel.Settings.ShowManufacturer == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Manufacturer).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                    
                    if (viewModel.Settings.ShowCountryOfProduction == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.CountryOfProduction).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                            
                    if (viewModel.Settings.ShowCustomsDeclarationNumber == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.CustomsDeclarationNumber).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
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

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        /// <summary>
        /// Подсчет колонок в таблицах
        /// </summary>
        private void GetColumnCount(Report0009SettingsViewModel settings, out int detailsTableColumnCount, out int summaryTableColumnCount)
        {
            detailsTableColumnCount = 4;
            summaryTableColumnCount = 1;

            if (settings.CalculateMarkup == "1") 
            { 
                summaryTableColumnCount++;
                detailsTableColumnCount++;
            }

            if (settings.ShowBatch == "1") detailsTableColumnCount++;

            if (settings.ShowCountArticleInPack == "1") detailsTableColumnCount++;

            if (settings.InPurchaseCost == "1")
            {
                detailsTableColumnCount++;
                summaryTableColumnCount++;
            }

            if (settings.InRecipientWaybillAccountingPrice == "1")
            {
                detailsTableColumnCount++;
                summaryTableColumnCount++;
            }

            if (settings.InCurrentAccountingPrice == "1")
            {
                detailsTableColumnCount++;
                summaryTableColumnCount++;
            }
                    
            if (settings.ShowManufacturer == "1") detailsTableColumnCount++;
                    
            if (settings.ShowCountryOfProduction == "1") detailsTableColumnCount++;
                    
            if (settings.ShowCustomsDeclarationNumber == "1") detailsTableColumnCount++;
        }
        #endregion

        #endregion


    }
}
