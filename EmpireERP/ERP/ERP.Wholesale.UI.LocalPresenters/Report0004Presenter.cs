using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Report.Report0004;
using OfficeOpenXml;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class Report0004Presenter : BaseReportPresenter, IReport0004Presenter 
    {
        #region Поля

        private readonly IStorageService storageService;
        private readonly IArticleService articleService;
        private readonly IReceiptWaybillService receiptWaybillService;
        private readonly IMovementWaybillService movementWaybillService;
        private readonly IExpenditureWaybillService expenditureWaybillService;
        private readonly IChangeOwnerWaybillService changeOwnerWaybillService;
        private readonly IWriteoffWaybillService writeoffWaybillService;
        private readonly IReturnFromClientWaybillService returnFromClientWaybillService;
        private readonly IArticleAvailabilityService articleAvailabilityService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly IArticlePriceService articlePriceService;

        #endregion

        #region Конструкторы

        public Report0004Presenter(IUnitOfWorkFactory unitOfWorkFactory, IUserService userService, IStorageService storageService, IArticleService articleService, IReceiptWaybillService receiptWaybillService,
        IMovementWaybillService movementWaybillService, IExpenditureWaybillService expenditureWaybillService, IChangeOwnerWaybillService changeOwnerWaybillService,
        IWriteoffWaybillService writeoffWaybillService, IReturnFromClientWaybillService returnFromClientWaybillService,
        IArticleAvailabilityService articleAvailabilityService,
        IAccountOrganizationService accountOrganizationService, IArticlePriceService articlePriceService) :
            base(unitOfWorkFactory, userService)
        {
            this.storageService = storageService;
            this.articleService = articleService;
            this.accountOrganizationService = accountOrganizationService;

            this.receiptWaybillService = receiptWaybillService;
            this.movementWaybillService = movementWaybillService;
            this.expenditureWaybillService = expenditureWaybillService;
            this.changeOwnerWaybillService = changeOwnerWaybillService;
            this.writeoffWaybillService = writeoffWaybillService;
            this.returnFromClientWaybillService = returnFromClientWaybillService;            
            this.articleAvailabilityService = articleAvailabilityService;
            this.articlePriceService = articlePriceService;
        }

        #endregion


        public Report0004SettingsViewModel Report0004Settings(string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Report0004_View);

                var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForReceipt);
                var allowToViewRecipientAccPrices = user.HasPermission(Permission.AccountingPrice_NotCommandStorage_View);
                var allowToViewSenderAccPrices = user.HasPermission(Permission.AccountingPrice_NotCommandStorage_View);

                var model = new Report0004SettingsViewModel()
                {
                    BackURL = backURL,
                    Storages = storageService.GetList(user, Permission.Report0004_Storage_List).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name),
                    AllowToViewPurchaseCost = allowToViewPurchaseCost,
                    AllowToViewSenderAccountingPrices = allowToViewSenderAccPrices,
                    AllowToViewRecipientAccountingPrices = allowToViewRecipientAccPrices,
                    ArticleName = "Выберите наименование товара"
                };

                return model;
            }
        }

        public Report0004ViewModel Report0004(Report0004SettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Report0004_View);

                var articleId = settings.ArticleId;
                var article = articleService.CheckArticleExistence(articleId);

                var currentDate = DateTimeUtils.GetCurrentDateTime();

                var model = new Report0004ViewModel()
                {
                    Settings = settings,
                    CreatedBy = currentUser.DisplayName
                };

                var allowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForReceipt);
                var allowToViewRecipientAccPrices = user.HasPermission(Permission.AccountingPrice_NotCommandStorage_View);
                var allowToViewSenderAccPrices = user.HasPermission(Permission.AccountingPrice_NotCommandStorage_View);
                var allowToViewProducers = user.HasPermission(Permission.Producer_List_Details);
                var allowToViewProviders = user.HasPermission(Permission.Provider_List_Details);

                model.AllowToViewContractors = allowToViewProducers || allowToViewProviders;
                model.AllowToViewProviders = allowToViewProviders;
                model.AllowToViewClients = user.HasPermission(Permission.Client_List_Details);

                settings.ShowRecipientAccountingPrices =
                    (settings.ShowRecipientAccountingPrices == "1" && allowToViewRecipientAccPrices) ? "1" : "0";

                settings.ShowSenderAccountingPrices =
                    ((settings.ShowSenderAccountingPrices == "1") && allowToViewSenderAccPrices) ? "1" : "0";

                settings.ShowPurchaseCosts =
                    ((settings.ShowPurchaseCosts == "1") && allowToViewPurchaseCosts) ? "1" : "0";

                #region Проверки

                ValidationUtils.Assert(!String.IsNullOrEmpty(settings.StorageIDs) || settings.AllStorages == "1", "Не выбрано ни одного места хранения.");

                DateTime startDate;
                DateTime endDate;

                ParseDatePeriod(settings.StartDate, settings.EndDate, currentDate, out startDate, out endDate);

                #endregion

                model.ArticleName = article.FullName;

                var showOnlyExactAvailability = settings.ShowOnlyExactAvailability == "1";

                var quantityByOrganization = new Dictionary<int, StartEndAvailabilityItem>();
                var quantityByStorage = new Dictionary<short, StartEndAvailabilityItem>();

                Dictionary<short, Storage> storageList;
                IEnumerable<short> storageIds;
                if (settings.AllStorages == "1")
                {
                    storageList = storageService.GetList(user, Permission.Report0004_Storage_List).ToDictionary(x => x.Id, x => x);
                    storageIds = storageList.Select(x => x.Key);
                }
                else
                {
                    storageIds = model.Settings.StorageIDs.Split('_').Select(x => ValidationUtils.TryGetShort(x));
                    storageList = storageService.CheckStorageListExistence(storageIds, user, Permission.Report0004_Storage_List);
                }

                
                if (showOnlyExactAvailability)
                {
                    var startList = articleAvailabilityService.GetExactArticleAvailability(storageIds, articleId, startDate);
                    var endList = articleAvailabilityService.GetExactArticleAvailability(storageIds, articleId, endDate);

                    foreach (var item in startList)
                    {
                        if (quantityByOrganization.ContainsKey(item.AccountOrganizationId))
                        {
                            quantityByOrganization[item.AccountOrganizationId].StartCount += item.Count;
                        }
                        else
                        {
                            quantityByOrganization.Add(item.AccountOrganizationId, new StartEndAvailabilityItem() { StartCount = item.Count });
                        }

                        if (quantityByStorage.ContainsKey(item.StorageId))
                        {
                            quantityByStorage[item.StorageId].StartCount += item.Count;
                        }
                        else
                        {
                            quantityByStorage.Add(item.StorageId, new StartEndAvailabilityItem() { StartCount = item.Count });
                        }
                    }

                    foreach (var item in endList)
                    {
                        if (quantityByOrganization.ContainsKey(item.AccountOrganizationId))
                        {
                            quantityByOrganization[item.AccountOrganizationId].EndCount += item.Count;
                        }
                        else
                        {
                            quantityByOrganization.Add(item.AccountOrganizationId, new StartEndAvailabilityItem() { EndCount = item.Count });
                        }

                        if (quantityByStorage.ContainsKey(item.StorageId))
                        {
                            quantityByStorage[item.StorageId].EndCount += item.Count;
                        }
                        else
                        {
                            quantityByStorage.Add(item.StorageId, new StartEndAvailabilityItem() { EndCount = item.Count });
                        }
                    }
                }
                else
                {
                    var startList = articleAvailabilityService.GetExtendedArticleAvailability(article.Id, storageList.Values.Select(x => x.Id), startDate).ToList();
                    var endList = articleAvailabilityService.GetExtendedArticleAvailability(article.Id, storageList.Values.Select(x => x.Id), endDate);

                    foreach (var item in startList)
                    {
                        if (quantityByOrganization.ContainsKey(item.AccountOrganizationId))
                        {
                            quantityByOrganization[item.AccountOrganizationId].StartCount += item.Count;
                        }
                        else
                        {
                            quantityByOrganization.Add(item.AccountOrganizationId, new StartEndAvailabilityItem() { StartCount = item.Count });
                        }

                        if (quantityByStorage.ContainsKey(item.StorageId))
                        {
                            quantityByStorage[item.StorageId].StartCount += item.Count;
                        }
                        else
                        {
                            quantityByStorage.Add(item.StorageId, new StartEndAvailabilityItem() { StartCount = item.Count });
                        }
                    }

                    foreach (var item in endList)
                    {
                        if (quantityByOrganization.ContainsKey(item.AccountOrganizationId))
                        {
                            quantityByOrganization[item.AccountOrganizationId].EndCount += item.Count;
                        }
                        else
                        {
                            quantityByOrganization.Add(item.AccountOrganizationId, new StartEndAvailabilityItem() { EndCount = item.Count });
                        }

                        if (quantityByStorage.ContainsKey(item.StorageId))
                        {
                            quantityByStorage[item.StorageId].EndCount += item.Count;
                        }
                        else
                        {
                            quantityByStorage.Add(item.StorageId, new StartEndAvailabilityItem() { EndCount = item.Count });
                        }
                    }
                }

                var storagesWithAvailabilityList = quantityByStorage.Keys.ToDictionary(x => x, x => storageList[x]);
                var accountOrganizationsWithAvailabilityList = accountOrganizationService.GetList(quantityByOrganization.Keys);

                var quantityByStorageList = quantityByStorage
                    .OrderBy(x => storagesWithAvailabilityList[x.Key].Type).ThenBy(x => storagesWithAvailabilityList[x.Key].Name);
                var quantityByOrganizationList = quantityByOrganization
                    .OrderBy(x => accountOrganizationsWithAvailabilityList[x.Key].ShortName);

                model.StartQuantityGroupByStorage.Items = quantityByStorageList.Where(x => model.Settings.ShowEndQuantityByStorage == "1" || x.Value.StartCount > 0)
                    .Select(x => new Report0004QuantityTableItemViewModel()
                    {
                        IndicatorName = storagesWithAvailabilityList[x.Key].Name,
                        Quantity = x.Value.StartCount
                    });
                model.StartQuantityGroupByStorage.FirstColumnName = "Место хранения";

                model.EndQuantityGroupByStorage.Items = quantityByStorageList.Where(x => model.Settings.ShowStartQuantityByStorage == "1" || x.Value.EndCount > 0)
                    .Select(x => new Report0004QuantityTableItemViewModel()
                    {
                        IndicatorName = storagesWithAvailabilityList[x.Key].Name,
                        Quantity = x.Value.EndCount
                    });
                model.EndQuantityGroupByStorage.FirstColumnName = "Место хранения";

                model.StartQuantityGroupByOrganization.Items = quantityByOrganizationList.Where(x => model.Settings.ShowEndQuantityByOrganization == "1" || x.Value.StartCount > 0)
                   .Select(x => new Report0004QuantityTableItemViewModel()
                   {
                       IndicatorName = accountOrganizationsWithAvailabilityList[x.Key].ShortName,
                       Quantity = x.Value.StartCount
                   });
                model.StartQuantityGroupByOrganization.FirstColumnName = "Организация";

                model.EndQuantityGroupByOrganization.Items = quantityByOrganizationList.Where(x => model.Settings.ShowStartQuantityByOrganization == "1" || x.Value.EndCount > 0)
                    .Select(x => new Report0004QuantityTableItemViewModel()
                    {
                        IndicatorName = accountOrganizationsWithAvailabilityList[x.Key].ShortName,
                        Quantity = x.Value.EndCount
                    });
                model.EndQuantityGroupByOrganization.FirstColumnName = "Организация";

                var receiptWaybillRows = receiptWaybillService.GetRows(articleId, storageIds, startDate, endDate, showOnlyExactAvailability);
                var movementRows = movementWaybillService.GetRows(articleId, storageIds, startDate, endDate, showOnlyExactAvailability);
                var expenditureWaybillRows = expenditureWaybillService.GetRows(articleId, storageIds, startDate, endDate, showOnlyExactAvailability);
                var writeoffWaybillRows = writeoffWaybillService.GetRows(articleId, storageIds, startDate, endDate, showOnlyExactAvailability);
                var returnFromClientWaybillRows = returnFromClientWaybillService.GetRows(articleId, storageIds, startDate, endDate, showOnlyExactAvailability);
                var changeOwnerWaybillRows = changeOwnerWaybillService.GetRows(articleId, storageIds, startDate, endDate, showOnlyExactAvailability);

                var divergentReceiptWaybillRows = receiptWaybillService.GetDivergenceRows(articleId, storageIds, startDate, endDate, showOnlyExactAvailability);

                model.ReceiptWaybillRows = receiptWaybillRows.OrderBy(x => x.ReceiptWaybill.Date).ThenBy(x => x.ReceiptWaybill.CreationDate).Select(
                    x => new Report0004ItemViewModel
                    {
                        WaybillId = x.ReceiptWaybill.Id,
                        Number = x.ReceiptWaybill.Number,
                        Date = x.ReceiptWaybill.Date,
                        StateName = x.ReceiptWaybill.State.GetDisplayName(),
                        Count = x.CurrentCount,
                        RecipientStorage = x.ReceiptWaybill.ReceiptStorage.Name,
                        Recipient = x.ReceiptWaybill.AccountOrganization.ShortName,
                        Contractor = x.ReceiptWaybill.IsCreatedFromProductionOrderBatch && allowToViewProducers ||
                            !x.ReceiptWaybill.IsCreatedFromProductionOrderBatch && allowToViewProviders ?
                            x.ReceiptWaybill.ContractorName : "---",
                        PurchaseCost = x.PurchaseCost,
                        RecipientAccountingPrice = x.ReceiptWaybill.IsAccepted && user.HasPermissionToViewStorageAccountingPrices(x.ReceiptWaybill.ReceiptStorage) ?
                            x.RecipientArticleAccountingPrice.AccountingPrice : (decimal?)null
                    }
                );

                model.MovementAndChangeOwnerWaybillRows = movementRows.Select(
                    x => new Report0004ItemViewModel
                    {
                        WaybillId = x.MovementWaybill.Id,
                        Number = x.MovementWaybill.Number,
                        Date = x.MovementWaybill.Date,
                        CreationDate = x.MovementWaybill.CreationDate,
                        StateName = x.MovementWaybill.State.GetDisplayName(),
                        Count = x.MovingCount,
                        SenderStorage = x.MovementWaybill.SenderStorage.Name,
                        Sender = x.MovementWaybill.Sender.ShortName,
                        RecipientStorage = x.MovementWaybill.RecipientStorage.Name,
                        Recipient = x.MovementWaybill.Recipient.ShortName,
                        BatchName = x.ReceiptWaybillRow.BatchName,
                        SenderAccountingPrice = x.MovementWaybill.IsAccepted && user.HasPermissionToViewStorageAccountingPrices(x.MovementWaybill.SenderStorage) ?
                            x.SenderArticleAccountingPrice.AccountingPrice : (decimal?)null,
                        RecipientAccountingPrice = x.MovementWaybill.IsAccepted && user.HasPermissionToViewStorageAccountingPrices(x.MovementWaybill.RecipientStorage) ?
                            x.RecipientArticleAccountingPrice.AccountingPrice : (decimal?)null
                    }
                ).Concat(changeOwnerWaybillRows.Select(
                    x => new Report0004ItemViewModel
                    {
                        WaybillId = x.ChangeOwnerWaybill.Id,
                        Number = x.ChangeOwnerWaybill.Number,
                        Date = x.ChangeOwnerWaybill.Date,
                        CreationDate = x.ChangeOwnerWaybill.CreationDate,
                        StateName = x.ChangeOwnerWaybill.State.GetDisplayName(),
                        Count = x.MovingCount,
                        SenderStorage = x.ChangeOwnerWaybill.Storage.Name,
                        Sender = x.ChangeOwnerWaybill.Sender.ShortName,
                        RecipientStorage = x.ChangeOwnerWaybill.Storage.Name,
                        Recipient = x.ChangeOwnerWaybill.Recipient.ShortName,
                        BatchName = x.ReceiptWaybillRow.BatchName,
                        SenderAccountingPrice = x.ChangeOwnerWaybill.IsAccepted && user.HasPermissionToViewStorageAccountingPrices(x.ChangeOwnerWaybill.Storage) ?
                            x.ArticleAccountingPrice.AccountingPrice : (decimal?)null,
                        RecipientAccountingPrice = x.ChangeOwnerWaybill.IsAccepted && user.HasPermissionToViewStorageAccountingPrices(x.ChangeOwnerWaybill.Storage) ?
                            x.ArticleAccountingPrice.AccountingPrice : (decimal?)null
                    }
                )).OrderBy(x => x.Date).ThenBy(x => x.CreationDate);

                model.ExpenditureWaybillRows = expenditureWaybillRows.OrderBy(x => x.ExpenditureWaybill.Date).ThenBy(x => x.ExpenditureWaybill.CreationDate).Select(
                    x => new Report0004ItemViewModel
                    {
                        WaybillId = x.ExpenditureWaybill.Id,
                        Number = x.ExpenditureWaybill.Number,
                        Date = x.ExpenditureWaybill.Date,
                        StateName = x.ExpenditureWaybill.State.GetDisplayName(),
                        Count = x.SellingCount,
                        SalePrice = x.SalePrice.HasValue ?
                            x.SalePrice : (decimal?)null,
                        SenderStorage = x.ExpenditureWaybill.SenderStorage.Name,
                        Sender = x.ExpenditureWaybill.Sender.ShortName,
                        ClientName = x.ExpenditureWaybill.Deal.Client.Name,
                        BatchName = x.ReceiptWaybillRow.BatchName,
                        SenderAccountingPrice = x.ExpenditureWaybill.IsAccepted && user.HasPermissionToViewStorageAccountingPrices(x.ExpenditureWaybill.SenderStorage) ?
                            x.SenderArticleAccountingPrice.AccountingPrice : (decimal?)null
                    }
                );

                model.WriteoffWaybillRows = writeoffWaybillRows.OrderBy(x => x.WriteoffWaybill.Date).ThenBy(x => x.WriteoffWaybill.CreationDate).Select(
                    x => new Report0004ItemViewModel
                    {
                        WaybillId = x.WriteoffWaybill.Id,
                        Number = x.WriteoffWaybill.Number,
                        Date = x.WriteoffWaybill.Date,
                        StateName = x.WriteoffWaybill.State.GetDisplayName(),
                        Count = x.WritingoffCount,
                        SenderStorage = x.WriteoffWaybill.SenderStorage.Name,
                        Sender = x.WriteoffWaybill.Sender.ShortName,
                        Reason = x.WriteoffWaybill.WriteoffReason.Name,
                        BatchName = x.ReceiptWaybillRow.BatchName,
                        SenderAccountingPrice = x.WriteoffWaybill.IsAccepted && user.HasPermissionToViewStorageAccountingPrices(x.WriteoffWaybill.SenderStorage) ?
                            x.SenderArticleAccountingPrice.AccountingPrice : (decimal?)null
                    }
                );

                model.ReturnFromClientWaybillRows = returnFromClientWaybillRows.OrderBy(x => x.ReturnFromClientWaybill.Date).ThenBy(x => x.ReturnFromClientWaybill.CreationDate).Select(
                    x => new Report0004ItemViewModel
                    {
                        WaybillId = x.ReturnFromClientWaybill.Id,
                        Number = x.ReturnFromClientWaybill.Number,
                        Date = x.ReturnFromClientWaybill.Date,
                        StateName = x.ReturnFromClientWaybill.State.GetDisplayName(),
                        Count = x.ReturnCount,
                        RecipientStorage = x.ReturnFromClientWaybill.RecipientStorage.Name,
                        Recipient = x.ReturnFromClientWaybill.Recipient.ShortName,
                        ClientName = x.ReturnFromClientWaybill.Client.Name,
                        Reason = x.ReturnFromClientWaybill.ReturnFromClientReason.Name,
                        BatchName = x.ReceiptWaybillRow.BatchName,
                        RecipientAccountingPrice = x.ReturnFromClientWaybill.IsAccepted && user.HasPermissionToViewStorageAccountingPrices(x.ReturnFromClientWaybill.RecipientStorage) ?
                            x.ArticleAccountingPrice.AccountingPrice : (decimal?)null
                    }
                );

                model.ReceiptDivergences = divergentReceiptWaybillRows.OrderBy(x => x.ReceiptWaybill.Date).ThenBy(x => x.ReceiptWaybill.CreationDate).Select(
                    x => new Report0004ItemViewModel
                    {
                        WaybillId = x.ReceiptWaybill.Id,
                        Number = x.ReceiptWaybill.Number,
                        Date = x.ReceiptWaybill.Date,
                        StateName = x.ReceiptWaybill.State.GetDisplayName(),
                        Count = x.ApprovedCount.Value - x.PendingCount,
                        RecipientStorage = x.ReceiptWaybill.ReceiptStorage.Name,
                        Recipient = x.ReceiptWaybill.AccountOrganization.ShortName,
                        Contractor = allowToViewProviders ? x.ReceiptWaybill.ContractorName : "---",
                        PurchaseCost = x.PurchaseCost,
                        RecipientAccountingPrice = x.ReceiptWaybill.IsAccepted && user.HasPermissionToViewStorageAccountingPrices(x.ReceiptWaybill.ReceiptStorage) ?
                            x.RecipientArticleAccountingPrice.AccountingPrice : (decimal?)null
                    }
                );

                if (model.Settings.ShowBatches == "0")
                {
                    model.MovementAndChangeOwnerWaybillRows = model.MovementAndChangeOwnerWaybillRows.GroupBy(x => x.WaybillId, (x, y) => { var item = y.First(); item.Count = y.Sum(z => z.Count); return item; });

                    model.ExpenditureWaybillRows = model.ExpenditureWaybillRows.GroupBy(x => x.WaybillId, (x, y) => { var item = y.First(); item.Count = y.Sum(z => z.Count); return item; });

                    model.WriteoffWaybillRows = model.WriteoffWaybillRows.GroupBy(x => x.WaybillId, (x, y) => { var item = y.First(); item.Count = y.Sum(z => z.Count); return item; });

                    model.ReturnFromClientWaybillRows = model.ReturnFromClientWaybillRows.GroupBy(x => x.WaybillId, (x, y) => { var item = y.First(); item.Count = y.Sum(z => z.Count); return item; });
                }

                return model;
            }
        }

        #region Выгрузка в Excel
        /// <summary>
        /// Экспорт отчета в Excel
        /// </summary>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="currentUser">Текущий пользователь</param>
        public byte[] Report0004ExportToExcel(Report0004SettingsViewModel settings, UserInfo currentUser)
        {
            var viewModel = Report0004(settings, currentUser);

            string reportHeader = "Движение товара \r\n" + viewModel.ArticleName + "\r\nза период с " + viewModel.Settings.StartDate + " по " + viewModel.Settings.EndDate;
            string sign = "Форма: Report0004" + "\r\nАвтор: " + viewModel.CreatedBy + "\r\nСоставлен: " + DateTime.Now.ToString();

            using (ExcelPackage pck = new ExcelPackage())
            {
                if (viewModel.Settings.ShowStartQuantityByStorage == "1" || viewModel.Settings.ShowEndQuantityByStorage == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Кол-во по МХ");
                    int startRow = sheet.PrintHeader(5, reportHeader, sign, "", 1);
                    sheet.DeleteRow(--startRow, 1);
                    int startCol = 1;
                    if (viewModel.Settings.ShowStartQuantityByStorage == "1")
                    {
                        sheet.Cells[startRow, startCol, startRow, startCol + 1].MergeRange().SetFormattedValue("Кол-во на " + viewModel.Settings.StartDate + " по местам хранения:", ExcelUtils.GetDefaultStyle())
                            .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        FillQuantityTable(sheet, 2, viewModel.StartQuantityGroupByStorage, startRow + 1, startCol);
                        startCol = 4;
                    }
                    if (viewModel.Settings.ShowEndQuantityByStorage == "1")
                    {
                        sheet.Cells[startRow, startCol, startRow, startCol + 1].MergeRange().SetFormattedValue("Кол-во на " + viewModel.Settings.EndDate + " по местам хранения:", ExcelUtils.GetDefaultStyle())
                            .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        FillQuantityTable(sheet, 2, viewModel.EndQuantityGroupByStorage, startRow + 1, startCol);
                    }
                }

                if (viewModel.Settings.ShowStartQuantityByOrganization == "1" || viewModel.Settings.ShowEndQuantityByOrganization == "1")
                {
                    ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Кол-во по организациям");
                    int startRow = sheet.PrintHeader(5, reportHeader, sign, "", 1);
                    sheet.DeleteRow(--startRow, 1);
                    int startCol = 1;
                    if (viewModel.Settings.ShowStartQuantityByOrganization == "1")
                    {
                        sheet.Cells[startRow, startCol, startRow, startCol + 1].MergeRange().SetFormattedValue("Кол-во на " + viewModel.Settings.StartDate + " по организациям:", ExcelUtils.GetDefaultStyle())
                            .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        FillQuantityTable(sheet, 2, viewModel.StartQuantityGroupByOrganization, startRow + 1, startCol);
                        startCol = 4;
                    }
                    if (viewModel.Settings.ShowEndQuantityByOrganization == "1")
                    {
                        sheet.Cells[startRow, startCol, startRow, startCol + 1].MergeRange().SetFormattedValue("Кол-во на " + viewModel.Settings.EndDate + " по организациям:", ExcelUtils.GetDefaultStyle())
                            .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        FillQuantityTable(sheet, 2, viewModel.EndQuantityGroupByOrganization, startRow + 1, startCol);
                    }
                }

                int receiptTableColumnCount = 0;
                int movementAndChangeOwnerTableColumnCount = 0;
                int expenditureTableColumnCount = 0;
                int writeOffTableColumnCount = 0;
                int returnsTableColumnCount = 0;
                int receiptDivergencesTableColumnCount = 0;

                GetColumnCount(out receiptTableColumnCount, out movementAndChangeOwnerTableColumnCount, out expenditureTableColumnCount,
                    out writeOffTableColumnCount, out returnsTableColumnCount, out receiptDivergencesTableColumnCount, viewModel);

                ExcelWorksheet receipt = pck.Workbook.Worksheets.Add("Приходы");
                FillReceiptTable(receipt, receiptTableColumnCount, viewModel, receipt.PrintHeader(receiptTableColumnCount, reportHeader, sign, "Приходы:", 1));

                ExcelWorksheet movementAndChangeOwner = pck.Workbook.Worksheets.Add("Перемещения и смены собственника");
                FillMovementAndChangeOwnerTable(movementAndChangeOwner, movementAndChangeOwnerTableColumnCount, viewModel, 
                    movementAndChangeOwner.PrintHeader(movementAndChangeOwnerTableColumnCount, reportHeader, sign, "Внутренние перемещения и смены собственника:", 1));

                ExcelWorksheet expenditure = pck.Workbook.Worksheets.Add("Реализация товаров");
                FillExpendituresTable(expenditure, expenditureTableColumnCount, viewModel, expenditure.PrintHeader(expenditureTableColumnCount, reportHeader, sign, "Реализация товаров:", 1));

                ExcelWorksheet writeOff = pck.Workbook.Worksheets.Add("Списания");
                FillWriteOffTable(writeOff, writeOffTableColumnCount, viewModel, writeOff.PrintHeader(writeOffTableColumnCount, reportHeader, sign, "Списания:", 1));

                ExcelWorksheet returns = pck.Workbook.Worksheets.Add("Возвраты");
                FillReturnsTable(returns, returnsTableColumnCount, viewModel, returns.PrintHeader(returnsTableColumnCount, reportHeader, sign, "Возвраты:", 1));

                if (viewModel.ReceiptDivergences.Any())
                {
                    ExcelWorksheet receiptDivergences = pck.Workbook.Worksheets.Add("Расхождения при приходе");
                    FillReceiptDivergencesTable(receiptDivergences, receiptDivergencesTableColumnCount, viewModel, receiptDivergences.PrintHeader(receiptDivergencesTableColumnCount, reportHeader, sign, "Расхождения при приходе:", 1));
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
        /// Подсчет колонок в таблицах
        /// </summary>
        private void GetColumnCount(out int receiptTableColumnCount, out int movementAndChangeOwnerTableColumnCount, out int expenditureTableColumnCount,
            out int writeOffTableColumnCount, out int returnsTableColumnCount, out int receiptDivergencesTableColumnCount, Report0004ViewModel viewModel)
        {
            receiptTableColumnCount = 6;
            movementAndChangeOwnerTableColumnCount = 8;
            expenditureTableColumnCount = 7;
            writeOffTableColumnCount = 7;
            returnsTableColumnCount = 7;
            receiptDivergencesTableColumnCount = 6;

            if (viewModel.AllowToViewProviders)
            { 
                receiptDivergencesTableColumnCount++;
            }
            if (viewModel.AllowToViewClients)
            {
                expenditureTableColumnCount++;
                returnsTableColumnCount++;
            }
            if (viewModel.Settings.ShowBatches == "1")
            {
                expenditureTableColumnCount++;
                movementAndChangeOwnerTableColumnCount++;
                writeOffTableColumnCount++;
                returnsTableColumnCount++;
            }
            if (viewModel.Settings.ShowSenderAccountingPrices == "1")
            { 
                expenditureTableColumnCount++;
                movementAndChangeOwnerTableColumnCount++;
                writeOffTableColumnCount++;
            }
            if (viewModel.AllowToViewContractors)
            {
                receiptTableColumnCount++;
            }
            if (viewModel.Settings.ShowPurchaseCosts == "1")
            {
                receiptTableColumnCount++;
                receiptDivergencesTableColumnCount++;
            }
            if (viewModel.Settings.ShowRecipientAccountingPrices == "1")
            {
                receiptTableColumnCount++;
                movementAndChangeOwnerTableColumnCount++;
                returnsTableColumnCount++;
                receiptDivergencesTableColumnCount++;
            }
        }

        /// <summary>
        /// Формирует таблицу с расхождениями при приеме
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillReceiptDivergencesTable(ExcelWorksheet workSheet, int columns, Report0004ViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("№ накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Дата накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Статус накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Кол-во расхождений");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Место хранения-приемщик");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Организация-приемщик");
            currentCol++;

            if (viewModel.AllowToViewProviders)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("Поставщик");
                currentCol++;
            }
            if (viewModel.Settings.ShowPurchaseCosts == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("ЗЦ");
                currentCol++;
            }
            if (viewModel.Settings.ShowRecipientAccountingPrices == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("УЦ приемки");
                currentCol++;
            }

            currentCol = 1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки отчета
            bool flag = false;
            if (viewModel.ReceiptDivergences.Any())
            {
                foreach (var item in viewModel.ReceiptDivergences)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Date.ToShortDateString()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.StateName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Count, ValueDisplayType.PackCount);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.RecipientStorage).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Recipient).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    if (viewModel.AllowToViewProviders)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Contractor).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowPurchaseCosts == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.PurchaseCost, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowRecipientAccountingPrices == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.RecipientAccountingPrice, ValueDisplayType.Money);
                        currentCol++;
                    }
                    currentCol = 1;
                    currentRow++;
                    flag = !flag;
                }

            }
            else
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle())
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous).MergeRange().SetFormattedValue("Нет данных");
                currentRow++;
            }
            #endregion

            #region Итого
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого:");
            currentCol += 3;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ReceiptDivergences.Sum(x => x.Count), ValueDisplayType.PackCount);
            currentRow++;
            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        /// <summary>
        /// Формирует таблицу с возвратами
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillReturnsTable(ExcelWorksheet workSheet, int columns, Report0004ViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("№ накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Дата накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Статус накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Кол-во");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Место хранения-приемщик");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Организация-приемщик");
            currentCol++;
            if (viewModel.AllowToViewClients)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("Клиент");
                currentCol++;
            }
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Основание");
            currentCol++;
            if (viewModel.Settings.ShowBatches == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("Партия");
                currentCol++;
            }
            if (viewModel.Settings.ShowRecipientAccountingPrices == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("УЦ приемки");
                currentCol++;
            }

            currentCol = 1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки отчета
            bool flag = false;
            if (viewModel.ReturnFromClientWaybillRows.Any())
            {
                foreach (var item in viewModel.ReturnFromClientWaybillRows)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Date.ToShortDateString()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.StateName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Count, ValueDisplayType.PackCount);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.RecipientStorage).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Recipient).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    if (viewModel.AllowToViewClients)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ClientName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Reason).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    if (viewModel.Settings.ShowBatches == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.BatchName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowRecipientAccountingPrices == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.RecipientAccountingPrice, ValueDisplayType.Money);
                        currentCol++;
                    }
                    currentCol = 1;
                    currentRow++;
                    flag = !flag;
                }

            }
            else
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle())
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous).MergeRange().SetFormattedValue("Нет данных");
                currentRow++;
            }
            #endregion

            #region Итого
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого:");
            currentCol += 3;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ReturnFromClientWaybillRows.Sum(x => x.Count), ValueDisplayType.PackCount);
            currentRow++;
            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        /// <summary>
        /// Формирует таблицу со списаниями
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillWriteOffTable(ExcelWorksheet workSheet, int columns, Report0004ViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("№ накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Дата накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Статус накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Кол-во");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Место хранения-отправитель");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Организация-отправитель");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Основание");
            currentCol++;

            if (viewModel.Settings.ShowBatches == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("Партия");
                currentCol++;
            }
            if (viewModel.Settings.ShowSenderAccountingPrices == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("УЦ отправки");
                currentCol++;
            }

            currentCol = 1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки отчета
            bool flag = false;
            if (viewModel.WriteoffWaybillRows.Any())
            {
                foreach (var item in viewModel.WriteoffWaybillRows)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Date.ToShortDateString()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.StateName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Count, ValueDisplayType.PackCount);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.SenderStorage).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Sender).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Reason).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    if (viewModel.Settings.ShowBatches == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.BatchName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowSenderAccountingPrices == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.SenderAccountingPrice, ValueDisplayType.Money);
                        currentCol++;
                    }
                    currentCol = 1;
                    currentRow++;
                    flag = !flag;
                }

            }
            else
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle())
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous).MergeRange().SetFormattedValue("Нет данных");
                currentRow++;
            }
            #endregion

            #region Итого
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого:");
            currentCol += 3;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.WriteoffWaybillRows.Sum(x => x.Count), ValueDisplayType.PackCount);
            currentRow++;
            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        /// <summary>
        /// Формирует таблицу с реализациями
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillExpendituresTable(ExcelWorksheet workSheet, int columns, Report0004ViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("№ накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Дата накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Статус накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Кол-во");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Отпускная цена");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Место хранения-отправитель");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Организация-отправитель");
            currentCol++;

            if (viewModel.AllowToViewClients)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("Клиент");
                currentCol++;
            }
            if (viewModel.Settings.ShowBatches == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("Партия");
                currentCol++;
            }
            if (viewModel.Settings.ShowSenderAccountingPrices == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("УЦ отправки");
                currentCol++;
            }

            currentCol = 1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки отчета
            bool flag = false;
            if (viewModel.ExpenditureWaybillRows.Any())
            {
                foreach (var item in viewModel.ExpenditureWaybillRows)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Date.ToShortDateString()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.StateName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Count, ValueDisplayType.PackCount);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.SalePrice, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.SenderStorage).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Sender).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    if (viewModel.AllowToViewClients)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ClientName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowBatches == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.BatchName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowSenderAccountingPrices == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.SenderAccountingPrice, ValueDisplayType.Money);
                        currentCol++;
                    }
                    currentCol = 1;
                    currentRow++;
                    flag = !flag;
                }

            }
            else
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle())
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous).MergeRange().SetFormattedValue("Нет данных");
                currentRow++;
            }
            #endregion

            #region Итого
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого:");
            currentCol += 3;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ExpenditureWaybillRows.Sum(x => x.Count), ValueDisplayType.PackCount);
            currentRow++;
            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        /// <summary>
        /// Формирует таблицу с приходами
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillReceiptTable(ExcelWorksheet workSheet, int columns, Report0004ViewModel viewModel, int startRow)
        { 
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
             //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("№ накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Дата накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Статус накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Кол-во");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Место хранения-приемщик");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Организация-приемщик");
            currentCol++;
            
            if (viewModel.AllowToViewContractors)
            { 
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("Поставщик / Производитель");
                currentCol++;
            }
            if (viewModel.Settings.ShowPurchaseCosts == "1")
            { 
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("ЗЦ");
                currentCol++;
            }
            if (viewModel.Settings.ShowRecipientAccountingPrices == "1")
            { 
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("УЦ приемки");
                currentCol++;
            }

            currentCol = 1;
            currentRow ++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки отчета
            bool flag = false;
            if (viewModel.ReceiptWaybillRows.Any())
            {
                foreach (var item in viewModel.ReceiptWaybillRows)
                {        
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Date.ToShortDateString()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.StateName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Count, ValueDisplayType.PackCount);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.RecipientStorage).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Recipient).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    if (viewModel.AllowToViewContractors)
                    { 
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Contractor).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowPurchaseCosts == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.PurchaseCost, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowRecipientAccountingPrices == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.RecipientAccountingPrice, ValueDisplayType.Money);
                        currentCol++;
                    }
                    currentCol = 1;
                    currentRow++;
                    flag = !flag;
                }
                         
            }
            else
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle())
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous).MergeRange().SetFormattedValue("Нет данных");
                currentRow++;
            }
            #endregion

            #region Итого
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());
            
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого:");
            currentCol += 3;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ReceiptWaybillRows.Sum(x => x.Count), ValueDisplayType.PackCount);
            currentRow++;
            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        /// <summary>
        /// Формирует таблицу с перемещениями и сменами собственника
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillMovementAndChangeOwnerTable(ExcelWorksheet workSheet, int columns, Report0004ViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("№ накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Дата накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Статус накладной");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Кол-во");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Место хранения-отправитель");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Организация-отправитель");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Место хранения-приемщик");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Организация-приемщик");
            currentCol++;

            if (viewModel.Settings.ShowBatches == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("Партия");
                currentCol++;
            }
            if (viewModel.Settings.ShowSenderAccountingPrices == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("УЦ отправки");
                currentCol++;
            }
            if (viewModel.Settings.ShowRecipientAccountingPrices == "1")
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue("УЦ приемки");
                currentCol++;
            }

            currentCol = 1;
            currentRow++;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки отчета
            bool flag = false;
            if (viewModel.MovementAndChangeOwnerWaybillRows.Any())
            {
                foreach (var item in viewModel.MovementAndChangeOwnerWaybillRows)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Date.ToShortDateString()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.StateName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Count, ValueDisplayType.PackCount);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.SenderStorage).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Sender).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.RecipientStorage).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Recipient).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    if (viewModel.Settings.ShowBatches == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.BatchName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowSenderAccountingPrices == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.SenderAccountingPrice, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowRecipientAccountingPrices == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.RecipientAccountingPrice, ValueDisplayType.Money);
                        currentCol++;
                    }
                    currentCol = 1;
                    currentRow++;
                    flag = !flag;
                }
            }
            else
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle())
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous).MergeRange().SetFormattedValue("Нет данных");
                currentRow++;
            }
            #endregion

            #region Итого
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого:");
            currentCol += 3;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.MovementAndChangeOwnerWaybillRows.Sum(x => x.Count), ValueDisplayType.PackCount);
            currentRow++;
            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }

        /// <summary>
        /// Формирует сводную таблицу с количеством
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <param name="startCol">Начальная колонка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillQuantityTable(ExcelWorksheet workSheet, int columns, Report0004QuantityTableViewModel viewModel, int startRow, int startCol)
        {
            int currentRow = startRow;
            int currentCol = startCol;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + columns - 1].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.FirstColumnName);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Кол-во");
            currentCol = startCol;
            currentRow ++;
            workSheet.View.FreezePanes(currentRow, 1);
            #endregion

            #region Строки отчета
            bool flag = false;

            if (viewModel.Items.Any())
            {
                foreach (var item in viewModel.Items)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + columns - 1].ApplyStyle(flag ? ExcelUtils.GetTableEvenRowStyle() : ExcelUtils.GetTableUnEvenRowStyle());
                    
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.IndicatorName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Quantity, ValueDisplayType.PackCount);
                    currentCol = startCol;
                    currentRow ++;
                    flag = !flag;
                }
            }
            else
            {
                workSheet.Cells[currentRow, currentCol, currentRow, currentCol + columns - 1].MergeRange().SetFormattedValue("Нет данных", ExcelUtils.GetTableEvenRowStyle())
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                currentRow++;
            }
            #endregion

            #region Итого
            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + columns - 1].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого:");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.TotalQuantity, ValueDisplayType.PackCount);
            currentCol = startCol;
            currentRow++;
            #endregion

            workSheet.Cells[startRow, startCol, currentRow, startCol + columns - 1].AutofitRangeColumns(100);
            if (workSheet.Column(startCol).Width < 50) workSheet.Column(startCol).Width = 50;
            workSheet.Select(new ExcelAddress(startRow, startCol + columns + 3, startRow, startCol + columns + 3));

            return currentRow;
        }
        #endregion
    }
}