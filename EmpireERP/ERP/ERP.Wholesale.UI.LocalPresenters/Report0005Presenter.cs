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
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Report.Report0005;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class Report0005Presenter : IReport0005Presenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        IUserService userService;
        IStorageService storageService;
        IArticleService articleService;
        IReceiptWaybillService receiptWaybillService;
        IReturnFromClientWaybillService returnFromClientWaybillService;
        IArticlePriceService articlePriceService;
        IWaybillRowArticleMovementRepository waybillRowArticleMovementRepository;
        IIncomingWaybillRowService incomingWaybillRowService;
        IOutgoingWaybillRowService outgoingWaybillRowService;
        IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService;


        //Глобальные переменные - роль кэша. Хранят позиции и прочее. Вынесено в переменные из за того, что есть много методов, которым нужны эти данные, но делать в методах по 10 параметров - кажется неудобным
        Dictionary<Guid, BaseWaybillRow> sourceWaybills;
        /// <summary>
        /// Коллекция хранит количество раз, сколько позиция накладной встречается в дереве - нужно для отображения повторных позиций
        /// </summary>
        Dictionary<Guid, int> waybillRowsCount;
        /// <summary>
        /// Список id позиций источников
        /// </summary>
        List<Guid> sourceWaybillsIds;
        DynamicDictionary<int, decimal?> prices;
        Report0005Permissions permissions;
        IEnumerable<short> storageIDs;

        #endregion

        #region Конструкторы

        public Report0005Presenter(IUnitOfWorkFactory unitOfWorkFactory, IUserService userService, IStorageService storageService, IArticleService articleService,
            IReceiptWaybillService receiptWaybillService, IReturnFromClientWaybillService returnFromClientWaybillService, IArticlePriceService articlePriceService,
            IWaybillRowArticleMovementRepository waybillRowArticleMovementRepository, IIncomingWaybillRowService incomingWaybillRowService,
            IOutgoingWaybillRowService outgoingWaybillRowService, IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.userService = userService;
            this.storageService = storageService;
            this.articleService = articleService;

            this.receiptWaybillService = receiptWaybillService;
            this.returnFromClientWaybillService = returnFromClientWaybillService;
            this.articlePriceService = articlePriceService;
            this.waybillRowArticleMovementRepository = waybillRowArticleMovementRepository;
            this.incomingWaybillRowService = incomingWaybillRowService;
            this.outgoingWaybillRowService = outgoingWaybillRowService;
            this.expenditureWaybillIndicatorService = expenditureWaybillIndicatorService;
        }

        #endregion

        public Report0005SettingsViewModel Report0005Settings(string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Report0005_View);

                var model = new Report0005SettingsViewModel()
                {
                    BackURL = backURL,
                    Storages = storageService.GetList(user, Permission.Report0005_Storage_List).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name),
                    ArticleName = "Выберите наименование товара",
                    ArticleId = 0,
                    IncomingWaybillName = "Выберите входящую накладную",
                    IncomingWaybillTypeList = ComboBoxBuilder.GetComboBoxItemList<IncomingWaybillType>(sort: false),

                    ReportSourceType_caption1 = "По партиям за период",
                    ReportSourceType_caption2 = "По возвратам за период",
                    ReportSourceType_caption3 = "По указанной накладной",
                };

                return model;
            }
        }

        public Report0005ViewModel Report0005(Report0005SettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                #region Инициализация и проверки

                sourceWaybillsIds = new List<Guid>();
                sourceWaybills = new Dictionary<Guid, BaseWaybillRow>();
                waybillRowsCount = new Dictionary<Guid, int>();
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Report0005_View);
                var articleId = settings.ArticleId;
                var article = articleService.CheckArticleExistence(articleId);
                //Просто начальная инициализация. Реальный тип проверяется дальше
                WaybillType waybillType = WaybillType.WriteoffWaybill;

                var model = new Report0005ViewModel()
                {
                    Settings = settings,
                    CreatedBy = currentUser.DisplayName,
                    Permissions = new Report0005Permissions()
                    {
                        AllowToViewProviders = user.HasPermission(Permission.Provider_List_Details),
                        AllowToViewProducers = user.HasPermission(Permission.Producer_List_Details),
                        AllowToViewClients = user.HasPermission(Permission.Client_List_Details),
                        AllowToViewRecipientAccPrices = user.HasPermission(Permission.AccountingPrice_NotCommandStorage_View),
                        AllowToViewSenderAccPrices = user.HasPermission(Permission.AccountingPrice_NotCommandStorage_View),
                        AllowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForReceipt)
                    }
                };

                permissions = model.Permissions;
                model.ArticleName = article.FullName;

                switch (model.Settings.ReportSourceType)
                {
                    case "1":
                        model.ReportName = String.Format("по партиям за период c {0} по {1}", model.Settings.StartDate, model.Settings.EndDate);
                        break;

                    case "2":
                        model.ReportName = String.Format("по возвратам за период c {0} по {1}", model.Settings.StartDate, model.Settings.EndDate);
                        break;

                    case "3":
                        model.ReportName = String.Format("по указанной накладной");
                        break;
                    default: throw new Exception("Ошибка при определении типа построения отчета.");
                }

                ValidationUtils.Assert(!String.IsNullOrEmpty(settings.StorageIDs) || settings.AllStorages == "1", "Не выбрано ни одного места хранения.");
                // получаем список кодов мест хранения
                Dictionary<short, Storage> storageList;
                if (settings.AllStorages == "1")
                {
                    storageList = storageService.GetList(user, Permission.Report0005_Storage_List).ToDictionary(x => x.Id, x => x);
                    storageIDs = storageList.Select(x => x.Key);
                }
                else
                {
                    storageIDs = model.Settings.StorageIDs.Split('_').Select(x => ValidationUtils.TryGetShort(x));
                    storageList = storageService.CheckStorageListExistence(storageIDs, user, Permission.Report0005_Storage_List);
                }

                var startDate = new DateTime();
                var endDate = new DateTime();

                if (model.Settings.ReportSourceType == "1" || model.Settings.ReportSourceType == "2")
                {
                    startDate = ValidationUtils.TryGetDate(settings.StartDate);
                    endDate = ValidationUtils.TryGetDate(settings.EndDate);

                    ValidationUtils.Assert(startDate < endDate, "Дата начала периода для отчета должна быть меньше даты конца.");
                    ValidationUtils.Assert(endDate <= DateTime.Now.Date, "Дата окончания периода для отчета должна быть меньше или равна текущей дате.");
                    // устанавливаем последнюю секунду указанной даты
                    endDate = endDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                }

                if (model.Settings.ReportSourceType == "3")
                {   //для одной накладной
                    ValidationUtils.Assert(!String.IsNullOrEmpty(settings.IncomingWaybillTypeId), "Не указан тип накладной.");
                    waybillType = (WaybillType)ValidationUtils.TryGetInt(model.Settings.IncomingWaybillTypeId);
                    switch (model.Settings.IncomingWaybillTypeId)
                    {
                        case "1":
                            waybillType = WaybillType.ReceiptWaybill;
                            break;

                        case "2":
                            waybillType = WaybillType.MovementWaybill;
                            break;
                        case "3":
                            waybillType = WaybillType.ChangeOwnerWaybill;
                            break;

                        case "4":
                            waybillType = WaybillType.ReturnFromClientWaybill;
                            break;
                        default: throw new Exception("Ошибка при определении типа исходной накладной.");
                    }
                }

                #endregion

                #region Получение данных


                model.Items = new List<Report0005ItemViewModel>();
                Guid waybillSourseId = new Guid();
                Guid waybillRowSourseId = new Guid();
                var result = new List<Report0005ItemViewModel>();

                //Подгрузка цен
                prices = articlePriceService.GetAccountingPrice(storageIDs, articleId);

                if (model.Settings.ReportSourceType == "3")
                {   //для одной накладной
                    waybillSourseId = ValidationUtils.TryGetGuid(settings.IncomingWaybillId);
                    var waybillRowSourse = incomingWaybillRowService.GetRow(waybillType, waybillSourseId, articleId);
                    ValidationUtils.NotNull(waybillRowSourse, "Накладная не найдена. Возможно, она была удалена.");
                    waybillRowSourseId = waybillRowSourse.Id;

                    var rootItem = new Report0005ItemViewModel();
                    rootItem.WaybillId = waybillRowSourseId;
                    rootItem.SubItems = GetChildNodes(rootItem);

                    waybillRowsCount = sourceWaybillsIds.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
                    //Подгрузка всех позиций
                    sourceWaybills = waybillRowArticleMovementRepository.Query<BaseWaybillRow>()
                    .OneOf(x => x.Id, sourceWaybillsIds.Distinct())
                    .ToList<BaseWaybillRow>().ToDictionary(x => x.Id, x => x);

                    TreeInitialization(rootItem, user);

                    ConvertToPlainStructure(ref result, 0, rootItem);
                    model.Items = result;
                }
                else
                {   //для набора накладных
                    var sourceIds = new List<Guid>();
                    if (model.Settings.ReportSourceType == "1")
                    {   //для приходов
                        var receipt = receiptWaybillService.GetRows(articleId, storageIDs, startDate, endDate, false);
                        sourceIds.AddRange(receipt.Select(x => x.Id));
                    }
                    else if (model.Settings.ReportSourceType == "2")
                    {   //для возвратов
                        var returns = returnFromClientWaybillService.GetRows(articleId, storageIDs, startDate, endDate, false);
                        sourceIds.AddRange(returns.Select(x => x.Id));
                    }
                    var allItems = new List<Report0005ItemViewModel>();
                    foreach (var sourceId in sourceIds)
                    {
                        var rootItem = new Report0005ItemViewModel();
                        rootItem.WaybillId = sourceId;
                        rootItem.SubItems = GetChildNodes(rootItem);

                        allItems.Add(rootItem);
                    }

                    waybillRowsCount = sourceWaybillsIds.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
                    //Подгрузка всех позиций
                    sourceWaybills = waybillRowArticleMovementRepository.Query<BaseWaybillRow>()
                    .OneOf(x => x.Id, sourceWaybillsIds.Distinct())
                    .ToList<BaseWaybillRow>().ToDictionary(x => x.Id, x => x);

                    //Цикл инициализации дерева
                    foreach (var items in allItems)
                    {
                        TreeInitialization(items, user);
                    }

                    //Вывод дерева с сортировками (на данном этапе в дереве уже есть даты)
                    foreach (var items in allItems.OrderByDescending(x => x.Date))
                    {
                        ConvertToPlainStructure(ref result, 0, items);
                    }
                }

                model.Items = result;


                #endregion

                #region Чистим память

                sourceWaybills = null;
                waybillRowsCount = null;
                sourceWaybillsIds = null;
                prices = null;
                permissions = null;
                storageIDs = null;

                #endregion

                return model;
            }
        }

        #region Рекурсивная работа с деревьями

        /// <summary>
        /// Рекурсивная функция получения плоского списка с указанием уровня вложенности
        /// </summary>
        /// <param name="result">Результирующий плосский список</param>
        /// <param name="level">Уровень вложенности</param>
        /// <param name="rootItem">Текущий корневой элемент</param>
        private void ConvertToPlainStructure(ref List<Report0005ItemViewModel> result, int level, Report0005ItemViewModel rootItem)
        {
            var throwLevel = 0;
            rootItem.ItemLevel = level;
            result.Add(rootItem);
            foreach (var subItem in rootItem.SubItems.OrderByDescending(x => x.Date))
            {
                throwLevel = level;
                if (!subItem.HiddenWaybill)
                {//Уловка, для реализации "визуальных" уровней в отчете. То есть, скрываются скрытые уровни движения товара.
                    ++throwLevel;
                }

                ConvertToPlainStructure(ref result, throwLevel, subItem);
            }
        }

        /// <summary>
        /// Рекурсивная функция инициализации
        /// </summary>
        private void TreeInitialization(Report0005ItemViewModel result, User user)
        {
            ConvertToTreeNode(result, user);
            foreach (var subItem in result.SubItems)
            {
                TreeInitialization(subItem, user);
                if (result.HiddenWaybill)
                {//Колдунство для скрытия и маркировки пунктов
                    if (subItem.WaybillType.ContainsIn(WaybillType.WriteoffWaybill, WaybillType.ExpenditureWaybill))
                    {
                        subItem.HiddenWaybill = true;
                    }
                    else
                    {
                        if (!subItem.HiddenWaybill)
                        {
                            subItem.MarkedWaybill = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Получить дочерние узлы
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private IEnumerable<Report0005ItemViewModel> GetChildNodes(Report0005ItemViewModel item)
        {
            sourceWaybillsIds.Add(item.WaybillId);
            var result = new List<Report0005ItemViewModel>();

            var waybillArticleMovementsSubQuery = waybillRowArticleMovementRepository.SubQuery<WaybillRowArticleMovement>()
                .Where(x => x.SourceWaybillRowId == item.WaybillId)
                .Select(x => x.Id);

            var waybillArticleMovementsDestQueryIds = waybillRowArticleMovementRepository.Query<WaybillRowArticleMovement>()
                .PropertyIn(x => x.Id, waybillArticleMovementsSubQuery)
                .Select(x => x.DestinationWaybillRowId)
                .ToList<Guid>().ToList();

            if (waybillArticleMovementsDestQueryIds.Count > 0)
            {   //Если есть записи в товародвижении - то накладная не реализации
                foreach (var waybillRowId in waybillArticleMovementsDestQueryIds)
                {
                    var subItem = new Report0005ItemViewModel();
                    subItem.WaybillId = waybillRowId;

                    subItem.SubItems = GetChildNodes(subItem);
                    result.Add(subItem);
                }
            }
            else
            {   //есть вероятность - что исходящих накладнных по товародвижению нет потому, что это накладная реализации, а у ней нужно поискать возвраты
                var returnFromClientWaybillRows = waybillRowArticleMovementRepository.Query<ReturnFromClientWaybillRow>()
                .Where(x => x.SaleWaybillRow.Id == item.WaybillId)
                .Select(x => x.Id)
                .ToList<Guid>().ToList();

                foreach (var waybillRowId in returnFromClientWaybillRows)
                {
                    var subItem = new Report0005ItemViewModel();
                    subItem.WaybillId = waybillRowId;

                    subItem.SubItems = GetChildNodes(subItem);
                    result.Add(subItem);
                }
            }

            return result;
        }

        #endregion

        #region Работа с преобразованием накладных

        /// <summary>
        /// Создание узла дерева из позиции накладной
        /// </summary>
        /// <param name="waybillRow"></param>
        /// <returns></returns>
        private void ConvertToTreeNode(Report0005ItemViewModel subItem, User user)
        {
            var waybillId = subItem.WaybillId;
            ValidationUtils.Assert(sourceWaybills.ContainsKey(waybillId), "Накладная не найдена.");
            var waybillRow = sourceWaybills[waybillId];

            //Используется ли позиция больше одного раза
            if (waybillRowsCount.ContainsKey(subItem.WaybillId))
            {
                if (waybillRowsCount[subItem.WaybillId] > 1)
                {
                    subItem.IsUsedMoreThenOnce = true;
                }
            }

            if (waybillRow.Is<ReceiptWaybillRow>())
            {
                var receiptWaybillRow = waybillRow.As<ReceiptWaybillRow>();
                var incomingWaybillRow = incomingWaybillRowService.ConvertToIncomingWaybillRow(waybillRow);
                InitializingTreeNode(subItem, incomingWaybillRow);

                subItem.WaybillTypeName = "Приход";
                subItem.WaybillName = receiptWaybillRow.ReceiptWaybill.Name;
                subItem.WaybillStateName = receiptWaybillRow.ReceiptWaybill.State.GetDisplayName();
                subItem.WaybillType = WaybillType.ReceiptWaybill;
                subItem.Date = receiptWaybillRow.ReceiptWaybill.Date;
                subItem.PurchaseCost = permissions.AllowToViewPurchaseCosts ? receiptWaybillRow.PurchaseCost.ForDisplay(ValueDisplayType.Money) + " р." : "---";

                subItem.RecipientAccountingPrice = ResolveAccountingPrice(receiptWaybillRow.RecipientArticleAccountingPrice, receiptWaybillRow.ReceiptWaybill.ReceiptStorage, subItem.Date, user);
            }

            if (waybillRow.Is<ReturnFromClientWaybillRow>())
            {
                var returnFromClientWaybillRow = waybillRow.As<ReturnFromClientWaybillRow>();
                var incomingWaybillRow = incomingWaybillRowService.ConvertToIncomingWaybillRow(waybillRow);
                InitializingTreeNode(subItem, incomingWaybillRow);

                subItem.WaybillTypeName = "Возврат от клиента";
                subItem.WaybillName = returnFromClientWaybillRow.ReturnFromClientWaybill.Name;
                subItem.WaybillStateName = returnFromClientWaybillRow.ReturnFromClientWaybill.State.GetDisplayName();
                subItem.WaybillType = WaybillType.ReturnFromClientWaybill;
                subItem.Date = returnFromClientWaybillRow.ReturnFromClientWaybill.Date;
                subItem.ClientName = permissions.AllowToViewClients ? returnFromClientWaybillRow.ReturnFromClientWaybill.Client.Name : "---";

                subItem.RecipientAccountingPrice = ResolveAccountingPrice(returnFromClientWaybillRow.ArticleAccountingPrice, returnFromClientWaybillRow.ReturnFromClientWaybill.RecipientStorage, subItem.Date, user);

                subItem.SalePrice = (returnFromClientWaybillRow.SalePrice ?? 0).ForDisplay(ValueDisplayType.Money) + " р.";
            }

            if (waybillRow.Is<ChangeOwnerWaybillRow>())
            {
                var changeOwnerWaybillRow = waybillRow.As<ChangeOwnerWaybillRow>();
                var incomingWaybillRow = incomingWaybillRowService.ConvertToIncomingWaybillRow(waybillRow);
                var outgoingWaybillRow = outgoingWaybillRowService.ConvertToOutgoingWaybillRow(changeOwnerWaybillRow);
                InitializingTreeNode(subItem, incomingWaybillRow, outgoingWaybillRow);

                subItem.WaybillTypeName = "Смена собственника";
                subItem.WaybillName = changeOwnerWaybillRow.ChangeOwnerWaybill.Name;
                subItem.WaybillStateName = changeOwnerWaybillRow.ChangeOwnerWaybill.State.GetDisplayName();
                subItem.WaybillType = WaybillType.ChangeOwnerWaybill;
                subItem.Date = changeOwnerWaybillRow.ChangeOwnerWaybill.Date;

                subItem.SenderAccountingPrice = ResolveAccountingPrice(changeOwnerWaybillRow.ArticleAccountingPrice, changeOwnerWaybillRow.ChangeOwnerWaybill.Storage, subItem.Date, user);
            }

            if (waybillRow.Is<MovementWaybillRow>())
            {
                var movementWaybillRow = waybillRow.As<MovementWaybillRow>();
                var incomingWaybillRow = incomingWaybillRowService.ConvertToIncomingWaybillRow(waybillRow);
                var outgoingWaybillRow = outgoingWaybillRowService.ConvertToOutgoingWaybillRow(movementWaybillRow);
                InitializingTreeNode(subItem, incomingWaybillRow, outgoingWaybillRow);

                subItem.WaybillTypeName = "Внутреннее перемещение";
                subItem.WaybillName = movementWaybillRow.MovementWaybill.Name;
                subItem.WaybillStateName = movementWaybillRow.MovementWaybill.State.GetDisplayName();
                subItem.WaybillType = WaybillType.MovementWaybill;
                subItem.Date = movementWaybillRow.MovementWaybill.Date;

                subItem.RecipientAccountingPrice = ResolveAccountingPrice(movementWaybillRow.RecipientArticleAccountingPrice, movementWaybillRow.MovementWaybill.RecipientStorage, subItem.Date, user);

                subItem.SenderAccountingPrice = ResolveAccountingPrice(movementWaybillRow.SenderArticleAccountingPrice, movementWaybillRow.MovementWaybill.SenderStorage, subItem.Date, user);
            }

            if (waybillRow.Is<WriteoffWaybillRow>())
            {
                var writeoffWaybillRow = waybillRow.As<WriteoffWaybillRow>();
                var outgoingWaybillRow = outgoingWaybillRowService.ConvertToOutgoingWaybillRow(writeoffWaybillRow);
                InitializingTreeNode(subItem, outgoingWaybillRow);

                subItem.WaybillTypeName = "Списание";
                subItem.WaybillName = writeoffWaybillRow.WriteoffWaybill.Name;
                subItem.WaybillStateName = writeoffWaybillRow.WriteoffWaybill.State.GetDisplayName();
                subItem.WaybillType = WaybillType.WriteoffWaybill;
                subItem.Date = writeoffWaybillRow.WriteoffWaybill.Date;

                subItem.SenderAccountingPrice = ResolveAccountingPrice(writeoffWaybillRow.SenderArticleAccountingPrice, writeoffWaybillRow.WriteoffWaybill.SenderStorage, subItem.Date, user);

                subItem.Reason = writeoffWaybillRow.WriteoffWaybill.WriteoffReason.Name;
            }

            if (waybillRow.Is<ExpenditureWaybillRow>())
            {
                var expenditureWaybillRow = waybillRow.As<ExpenditureWaybillRow>();
                var outgoingWaybillRow = outgoingWaybillRowService.ConvertToOutgoingWaybillRow(expenditureWaybillRow);
                InitializingTreeNode(subItem, outgoingWaybillRow);

                subItem.WaybillTypeName = "Реализация товара";
                subItem.WaybillName = expenditureWaybillRow.ExpenditureWaybill.Name;
                subItem.WaybillStateName = expenditureWaybillRow.ExpenditureWaybill.State.GetDisplayName();
                subItem.WaybillType = WaybillType.ExpenditureWaybill;
                subItem.Date = expenditureWaybillRow.ExpenditureWaybill.Date;

                subItem.SenderAccountingPrice = ResolveAccountingPrice(expenditureWaybillRow.SenderArticleAccountingPrice, expenditureWaybillRow.ExpenditureWaybill.SenderStorage, subItem.Date, user);

                subItem.SalePrice = expenditureWaybillRow.SalePrice.HasValue ?
                    expenditureWaybillRow.SalePrice.ForDisplay(ValueDisplayType.Money) + " р." : "---";
                subItem.ClientName = permissions.AllowToViewClients ? expenditureWaybillRow.ExpenditureWaybill.Deal.Client.Name : "---";

                subItem.ReturnedCount = expenditureWaybillRow.ReservedByReturnCount.ForDisplay();
            }
        }

        /// <summary>
        /// Инициализация части общих значения для узла дерева для накладных прихода
        /// </summary>
        /// <param name="item"></param>
        /// <param name="incomingWaybillRow"></param>
        private void InitializingTreeNode(Report0005ItemViewModel item, IncomingWaybillRow incomingWaybillRow, OutgoingWaybillRow outgoingWaybillRow = null)
        {
            bool isCreatedFromProductionOrderBatch = incomingWaybillRow.Batch.ReceiptWaybill.IsCreatedFromProductionOrderBatch;

            item.BatchName = incomingWaybillRow.Batch.BatchName;
            item.IsCreatedFromProductionOrderBatch = isCreatedFromProductionOrderBatch;
            item.ContractorName = isCreatedFromProductionOrderBatch && permissions.AllowToViewProducers ||
                !isCreatedFromProductionOrderBatch && permissions.AllowToViewProviders ? incomingWaybillRow.Batch.ReceiptWaybill.ContractorName : "---";
            item.RecipientName = incomingWaybillRow.Recipient.ShortName;
            item.RecipientStorageName = incomingWaybillRow.RecipientStorage.Name;
            item.RecipientStorageId = incomingWaybillRow.RecipientStorage.Id;
            item.Count = incomingWaybillRow.Count.ForDisplay();
            item.ReservedCount = (incomingWaybillRow.AcceptedCount + incomingWaybillRow.ShippedCount)
                .ForDisplay();
            item.ShippedCount = incomingWaybillRow.FinallyMovedCount.ForDisplay();
            item.RemainCount = (incomingWaybillRow.Count -
                (incomingWaybillRow.AcceptedCount + incomingWaybillRow.ShippedCount + incomingWaybillRow.FinallyMovedCount))
                .ForDisplay();
            if (outgoingWaybillRow != null)
            {
                item.SenderName = outgoingWaybillRow.Sender.ShortName;
                item.SenderStorageName = outgoingWaybillRow.SenderStorage.Name;
                item.SenderStorageId = outgoingWaybillRow.SenderStorage.Id;
            }

            if (outgoingWaybillRow == null)
            {
                if (!storageIDs.Contains(item.RecipientStorageId))
                {
                    item.HiddenWaybill = true;
                }
            }
            else
            {
                if (!storageIDs.Contains(item.RecipientStorageId) && !storageIDs.Contains(item.SenderStorageId))
                {
                    item.HiddenWaybill = true;
                }
            }
        }

        /// <summary>
        /// Инициализация части общих значения для узла дерева для накладных ухода
        /// </summary>
        /// <param name="item"></param>
        /// <param name="outgoingWaybillRow"></param>
        private void InitializingTreeNode(Report0005ItemViewModel item, OutgoingWaybillRow outgoingWaybillRow)
        {
            item.BatchName = outgoingWaybillRow.Batch.BatchName;
            item.Count = outgoingWaybillRow.Count.ForDisplay();

            item.SenderName = outgoingWaybillRow.Sender.ShortName;
            item.SenderStorageName = outgoingWaybillRow.SenderStorage.Name;
            item.SenderStorageId = outgoingWaybillRow.SenderStorage.Id;
            if (!storageIDs.Contains(item.SenderStorageId))
            {
                item.HiddenWaybill = true;
            }
        }

        #endregion

        #region Вспомогательные методы - хелперы

        /// <summary>
        /// Хэлпер вывода УЦ
        /// </summary>
        /// <param name="accountingPrice">УЦ</param>
        /// <param name="storage">Склад для поиска УЦ, если уц нет в позиции</param>
        /// <param name="date">Дата</param>
        /// <param name="user">Пользователь</param>
        /// 
        /// <returns></returns>
        private string ResolveAccountingPrice(ArticleAccountingPrice accountingPrice, Storage storage, DateTime date, User user)
        {
            bool permission = user.HasPermissionToViewStorageAccountingPrices(storage);

            string result;
            if (permission)
            {
                if (accountingPrice != null)
                {
                    result = accountingPrice.AccountingPrice.ForDisplay(ValueDisplayType.Money) + " р.";
                }
                else
                {
                    result = (prices[storage.Id] ?? 0).ForDisplay(ValueDisplayType.Money) + " р.";
                }
            }
            else
            {
                result = "---";
            }

            return result;
        }

        #endregion
    }
}