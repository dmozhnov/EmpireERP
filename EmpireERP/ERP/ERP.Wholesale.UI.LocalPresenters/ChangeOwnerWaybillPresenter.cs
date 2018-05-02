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
using ERP.Wholesale.UI.ViewModels;
using ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybill;
using ERP.Wholesale.UI.ViewModels.OutgoingWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.ChangeOwnerWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.Т1;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ChangeOwnerWaybillPresenter : OutgoingWaybillPresenter<ChangeOwnerWaybill>, IChangeOwnerWaybillPresenter
    {
        #region Поля

        private readonly IChangeOwnerWaybillService changeOwnerWaybillService;
        private readonly IChangeOwnerWaybillMainIndicatorService changeOwnerWaybillIndicatorService;
        private readonly IValueAddedTaxService valueAddedTaxService;
        private readonly IReceiptWaybillService receiptWaybillService;
        private readonly IOutgoingWaybillRowService outgoingWaybillRowService;

        #endregion

        #region Конструкторы

        public ChangeOwnerWaybillPresenter(IUnitOfWorkFactory unitOfWorkFactory, IChangeOwnerWaybillService changeOwnerWaybillService,
            IChangeOwnerWaybillMainIndicatorService changeOwnerWaybillIndicatorService, IStorageService storageService,
            IAccountOrganizationService accountOrganizationService, IValueAddedTaxService valueAddedTaxService, IReceiptWaybillService receiptWaybillService,
            IArticlePriceService articlePriceService, IOutgoingWaybillRowService outgoingWaybillRowService, IArticleAvailabilityService articleAvailabilityService,
            IArticleService articleService, IUserService userService)
            : base(unitOfWorkFactory, changeOwnerWaybillService, userService, storageService, accountOrganizationService,
            articleService, articlePriceService, articleAvailabilityService)
        {
            this.changeOwnerWaybillService = changeOwnerWaybillService;
            this.changeOwnerWaybillIndicatorService = changeOwnerWaybillIndicatorService;
            this.valueAddedTaxService = valueAddedTaxService;
            this.receiptWaybillService = receiptWaybillService;
            this.outgoingWaybillRowService = outgoingWaybillRowService;
        }

        #endregion

        #region Методы

        #region Список

        /// <summary>
        /// Отображение списка всех накладных смены собственника
        /// </summary>
        /// <returns></returns>
        public ChangeOwnerWaybillListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ChangeOwnerWaybill_List_Details);

                var model = new ChangeOwnerWaybillListViewModel();
                model.Title = "Накладные смены собственника";
                model.ChangeOwnerWaybillNewGrid = GetChangeOwnerWaybillNewGridLocal(new GridState() { Sort = "CreationDate=Desc;" }, user);
                model.ChangeOwnerWaybillAcceptedGrid = GetChangeOwnerWaybillAcceptedGridLocal(new GridState() { Sort = "CreationDate=Desc;" }, user);

                model.FilterData = GetFilterData(user);

                return model;
            }
        }

        private FilterData GetFilterData(User user)
        {
            var filterData = new FilterData();

            var storageList = storageService.GetList(user, Permission.ChangeOwnerWaybill_List_Details).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false);

            var accountOrganizationList = accountOrganizationService.GetList()
                .GetComboBoxItemList(a => a.ShortName, a => a.Id.ToString());

            filterData.Items.Add(new FilterDateRangePicker("Date", "Дата накладной"));
            filterData.Items.Add(new FilterTextEditor("Number", "№ накл. в системе"));
            filterData.Items.Add(new FilterComboBox("Storage", "Место хранения", storageList));
            filterData.Items.Add(new FilterComboBox("Sender", "Организация-отправитель", accountOrganizationList));
            filterData.Items.Add(new FilterComboBox("Recipient", "Организация-получатель", accountOrganizationList));

            return filterData;
        }

        /// <summary>
        /// Формирование грида новых накладных (если оба грида не различаются по столбцам, то оставить только один метод)
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public GridData GetChangeOwnerWaybillNewGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetChangeOwnerWaybillNewGridLocal(state, user);
            }
        }

        /// <summary>
        /// Формирование грида новых накладных (если оба грида не различаются по столбцам, то оставить только один метод)
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private GridData GetChangeOwnerWaybillNewGridLocal(GridState state, User user)
        {
            if (state == null)
            {
                state = new GridState();
            }

            var model = new GridData();
            model.AddColumn("IsAccepted", "Пр.", Unit.Pixel(22), align: GridColumnAlign.Center);
            model.AddColumn("Number", "Номер", Unit.Pixel(50), GridCellStyle.Link);
            model.AddColumn("Data", "Дата", Unit.Pixel(40));
            model.AddColumn("PurchaseCostSum", "Сумма в ЗЦ", Unit.Pixel(100), GridCellStyle.Label, GridColumnAlign.Right);
            model.AddColumn("AccountingPriceSum", "Сумма в УЦ", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("StorageName", "Место хранения", Unit.Percentage(20), GridCellStyle.Link);
            model.AddColumn("SenderName", "Отправитель", Unit.Percentage(40), GridCellStyle.Link);
            model.AddColumn("RecipientName", "Получатель", Unit.Percentage(40), GridCellStyle.Link);
            model.AddColumn("Shipping", "Отгрузка", Unit.Pixel(50), align: GridColumnAlign.Right);

            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("RecipientId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("StorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.ChangeOwnerWaybill_Create_Edit);

            ParameterString ps = new ParameterString("");
            ps.Add("State", ParameterStringItem.OperationType.OneOf);

            ps["State"].Value = new List<string>()
                {
                    ChangeOwnerWaybillState.Draft.ValueToString(),
                    ChangeOwnerWaybillState.ReadyToAccept.ValueToString(),
                    ChangeOwnerWaybillState.ArticlePending.ValueToString(),
                    ChangeOwnerWaybillState.ConflictsInArticle.ValueToString(),
                };

            var list = changeOwnerWaybillService.GetFilteredList(state, ps, user);

            var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

            foreach (var waybill in list)
            {
                GridRowStyle rowStyle;

                switch (waybill.State)
                {
                    case ChangeOwnerWaybillState.ConflictsInArticle:
                        rowStyle = GridRowStyle.Error;
                        break;

                    case ChangeOwnerWaybillState.ArticlePending:
                        rowStyle = GridRowStyle.Warning;
                        break;

                    default:
                        rowStyle = GridRowStyle.Normal;
                        break;
                }

                var acceptanceStr = "";
                if (waybill.IsReadyToAccept)
                {
                    acceptanceStr = "Г";
                }
                else if (waybill.AcceptanceDate != null)
                {
                    acceptanceStr = "П";
                }

                decimal? accountingPriceSum = null;
                if (user.HasPermissionToViewStorageAccountingPrices(waybill.Storage))
                {
                    accountingPriceSum = changeOwnerWaybillIndicatorService.GetMainIndicators(waybill, user).AccountingPriceSum;
                }

                model.AddRow(new GridRow(
                    new GridLabelCell("IsAccepted") { Value = acceptanceStr },
                    new GridLinkCell("Number") { Value = StringUtils.PadLeftZeroes(waybill.Number, 8) },
                    new GridLabelCell("Data") { Value = waybill.Date.ToShortDateString() },
                    new GridLabelCell("PurchaseCostSum") { Value = allowToViewPurchaseCost ? waybill.PurchaseCostSum.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("AccountingPriceSum") { Value = accountingPriceSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLinkCell("StorageName") { Value = waybill.Storage.Name },
                    new GridLinkCell("SenderName") { Value = waybill.Sender.ShortName },
                    new GridLinkCell("RecipientName") { Value = waybill.Recipient.ShortName },
                    new GridLabelCell("Shipping") { Value = changeOwnerWaybillIndicatorService.CalculateShippingPercent(waybill).ForDisplay(ValueDisplayType.Percent) + " %" },

                    new GridHiddenCell("Id") { Value = waybill.Id.ToString() },
                    new GridHiddenCell("SenderId") { Value = waybill.Sender.Id.ToString() },
                    new GridHiddenCell("RecipientId") { Value = waybill.Recipient.Id.ToString() },
                    new GridHiddenCell("StorageId") { Value = waybill.Storage.Id.ToString() }) { Style = rowStyle });

            }
            model.State = state;

            return model;
        }

        /// <summary>
        /// Формирование грида принятых накладных
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public GridData GetChangeOwnerWaybillAcceptedGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetChangeOwnerWaybillAcceptedGridLocal(state, user);
            }
        }

        /// <summary>
        /// Формирование грида принятых накладных
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private GridData GetChangeOwnerWaybillAcceptedGridLocal(GridState state, User user)
        {
            if (state == null)
            {
                state = new GridState();
            }

            var model = new GridData();
            model.AddColumn("Number", "Номер", Unit.Pixel(50), GridCellStyle.Link);
            model.AddColumn("Data", "Дата", Unit.Pixel(40));
            model.AddColumn("PurchaseCostSum", "Сумма в ЗЦ", Unit.Pixel(100), GridCellStyle.Label, GridColumnAlign.Right);
            model.AddColumn("AccountingPriceSum", "Сумма в УЦ", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("StorageName", "Место хранения", Unit.Percentage(20), GridCellStyle.Link);
            model.AddColumn("SenderName", "Отправитель", Unit.Percentage(40), GridCellStyle.Link);
            model.AddColumn("RecipientName", "Получатель", Unit.Percentage(40), GridCellStyle.Link);
            model.AddColumn("Shipping", "Отгрузка", Unit.Pixel(50), align: GridColumnAlign.Right);

            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("RecipientId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("StorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString ps = new ParameterString(state.Filter);
            ps.Add("State", ParameterStringItem.OperationType.Eq, ((byte)ChangeOwnerWaybillState.OwnerChanged).ToString());

            var list = changeOwnerWaybillService.GetFilteredList(state, ps, user);

            var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

            foreach (var waybill in list)
            {
                decimal? accountingPriceSum = null;
                if (user.HasPermissionToViewStorageAccountingPrices(waybill.Storage))
                {
                    accountingPriceSum = changeOwnerWaybillIndicatorService.GetMainIndicators(waybill, user).AccountingPriceSum;
                }

                model.AddRow(new GridRow(
                    new GridLinkCell("Number") { Value = StringUtils.PadLeftZeroes(waybill.Number, 8) },
                    new GridLabelCell("Data") { Value = waybill.Date.ToShortDateString() },
                    new GridLabelCell("PurchaseCostSum") { Value = allowToViewPurchaseCost ? waybill.PurchaseCostSum.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("AccountingPriceSum") { Value = accountingPriceSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLinkCell("StorageName") { Value = waybill.Storage.Name },
                    new GridLinkCell("SenderName") { Value = waybill.Sender.ShortName },
                    new GridLinkCell("RecipientName") { Value = waybill.Recipient.ShortName },
                    new GridLabelCell("Shipping") { Value = changeOwnerWaybillIndicatorService.CalculateShippingPercent(waybill).ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = waybill.Id.ToString() },
                    new GridHiddenCell("SenderId") { Value = waybill.Sender.Id.ToString() },
                    new GridHiddenCell("RecipientId") { Value = waybill.Recipient.Id.ToString() },
                    new GridHiddenCell("StorageId") { Value = waybill.Storage.Id.ToString() }));
            }
            model.State = state;

            return model;
        }

        #endregion

        #region Создание/Редактирование

        /// <summary>
        /// Создание новой накладной
        /// </summary>
        /// <param name="backURL">Обратный адрес</param>
        /// <returns></returns>
        public ChangeOwnerWaybillEditViewModel Create(string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ChangeOwnerWaybill_Create_Edit);

                var model = new ChangeOwnerWaybillEditViewModel();
                model.Number = "";
                model.AllowToGenerateNumber = true;
                model.IsAutoNumber = "1";
                model.Date = DateTime.Now.ToShortDateString();
                model.StorageList = storageService.GetList(user, Permission.ChangeOwnerWaybill_List_Details).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name).GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true, false);
                model.ValueAddedTaxId = 2; // TODO: может, сделать, как в Exp.WaybillPresenter, т.е. через GetList / проверку поля IsDefault??
                model.ValueAddedTaxList = valueAddedTaxService.GetList().GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true);
                model.AccountOrganizationList = accountOrganizationService.GetList().GetComboBoxItemList(x => x.ShortName, x => x.Id.ToString(), true, true);
                model.CuratorName = user.DisplayName;
                model.CuratorId = user.Id.ToString();

                model.Title = "Добавление накладной смены собственника";
                model.Name = "";
                model.BackURL = backURL;

                model.AllowToEdit = true;
                model.AllowToChangeCurator = user.HasPermission(Permission.ChangeOwnerWaybill_Curator_Change);

                return model;
            }
        }

        /// <summary>
        /// Сохранение накладной
        /// </summary>
        /// <param name="model"></param>
        public Guid Save(ChangeOwnerWaybillEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                if (model.SenderId == model.RecipientId)
                {
                    throw new Exception("Одна и та же организация не может быть отправителем и получателем товара.");
                }

                var sender = accountOrganizationService.CheckAccountOrganizationExistence(model.SenderId);
                if (!sender.Storages.Any(x => x.Id == model.StorageId))
                {
                    throw new Exception("Организация отправителя должна быть связана с местом хранения.");
                }

                var recipient = accountOrganizationService.CheckAccountOrganizationExistence(model.RecipientId);
                if (!recipient.Storages.Any(x => x.Id == model.StorageId))
                {
                    throw new Exception("Организация получателя должна быть связана с местом хранения.");
                }

                var valueAddedTax = valueAddedTaxService.CheckExistence(model.ValueAddedTaxId);

                ChangeOwnerWaybill waybill;
                int curatorId = ValidationUtils.TryGetInt(model.CuratorId); // Код куратора

                bool isAutoNumber = ValidationUtils.TryGetBool(model.IsAutoNumber);

                if (model.Id == Guid.Empty)
                {
                    user.CheckPermission(Permission.ChangeOwnerWaybill_Create_Edit);

                    var curator = userService.CheckUserExistence(curatorId, user, "Куратор не найден. Возможно, он был удален.");

                    if (isAutoNumber)
                    {
                        model.Number = "";
                    }

                    waybill = new ChangeOwnerWaybill(model.Number, DateTime.Parse(model.Date),
                        storageService.CheckStorageExistence(model.StorageId, user, Permission.ChangeOwnerWaybill_List_Details), sender,
                        recipient, valueAddedTax, curator, user, currentDateTime);

                    // если куратор не соответствует пользователю, то проверяем права на смену куратора
                    if (curator != user)
                    {
                        user.CheckPermission(Permission.ChangeOwnerWaybill_Curator_Change);
                        changeOwnerWaybillService.CheckPossibilityToViewDetailsByUser(waybill, curator);
                    }
                }
                else
                {
                    waybill = changeOwnerWaybillService.CheckWaybillExistence(model.Id, user);

                    changeOwnerWaybillService.CheckPossibilityToEdit(waybill, user);

                    waybill.Number = model.Number;
                    waybill.ValueAddedTax = valueAddedTax;
                    if (waybill.Curator.Id != curatorId)
                    {
                        var curator = userService.CheckUserExistence(curatorId, user, "Куратор не найден. Возможно, он был удален.");

                        changeOwnerWaybillService.CheckPossibilityToChangeCurator(waybill, user);
                        changeOwnerWaybillService.CheckPossibilityToViewDetailsByUser(waybill, curator);

                        waybill.Curator = curator;
                    }
                }
                waybill.Comment = StringUtils.ToHtml(model.Comment);

                var waybillId = changeOwnerWaybillService.Save(waybill);

                uow.Commit();

                return waybillId;
            }
        }

        /// <summary>
        /// Редактирование накладной
        /// </summary>
        /// <param name="id">Идентификатор накладной</param>
        /// <param name="backURL">Обратный адрес</param>
        /// <returns></returns>
        public ChangeOwnerWaybillEditViewModel Edit(Guid id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var waybill = changeOwnerWaybillService.CheckWaybillExistence(id, user);

                var model = new ChangeOwnerWaybillEditViewModel();
                model.Id = waybill.Id;
                model.Number = waybill.Number;
                model.IsAutoNumber = "0";
                model.AllowToGenerateNumber = false;
                model.Date = waybill.Date.ToShortDateString();
                model.SenderId = waybill.Sender.Id;
                model.RecipientId = waybill.Recipient.Id;
                model.StorageId = waybill.Storage.Id;
                model.ValueAddedTaxId = waybill.ValueAddedTax.Id;
                model.CuratorName = waybill.Curator.DisplayName;
                model.CuratorId = waybill.Curator.Id.ToString();
                model.Comment = waybill.Comment;

                model.StorageList = storageService.GetList(user, Permission.ChangeOwnerWaybill_List_Details).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                    .GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true, false);
                model.ValueAddedTaxList = valueAddedTaxService.GetList().GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true);
                model.AccountOrganizationList = accountOrganizationService.GetList().GetComboBoxItemList(x => x.ShortName, x => x.Id.ToString(), true, true);

                model.Title = "Редактирование накладной смены собственника";
                model.Name = waybill.Name;
                model.BackURL = backURL;

                model.AllowToEdit = user.HasPermission(Permission.ChangeOwnerWaybill_Create_Edit);
                model.AllowToChangeCurator = changeOwnerWaybillService.IsPossibilityToChangeCurator(waybill, user);

                return model;
            }
        }

        /// <summary>
        /// Получение списка связанных организаций
        /// </summary>
        /// <param name="stirageId"></param>
        /// <returns></returns>
        public object GetOrganizationList(short storageId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var storage = storageService.GetById(storageId);

                return ComboBoxBuilder.GetComboBoxItemList(storage.AccountOrganizations, x => x.ShortName, x => x.Id.ToString(), false);
            }
        }
        #endregion

        #region Детали

        public ChangeOwnerWaybillDetailsViewModel Details(Guid id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(id, user);

                changeOwnerWaybillService.CheckPossibilityToViewDetails(waybill, user);

                var model = new ChangeOwnerWaybillDetailsViewModel();
                model.Title = "Детали накладной смены собственника";
                model.Name = waybill.Name;
                model.BackURL = backURL;
                model.Id = waybill.Id;
                model.IsShipped = waybill.IsAccepted;
                model.ChangeOwnerWaybillRows = GetChangeOwnerWaybillRowGridLocal(new GridState() { Parameters = "WaybillId=" + waybill.Id.ToString() }, user);
                model.ChangeOwnerWaybillArticleGroupsGridState = new GridState() { Parameters = "WaybillId=" + waybill.Id.ToString() };

                model.DocGrid = GetChangeOwnerWaybillDocsGridLocal(new GridState() { Parameters = "WaybillId=" + waybill.Id.ToString() }, user);

                model.MainDetails = GetMainDetails(waybill, user);

                model.AllowToEdit = changeOwnerWaybillService.IsPossibilityToEdit(waybill, user);
                model.AllowToDelete = changeOwnerWaybillService.IsPossibilityToDelete(waybill, user);
                model.AllowToPrepareToAccept = changeOwnerWaybillService.IsPossibilityToPrepareToAccept(waybill, user);
                model.IsPossibilityToPrepareToAccept = changeOwnerWaybillService.IsPossibilityToPrepareToAccept(waybill, user, true);
                model.AllowToCancelReadinessToAccept = changeOwnerWaybillService.IsPossibilityToCancelReadinessToAccept(waybill, user);
                model.AllowToAccept = changeOwnerWaybillService.IsPossibilityToAccept(waybill, user);
                model.IsPossibilityToAccept = changeOwnerWaybillService.IsPossibilityToAccept(waybill, user, true);
                model.AllowToCancelAcceptance = changeOwnerWaybillService.IsPossibilityToCancelAcceptance(waybill, user);
                model.AllowToPrintForms = changeOwnerWaybillService.IsPossibilityToPrintForms(waybill, user) && user.HasPermissionToViewStorageAccountingPrices(waybill.Storage);

                return model;
            }
        }

        /// <summary>
        /// Получение главных деталей
        /// </summary>
        /// <param name="details"></param>
        private ChangeOwnerWaybillMainDetailsViewModel GetMainDetails(ChangeOwnerWaybill waybill, User user)
        {
            var model = new ChangeOwnerWaybillMainDetailsViewModel();

            model.StateName = waybill.State.GetDisplayName();

            model.SenderName = waybill.Sender.ShortName;
            model.SenderId = waybill.Sender.Id.ToString();

            model.RecipientName = waybill.Recipient.ShortName;
            model.RecipientId = waybill.Recipient.Id.ToString();

            model.StorageName = waybill.Storage.Name;
            model.StorageId = waybill.Storage.Id.ToString();

            model.AcceptanceDate = waybill.IsAccepted ? waybill.AcceptanceDate.Value.ToShortDateString() : "---";

            model.CuratorId = waybill.Curator.Id.ToString();
            model.CuratorName = waybill.Curator.DisplayName;

            model.Comment = waybill.Comment;

            model.TotalVolume = waybill.Volume.ForDisplay(ValueDisplayType.Volume);
            model.TotalWeight = waybill.Weight.ForDisplay(ValueDisplayType.Weight);

            model.CreatedById = waybill.CreatedBy.Id.ToString();
            model.CreatedByName = waybill.CreatedBy.DisplayName;
            model.CreationDate = String.Format("({0})", waybill.CreationDate.ToShortDateTimeString());

            model.AcceptedById = waybill.AcceptedBy != null ? waybill.AcceptedBy.Id.ToString() : "";
            model.AcceptedByName = waybill.AcceptedBy != null ? waybill.AcceptedBy.DisplayName : "";
            model.AcceptanceDate = waybill.AcceptedBy != null ? String.Format("({0})", waybill.AcceptanceDate.Value.ToShortDateTimeString()) : "";

            model.ChangedOwnerById = waybill.ChangedOwnerBy != null ? waybill.ChangedOwnerBy.Id.ToString() : "";
            model.ChangedOwnerByName = waybill.ChangedOwnerBy != null ? waybill.ChangedOwnerBy.DisplayName : "";
            model.ChangeOwnerDate = waybill.ChangedOwnerBy != null ? String.Format("({0})", waybill.ChangeOwnerDate.Value.ToShortDateTimeString()) : "";

            var ind = changeOwnerWaybillIndicatorService.GetMainIndicators(waybill, user, true);

            model.PurchaseCostSum = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere) ?
                waybill.PurchaseCostSum.ForDisplay(ValueDisplayType.Money) : "---";
            model.AccountingPriceSum = ind.AccountingPriceSum.ForDisplay(ValueDisplayType.Money);
            model.ShippingPercent = changeOwnerWaybillIndicatorService.CalculateShippingPercent(waybill).ForDisplay(ValueDisplayType.Percent);

            model.ValueAddedTaxString = VatUtils.GetValueAddedTaxString(ind.VatInfoList, waybill.ValueAddedTax.Value);

            model.RowCount = waybill.RowCount.ToString();

            model.AllowToChangeRecipient = changeOwnerWaybillService.IsPossibilityToChangeRecipient(waybill, user);
            model.AllowToViewCuratorDetails = userService.IsPossibilityToViewDetails(waybill.Curator, user);
            model.AllowToChangeCurator = changeOwnerWaybillService.IsPossibilityToChangeCurator(waybill, user);
            model.AllowToViewCreatedByDetails = userService.IsPossibilityToViewDetails(waybill.CreatedBy, user);
            model.AllowToViewAcceptedByDetails = userService.IsPossibilityToViewDetails(waybill.AcceptedBy, user);
            model.AllowToViewChangedOwnerByDetails = userService.IsPossibilityToViewDetails(waybill.ChangedOwnerBy, user);

            return model;
        }

        /// <summary>
        /// Получение грида строк накладной
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public GridData GetChangeOwnerWaybillRowGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetChangeOwnerWaybillRowGridLocal(state, user);
            }
        }

        /// <summary>
        /// Получение грида строк накладной
        /// </summary>
        private GridData GetChangeOwnerWaybillRowGridLocal(GridState state, User user)
        {
            if (state == null)
            {
                state = new GridState();
            }

            var ps = new ParameterString(state.Parameters);
            var waybill = changeOwnerWaybillService.CheckWaybillExistence(Guid.Parse((string)ps["WaybillId"].Value), user);

            var model = new GridData();

            model.ButtonPermissions["AllowToAddRow"] = changeOwnerWaybillService.IsPossibilityToCreateRow(waybill, user);

            model.AddColumn("Action", "Действие", Unit.Pixel(120), GridCellStyle.Action);
            model.AddColumn("Batch", "Партия", Unit.Pixel(100));
            model.AddColumn("Code", "Код", Unit.Pixel(35), GridCellStyle.Label, GridColumnAlign.Right);
            model.AddColumn("Number", "Артикул", Unit.Pixel(75));
            model.AddColumn("ArticleName", "Товар", Unit.Percentage(100));
            model.AddColumn("AccountingPrice", "Уч. цена поставки", Unit.Pixel(65), GridCellStyle.Label, GridColumnAlign.Right);
            model.AddColumn("ValueAddedTax", "Ставка НДС", Unit.Pixel(45), align: GridColumnAlign.Right);
            model.AddColumn("ValueAddedTaxSum", "Сумма НДС", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("Sum", "Сумма", Unit.Pixel(70), GridCellStyle.Label, GridColumnAlign.Right);
            model.AddColumn("PackCount", "Кол-во ЕУ", Unit.Pixel(35), GridCellStyle.Label, GridColumnAlign.Right);
            model.AddColumn("Count", "Кол-во", Unit.Pixel(40), GridCellStyle.Label, GridColumnAlign.Right);
            model.AddColumn("Shipping", "Отгрузка", Unit.Pixel(50), GridCellStyle.Label, GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            // получение стиля строк грида
            var rowStyles = GetRowsStyle(changeOwnerWaybillService.GetRowStates(waybill));

            var indicators = changeOwnerWaybillIndicatorService.GetMainRowsIndicators(waybill, user, true);

            foreach (var row in waybill.Rows.OrderBy(x => x.CreationDate))
            {
                var ind = indicators[row.Id];

                var actions = new GridActionCell("Action");
                actions.AddAction(changeOwnerWaybillService.IsPossibilityToEditRow(row, user) ? "Ред." : "Дет.", "edit_link");
                if (changeOwnerWaybillService.IsPossibilityToDeleteRow(row, user))
                {
                    actions.AddAction("Удал.", "delete_link");
                }

                if (row.AreSourcesDetermined)
                {
                    actions.AddAction("Источ.", "source_link");
                }

                decimal? accountingPrice = ind.AccountingPrice;
                decimal? sum = accountingPrice.HasValue ? accountingPrice.Value * row.MovingCount : (decimal?)null;

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Batch") { Value = row.ReceiptWaybillRow.BatchName },
                    new GridLabelCell("Code") { Value = row.Article.Id.ToString() },
                    new GridLabelCell("Number") { Value = row.Article.Number },
                    new GridLabelCell("ArticleName") { Value = row.Article.FullName },
                    new GridLabelCell("AccountingPrice") { Value = accountingPrice.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("ValueAddedTax") { Value = row.ValueAddedTax.Name },
                    new GridLabelCell("ValueAddedTaxSum") { Value = ind.ValueAddedTaxSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("Sum") { Value = sum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PackCount") { Value = row.PackCount.ForDisplay(ValueDisplayType.PackCount) },
                    new GridLabelCell("Count") { Value = row.MovingCount.ForDisplay() },
                    new GridLabelCell("Shipping") { Value = row.TotallyReservedCount.ForDisplay() },
                    new GridLabelCell("Id") { Value = row.Id.ToString() })
                    {
                        Style = rowStyles[row.Id]
                    });
            }
            model.State = state;

            return model;
        }

        /// <summary>
        /// Получить грид с группами товаров
        /// </summary>
        public GridData GetChangeOwnerWaybillArticleGroupGrid(GridState state, UserInfo currentUser)
        {

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetChangeOwnerWaybillArticleGroupGridLocal(state, user);
            }

        }

        private GridData GetChangeOwnerWaybillArticleGroupGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = changeOwnerWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid((deriveParams["WaybillId"].Value.ToString())), user);

            var articleGroups = waybill.Rows.GroupBy(x => x.Article.ArticleGroup);

            var rows = new List<BaseWaybillArticleGroupRow>();

            // получение основных индикаторов для списка позиций
            var indicators = changeOwnerWaybillIndicatorService.GetMainRowsIndicators(waybill, user, true);

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
                    var ind = indicators[waybillRow.Id];

                    var accountingPrice = ind.AccountingPrice;
                    var waybillRowSum = accountingPrice.HasValue ? Math.Round(accountingPrice.Value * waybillRow.MovingCount, 2)
                                                                      : (decimal?)null;

                    if (waybillRowSum.HasValue)
                    {
                        if (row.Sum.HasValue)
                            row.Sum += waybillRowSum;
                        else
                            row.Sum = waybillRowSum;
                    }

                    if (ind.ValueAddedTaxSum.HasValue)
                    {
                        if (row.ValueAddedTaxSum.HasValue)
                            row.ValueAddedTaxSum += ind.ValueAddedTaxSum;
                        else
                            row.ValueAddedTaxSum = ind.ValueAddedTaxSum;
                    }

                }

                rows.Add(row);
            }

            GridData model = GetArticleGroupGrid(rows);
            model.State = state;
            model.State.TotalRow = model.RowCount;


            return model;
        }

        /// <summary>
        /// Получение грида документов по накладной
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public GridData GetChangeOwnerWaybillDocsGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetChangeOwnerWaybillDocsGridLocal(state, user);
            }
        }

        /// <summary>
        /// Получение грида документов по накладной
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private GridData GetChangeOwnerWaybillDocsGridLocal(GridState state, User user)
        {
            if (state == null)
            {
                state = new GridState();
            }

            var ps = new ParameterString(state.Parameters);

            var waybill = changeOwnerWaybillService.CheckWaybillExistence(Guid.Parse((string)ps["WaybillId"].Value), user);

            var model = new GridData();
            model.AddColumn("DocName", "Наименование документа", Unit.Percentage(50));
            model.AddColumn("Creater", "Кто создал", Unit.Percentage(50));
            model.AddColumn("ChangeDate", "Последнее изменение", Unit.Pixel(100));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.State = state;

            return model;
        }

        /// <summary>
        /// Удаление накладной
        /// </summary>
        /// <param name="id"></param>
        public void DeleteChangeOwnerWaybill(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var waybill = changeOwnerWaybillService.CheckWaybillExistence(id, user);

                changeOwnerWaybillService.Delete(waybill, user);

                uow.Commit();
            }
        }

        #endregion

        #region Добавление/Редактирование/Удаление позиций накладной

        /// <summary>
        /// Переход на форму добавления позиции накладной
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ChangeOwnerWaybillRowEditViewModel AddRow(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var changeOwnerWaybill = changeOwnerWaybillService.CheckWaybillExistence(waybillId, user);

                changeOwnerWaybillService.CheckPossibilityToCreateRow(changeOwnerWaybill, user);

                var model = new ChangeOwnerWaybillRowEditViewModel()
                {
                    Title = "Добавление позиции в накладную",
                    ArticleName = "Выберите товар",
                    BatchName = "не выбрана",
                    MeasureUnitName = "",
                    MeasureUnitScale = "0",
                    ChangeOwnerWaybillDate = changeOwnerWaybill.Date.ToString(),
                    AvailableToReserveFromStorageCount = "---",
                    AvailableToReserveCount = "---",
                    AvailableToReserveFromPendingCount = "---",
                    PurchaseCost = "---",
                    AccountingPriceString = "---",
                    StorageId = changeOwnerWaybill.Storage.Id.ToString(),
                    SenderId = changeOwnerWaybill.Sender.Id.ToString(),
                    ChangeOwnerWaybillId = changeOwnerWaybill.Id,

                    TotallyReserved = "0",
                    ValueAddedTaxList = valueAddedTaxService.GetList().GetParamComboBoxItemList(x => x.Name, x => x.Id.ToString(), x => x.Value.ForEdit(), emptyParamValue: "0"),
                    ValueAddedTaxId = changeOwnerWaybill.ValueAddedTax.Id,
                    ValueAddedTaxSum = 0M.ForDisplay(ValueDisplayType.Money),

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
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(waybillId, user);

                changeOwnerWaybillService.CheckPossibilityToEdit(waybill, user);

                var model = new OutgoingWaybillAddRowsByListViewModel()
                {
                    Id = waybill.Id.ToString(),
                    Name = waybill.Name,
                    BackURL = backURL,
                    StorageId = waybill.Storage.Id.ToString(),
                    AccountOrganizationId = waybill.Sender.Id.ToString()
                };

                model.Filter.Items.Add(new FilterTextEditor("Number", "Артикул"));
                model.Filter.Items.Add(new FilterHyperlink("ArticleGroup", "Группа товара", "Выберите группу"));
                model.Filter.Items.Add(new FilterTextEditor("FullName", "Наименование"));
                model.Filter.Items.Add(new FilterTextEditor("Id", "Код товара"));
                model.Filter.Items.Add(new FilterYesNoToggle("OnlyAvailable", "Только наличие", true));

                model.ArticleGrid = GetArticlesForWaybillRowsAdditionByListGridLocal(new GridState()
                {
                    Parameters = "StorageId=" + waybill.Storage.Id +
                        ";WaybillId=" + waybill.Id +
                        ";AccountOrganizationId=" + waybill.Sender.Id +
                        ";ArticleTakingsInfo=" + GetArticlesForWaybillRowsAdditionByListGridData(waybill) +
                        ";ActionName=/ChangeOwnerWaybill/ShowArticlesForWaybillRowsAdditionByListGrid/",
                    Sort = "Number=Asc"
                }, user);

                model.RowGrid = GetChangeOwnerWaybillRowGridLocal(new GridState() { Parameters = "WaybillId=" + waybill.Id, Sort = "CreationDate=Asc" }, user);

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
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid(deriveParams["WaybillId"].Value.ToString()), user);

                //убран вызов временно, чтобы фильтр нормально фильтровал в случае, когда открываем в одном окне добавление товаров списком, а в другом окне проводим накладную
                //changeOwnerWaybillService.CheckPossibilityToEdit(waybill, user);

                deriveParams["ArticleTakingsInfo"].Value = GetArticlesForWaybillRowsAdditionByListGridData(waybill);

                state.Parameters = deriveParams.ToString();

                return GetArticlesForWaybillRowsAdditionByListGridLocal(state, user);
            }
        }

        private string GetArticlesForWaybillRowsAdditionByListGridData(ChangeOwnerWaybill waybill)
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
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(waybillId, user);
                var article = articleService.CheckArticleExistence(articleId);

                changeOwnerWaybillService.AddRowSimply(waybill, article, count, user);

                uow.Commit();
            }
        }

        #endregion

        /// <summary>
        /// Сохранение строки накладной
        /// </summary>
        /// <param name="model"></param>
        public object SaveRow(ChangeOwnerWaybillRowEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                if (model.ChangeOwnerWaybillId == Guid.Empty)
                {
                    throw new Exception("Накладная смены собственника не найдена. Возможно, она была удалена.");
                }

                if (model.ReceiptWaybillRowId == Guid.Empty)
                {
                    throw new Exception("Партия товара не найдена. Возможно, она была удалена.");
                }

                var changeOwnerWaybill = changeOwnerWaybillService.CheckWaybillExistence(model.ChangeOwnerWaybillId, user);
                var receiptWaybillRow = receiptWaybillService.GetRowById(model.ReceiptWaybillRowId);
                var valueAddedTax = valueAddedTaxService.CheckExistence(model.ValueAddedTaxId);

                ChangeOwnerWaybillRow row = null;
                var movingCount = ValidationUtils.TryGetDecimal(model.MovingCount);

                // добавление
                if (model.ChangeOwnerWaybillRowId == Guid.Empty)
                {
                    changeOwnerWaybillService.CheckPossibilityToCreateRow(changeOwnerWaybill, user);
                    row = new ChangeOwnerWaybillRow(receiptWaybillRow, movingCount, valueAddedTax);

                    if (!String.IsNullOrEmpty(model.ManualSourcesInfo))
                    {
                        var sourcesInfo = DeserializeWaybillRowManualSourceInfo(model.ManualSourcesInfo);
                        changeOwnerWaybillService.AddRow(changeOwnerWaybill, row, sourcesInfo, user);
                    }
                    else
                    {
                        changeOwnerWaybillService.AddRow(changeOwnerWaybill, row, user);
                    }
                }
                // редактирование
                else
                {
                    row = changeOwnerWaybill.Rows.FirstOrDefault(x => x.Id == model.ChangeOwnerWaybillRowId);

                    ValidationUtils.NotNull(row, "Позиция накладной не найдена. Возможно, она была удалена.");
                    changeOwnerWaybillService.CheckPossibilityToEditRow(row, user);

                    row.ReceiptWaybillRow = receiptWaybillRow; // Партию важно присваивать перед количеством, чтобы при смене товара проверялась точность количества уже по новому товару

                    row.MovingCount = movingCount;
                    row.ValueAddedTax = valueAddedTax;

                    if (!String.IsNullOrEmpty(model.ManualSourcesInfo))
                    {
                        var sourcesInfo = DeserializeWaybillRowManualSourceInfo(model.ManualSourcesInfo);
                        changeOwnerWaybillService.SaveRow(changeOwnerWaybill, row, sourcesInfo, user);
                    }
                    else
                    {
                        changeOwnerWaybillService.SaveRow(changeOwnerWaybill, row, user);
                    }
                }

                uow.Commit();

                return GetMainChangeableIndicators(changeOwnerWaybill, user);
            }
        }

        /// <summary>
        /// Получение значений основных пересчитываемых показателей
        /// </summary>
        private object GetMainChangeableIndicators(ChangeOwnerWaybill changeOwnerWaybill, User user)
        {
            return new
            {
                MainDetails = GetMainDetails(changeOwnerWaybill, user),
                AllowToEdit = changeOwnerWaybillService.IsPossibilityToEdit(changeOwnerWaybill, user),
                AllowToPrepareToAccept = changeOwnerWaybillService.IsPossibilityToPrepareToAccept(changeOwnerWaybill, user),
                IsPossibilityToPrepareToAccept = changeOwnerWaybillService.IsPossibilityToPrepareToAccept(changeOwnerWaybill, user, true),
                AllowToCancelReadinessToAccept = changeOwnerWaybillService.IsPossibilityToCancelReadinessToAccept(changeOwnerWaybill, user),
                AllowToAccept = changeOwnerWaybillService.IsPossibilityToAccept(changeOwnerWaybill, user),
                IsPossibilityToAccept = changeOwnerWaybillService.IsPossibilityToAccept(changeOwnerWaybill, user, true),
                AllowToCancelAcceptance = changeOwnerWaybillService.IsPossibilityToCancelAcceptance(changeOwnerWaybill, user),
                AllowToDelete = changeOwnerWaybillService.IsPossibilityToDelete(changeOwnerWaybill, user),
                AllowToPrintForms = changeOwnerWaybillService.IsPossibilityToPrintForms(changeOwnerWaybill, user)
            };
        }

        /// <summary>
        /// Редактирование строки накладной
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        [HttpGet]
        public ChangeOwnerWaybillRowEditViewModel EditRow(Guid waybillId, Guid waybillRowId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var changeOwnerWaybill = changeOwnerWaybillService.CheckWaybillExistence(waybillId, user);
                var changeOwnerWaybillRow = changeOwnerWaybillService.CheckWaybillRowExistence(waybillRowId);

                var indicators = changeOwnerWaybillIndicatorService.GetMainRowIndicators(changeOwnerWaybillRow, user, true);

                // получение расширенного наличия по партии
                var articleBatchAvailability = articleAvailabilityService.GetExtendedArticleBatchAvailability(changeOwnerWaybillRow.ReceiptWaybillRow,
                    changeOwnerWaybill.Storage, changeOwnerWaybill.Sender, DateTime.Now);

                var allowToEdit = changeOwnerWaybillService.IsPossibilityToEditRow(changeOwnerWaybillRow, user);

                var manualSourcesInfoString = "";
                if (changeOwnerWaybillRow.IsUsingManualSource)
                {
                    manualSourcesInfoString = SerializeWaybillRowManualSourceInfo(outgoingWaybillRowService.GetManualSources(changeOwnerWaybillRow.Id));
                }

                var model = new ChangeOwnerWaybillRowEditViewModel()
                {
                    Title = allowToEdit ? "Редактирование позиции накладной" : "Детали позиции накладной",
                    ArticleName = changeOwnerWaybillRow.Article.FullName,
                    ArticleId = changeOwnerWaybillRow.Article.Id,
                    BatchName = changeOwnerWaybillRow.ReceiptWaybillRow.BatchName,
                    MeasureUnitName = changeOwnerWaybillRow.Article.MeasureUnit.ShortName,
                    MeasureUnitScale = changeOwnerWaybillRow.ReceiptWaybillRow.ArticleMeasureUnitScale.ToString(),
                    CurrentReceiptWaybillRowId = changeOwnerWaybillRow.ReceiptWaybillRow.Id,
                    ChangeOwnerWaybillDate = changeOwnerWaybill.Date.ToString(),
                    MovingCount = changeOwnerWaybillRow.MovingCount.ForEdit(),

                    AvailableToReserveFromStorageCount = articleBatchAvailability.AvailableToReserveFromStorageCount.ForDisplay(),
                    AvailableToReserveFromPendingCount = articleBatchAvailability.AvailableToReserveFromPendingCount.ForDisplay(),
                    AvailableToReserveCount = articleBatchAvailability.AvailableToReserveCount.ForEdit(),
                    PurchaseCost = (user.HasPermission(Permission.PurchaseCost_View_ForEverywhere) ?
                        changeOwnerWaybillRow.ReceiptWaybillRow.PurchaseCost : (decimal?)null).ForDisplay(ValueDisplayType.Money),
                    AccountingPriceString = indicators.AccountingPrice.ForDisplay(ValueDisplayType.Money),
                    AccountingPriceValue = indicators.AccountingPrice.ForEdit(),
                    StorageId = changeOwnerWaybill.Storage.Id.ToString(),
                    SenderId = changeOwnerWaybill.Sender.Id.ToString(),
                    ChangeOwnerWaybillId = changeOwnerWaybill.Id,
                    ChangeOwnerWaybillRowId = changeOwnerWaybillRow.Id,
                    ReceiptWaybillRowId = changeOwnerWaybillRow.ReceiptWaybillRow.Id,
                    ManualSourcesInfo = manualSourcesInfoString,

                    TotallyReserved = changeOwnerWaybillRow.TotallyReservedCount.ForEdit(),
                    ValueAddedTaxList = valueAddedTaxService.GetList().GetParamComboBoxItemList(x => x.Name, x => x.Id.ToString(), x => x.Value.ForEdit(), emptyParamValue: "0"),
                    ValueAddedTaxId = changeOwnerWaybillRow.ValueAddedTax.Id,
                    ValueAddedTaxSum = indicators.ValueAddedTaxSum.ForDisplay(ValueDisplayType.Money),

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

                var waybill = changeOwnerWaybillService.CheckWaybillExistence(waybillId, user);

                var batch = receiptWaybillService.GetRowById(batchId);
                var article = batch.Article;
                var storage = waybill.Storage;

                var articleBatchAvailability = articleAvailabilityService.GetExtendedArticleBatchAvailability(batch, storage, waybill.Sender, DateTime.Now);

                var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);
                var allowToViewAccPrice = user.HasPermissionToViewStorageAccountingPrices(waybill.Storage);

                decimal? accountingPrice = null;
                if (allowToViewAccPrice)
                {
                    accountingPrice = articlePriceService.GetAccountingPrice(new List<short>() { storage.Id }, article.Id)[storage.Id];

                    ValidationUtils.NotNull(accountingPrice, String.Format("Не установлена учетная цена для данного места хранения на товар «{0}».", article.FullName));
                }

                var model = new ChangeOwnerWaybillRowEditViewModel()
                {
                    AvailableToReserveFromStorageCount = articleBatchAvailability.AvailableToReserveFromStorageCount.ForDisplay(),
                    AvailableToReserveFromPendingCount = articleBatchAvailability.AvailableToReserveFromPendingCount.ForDisplay(),
                    AvailableToReserveCount = articleBatchAvailability.AvailableToReserveCount.ForDisplay(),
                    PurchaseCost = (allowToViewPurchaseCost ?
                        batch.PurchaseCost : (decimal?)null).ForDisplay(ValueDisplayType.Money),

                    AccountingPriceString = accountingPrice.ForDisplay(ValueDisplayType.Money),
                    AccountingPriceValue = accountingPrice.ForEdit()
                };

                return model;
            }
        }

        /// <summary>
        /// Удаление строки накладной
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rowId"></param>
        public object DeleteRow(Guid waybillId, Guid waybillRowId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var changeOwnerWaybill = changeOwnerWaybillService.CheckWaybillExistence(waybillId, user);
                var changeOwnerWaybillRow = changeOwnerWaybillService.CheckWaybillRowExistence(waybillRowId);

                changeOwnerWaybillService.DeleteRow(changeOwnerWaybillRow, user);

                uow.Commit();

                return GetMainChangeableIndicators(changeOwnerWaybill, user);
            }
        }

        #endregion

        #region Подготовить / Отменить готовность к проводке накладной

        public object PrepareToAccept(Guid id, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(id, user);

                changeOwnerWaybillService.PrepareToAccept(waybill, user);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        public object CancelReadinessToAccept(Guid id, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(id, user);

                changeOwnerWaybillService.CancelReadinessToAccept(waybill, user);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        #endregion

        #region Смена получателя

        public ChangeOwnerWaybillChangeRecipientViewModel ChangeRecipient(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(id, user);

                changeOwnerWaybillService.CheckPossibilityToChangeRecipient(waybill, user);

                var model = new ChangeOwnerWaybillChangeRecipientViewModel();
                model.Title = "Смена получателя";
                model.WaybillId = waybill.Id.ToString();
                model.RecipientId = waybill.Recipient.Id.ToString();
                model.OrganizationList = ComboBoxBuilder.GetComboBoxItemList(waybill.Storage.AccountOrganizations.Where(x => x.Id != waybill.Sender.Id && x.Id != waybill.Recipient.Id),
                    x => x.ShortName, x => x.Id.ToString());

                return model;
            }
        }

        /// <summary>
        /// Сохранение нового получателя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object SaveNewRecipient(ChangeOwnerWaybillChangeRecipientViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid(model.WaybillId), user);
                var recipient = accountOrganizationService.CheckAccountOrganizationExistence(ValidationUtils.TryGetInt(model.RecipientId));

                changeOwnerWaybillService.ChangeRecipient(waybill, recipient, user);

                uow.Commit();

                return new { Name = recipient.ShortName, Id = recipient.Id.ToString() };
            }
        }

        #endregion

        #region Проводка/Отмена проводки накладной

        public object Accept(Guid Id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(Id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                changeOwnerWaybillService.Accept(waybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        public object CancelAcceptance(Guid Id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(Id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                changeOwnerWaybillService.CancelAcceptance(waybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        #endregion

        #region Печатные формы

        #region Счет-фактура

        /// <summary>
        /// Получение модели параметров счет-фактуры
        /// </summary>        
        public InvoicePrintingFormSettingsViewModel GetInvoicePrintingFormSettings(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(waybillId, user);

                var allowToViewSenderAccountingPrices = changeOwnerWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(waybill, user);
                var allowToViewPurchaseCosts = changeOwnerWaybillService.IsPossibilityToPrintFormInPurchaseCosts(waybill, user);

                ValidationUtils.Assert(allowToViewSenderAccountingPrices || allowToViewPurchaseCosts,
                    "Нет прав на просмотр ни учетных цен, ни закупочных цен.");

                var priceTypes = new List<PrintingFormPriceType>();
                if (allowToViewSenderAccountingPrices) priceTypes.Add(PrintingFormPriceType.SenderAccountingPrice);
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
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(settings.WaybillId, user);

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
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(settings.WaybillId, user);

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
                            price = row.ArticleAccountingPrice.AccountingPrice;
                            valueAddedTaxSum = row.ValueAddedTaxSum;
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

        #region Товарный чек

        public CashMemoPrintingFormViewModel GetCashMemoPrintingForm(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(id, user);

                changeOwnerWaybillService.CheckPossibilityToPrintFormInSenderAccountingPrices(waybill, user);

                var model = new CashMemoPrintingFormViewModel();
                model.WaybillId = id;
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

                    //так как чек можно печатать только по проведенной накладной, то AccountingPrice должен никогда не быть null. 
                    //Но данная проверка пока не реализована, поэтому пока для непроведенных накладных будет подставляться цена 0
                    var price = (row.ArticleAccountingPrice != null) ? row.ArticleAccountingPrice.AccountingPrice : 0;

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

        #region ТОРГ 12

        /// <summary>
        /// Получение модели параметров ТОРГ12
        /// </summary>
        public TORG12PrintingFormSettingsViewModel GetTORG12PrintingFormSettings(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(waybillId, user);

                var allowToViewSenderAccountingPrices = changeOwnerWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(waybill, user);
                var allowToViewPurchaseCosts = changeOwnerWaybillService.IsPossibilityToPrintFormInPurchaseCosts(waybill, user);

                ValidationUtils.Assert(allowToViewSenderAccountingPrices || allowToViewPurchaseCosts,
                    "Нет прав на просмотр ни учетных цен, ни закупочных цен.");

                var priceTypes = new List<PrintingFormPriceType>();
                if (allowToViewSenderAccountingPrices) priceTypes.Add(PrintingFormPriceType.SenderAccountingPrice);
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
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(settings.WaybillId, user);

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
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(settings.WaybillId, user);

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
                            ArticleAccountingPrice = x.First().ArticleAccountingPrice,
                            PurchaseCost = x.Key.ReceiptWaybillRow.PurchaseCost,
                            MovingCount = x.Sum(y => y.MovingCount),
                            Article = x.Key.ReceiptWaybillRow.Article,
                            CreationDate = x.Min(y => y.CreationDate),
                            ValueAddedTaxSum = x.Sum(y => y.ValueAddedTaxSum),
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
                            ArticleAccountingPrice = x.First().ArticleAccountingPrice,
                            MovingCount = x.Sum(y => y.MovingCount),
                            Article = x.Key.Article,
                            CreationDate = x.Min(y => y.CreationDate),
                            ValueAddedTaxSum = x.Sum(y => y.ValueAddedTaxSum),
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
                            price = row.ArticleAccountingPrice.AccountingPrice;
                            valueAddedTaxSum = row.ValueAddedTaxSum;
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

        #region Накладная смены собственника

        public ChangeOwnerWaybillPrintingFormSettingsViewModel GetPrintingFormSettings(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var waybill = changeOwnerWaybillService.CheckWaybillExistence(waybillId, user);

                changeOwnerWaybillService.CheckPossibilityToPrintFormInSenderAccountingPrices(waybill, user);

                var model = new ChangeOwnerWaybillPrintingFormSettingsViewModel()
                {
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
        public ChangeOwnerWaybillPrintingFormViewModel GetPrintingForm(ChangeOwnerWaybillPrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var changeOwnerWaybill = changeOwnerWaybillService.CheckWaybillExistence(settings.WaybillId, user);

                changeOwnerWaybillService.CheckPossibilityToPrintFormInSenderAccountingPrices(changeOwnerWaybill, user);

                if (!user.HasPermission(Permission.PurchaseCost_View_ForEverywhere))
                {
                    settings.PrintPurchaseCost = false;
                }

                var model = new ChangeOwnerWaybillPrintingFormViewModel()
                {
                    Settings = settings,
                    OrganizationName = changeOwnerWaybill.Recipient.FullName,
                    INN = changeOwnerWaybill.Recipient.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                        changeOwnerWaybill.Recipient.EconomicAgent.As<JuridicalPerson>().INN :
                        changeOwnerWaybill.Recipient.EconomicAgent.As<PhysicalPerson>().INN,
                    KPP = changeOwnerWaybill.Recipient.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                        changeOwnerWaybill.Recipient.EconomicAgent.As<JuridicalPerson>().KPP : "",
                    Address = changeOwnerWaybill.Recipient.Address,
                    Storage = changeOwnerWaybill.Storage.Name,
                    Title = String.Format("Накладная смены собственника {0}", changeOwnerWaybill.Name),
                    Date = DateTime.Now.ToShortDateString(),
                    RecepientStorageOrganization = changeOwnerWaybill.Sender.FullName
                };

                //Если учет по партиям
                if (model.Settings.DevideByBatch)
                {
                    bool possibilityToViewProducer = user.HasPermission(Permission.Producer_List_Details);
                    bool possibilityToViewProvider = user.HasPermission(Permission.Provider_List_Details);

                    var query = changeOwnerWaybill.Rows.OrderBy(x => x.CreationDate);
                    foreach (var row in query)
                    {
                        var articleAccountingPriceSum = row.ArticleAccountingPrice != null ?
                            row.ArticleAccountingPrice.AccountingPrice * row.MovingCount : 0;
                        bool possibilityToViewContractor = row.ReceiptWaybillRow.ReceiptWaybill.IsCreatedFromProductionOrderBatch ?
                            possibilityToViewProducer : possibilityToViewProvider;

                        var changeOwnerWaybillPrintingFormItem = new ChangeOwnerWaybillPrintingFormItemViewModel()
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
                            changeOwnerWaybillPrintingFormItem.PurchaseCost = row.ReceiptWaybillRow.PurchaseCost.ForDisplay(ValueDisplayType.Money);
                            changeOwnerWaybillPrintingFormItem.PurchaseSum = (row.MovingCount * row.ReceiptWaybillRow.PurchaseCost).ForDisplay(ValueDisplayType.Money);

                            model.TotalPurchaseSum += row.MovingCount * row.ReceiptWaybillRow.PurchaseCost;
                        }

                        changeOwnerWaybillPrintingFormItem.Price = row.ArticleAccountingPrice != null ?
                        row.ArticleAccountingPrice.AccountingPrice.ForDisplay(ValueDisplayType.Money) : "---";
                        changeOwnerWaybillPrintingFormItem.PriceSum = row.ArticleAccountingPrice != null ?
                        articleAccountingPriceSum.ForDisplay(ValueDisplayType.Money) : "---";
                        model.TotalPriceSum += articleAccountingPriceSum;

                        model.TotalCount += row.MovingCount;
                        model.Rows.Add(changeOwnerWaybillPrintingFormItem);
                    }
                }
                else
                {
                    //Учет без партий. Получаем аггрегированную выборку
                    var query = changeOwnerWaybill.Rows
                        .GroupBy(x => x.Article)
                        .Select(x => new
                        {
                            ArticleAccountingPrice = x.First().ArticleAccountingPrice,
                            MovingCount = x.Sum(y => y.MovingCount),
                            Article = x.Key,
                            CreationDate = x.Min(y => y.CreationDate),
                            Weight = x.Sum(y => y.Weight),
                            Volume = x.Sum(y => y.Volume),
                            PackCount = x.Sum(y => y.PackCount)
                        });

                    foreach (var row in query.OrderBy(x => x.CreationDate))
                    {
                        var articleAccountingPriceSum = row.ArticleAccountingPrice != null ?
                            row.ArticleAccountingPrice.AccountingPrice * row.MovingCount : 0;

                        var changeOwnerWaybillPrintingFormItem = new ChangeOwnerWaybillPrintingFormItemViewModel()
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

                        changeOwnerWaybillPrintingFormItem.Price = row.ArticleAccountingPrice != null ?
                        row.ArticleAccountingPrice.AccountingPrice.ForDisplay(ValueDisplayType.Money) : "---";
                        changeOwnerWaybillPrintingFormItem.PriceSum = row.ArticleAccountingPrice != null ?
                        articleAccountingPriceSum.ForDisplay(ValueDisplayType.Money) : "---";
                        model.TotalPriceSum += articleAccountingPriceSum;

                        model.TotalCount += row.MovingCount;
                        model.Rows.Add(changeOwnerWaybillPrintingFormItem);
                    }
                }

                return model;
            }
        }

        #endregion

        /// <summary>
        /// Проверка возможности просмотра определенного типа цен
        /// </summary>        
        private void CheckPriceTypePermissions(User user, ChangeOwnerWaybill waybill, PrintingFormPriceType priceType)
        {
            switch (priceType)
            {
                case PrintingFormPriceType.SenderAccountingPrice:
                    changeOwnerWaybillService.CheckPossibilityToPrintFormInSenderAccountingPrices(waybill, user);
                    break;
                case PrintingFormPriceType.PurchaseCost:
                    changeOwnerWaybillService.CheckPossibilityToPrintFormInPurchaseCosts(waybill, user);
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
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(waybillId, user);

                var allowToViewSenderAccountingPrices = changeOwnerWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(waybill, user);
                ValidationUtils.Assert(allowToViewSenderAccountingPrices, "Нет прав на просмотр учетных цен.");

                return new T1PrintingFormSettingsViewModel()
                {
                    ActionUrl = "",
                    IsPrintProductSection = true,
                    IsNeedSelectPriceType = false,
                    WaybillId = waybillId
                };
            }
        }

        /// <summary>
        /// Формирование общей части формы Т1
        /// </summary>
        /// <param name="settings">Настройки печати</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель</returns>
        public T1ProductSectionPrintingFormViewModel ShowT1ProductSectionPrintingForm(T1PrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(settings.WaybillId, user);
                var currentDate = DateTimeUtils.GetCurrentDateTime();

                var allowToViewSenderAccountingPrices = changeOwnerWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(waybill, user);
                ValidationUtils.Assert(allowToViewSenderAccountingPrices, "Нет прав на просмотр учетных цен.");

                var scale = waybill.Rows.Max(x => x.Article.MeasureUnit.Scale);
                var model = new T1ProductSectionPrintingFormViewModel()
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
                    TotalSumSeniorString = SpelledOutCurrency.Get(waybill.AccountingPriceSum, true, "", "", "", "", "", "", false),
                    TotalSumJuniorString = ((waybill.AccountingPriceSum % 1) * 100).ToString("00"),   // Получаем копейки
                    TotalSumValue = waybill.AccountingPriceSum.ForDisplay(ValueDisplayType.MoneyWithZeroCopecks),
                    ValueAddedTaxSum = waybill.Rows.Sum(x => x.ValueAddedTaxSum).ForDisplay(ValueDisplayType.MoneyWithZeroCopecks),
                    ValueAddedTaxPercentage = waybill.Rows.GroupBy(x => x.ValueAddedTax).Count() == 1 ?
                        waybill.Rows.First().ValueAddedTax.Value.ForDisplay(ValueDisplayType.Percent) : "",
                    WaybillId = waybill.Id.ToString(),
                    TotalCount = waybill.Rows.Sum(x => x.MovingCount).ForDisplay(scale),
                    CountScale = scale.ToString(),  // точность вывода количества (для JS)
                    TotalWeight = (waybill.Weight / 1000).ForDisplay(ValueDisplayType.WeightWithZeroFractionPart), //Выводим в тоннах
                    TotalSum = waybill.AccountingPriceSum.ForDisplay(ValueDisplayType.MoneyWithZeroCopecks)
                };

                return model;
            }
        }

        /// <summary>
        /// Получение позиций для формы Т1
        /// </summary>
        /// <param name="settings">Настройки печати</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель позиций</returns>
        public T1ProductSectionPrintingFormRowsViewModel ShowT1ProductSectionPrintingFormRows(T1PrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = changeOwnerWaybillService.CheckWaybillExistence(settings.WaybillId, user);
                var currentDate = DateTimeUtils.GetCurrentDateTime();

                var allowToViewSenderAccountingPrices = changeOwnerWaybillService.IsPossibilityToPrintFormInSenderAccountingPrices(waybill, user);
                ValidationUtils.Assert(allowToViewSenderAccountingPrices, "Нет прав на просмотр учетных цен.");

                var model = new T1ProductSectionPrintingFormRowsViewModel();

                foreach (var row in waybill.Rows)
                {
                    model.Rows.Add(new T1ProductSectionPrintingFormItemViewModel()
                    {
                        Count = row.MovingCount.ForDisplay(),
                        ItemNumber = row.Article.Id.ForDisplay(),
                        MeasureUnit = row.Article.MeasureUnit.ShortName,
                        Name = row.Article.FullName,
                        Number = row.Article.Number,
                        ListPriseNumber = "-",
                        Price = row.ArticleAccountingPrice.AccountingPrice.ForDisplay(ValueDisplayType.MoneyWithZeroCopecks),
                        Sum = (row.ArticleAccountingPrice.AccountingPrice * row.MovingCount).ForDisplay(ValueDisplayType.MoneyWithZeroCopecks),
                        Weight = (row.Weight / 1000).ForDisplay(ValueDisplayType.WeightWithZeroFractionPart)   // переводим в тонны
                    });
                }

                return model;
            }
        }

        #endregion

        #endregion

        #region Выбор накладной

        public ChangeOwnerWaybillSelectViewModel SelectWaybill(int articleId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new ChangeOwnerWaybillSelectViewModel();

                model.Data = GetWaybillSelectGrid(new GridState { PageSize = 5, Sort = "Date=Desc;CreationDate=Desc", Parameters = "Article=" + articleId.ToString() }, currentUser);

                model.FilterData = GetFilterData(user);

                return model;
            }
        }

        public GridData GetWaybillSelectGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                if (state == null)
                    state = new GridState();

                GridData model = new GridData();
                model.AddColumn("Action", "Действие", Unit.Pixel(58));
                model.AddColumn("IsAccepted", "Пр.", Unit.Pixel(22), align: GridColumnAlign.Center);
                model.AddColumn("Number", "Номер", Unit.Pixel(50), GridCellStyle.Link);
                model.AddColumn("Data", "Дата", Unit.Pixel(40));
                model.AddColumn("PurchaseCostSum", "Сумма в ЗЦ", Unit.Pixel(100), GridCellStyle.Label, GridColumnAlign.Right);
                model.AddColumn("AccountingPriceSum", "Сумма в УЦ", Unit.Pixel(100), align: GridColumnAlign.Right);
                model.AddColumn("StorageName", "Место хранения", Unit.Percentage(20), GridCellStyle.Link);
                model.AddColumn("SenderName", "Отправитель", Unit.Percentage(40), GridCellStyle.Link);
                model.AddColumn("RecipientName", "Получатель", Unit.Percentage(40), GridCellStyle.Link);
                model.AddColumn("Shipping", "Отгрузка", Unit.Pixel(50), align: GridColumnAlign.Right);
                model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
                model.AddColumn("Name", "", Unit.Pixel(0), GridCellStyle.Hidden);

                ParameterString deriveFilter = new ParameterString(state.Filter);

                var deriveParameters = new ParameterString(state.Parameters);

                if (deriveParameters.Keys.Contains("Article"))
                {
                    var articleId = deriveParameters["Article"].Value as string;
                    deriveFilter.Add("Article", ParameterStringItem.OperationType.Eq, articleId);
                }

                var rows = changeOwnerWaybillService.GetFilteredList(state, deriveFilter, user);

                GridActionCell actions = new GridActionCell("Action");
                actions.AddAction("Выбрать", "changeowner_waybill_select_link");

                var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

                foreach (var waybill in rows)
                {
                    decimal? accountingPriceSum = null;
                    if (user.HasPermissionToViewStorageAccountingPrices(waybill.Storage))
                    {
                        accountingPriceSum = changeOwnerWaybillIndicatorService.GetMainIndicators(waybill, user).AccountingPriceSum;
                    }

                    model.AddRow(new GridRow(
                        actions,
                        new GridLabelCell("IsAccepted") { Value = waybill.AcceptanceDate != null ? "П" : "" },
                        new GridLabelCell("Number") { Value = StringUtils.PadLeftZeroes(waybill.Number, 8) },
                        new GridLabelCell("Data") { Value = waybill.Date.ToShortDateString() },
                        new GridLabelCell("PurchaseCostSum") { Value = allowToViewPurchaseCost ? waybill.PurchaseCostSum.ForDisplay(ValueDisplayType.Money) : "---" },
                        new GridLabelCell("AccountingPriceSum") { Value = accountingPriceSum.ForDisplay(ValueDisplayType.Money) },
                        new GridLabelCell("StorageName") { Value = waybill.Storage.Name },
                        new GridLabelCell("SenderName") { Value = waybill.Sender.ShortName },
                        new GridLabelCell("RecipientName") { Value = waybill.Recipient.ShortName },
                        new GridLabelCell("Shipping") { Value = changeOwnerWaybillIndicatorService.CalculateShippingPercent(waybill).ForDisplay(ValueDisplayType.Percent) + " %" },

                        new GridHiddenCell("Id") { Value = waybill.Id.ToString() },
                        new GridHiddenCell("Name") { Value = waybill.Name, Key = "name" }
                        )
                    );
                }
                model.State = state;

                return model;
            }
        }

        #endregion

        #endregion
    }
}