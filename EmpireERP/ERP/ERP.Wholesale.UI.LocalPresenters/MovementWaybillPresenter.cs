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
using ERP.Wholesale.UI.ViewModels.MovementWaybill;
using ERP.Wholesale.UI.ViewModels.OutgoingWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;
using ERP.Wholesale.UI.ViewModels.PrintingForm.MovementWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.Т1;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class MovementWaybillPresenter : OutgoingWaybillPresenter<MovementWaybill>, IMovementWaybillPresenter
    {
        #region Поля

        private readonly IMovementWaybillService movementWaybillService;
        private readonly IMovementWaybillMainIndicatorService movementWaybillIndicatorService;
        private readonly IValueAddedTaxService valueAddedTaxService;
        private readonly IReceiptWaybillService receiptWaybillService;
        private readonly IOutgoingWaybillRowService outgoingWaybillRowService;

        #endregion

        #region Конструкторы

        public MovementWaybillPresenter(IUnitOfWorkFactory unitOfWorkFactory, IMovementWaybillService movementWaybillService, 
            IMovementWaybillMainIndicatorService movementWaybillIndicatorService, IStorageService storageService, 
            IAccountOrganizationService accountOrganizationService, IArticlePriceService articlePriceService, IValueAddedTaxService valueAddedTaxService, 
            IArticleAvailabilityService articleAvailabilityService, IReceiptWaybillService receiptWaybillService, 
            IOutgoingWaybillRowService outgoingWaybillRowService, IArticleService articleService, IUserService userService)
            : base(unitOfWorkFactory,movementWaybillService, userService, storageService, accountOrganizationService, 
            articleService, articlePriceService, articleAvailabilityService)
        {
            this.movementWaybillService = movementWaybillService;
            this.movementWaybillIndicatorService = movementWaybillIndicatorService;
            this.valueAddedTaxService = valueAddedTaxService;
            this.receiptWaybillService = receiptWaybillService;
            this.outgoingWaybillRowService = outgoingWaybillRowService;
        }
        #endregion

        #region Методы

        #region Список

        public MovementWaybillListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.MovementWaybill_List_Details);

                var model = new MovementWaybillListViewModel();
                model.ShippingPending = GetShippingPendingGridLocal(new GridState() { Sort = "Date=Desc;CreationDate=Desc" }, user);
                model.Shipped = GetShippedGridLocal(new GridState() { Sort = "Date=Desc;CreationDate=Desc" }, user);
                model.Receipted = GetReceiptedGridLocal(new GridState() { Sort = "Date=Desc;CreationDate=Desc" }, user);

                model.FilterData = GetFilterData(user);

                return model;
            }
        }

        private FilterData GetFilterData(User user)
        {
            var filterData = new FilterData();

            var senderStorageList = storageService.GetList(user, Permission.MovementWaybill_List_Details).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false);
            // место хранения получателя ограничивать по пользователю не надо
            var recipientStorageList = storageService.GetList().OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false);
            var accountOrganizationList = accountOrganizationService.GetList()
                .GetComboBoxItemList(a => a.ShortName, a => a.Id.ToString());

            filterData.Items.Add(new FilterDateRangePicker("Date", "Дата накладной"));
            filterData.Items.Add(new FilterTextEditor("Number", "№ накладной"));
            filterData.Items.Add(new FilterComboBox("SenderStorage", "Отправитель", senderStorageList));
            filterData.Items.Add(new FilterComboBox("Sender", "Организация отпр.", accountOrganizationList));
            filterData.Items.Add(new FilterComboBox("RecipientStorage", "Получатель", recipientStorageList));
            filterData.Items.Add(new FilterComboBox("Recipient", "Организация получ.", accountOrganizationList));

            return filterData;
        }

        public GridData GetShippingPendingGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetShippingPendingGridLocal(state, user);
            }
        }

        private GridData GetShippingPendingGridLocal(GridState state, User user)
        {
            GridData model = new GridData();
            model.AddColumn("IsAccepted", "Пр.", Unit.Pixel(22), align: GridColumnAlign.Center);
            model.AddColumn("Number", "Номер", Unit.Pixel(60), GridCellStyle.Link);
            model.AddColumn("Date", "Дата", Unit.Pixel(54));
            model.AddColumn("RecipientAccountingPriceSum", "Сумма в УЦ получ.", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("SenderStorageName", "Отправитель", Unit.Percentage(50));
            model.AddColumn("RecipientStorageName", "Получатель", Unit.Percentage(50));
            model.AddColumn("ShippingPercent", "Отгрузка", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("RecipientStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.MovementWaybill_Create_Edit);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            string filter = state.Filter;
            ParameterString deriveFilter = new ParameterString(state.Filter);
            var item = deriveFilter.Add("State", ParameterStringItem.OperationType.OneOf);

            deriveFilter["State"].Value = new List<string>()
            {
                MovementWaybillState.Draft.ValueToString(),
                MovementWaybillState.ReadyToAccept.ValueToString(),
                MovementWaybillState.ArticlePending.ValueToString(),
                MovementWaybillState.ConflictsInArticle.ValueToString(),
                MovementWaybillState.ReadyToShip.ValueToString()
            };
            state.Filter = deriveFilter.ToString();

            var rows = movementWaybillService.GetFilteredList(state, user);

            state.Filter = filter;

            foreach (var row in rows)
            {
                var indicators = movementWaybillIndicatorService.GetMainIndicators(row, user);

                GridRowStyle rowStyle;

                switch (row.State)
                {
                    case MovementWaybillState.ConflictsInArticle:
                        rowStyle = GridRowStyle.Error;
                        break;

                    case MovementWaybillState.ArticlePending:
                        rowStyle = GridRowStyle.Warning;
                        break;

                    default:
                        rowStyle = GridRowStyle.Normal;
                        break;
                }

                GridCell senderStorageCell = (storageService.IsPossibilityToViewDetails(row.SenderStorage, user) ? (GridCell)new GridLinkCell("SenderStorageName") { Value = row.SenderStorage.Name } :
                        new GridLabelCell("SenderStorageName") { Value = row.SenderStorage.Name });

                GridCell recipientStorageCell = (storageService.IsPossibilityToViewDetails(row.RecipientStorage, user) ? (GridCell)new GridLinkCell("RecipientStorageName") { Value = row.RecipientStorage.Name } :
                        new GridLabelCell("RecipientStorageName") { Value = row.RecipientStorage.Name });

                var stateStr = "";
                if (row.IsReadyToAccept)
                {
                    stateStr = "Г";
                }
                else if (row.AcceptanceDate != null)
                {
                    stateStr = "П";
                }
                
                model.AddRow(new GridRow(
                    new GridLabelCell("IsAccepted") { Value = stateStr },
                    new GridLinkCell("Number") { Value = StringUtils.PadLeftZeroes(row.Number, 8) },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("RecipientAccountingPriceSum") { Value = indicators.RecipientAccountingPriceSum.ForDisplay(ValueDisplayType.Money) },
                    senderStorageCell,
                    recipientStorageCell,
                    new GridLabelCell("ShippingPercent") { Value = movementWaybillIndicatorService.CalculateShippingPercent(row).ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() },
                    new GridHiddenCell("SenderStorageId") { Value = row.SenderStorage.Id.ToString() },
                    new GridHiddenCell("RecipientStorageId") { Value = row.RecipientStorage.Id.ToString() }
                ) { Style = rowStyle });
            }
            model.State = state;

            return model;
        }

        public GridData GetShippedGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetShippedGridLocal(state, user);
            }
        }

        private GridData GetShippedGridLocal(GridState state, User user)
        {
            GridData model = new GridData();
            model.AddColumn("Number", "Номер", Unit.Pixel(60), GridCellStyle.Link);
            model.AddColumn("Date", "Дата", Unit.Pixel(54));
            model.AddColumn("RecipientAccountingPriceSum", "Сумма в УЦ получ.", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("SenderStorageName", "Отправитель", Unit.Percentage(50));
            model.AddColumn("RecipientStorageName", "Получатель", Unit.Percentage(50));
            model.AddColumn("ShippingPercent", "Отгрузка", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("RecipientStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            string filter = state.Filter;
            ParameterString deriveFilter = new ParameterString(state.Filter);
            deriveFilter.Add("State", ParameterStringItem.OperationType.Eq, ((byte)MovementWaybillState.ShippedBySender).ToString());
            state.Filter = deriveFilter.ToString();

            var rows = movementWaybillService.GetFilteredList(state, user);

            state.Filter = filter;

            foreach (var row in rows)
            {
                var allowToViewRecipientAccountingPrices = user.HasPermissionToViewStorageAccountingPrices(row.RecipientStorage);

                GridCell senderStorageCell = (storageService.IsPossibilityToViewDetails(row.SenderStorage, user) ? (GridCell)new GridLinkCell("SenderStorageName") { Value = row.SenderStorage.Name } :
                        new GridLabelCell("SenderStorageName") { Value = row.SenderStorage.Name });

                GridCell recipientStorageCell = (storageService.IsPossibilityToViewDetails(row.RecipientStorage, user) ? (GridCell)new GridLinkCell("RecipientStorageName") { Value = row.RecipientStorage.Name } :
                        new GridLabelCell("RecipientStorageName") { Value = row.RecipientStorage.Name });

                model.AddRow(new GridRow(
                    new GridLinkCell("Number") { Value = StringUtils.PadLeftZeroes(row.Number, 8) },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("RecipientAccountingPriceSum")
                    {
                        Value = (allowToViewRecipientAccountingPrices ? (decimal?)row.RecipientAccountingPriceSum.Value : null).ForDisplay(ValueDisplayType.Money)
                    },
                    senderStorageCell,
                    recipientStorageCell,
                    new GridLabelCell("ShippingPercent") { Value = movementWaybillIndicatorService.CalculateShippingPercent(row).ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() },
                    new GridHiddenCell("SenderStorageId") { Value = row.SenderStorage.Id.ToString() },
                    new GridHiddenCell("RecipientStorageId") { Value = row.RecipientStorage.Id.ToString() }
                ));
            }
            model.State = state;

            return model;
        }

        public GridData GetReceiptedGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetReceiptedGridLocal(state, user);
            }
        }

        private GridData GetReceiptedGridLocal(GridState state, User user)
        {
            GridData model = new GridData();
            model.AddColumn("Number", "Номер", Unit.Pixel(60), GridCellStyle.Link);
            model.AddColumn("Date", "Дата", Unit.Pixel(54));
            model.AddColumn("RecipientAccountingPriceSum", "Сумма в УЦ получ.", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("SenderStorageName", "Отправитель", Unit.Percentage(50));
            model.AddColumn("RecipientStorageName", "Получатель", Unit.Percentage(50));
            model.AddColumn("ShippingPercent", "Отгрузка", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("RecipientStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            string filter = state.Filter;
            ParameterString deriveFilter = new ParameterString(state.Filter);
            var item = deriveFilter.Add("State", ParameterStringItem.OperationType.OneOf);
            deriveFilter["State"].Value = new List<string>()
            {
                ((byte)MovementWaybillState.ReceiptedAfterDivergences).ToString(),
                ((byte)MovementWaybillState.ReceiptedWithDivergences).ToString(),
                ((byte)MovementWaybillState.ReceiptedWithoutDivergences).ToString()
            };

            state.Filter = deriveFilter.ToString();

            var rows = movementWaybillService.GetFilteredList(state, user);

            state.Filter = filter;

            foreach (var row in rows)
            {
                var allowToViewRecipientAccountingPrices = user.HasPermissionToViewStorageAccountingPrices(row.RecipientStorage);

                GridCell senderStorageCell = (storageService.IsPossibilityToViewDetails(row.SenderStorage, user) ? (GridCell)new GridLinkCell("SenderStorageName") { Value = row.SenderStorage.Name } :
                        new GridLabelCell("SenderStorageName") { Value = row.SenderStorage.Name });

                GridCell recipientStorageCell = (storageService.IsPossibilityToViewDetails(row.RecipientStorage, user) ? (GridCell)new GridLinkCell("RecipientStorageName") { Value = row.RecipientStorage.Name } :
                        new GridLabelCell("RecipientStorageName") { Value = row.RecipientStorage.Name });

                model.AddRow(new GridRow(
                    new GridLinkCell("Number") { Value = StringUtils.PadLeftZeroes(row.Number, 8) },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("RecipientAccountingPriceSum")
                    {
                        Value = (allowToViewRecipientAccountingPrices ? (decimal?)row.RecipientAccountingPriceSum.Value : null).ForDisplay(ValueDisplayType.Money)
                    },
                    senderStorageCell,
                    recipientStorageCell,
                    new GridLabelCell("ShippingPercent") { Value = movementWaybillIndicatorService.CalculateShippingPercent(row).ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() },
                    new GridHiddenCell("SenderStorageId") { Value = row.SenderStorage.Id.ToString() },
                    new GridHiddenCell("RecipientStorageId") { Value = row.RecipientStorage.Id.ToString() }
                ));
            }
            model.State = state;

            return model;
        }

        #endregion

        #region Добавление / редактирование

        public MovementWaybillEditViewModel Create(string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.MovementWaybill_Create_Edit);

                var valueAddedTaxList = valueAddedTaxService.GetList();
                var defaultValue = valueAddedTaxList.Where(x => x.IsDefault == true).FirstOrDefault();

                var model = new MovementWaybillEditViewModel()
                {
                    Title = "Добавление накладной перемещения",
                    Date = DateTime.Today.ToShortDateString(),
                    Number = "",
                    AllowToGenerateNumber = true, 
                    IsAutoNumber = "1",
                    BackURL = backURL,
                    SenderStorageList = storageService.GetList(user, Permission.MovementWaybill_Create_Edit).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                        .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false),
                    RecipientStorageList = storageService.GetList().OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)    // ограничивать по пользователю не надо
                        .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false),
                    ValueAddedTaxList = valueAddedTaxList.GetParamComboBoxItemList(x => x.Name, x => x.Id.ToString(), x => x.Value.ForEdit(), emptyParamValue: "0"),
                    ValueAddedTaxId = (defaultValue != null) ? defaultValue.Id : (short)0,
                    CuratorId = currentUser.Id.ToString(),
                    CuratorName = currentUser.DisplayName,

                    AllowToEdit = true,
                    AllowToEditRecipientAndRecipientStorage = true,
                    AllowToEditSenderAndSenderStorage = true,
                    AllowToChangeValueAddedTax = true,
                    AllowToChangeCurator = user.HasPermission(Permission.MovementWaybill_Curator_Change)
                };

                return model;
            }
        }

        public MovementWaybillEditViewModel Edit(Guid id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var movementWaybill = movementWaybillService.CheckWaybillExistence(id, user);

                movementWaybillService.CheckPossibilityToEdit(movementWaybill, user);
                movementWaybillService.CheckPossibilityToEditRecipientAndRecipientStorage(movementWaybill, user);
                
                //if (!movementWaybillService.IsPossibilityToEdit(movementWaybill, user) && !movementWaybillService.IsPossibilityToEditRecipientAndRecipientStorage(movementWaybill, user))
                //{
                //    throw new Exception("Недостаточно прав для выполнения операции.");
                //}

                var isValueAddedTax = movementWaybill.Sender != movementWaybill.Recipient;

                var model = new MovementWaybillEditViewModel
                {
                    Title = "Редактирование накладной перемещения",
                    Id = movementWaybill.Id,
                    Date = movementWaybill.Date.ToShortDateString(),
                    Number = movementWaybill.Number,
                    IsAutoNumber = "0",
                    AllowToGenerateNumber = false,
                    Comment = movementWaybill.Comment,
                    BackURL = backURL,
                    SenderStorageList = storageService.GetList(user, Permission.MovementWaybill_Create_Edit).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                        .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false),
                    RecipientStorageList = storageService.GetList().OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)    // ограничивать по пользователю не надо
                        .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false),
                    SenderStorageId = movementWaybill.SenderStorage.Id,
                    SenderAccountOrganizationList = movementWaybill.SenderStorage.AccountOrganizations.GetComboBoxItemList(s => s.ShortName, s => s.Id.ToString()),
                    SenderId = movementWaybill.Sender.Id,
                    RecipientStorageId = movementWaybill.RecipientStorage.Id,
                    RecipientAccountOrganizationList = movementWaybill.RecipientStorage.AccountOrganizations.GetComboBoxItemList(s => s.ShortName, s => s.Id.ToString()),
                    RecipientId = movementWaybill.Recipient.Id,
                    Name = movementWaybill.Name,
                    ValueAddedTaxList = valueAddedTaxService.GetList().GetParamComboBoxItemList(x => x.Name, x => x.Id.ToString(), x => x.Value.ForEdit(), emptyParamValue: "0"),
                    ValueAddedTaxId = movementWaybill.ValueAddedTax.Id,
                    CuratorId = movementWaybill.Curator.Id.ToString(),
                    CuratorName = movementWaybill.Curator.DisplayName,

                    AllowToEdit = movementWaybillService.IsPossibilityToEdit(movementWaybill, user),
                    AllowToEditRecipientAndRecipientStorage = movementWaybillService.IsPossibilityToEditRecipientAndRecipientStorage(movementWaybill, user),
                    AllowToEditSenderAndSenderStorage = false,
                    AllowToChangeValueAddedTax = isValueAddedTax,
                    AllowToChangeCurator = movementWaybillService.IsPossibilityToChangeCurator(movementWaybill, user)
                };

                return model;
            }
        }

        public string Save(MovementWaybillEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var date = ValidationUtils.TryGetDate(model.Date, "Неверный формат даты накладной.");
                var senderStorage = storageService.CheckStorageExistence(model.SenderStorageId, user, Permission.MovementWaybill_Create_Edit, "Место хранения отправителя не найдено. Возможно, оно было удалено.");
                var recipientStorage = storageService.CheckStorageExistence(model.RecipientStorageId, "Место хранения получателя не найдено. Возможно, оно было удалено.");
                var sender = accountOrganizationService.CheckAccountOrganizationExistence(model.SenderId, "Организация отправителя не найдена. Возможно, она была удалена.");
                var recipient = accountOrganizationService.CheckAccountOrganizationExistence(model.RecipientId, "Организация получателя не найдена. Возможно, она была удалена.");
                var valueAddedTax = valueAddedTaxService.CheckExistence(model.ValueAddedTaxId);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var htmlComment = StringUtils.ToHtml(model.Comment);

                MovementWaybill movementWaybill = null;
                int curatorId = ValidationUtils.TryGetInt(model.CuratorId);

                bool isAutoNumber = ValidationUtils.TryGetBool(model.IsAutoNumber);

                if (model.Id == Guid.Empty)
                // создание
                {
                    user.CheckPermission(Permission.MovementWaybill_Create_Edit);
                    
                    var curator = userService.CheckUserExistence(curatorId, user, "Куратор не найден. Возможно, он был удален.");

                    if (isAutoNumber)
                    {
                        model.Number = "";
                    }

                    movementWaybill = new MovementWaybill(model.Number, date, senderStorage, sender, recipientStorage, recipient, valueAddedTax, curator, user, currentDateTime);

                    // если куратор не соответствует пользователю, то ...
                    if (curator != user)
                    {
                        user.CheckPermission(Permission.MovementWaybill_Curator_Change);    // ... проверяем права на смену куратора и ...
                        movementWaybillService.CheckPossibilityToViewDetailsByUser(movementWaybill, curator);   // ... видимость накладной куратору
                    }
                }
                else
                // редактирование
                {
                    movementWaybill = movementWaybillService.CheckWaybillExistence(model.Id, user);

                    ValidationUtils.Assert(valueAddedTax.Value == 0M || sender != recipient,
                        "Накладная, в которой организации-отправитель и получатель совпадают, не может иметь ненулевой НДС.");

                    if (model.Number != movementWaybill.Number || valueAddedTax != movementWaybill.ValueAddedTax || htmlComment != movementWaybill.Comment)
                    {
                        movementWaybillService.CheckPossibilityToEdit(movementWaybill, user);

                        movementWaybill.Number = model.Number;
                        movementWaybill.ValueAddedTax = valueAddedTax;
                    }

                    if (movementWaybill.RecipientStorage.Id != model.RecipientStorageId || movementWaybill.Recipient.Id != model.RecipientId)
                    {
                        movementWaybillService.CheckPossibilityToEditRecipientAndRecipientStorage(movementWaybill, user);

                        movementWaybill.RecipientStorage = recipientStorage;
                        movementWaybill.Recipient = recipient;
                    }
                    if (movementWaybill.Curator.Id != curatorId)
                    {
                        var curator = userService.CheckUserExistence(curatorId, user, "Куратор не найден. Возможно, он был удален.");

                        movementWaybillService.CheckPossibilityToChangeCurator(movementWaybill, user);
                        movementWaybillService.CheckPossibilityToViewDetailsByUser(movementWaybill, curator);   // Проверяем видимость накладной куратору

                        movementWaybill.Curator = curator;
                    }
                }
                movementWaybill.Comment = htmlComment;

                // Если новая организация-получатель и организация-отправитель совпадают, обнуляем НДС у всех позиций накладной (берем из самой накладной)
                if (movementWaybill.Sender == movementWaybill.Recipient)
                {
                    foreach (var row in movementWaybill.Rows)
                    {
                        row.ValueAddedTax = movementWaybill.ValueAddedTax;
                    }
                }

                movementWaybillService.Save(movementWaybill);

                uow.Commit();

                return movementWaybill.Id.ToString();
            }
        }

        public object GetAccountOrganizationsForRecipientStorage(short storageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var organizationList = from c in storageService.CheckStorageExistence(storageId).AccountOrganizations
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

        public object GetAccountOrganizationsForSenderStorage(short storageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var organizationList = from c in storageService.CheckStorageExistence(storageId, user, Permission.MovementWaybill_Create_Edit).AccountOrganizations
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

        #region Добавление позиций списком

        public OutgoingWaybillAddRowsByListViewModel AddRowsByList(Guid waybillId, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = movementWaybillService.CheckWaybillExistence(waybillId, user);

                movementWaybillService.CheckPossibilityToEdit(waybill, user);

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
                        ";ActionName=/MovementWaybill/ShowArticlesForWaybillRowsAdditionByListGrid/", 
                    Sort = "Number=Asc"
                }, user);

                model.RowGrid = GetMovementWaybillRowGridLocal(new GridState() { Parameters = "MovementWaybillId=" + waybill.Id, Sort = "CreationDate=Asc" }, user);

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
                var waybill = movementWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid(deriveParams["WaybillId"].Value.ToString()), user);

                //убран вызов временно, чтобы фильтр нормально фильтровал в случае, когда открываем в одном окне добавление товаров списком, а в другом окне проводим накладную
                //movementWaybillService.CheckPossibilityToEdit(waybill, user); 

                deriveParams["ArticleTakingsInfo"].Value = GetArticlesForWaybillRowsAdditionByListGridData(waybill);

                state.Parameters = deriveParams.ToString();

                return GetArticlesForWaybillRowsAdditionByListGridLocal(state, user);
            }
        }

        private string GetArticlesForWaybillRowsAdditionByListGridData(MovementWaybill waybill)
        {
            // информация об уже имеющихся товарах в накладной с группировкой по товару
            return string.Concat(waybill.Rows.GroupBy(x => x.Article)
                .Select(x => string.Format("{0}_{1}:", x.Key.Id, x.Sum(y => y.MovingCount).ForEdit())));
        }

        public void AddRowSimply(Guid waybillId, int articleId, decimal count, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = movementWaybillService.CheckWaybillExistence(waybillId, user);
                var article = articleService.CheckArticleExistence(articleId);

                movementWaybillService.AddRowSimply(waybill, article, count, user);

                uow.Commit();
            }
        }

        #endregion

        #region Детали накладной

        #region Детали общие

        public MovementWaybillDetailsViewModel Details(Guid id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = movementWaybillService.CheckWaybillExistence(id, user);

                var model = new MovementWaybillDetailsViewModel();
                model.Id = waybill.Id;
                model.BackURL = backURL;
                model.MainDetails = GetMainDetails(waybill, user);

                model.MovementWaybillRows = GetMovementWaybillRowGridLocal(new GridState() { Parameters = "MovementWaybillId=" + waybill.Id, Sort = "CreationDate=Asc" }, user);
                model.MovementWaybillArticleGroupsGridState = new GridState() { Parameters = "MovementWaybillId=" + waybill.Id };
                
                model.DocGrid = GetDocGrid(null, currentUser);

                model.AllowToEdit = movementWaybillService.IsPossibilityToEdit(waybill, user) || movementWaybillService.IsPossibilityToEditRecipientAndRecipientStorage(waybill, user);
                model.AllowToDelete = movementWaybillService.IsPossibilityToDelete(waybill, user);
                
                model.AllowToPrepareToAccept = movementWaybillService.IsPossibilityToPrepareToAccept(waybill, user);
                model.IsPossibilityToPrepareToAccept = movementWaybillService.IsPossibilityToPrepareToAccept(waybill, user, true);
                model.AllowToCancelReadinessToAccept = movementWaybillService.IsPossibilityToCancelReadinessToAccept(waybill, user);
                model.AllowToAccept = movementWaybillService.IsPossibilityToAccept(waybill, user);
                model.IsPossibilityToAccept = movementWaybillService.IsPossibilityToAccept(waybill, user, true);
                model.AllowToCancelAcceptance = movementWaybillService.IsPossibilityToCancelAcceptance(waybill, user);
                model.AllowToShip = movementWaybillService.IsPossibilityToShip(waybill, user);
                model.IsPossibilityToShip = movementWaybillService.IsPossibilityToShip(waybill, user, true);
                model.AllowToCancelShipping = movementWaybillService.IsPossibilityToCancelShipping(waybill, user);
                model.AllowToReceipt = movementWaybillService.IsPossibilityToReceipt(waybill, user);
                model.AllowToCancelReceipt = movementWaybillService.IsPossibilityToCancelReceipt(waybill, user);
                model.AllowToPrintForms = movementWaybillService.IsPossibilityToPrintForms(waybill, user);

                model.AllowToPrintCashMemoForm = model.AllowToPrintWaybillFormInBothPrices = 
                    movementWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(waybill, user) || 
                    movementWaybillService.IsPossibilityToPrintFormInRecipientAccountingPrices(waybill, user);

                model.AllowToPrintWaybillFormInSenderPrices = movementWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(waybill, user);
                model.AllowToPrintWaybillFormInRecipientPrices = movementWaybillService.IsPossibilityToPrintFormInRecipientAccountingPrices(waybill, user);
                
                model.AllowToPrintTORG12Form = model.AllowToPrintInvoiceForm = 
                    movementWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(waybill, user) || 
                    movementWaybillService.IsPossibilityToPrintFormInRecipientAccountingPrices(waybill, user) || 
                    movementWaybillService.IsPossibilityToPrintFormInPurchaseCosts(waybill, user);
                
                model.AllowToPrintT1Form = true;
                
                return model;
            }
        }

        private MovementWaybillMainDetailsViewModel GetMainDetails(MovementWaybill waybill, User user)
        {
            var model = new MovementWaybillMainDetailsViewModel();

            model.Number = waybill.Number;
            model.Date = waybill.Date.ToShortDateString();
            model.StateName = waybill.State.GetDisplayName();
            model.SenderStorageName = waybill.SenderStorage.Name;
            model.SenderStorageId = waybill.SenderStorage.Id.ToString();
            model.SenderName = waybill.Sender.ShortName;
            model.SenderId = waybill.Sender.Id.ToString();
            model.RecipientStorageName = waybill.RecipientStorage.Name;
            model.RecipientStorageId = waybill.RecipientStorage.Id.ToString();
            model.RecipientName = waybill.Recipient.ShortName;
            model.RecipientId = waybill.Recipient.Id.ToString();
            model.Comment = waybill.Comment;
            model.CuratorId = waybill.Curator.Id.ToString();
            model.CuratorName = waybill.Curator.DisplayName;
            model.PurchaseCostSum = (user.HasPermission(Permission.PurchaseCost_View_ForEverywhere) ? waybill.PurchaseCostSum.ForDisplay(ValueDisplayType.Money) : "---");
            model.TotalWeight = waybill.Weight.ForDisplay(ValueDisplayType.Weight);
            model.TotalVolume = waybill.Volume.ForDisplay(ValueDisplayType.Volume);

            model.CreatedById = waybill.CreatedBy.Id.ToString();
            model.CreatedByName = waybill.CreatedBy.DisplayName;
            model.CreationDate = String.Format("({0})", waybill.CreationDate.ToShortDateTimeString());
            model.AcceptedById = waybill.AcceptedBy != null ? waybill.AcceptedBy.Id.ToString() : "";
            model.AcceptedByName = waybill.AcceptedBy != null ? waybill.AcceptedBy.DisplayName : "";
            model.AcceptanceDate = waybill.AcceptedBy != null ? String.Format("({0})", waybill.AcceptanceDate.Value.ToShortDateTimeString()) : "";
            model.ShippedById = waybill.ShippedBy != null ? waybill.ShippedBy.Id.ToString() : "";
            model.ShippedByName = waybill.ShippedBy != null ? waybill.ShippedBy.DisplayName : "";
            model.ShippingDate = waybill.ShippedBy != null ? String.Format("({0})", waybill.ShippingDate.Value.ToShortDateTimeString()) : "";
            model.ReceiptedById = waybill.ReceiptedBy != null ? waybill.ReceiptedBy.Id.ToString() : "";
            model.ReceiptedByName = waybill.ReceiptedBy != null ? waybill.ReceiptedBy.DisplayName : "";
            model.ReceiptDate = waybill.ReceiptedBy != null ? String.Format("({0})",waybill.ReceiptDate.Value.ToShortDateTimeString()) : "";

            var indicators = movementWaybillIndicatorService.GetMainIndicators(waybill, user, true, true);

            var allowToViewSenderAccountingPrices = user.HasPermissionToViewStorageAccountingPrices(waybill.SenderStorage);
            model.SenderValueAddedTaxString = allowToViewSenderAccountingPrices ? VatUtils.GetValueAddedTaxString(indicators.SenderVatInfoList, waybill.ValueAddedTax.Value) : "---";

            var allowToViewRecipientAccountingPrices = user.HasPermissionToViewStorageAccountingPrices(waybill.RecipientStorage);
            model.RecipientValueAddedTaxString = allowToViewRecipientAccountingPrices ? VatUtils.GetValueAddedTaxString(indicators.RecipientVatInfoList, waybill.ValueAddedTax.Value) : "---";

            model.SenderAccountingPriceSum = indicators.SenderAccountingPriceSum.ForDisplay(ValueDisplayType.Money);
            model.RecipientAccountingPriceSum = indicators.RecipientAccountingPriceSum.ForDisplay(ValueDisplayType.Money);

            model.MovementMarkupPercent =
                (allowToViewRecipientAccountingPrices && allowToViewSenderAccountingPrices ? indicators.MovementMarkupPercent : null).ForDisplay(ValueDisplayType.Percent);

            model.MovementMarkupSum = (allowToViewRecipientAccountingPrices && allowToViewSenderAccountingPrices ? (decimal?)indicators.MovementMarkupSum : null).ForDisplay(ValueDisplayType.Money);

            model.RowCount = waybill.RowCount.ToString();
            model.ShippingPercent = movementWaybillIndicatorService.CalculateShippingPercent(waybill).ForDisplay(ValueDisplayType.Percent);

            model.AllowToViewCuratorDetails = userService.IsPossibilityToViewDetails(waybill.Curator, user);
            model.AllowToViewSenderStorageDetails = storageService.IsPossibilityToViewDetails(waybill.SenderStorage, user);
            model.AllowToViewRecipientStorageDetails = storageService.IsPossibilityToViewDetails(waybill.RecipientStorage, user);
            model.AllowToChangeCurator = movementWaybillService.IsPossibilityToChangeCurator(waybill, user);

            model.AllowToViewCreatedByDetails = userService.IsPossibilityToViewDetails(waybill.CreatedBy, user);
            model.AllowToViewAcceptedByDetails = userService.IsPossibilityToViewDetails(waybill.AcceptedBy, user);
            model.AllowToViewShippedByDetails = userService.IsPossibilityToViewDetails(waybill.ShippedBy, user);
            model.AllowToViewReceiptedByDetails = userService.IsPossibilityToViewDetails(waybill.ReceiptedBy, user);

            return model;
        }

        #endregion

        #region Список позиций и групп товаров

        public GridData GetMovementWaybillRowGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetMovementWaybillRowGridLocal(state, user);
            }
        }

        private GridData GetMovementWaybillRowGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = movementWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid(deriveParams["MovementWaybillId"].Value.ToString()), user);

            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(120));
            model.AddColumn("Batch", "Партия", Unit.Pixel(115));
            model.AddColumn("ArticleId", "Код", Unit.Pixel(30), align: GridColumnAlign.Right);
            model.AddColumn("ArticleNumber", "Артикул", Unit.Pixel(60));
            model.AddColumn("ArticleName", "Товар", Unit.Percentage(100));
            model.AddColumn("Weight", "Вес (кг)", Unit.Pixel(35), align: GridColumnAlign.Right);
            model.AddColumn("Volume", "Объем (м3)", Unit.Pixel(35), align: GridColumnAlign.Right);
            model.AddColumn("RecipientAccountingPrice", "Уч. цена поставки", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("ValueAddedTax", "Ставка НДС", Unit.Pixel(45), align: GridColumnAlign.Right);
            model.AddColumn("Sum", "Сумма", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("PackCount", "Кол-во ЕУ", Unit.Pixel(35), align: GridColumnAlign.Right);
            model.AddColumn("Count", "Кол-во", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("ShippedCount", "Отгрузка", Unit.Pixel(48), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToAddRow"] = movementWaybillService.IsPossibilityToEdit(waybill, user);

            // получение стиля строк грида
            var rowStyles = GetRowsStyle(movementWaybillService.GetRowStates(waybill));

            // получение основных индикаторов для списка позиций
            var mainIndicators = movementWaybillIndicatorService.GetMainRowIndicators(waybill, user, calculateValueAddedTaxSums: true);
            
            foreach (var row in waybill.Rows.OrderBy(x => x.CreationDate))
            {
                var actions = new GridActionCell("Action");
                if (movementWaybillService.IsPossibilityToEdit(waybill, user))
                {
                    actions.AddAction("Ред.", "edit_link");
                }
                else
                {
                    actions.AddAction("Детали", "details_link");
                }

                if (movementWaybillService.IsPossibilityToDeleteRow(row, user))
                {
                    actions.AddAction("Удал.", "delete_link");
                }

                if (row.AreSourcesDetermined)
                {
                    actions.AddAction("Источ.", "source_link");
                }
                
                var receiptWaybillRow = row.ReceiptWaybillRow;
                var article = row.Article;

                // получаем основные показатели для текущей позиции
                var ind = mainIndicators[row.Id];
                
                decimal? sum = ind.RecipientAccountingPrice != null ? Math.Round(ind.RecipientAccountingPrice.Value * row.MovingCount, 2) : (decimal?)null;

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Batch") { Value = receiptWaybillRow.BatchName },
                    new GridLabelCell("ArticleId") { Value = article.Id.ToString() },
                    new GridLabelCell("ArticleNumber") { Value = article.Number },
                    new GridLabelCell("ArticleName") { Value = article.FullName },
                    new GridLabelCell("Weight") { Value = row.Weight.ForDisplay(ValueDisplayType.Weight) },
                    new GridLabelCell("Volume") { Value = row.Volume.ForDisplay(ValueDisplayType.Volume) },
                    new GridLabelCell("RecipientAccountingPrice") { Value = ind.RecipientAccountingPrice.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("ValueAddedTax") { Value = row.ValueAddedTax.Name },
                    new GridLabelCell("Sum") { Value = sum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PackCount") { Value = row.PackCount.ForDisplay(ValueDisplayType.PackCount) },
                    new GridLabelCell("Count") { Value = row.MovingCount.ForDisplay() },
                    new GridLabelCell("ShippedCount") { Value = row.TotallyReservedCount.ForDisplay() },
                    new GridHiddenCell("Id") { Value = row.Id.ToString(), Key = "movementWaybillRowId" }
               ) { Style = rowStyles[row.Id] });
            }
            model.State = state;
            model.State.TotalRow = model.RowCount;

            return model;
        }

        /// <summary>
        /// Получить грид с группами товаров
        /// </summary>
        public GridData GetMovementWaybillArticleGroupGrid(GridState state, UserInfo currentUser)
        {

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetMovementWaybillArticleGroupGridLocal(state, user);
            }

        }

        private GridData GetMovementWaybillArticleGroupGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = movementWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid((deriveParams["MovementWaybillId"].Value.ToString())), user);

            var articleGroups = waybill.Rows.GroupBy(x => x.Article.ArticleGroup);

            var rows= new List<BaseWaybillArticleGroupRow>();

            // получение основных индикаторов для списка позиций
            var mainIndicators = movementWaybillIndicatorService.GetMainRowIndicators(waybill, user, calculateValueAddedTaxSums: true);

            foreach (var articleGroup in articleGroups.OrderBy(x => x.Key.Name))
            {
                var row = new BaseWaybillArticleGroupRow();

                row.Name = articleGroup.Key.Name;
                row.ArticleCount = articleGroup.Sum(x => x.MovingCount);
                row.PackCount = articleGroup.Sum(x => x.PackCount);
                
                //вычисляем сумму по группе и сумму ндс по группе
                foreach (var waybillRow in articleGroup)
                {
                    // получаем основные показатели для текущей позиции
                    var ind = mainIndicators[waybillRow.Id];
                    
                    var waybillRowSum = ind.RecipientAccountingPrice != null ? Math.Round(ind.RecipientAccountingPrice.Value * waybillRow.MovingCount, 2)
                                                                                  : (decimal?)null;
                    if(waybillRowSum.HasValue)
                    {
                        if(row.Sum.HasValue)
                            row.Sum += waybillRowSum;
                        else
                            row.Sum = waybillRowSum;
                    }

                    if(ind.RecipientValueAddedTaxSum.HasValue)
                    {
                        if(row.ValueAddedTaxSum.HasValue)
                            row.ValueAddedTaxSum += ind.RecipientValueAddedTaxSum;
                        else
                            row.ValueAddedTaxSum = ind.RecipientValueAddedTaxSum;
                    }
                       
                }
             
                rows.Add(row);
            }

            GridData model = GetArticleGroupGrid(rows);
            model.State = state;
            model.State.TotalRow = model.RowCount;


            return model;
        }

        #endregion

        #region Добавление / редактирование позиций накладной

        public MovementWaybillRowEditViewModel AddRow(Guid movementWaybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var movementWaybill = movementWaybillService.CheckWaybillExistence(movementWaybillId, user);

                movementWaybillService.CheckPossibilityToEdit(movementWaybill, user);

                var isValueAddedTax = movementWaybill.Sender != movementWaybill.Recipient;
                var allowToChangeValueAddedTax = isValueAddedTax;
                var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

                var model = new MovementWaybillRowEditViewModel()
                {
                    Title = "Добавление позиции в накладную",
                    ArticleName = "Выберите товар",
                    MeasureUnitName = "",
                    MeasureUnitScale = "0",
                    BatchName = "не выбрана",
                    MovementWaybillDate = movementWaybill.Date.ToString(),
                    AvailableToReserveFromStorageCount = "---",
                    AvailableToReserveCount = "---",
                    AvailableToReserveFromPendingCount = "---",
                    PurchaseCost = "---",
                    RecipientAccountingPriceString = "---",
                    SenderAccountingPriceString = "---",
                    MovementMarkupPercent = "---",
                    MovementMarkupSum = "---",
                    PurchaseMarkupPercent = "---",
                    PurchaseMarkupSum = "---",
                    SenderId = movementWaybill.Sender.Id.ToString(),
                    SenderStorageId = movementWaybill.SenderStorage.Id.ToString(),
                    RecipientStorageId = movementWaybill.RecipientStorage.Id.ToString(),
                    TotallyReserved = "0",
                    ValueAddedTaxList = valueAddedTaxService.GetList().GetParamComboBoxItemList(x => x.Name, x => x.Id.ToString(), x => x.Value.ForEdit(), emptyParamValue: "0"),
                    ValueAddedTaxId = movementWaybill.ValueAddedTax.Id,
                    SenderValueAddedTaxSum = (allowToViewPurchaseCost ? 0M : (decimal?)null).ForDisplay(ValueDisplayType.Money),
                    RecipientValueAddedTaxSum = (allowToViewPurchaseCost ? 0M : (decimal?)null).ForDisplay(ValueDisplayType.Money),

                    AllowToEdit = true,
                    AllowToViewPurchaseCost = allowToViewPurchaseCost,
                    AllowToChangeValueAddedTax = allowToChangeValueAddedTax
                };

                return model;
            }
        }

        public MovementWaybillRowEditViewModel EditRow(Guid waybillId, Guid waybillRowId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var movementWaybill = movementWaybillService.CheckWaybillExistence(waybillId, user);

                var allowToEdit = movementWaybillService.IsPossibilityToEdit(movementWaybill, user);
                var isValueAddedTax = movementWaybill.Sender != movementWaybill.Recipient;
                var allowToChangeValueAddedTax = allowToEdit && isValueAddedTax;

                var movementWaybillRow = movementWaybill.Rows.FirstOrDefault(x => x.Id == waybillRowId);
                ValidationUtils.NotNull(movementWaybillRow, "Позиция накладной не найдена. Возможно, она была удалена.");
                                
                // получаем основные показатели для позиции накладной
                var ind = movementWaybillIndicatorService.GetMainRowIndicators(movementWaybillRow, user, calculateValueAddedTaxSums: true, calculateMarkups: true);

                // получение расширенного наличия по партии
                var articleBatchAvailability = articleAvailabilityService.GetExtendedArticleBatchAvailability(movementWaybillRow.ReceiptWaybillRow,
                    movementWaybill.SenderStorage, movementWaybill.Sender, DateTime.Now);

                bool allowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

                var manualSourcesInfoString = "";
                if (movementWaybillRow.IsUsingManualSource)
                {
                    manualSourcesInfoString = SerializeWaybillRowManualSourceInfo(outgoingWaybillRowService.GetManualSources(movementWaybillRow.Id));
                }

                var model = new MovementWaybillRowEditViewModel()
                {
                    Title = movementWaybillService.IsPossibilityToEdit(movementWaybill, user) ? "Редактирование позиции накладной" : "Детали позиции накладной",
                    Id = movementWaybillRow.Id,
                    MovementWaybillId = movementWaybill.Id,
                    MovementWaybillDate = movementWaybill.Date.ToString(),
                    ReceiptWaybillRowId = movementWaybillRow.ReceiptWaybillRow.Id,
                    CurrentReceiptWaybillRowId = movementWaybillRow.ReceiptWaybillRow.Id,
                    RecipientStorageId = movementWaybill.RecipientStorage.Id.ToString(),
                    SenderStorageId = movementWaybill.SenderStorage.Id.ToString(),
                    SenderId = movementWaybill.Sender.Id.ToString(),
                    ArticleId = movementWaybillRow.Article.Id,
                    ArticleName = movementWaybillRow.Article.FullName,
                    MeasureUnitName = movementWaybillRow.Article.MeasureUnit.ShortName,
                    MeasureUnitScale = movementWaybillRow.ReceiptWaybillRow.ArticleMeasureUnitScale.ToString(),
                    BatchName = movementWaybillRow.ReceiptWaybillRow.BatchName,
                    PurchaseCost = (allowToViewPurchaseCosts ? movementWaybillRow.ReceiptWaybillRow.PurchaseCost.ForDisplay(ValueDisplayType.Money) : "---"),
                    SenderAccountingPriceString = ind.SenderAccountingPrice.ForDisplay(ValueDisplayType.Money),
                    SenderAccountingPriceValue = ind.SenderAccountingPrice.ForEdit(),
                    RecipientAccountingPriceString = ind.RecipientAccountingPrice.ForDisplay(ValueDisplayType.Money),
                    RecipientAccountingPriceValue = ind.RecipientAccountingPrice.ForEdit(),
                    MovementMarkupPercent = ind.MovementMarkupPercent.ForDisplay(ValueDisplayType.Percent),
                    MovementMarkupSum = ind.MovementMarkupSum.ForDisplay(ValueDisplayType.Money),
                    PurchaseMarkupPercent = allowToViewPurchaseCosts ? ind.PurchaseMarkupPercent.ForDisplay(ValueDisplayType.Percent) : "---",
                    PurchaseMarkupSum = allowToViewPurchaseCosts ? ind.PurchaseMarkupSum.ForDisplay(ValueDisplayType.Money) : "---",
                    AvailableToReserveFromStorageCount = articleBatchAvailability.AvailableToReserveFromStorageCount.ForDisplay(),
                    AvailableToReserveFromPendingCount = articleBatchAvailability.AvailableToReserveFromPendingCount.ForDisplay(),
                    AvailableToReserveCount = articleBatchAvailability.AvailableToReserveCount.ForDisplay(),
                    MovingCount = movementWaybillRow.MovingCount.ForEdit(),
                    TotallyReserved = movementWaybillRow.TotallyReservedCount.ForEdit(),
                    ValueAddedTaxList = valueAddedTaxService.GetList().GetParamComboBoxItemList(x => x.Name, x => x.Id.ToString(), x => x.Value.ForEdit(), emptyParamValue: "0"),
                    ValueAddedTaxId = movementWaybillRow.ValueAddedTax.Id,
                    SenderValueAddedTaxSum = ind.SenderValueAddedTaxSum.ForDisplay(ValueDisplayType.Money),
                    RecipientValueAddedTaxSum = ind.RecipientValueAddedTaxSum.ForDisplay(ValueDisplayType.Money),
                    ManualSourcesInfo = manualSourcesInfoString,

                    AllowToEdit = allowToEdit,
                    AllowToViewPurchaseCost = allowToViewPurchaseCosts,
                    AllowToChangeValueAddedTax = allowToChangeValueAddedTax
                };

                return model;
            }
        }

        public object GetRowInfo(Guid waybillId, Guid batchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                
                var waybill = movementWaybillService.CheckWaybillExistence(waybillId, user);

                var senderStorage = waybill.SenderStorage;
                var recipientStorage = waybill.RecipientStorage;
                var sender = waybill.Sender;
                var batch = receiptWaybillService.GetRowById(batchId);
                var article = batch.Article;

                bool allowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);
                
                var allowToViewSenderAccPrices = user.HasPermissionToViewStorageAccountingPrices(senderStorage);
                var allowToViewRecipientAccPrices = user.HasPermissionToViewStorageAccountingPrices(recipientStorage);
                var allowToViewBothAccPrices = allowToViewRecipientAccPrices && allowToViewSenderAccPrices;

                var accountingPrices = new DynamicDictionary<int, decimal?>();

                if (allowToViewRecipientAccPrices || allowToViewSenderAccPrices)
                {
                    accountingPrices = articlePriceService.GetAccountingPrice(new List<short>() { senderStorage.Id, recipientStorage.Id }, article.Id);

                    ValidationUtils.NotNull(accountingPrices[senderStorage.Id], String.Format("Не установлена учетная цена МХ-отправителя на товар «{0}».", article.FullName));
                    ValidationUtils.NotNull(accountingPrices[recipientStorage.Id], String.Format("Не установлена учетная цена МХ-получателя на товар «{0}».", article.FullName));
                }

                var senderAccountingPrice = allowToViewSenderAccPrices ? accountingPrices[senderStorage.Id] : (decimal?)null;
                var recipientAccountingPrice = allowToViewRecipientAccPrices ? accountingPrices[recipientStorage.Id] : (decimal?)null;
                var purchaseCost = allowToViewPurchaseCosts ? batch.PurchaseCost : (decimal?)null;

                decimal? movementMarkupPercent = allowToViewBothAccPrices && accountingPrices[senderStorage.Id].Value != 0M ?
                    (((accountingPrices[recipientStorage.Id].Value - accountingPrices[senderStorage.Id].Value) / accountingPrices[senderStorage.Id].Value) * 100M) :
                    (decimal?)null;

                var availability = articleAvailabilityService.GetExtendedArticleBatchAvailability(batch, senderStorage, sender, DateTime.Now);

                var purchaseMarkupSum = allowToViewRecipientAccPrices && allowToViewPurchaseCosts ? (recipientAccountingPrice - purchaseCost) : (decimal?)null;
                var purchaseMarkupPercent = purchaseMarkupSum.HasValue && purchaseCost.HasValue && purchaseCost != 0M ? purchaseMarkupSum / purchaseCost : (decimal?)null;

                return new
                {
                    PurchaseCost = (allowToViewPurchaseCosts ? batch.PurchaseCost : (decimal?)null).ForDisplay(ValueDisplayType.Money),
                    SenderAccountingPrice = (allowToViewSenderAccPrices ? accountingPrices[senderStorage.Id].Value : (decimal?)null).ForDisplay(ValueDisplayType.Money),
                    RecipientAccountingPrice = (allowToViewRecipientAccPrices ? accountingPrices[recipientStorage.Id].Value : (decimal?)null).ForDisplay(ValueDisplayType.Money),
                    MovementMarkupSum = (allowToViewBothAccPrices ? (accountingPrices[recipientStorage.Id].Value - accountingPrices[senderStorage.Id].Value) : (decimal?)null).ForDisplay(ValueDisplayType.Money),
                    MovementMarkupPercent = movementMarkupPercent.ForDisplay(ValueDisplayType.Percent),
                    AvailableToReserveFromStorageCount = availability.AvailableToReserveFromStorageCount.ForDisplay(),
                    AvailableToReserveFromPendingCount = availability.AvailableToReserveFromPendingCount.ForDisplay(),
                    AvailableToReserveCount = availability.AvailableToReserveCount.ForDisplay(),                    
                    PurchaseMarkupSum = purchaseMarkupSum.ForDisplay(ValueDisplayType.Money),
                    PurchaseMarkupPercent = purchaseMarkupPercent.ForDisplay(ValueDisplayType.Percent)
                };
            }
        }

        public object SaveRow(MovementWaybillRowEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var movementWaybill = movementWaybillService.CheckWaybillExistence(model.MovementWaybillId, user);

                movementWaybillService.CheckPossibilityToEdit(movementWaybill, user);

                var receiptWaybillRow = receiptWaybillService.GetRowById(model.ReceiptWaybillRowId);

                MovementWaybillRow row = null;
                var movingCount = ValidationUtils.TryGetDecimal(model.MovingCount);

                var valueAddedTax = valueAddedTaxService.CheckExistence(model.ValueAddedTaxId);

                // добавление
                if (model.Id == Guid.Empty)
                {
                    row = new MovementWaybillRow(receiptWaybillRow, movingCount, valueAddedTax);

                    if (!String.IsNullOrEmpty(model.ManualSourcesInfo))
                    {
                        var sourcesInfo = DeserializeWaybillRowManualSourceInfo(model.ManualSourcesInfo);
                        movementWaybillService.AddRow(movementWaybill, row, sourcesInfo, user);
                    }
                    else
                    {
                        movementWaybillService.AddRow(movementWaybill, row, user);
                    }
                }
                // редактирование
                else
                {
                    row = movementWaybill.Rows.FirstOrDefault(x => x.Id == model.Id);
                    ValidationUtils.NotNull(row, "Позиция накладной не найдена. Возможно, она была удалена.");

                    row.ValueAddedTax = valueAddedTax;
                    row.ReceiptWaybillRow = receiptWaybillRow; // Партию важно присваивать перед количеством, чтобы при смене товара проверялась точность количества уже по новому товару
                    row.MovingCount = movingCount;                    

                    if (!String.IsNullOrEmpty(model.ManualSourcesInfo))
                    {
                        var sourcesInfo = DeserializeWaybillRowManualSourceInfo(model.ManualSourcesInfo);
                        movementWaybillService.SaveRow(movementWaybill, row, sourcesInfo, user);
                    }
                    else
                    {
                        movementWaybillService.SaveRow(movementWaybill, row, user);
                    }                    
                }

                uow.Commit();

                return GetMainChangeableIndicators(movementWaybill, user);
            }
        }

        public object DeleteRow(Guid movementWaybillId, Guid movementWaybillRowId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var movementWaybill = movementWaybillService.CheckWaybillExistence(movementWaybillId, user);

                var movementWaybillRow = movementWaybill.Rows.FirstOrDefault(x => x.Id == movementWaybillRowId);
                ValidationUtils.NotNull(movementWaybillRow, "Позиция накладной не найдена. Возможно, она была удалена.");

                movementWaybillService.DeleteRow(movementWaybill, movementWaybillRow, user);

                uow.Commit();

                return GetMainChangeableIndicators(movementWaybill, user);
            }
        }

        #endregion

        #region Документы по накладной

        public GridData GetDocGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetDocGridLocal(state, user);
            }
        }

        private GridData GetDocGridLocal(GridState state, User user)
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

        #region Подготовка/ отмена готовности к проводке накладной

        public object PrepareToAccept(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var movementWaybill = movementWaybillService.CheckWaybillExistence(id, user);

                movementWaybillService.PrepareToAccept(movementWaybill, user);

                uow.Commit();

                return GetMainChangeableIndicators(movementWaybill, user);
            }
        }

        public object CancelReadinessToAccept(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var movementWaybill = movementWaybillService.CheckWaybillExistence(id, user);

                movementWaybillService.CancelReadinessToAccept(movementWaybill, user);

                uow.Commit();

                return GetMainChangeableIndicators(movementWaybill, user);
            }
        }

        #endregion

        #region Проводка / отмена проводки

        public object Accept(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var movementWaybill = movementWaybillService.CheckWaybillExistence(id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                movementWaybillService.Accept(movementWaybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(movementWaybill, user);
            }
        }

        public object CancelAcceptance(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var movementWaybill = movementWaybillService.CheckWaybillExistence(id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                movementWaybillService.CancelAcceptance(movementWaybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(movementWaybill, user);
            }
        }
        #endregion

        #region Отгрузка / отмена отгрузки

        public object Ship(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var movementWaybill = movementWaybillService.CheckWaybillExistence(id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                movementWaybillService.Ship(movementWaybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(movementWaybill, user);
            }
        }

        public object CancelShipping(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var movementWaybill = movementWaybillService.CheckWaybillExistence(id, user);

                movementWaybillService.CancelShipping(movementWaybill, user);

                uow.Commit();

                return GetMainChangeableIndicators(movementWaybill, user);
            }
        }
        #endregion

        #region Приемка / отмена приемки

        public object Receipt(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var movementWaybill = movementWaybillService.CheckWaybillExistence(id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                movementWaybillService.Receipt(movementWaybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(movementWaybill, user);
            }
        }

        public object CancelReceipt(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var movementWaybill = movementWaybillService.CheckWaybillExistence(id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                movementWaybillService.CancelReceipt(movementWaybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(movementWaybill, user);
            }
        }

        #endregion

        #region Удаление

        public void Delete(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var movementWaybill = movementWaybillService.CheckWaybillExistence(id, user);

                movementWaybillService.Delete(movementWaybill, user);

                uow.Commit();
            }
        }

        #endregion

        /// <summary>
        /// Получение значений основных пересчитываемых показателей
        /// </summary>
        /// <param name="movementWaybill"></param>
        /// <returns></returns>
        private object GetMainChangeableIndicators(MovementWaybill movementWaybill, User user)
        {
            var j = new
            {
                MainDetails = GetMainDetails(movementWaybill, user),

                AllowToEdit = movementWaybillService.IsPossibilityToEdit(movementWaybill, user) || movementWaybillService.IsPossibilityToEditRecipientAndRecipientStorage(movementWaybill, user),
                AllowToAddRow = movementWaybillService.IsPossibilityToEdit(movementWaybill, user),
                AllowToDelete = movementWaybillService.IsPossibilityToDelete(movementWaybill, user),
                AllowToPrepareToAccept = movementWaybillService.IsPossibilityToPrepareToAccept(movementWaybill, user),
                IsPossibilityToPrepareToAccept = movementWaybillService.IsPossibilityToPrepareToAccept(movementWaybill, user, true),
                AllowToCancelReadinessToAccept = movementWaybillService.IsPossibilityToCancelReadinessToAccept(movementWaybill, user),
                AllowToAccept = movementWaybillService.IsPossibilityToAccept(movementWaybill, user),
                IsPossibilityToAccept = movementWaybillService.IsPossibilityToAccept(movementWaybill, user, true),
                AllowToCancelAcceptance = movementWaybillService.IsPossibilityToCancelAcceptance(movementWaybill, user),
                AllowToShip = movementWaybillService.IsPossibilityToShip(movementWaybill, user),
                IsPossibilityToShip = movementWaybillService.IsPossibilityToShip(movementWaybill, user, true),
                AllowToCancelShipping = movementWaybillService.IsPossibilityToCancelShipping(movementWaybill, user),
                AllowToReceipt = movementWaybillService.IsPossibilityToReceipt(movementWaybill, user),
                AllowToCancelReceipt = movementWaybillService.IsPossibilityToCancelReceipt(movementWaybill, user),
                AllowToPrintForms = movementWaybillService.IsPossibilityToPrintForms(movementWaybill, user),

                AllowToPrintCashMemoForm =  
                    movementWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(movementWaybill, user) || 
                    movementWaybillService.IsPossibilityToPrintFormInRecipientAccountingPrices(movementWaybill, user),
                AllowToPrintWaybillFormInBothPrices = 
                    movementWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(movementWaybill, user) || 
                    movementWaybillService.IsPossibilityToPrintFormInRecipientAccountingPrices(movementWaybill, user),

                AllowToPrintWaybillFormInSenderPrices = movementWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(movementWaybill, user),
                AllowToPrintWaybillFormInRecipientPrices = movementWaybillService.IsPossibilityToPrintFormInRecipientAccountingPrices(movementWaybill, user),
                
                AllowToPrintTORG12Form =  
                    movementWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(movementWaybill, user) || 
                    movementWaybillService.IsPossibilityToPrintFormInRecipientAccountingPrices(movementWaybill, user) || 
                    movementWaybillService.IsPossibilityToPrintFormInPurchaseCosts(movementWaybill, user),

                AllowToPrintInvoiceForm = 
                    movementWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(movementWaybill, user) || 
                    movementWaybillService.IsPossibilityToPrintFormInRecipientAccountingPrices(movementWaybill, user) || 
                    movementWaybillService.IsPossibilityToPrintFormInPurchaseCosts(movementWaybill, user),

                AllowToChangeCurator = movementWaybillService.IsPossibilityToChangeCurator(movementWaybill, user)
            };

            return j;
        }

        #region Печатные формы

        #region Товарный чек

        /// <summary>
        /// Получение модели параметров ПФ "Товарный чек"
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public CashMemoPrintingFormSettingsViewModel GetCashMemoPrintingFormSettings(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = movementWaybillService.CheckWaybillExistence(waybillId, user);

                var allowToViewSenderAccountingPrices = movementWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(waybill, user);
                var allowToViewRecipientAccountingPrices = movementWaybillService.IsPossibilityToPrintFormInRecipientAccountingPrices(waybill, user);

                ValidationUtils.Assert(allowToViewSenderAccountingPrices || allowToViewRecipientAccountingPrices, 
                    "Нет прав на просмотр учетных цен ни отправителя ни получателя.");

                var priceTypes = new List<PrintingFormPriceType>();
                if (allowToViewSenderAccountingPrices) priceTypes.Add(PrintingFormPriceType.SenderAccountingPrice);
                if (allowToViewRecipientAccountingPrices) priceTypes.Add(PrintingFormPriceType.RecipientAccountingPrice);

                var model = new CashMemoPrintingFormSettingsViewModel()
                {
                    PriceTypes = priceTypes.GetComboBoxItemList(s => s.GetDisplayName(), s => s.ValueToString(), false, false)
                };

                return model;
            }
        }

        public CashMemoPrintingFormViewModel GetCashMemoPrintingForm(CashMemoPrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {                
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = movementWaybillService.CheckWaybillExistence(settings.WaybillId, user);

                var priceType = ValidationUtils.TryGetEnum<PrintingFormPriceType>(settings.PriceTypeId);
                switch (priceType)
                {
                    case PrintingFormPriceType.SenderAccountingPrice:
                        movementWaybillService.CheckPossibilityToPrintFormInSenderAccountingPrices(waybill, user);
                        break;
                    case PrintingFormPriceType.RecipientAccountingPrice:
                        movementWaybillService.CheckPossibilityToPrintFormInRecipientAccountingPrices(waybill, user);
                        break;                    
                    default:
                        throw new Exception("Неверное значение типа цен, в которых печается отчет.");
                }

                var model = new CashMemoPrintingFormViewModel();
                model.WaybillId = settings.WaybillId;
                model.Title = "ТОВАРНЫЙ ЧЕК";
                model.Number = waybill.Number;
                model.Date = waybill.Date.ToShortDateString();
                
                var organization = waybill.Sender;
                model.OrganizationName = organization.FullName;

                var organizationType = organization.EconomicAgent.Type;
                switch (organizationType)
                {
                    case EconomicAgentType.JuridicalPerson:
                        model.OGRN = (organization.EconomicAgent.As<JuridicalPerson>()).OGRN;
                        model.OGRN_Caption = "ОГРН";
                        break;
                    case EconomicAgentType.PhysicalPerson:
                        model.OGRN = (organization.EconomicAgent.As<PhysicalPerson>()).OGRNIP;
                        model.OGRN_Caption = "ОГРН ИП";
                        break;
                }

                var printingFormRows = new List<CashMemoPrintingFormItemViewModel>();
                decimal totalSum = 0;

                foreach (var row in waybill.Rows.OrderBy(x => x.CreationDate))
                {
                    var printingFormRow = new CashMemoPrintingFormItemViewModel();

                    var article = row.Article;
                    printingFormRow.ArticleName = article.Number + " " + article.FullName;
                    printingFormRow.Count = row.MovingCount.ForDisplay();

                    var price = priceType == PrintingFormPriceType.SenderAccountingPrice ? row.SenderArticleAccountingPrice.AccountingPrice : row.RecipientArticleAccountingPrice.AccountingPrice;

                    printingFormRow.Price = price.ForDisplay(ValueDisplayType.Money);
                    printingFormRow.PackSize = article.PackSize.ForDisplay();

                    var sum = row.MovingCount * price;

                    totalSum += sum;
                    printingFormRow.Sum = sum.ForDisplay(ValueDisplayType.Money);

                    printingFormRows.Add(printingFormRow);
                }

                model.Rows = printingFormRows;
                model.TotalSum = totalSum.ForDisplay(ValueDisplayType.Money);

                return model;
            }
        }

        #endregion

        #region Счет-фактура

        /// <summary>
        /// Получение модели параметров счет-фактуры
        /// </summary>
        public InvoicePrintingFormSettingsViewModel GetInvoicePrintingFormSettings(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = movementWaybillService.CheckWaybillExistence(waybillId, user);

                var allowToViewSenderAccountingPrices = movementWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(waybill, user);
                var allowToViewRecipientAccountingPrices = movementWaybillService.IsPossibilityToPrintFormInRecipientAccountingPrices(waybill, user);
                var allowToViewPurchaseCosts = movementWaybillService.IsPossibilityToPrintFormInPurchaseCosts(waybill, user);

                ValidationUtils.Assert(allowToViewSenderAccountingPrices || allowToViewRecipientAccountingPrices || allowToViewPurchaseCosts,
                    "Нет прав на просмотр ни учетных цен отправителя, ни учетных цен получателя, ни закупочных цен.");

                var priceTypes = new List<PrintingFormPriceType>();
                if (allowToViewSenderAccountingPrices) priceTypes.Add(PrintingFormPriceType.SenderAccountingPrice);
                if (allowToViewRecipientAccountingPrices) priceTypes.Add(PrintingFormPriceType.RecipientAccountingPrice);
                if (allowToViewPurchaseCosts) priceTypes.Add(PrintingFormPriceType.PurchaseCost);

                var model = new InvoicePrintingFormSettingsViewModel()
                {
                    PriceTypes = priceTypes.GetComboBoxItemList(s => s.GetDisplayName(), s => s.ValueToString(), false, false)
                };

                return model;
            }
        }

        public InvoicePrintingFormViewModel GetInvoicePrintingForm(InvoicePrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {                
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = movementWaybillService.CheckWaybillExistence(settings.WaybillId, user);

                var priceType = ValidationUtils.TryGetEnum<PrintingFormPriceType>(settings.PriceTypeId);
                
                // проверка возможности просмотра определенного типа цен
                CheckPriceTypePermissions(user, waybill, priceType);

                var model = new InvoicePrintingFormViewModel();
                model.PriceTypeId = settings.PriceTypeId;
                model.WaybillId = settings.WaybillId;
                model.Title = "СЧЕТ-ФАКТУРА";
                model.Number = waybill.Number;
                model.Date = waybill.Date.ToShortDateString();

                model.SellerName = waybill.Sender.ShortName;
                model.SellerAddress = waybill.Sender.Address;

                if (waybill.Sender.EconomicAgent.Type == EconomicAgentType.JuridicalPerson)
                {
                    var juridicalPerson = waybill.Sender.EconomicAgent.As<JuridicalPerson>();
                    model.SellerINN_KPP = juridicalPerson.INN + " / " + juridicalPerson.KPP;
                }
                else
                {
                    var physicalPerson = waybill.Sender.EconomicAgent.As<PhysicalPerson>();
                    model.SellerINN_KPP = physicalPerson.INN;
                }

                model.ArticleSenderInfo = "он же";
                model.ArticleRecipientInfo = String.Format("{0}, {1}", waybill.Recipient.ShortName, waybill.Recipient.Address);
                model.PaymentDocumentNumber = "-";

                model.BuyerName = waybill.Recipient.ShortName;
                model.BuyerAddress = waybill.Recipient.Address;

                if (waybill.Recipient.EconomicAgent.Type == EconomicAgentType.JuridicalPerson)
                {
                    var juridicalPerson = waybill.Recipient.EconomicAgent.As<JuridicalPerson>();
                    model.BuyerINN_KPP = juridicalPerson.INN + " / " + juridicalPerson.KPP;
                }
                else
                {
                    var physicalPerson = waybill.Recipient.EconomicAgent.As<PhysicalPerson>();
                    model.BuyerINN_KPP = physicalPerson.INN;
                }
                
                return model;
            }
        }

        /// <summary>
        /// Получение информации о строках печатной формы
        /// </summary>
        public InvoicePrintingFormRowsViewModel GetInvoicePrintingFormRows(InvoicePrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = movementWaybillService.CheckWaybillExistence(settings.WaybillId, user);

                var priceType = ValidationUtils.TryGetEnum<PrintingFormPriceType>(settings.PriceTypeId);

                // проверка возможности просмотра определенного типа цен
                CheckPriceTypePermissions(user, waybill, priceType);

                var model = new InvoicePrintingFormRowsViewModel();

                foreach (var row in waybill.Rows.OrderBy(x => x.CreationDate))
                {
                    var item = new InvoicePrintingFormItemViewModel();

                    var batch = row.ReceiptWaybillRow;
                    var article = batch.Article;

                    item.ArticleName = article.Number + " " + article.FullName;
                    item.MeasureUnitName = article.MeasureUnit.ShortName;
                    item.MeasureUnitCode = article.MeasureUnit.NumericCode;
                    item.CountValue = row.MovingCount;

                    decimal price = 0M, valueAddedTaxSum = 0M;
                    switch (priceType)
                    {
                        case PrintingFormPriceType.SenderAccountingPrice:
                            price = row.SenderArticleAccountingPrice.AccountingPrice;
                            valueAddedTaxSum = row.SenderValueAddedTaxSum;
                            break;
                        case PrintingFormPriceType.RecipientAccountingPrice:
                            price = row.RecipientArticleAccountingPrice.AccountingPrice;
                            valueAddedTaxSum = row.RecipientValueAddedTaxSum;
                            break;
                        case PrintingFormPriceType.PurchaseCost:
                            price = row.ReceiptWaybillRow.PurchaseCost;
                            valueAddedTaxSum = row.PurchaseCostValueAddedTaxSum;
                            break;
                    }

                    item.Price = VatUtils.CalculateWithoutVatSum(price, row.ValueAddedTax.Value).ForDisplay(ValueDisplayType.MoneyWithZeroCopecks);
                    item.ExciseValue = "без акциза";
                    item.TaxValue = row.ValueAddedTax.Value > 0 ? row.ValueAddedTax.Value.ForDisplay(ValueDisplayType.Percent) + "%" : "без НДС";

                    if (batch.ProductionCountry.NumericCode != "643") // столбцы 10, 10а, 11 заполняются только в случае, если  товар произведен не в России                    
                    {
                        item.CountryName = batch.ProductionCountry.Name;
                        item.CountryCode = batch.ProductionCountry.NumericCode;
                        item.CustomsDeclarationNumber = String.IsNullOrEmpty(batch.CustomsDeclarationNumber) ? "-" : batch.CustomsDeclarationNumber;
                    }

                    var taxedCost = Math.Round(price * row.MovingCount, (priceType == PrintingFormPriceType.PurchaseCost ? 6 : 2));
                    var taxSum = valueAddedTaxSum;
                    var cost = taxedCost - taxSum;

                    item.TaxedCostValue = taxedCost;
                    item.CostValue = cost;
                    item.TaxSumValue = taxSum;

                    model.Rows.Add(item);
                }

                return model;
            }
        }

        /// <summary>
        /// Выгрузка формы в Excel
        /// </summary>
        public byte[] ExportInvoicePrintingFormToExcel(InvoicePrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            return ExportInvoicePrintingFormToExcel(GetInvoicePrintingForm(settings, currentUser), GetInvoicePrintingFormRows(settings, currentUser));
        }

        #endregion

        #region Печатная форма

        /// <summary>
        /// Получение модели параметров печатных форм
        /// </summary>
        /// <param name="PrintSenderPrice"></param>
        /// <param name="PrintRecepientPrice"></param>
        /// <param name="PrintMarkup"></param>
        /// <returns>Модель параметров печатных форм</returns>
        public MovementWaybillPrintingFormSettingsViewModel GetPrintingFormSettings(bool PrintSenderPrice, bool PrintRecepientPrice, bool PrintMarkup, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new MovementWaybillPrintingFormSettingsViewModel()
                {
                    PrintSenderPrice = PrintSenderPrice,
                    PrintRecepientPrice = PrintRecepientPrice,
                    PrintMarkup = PrintMarkup,
                    AllowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere)
                };

                return model;
            }
        }

        /// <summary>
        /// Получение модели печатной формы
        /// </summary>
        /// <param name="settings">Параметры печатной формы</param>
        /// <returns>Модель печатной формы</returns>
        public MovementWaybillPrintingFormViewModel GetPrintingForm(MovementWaybillPrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var movementWaybill = movementWaybillService.CheckWaybillExistence(settings.WaybillId, user);

                movementWaybillService.CheckPossibilityToPrintWaybillForm(movementWaybill, settings.PrintSenderPrice, settings.PrintRecepientPrice, user);

                if (!user.HasPermission(Permission.PurchaseCost_View_ForEverywhere))
                {
                    settings.PrintPurchaseCost = false;
                }

                var model = new MovementWaybillPrintingFormViewModel()
                {
                    Settings = settings,
                    OrganizationName = movementWaybill.Recipient.FullName,
                    INN = movementWaybill.Recipient.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                        movementWaybill.Recipient.EconomicAgent.As<JuridicalPerson>().INN :
                        movementWaybill.Recipient.EconomicAgent.As<PhysicalPerson>().INN,
                    KPP = movementWaybill.Recipient.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                        movementWaybill.Recipient.EconomicAgent.As<JuridicalPerson>().KPP : "",
                    Address = movementWaybill.Recipient.Address,
                    SenderStorage = movementWaybill.SenderStorage.Name,
                    RecepientStorage = movementWaybill.RecipientStorage.Name,
                    Title = String.Format("Внутреннее перемещение {0}", movementWaybill.Name),
                    Date = DateTime.Now.ToShortDateString(),                    
                    RecepientStorageOrganization = movementWaybill.Sender.FullName
                };

                //Если учет по партиям
                if (model.Settings.DevideByBatch)
                {
                    bool possibilityToViewProducer = user.HasPermission(Permission.Producer_List_Details);
                    bool possibilityToViewProvider = user.HasPermission(Permission.Provider_List_Details);

                    var query = movementWaybill.Rows.OrderBy(x => x.CreationDate);
                    foreach (var row in query)
                    {
                        var senderArticleAccountingPriceSum = row.SenderArticleAccountingPrice != null ?
                            row.SenderArticleAccountingPrice.AccountingPrice * row.MovingCount : 0;
                        var recipientArticleAccountingPriceSum = row.RecipientArticleAccountingPrice != null ?
                            row.RecipientArticleAccountingPrice.AccountingPrice * row.MovingCount : 0;
                        bool possibilityToViewContractor = row.ReceiptWaybillRow.ReceiptWaybill.IsCreatedFromProductionOrderBatch ?
                            possibilityToViewProducer : possibilityToViewProvider;

                        var movementWaybillPrintingFormItem = new MovementWaybillPrintingFormItemViewModel()
                        {
                            Id = row.Article.Id.ForDisplay(),
                            Number = row.Article.Number,
                            ArticleName = row.Article.FullName,
                            Count = row.MovingCount.ForDisplay(),
                            PackSize = row.Article.PackSize.ForDisplay(),
                            PackCount = row.PackCount.ForDisplay(ValueDisplayType.PackCount),
                            MeasureUnit = row.Article.MeasureUnit.ShortName,
                            Weight = row.Weight.ForDisplay(ValueDisplayType.Weight),
                            Volume = row.Volume.ForDisplay(ValueDisplayType.Volume),
                            ContractorName = possibilityToViewContractor ? row.ReceiptWaybillRow.ReceiptWaybill.ContractorName : "---",
                            BatchName = row.ReceiptWaybillRow.BatchName
                        };

                        if (model.Settings.PrintPurchaseCost)
                        {
                            movementWaybillPrintingFormItem.PurchaseCost = row.ReceiptWaybillRow.PurchaseCost.ForDisplay(ValueDisplayType.Money);
                            movementWaybillPrintingFormItem.PurchaseSum = (row.MovingCount * row.ReceiptWaybillRow.PurchaseCost).ForDisplay(ValueDisplayType.Money);
                            model.TotalPurchaseSum += row.MovingCount * row.ReceiptWaybillRow.PurchaseCost;
                        }

                        if (model.Settings.PrintSenderPrice)
                        {
                            movementWaybillPrintingFormItem.SenderPrice = row.SenderArticleAccountingPrice != null ?
                            row.SenderArticleAccountingPrice.AccountingPrice.ForDisplay(ValueDisplayType.Money) : "---";
                            movementWaybillPrintingFormItem.SenderPriceSum = row.SenderArticleAccountingPrice != null ?
                            senderArticleAccountingPriceSum.ForDisplay(ValueDisplayType.Money) : "---";
                            model.TotalSenderPriceSum += senderArticleAccountingPriceSum;
                        }

                        if (model.Settings.PrintRecepientPrice)
                        {
                            movementWaybillPrintingFormItem.RecepientPrice = row.RecipientArticleAccountingPrice != null ?
                            row.RecipientArticleAccountingPrice.AccountingPrice.ForDisplay(ValueDisplayType.Money) : "---";
                            movementWaybillPrintingFormItem.RecepientPriceSum = row.RecipientArticleAccountingPrice != null ?
                            recipientArticleAccountingPriceSum.ForDisplay(ValueDisplayType.Money) : "---";
                            model.TotalRecepientPriceSum += recipientArticleAccountingPriceSum;
                        }
                        if (model.Settings.PrintMarkup)
                        {
                            movementWaybillPrintingFormItem.Markup = (recipientArticleAccountingPriceSum - senderArticleAccountingPriceSum).ForDisplay(ValueDisplayType.Money);
                            model.TotalMarkup += recipientArticleAccountingPriceSum - senderArticleAccountingPriceSum;
                        }

                        model.TotalCount += row.MovingCount;
                        model.Rows.Add(movementWaybillPrintingFormItem);
                    }
                }
                else
                {
                    //Учет без партий. Получаем аггрегированную выборку
                    var query = movementWaybill.Rows
                        .GroupBy(x => x.Article)
                        .Select(x => new
                        {
                            SenderArticleAccountingPrice = x.First().SenderArticleAccountingPrice,
                            MovingCount = x.Sum(y => y.MovingCount),
                            RecipientArticleAccountingPrice = x.First().RecipientArticleAccountingPrice,
                            Article = x.Key,
                            CreationDate = x.Min(y => y.CreationDate),
                            Weight = x.Sum(y => y.Weight),
                            Volume = x.Sum(y => y.Volume),
                            PackCount = x.Sum(y => y.PackCount)
                        });

                    foreach (var row in query.OrderBy(x => x.CreationDate))
                    {
                        var senderArticleAccountingPriceSum = row.SenderArticleAccountingPrice != null ?
                            row.SenderArticleAccountingPrice.AccountingPrice * row.MovingCount : 0;
                        var recipientArticleAccountingPriceSum = row.RecipientArticleAccountingPrice != null ?
                            row.RecipientArticleAccountingPrice.AccountingPrice * row.MovingCount : 0;

                        var movementWaybillPrintingFormItem = new MovementWaybillPrintingFormItemViewModel()
                        {
                            Id = row.Article.Id.ForDisplay(),
                            Number = row.Article.Number,
                            ArticleName = row.Article.FullName,
                            Count = row.MovingCount.ForDisplay(),
                            PackSize = row.Article.PackSize.ForDisplay(),
                            PackCount = row.PackCount.ForDisplay(ValueDisplayType.PackCount),
                            MeasureUnit = row.Article.MeasureUnit.ShortName,
                            Weight = row.Weight.ForDisplay(ValueDisplayType.Weight),
                            Volume = row.Volume.ForDisplay(ValueDisplayType.Volume),
                        };

                        if (model.Settings.PrintSenderPrice)
                        {
                            movementWaybillPrintingFormItem.SenderPrice = row.SenderArticleAccountingPrice != null ?
                            row.SenderArticleAccountingPrice.AccountingPrice.ForDisplay(ValueDisplayType.Money) : "---";
                            movementWaybillPrintingFormItem.SenderPriceSum = row.SenderArticleAccountingPrice != null ?
                            senderArticleAccountingPriceSum.ForDisplay(ValueDisplayType.Money) : "---";
                            model.TotalSenderPriceSum += senderArticleAccountingPriceSum;
                        }

                        if (model.Settings.PrintRecepientPrice)
                        {
                            movementWaybillPrintingFormItem.RecepientPrice = row.RecipientArticleAccountingPrice != null ?
                            row.RecipientArticleAccountingPrice.AccountingPrice.ForDisplay(ValueDisplayType.Money) : "---";
                            movementWaybillPrintingFormItem.RecepientPriceSum = row.RecipientArticleAccountingPrice != null ?
                            recipientArticleAccountingPriceSum.ForDisplay(ValueDisplayType.Money) : "---";
                            model.TotalRecepientPriceSum += recipientArticleAccountingPriceSum;
                        }
                        if (model.Settings.PrintMarkup)
                        {
                            movementWaybillPrintingFormItem.Markup = (recipientArticleAccountingPriceSum - senderArticleAccountingPriceSum).ForDisplay(ValueDisplayType.Money);
                            model.TotalMarkup += recipientArticleAccountingPriceSum - senderArticleAccountingPriceSum;
                        }

                        model.TotalCount += row.MovingCount;
                        model.Rows.Add(movementWaybillPrintingFormItem);
                    }
                }

                return model;
            }
        }
        #endregion

        #region ТОРГ 12

        /// <summary>
        /// Получение модели параметров ТОРГ12
        /// </summary>
        public TORG12PrintingFormSettingsViewModel GetTORG12PrintingFormSettings(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = movementWaybillService.CheckWaybillExistence(waybillId, user);

                var allowToViewSenderAccountingPrices = movementWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(waybill, user);
                var allowToViewRecipientAccountingPrices = movementWaybillService.IsPossibilityToPrintFormInRecipientAccountingPrices(waybill, user);
                var allowToViewPurchaseCosts = movementWaybillService.IsPossibilityToPrintFormInPurchaseCosts(waybill, user);

                ValidationUtils.Assert(allowToViewSenderAccountingPrices || allowToViewRecipientAccountingPrices || allowToViewPurchaseCosts,
                    "Нет прав на просмотр ни учетных цен отправителя, ни учетных цен получателя, ни закупочных цен.");

                var priceTypes = new List<PrintingFormPriceType>();
                if (allowToViewSenderAccountingPrices) priceTypes.Add(PrintingFormPriceType.SenderAccountingPrice);
                if (allowToViewRecipientAccountingPrices) priceTypes.Add(PrintingFormPriceType.RecipientAccountingPrice);
                if (allowToViewPurchaseCosts) priceTypes.Add(PrintingFormPriceType.PurchaseCost);

                var model = new TORG12PrintingFormSettingsViewModel()
                {
                    PriceTypes = priceTypes.GetComboBoxItemList(s => s.GetDisplayName(), s => s.ValueToString(), false, false)
                };

                return model;
            }
        }

        /// <summary>
        /// Получение модели печатной формы ТОРГ 12
        /// </summary>
        /// <param name="settings">Параметры печатной формы</param>
        /// <returns>Модель печатной формы</returns>
        public TORG12PrintingFormViewModel GetTORG12PrintingForm(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {                
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = movementWaybillService.CheckWaybillExistence(settings.WaybillId, user);

                var priceType = ValidationUtils.TryGetEnum<PrintingFormPriceType>(settings.PriceTypeId);
                
                // проверка возможности просмотра определенного типа цен
                CheckPriceTypePermissions(user, waybill, priceType);

                var organizationJuridicalPerson = waybill.Sender.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                    waybill.Recipient.EconomicAgent.As<JuridicalPerson>() : null;

                var payerJuridicalPerson = waybill.Recipient.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                    waybill.Recipient.EconomicAgent.As<JuridicalPerson>() : null;

                var recepientJuridicalPerson = waybill.Recipient.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                    waybill.Recipient.EconomicAgent.As<JuridicalPerson>() : null;

                var model = new TORG12PrintingFormViewModel()
                {
                    PriceTypeId = settings.PriceTypeId,
                    WaybillId = waybill.Id.ToString(),
                    OrganizationName = waybill.Sender.GetFullInfo(),
                    Date = waybill.Date.ToShortDateString(),
                    OrganizationOKPO = organizationJuridicalPerson != null ? organizationJuridicalPerson.OKPO : "",
                    Payer = waybill.Recipient.GetFullInfo(),
                    PayerOKPO = payerJuridicalPerson != null ? payerJuridicalPerson.OKPO : "",
                    Reason = "",
                    ReasonDate = "",
                    ReasonNumber = "",
                    Recepient = waybill.Recipient.GetFullInfo(),
                    RecepientOKPO = recepientJuridicalPerson != null ? recepientJuridicalPerson.OKPO : "",
                    Sender = waybill.Sender.GetFullInfo(),
                    SenderOKPO = organizationJuridicalPerson != null ? organizationJuridicalPerson.OKPO : "",
                    ShippingWaybillDate = "",
                    ShippingWaybillNumber = "",
                    Number = waybill.Number,
                };
                
                return model;
            }
        }

        /// <summary>
        /// Получение информации о строках печатной формы
        /// </summary>
        public TORG12PrintingFormRowsViewModel GetTORG12PrintingFormRows(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = movementWaybillService.CheckWaybillExistence(settings.WaybillId, user);

                var priceType = ValidationUtils.TryGetEnum<PrintingFormPriceType>(settings.PriceTypeId);
                
                // проверка возможности просмотра определенного типа цен
                CheckPriceTypePermissions(user, waybill, priceType);

                var model = new TORG12PrintingFormRowsViewModel();

                //Для получения кодов ОКЕИ
                var measureUnitConv = new Dictionary<string, string>();
                measureUnitConv.Add("шт.", "796");
                
                dynamic query;

                // учитываем партии
                if (priceType == PrintingFormPriceType.PurchaseCost)
                {
                    query = waybill.Rows
                        .GroupBy(x => new { x.ReceiptWaybillRow, x.ValueAddedTax })
                        .Select(x => new
                        {
                            SenderArticleAccountingPrice = x.First().SenderArticleAccountingPrice,
                            RecipientArticleAccountingPrice = x.First().RecipientArticleAccountingPrice,
                            PurchaseCost = x.Key.ReceiptWaybillRow.PurchaseCost,
                            MovingCount = x.Sum(y => y.MovingCount),
                            Article = x.Key.ReceiptWaybillRow.Article,
                            CreationDate = x.Min(y => y.CreationDate),
                            SenderValueAddedTaxSum = x.Sum(y => y.SenderValueAddedTaxSum),
                            RecipientValueAddedTaxSum = x.Sum(y => y.RecipientValueAddedTaxSum),
                            PurchaseCostValueAddedTaxSum = x.Sum(y => y.PurchaseCostValueAddedTaxSum),
                            ValueAddedTax = x.Key.ValueAddedTax
                        })
                        .OrderBy(x => x.CreationDate);
                }
                // партии не учитываем
                else
                {
                    query = waybill.Rows
                        .GroupBy(x => new { x.Article, x.ValueAddedTax })
                        .Select(x => new
                        {
                            SenderArticleAccountingPrice = x.First().SenderArticleAccountingPrice,
                            RecipientArticleAccountingPrice = x.First().RecipientArticleAccountingPrice,
                            MovingCount = x.Sum(y => y.MovingCount),
                            Article = x.Key.Article,
                            CreationDate = x.Min(y => y.CreationDate),
                            SenderValueAddedTaxSum = x.Sum(y => y.SenderValueAddedTaxSum),
                            RecipientValueAddedTaxSum = x.Sum(y => y.RecipientValueAddedTaxSum),
                            ValueAddedTax = x.Key.ValueAddedTax,
                            Weight = x.Sum(y => y.Weight)
                        })
                        .OrderBy(x => x.CreationDate);
                }
                
                var rowNumber = 0;
                foreach (var row in query)
                {
                    decimal price = 0M, priceSum = 0M, valueAddedTaxSum = 0M;

                    switch (priceType)
                    {
                        case PrintingFormPriceType.SenderAccountingPrice:
                            price = row.SenderArticleAccountingPrice.AccountingPrice;
                            valueAddedTaxSum = row.SenderValueAddedTaxSum;
                            break;
                        case PrintingFormPriceType.RecipientAccountingPrice:
                            price = row.RecipientArticleAccountingPrice.AccountingPrice;
                            valueAddedTaxSum = row.RecipientValueAddedTaxSum;
                            break;
                        case PrintingFormPriceType.PurchaseCost:
                            price = row.PurchaseCost;
                            valueAddedTaxSum = row.PurchaseCostValueAddedTaxSum;
                            break;
                    }

                    priceSum = Math.Round(price * row.MovingCount, (priceType == PrintingFormPriceType.PurchaseCost ? 6 : 2));

                    var formItem = new TORG12PrintingFormItemViewModel(price, priceSum, row.MovingCount, valueAddedTaxSum, row.ValueAddedTax.Value)
                    {
                        RowNumber = (++rowNumber).ToString(),
                        Id = ((int)row.Article.Id).ForDisplay(),
                        ArticleName = row.Article.Number + " " + row.Article.FullName,
                        MeasureUnit = row.Article.MeasureUnit.ShortName,
                        MeasureUnitScale = row.Article.MeasureUnit.Scale,
                        MeasureUnitOKEI = measureUnitConv.Keys.Contains((string)row.Article.MeasureUnit.ShortName) ?
                            measureUnitConv[row.Article.MeasureUnit.ShortName] : row.Article.MeasureUnit.ShortName,
                        WeightBruttoValue = row.Weight,
                        WeightBrutto = ((decimal)(row.Weight)).ForDisplay(ValueDisplayType.WeightWithZeroFractionPart)
                    };
                    model.Rows.Add(formItem);
                }

                return model;
            }
        }

        /// <summary>
        /// Выгрузка формы в Excel
        /// </summary>
        public byte[] ExportTORG12PrintingFormToExcel(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            return ExportTORG12PrintingFormToExcel(GetTORG12PrintingForm(settings, currentUser), GetTORG12PrintingFormRows(settings, currentUser));
        }

        #endregion

        /// <summary>
        /// Проверка возможности просмотра определенного типа цен
        /// </summary>        
        private void CheckPriceTypePermissions(User user, MovementWaybill waybill, PrintingFormPriceType priceType)
        {
            switch (priceType)
            {
                case PrintingFormPriceType.SenderAccountingPrice:
                    movementWaybillService.CheckPossibilityToPrintFormInSenderAccountingPrices(waybill, user);
                    break;
                case PrintingFormPriceType.RecipientAccountingPrice:
                    movementWaybillService.CheckPossibilityToPrintFormInRecipientAccountingPrices(waybill, user);
                    break;
                case PrintingFormPriceType.PurchaseCost:
                    movementWaybillService.CheckPossibilityToPrintFormInPurchaseCosts(waybill, user);
                    break;
                default:
                    throw new Exception("Неверное значение типа цен, в которых печается отчет.");
            }
        }

        #region T1 (TTH)

        /// <summary>
        /// Форма настройки печати Т1 (ТТН)
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель настроек</returns>
        public T1PrintingFormSettingsViewModel GetT1PrintingFormSettings(Guid waybillId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = movementWaybillService.CheckWaybillExistence(waybillId, user);

                var allowToViewSenderAccountingPrices = movementWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(waybill, user);
                var allowToViewRecipientAccountingPrices = movementWaybillService.IsPossibilityToPrintFormInRecipientAccountingPrices(waybill, user);
                var allowToViewPurchaseCosts = movementWaybillService.IsPossibilityToPrintFormInPurchaseCosts(waybill, user);

                ValidationUtils.Assert(allowToViewSenderAccountingPrices || allowToViewRecipientAccountingPrices || allowToViewPurchaseCosts,
                    "Нет прав на просмотр ни учетных цен отправителя, ни учетных цен получателя, ни закупочных цен.");

                var priceTypes = new List<PrintingFormPriceType>();
                if (allowToViewSenderAccountingPrices) priceTypes.Add(PrintingFormPriceType.SenderAccountingPrice);
                if (allowToViewRecipientAccountingPrices) priceTypes.Add(PrintingFormPriceType.RecipientAccountingPrice);

                return new T1PrintingFormSettingsViewModel()
                {
                    ActionUrl = "",
                    IsPrintProductSection = true,
                    IsNeedSelectPriceType = true,
                    PriceTypes = priceTypes.GetComboBoxItemList(s => s.GetDisplayName(), s => s.ValueToString(), false, false),
                    WaybillId = waybillId
                };
            }
        }

        /// <summary>
        /// Формирование общей части формы Т1
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель</returns>
        public T1ProductSectionPrintingFormViewModel ShowT1ProductSectionPrintingForm(T1PrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = movementWaybillService.CheckWaybillExistence(settings.WaybillId, user);
                var currentDate = DateTimeUtils.GetCurrentDateTime();
                var priceType = ValidationUtils.TryGetEnum<PrintingFormPriceType>(settings.PriceTypeId);

                // проверка возможности просмотра определенного типа цен
                CheckPriceTypePermissions(user, waybill, priceType);

                var scale = waybill.Rows.Max(x => x.Article.MeasureUnit.Scale);
                var priceSum = priceType == PrintingFormPriceType.RecipientAccountingPrice ? waybill.RecipientAccountingPriceSum : waybill.SenderAccountingPriceSum;
                var valueAddedTaxSum = priceType == PrintingFormPriceType.RecipientAccountingPrice ?
                    waybill.Rows.Sum(x => x.RecipientValueAddedTaxSum) : waybill.Rows.Sum(x => x.SenderValueAddedTaxSum);
                
                return new T1ProductSectionPrintingFormViewModel()
                {
                    CreationDay = currentDate.Day.ForDisplay(),
                    CreationMonth = currentDate.Month.ForDisplay(),
                    CreationYear = (currentDate.Year % 100).ForDisplay(),
                    GrousWeightString = SpelledOutCurrency.Get(Math.Round(waybill.Weight / 1000, 3) * 1000,
                        true, "килограмм", "килограмма", "килограмм", "", "", "", false),
                    GrousWeightValue = (waybill.Weight / 1000).ForDisplay(ValueDisplayType.WeightWithZeroFractionPart), //Выводим в тоннах
                    Number = waybill.Number,
                    Payer = waybill.Recipient.GetFullInfo(),
                    ArticleCountString = SpelledOutCurrency.Get(waybill.Rows.GroupBy(x => x.Article).Count(), true, "", "", "", "", "", "", false),
                    Recipient = waybill.Recipient.GetFullInfo(),
                    RowCountString = SpelledOutCurrency.Get(waybill.RowCount, true, "", "", "", "", "", "", false),
                    Sender = waybill.Sender.GetFullInfo(),
                    TotalSumSeniorString = SpelledOutCurrency.Get(priceSum.Value, true, "", "", "", "", "", "", false),
                    TotalSumJuniorString = ((priceSum % 1) * 100).Value.ToString("00"),   // Получаем копейки
                    TotalSumValue = priceSum.ForDisplay(ValueDisplayType.MoneyWithZeroCopecks),
                    ValueAddedTaxSum = valueAddedTaxSum.ForDisplay(ValueDisplayType.MoneyWithZeroCopecks),
                    ValueAddedTaxPercentage = waybill.Rows.GroupBy(x => x.ValueAddedTax).Count() == 1 ?
                        waybill.Rows.First().ValueAddedTax.Value.ForDisplay(ValueDisplayType.Percent) : "",
                    WaybillId = waybill.Id.ToString(),
                    TotalCount = waybill.Rows.Sum(x => x.MovingCount).ForDisplay(scale),
                    CountScale = scale.ToString(),  // точность вывода количества (для JS)
                    TotalWeight = (waybill.Weight / 1000).ForDisplay(ValueDisplayType.WeightWithZeroFractionPart), //Выводим в тоннах
                    TotalSum = priceSum.ForDisplay(ValueDisplayType.MoneyWithZeroCopecks),
                    PriceTypeId = settings.PriceTypeId
                };
            }
        }

        /// <summary>
        /// Получение позиций для формы Т1
        /// </summary>
        /// <param name="settings">Настройки</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель позиций</returns>
        public T1ProductSectionPrintingFormRowsViewModel ShowT1ProductSectionPrintingFormRows(T1PrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = movementWaybillService.CheckWaybillExistence(settings.WaybillId, user);
                var currentDate = DateTimeUtils.GetCurrentDateTime();

                var priceType = ValidationUtils.TryGetEnum<PrintingFormPriceType>(settings.PriceTypeId);

                // проверка возможности просмотра определенного типа цен
                CheckPriceTypePermissions(user, waybill, priceType);

                var model = new T1ProductSectionPrintingFormRowsViewModel();
                var scale = waybill.Rows.Max(x => x.Article.MeasureUnit.Scale);

                foreach (var row in waybill.Rows)
                {
                    var price = priceType == PrintingFormPriceType.RecipientAccountingPrice ? 
                        row.RecipientArticleAccountingPrice.AccountingPrice : row.SenderArticleAccountingPrice.AccountingPrice;

                    model.Rows.Add(new T1ProductSectionPrintingFormItemViewModel()
                    {
                        Count = row.MovingCount.ForDisplay(scale),
                        ItemNumber = row.Article.Id.ForDisplay(),
                        MeasureUnit = row.Article.MeasureUnit.ShortName,
                        Name = row.Article.FullName,
                        Number = row.Article.Number,
                        ListPriseNumber = "-",
                        Price = price.ForDisplay(ValueDisplayType.MoneyWithZeroCopecks),
                        Sum = (price * row.MovingCount).ForDisplay(ValueDisplayType.MoneyWithZeroCopecks),
                        Weight = (row.Weight / 1000).ForDisplay(ValueDisplayType.WeightWithZeroFractionPart)   // переводим в тонны
                    });
                }

                return model;
            }
        }

        #endregion

        #endregion

        #region Выбор накладной

        public MovementWaybillSelectViewModel SelectWaybill(int articleId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new MovementWaybillSelectViewModel();

                model.Data = GetWaybillSelectGridLocal(new GridState { PageSize = 5, Sort = "Date=Desc;CreationDate=Desc", Parameters = "Article=" + articleId.ToString() }, user);

                model.FilterData = GetFilterData(user);

                return model;
            }
        }

        public GridData GetWaybillSelectGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetWaybillSelectGridLocal(state, user);
            }
        }

        private GridData GetWaybillSelectGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(58));
            model.AddColumn("IsAccepted", "Пр.", Unit.Pixel(22), align: GridColumnAlign.Center);
            model.AddColumn("Number", "Номер", Unit.Pixel(60), GridCellStyle.Link);
            model.AddColumn("Date", "Дата", Unit.Pixel(60));
            model.AddColumn("RecipientAccountingPriceSum", "Сумма в УЦ", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("SenderStorageName", "Отправитель", Unit.Percentage(50));
            model.AddColumn("RecipientStorageName", "Получатель", Unit.Percentage(50));
            model.AddColumn("ShippingPercent", "Отгрузка", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Name", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveFilter = new ParameterString(state.Filter);

            var deriveParameters = new ParameterString(state.Parameters);

            if (deriveParameters.Keys.Contains("Article"))
            {
                var articleId = deriveParameters["Article"].Value as string;
                deriveFilter.Add("Article", ParameterStringItem.OperationType.Eq, articleId);
            }

            var rows = movementWaybillService.GetFilteredList(state, user, deriveFilter);

            GridActionCell actions = new GridActionCell("Action");
            actions.AddAction("Выбрать", "movement_waybill_select_link");

            foreach (var row in rows)
            {
                var indicators = movementWaybillIndicatorService.GetMainIndicators(row, user);

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("IsAccepted") { Value = row.AcceptanceDate != null ? "П" : "" },
                    new GridLabelCell("Number") { Value = StringUtils.PadLeftZeroes(row.Number, 8) },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("RecipientAccountingPriceSum") { Value = indicators.RecipientAccountingPriceSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("SenderStorageName") { Value = row.SenderStorage.Name },
                    new GridLabelCell("RecipientStorageName") { Value = row.RecipientStorage.Name },
                    new GridLabelCell("ShippingPercent") { Value = movementWaybillIndicatorService.CalculateShippingPercent(row).ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() },
                    new GridHiddenCell("Name") { Value = row.Name, Key = "name" }
                ));
            }
            model.State = state;

            return model;
        }

        #endregion

        #endregion
    }
}