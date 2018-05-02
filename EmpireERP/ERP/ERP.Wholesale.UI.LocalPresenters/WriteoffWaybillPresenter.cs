using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.PrintingForm.WriteoffWaybill;
using ERP.Wholesale.UI.ViewModels.WriteoffWaybill;
using ERP.Wholesale.UI.ViewModels.OutgoingWaybill;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class WriteoffWaybillPresenter : OutgoingWaybillPresenter<WriteoffWaybill>, IWriteoffWaybillPresenter
    {
        #region Поля

        private readonly IWriteoffWaybillService writeoffWaybillService;
        private readonly IWriteoffWaybillMainIndicatorService writeoffWaybillIndicatorService;
        private readonly IWriteoffReasonService writeoffReasonService;
        private readonly IReceiptWaybillService receiptWaybillService;
        private readonly IOutgoingWaybillRowService outgoingWaybillRowService;

        #endregion

        #region Конструкторы

        public WriteoffWaybillPresenter(IUnitOfWorkFactory unitOfWorkFactory, IWriteoffWaybillService writeoffWaybillService, 
            IWriteoffWaybillMainIndicatorService writeoffWaybillIndicatorService, IStorageService storageService, IWriteoffReasonService writeoffReasonService,
            IArticleAvailabilityService articleAvailabilityService, IAccountOrganizationService accountOrganizationService,
            IReceiptWaybillService receiptWaybillService, IArticleService articleService, IArticlePriceService articlePriceService, 
            IMovementWaybillService movementWaybillService, IOutgoingWaybillRowService outgoingWaybillRowService, IUserService userService)
            : base(unitOfWorkFactory,writeoffWaybillService,userService,storageService, accountOrganizationService, 
            articleService, articlePriceService, articleAvailabilityService)
        {
            this.writeoffWaybillService = writeoffWaybillService;
            this.writeoffWaybillIndicatorService = writeoffWaybillIndicatorService;
            this.writeoffReasonService = writeoffReasonService;
            this.receiptWaybillService = receiptWaybillService;
            this.outgoingWaybillRowService = outgoingWaybillRowService;
        }

        #endregion

        #region Методы

        #region Список

        public WriteoffWaybillListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.WriteoffWaybill_List_Details);

                var model = new WriteoffWaybillListViewModel();
                model.WriteoffPendingGrid = GetWriteoffPendingGridLocal(new GridState() { Sort = "Date=Desc;CreationDate=Desc" }, user);
                model.WrittenoffGrid = GetWrittenOffGridLocal(new GridState() { Sort = "Date=Desc;CreationDate=Desc" }, user);

                var storageList = storageService.GetList(user, Permission.WriteoffWaybill_List_Details).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                    .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false);

                var writeoffReasonList = writeoffReasonService.GetList()
                    .GetComboBoxItemList(s => s.Name, s => s.Id.ToString());

                model.FilterData.Items.Add(new FilterDateRangePicker("Date", "Дата накладной"));
                model.FilterData.Items.Add(new FilterTextEditor("Number", "№ накладной"));
                model.FilterData.Items.Add(new FilterComboBox("SenderStorage", "Место хранения", storageList));
                model.FilterData.Items.Add(new FilterComboBox("WriteoffReason", "Основание", writeoffReasonList));

                return model;
            }
        }

        #region Новые (Ожидается списание)

        public GridData GetWriteoffPendingGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetWriteoffPendingGridLocal(state, user);
            }
        }

        private GridData GetWriteoffPendingGridLocal(GridState state, User user)
        {
            GridData model = new GridData();

            model.AddColumn("IsAccepted", "Пр.", Unit.Pixel(22), align: GridColumnAlign.Center);
            model.AddColumn("Number", "Номер", Unit.Pixel(60), GridCellStyle.Link);
            model.AddColumn("Date", "Дата", Unit.Pixel(60));
            model.AddColumn("PurchaseCostSum", "Сумма в ЗЦ", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("SenderAccountingPriceSum", "Сумма в УЦ", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("SenderStorageName", "Место хранения", Unit.Percentage(32));
            model.AddColumn("WriteoffReason", "Основание", Unit.Percentage(32));
            model.AddColumn("SenderOrganizationName", "Организация", Unit.Percentage(36), GridCellStyle.Link);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.WriteoffWaybill_Create_Edit);

            string filter = state.Filter;
            ParameterString deriveFilter = new ParameterString(state.Filter);
            var item = deriveFilter.Add("State", ParameterStringItem.OperationType.OneOf);
            deriveFilter["State"].Value = new List<string>()
            {
                WriteoffWaybillState.Draft.ValueToString(),
                WriteoffWaybillState.ReadyToAccept.ValueToString(),
                WriteoffWaybillState.ArticlePending.ValueToString(),
                WriteoffWaybillState.ConflictsInArticle.ValueToString(),
                WriteoffWaybillState.ReadyToWriteoff.ValueToString()
            };

            state.Filter = deriveFilter.ToString();

            var rows = writeoffWaybillService.GetFilteredList(state, user);

            state.Filter = filter;

            foreach (var waybill in rows)
            {
                GridRowStyle rowStyle;

                switch (waybill.State)
                {
                    case WriteoffWaybillState.ConflictsInArticle:
                        rowStyle = GridRowStyle.Error; break;
                    case WriteoffWaybillState.ArticlePending:
                        rowStyle = GridRowStyle.Warning; break;
                    default:
                        rowStyle = GridRowStyle.Normal; break;
                }

                var ind = writeoffWaybillIndicatorService.GetMainIndicators(waybill, user);
                var isAcceptedStr = "";
                if (waybill.IsReadyToAccept)
                {
                    isAcceptedStr = "Г";
                }
                else if (waybill.IsAccepted)
                {
                    isAcceptedStr = "П";
                }
                model.AddRow(new GridRow(
                    new GridLabelCell("IsAccepted") { Value = isAcceptedStr },
                    new GridLinkCell("Number") { Value = StringUtils.PadLeftZeroes(waybill.Number, 8) },
                    new GridLabelCell("Date") { Value = waybill.Date.ToShortDateString() },
                    new GridLabelCell("PurchaseCostSum") { Value = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere) ? waybill.PurchaseCostSum.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("SenderAccountingPriceSum") { Value = ind.SenderAccountingPriceSum.HasValue ? ind.SenderAccountingPriceSum.Value.ForDisplay(ValueDisplayType.Money) : "---" },
                    storageService.IsPossibilityToViewDetails(waybill.SenderStorage, user) ?
                        (GridCell)new GridLinkCell("SenderStorageName") { Value = waybill.SenderStorage.Name } :
                        new GridLabelCell("SenderStorageName") { Value = waybill.SenderStorage.Name },
                    new GridLabelCell("WriteoffReason") { Value = waybill.WriteoffReason.Name },
                    new GridLinkCell("SenderOrganizationName") { Value = waybill.Sender.ShortName },
                    new GridHiddenCell("Id") { Value = waybill.Id.ToString() },
                    new GridHiddenCell("SenderOrganizationId") { Value = waybill.Sender.Id.ToString() },
                    new GridHiddenCell("SenderStorageId") { Value = waybill.SenderStorage.Id.ToString() }
                ) { Style = rowStyle });
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Выполненные списания

        public GridData GetWrittenOffGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetWrittenOffGridLocal(state, user);
            }
        }

        private GridData GetWrittenOffGridLocal(GridState state, User user)
        {
            GridData model = new GridData();

            model.AddColumn("Number", "Номер", Unit.Pixel(60), GridCellStyle.Link);
            model.AddColumn("Date", "Дата", Unit.Pixel(60));
            model.AddColumn("PurchaseCostSum", "Сумма в ЗЦ", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("SenderAccountingPriceSum", "Сумма в УЦ", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("SenderStorageName", "Место хранения", Unit.Percentage(32));
            model.AddColumn("WriteoffReason", "Основание", Unit.Percentage(32));
            model.AddColumn("SenderOrganizationName", "Организация", Unit.Percentage(36), GridCellStyle.Link);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            string filter = state.Filter;
            ParameterString deriveFilter = new ParameterString(state.Filter);
            deriveFilter.Add("State", ParameterStringItem.OperationType.Eq, ((byte)WriteoffWaybillState.Writtenoff).ToString());

            state.Filter = deriveFilter.ToString();

            var rows = writeoffWaybillService.GetFilteredList(state, user);

            state.Filter = filter;

            foreach (var row in rows)
            {
                model.AddRow(new GridRow(
                    new GridLinkCell("Number") { Value = StringUtils.PadLeftZeroes(row.Number, 8) },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("PurchaseCostSum") { Value = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere) ? row.PurchaseCostSum.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("SenderAccountingPriceSum") { Value = row.SenderAccountingPriceSum.ForDisplay(ValueDisplayType.Money) },
                    storageService.IsPossibilityToViewDetails(row.SenderStorage, user) ?
                        (GridCell)new GridLinkCell("SenderStorageName") { Value = row.SenderStorage.Name } :
                        new GridLabelCell("SenderStorageName") { Value = row.SenderStorage.Name },
                    new GridLabelCell("WriteoffReason") { Value = row.WriteoffReason.Name },
                    new GridLinkCell("SenderOrganizationName") { Value = row.Sender.ShortName },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() },
                    new GridHiddenCell("SenderOrganizationId") { Value = row.Sender.Id.ToString() },
                    new GridHiddenCell("SenderStorageId") { Value = row.SenderStorage.Id.ToString() }
                ));
            }

            model.State = state;

            return model;
        }

        #endregion

        #endregion

        #region Добавление / редактирование

        public WriteoffWaybillEditViewModel Create(string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.WriteoffWaybill_Create_Edit);

                var model = new WriteoffWaybillEditViewModel()
                {
                    Title = "Добавление накладной списания",
                    BackURL = backURL,
                    Date = DateTime.Today.ToShortDateString(),
                    StorageList = storageService.GetList(user, Permission.WriteoffWaybill_Create_Edit).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                        .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false),
                    WriteoffReasonList = writeoffReasonService.GetList().GetComboBoxItemList(s => s.Name, s => s.Id.ToString()),
                    Number = "",
                    AllowToGenerateNumber = true,
                    IsAutoNumber = "1",
                    CuratorId = user.Id.ToString(),
                    CuratorName = user.DisplayName,
                    AllowToEdit = true,
                    AllowToAddReason = user.HasPermission(Permission.WriteoffReason_Create),
                    AllowToChangeCurator = user.HasPermission(Permission.WriteoffWaybill_Curator_Change)
                };

                return model;
            }
        }

        public WriteoffWaybillEditViewModel Edit(Guid waybillId, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.WriteoffWaybill_Create_Edit);

                var waybill = writeoffWaybillService.CheckWaybillExistence(waybillId, user);

                var model = new WriteoffWaybillEditViewModel()
                {
                    Number = waybill.Number,
                    IsAutoNumber = "0",
                    AllowToGenerateNumber = false,
                    Date = waybill.Date.ToShortDateString(),
                    SenderStorageId = waybill.SenderStorage.Id,
                    StorageList = storageService.GetList(user, Permission.WriteoffWaybill_Create_Edit).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                        .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false),
                    SenderList = waybill.SenderStorage.AccountOrganizations.GetComboBoxItemList(s => s.ShortName, s => s.Id.ToString()),
                    SenderId = waybill.Sender.Id,
                    WriteoffReasonId = waybill.WriteoffReason.Id,
                    WriteoffReasonList = writeoffReasonService.GetList().GetComboBoxItemList(s => s.Name, s => s.Id.ToString()),
                    Title = "Редактирование накладной списания",
                    BackURL = backURL,
                    Comment = waybill.Comment,
                    Id = waybill.Id,
                    Name = waybill.Name,
                    CuratorId = waybill.Curator.Id.ToString(),
                    CuratorName = waybill.Curator.DisplayName,
                    AllowToEdit = writeoffWaybillService.IsPossibilityToEdit(waybill, user),
                    AllowToAddReason = user.HasPermission(Permission.WriteoffReason_Create),
                    AllowToChangeCurator = writeoffWaybillService.IsPossibilityToChangeCurator(waybill, user)
                };

                return model;
            }
        }

        public string Save(WriteoffWaybillEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                WriteoffWaybill waybill = null;

                var senderStorage = storageService.CheckStorageExistence(model.SenderStorageId, user, "Место хранения отправителя не найдено. Возможно, оно было удалено.");
                var sender = accountOrganizationService.CheckAccountOrganizationExistence(model.SenderId, "Организация отправителя не найдена. Возможно, она была удалена.");
                var writeoffReason = writeoffReasonService.CheckExistence(model.WriteoffReasonId);
                var curatorId = ValidationUtils.TryGetInt(model.CuratorId);

                bool isAutoNumber = ValidationUtils.TryGetBool(model.IsAutoNumber);

                // добавление
                if (model.Id == Guid.Empty)
                {
                    user.CheckPermission(Permission.WriteoffWaybill_Create_Edit);
                    var curator = userService.CheckUserExistence(curatorId, user, "Куратор не найден. Возможно, он был удален.");

                    if (isAutoNumber)
                    {
                        model.Number = "";
                    }

                    waybill = new WriteoffWaybill(model.Number, DateTime.Parse(model.Date), senderStorage, sender, writeoffReason, curator, user, currentDateTime);
                    // если куратор не совпадает с пользователем, то ...
                    if (user != curator)
                    {
                        user.CheckPermission(Permission.WriteoffWaybill_Curator_Change);    // ... проверяем права на смену куратора и ...
                        writeoffWaybillService.CheckPossibilityToViewDetailsByUser(waybill, curator);   // ... видимость накладной куратору
                    }
                }
                // редактирование
                else
                {
                    waybill = writeoffWaybillService.CheckWaybillExistence(model.Id, user);

                    writeoffWaybillService.CheckPossibilityToEdit(waybill, user);

                    waybill.Number = model.Number;
                    waybill.WriteoffReason = writeoffReason;

                    if (waybill.Curator.Id != curatorId)
                    {
                        var curator = userService.CheckUserExistence(curatorId, user, "Куратор не найден. Возможно, он был удален.");

                        writeoffWaybillService.CheckPossibilityToChangeCurator(waybill, user);
                        writeoffWaybillService.CheckPossibilityToViewDetailsByUser(waybill, curator);

                        waybill.Curator = curator;
                    }
                }

                waybill.Comment = StringUtils.ToHtml(model.Comment);

                writeoffWaybillService.Save(waybill);

                uow.Commit();

                return waybill.Id.ToString();
            }
        }

        public object GetAccountOrganizationsForStorage(short storageId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var organizationList = from c in storageService.GetById(storageId).AccountOrganizations
                                       select new SelectListItem
                                       {
                                           Value = c.Id.ToString(),
                                           Text = c.ShortName
                                       };
                var selected = organizationList.Count() == 1 ? organizationList.First().Value : "0";
                var x = new { List = organizationList.OrderBy(z => z.Text).ToList(), SelectedOption = selected };

                return x;
            }
        }

        #endregion

        #region Удаление

        public void Delete(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var writeoffWaybill = writeoffWaybillService.CheckWaybillExistence(id, user);

                writeoffWaybillService.Delete(writeoffWaybill, user);

                uow.Commit();
            }
        }

        #endregion

        #region Детали накладной

        public WriteoffWaybillDetailsViewModel Details(Guid id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.WriteoffWaybill_List_Details);

                var waybill = writeoffWaybillService.CheckWaybillExistence(id, user);

                var model = new WriteoffWaybillDetailsViewModel()
                {
                    Id = waybill.Id,
                    Name = waybill.Name,
                    MainDetails = GetMainDetails(waybill, user),
                    BackURL = backURL,
                    RowGrid = GetWriteoffWaybillRowGridLocal(new GridState() { Parameters = "WriteoffWaybillId=" + waybill.Id, Sort = "CreationDate=Asc" }, user),
                    ArticleGroupGridState = new GridState() { Parameters = "WriteoffWaybillId=" + waybill.Id },
                    DocumentGrid = GetDocumentGridLocal(),
                    AllowToDelete = writeoffWaybillService.IsPossibilityToDelete(waybill, user),
                    AllowToEdit = writeoffWaybillService.IsPossibilityToEdit(waybill, user),
                    AllowToWriteoff = writeoffWaybillService.IsPossibilityToWriteoff(waybill, user),
                    IsPossibilityToWriteoff = writeoffWaybillService.IsPossibilityToWriteoff(waybill, user, false) && !waybill.IsWrittenOff && waybill.IsAccepted,
                    AllowToCancelWriteoff = writeoffWaybillService.IsPossibilityToCancelWriteoff(waybill, user),
                    AllowToPrintForms = writeoffWaybillService.IsPossibilityToPrintForms(waybill, user),
                    AllowToPrepareToAccept = writeoffWaybillService.IsPossibilityToPrepareToAccept(waybill, user),
                    IsPossibilityToPrepareToAccept = writeoffWaybillService.IsPossibilityToPrepareToAccept(waybill, user, true),
                    AllowToCancelReadinessToAccept = writeoffWaybillService.IsPossibilityToCancelReadinessToAccept(waybill, user),
                    AllowToAccept = writeoffWaybillService.IsPossibilityToAccept(waybill, user),
                    IsPossibilityToAccept = writeoffWaybillService.IsPossibilityToAccept(waybill, user, true),
                    AllowToCancelAcceptance = writeoffWaybillService.IsPossibilityToCancelAcceptance(waybill, user)
                };

                return model;
            }
        }

        private WriteoffWaybillMainDetailsViewModel GetMainDetails(WriteoffWaybill waybill, User user)
        {
            var ind = writeoffWaybillIndicatorService.GetMainIndicators(waybill, user, calculateReceivelessProfit: true);

            var model = new WriteoffWaybillMainDetailsViewModel();
        
            model.Comment = waybill.Comment;
            model.PurchaseCostSum = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere) ? waybill.PurchaseCostSum.ForDisplay(ValueDisplayType.Money) : "---"; 
            model.WriteoffReasonName = waybill.WriteoffReason.Name;
            model.ReceivelessProfitPercent = ind.ReceivelessProfitPercent.HasValue ? Math.Round(ind.ReceivelessProfitPercent.Value, 2).ForDisplay(ValueDisplayType.Percent) : "---";
            model.ReceivelessProfitSum = ind.ReceivelessProfitSum.ForDisplay(ValueDisplayType.Money);
            model.RowCount = waybill.RowCount.ForDisplay();
            model.SenderAccountingPriceSum = ind.SenderAccountingPriceSum.HasValue ? ind.SenderAccountingPriceSum.Value.ForDisplay(ValueDisplayType.Money) : "---";
            model.StateName = waybill.State.GetDisplayName();
            model.SenderStorageName = waybill.SenderStorage.Name;
            model.SenderStorageId = waybill.SenderStorage.Id.ToString();
            model.SenderName = waybill.Sender.ShortName;
            model.SenderId = waybill.Sender.Id.ToString();
            model.CuratorId = waybill.Curator.Id.ToString();
            model.CuratorName = waybill.Curator.DisplayName;
            model.CreatedById = waybill.CreatedBy.Id.ToString();
            model.CreatedByName = waybill.CreatedBy.DisplayName;
            model.CreationDate = String.Format("({0})", waybill.CreationDate.ToShortDateTimeString());
            model.AcceptedById = waybill.AcceptedBy != null ? waybill.AcceptedBy.Id.ToString() : "";
            model.AcceptedByName = waybill.AcceptedBy != null ? waybill.AcceptedBy.DisplayName : "";
            model.AcceptanceDate = waybill.AcceptedBy != null ? String.Format("({0})", waybill.AcceptanceDate.Value.ToShortDateTimeString()) : "";
            model.WrittenoffById = waybill.WrittenoffBy != null ? waybill.WrittenoffBy.Id.ToString() : "";
            model.WrittenoffByName = waybill.WrittenoffBy != null ? waybill.WrittenoffBy.DisplayName : "";
            model.WriteoffDate = waybill.WrittenoffBy != null ? String.Format("({0})", waybill.WriteoffDate.Value.ToShortDateTimeString()) : "";
            model.TotalWeight = waybill.Weight.ForDisplay(ValueDisplayType.Weight);
            model.TotalVolume = waybill.Volume.ForDisplay(ValueDisplayType.Volume);

            model.AllowToViewCuratorDetails = userService.IsPossibilityToViewDetails(waybill.Curator, user);
            model.AllowToViewSenderStorageDetails = storageService.IsPossibilityToViewDetails(waybill.SenderStorage, user);
            model.AllowToChangeCurator = writeoffWaybillService.IsPossibilityToChangeCurator(waybill, user);
            model.AllowToViewCreatedByDetails = userService.IsPossibilityToViewDetails(waybill.CreatedBy, user);
            model.AllowToViewAcceptedByDetails = userService.IsPossibilityToViewDetails(waybill.AcceptedBy, user);
            model.AllowToViewWrittenoffByDetails = userService.IsPossibilityToViewDetails(waybill.WrittenoffBy, user);

            return model;
        }

        #region Список позиций

        public GridData GetWriteoffWaybillRowGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetWriteoffWaybillRowGridLocal(state, user);
            }
        }

        private GridData GetWriteoffWaybillRowGridLocal(GridState state, User user)
        {
            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(120));
            model.AddColumn("Batch", "Партия", Unit.Pixel(120));
            model.AddColumn("ArticleId", "Код товара", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("ArticleNumber", "Артикул", Unit.Pixel(80));
            model.AddColumn("ArticleName", "Наименование товара", Unit.Percentage(100));
            model.AddColumn("Weight", "Вес (кг)", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Volume", "Объем (м3)", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("SenderAccountingPrice", "Уч. цена", Unit.Pixel(82), align: GridColumnAlign.Right);
            model.AddColumn("Sum", "Сумма", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("PackCount", "Кол-во ЕУ", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Count", "Кол-во", Unit.Pixel(50), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = writeoffWaybillService.CheckWaybillExistence(ValidationUtils.TryGetNotEmptyGuid(deriveParams["WriteoffWaybillId"].Value as string), user);

            // получение стиля строк грида
            var rowStyles = GetRowsStyle(writeoffWaybillService.GetRowStates(waybill));

            var allowToEdit = writeoffWaybillService.IsPossibilityToEdit(waybill, user);
            model.ButtonPermissions["AllowToAddRow"] = allowToEdit;

            var indicators = writeoffWaybillIndicatorService.GetMainRowsIndicators(waybill, user);

            foreach (var row in waybill.Rows.OrderBy(x => x.CreationDate))
            {
                var actions = new GridActionCell("Action");
                if (allowToEdit)
                {
                    actions.AddAction("Ред.", "edit_link");
                }
                else
                {
                    actions.AddAction("Дет.", "details_link");
                }

                if (writeoffWaybillService.IsPossibilityToDeleteRow(row, user))
                {
                    actions.AddAction("Удал.", "delete_link");
                }

                if (row.AreSourcesDetermined)
                {
                    actions.AddAction("Источ.", "source_link");
                }

                var receiptWaybillRow = row.ReceiptWaybillRow;
                var article = row.Article;

                // получаем учетную цену для указанного товара по указанному месту хранению                
                var ind = indicators[row.Id];

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Batch") { Value = receiptWaybillRow.BatchName },
                    new GridLabelCell("ArticleId") { Value = article.Id.ToString() },
                    new GridLabelCell("ArticleNumber") { Value = article.Number },
                    new GridLabelCell("ArticleName") { Value = article.FullName },
                    new GridLabelCell("Weight") { Value = row.Weight.ForDisplay(ValueDisplayType.Weight) },
                    new GridLabelCell("Volume") { Value = row.Volume.ForDisplay(ValueDisplayType.Volume) },
                    new GridLabelCell("SenderAccountingPrice") { Value = ind.SenderAccountingPrice.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("Sum") { Value = (ind.SenderAccountingPrice != null ? (ind.SenderAccountingPrice.Value * row.WritingoffCount).ForDisplay(ValueDisplayType.Money) : "---") },
                    new GridLabelCell("PackCount") { Value = row.PackCount.ForDisplay(ValueDisplayType.PackCount) },
                    new GridLabelCell("Count") { Value = row.WritingoffCount.ForDisplay() },
                    new GridHiddenCell("Id") { Value = row.Id.ToString(), Key = "writeoffWaybillRowId" }
               ) { Style = rowStyles[row.Id] });
            }
            model.State = state;
            model.State.TotalRow = model.RowCount;

            return model;
        }

        /// <summary>
        /// Получить грид с группами товаров
        /// </summary>
        public GridData GetWriteoffWaybillArticleGroupGrid(GridState state, UserInfo currentUser)
        {

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetWriteoffWaybillArticleGroupGridLocal(state, user);
            }

        }

        private GridData GetWriteoffWaybillArticleGroupGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = writeoffWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid((deriveParams["WriteoffWaybillId"].Value.ToString())), user);

            var articleGroups = waybill.Rows.GroupBy(x => x.Article.ArticleGroup);

            var rows= new List<BaseWaybillArticleGroupRow>();

            // получение основных индикаторов для списка позиций
            var indicators = writeoffWaybillIndicatorService.GetMainRowsIndicators(waybill, user);
            
            foreach (var articleGroup in articleGroups.OrderBy(x => x.Key.Name))
            {
                var row = new BaseWaybillArticleGroupRow();

                row.Name = articleGroup.Key.Name;
                row.ArticleCount = articleGroup.Sum(x => x.WritingoffCount);
                row.PackCount = articleGroup.Sum(x => x.PackCount);

                //вычисляем сумму по группе и сумму ндс по группе
                foreach (var waybillRow in articleGroup)
                {
                    // получаем основные показатели для текущей позиции
                    var ind = indicators[waybillRow.Id];

                    decimal? waybillRowSum = ind.SenderAccountingPrice != null ? Math.Round(ind.SenderAccountingPrice.Value * waybillRow.WritingoffCount, 2) : (decimal?)null;
                    
                    if (waybillRowSum.HasValue)
                    {
                        if (row.Sum.HasValue)
                            row.Sum += waybillRowSum;
                        else
                            row.Sum = waybillRowSum;
                    }
                }

                rows.Add(row);
            }

            GridData model = GetArticleGroupGrid(rows, false);
            model.State = state;
            model.State.TotalRow = model.RowCount;

            return model;
        }

        #endregion

        #region Добавление / редактирование позиций

        public WriteoffWaybillRowEditViewModel AddRow(Guid writeoffWaybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var writeoffWaybill = writeoffWaybillService.CheckWaybillExistence(writeoffWaybillId, user);

                writeoffWaybillService.CheckPossibilityToEdit(writeoffWaybill, user);

                var model = new WriteoffWaybillRowEditViewModel()
                {
                    Title = "Добавление позиции в накладную",
                    AvailableToReserveFromStorageCount = "---",
                    ArticleName = "Выберите товар",
                    AvailableToReserveCount = "---",
                    BatchName = "не выбрана",
                    MeasureUnitName = "",
                    MeasureUnitScale = "0",
                    WriteoffWaybillDate = writeoffWaybill.Date.ToString(),
                    MarkupPercent = "---",
                    MarkupSum = "---",
                    AvailableToReserveFromPendingCount = "---",
                    PurchaseCost = "---",
                    SenderAccountingPrice = "---",
                    SenderStorageId = writeoffWaybill.SenderStorage.Id.ToString(),
                    SenderId = writeoffWaybill.Sender.Id.ToString(),
                    WritingoffCount = "",
                    AllowToEdit = true,
                    AllowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere)
                };

                return model;
            }
        }

        #region Добавление позиций списком

        public OutgoingWaybillAddRowsByListViewModel AddRowsByList(Guid waybillId, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = writeoffWaybillService.CheckWaybillExistence(waybillId, user);

                writeoffWaybillService.CheckPossibilityToEdit(waybill, user);

                var model = new OutgoingWaybillAddRowsByListViewModel()
                {
                    Id = waybill.Id.ToString(),
                    Name = waybill.Name,
                    BackURL = backURL,
                    StorageId = waybill.SenderStorage.Id.ToString(),
                    AccountOrganizationId = waybill.Sender.Id.ToString()
                };

                model.Filter.Items.Add(new FilterTextEditor("Number", "Артикул"));
                model.Filter.Items.Add(new FilterHyperlink("ArticleGroup", "Группа товара", "Выберите группу"));
                model.Filter.Items.Add(new FilterTextEditor("FullName", "Наименование"));
                model.Filter.Items.Add(new FilterTextEditor("Id", "Код товара"));
                model.Filter.Items.Add(new FilterYesNoToggle("OnlyAvailable", "Только наличие", true));

                model.ArticleGrid = GetArticlesForWaybillRowsAdditionByListGridLocal(new GridState()
                {
                    Parameters = "StorageId=" + waybill.SenderStorage.Id +
                        ";WaybillId=" + waybill.Id +
                        ";AccountOrganizationId=" + waybill.Sender.Id +
                        ";ArticleTakingsInfo=" + GetArticlesForWaybillRowsAdditionByListGridData(waybill) +
                        ";ActionName=/WriteoffWaybill/ShowArticlesForWaybillRowsAdditionByListGrid/",
                    Sort = "Number=Asc"
                }, user);

                model.RowGrid = GetWriteoffWaybillRowGridLocal(new GridState() { Parameters = "WriteoffWaybillId=" + waybill.Id, Sort = "CreationDate=Asc" }, user);

                return model;
            }
        }

        public GridData GetArticlesForWaybillRowsAdditionByListGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ParameterString deriveParams = new ParameterString(state.Parameters);

                ValidationUtils.NotNull(deriveParams["WaybillId"], "Накладная не указана.");
                var waybill = writeoffWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid(deriveParams["WaybillId"].Value.ToString()), user);

                //убран вызов временно, чтобы фильтр нормально фильтровал в случае, когда открываем в одном окне добавление товаров списком, а в другом окне проводим накладную
                //writeoffWaybillService.CheckPossibilityToEdit(waybill, user);

                deriveParams["ArticleTakingsInfo"].Value = GetArticlesForWaybillRowsAdditionByListGridData(waybill);

                state.Parameters = deriveParams.ToString();

                return GetArticlesForWaybillRowsAdditionByListGridLocal(state, user);
            }
        }

        private string GetArticlesForWaybillRowsAdditionByListGridData(WriteoffWaybill waybill)
        {
            // информация об уже имеющихся товарах в накладной с группировкой по товару
            return string.Concat(waybill.Rows.GroupBy(x => x.Article)
                .Select(x => string.Format("{0}_{1}:", x.Key.Id, x.Sum(y => y.WritingoffCount).ForEdit())));
        }

        public void AddRowSimply(Guid waybillId, int articleId, decimal count, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = writeoffWaybillService.CheckWaybillExistence(waybillId, user);
                var article = articleService.CheckArticleExistence(articleId);

                writeoffWaybillService.AddRowSimply(waybill, article, count, user);

                uow.Commit();
            }
        }

        #endregion

        public WriteoffWaybillRowEditViewModel EditRow(Guid waybillId, Guid rowId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var writeoffWaybill = writeoffWaybillService.CheckWaybillExistence(waybillId, user);

                var writeoffWaybillRow = writeoffWaybill.Rows.FirstOrDefault(x => x.Id == rowId);
                ValidationUtils.NotNull(writeoffWaybillRow, "Позиция накладной списания не найдена. Возможно, она была удалена.");

                // получение расширенного наличия по партии
                var articleBatchAvailability = articleAvailabilityService.GetExtendedArticleBatchAvailability(writeoffWaybillRow.ReceiptWaybillRow,
                    writeoffWaybill.SenderStorage, writeoffWaybill.Sender, DateTime.Now);

                var indicators = writeoffWaybillIndicatorService.GetMainRowIndicators(writeoffWaybillRow, user);

                var allowToEdit = writeoffWaybillService.IsPossibilityToEdit(writeoffWaybill, user);

                var manualSourcesInfoString = "";
                if (writeoffWaybillRow.IsUsingManualSource)
                {
                    manualSourcesInfoString = SerializeWaybillRowManualSourceInfo(outgoingWaybillRowService.GetManualSources(writeoffWaybillRow.Id));
                }

                var purchaseCost = writeoffWaybillRow.ReceiptWaybillRow.PurchaseCost;
                var permissionToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

                var model = new WriteoffWaybillRowEditViewModel()
                {
                    Title = allowToEdit ? "Редактирование позиции накладной" : "Детали позиции накладной",
                    Id = writeoffWaybillRow.Id,
                    ReceiptWaybillRowId = writeoffWaybillRow.ReceiptWaybillRow.Id,
                    CurrentReceiptWaybillRowId = writeoffWaybillRow.ReceiptWaybillRow.Id,
                    ArticleId = writeoffWaybillRow.Article.Id,
                    AvailableToReserveFromStorageCount = articleBatchAvailability.AvailableToReserveFromStorageCount.ForDisplay(),
                    AvailableToReserveFromPendingCount = articleBatchAvailability.AvailableToReserveFromPendingCount.ForDisplay(),
                    AvailableToReserveCount = articleBatchAvailability.AvailableToReserveCount.ForDisplay(),
                    ArticleName = writeoffWaybillRow.Article.FullName,
                    BatchName = writeoffWaybillRow.ReceiptWaybillRow.BatchName,
                    MeasureUnitName = writeoffWaybillRow.Article.MeasureUnit.ShortName,
                    MeasureUnitScale = writeoffWaybillRow.ReceiptWaybillRow.ArticleMeasureUnitScale.ToString(),
                    WriteoffWaybillDate = writeoffWaybill.Date.ToString(),
                    MarkupPercent = permissionToViewPurchaseCost && indicators.SenderAccountingPrice.HasValue && purchaseCost != 0M ?
                        ((indicators.SenderAccountingPrice.Value - purchaseCost) / purchaseCost * 100).ForDisplay(ValueDisplayType.Percent) : "---",
                    MarkupSum = permissionToViewPurchaseCost && indicators.SenderAccountingPrice.HasValue ? (indicators.SenderAccountingPrice.Value - purchaseCost).ForDisplay(ValueDisplayType.Money) : "---",
                    PurchaseCost = permissionToViewPurchaseCost ? purchaseCost.ForDisplay(ValueDisplayType.Money) : "---",
                    SenderAccountingPrice = indicators.SenderAccountingPrice.ForDisplay(ValueDisplayType.Money),
                    SenderStorageId = writeoffWaybill.SenderStorage.Id.ToString(),
                    SenderId = writeoffWaybill.Sender.Id.ToString(),
                    WritingoffCount = writeoffWaybillRow.WritingoffCount.ForEdit(),

                    ManualSourcesInfo = manualSourcesInfoString,

                    AllowToEdit = allowToEdit,
                    AllowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere)
                };

                return model;
            }
        }

        public object GetRowInfo(Guid waybillId, Guid batchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var waybill = writeoffWaybillService.CheckWaybillExistence(waybillId, user);

                var batch = receiptWaybillService.GetRowById(batchId);
                var article = batch.Article;
                var storage = waybill.SenderStorage;

                var articleBatchAvailability = articleAvailabilityService.GetExtendedArticleBatchAvailability(batch,
                    storage, waybill.Sender, DateTime.Now);

                var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);
                var allowToViewAccPrice = user.HasPermissionToViewStorageAccountingPrices(storage);

                decimal? accountingPrice = null;
                if (allowToViewAccPrice)
                {
                    accountingPrice = articlePriceService.GetAccountingPrice(new List<short>() { storage.Id }, article.Id)[storage.Id];

                    ValidationUtils.NotNull(accountingPrice, String.Format("Не установлена учетная цена МХ-отправителя на товар «{0}».", article.FullName));
                }

                var purchaseCost = allowToViewPurchaseCost ? batch.PurchaseCost : (decimal?)null;

                decimal? markupPercent = null, markupSum = null;

                if (allowToViewPurchaseCost && allowToViewAccPrice)
                {
                    markupSum = accountingPrice - purchaseCost;
                    markupPercent = purchaseCost != 0M ? markupSum / purchaseCost * 100M : null;
                }

                var model = new WriteoffWaybillRowEditViewModel()
                {
                    AvailableToReserveFromStorageCount = articleBatchAvailability.AvailableToReserveFromStorageCount.ForDisplay(),
                    AvailableToReserveFromPendingCount = articleBatchAvailability.AvailableToReserveFromPendingCount.ForDisplay(),
                    AvailableToReserveCount = articleBatchAvailability.AvailableToReserveCount.ForDisplay(),
                    PurchaseCost = purchaseCost.ForDisplay(ValueDisplayType.Money),
                    SenderAccountingPrice = accountingPrice.ForDisplay(ValueDisplayType.Money),

                    MarkupPercent = markupPercent.ForDisplay(ValueDisplayType.Percent),
                    MarkupSum = markupSum.ForDisplay(ValueDisplayType.Money)
                };

                return model;
            }
        }

        public object SaveRow(WriteoffWaybillRowEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var writeoffWaybill = writeoffWaybillService.CheckWaybillExistence(model.WriteoffWaybillId, user);

                writeoffWaybillService.CheckPossibilityToEdit(writeoffWaybill, user);

                var receiptWaybillRow = receiptWaybillService.GetRowById(model.ReceiptWaybillRowId);

                WriteoffWaybillRow row = null;
                var writingoffCount = ValidationUtils.TryGetDecimal(model.WritingoffCount);

                // добавление
                if (model.Id == Guid.Empty)
                {
                    row = new WriteoffWaybillRow(receiptWaybillRow, writingoffCount);

                    if (!String.IsNullOrEmpty(model.ManualSourcesInfo))
                    {
                        var sourcesInfo = DeserializeWaybillRowManualSourceInfo(model.ManualSourcesInfo);
                        writeoffWaybillService.AddRow(writeoffWaybill, row, sourcesInfo, user);
                    }
                    else
                    {
                        writeoffWaybillService.AddRow(writeoffWaybill, row, user);
                    }
                }
                // редактирование
                else
                {
                    row = writeoffWaybill.Rows.FirstOrDefault(x => x.Id == model.Id);
                    ValidationUtils.NotNull(row, "Позиция накладной не найдена. Возможно, она была удалена.");

                    row.ReceiptWaybillRow = receiptWaybillRow; // Партию важно присваивать перед количеством, чтобы при смене товара проверялась точность количества уже по новому товару

                    row.WritingoffCount = writingoffCount;

                    if (!String.IsNullOrEmpty(model.ManualSourcesInfo))
                    {
                        var sourcesInfo = DeserializeWaybillRowManualSourceInfo(model.ManualSourcesInfo);
                        writeoffWaybillService.SaveRow(writeoffWaybill, row, sourcesInfo, user);
                    }
                    else
                    {
                        writeoffWaybillService.SaveRow(writeoffWaybill, row, user);
                    }
                }

                uow.Commit();

                return GetMainChangeableIndicators(writeoffWaybill, user);
            }
        }

        #endregion

        #region Удаление позиции

        public object DeleteRow(Guid writeoffWaybillId, Guid writeoffWaybillRowId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var writeoffWaybill = writeoffWaybillService.CheckWaybillExistence(writeoffWaybillId, user);

                var writeoffWaybillRow = writeoffWaybill.Rows.FirstOrDefault(x => x.Id == writeoffWaybillRowId);
                ValidationUtils.NotNull(writeoffWaybillRow, "Позиция накладной не найдена. Возможно, она была удалена.");

                writeoffWaybillService.DeleteRow(writeoffWaybill, writeoffWaybillRow, user);

                uow.Commit();

                return GetMainChangeableIndicators(writeoffWaybill, user);
            }
        }

        #endregion

        #region Документы по накладной

        public GridData GetDocumentGrid(GridState state = null)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                return GetDocumentGridLocal(state);
            }
        }

        private GridData GetDocumentGridLocal(GridState state = null)
        {
            if (state == null)
                state = new GridState();

            GridData model = new GridData();

            model.AddColumn("Id", "", Unit.Pixel(0), style: GridCellStyle.Hidden);
            model.AddColumn("Name", "Наименование документа", Unit.Percentage(40));
            model.AddColumn("CreatedBy", "Кто создал", Unit.Percentage(30));
            model.AddColumn("ChangedBy", "Последнее изменение", Unit.Percentage(30));

            ParameterString deriveParams = new ParameterString(state.Parameters);

            return model;
        }

        #endregion

        #endregion

        #region Подготовка / отмена готовности к проводке

        public object PrepareToAccept(Guid id, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = writeoffWaybillService.CheckWaybillExistence(id, user);

                writeoffWaybillService.PrepareToAccept(waybill, user);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        public object CancelReadinessToAccept(Guid id, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = writeoffWaybillService.CheckWaybillExistence(id, user);

                writeoffWaybillService.CancelReadinessToAccept(waybill, user);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        #endregion

        #region Проводка / отмена проводки

        public object Accept(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = writeoffWaybillService.CheckWaybillExistence(id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                writeoffWaybillService.Accept(waybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        public object CancelAcceptance(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = writeoffWaybillService.CheckWaybillExistence(id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                writeoffWaybillService.CancelAcceptance(waybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }
        #endregion

        #region Списание / отмена списания

        public object Writeoff(Guid writeoffWaybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = writeoffWaybillService.CheckWaybillExistence(writeoffWaybillId, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                writeoffWaybillService.Writeoff(waybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        public object CancelWriteoff(Guid writeoffWaybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = writeoffWaybillService.CheckWaybillExistence(writeoffWaybillId, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                writeoffWaybillService.CancelWriteoff(waybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получение значений основных пересчитываемых показателей
        /// </summary>
        /// <param name="writeoffWaybill"></param>
        /// <returns></returns>
        private object GetMainChangeableIndicators(WriteoffWaybill writeoffWaybill, User user)
        {
            var ind = writeoffWaybillIndicatorService.GetMainIndicators(writeoffWaybill, user, calculateReceivelessProfit: true);

            var permissionToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

            var j = new
            {
                MainDetails = GetMainDetails(writeoffWaybill, user),

                AllowToEdit = writeoffWaybillService.IsPossibilityToEdit(writeoffWaybill, user),
                AllowToDelete = writeoffWaybillService.IsPossibilityToDelete(writeoffWaybill, user),
                AllowToWriteoff = writeoffWaybillService.IsPossibilityToWriteoff(writeoffWaybill, user),
                IsPossibilityToWriteoff = writeoffWaybillService.IsPossibilityToWriteoff(writeoffWaybill, user, true),
                AllowToCancelWriteoff = writeoffWaybillService.IsPossibilityToCancelWriteoff(writeoffWaybill, user),
                AllowToPrintForms = writeoffWaybillService.IsPossibilityToPrintForms(writeoffWaybill, user),
                AllowToPrepareToAccept = writeoffWaybillService.IsPossibilityToPrepareToAccept(writeoffWaybill, user),
                IsPossibilityToPrepareToAccept = writeoffWaybillService.IsPossibilityToPrepareToAccept(writeoffWaybill, user, true),
                AllowToCancelReadinessToAccept = writeoffWaybillService.IsPossibilityToCancelReadinessToAccept(writeoffWaybill, user),
                AllowToAccept = writeoffWaybillService.IsPossibilityToAccept(writeoffWaybill, user),
                IsPossibilityToAccept = writeoffWaybillService.IsPossibilityToAccept(writeoffWaybill, user,true),
                AllowToCancelAcceptance = writeoffWaybillService.IsPossibilityToCancelAcceptance(writeoffWaybill, user),
                AllowToChangeCurator = writeoffWaybillService.IsPossibilityToChangeCurator(writeoffWaybill, user)
            };

            return j;
        }

        #endregion

        #region Печатные формы

        #region Накладная списания

        public WriteoffWaybillPrintingFormSettingsViewModel GetWriteoffWaybillPrintingFormSettings(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var waybill = writeoffWaybillService.CheckWaybillExistence(waybillId, user);

                var model = new WriteoffWaybillPrintingFormSettingsViewModel();
                model.AllowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);
                model.AllowToViewAccountingPrices = user.HasPermissionToViewStorageAccountingPrices(waybill.SenderStorage);
                model.PrintAccountingPrice = model.AllowToViewAccountingPrices;

                return model;
            }
        }

        public WriteoffWaybillPrintingFormViewModel GetWriteoffWaybillPrintingForm(WriteoffWaybillPrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var writeoffWaybill = writeoffWaybillService.CheckWaybillExistence(settings.WaybillId, user);

                writeoffWaybillService.CheckPossibilityToPrintForms(writeoffWaybill, user);

                if (!user.HasPermission(Permission.PurchaseCost_View_ForEverywhere))
                {
                    settings.PrintPurchaseCost = false;
                }

                if (!user.HasPermissionToViewStorageAccountingPrices(writeoffWaybill.SenderStorage))
                {
                    settings.PrintAccountingPrice = false;
                }

                var model = new WriteoffWaybillPrintingFormViewModel()
                {
                    Date = DateTime.Now.ToShortDateString(),
                    OrganizationName = writeoffWaybill.Sender.FullName,
                    SenderStorageName = writeoffWaybill.SenderStorage.Name,
                    Settings = settings,
                    Title = String.Format("Накладная списания {0}", writeoffWaybill.Name),
                    TotalAccountingPriceSum = writeoffWaybill.SenderAccountingPriceSum.ForDisplay(ValueDisplayType.Money),
                    TotalPurchaseSum = writeoffWaybill.PurchaseCostSum.ForDisplay(ValueDisplayType.Money),
                    WriteoffReason = writeoffWaybill.WriteoffReason.Name
                };

                var indicators = writeoffWaybillIndicatorService.GetMainRowsIndicators(writeoffWaybill, user);

                foreach (var row in writeoffWaybill.Rows.OrderBy(x => x.CreationDate))
                {
                    var ind = indicators[row.Id];

                    var articleAccountingPriceSum = ind.SenderAccountingPrice.HasValue ?
                        ind.SenderAccountingPrice.Value * row.WritingoffCount : 0;

                    var purchaseCost = row.ReceiptWaybillRow.PurchaseCost;
                    var permissionToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

                    var writeoffWaybillPrintingFormItem = new WriteoffWaybillPrintingFormItemViewModel()
                    {
                        AccountingPrice = ind.SenderAccountingPrice.ForDisplay(ValueDisplayType.Money),
                        AccountingPriceSum = ind.SenderAccountingPrice.ForDisplay(ValueDisplayType.Money),
                        ArticleName = row.Article.FullName,
                        BatchName = row.ReceiptWaybillRow.BatchName,
                        Count = row.WritingoffCount.ForDisplay(),
                        Id = row.Article.Id.ForDisplay(),
                        Number = row.Article.Number,
                        PurchaseCost = purchaseCost.ForDisplay(ValueDisplayType.Money),
                        PurchaseSum = (purchaseCost * row.WritingoffCount).ForDisplay(ValueDisplayType.Money),
                        PackSize = row.Article.PackSize.ForDisplay(),
                        PackCount = row.PackCount.ForDisplay(ValueDisplayType.PackCount),
                        Weight = row.Weight.ForDisplay(ValueDisplayType.Weight),
                        Volume = row.Volume.ForDisplay(ValueDisplayType.Volume)
                    };

                    model.Rows.Add(writeoffWaybillPrintingFormItem);
                }

                return model;
            }
        }

        #endregion

        #endregion

        #endregion
    }
}