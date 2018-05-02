using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
using ERP.Wholesale.UI.ViewModels.ReturnFromClientWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ReturnFromClientWaybillPresenter : BaseWaybillPresenter<ReturnFromClientWaybill>, IReturnFromClientWaybillPresenter
    {
        #region Поля

        private readonly IReturnFromClientWaybillService returnFromClientWaybillService;
        private readonly IReturnFromClientWaybillMainIndicatorService returnFromClientWaybillIndicatorService;
        private readonly IExpenditureWaybillService expenditureWaybillService;
        private readonly IStorageService storageService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly IReturnFromClientReasonService returnFromClientReasonService;
        private readonly IClientService clientService;
        private readonly IDealService dealService;
        private readonly IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService;
        private readonly ITeamService teamService;

        #endregion

        #region Конструкторы

        public ReturnFromClientWaybillPresenter(IUnitOfWorkFactory unitOfWorkFactory,
            IReturnFromClientWaybillService returnFromClientWaybillService, IReturnFromClientWaybillMainIndicatorService returnFromClientWaybillIndicatorService, IStorageService storageService,
            IAccountOrganizationService accountOrganizationService, IReturnFromClientReasonService returnFromClientReasonService,
            IUserService userService, IDealService dealService, IClientService clientService, IExpenditureWaybillService expenditureWaybillService,
            IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService, ITeamService teamService)
            :base(unitOfWorkFactory, returnFromClientWaybillService, userService)
        {
            this.returnFromClientWaybillService = returnFromClientWaybillService;
            this.returnFromClientWaybillIndicatorService = returnFromClientWaybillIndicatorService;
            this.storageService = storageService;
            this.accountOrganizationService = accountOrganizationService;
            this.returnFromClientReasonService = returnFromClientReasonService;
            this.clientService = clientService;
            this.dealService = dealService;
            this.expenditureWaybillService = expenditureWaybillService;
            this.expenditureWaybillIndicatorService = expenditureWaybillIndicatorService;
            this.teamService = teamService;
        }

        #endregion

        #region Методы

        #region Список

        public ReturnFromClientWaybillListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ReturnFromClientWaybill_List_Details);

                var model = new ReturnFromClientWaybillListViewModel();
                model.NewAndAcceptedReturnFromClientWaybillGrid = GetNewAndAcceptedReturnFromClientWaybillGridLocal(new GridState() { Sort = "Date=Desc;CreationDate=Desc" }, user);
                model.ReceiptedReturnFromClientWaybillGrid = GetReceiptedReturnFromClientWaybillGridLocal(new GridState() { Sort = "Date=Desc;CreationDate=Desc" }, user);

                model.FilterData = GetFilterData(user);

                return model;
            }
        }

        private FilterData GetFilterData(User user)
        {
            var filterData = new FilterData();

            var storageList = storageService.GetList().OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false);

            var accountOrganizationList = accountOrganizationService.GetList().OrderBy(x => x.ShortName)
                .GetComboBoxItemList(s => s.ShortName, s => s.Id.ToString());

            filterData.Items.Add(new FilterDateRangePicker("Date", "Дата возврата"));
            filterData.Items.Add(new FilterTextEditor("Number", "№ накладной"));
            filterData.Items.Add(new FilterTextEditor("Deal_Client_Name", "Клиент"));
            filterData.Items.Add(new FilterComboBox("RecipientStorage", "МХ-приемщик", storageList));
            filterData.Items.Add(new FilterTextEditor("Deal_Name", "Сделка"));
            filterData.Items.Add(new FilterComboBox("Recipient", "Организацияция-приемщик", accountOrganizationList));
            filterData.Items.Add(new FilterTextEditor("Curator_DisplayName", "Куратор"));

            return filterData;
        }

        #region Формирование гридов

        private GridData GetReturnFromClientWaybillGrid(GridState state, bool showIsAcceptedColumn, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData();

            if (showIsAcceptedColumn)
            {
                model.AddColumn("IsAccepted", "Пр.", Unit.Pixel(22), GridCellStyle.Link, align: GridColumnAlign.Center);
            }

            model.AddColumn("Number", "Номер", Unit.Pixel(60), GridCellStyle.Link);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Date", "Дата", Unit.Pixel(55));
            model.AddColumn("SalePriceSum", "Сумма в ОЦ", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("AccountingPriceSum", "Сумма в УЦ приемщика", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("ClientName", "Клиент", Unit.Percentage(34));
            model.AddColumn("ClientId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("RecipientStorageName", "Место хранения-приемщик", Unit.Percentage(36));
            model.AddColumn("RecipientName", "Организация-приемщик", Unit.Percentage(33));
            model.AddColumn("ShippingPercent", "Отгрузка", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("RecipientStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("RecipientId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var rows = returnFromClientWaybillService.GetFilteredList(state, user);

            foreach (var row in rows)
            {
                decimal accountingPriceSum = returnFromClientWaybillIndicatorService.CalculateAccountingPriceSum(row);
                var isAcceptedStr = "";
                if (row.IsReadyToAccept)
                {
                    isAcceptedStr = "Г";
                }
                else if (row.IsAccepted)
                {
                    isAcceptedStr = "П";
                }

                model.AddRow(new GridRow(
                    showIsAcceptedColumn ? new GridLabelCell("IsAccepted") { Value = isAcceptedStr } : null,
                    new GridLinkCell("Number") { Value = StringUtils.PadLeftZeroes(row.Number, 8) },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("SalePriceSum") { Value = row.SalePriceSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("AccountingPriceSum")
                    {
                        Value = (user.HasPermissionToViewStorageAccountingPrices(row.RecipientStorage) ?
                            accountingPriceSum.ForDisplay(ValueDisplayType.Money) : "---")
                    },
                    user.HasPermission(Permission.Client_List_Details) ?
                        (GridCell)new GridLinkCell("ClientName") { Value = row.Client.Name } :
                        new GridLabelCell("ClientName") { Value = row.Client.Name },
                    new GridHiddenCell("ClientId") { Value = row.Client.Id.ToString() },
                    storageService.IsPossibilityToViewDetails(row.RecipientStorage, user) ?
                        (GridCell)new GridLinkCell("RecipientStorageName") { Value = row.RecipientStorage.Name } :
                        new GridLabelCell("RecipientStorageName") { Value = row.RecipientStorage.Name },
                        new GridLinkCell("RecipientName") { Value = row.Recipient.ShortName },
                        new GridLabelCell("ShippingPercent") { Value = returnFromClientWaybillIndicatorService.CalculateShippingPercent(row).ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("RecipientStorageId") { Value = row.RecipientStorage.Id.ToString() },
                    new GridHiddenCell("RecipientId") { Value = row.Recipient.Id.ToString() }
                ));
            }

            model.State = state;

            return model;
        }

        public GridData GetNewAndAcceptedReturnFromClientWaybillGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetNewAndAcceptedReturnFromClientWaybillGridLocal(state, user);
            }
        }

        private GridData GetNewAndAcceptedReturnFromClientWaybillGridLocal(GridState state, User user)
        {
            var deriveFilter = new ParameterString(state.Filter);
            if (!deriveFilter.Keys.Contains("State"))
            {
                var item = deriveFilter.Add("State", ParameterStringItem.OperationType.OneOf);
                deriveFilter["State"].Value = new List<string>()
                {
                    ReturnFromClientWaybillState.Draft.ValueToString(),
                    ReturnFromClientWaybillState.ReadyToAccept.ValueToString(),
                    ReturnFromClientWaybillState.Accepted.ValueToString()
                };
                state.Filter = deriveFilter.ToString();
            }

            var model = GetReturnFromClientWaybillGrid(state, true, user);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.ReturnFromClientWaybill_Create_Edit);

            return model;
        }

        public GridData GetReceiptedReturnFromClientWaybillGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetReceiptedReturnFromClientWaybillGridLocal(state, user);
            }
        }

        private GridData GetReceiptedReturnFromClientWaybillGridLocal(GridState state, User user)
        {
            var deriveFilter = new ParameterString(state.Filter);
            if (!deriveFilter.Keys.Contains("State"))
            {
                deriveFilter.Add("State", ParameterStringItem.OperationType.Eq, ReturnFromClientWaybillState.Receipted.ValueToString());
                state.Filter = deriveFilter.ToString();
            }

            return GetReturnFromClientWaybillGrid(state, false, user);
        }

        #endregion

        #endregion

        #region Добавление / редактирование / сохранение

        public ReturnFromClientWaybillEditViewModel Create(int? clientId, int? dealId, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ReturnFromClientWaybill_Create_Edit);

                var model = new ReturnFromClientWaybillEditViewModel()
                {
                    Title = "Добавление накладной возврата от клиента",
                    BackURL = backURL,
                    Date = DateTime.Today.ToShortDateString(),
                    Number = "",
                    AllowToGenerateNumber = true,
                    IsAutoNumber = "1",
                    CuratorId = user.Id.ToString(),
                    CuratorName = user.DisplayName,
                    ClientId = "0",
                    ClientName = "Выберите клиента",
                    DealId = "0",
                    DealName = "Выберите сделку",
                    TeamId = "",
                    TeamList = ComboBoxBuilder.GetComboBoxItemList(new List<Team>(), x => x.Name, x => x.Id.ToString()),
                    AllowToEditTeam = true,
                    AccountOrganizationName = "---",
                    ReturnFromClientReasonList = returnFromClientReasonService.GetList().GetComboBoxItemList(x => x.Name, x => x.Id.ToString()),

                    AllowToEdit = true,
                    AllowToCreateReturnFromClientReason = user.HasPermission(Permission.ReturnFromClientReason_Create),
                    AllowToChangeCurator = user.HasPermission(Permission.ReturnFromClientWaybill_Curator_Change)
                };

                if (dealId != null)
                {
                    var deal = dealService.CheckDealExistence(dealId.Value, user, Permission.ReturnFromClientWaybill_Create_Edit);
                    dealService.CheckPossibilityToCreateReturnFromClientWaybill(deal, user);

                    model.DealId = dealId.Value.ToString();
                    model.DealName = deal.Name;

                    clientId = deal.Client.Id;

                    model.AccountOrganizationId = deal.Contract.AccountOrganization.Id.ToString();
                    model.AccountOrganizationName = deal.Contract.AccountOrganization.ShortName;

                    var storageList = GetStorageList(deal.Contract.AccountOrganization.Id, currentUser);

                    model.ReceiptStorageList = storageList.GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), addEmptyItem: true, sort: false);

                    if (storageList.Count() == 1)
                    {
                        model.ReceiptStorageId = storageList.First().Id.ToString();
                    }

                    var teamList = dealService.GetTeamListFromSales(deal);
                    model.TeamList = ComboBoxBuilder.GetComboBoxItemList(teamList, x => x.Name, x => x.Id.ToString(), teamList.Count() != 1);

                    if (teamList.Count() == 1)
                    {
                        model.TeamId = teamList.First().Id.ToString();
                    }
                }

                if (clientId != null)
                {
                    var client = clientService.CheckClientExistence(clientId.Value, user);

                    model.ClientId = clientId.Value.ToString();
                    model.ClientName = client.Name;
                }

                return model;
            }
        }

        /// <summary>
        /// Получение списка команд для возврата товара по сделке
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        public object GetTeamListForReturnFromClientWaybill(int dealId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);

                var teamList = dealService.GetTeamListFromSales(deal);
                var itemList = ComboBoxBuilder.GetComboBoxItemList(teamList, x => x.Name, x => x.Id.ToString(), teamList.Count() != 1);
                var firstTeam = (teamList.Count() == 1 ? teamList.First() : null);

                return new { List = itemList, SelectedOption = firstTeam != null ? firstTeam.Id.ToString() : "" };
            }
        }

        public ReturnFromClientWaybillEditViewModel Edit(Guid id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = returnFromClientWaybillService.CheckWaybillExistence(id, user);

                returnFromClientWaybillService.CheckPossibilityToEdit(waybill, user);

                var model = new ReturnFromClientWaybillEditViewModel()
                {
                    Title = "Редактирование накладной возврата от клиента",
                    AccountOrganizationId = waybill.Recipient.Id.ToString(),
                    AccountOrganizationName = waybill.Recipient.ShortName,
                    BackURL = backURL,
                    ClientId = waybill.Client.Id.ToString(),
                    ClientName = waybill.Client.Name,
                    DealId = waybill.Deal.Id.ToString(),
                    DealName = waybill.Deal.Name,
                    TeamId = waybill.Team.Id.ToString(),
                    TeamList = ComboBoxBuilder.GetComboBoxItemList(dealService.GetTeamListFromSales(waybill.Deal), x => x.Name, x => x.Id.ToString(), false),
                    Comment = waybill.Comment,
                    Date = DateTime.Today.ToShortDateString(),
                    Id = waybill.Id,
                    Name = waybill.Name,
                    Number = waybill.Number,
                    AllowToGenerateNumber = false,
                    IsAutoNumber = "0",
                    ReceiptStorageId = waybill.RecipientStorage.Id.ToString(),
                    ReturnFromClientReasonId = waybill.ReturnFromClientReason.Id,
                    CuratorId = waybill.Curator.Id.ToString(),
                    CuratorName = waybill.Curator.DisplayName,
                    ReceiptStorageList = new List<Storage> { waybill.RecipientStorage }.GetComboBoxItemList(x => x.Name, x => x.Id.ToString()),

                    ReturnFromClientReasonList = returnFromClientReasonService.GetList().GetComboBoxItemList(x => x.Name, x => x.Id.ToString()),

                    AllowToEdit = false,
                    AllowToEditTeam = returnFromClientWaybillService.IsPossibilityToEditTeam(waybill, user),
                    AllowToCreateReturnFromClientReason = user.HasPermission(Permission.ReturnFromClientReason_Create),
                    AllowToChangeCurator = returnFromClientWaybillService.IsPossibilityToChangeCurator(waybill, user)
                };

                return model;
            }
        }

        public Guid Save(ReturnFromClientWaybillEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var recipientStorage = storageService.CheckStorageExistence(ValidationUtils.TryGetShort(model.ReceiptStorageId), user, "Место хранения получателя не найдено. Возможно, оно было удалено.");
                var recipient = accountOrganizationService.CheckAccountOrganizationExistence(ValidationUtils.TryGetInt(model.AccountOrganizationId),
                    "Организация получателя не найдена. Возможно, она было удалена.");
                var client = clientService.CheckClientExistence(ValidationUtils.TryGetInt(model.ClientId), user);
                var deal = dealService.CheckDealExistence(ValidationUtils.TryGetInt(model.DealId), user, Permission.ReturnFromClientWaybill_Create_Edit);
                var returnFromClientReason = returnFromClientReasonService.CheckExistence(model.ReturnFromClientReasonId);
                var team = teamService.CheckTeamExistence(ValidationUtils.TryGetShort(model.TeamId));
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                ReturnFromClientWaybill waybill = null;

                bool isAutoNumber = ValidationUtils.TryGetBool(model.IsAutoNumber);

                // добавление
                if (model.Id == Guid.Empty)
                {
                    user.CheckPermission(Permission.ReturnFromClientWaybill_Create_Edit);

                    var curator = userService.CheckUserExistence(ValidationUtils.TryGetInt(model.CuratorId), user, "Куратор не найден. Возможно, он был удален.");

                    if (isAutoNumber)
                    {
                        model.Number = "";
                    }

                    waybill = new ReturnFromClientWaybill(model.Number, DateTime.Parse(model.Date), recipient, deal, team, recipientStorage, returnFromClientReason, curator, user, currentDateTime);

                    if (curator != user)
                    {
                        user.CheckPermission(Permission.ReturnFromClientWaybill_Curator_Change);
                        returnFromClientWaybillService.CheckPossibilityToViewDetailsByUser(waybill, curator);
                    }
                }
                // редактирование
                else
                {
                    waybill = returnFromClientWaybillService.CheckWaybillExistence(model.Id, user);

                    returnFromClientWaybillService.CheckPossibilityToEdit(waybill, user);

                    waybill.Team = team;
                    waybill.ReturnFromClientReason = returnFromClientReason;
                    waybill.Number = model.Number;

                    var curatorId = ValidationUtils.TryGetInt(model.CuratorId);
                    if (waybill.Curator.Id != curatorId)
                    {
                        var curator = userService.CheckUserExistence(curatorId, user, "Куратор не найден. Возможно, он был удален.");

                        returnFromClientWaybillService.CheckPossibilityToChangeCurator(waybill, user);
                        returnFromClientWaybillService.CheckPossibilityToViewDetailsByUser(waybill, curator);

                        waybill.Curator = curator;
                    }
                }

                waybill.Comment = StringUtils.ToHtml(model.Comment); 

                var waybillId = returnFromClientWaybillService.Save(waybill);

                uow.Commit();

                return waybillId;
            }
        }

        public object GetStorageListForAccountOrganization(int accountOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var storageList = GetStorageList(accountOrganizationId, currentUser);

                var selected = storageList.Count() == 1 ? storageList.First().Id.ToString() : "0";

                var json = new
                {
                    List = storageList.OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                        .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), addEmptyItem: false, sort: false),
                    SelectedOption = selected
                };

                return json;
            }
        }

        private IEnumerable<Storage> GetStorageList(int accountOrganizationId, UserInfo currentUser)
        {
            var user = userService.CheckUserExistence(currentUser.Id);
            var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(accountOrganizationId);

            // выбираем места хранения, связанные с указанной собственной организацией и фильтруем по командам текущего пользователя
            return storageService.FilterByUser(accountOrganization.Storages, user, Permission.ReturnFromClientWaybill_Create_Edit);
        }

        public object GetReturnFromClientReasonList(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new { List = ComboBoxBuilder.GetComboBoxItemList(returnFromClientReasonService.GetList(), x => x.Name, x => x.Id.ToString(), false) };

                return model;
            }
        }

        #region Удаление

        public void Delete(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = returnFromClientWaybillService.CheckWaybillExistence(id, user);

                returnFromClientWaybillService.Delete(waybill, user);

                uow.Commit();
            }
        }

        #endregion

        #endregion

        #region Детали

        public ReturnFromClientWaybillDetailsViewModel Details(Guid id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = returnFromClientWaybillService.CheckWaybillExistence(id, user);

                var model = new ReturnFromClientWaybillDetailsViewModel()
                {
                    Id = waybill.Id,
                    Name = waybill.Name,
                    MainDetails = GetMainDetails(waybill, user),
                    RowGrid = GetReturnFromClientWaybillRowGridLocal(new GridState() { Parameters = "ReturnFromClientWaybillId=" + waybill.Id, Sort = "CreationDate=Asc" }, user),
                    ArticleGroupGridState = new GridState() { Parameters = "ReturnFromClientWaybillId=" + waybill.Id },
                    
                    BackURL = backURL,
                    DocumentGrid = GetDocumentGridLocal(new GridState()),

                    AllowToEdit = returnFromClientWaybillService.IsPossibilityToEdit(waybill, user),
                    AllowToDelete = returnFromClientWaybillService.IsPossibilityToDelete(waybill, user),
                    AllowToPrepareToAccept = returnFromClientWaybillService.IsPossibilityToPrepareToAccept(waybill, user),
                    IsPossibilityToPrepareToAccept = returnFromClientWaybillService.IsPossibilityToPrepareToAccept(waybill, user, true),
                    AllowToCancelReadinessToAccept = returnFromClientWaybillService.IsPossibilityToCancelReadinessToAccept(waybill, user),
                    AllowToAccept = returnFromClientWaybillService.IsPossibilityToAccept(waybill, user),
                    IsPossibilityToAccept = returnFromClientWaybillService.IsPossibilityToAccept(waybill, user, true),
                    AllowToCancelAcceptance = returnFromClientWaybillService.IsPossibilityToCancelAcceptance(waybill, user),
                    AllowToReceipt = returnFromClientWaybillService.IsPossibilityToReceipt(waybill, user),
                    AllowToCancelReceipt = returnFromClientWaybillService.IsPossibilityToCancelReceipt(waybill, user),
                    AllowToPrintForms = returnFromClientWaybillService.IsPossibilityToPrintForms(waybill, user)
                };

                return model;
            }
        }

        /// <summary>
        /// Получение главных деталей накладной реализации товаров
        /// </summary>
        private ReturnFromClientWaybillMainDetailsViewModel GetMainDetails(ReturnFromClientWaybill waybill, User user)
        {
            var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

            decimal accountingPriceSum = returnFromClientWaybillIndicatorService.CalculateAccountingPriceSum(waybill);

            var model = new ReturnFromClientWaybillMainDetailsViewModel()
            {
                AcceptedById = waybill.AcceptedBy != null ? waybill.AcceptedBy.Id.ToString() : "",
                AcceptedByName = waybill.AcceptedBy != null ? waybill.AcceptedBy.DisplayName : "",
                AcceptanceDate = waybill.AcceptedBy != null ? String.Format("({0})",waybill.AcceptanceDate.Value.ToShortDateTimeString()) : "",
                AccountingPriceSum = (user.HasPermissionToViewStorageAccountingPrices(waybill.RecipientStorage) ?
                    accountingPriceSum.ForDisplay(ValueDisplayType.Money) : "---"),
                Comment = waybill.Comment,
                ClientId = waybill.Client.Id.ToString(),
                ClientName = waybill.Client.Name,
                DealId = waybill.Deal.Id.ToString(),
                DealName = waybill.Deal.Name,
                TeamName = waybill.Team.Name,
                TeamId = waybill.Team.Id.ToString(),
                CuratorId = waybill.Curator.Id.ToString(),
                CuratorName = waybill.Curator.DisplayName,
                CreatedById = waybill.CreatedBy.Id.ToString(),
                CreatedByName = waybill.CreatedBy.DisplayName,
                CreationDate = String.Format("({0})",waybill.CreationDate.ToShortDateTimeString()),
                RecipientId = waybill.Recipient.Id.ToString(),
                RecipientName = waybill.Recipient.ShortName,
                PurchaseCostSum = allowToViewPurchaseCost ? waybill.PurchaseCostSum.ForDisplay(ValueDisplayType.Money) : "---",
                ReasonName = waybill.ReturnFromClientReason.Name,
                ReceiptedById = waybill.ReceiptedBy != null ? waybill.ReceiptedBy.Id.ToString() : "",
                ReceiptedByName = waybill.ReceiptedBy != null ? waybill.ReceiptedBy.DisplayName : "",
                ReceiptDate = waybill.ReceiptedBy != null ? String.Format("({0})",waybill.ReceiptDate.Value.ToShortDateTimeString()) : "",
                RowCount = waybill.RowCount.ForDisplay(),
                ShippingPercent = returnFromClientWaybillIndicatorService.CalculateShippingPercent(waybill).ForDisplay(ValueDisplayType.Percent),
                SalePriceSum = waybill.SalePriceSum.ForDisplay(ValueDisplayType.Money),
                StateName = waybill.State.GetDisplayName(),
                RecipientStorageId = waybill.RecipientStorage.Id.ToString(),
                RecipientStorageName = waybill.RecipientStorage.Name,
                TotalWeight = waybill.Weight.ForDisplay(ValueDisplayType.Weight),
                TotalVolume = waybill.Volume.ForDisplay(ValueDisplayType.Volume),

                AllowToViewAcceptedByDetails = userService.IsPossibilityToViewDetails(waybill.AcceptedBy, user),
                AllowToViewClientDetails = user.HasPermission(Permission.Client_List_Details),
                AllowToViewCuratorDetails = userService.IsPossibilityToViewDetails(waybill.Curator, user),
                AllowToViewDealDetails = dealService.IsPossibilityToViewDetails(waybill.Deal, user),
                AllowToViewTeamDetails = teamService.IsPossibilityToViewDetails(waybill.Team, user),
                AllowToViewCreatedByDetails = userService.IsPossibilityToViewDetails(waybill.CreatedBy, user),
                AllowToViewReceiptedByDetails = userService.IsPossibilityToViewDetails(waybill.ReceiptedBy, user),
                AllowToViewRecipientStorageDetails = storageService.IsPossibilityToViewDetails(waybill.RecipientStorage, user),
                AllowToChangeCurator = returnFromClientWaybillService.IsPossibilityToChangeCurator(waybill, user)
            };

            return model;
        }

        /// <summary>
        /// Получение значений основных пересчитываемых показателей
        /// </summary>
        private object GetMainChangeableIndicators(ReturnFromClientWaybill waybill, User user)
        {
            var j = new
            {
                MainDetails = GetMainDetails(waybill, user),
                Permissions = new
                {
                    AllowToEdit = returnFromClientWaybillService.IsPossibilityToEdit(waybill, user),
                    AllowToDelete = returnFromClientWaybillService.IsPossibilityToDelete(waybill, user),
                    AllowToPrepareToAccept = returnFromClientWaybillService.IsPossibilityToPrepareToAccept(waybill, user),
                    IsPossibilityToPrepareToAccept = returnFromClientWaybillService.IsPossibilityToPrepareToAccept(waybill, user, true),
                    AllowToCancelReadinessToAccept = returnFromClientWaybillService.IsPossibilityToCancelReadinessToAccept(waybill, user),
                    AllowToAccept = returnFromClientWaybillService.IsPossibilityToAccept(waybill, user),
                    IsPossibilityToAccept = returnFromClientWaybillService.IsPossibilityToAccept(waybill, user, true) && !waybill.IsAccepted,
                    AllowToCancelAcceptance = returnFromClientWaybillService.IsPossibilityToCancelAcceptance(waybill, user),
                    AllowToReceipt = returnFromClientWaybillService.IsPossibilityToReceipt(waybill, user),
                    AllowToCancelReceipt = returnFromClientWaybillService.IsPossibilityToCancelReceipt(waybill, user),
                    AllowToPrintForms = returnFromClientWaybillService.IsPossibilityToPrintForms(waybill, user),
                    AllowToChangeCurator = returnFromClientWaybillService.IsPossibilityToChangeCurator(waybill, user)
                }
            };

            return j;
        }

        #region Позиции накладной и группы товаров

        public GridData GetReturnFromClientWaybillRowGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetReturnFromClientWaybillRowGridLocal(state, user);
            }
        }

        private GridData GetReturnFromClientWaybillRowGridLocal(GridState state, User user)
        {
            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(68));
            model.AddColumn("ArticleId", "Код товара", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("ArticleNumber", "Артикул", Unit.Pixel(60));
            model.AddColumn("ArticleName", "Наименование товара", Unit.Percentage(60));
            model.AddColumn("SaleWaybillName", "Документ реализации", Unit.Percentage(40));
            model.AddColumn("TotalSoldCount", "Кол-во реализовано", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("ReturnedCount", "Возвращено по другим накладным", Unit.Pixel(120), align: GridColumnAlign.Right);
            model.AddColumn("CurrentReturnCount", "Возврат по данной накладной", Unit.Pixel(120), align: GridColumnAlign.Right);
            model.AddColumn("ShippedCount", "Отгрузка", Unit.Pixel(55), align: GridColumnAlign.Right);
            model.AddColumn("SaleWaybillId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var deriveParams = new ParameterString(state.Parameters);
            var waybill = returnFromClientWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid(deriveParams["ReturnFromClientWaybillId"].Value as string), user);

            var allowToEdit = returnFromClientWaybillService.IsPossibilityToEdit(waybill, user);
            model.ButtonPermissions["AllowToCreateRow"] = allowToEdit;

            foreach (var item in waybill.Rows.OrderBy(x => x.CreationDate))
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

                if (returnFromClientWaybillService.IsPossibilityToDeleteRow(item, user))
                {
                    actions.AddAction("Удал.", "delete_link");
                }

                var saleWaybillRow = item.SaleWaybillRow;
                var article = saleWaybillRow.Article;

                decimal totalSoldCount = item.SaleWaybillRow.SellingCount;
                // уже возвращено всеми накладными
                var returnedAll = item.SaleWaybillRow.ReservedByReturnCount;
                // если накладная еще не проведена, то ее кол-во не учитываем (не вычитаем)
                var returnedCount = returnedAll - (waybill.IsNew ? 0 : item.ReturnCount);

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("ArticleId") { Value = article.Id.ToString() },
                    new GridLabelCell("ArticleNumber") { Value = article.Number },
                    new GridLabelCell("ArticleName") { Value = article.FullName },
                    expenditureWaybillService.IsPossibilityToViewDetails(saleWaybillRow.SaleWaybill.As<ExpenditureWaybill>(), user) ?
                        (GridCell)new GridLinkCell("SaleWaybillName") { Value = saleWaybillRow.SaleWaybill.Name } :
                        new GridLabelCell("SaleWaybillName") { Value = saleWaybillRow.SaleWaybill.Name },
                    new GridLabelCell("TotalSoldCount") { Value = totalSoldCount.ForDisplay() },
                    new GridLabelCell("ReturnedCount") { Value = returnedCount.ForDisplay() },
                    new GridLabelCell("CurrentReturnCount") { Value = item.ReturnCount.ForDisplay() },
                    new GridLabelCell("ShippedCount") { Value = item.TotallyReservedCount.ForDisplay() },
                    new GridHiddenCell("SaleWaybillId") { Value = saleWaybillRow.SaleWaybill.Id.ToString() },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "returnFromClientWaybillRowId" }
               ));
            }

            model.State = state;
            model.State.TotalRow = model.RowCount;

            return model;
        }

        /// <summary>
        /// Получить грид с группами товаров
        /// </summary>
        public GridData GetReturnFromClientWaybillArticleGroupGrid(GridState state, UserInfo currentUser)
        {

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetReturnFromClientWaybillArticleGroupGridLocal(state, user);
            }

        }

        private GridData GetReturnFromClientWaybillArticleGroupGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            var deriveParams = new ParameterString(state.Parameters);
            var waybill = returnFromClientWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid(deriveParams["ReturnFromClientWaybillId"].Value.ToString()), user);
            
            var articleGroups = waybill.Rows.GroupBy(x => x.Article.ArticleGroup);

            var rows= new List<BaseWaybillArticleGroupRow>();
        
            foreach (var articleGroup in articleGroups.OrderBy(x => x.Key.Name))
            {
                var row = new BaseWaybillArticleGroupRow();

                row.Name = articleGroup.Key.Name;
                row.ArticleCount = articleGroup.Sum(x => x.ReturnCount);
                row.PackCount = articleGroup.Sum(x => x.PackCount);
                row.Sum = articleGroup.Sum(x => Math.Round((x.SaleWaybillRow.SalePrice.HasValue ? x.SaleWaybillRow.SalePrice.Value : 0) * x.ReturnCount, 2));

                rows.Add(row);
            }

            GridData model = GetArticleGroupGrid(rows, false);
            model.State = state;
            model.State.TotalRow = model.RowCount;


            return model;
        }

        #endregion

        #region Добавление / удаление позиций

        public ReturnFromClientWaybillRowEditViewModel AddRow(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = returnFromClientWaybillService.CheckWaybillExistence(id, user);

                returnFromClientWaybillService.CheckPossibilityToEdit(waybill, user);

                var model = new ReturnFromClientWaybillRowEditViewModel()
                {
                    Title = "Добавление позиции в накладную",
                    AccountingPrice = "---",
                    ArticleName = "Выберите товар",
                    MeasureUnitName = "",
                    MeasureUnitScale = "0",
                    AvailableToReturnCount = "---",
                    RecipientId = waybill.Recipient.Id.ToString(),
                    RecipientStorageId = waybill.RecipientStorage.Id.ToString(),
                    ClientId = waybill.Client.Id.ToString(),
                    DealId = waybill.Deal.Id.ToString(),
                    TeamId = waybill.Team.Id.ToString(),
                    ReturnedCount = "---",
                    ReturnFromClientWaybillDate = waybill.Date.ToString(),
                    ReturnFromClientWaybillId = waybill.Id,
                    ReturningCount = "",
                    SaleWaybillName = "не выбран",
                    SalePrice = "---",
                    TotalSoldCount = "---",
                    PurchaseCost = "---",
                    AllowToEdit = true,
                    AllowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere),
                    AllowToViewAccountingPrice = user.HasPermissionToViewStorageAccountingPrices(waybill.RecipientStorage)
                };

                return model;
            }
        }

        public ReturnFromClientWaybillRowEditViewModel EditRow(Guid returnFromClientWaybillId, Guid returnFromClientWaybillRowId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var returnFromClientWaybill = returnFromClientWaybillService.CheckWaybillExistence(returnFromClientWaybillId, user);
                var returnFromClientWaybillRow = returnFromClientWaybillService.CheckWaybillRowExistence(returnFromClientWaybillRowId);
                var saleWaybillRow = returnFromClientWaybillRow.SaleWaybillRow;
                var article = returnFromClientWaybillRow.Article;

                var allowToEdit = returnFromClientWaybillService.IsPossibilityToEdit(returnFromClientWaybill, user);
                var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

                decimal totalSoldCount = saleWaybillRow.SellingCount;
                
                // уже возвращено всеми накладными
                var returnedCount = saleWaybillRow.ReservedByReturnCount;
                // если накладная уже проведена, то вычитаем резервируемое кол-во по данной позиции
                returnedCount -= (returnFromClientWaybill.IsNew ? 0 : returnFromClientWaybillRow.ReturnCount);
                
                var availableToReturnCount = saleWaybillRow.AvailableToReturnCount;
                decimal? salePrice = returnFromClientWaybillRow.SalePrice;

                var indicators = returnFromClientWaybillIndicatorService.CalculateMainRowIndicators(returnFromClientWaybillRow, user);

                var model = new ReturnFromClientWaybillRowEditViewModel()
                {
                    Title = allowToEdit ? "Редактирование позиции накладной" : "Детали позиции накладной",
                    Id = returnFromClientWaybillRow.Id,
                    SaleWaybillRowId = returnFromClientWaybillRow.SaleWaybillRow.Id,
                    CurrentSaleWaybillRowId = returnFromClientWaybillRow.SaleWaybillRow.Id,
                    ArticleId = returnFromClientWaybillRow.Article.Id,
                    MeasureUnitName = returnFromClientWaybillRow.Article.MeasureUnit.ShortName,
                    MeasureUnitScale = returnFromClientWaybillRow.SaleWaybillRow.ArticleMeasureUnitScale.ToString(),
                    TotalSoldCount = totalSoldCount.ForDisplay(),
                    ArticleName = returnFromClientWaybillRow.Article.FullName,
                    AvailableToReturnCount = availableToReturnCount.ForDisplay(),
                    SaleWaybillName = returnFromClientWaybillRow.SaleWaybillRow.SaleWaybill.Name,
                    ReturnFromClientWaybillDate = returnFromClientWaybill.Date.ToString(),
                    PurchaseCost = (allowToViewPurchaseCost ? returnFromClientWaybillRow.SaleWaybillRow.PurchaseCost.ForDisplay(ValueDisplayType.Money) : "---"),
                    AccountingPrice = (indicators.AccountingPrice != null && user.HasPermissionToViewStorageAccountingPrices(returnFromClientWaybill.RecipientStorage) ?
                        indicators.AccountingPrice.Value.ForDisplay(ValueDisplayType.Money) : "---"),
                    ClientId = returnFromClientWaybill.Client.Id.ToString(),
                    DealId = returnFromClientWaybill.Deal.Id.ToString(),
                    TeamId = returnFromClientWaybill.Team.Id.ToString(),
                    RecipientId = returnFromClientWaybill.Recipient.Id.ToString(),
                    RecipientStorageId = returnFromClientWaybill.RecipientStorage.Id.ToString(),
                    ReturningCount = returnFromClientWaybillRow.ReturnCount.ForEdit(),
                    ReturnFromClientWaybillId = returnFromClientWaybill.Id,
                    SalePrice = salePrice != null ? salePrice.Value.ForDisplay(ValueDisplayType.Money) : "---",
                    ReturnedCount = returnedCount.ForDisplay(),
                    AllowToEdit = allowToEdit,
                    AllowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere),
                    AllowToViewAccountingPrice = user.HasPermissionToViewStorageAccountingPrices(returnFromClientWaybill.RecipientStorage)
                };

                return model;
            }
        }

        public object SaveRow(ReturnFromClientWaybillRowEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ValidationUtils.NotNullOrDefault(model.ReturnFromClientWaybillId, "Накладная возврата товаров не найдена. Возможно, она была удалена.");
                ValidationUtils.NotNullOrDefault(model.SaleWaybillRowId, "Накладная реализации товара не найдена. Возможно, она была удалена.");

                var returnFromClientWaybill = returnFromClientWaybillService.CheckWaybillExistence(model.ReturnFromClientWaybillId, user);

                returnFromClientWaybillService.CheckPossibilityToEdit(returnFromClientWaybill, user);

                var saleWaybillRow = expenditureWaybillService.GetRowById(model.SaleWaybillRowId);

                ReturnFromClientWaybillRow row = null;
                var returningCount = ValidationUtils.TryGetDecimal(model.ReturningCount);

                // Добавление
                if (model.Id == Guid.Empty)
                {
                    row = new ReturnFromClientWaybillRow(saleWaybillRow, returningCount);

                    returnFromClientWaybillService.AddRow(returnFromClientWaybill, row);
                }
                // Редактирование
                else
                {
                    row = returnFromClientWaybill.Rows.FirstOrDefault(x => x.Id == model.Id);
                    ValidationUtils.NotNull(row, "Позиция накладной не найдена. Возможно, она была удалена.");

                    row.SaleWaybillRow = saleWaybillRow; // Партию важно присваивать перед количеством, чтобы при смене товара проверялась точность количества уже по новому товару
                    row.ReturnCount = returningCount;

                    returnFromClientWaybillService.Save(returnFromClientWaybill);
                }

                uow.Commit();

                return GetMainChangeableIndicators(returnFromClientWaybill, user);
            }
        }

        public object DeleteRow(Guid returnFromClientWaybillId, Guid returnFromClientWaybillRowId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var returnFromClientWaybill = returnFromClientWaybillService.CheckWaybillExistence(returnFromClientWaybillId, user);

                var returnFromClientWaybillRow = returnFromClientWaybill.Rows.FirstOrDefault(x => x.Id == returnFromClientWaybillRowId);
                ValidationUtils.NotNull(returnFromClientWaybillRow, "Позиция накладной не найдена. Возможно, она была удалена.");

                returnFromClientWaybillService.DeleteRow(returnFromClientWaybill, returnFromClientWaybillRow, user);

                uow.Commit();

                return GetMainChangeableIndicators(returnFromClientWaybill, user);
            }
        }

        #endregion

        #region Документы по накладной

        public GridData GetDocumentGrid(GridState state)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                return GetDocumentGridLocal(state);
            }
        }

        private GridData GetDocumentGridLocal(GridState state)
        {
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

        #region Подготовка / Отменить готовность к проводке

        public object PrepareToAccept(Guid id, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = returnFromClientWaybillService.CheckWaybillExistence(id, user);

                returnFromClientWaybillService.PrepareToAccept(waybill, user);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user); 
            }
        }

        public object CancelReadinessToAccept(Guid id, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = returnFromClientWaybillService.CheckWaybillExistence(id, user);

                returnFromClientWaybillService.CancelReadinessToAccept(waybill, user);

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
                var waybill = returnFromClientWaybillService.CheckWaybillExistence(id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                returnFromClientWaybillService.Accept(waybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        public object CancelAcceptance(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = returnFromClientWaybillService.CheckWaybillExistence(id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                returnFromClientWaybillService.CancelAcceptance(waybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }
        #endregion

        #region Приемка / отмена приемки

        public object Receipt(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = returnFromClientWaybillService.CheckWaybillExistence(id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                returnFromClientWaybillService.Receipt(waybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        public object CancelReceipt(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = returnFromClientWaybillService.CheckWaybillExistence(id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                returnFromClientWaybillService.CancelReceipt(waybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        #endregion

        #region Печатные формы

        #region ТОРГ 12

        /// <summary>
        /// Получение модели параметров ТОРГ12
        /// </summary>
        public TORG12PrintingFormSettingsViewModel GetTORG12PrintingFormSettings(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = returnFromClientWaybillService.CheckWaybillExistence(waybillId, user);

                var allowToViewSalePrices = returnFromClientWaybillService.IsPossibilityToPrintFormInSalePrices(waybill, user);
                var allowToViewPurchaseCosts = returnFromClientWaybillService.IsPossibilityToPrintFormInPurchaseCosts(waybill, user);

                ValidationUtils.Assert(allowToViewSalePrices || allowToViewPurchaseCosts,
                    "Нет прав на просмотр ни отпускных цен, ни закупочных цен.");

                var priceTypes = new List<PrintingFormPriceType>();
                if (allowToViewSalePrices) priceTypes.Add(PrintingFormPriceType.SalePrice);
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
                var waybill = returnFromClientWaybillService.CheckWaybillExistence(settings.WaybillId, user);

                var priceType = ValidationUtils.TryGetEnum<PrintingFormPriceType>(settings.PriceTypeId);

                // проверка возможности просмотра определенного типа цен
                CheckPriceTypePermissions(user, waybill, priceType);

                var organizationJuridicalPerson = waybill.Deal.Contract.ContractorOrganization.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                    waybill.Deal.Contract.ContractorOrganization.EconomicAgent.As<JuridicalPerson>() : null;

                var payerJuridicalPerson = waybill.Recipient.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                    waybill.Recipient.EconomicAgent.As<JuridicalPerson>() : null;

                var recepientJuridicalPerson = waybill.Recipient.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                    waybill.Recipient.EconomicAgent.As<JuridicalPerson>() : null;

                var model = new TORG12PrintingFormViewModel()
                {
                    PriceTypeId = settings.PriceTypeId,
                    WaybillId = waybill.Id.ToString(),
                    OrganizationName = waybill.Deal.Contract.ContractorOrganization.GetFullInfo(),
                    Date = waybill.Date.ToShortDateString(),
                    OrganizationOKPO = organizationJuridicalPerson != null ? organizationJuridicalPerson.OKPO : "",
                    Payer = waybill.Recipient.GetFullInfo(),
                    PayerOKPO = payerJuridicalPerson != null ? payerJuridicalPerson.OKPO : "",
                    Reason = string.Format("Возврат товаров по договору «{0}»", waybill.Deal.Contract.FullName),
                    ReasonDate = "",
                    ReasonNumber = "",
                    Recepient = waybill.Recipient.GetFullInfo(),
                    RecepientOKPO = recepientJuridicalPerson != null ? recepientJuridicalPerson.OKPO : "",
                    Sender = waybill.Deal.Contract.ContractorOrganization.GetFullInfo(),
                    SenderOKPO = organizationJuridicalPerson != null ? organizationJuridicalPerson.OKPO : "",
                    ShippingWaybillDate = "",
                    ShippingWaybillNumber = "",
                    Number = waybill.Number
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
                var waybill = returnFromClientWaybillService.CheckWaybillExistence(settings.WaybillId, user);

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
                            PurchaseCost = x.Key.ReceiptWaybillRow.PurchaseCost,
                            SalePrice = x.First().SalePrice,
                            ReturnCount = x.Sum(y => y.ReturnCount),
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
                            SalePrice = x.First().SalePrice,
                            ReturnCount = x.Sum(y => y.ReturnCount),
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
                        case PrintingFormPriceType.SalePrice:
                            price = row.SalePrice ?? 0;
                            valueAddedTaxSum = row.ValueAddedTaxSum;
                            break;
                        case PrintingFormPriceType.PurchaseCost:
                            price = row.PurchaseCost;
                            valueAddedTaxSum = row.PurchaseCostValueAddedTaxSum;
                            break;
                    }

                    priceSum = Math.Round(price * row.ReturnCount, (priceType == PrintingFormPriceType.PurchaseCost ? 6 : 2));

                    var formItem = new TORG12PrintingFormItemViewModel(price, priceSum, row.ReturnCount, valueAddedTaxSum, row.ValueAddedTax.Value)
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
        private void CheckPriceTypePermissions(User user, ReturnFromClientWaybill waybill, PrintingFormPriceType priceType)
        {
            switch (priceType)
            {
                case PrintingFormPriceType.SalePrice:
                    returnFromClientWaybillService.CheckPossibilityToPrintFormInSalePrices(waybill, user);
                    break;
                case PrintingFormPriceType.PurchaseCost:
                    returnFromClientWaybillService.CheckPossibilityToPrintFormInPurchaseCosts(waybill, user);
                    break;
                default:
                    throw new Exception("Неверное значение типа цен, в которых печается отчет.");
            }
        }

        #endregion

        #region Выбор накладной

        public ReturnFromClientWaybillSelectViewModel SelectWaybill(int articleId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new ReturnFromClientWaybillSelectViewModel();

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
            model.AddColumn("IsAccepted", "Пр.", Unit.Pixel(22), GridCellStyle.Link, align: GridColumnAlign.Center);
            model.AddColumn("Number", "Номер", Unit.Pixel(60), GridCellStyle.Link);
            model.AddColumn("Date", "Дата", Unit.Pixel(55));
            model.AddColumn("SalePriceSum", "Сумма в ОЦ", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("AccountingPriceSum", "Сумма в УЦ приемщика", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("ClientName", "Клиент", Unit.Percentage(34));
            model.AddColumn("RecipientStorageName", "Место хранения-приемщик", Unit.Percentage(33));
            model.AddColumn("RecipientName", "Организация-приемщик", Unit.Percentage(33));
            model.AddColumn("ShippingPercent", "Отгрузка", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Name", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveFilter = new ParameterString(state.Filter);

            var deriveParameters = new ParameterString(state.Parameters);

            if (deriveParameters.Keys.Contains("Article"))
            {
                var articleId = deriveParameters["Article"].Value as string;
                deriveFilter.Add("Article", ParameterStringItem.OperationType.Eq, articleId);
            }

            var rows = returnFromClientWaybillService.GetFilteredList(state, user, deriveFilter);

            GridActionCell actions = new GridActionCell("Action");
            actions.AddAction("Выбрать", "returnfromclient_waybill_select_link");

            foreach (var row in rows)
            {
                decimal accountingPriceSum = returnFromClientWaybillIndicatorService.CalculateAccountingPriceSum(row);

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("IsAccepted") { Value = row.IsAccepted ? "П" : "" },
                    new GridLabelCell("Number") { Value = StringUtils.PadLeftZeroes(row.Number, 8) },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("SalePriceSum") { Value = row.SalePriceSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("AccountingPriceSum") { Value = accountingPriceSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("ClientName") { Value = row.Client.Name },
                    new GridLabelCell("RecipientStorageName") { Value = row.RecipientStorage.Name },
                    new GridLabelCell("RecipientName") { Value = row.Recipient.ShortName },
                    new GridLabelCell("ShippingPercent") { Value = returnFromClientWaybillIndicatorService.CalculateShippingPercent(row).ForDisplay(ValueDisplayType.Percent) + " %" },
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