using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
using ERP.Wholesale.UI.ViewModels.Common;
using ERP.Wholesale.UI.ViewModels.ExpenditureWaybill;
using ERP.Wholesale.UI.ViewModels.OutgoingWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.Т1;
using ERP.Wholesale.UI.ViewModels.PrintingForm.ExpenditureWaybill;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ExpenditureWaybillPresenter : OutgoingWaybillPresenter<ExpenditureWaybill>, IExpenditureWaybillPresenter
    {
        #region Поля

        private readonly IExpenditureWaybillService expenditureWaybillService;
        private readonly IDealService dealService;
        private readonly IDealQuotaService dealQuotaService;
        private readonly IReceiptWaybillService receiptWaybillService;
        private readonly IValueAddedTaxService valueAddedTaxService;
        private readonly IClientService clientService;
        private readonly IClientContractService clientContractService;
        private readonly ITeamService teamService;
        private readonly IClientOrganizationService clientOrganizationService;
        private readonly IOutgoingWaybillRowService outgoingWaybillRowService;

        #endregion

        #region Конструкторы

        public ExpenditureWaybillPresenter(IUnitOfWorkFactory unitOfWorkFactory, IExpenditureWaybillService expenditureWaybillService, IStorageService storageService,
            IDealService dealService, IReceiptWaybillService receiptWaybillService, IClientContractService clientContractService,
            IValueAddedTaxService valueAddedTaxService, IUserService userService, IClientService clientService, ITeamService teamService,
            IDealQuotaService dealQuotaService, IClientOrganizationService clientOrganizationService, IArticleService articleService, IArticlePriceService articlePriceService,
            IArticleAvailabilityService articleAvailabilityService, IAccountOrganizationService accountOrganizationService, IOutgoingWaybillRowService outgoingWaybillRowService) :
            base(unitOfWorkFactory, expenditureWaybillService, userService, storageService, accountOrganizationService,
            articleService, articlePriceService, articleAvailabilityService)
        {
            this.expenditureWaybillService = expenditureWaybillService;
            this.dealService = dealService;
            this.dealQuotaService = dealQuotaService;
            this.receiptWaybillService = receiptWaybillService;
            this.valueAddedTaxService = valueAddedTaxService;
            this.clientContractService = clientContractService;
            this.clientService = clientService;
            this.teamService = teamService;
            this.clientOrganizationService = clientOrganizationService;
            this.outgoingWaybillRowService = outgoingWaybillRowService;
        }
        #endregion

        #region Методы

        #region Список

        public ExpenditureWaybillListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ExpenditureWaybill_List_Details);

                var model = new ExpenditureWaybillListViewModel();
                model.NewAndAcceptedGrid = GetNewAndAcceptedExpenditureWaybillGridLocal(new GridState() { Sort = "Date=Desc;CreationDate=Desc" }, user);
                model.ShippedGrid = GetShippedExpenditureWaybillGridLocal(new GridState() { Sort = "Date=Desc;CreationDate=Desc" }, user);

                // получаем список доступных мест хранения на основании прав
                var storageList = storageService.GetList(user, Permission.Storage_List_Details);

                model.FilterData.Items.Add(new FilterDateRangePicker("Date", "Дата накладной"));
                model.FilterData.Items.Add(new FilterTextEditor("Number", "№ накладной"));
                model.FilterData.Items.Add(new FilterComboBox("SenderStorage", "Место хранения",
                    storageList.OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name).GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false)));
                model.FilterData.Items.Add(new FilterTextEditor("Deal_Name", "Сделка"));
                model.FilterData.Items.Add(new FilterTextEditor("Deal_Client_Name", "Клиент"));
                model.FilterData.Items.Add(new FilterTextEditor("Curator_DisplayName", "Куратор"));

                return model;
            }
        }

        private GridData GetExpenditureWaybillGrid(GridState state, bool showIsAcceptedColumn, ParameterString ps, User user)
        {
            GridData model = new GridData();

            model.ButtonPermissions["AllowToCreateExpenditureWaybill"] = user.HasPermission(Permission.ExpenditureWaybill_Create_Edit);

            if (showIsAcceptedColumn)
            {
                model.AddColumn("IsAccepted", "Пр.", Unit.Pixel(22), GridCellStyle.Link, align: GridColumnAlign.Center);
            }

            model.AddColumn("Number", "Номер", Unit.Pixel(60), GridCellStyle.Link);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Date", "Дата", Unit.Pixel(60));
            model.AddColumn("SalePriceSum", "Итоговая сумма", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("ClientName", "Клиент", Unit.Percentage(30));
            model.AddColumn("ClientId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("DealName", "Сделка", Unit.Percentage(25));
            model.AddColumn("DealId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("StorageName", "Место хранения", Unit.Percentage(25));
            model.AddColumn("StorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("CuratorName", "Куратор", Unit.Percentage(20));
            model.AddColumn("CuratorId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var rows = expenditureWaybillService.GetFilteredList(state, user, ps);

            foreach (var row in rows)
            {
                GridRowStyle rowStyle;

                switch (row.State)
                {
                    case ExpenditureWaybillState.ConflictsInArticle:
                        rowStyle = GridRowStyle.Error; break;

                    case ExpenditureWaybillState.ArticlePending:
                        rowStyle = GridRowStyle.Warning; break;

                    default:
                        rowStyle = GridRowStyle.Normal; break;
                }

                var salePriceSum = expenditureWaybillService.CalculateSalePriceSum(row);
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
                    new GridHiddenCell("Id") { Value = row.Id.ToString(), Key = "Id" },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("SalePriceSum") { Value = salePriceSum.ForDisplay(ValueDisplayType.Money) },
                    user.HasPermission(Permission.Client_List_Details) ?
                        (GridCell)new GridLinkCell("ClientName") { Value = row.Deal.Client.Name } :
                        new GridLabelCell("ClientName") { Value = row.Deal.Client.Name },
                    new GridHiddenCell("ClientId") { Value = row.Deal.Client.Id.ToString(), Key = "ClientId" },
                    dealService.IsPossibilityToViewDetails(row.Deal, user) ?
                        (GridCell)new GridLinkCell("DealName") { Value = row.Deal.Name } :
                        new GridLabelCell("DealName") { Value = row.Deal.Name },
                    new GridHiddenCell("DealId") { Value = row.Deal.Id.ToString(), Key = "DealId" },
                    storageService.IsPossibilityToViewDetails(row.SenderStorage, user) ?
                        (GridCell)new GridLinkCell("StorageName") { Value = row.SenderStorage.Name } :
                        new GridLabelCell("StorageName") { Value = row.SenderStorage.Name },
                    new GridHiddenCell("StorageId") { Value = row.SenderStorage.Id.ToString(), Key = "StorageId" },
                    userService.IsPossibilityToViewDetails(row.Curator, user) ?
                        (GridCell)new GridLinkCell("CuratorName") { Value = row.Curator.DisplayName } :
                        new GridLabelCell("CuratorName") { Value = row.Curator.DisplayName },
                    new GridHiddenCell("CuratorId") { Value = row.Curator.Id.ToString() }
                ) { Style = rowStyle });
            }

            model.State = state;

            return model;
        }

        public GridData GetNewAndAcceptedExpenditureWaybillGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetNewAndAcceptedExpenditureWaybillGridLocal(state, user);
            }
        }

        private GridData GetNewAndAcceptedExpenditureWaybillGridLocal(GridState state, User user)
        {
            var ps = new ParameterString("");
            ps.Add("State", ParameterStringItem.OperationType.OneOf);
            ps["State"].Value = new List<string>()
                {
                    ExpenditureWaybillState.Draft.ValueToString(),
                    ExpenditureWaybillState.ReadyToAccept.ValueToString(),
                    ExpenditureWaybillState.ReadyToShip.ValueToString(),
                    ExpenditureWaybillState.ArticlePending.ValueToString(),
                    ExpenditureWaybillState.ConflictsInArticle.ValueToString()
                };

            return GetExpenditureWaybillGrid(state, true, ps, user);
        }

        public GridData GetShippedExpenditureWaybillGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetShippedExpenditureWaybillGridLocal(state, user);
            }
        }

        private GridData GetShippedExpenditureWaybillGridLocal(GridState state, User user)
        {
            var ps = new ParameterString("");
            ps.Add("State", ParameterStringItem.OperationType.Eq, ((byte)ExpenditureWaybillState.ShippedBySender).ToString());

            return GetExpenditureWaybillGrid(state, false, ps, user);
        }

        #endregion

        #region Добавление / редактирование / сохранение

        public void CheckPosibilityToCreate(int dealId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);

                expenditureWaybillService.CheckPossibilityToCreate(deal, user);
            }
        }

        public ExpenditureWaybillEditViewModel Create(string backURL, int? dealId, int? clientId, int? clientOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new ExpenditureWaybillEditViewModel()
                {
                    Title = "Добавление накладной реализации товаров",
                    BackURL = backURL,
                    Date = DateTime.Today.ToShortDateString(),
                    Number = "",
                    IsAutoNumber = "1",
                    TeamId = "",
                    AllowToEdit = true,
                    AllowToEditTeam = true,
                    AllowToGenerateNumber = true,
                    AllowToChangeDate = user.HasPermission(Permission.ExpenditureWaybill_Date_Change)
                };

                Deal deal = null;
                if (dealId != null)
                {
                    deal = dealService.CheckDealExistence(dealId.Value, user);
                    expenditureWaybillService.CheckPossibilityToCreate(deal, user);
                    model.ClientDeliveryAddress = deal.Client.FactualAddress;
                    model.OrganizationDeliveryAddress = deal.Contract.ContractorOrganization.Address;
                    model.DealContractCashPaymentSum =  clientContractService.CalculateDealContractCashPaymentSum(deal.Contract).ForDisplay();
                    // установка значений, связанных с командами, в которые можно добавить накладную
                    var teamList = dealService.GetTeamListForDealDocumentByDeal(deal, user);
                    model.TeamList = ComboBoxBuilder.GetComboBoxItemList(teamList, x => x.Name, x => x.Id.ToString(), teamList.Count() != 1);
                }
                else
                {
                    model.TeamList = ComboBoxBuilder.GetComboBoxItemList(new List<Team>(), x => x.Name, x => x.Id.ToString(), false);
                }

                Client client = null;
                if (clientId != null)
                {
                    client = clientService.CheckClientExistence(clientId.Value, user);
                }

                ClientOrganization clientOrganization = null;
                if (clientOrganizationId != null)
                {
                    clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(clientOrganizationId.Value, user);
                }

                // Провереям правильность входных данных
                if (deal != null && client != null)
                {
                    ValidationUtils.Assert(deal.Client == client, String.Format("Сделка «{0}» не принадлежит клиенту «{1}».", deal.Name, client.Name));
                }

                if (client == null && deal != null)
                {
                    client = deal.Client;
                }

                var singleQuota = (deal != null && deal.Quotas.Count() == 1) ? deal.Quotas.First() : null;

                model.CuratorId = user.Id.ToString();
                model.CuratorName = user.DisplayName;
                model.AllowToChangeCurator = user.HasPermission(Permission.ExpenditureWaybill_Curator_Change);
                model.DealId = deal != null ? deal.Id.ToString() : "";
                model.DealName = deal != null ? deal.Name : "Выберите сделку";
                model.DealQuotaId = deal != null && singleQuota != null ? singleQuota.Id.ToString() : "";
                model.DealQuotaName = (deal != null && singleQuota != null) ? singleQuota.FullName : "Выберите квоту";
                model.ClientId = client != null ? client.Id.ToString() : "";
                model.ClientName = client != null ? client.Name : "Выберите клиента";
                model.ClientOrganizationId = clientOrganization != null ? clientOrganization.Id.ToString() : "";
                model.ClientOrganizationName = clientOrganization != null ? clientOrganization.ShortName : "Выберите организацию клиента";
                model.IsPrepayment = (byte)(deal != null && singleQuota != null ? (singleQuota.IsPrepayment ? 1 : 0) : 1);
                model.AllowToChangePaymentType = deal != null && singleQuota != null ? !singleQuota.IsPrepayment : false;
                model.RoundSalePrice = "1";
                model.StorageList = deal != null ? GetStorageSelectListItems(deal, user) : new List<SelectListItem>();
                model.DeliveryAddressTypeList = ComboBoxBuilder.GetComboBoxItemList<DeliveryAddressType>();

                var valueAddedTaxList = valueAddedTaxService.GetList();
                var defaultValue = valueAddedTaxList.Where(x => x.IsDefault == true).FirstOrDefault();
                model.ValueAddedTaxList = valueAddedTaxList.GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true);
                model.ValueAddedTaxId = (defaultValue != null) ? defaultValue.Id : (short)0;

                return model;
            }
        }

        public ExpenditureWaybillEditViewModel Edit(Guid id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(id, user);
                var valueAddedTaxList = valueAddedTaxService.GetList();

                expenditureWaybillService.CheckPossibilityToEdit(waybill, user);

                var teamList = dealService.GetTeamListForDealDocumentByDeal(waybill.Deal, user);

                var model = new ExpenditureWaybillEditViewModel()
                {
                    AllowToChangePaymentType = !waybill.Quota.IsPrepayment,
                    BackURL = backURL,
                    ClientId = waybill.Deal.Client.Id.ToString(),
                    ClientName = waybill.Deal.Client.Name,
                    Comment = waybill.Comment,
                    CuratorId = waybill.Curator.Id.ToString(),
                    CuratorName = waybill.Curator.DisplayName,
                    AllowToChangeCurator = expenditureWaybillService.IsPossibilityToChangeCurator(waybill, user),
                    Date = waybill.Date.ToShortDateString(),
                    DealId = waybill.Deal.Id.ToString(),
                    DealName = waybill.Deal.Name,
                    DealQuotaId = waybill.Quota.Id.ToString(),
                    DealQuotaName = waybill.Quota.FullName,
                    Id = waybill.Id,
                    IsPrepayment = (byte)(waybill.IsPrepayment ? 1 : 0),
                    Name = waybill.Name,
                    Number = waybill.Number,
                    IsAutoNumber = "0",
                    AllowToGenerateNumber = false,
                    SenderStorageId = waybill.SenderStorage.Id,
                    StorageList = GetStorageSelectListItems(waybill.Deal, user),
                    Title = "Редактирование накладной реализации товаров",
                    ValueAddedTaxList = valueAddedTaxList.GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true),
                    ValueAddedTaxId = waybill.ValueAddedTax.Id,
                    RoundSalePrice = waybill.RoundSalePrice ? "1" : "0",
                    DeliveryAddressTypeList = ComboBoxBuilder.GetComboBoxItemList<DeliveryAddressType>(),
                    ClientDeliveryAddress = waybill.Deal.Client.FactualAddress,
                    OrganizationDeliveryAddress = waybill.Deal.Contract.ContractorOrganization.Address,
                    CustomDeliveryAddress = waybill.DeliveryAddressType == DeliveryAddressType.CustomAddress ? waybill.DeliveryAddress : "",
                    DeliveryAddressTypeId = waybill.DeliveryAddressType.ValueToString(),
                    AllowToEdit = false,
                    TeamId = waybill.Team.Id.ToString(),
                    TeamList = ComboBoxBuilder.GetComboBoxItemList(teamList, x => x.Name, x => x.Id.ToString(), teamList.Count() != 1),
                    AllowToEditTeam = expenditureWaybillService.IsPossibilityToEditTeam(waybill, user),
                    AllowToChangeDate = expenditureWaybillService.IsPossibilityToChangeDate(waybill, user)
                };

                return model;
            }
        }

        public Guid Save(ExpenditureWaybillEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var senderStorage = storageService.CheckStorageExistence(model.SenderStorageId, user,
                    "Место хранения отправителя не найдено. Возможно, оно было удалено.");

                var dealId = ValidationUtils.TryGetInt(model.DealId);
                var deal = dealService.CheckDealExistence(dealId, user);
                int curatorId = ValidationUtils.TryGetInt(model.CuratorId);
                var date = ValidationUtils.TryGetDate(model.Date);

                ValidationUtils.Assert(date.Date <= currentDateTime.Date, "Дата накладной не может быть больше текущей даты.");

                var currentDealQuota = dealQuotaService.CheckDealQuotaExistence(ValidationUtils.TryGetInt(model.DealQuotaId), user);
                ValidationUtils.NotNull(currentDealQuota, "Квота не найдена. Возможно, она была удалена.");

                var deliveryAddressType = ValidationUtils.TryGetEnum<DeliveryAddressType>(model.DeliveryAddressTypeId);
                if (deliveryAddressType == DeliveryAddressType.CustomAddress)
                {
                    ValidationUtils.Assert(!String.IsNullOrEmpty(model.CustomDeliveryAddress), "Необходимо указать адрес доставки.");
                }

                if (model.IsPrepayment != 0 && model.IsPrepayment != 1)
                {
                    throw new Exception("Укажите форму взаиморасчетов.");
                }
                bool isPrepayment = (model.IsPrepayment == 1);

                var teamId = ValidationUtils.TryGetShort(model.TeamId, "Необходимо указать команду.");
                Team team = teamService.CheckTeamExistence(teamId);

                ExpenditureWaybill waybill = null;
                var deliveryAddress = "";

                switch (deliveryAddressType)
                {
                    case DeliveryAddressType.CustomAddress:
                        deliveryAddress = model.CustomDeliveryAddress;
                        break;
                    case DeliveryAddressType.ClientAddress:
                        deliveryAddress = deal.Client.FactualAddress;
                        break;
                    case DeliveryAddressType.OrganizationAddress:
                        deliveryAddress = deal.Contract.ContractorOrganization.Address;
                        break;
                }

                bool isAutoNumber = ValidationUtils.TryGetBool(model.IsAutoNumber);

                // добавление
                if (model.Id == Guid.Empty)
                {
                    expenditureWaybillService.CheckPossibilityToCreate(deal, user);

                    var availableTeamList = dealService.GetTeamListForDealDocumentByDeal(deal, user);
                    ValidationUtils.Assert(availableTeamList.Contains(team), "Невозможно добавить накладную в данную команду.");

                    if (isAutoNumber)
                    {
                        model.Number = "";
                    }

                    var curator = userService.CheckUserExistence(curatorId, user, "Куратор не найден. Возможно, он был удален.");

                    waybill = new ExpenditureWaybill(model.Number, date, senderStorage, deal, team, currentDealQuota,
                        isPrepayment, curator, deliveryAddressType, deliveryAddress, currentDateTime, user);

                    // если куратор не совпадает с пользователем, то ...
                    if (user != curator)
                    {
                        user.CheckPermission(Permission.ExpenditureWaybill_Curator_Change); // ... проверяем права на смену куратора и ...
                        expenditureWaybillService.CheckPossibilityToViewDetailsByUser(waybill, curator);    // ... видимость накладной куратору
                    }
                }
                // редактирование
                else
                {
                    ValidationUtils.Assert(!String.IsNullOrWhiteSpace(model.Number), "Укажите номер накладной.");

                    waybill = expenditureWaybillService.CheckWaybillExistence(model.Id, user);
                    expenditureWaybillService.CheckPossibilityToEdit(waybill, user);

                    if (team != waybill.Team)
                    {
                        expenditureWaybillService.CheckPossibilityToEditTeam(waybill, user);

                        var availableTeamList = dealService.GetTeamListForDealDocumentByDeal(deal, waybill.Curator);
                        ValidationUtils.Assert(availableTeamList.Contains(team), "Невозможно добавить накладную в данную команду.");

                        waybill.Team = team;
                    }

                    if (date != waybill.Date)
                    {
                        expenditureWaybillService.CheckPossibilityToChangeDate(waybill, user);
                        waybill.Date = date;
                    }

                    expenditureWaybillService.SetDealQuota(waybill, currentDealQuota, user);
                    waybill.IsPrepayment = isPrepayment;

                    waybill.Number = model.Number;
                    waybill.DeliveryAddress = deliveryAddress;
                    waybill.DeliveryAddressType = deliveryAddressType;


                    // Если куратор изменен, то проверяем права
                    if (waybill.Curator.Id != curatorId)
                    {
                        var curator = userService.CheckUserExistence(curatorId, user, "Куратор не найден. Возможно, он был удален.");

                        expenditureWaybillService.CheckPossibilityToChangeCurator(waybill, user);
                        expenditureWaybillService.CheckPossibilityToViewDetailsByUser(waybill, curator);

                        // Права есть, меняем куратора.
                        waybill.Curator = curator;
                    }


                }
                waybill.RoundSalePrice = model.RoundSalePrice == "1" ? true : false;
                waybill.Comment = StringUtils.ToHtml(model.Comment);

                var valueAddedTax = valueAddedTaxService.CheckExistence(model.ValueAddedTaxId);
                waybill.ValueAddedTax = valueAddedTax;

                expenditureWaybillService.Save(waybill);

                uow.Commit();

                return waybill.Id;
            }
        }

        #endregion

        #region Работа с квотами

        /// <summary>
        /// Назначить накладной реализации товаров квоту.
        /// </summary>
        /// <param name="expenditureWaybillId">Идентификатор накладной реализации товаров.</param>
        /// <param name="dealQuotaId">Идентификатор квоты по сделке.</param>
        /// <param name="currentUser">Пользователь, выполняющий операцию.</param>  
        public object SetDealQuota(Guid expenditureWaybillId, int dealQuotaId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var dealQuota = dealQuotaService.CheckDealQuotaExistence(dealQuotaId, user);
                var waybill = expenditureWaybillService.CheckWaybillExistence(expenditureWaybillId, user);

                expenditureWaybillService.SetDealQuota(waybill, dealQuota, user);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        #endregion

        #region Удаление

        public void Delete(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(id, user);

                expenditureWaybillService.Delete(waybill, user);

                uow.Commit();
            }
        }
        #endregion

        #region Детали

        public ExpenditureWaybillDetailsViewModel Details(Guid id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(id, user);

                var model = new ExpenditureWaybillDetailsViewModel()
                {
                    Id = waybill.Id,
                    Name = waybill.Name,
                    MainDetails = GetMainDetails(waybill, user),
                    RowGrid = GetExpenditureWaybillRowGridLocal(new GridState() { Parameters = "ExpenditureWaybillId=" + waybill.Id, Sort = "CreationDate=Asc" }, user),
                    ArticleGroupGridState = new GridState() { Parameters = "ExpenditureWaybillId=" + waybill.Id },
                    BackURL = backURL,
                    DocumentGrid = GetDocumentGridLocal(new GridState()),

                    AllowToEdit = expenditureWaybillService.IsPossibilityToEdit(waybill, user),
                    AllowToDelete = expenditureWaybillService.IsPossibilityToDelete(waybill, user),
                    AllowToPrepareToAccept = expenditureWaybillService.IsPossibilityToPrepareToAccept(waybill, user),
                    IsPossibilityToPrepareToAccept = expenditureWaybillService.IsPossibilityToPrepareToAccept(waybill, user, true),
                    AllowToCancelReadinessToAccept = expenditureWaybillService.IsPossibilityToCancelReadinessToAccept(waybill, user),
                    AllowToAccept = expenditureWaybillService.IsPossibilityToAccept(waybill, user),
                    AllowToAcceptRetroactively = expenditureWaybillService.IsPossibilityToAcceptRetroactively(waybill, user),
                    IsPossibilityToAccept = expenditureWaybillService.IsPossibilityToAccept(waybill, user, true),
                    IsPossibilityToAcceptRetroactively = expenditureWaybillService.IsPossibilityToAcceptRetroactively(waybill, user, true),
                    AllowToCancelAcceptance = expenditureWaybillService.IsPossibilityToCancelAcceptance(waybill, user),
                    AllowToShip = expenditureWaybillService.IsPossibilityToShip(waybill, user),
                    AllowToShipRetroactively = expenditureWaybillService.IsPossibilityToShipRetroactively(waybill, user),
                    IsPossibilityToShip = expenditureWaybillService.IsPossibilityToShip(waybill, user, true),
                    IsPossibilityToShipRetroactively = expenditureWaybillService.IsPossibilityToShipRetroactively(waybill, user, true),
                    AllowToCancelShipping = expenditureWaybillService.IsPossibilityToCancelShipping(waybill, user),
                    AllowToPrintForms = expenditureWaybillService.IsPossibilityToPrintForms(waybill, user)
                };

                return model;
            }
        }

        /// <summary>
        /// Получение главных деталей накладной реализации товаров
        /// </summary>
        private ExpenditureWaybillMainDetailsViewModel GetMainDetails(ExpenditureWaybill waybill, User user)
        {
            var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);
            var allowToViewReturnsFromClient = expenditureWaybillService.IsPossibilityToViewReturnsFromClient(waybill, user);
            var allowToViewPayments = expenditureWaybillService.IsPossibilityToViewPayments(waybill, user);
            var allowToViewAccountingPrices = user.HasPermissionToViewStorageAccountingPrices(waybill.SenderStorage);

            // если права на просмотр определенной информации нет - то ее не вычисляем
            var ind = expenditureWaybillService.GetMainIndicators(waybill, calculateSenderAccountingPriceSum: allowToViewAccountingPrices,
                calculateSalePriceSum: true, calculatePaymentSum: allowToViewPayments, calculatePaymentPercent: allowToViewPayments, calculateVatInfoList: true,
                calculateProfit: true, calculateTotalDiscount: allowToViewAccountingPrices, calculateLostProfit: allowToViewReturnsFromClient,
                calculateTotalReservedByReturnSum: allowToViewReturnsFromClient, calculateTotalReturnedSum: allowToViewReturnsFromClient);

            var model = new ExpenditureWaybillMainDetailsViewModel()
            {
                AcceptedById = waybill.AcceptedBy != null ? waybill.AcceptedBy.Id.ToString() : "",
                AcceptedByName = waybill.AcceptedBy != null ? waybill.AcceptedBy.DisplayName : "",
                AcceptanceDate = waybill.AcceptedBy != null ? String.Format("({0})", waybill.AcceptanceDate.Value.ToShortDateTimeString()) : "",
                StateName = waybill.State.GetDisplayName(),
                PaymentType = waybill.IsPrepayment ? "Предоплата" : "Отсрочка платежа",
                AccountOrganizationId = waybill.Sender.Id.ToString(),
                AccountOrganizationName = waybill.Sender.ShortName,
                ClientName = waybill.Deal.Client.Name,
                ClientId = waybill.Deal.Client.Id.ToString(),
                DealName = waybill.Deal.Name,
                DealId = waybill.Deal.Id.ToString(),
                DealQuotaName = waybill.Quota.FullName,
                SenderStorageName = waybill.SenderStorage.Name,
                SenderStorageId = waybill.SenderStorage.Id.ToString(),
                CuratorId = waybill.Curator.Id.ToString(),
                CuratorName = waybill.Curator.DisplayName,
                CreatedById = waybill.CreatedBy.Id.ToString(),
                CreatedByName = waybill.CreatedBy.DisplayName,
                CreationDate = String.Format("({0})", waybill.CreationDate.ToShortDateTimeString()),
                TeamId = waybill.Team.Id.ToString(),
                TeamName = waybill.Team.Name,
                ShippedById = waybill.ShippedBy != null ? waybill.ShippedBy.Id.ToString() : "",
                ShippedByName = waybill.ShippedBy != null ? waybill.ShippedBy.DisplayName : "",
                ShippingDate = waybill.ShippedBy != null ? String.Format("({0})", waybill.ShippingDate.Value.ToShortDateTimeString()) : "",
                PurchaseCostSum = allowToViewPurchaseCost ? waybill.PurchaseCostSum.ForDisplay(ValueDisplayType.Money) : "---",
                SenderAccountingPriceSum = allowToViewAccountingPrices ? ind.SenderAccountingPriceSum.ForDisplay(ValueDisplayType.Money) : "---",
                SalePriceSum = ind.SalePriceSum.ForDisplay(ValueDisplayType.Money),
                TotalDiscountPercent = allowToViewAccountingPrices ? ind.TotalDiscountPercent.ForDisplay(ValueDisplayType.Percent) : "---",
                TotalDiscountSum = allowToViewAccountingPrices ? ind.TotalDiscountSum.ForDisplay(ValueDisplayType.Money) : "---",
                MarkupPercent = allowToViewPurchaseCost ? ind.ProfitPercent.ForDisplay(ValueDisplayType.Percent) : "---",
                MarkupSum = allowToViewPurchaseCost ? ind.ProfitSum.ForDisplay(ValueDisplayType.Money) : "---",
                PaymentPercent = (allowToViewPayments ? ind.PaymentPercent.ForDisplay(ValueDisplayType.Percent) : "---"),
                PaymentSum = (allowToViewPayments ? ind.PaymentSum.ForDisplay(ValueDisplayType.Money) : "---"),
                TotalWeight = waybill.Weight.ForDisplay(ValueDisplayType.Weight),
                TotalVolume = waybill.Volume.ForDisplay(ValueDisplayType.Volume),

                RowCount = waybill.RowCount.ForDisplay(),
                ValueAddedTaxString = VatUtils.GetValueAddedTaxString(ind.VatInfoList, waybill.ValueAddedTax.Value),
                Comment = waybill.Comment,

                TotalReturnedSum = (allowToViewReturnsFromClient ? ind.TotalReturnedSum.ForDisplay(ValueDisplayType.Money) : "---"),
                TotalReservedByReturnSum = (allowToViewReturnsFromClient ? ind.TotalReservedByReturnSum.ForDisplay(ValueDisplayType.Money) : "---"),
                ReturnLostProfitSum = (allowToViewReturnsFromClient && allowToViewPurchaseCost ? ind.ReturnLostProfitSum.ForDisplay(ValueDisplayType.Money) : "---"),
                ReservedByReturnLostProfitSum = (allowToViewReturnsFromClient && allowToViewPurchaseCost ? ind.ReservedByReturnLostProfitSum.ForDisplay(ValueDisplayType.Money) : "---"),
                DeliveryAddress = waybill.DeliveryAddress,

                AllowToViewCuratorDetails = userService.IsPossibilityToViewDetails(waybill.Curator, user),
                AllowToViewClientDetails = user.HasPermission(Permission.Client_List_Details),
                AllowToViewDealDetails = dealService.IsPossibilityToViewDetails(waybill.Deal, user),
                AllowToViewSenderStorageDetails = storageService.IsPossibilityToViewDetails(waybill.SenderStorage, user),
                AllowToChangeDealQuota = expenditureWaybillService.IsPossibilityToEdit(waybill, user),
                AllowToViewTeamDetails = teamService.IsPossibilityToViewDetails(waybill.Team, user),
                AllowToChangeCurator = expenditureWaybillService.IsPossibilityToChangeCurator(waybill, user),
                AllowToViewCreatedByDetails = userService.IsPossibilityToViewDetails(waybill.CreatedBy, user),
                AllowToViewAcceptedByDetails = userService.IsPossibilityToViewDetails(waybill.AcceptedBy, user),
                AllowToViewShippedByDetails = userService.IsPossibilityToViewDetails(waybill.ShippedBy, user)
            };

            return model;
        }

        #endregion

        #region Позиции накладной

        public GridData GetExpenditureWaybillRowGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetExpenditureWaybillRowGridLocal(state, user);
            }
        }

        private GridData GetExpenditureWaybillRowGridLocal(GridState state, User user)
        {
            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(120));
            model.AddColumn("Batch", "Партия", Unit.Pixel(120));
            model.AddColumn("ArticleId", "Код товара", Unit.Pixel(40), align: GridColumnAlign.Right); // переделать на 35 пикселей, когда будет называться просто "Код"
            model.AddColumn("ArticleNumber", "Артикул", Unit.Pixel(60));
            model.AddColumn("ArticleName", "Наименование товара", Unit.Percentage(100));
            model.AddColumn("Weight", "Вес (кг)", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Volume", "Объем (м3)", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("SalePrice", "Отпускная цена", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("ValueAddedTax", "Ставка НДС", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("ValueAddedTaxSum", "Сумма НДС", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("Sum", "Сумма", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("PackCount", "Кол-во ЕУ", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Count", "Кол-во", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = expenditureWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid(deriveParams["ExpenditureWaybillId"].Value as string), user);

            var rowStyles = GetRowsStyle(expenditureWaybillService.GetRowStates(waybill));

            var allowToEdit = expenditureWaybillService.IsPossibilityToEdit(waybill, user);
            model.ButtonPermissions["AllowToCreateRow"] = allowToEdit;

            var indicators = expenditureWaybillService.GetMainRowsIndicators(waybill, user, calculateSalePrice: true, calculateValueAddedTaxSum: true);

            foreach (var item in waybill.Rows.OrderBy(x => x.CreationDate))
            {
                var ind = indicators[item.Id];

                ExpenditureWaybillRow row = item.As<ExpenditureWaybillRow>();

                var actions = new GridActionCell("Action");
                if (allowToEdit)
                {
                    actions.AddAction("Ред.", "edit_link");
                }
                else
                {
                    actions.AddAction("Дет.", "details_link");
                }

                if (expenditureWaybillService.IsPossibilityToDeleteRow(row, user))
                {
                    actions.AddAction("Удал.", "delete_link");
                }

                if (row.AreSourcesDetermined)
                {
                    actions.AddAction("Источ.", "source_link");
                }

                var receiptWaybillRow = row.ReceiptWaybillRow;
                var article = row.Article;

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Batch") { Value = receiptWaybillRow.BatchName },
                    new GridLabelCell("ArticleId") { Value = article.Id.ToString() },
                    new GridLabelCell("ArticleNumber") { Value = article.Number },
                    new GridLabelCell("ArticleName") { Value = article.FullName },
                    new GridLabelCell("Weight") { Value = row.Weight.ForDisplay(ValueDisplayType.Weight) },
                    new GridLabelCell("Volume") { Value = row.Volume.ForDisplay(ValueDisplayType.Volume) },
                    new GridLabelCell("SalePrice") { Value = ind.SalePrice.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("ValueAddedTax") { Value = row.ValueAddedTax.Name },
                    new GridLabelCell("ValueAddedTaxSum") { Value = ind.ValueAddedTaxSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("Sum") { Value = (ind.SalePrice != null ? Math.Round(ind.SalePrice.Value * row.SellingCount, 2).ForDisplay(ValueDisplayType.Money) : "---") },
                    new GridLabelCell("PackCount") { Value = row.PackCount.ForDisplay(ValueDisplayType.PackCount) },
                    new GridLabelCell("Count") { Value = row.SellingCount.ForDisplay() },
                    new GridHiddenCell("Id") { Value = row.Id.ToString(), Key = "expenditureWaybillRowId" }
               ) { Style = rowStyles[row.Id] });
            }
            model.State = state;

            return model;
        }

        /// <summary>
        /// Получить грид с группами товаров
        /// </summary>
        public GridData GetExpenditureWaybillArticleGroupGrid(GridState state, UserInfo currentUser)
        {

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetExpenditureWaybillArticleGroupGridLocal(state, user);
            }

        }

        private GridData GetExpenditureWaybillArticleGroupGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            var deriveParams = new ParameterString(state.Parameters);
            var waybill = expenditureWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid(deriveParams["ExpenditureWaybillId"].Value.ToString()), user);

            var articleGroups = waybill.Rows.GroupBy(x => x.Article.ArticleGroup);

            var rows = new List<BaseWaybillArticleGroupRow>();

            // получение основных индикаторов для списка позиций
            var indicators = expenditureWaybillService.GetMainRowsIndicators(waybill, user, calculateSalePrice: true, calculateValueAddedTaxSum: true);

            foreach (var articleGroup in articleGroups.OrderBy(x => x.Key.Name))
            {
                var row = new BaseWaybillArticleGroupRow();

                row.Name = articleGroup.Key.Name;
                row.ArticleCount = articleGroup.Sum(x => x.SellingCount);
                row.PackCount = articleGroup.Sum(x => x.PackCount);

                //вычисляем сумму по группе и сумму ндс по группе
                foreach (var waybillRow in articleGroup)
                {
                    // получаем основные показатели для текущей позиции
                    var ind = indicators[waybillRow.Id];
                    var waybillRowSum = ind.SalePrice != null ? Math.Round(ind.SalePrice.Value * waybillRow.SellingCount, 2) : (decimal?)null;

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

        public ExpenditureWaybillRowEditViewModel CreateRow(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(id, user);

                expenditureWaybillService.CheckPossibilityToEdit(waybill, user);

                var model = new ExpenditureWaybillRowEditViewModel()
                {
                    Title = "Добавление позиции в накладную",
                    AvailableToReserveFromStorageCount = "---",
                    ArticleName = "Выберите товар",
                    MeasureUnitName = "",
                    MeasureUnitScale = "0",
                    AvailableToReserveCount = "---",
                    BatchName = "не выбрана",
                    ExpenditureWaybillDate = waybill.Date.ToString(),
                    MarkupPercent = "---",
                    MarkupSum = "---",
                    AvailableToReserveFromPendingCount = "---",
                    PurchaseCost = "---",
                    SenderAccountingPrice = "---",
                    SenderStorageId = waybill.SenderStorage.Id.ToString(),
                    SenderId = waybill.Sender.Id.ToString(),
                    SellingCount = "",
                    ExpenditureWaybillId = waybill.Id,
                    DealQuotaDiscountPercent = waybill.Quota.DiscountPercent.ForDisplay(ValueDisplayType.Percent),
                    SalePriceString = "---",
                    ValueAddedTaxList = valueAddedTaxService.GetList().GetParamComboBoxItemList(x => x.Name, x => x.Id.ToString(), x => x.Value.ForEdit(), emptyParamValue: "0"),
                    ValueAddedTaxId = waybill.ValueAddedTax.Id,
                    ValueAddedTaxSum = 0M.ForDisplay(ValueDisplayType.Money),
                    RoundSalePrice = waybill.RoundSalePrice,
                    AllowToEdit = true,
                    AllowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere)
                };

                return model;
            }
        }

        public ExpenditureWaybillRowEditViewModel EditRow(Guid expenditureWaybillId, Guid expenditureWaybillRowId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var expenditureWaybill = expenditureWaybillService.CheckWaybillExistence(expenditureWaybillId, user);

                var organization = expenditureWaybill.Sender;
                var expenditureWaybillRow = expenditureWaybill.Rows.FirstOrDefault(x => x.Id == expenditureWaybillRowId) as ExpenditureWaybillRow;
                ValidationUtils.NotNull(expenditureWaybillRow, "Позиция накладной не найдена. Возможно, она была удалена.");

                ExpenditureWaybillRow row = expenditureWaybillRow.As<ExpenditureWaybillRow>();

                var articleBatchAvailability = articleAvailabilityService.GetExtendedArticleBatchAvailability(row.ReceiptWaybillRow, expenditureWaybill.SenderStorage,
                    organization, DateTime.Now);

                var ind = expenditureWaybillService.GetMainRowIndicators(row, user, calculateSalePrice: true, calculateValueAddedTaxSum: true);

                var salePrice = ind.SalePrice;
                var valueAddedTaxSum = ind.ValueAddedTaxSum;
                var senderAccountingPrice = ind.SenderAccountingPrice;

                var allowToEdit = expenditureWaybillService.IsPossibilityToEdit(expenditureWaybill, user);
                var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

                var manualSourcesInfoString = "";
                if (expenditureWaybillRow.IsUsingManualSource)
                {
                    manualSourcesInfoString = SerializeWaybillRowManualSourceInfo(outgoingWaybillRowService.GetManualSources(expenditureWaybillRow.Id));
                }

                var model = new ExpenditureWaybillRowEditViewModel()
                {
                    Title = allowToEdit ? "Редактирование позиции накладной" : "Детали позиции накладной",
                    AllowToEdit = allowToEdit,
                    Id = expenditureWaybillRow.Id,
                    ReceiptWaybillRowId = row.ReceiptWaybillRow.Id,
                    CurrentReceiptWaybillRowId = row.ReceiptWaybillRow.Id,
                    ArticleId = row.Article.Id,
                    ArticleName = row.Article.FullName,
                    MeasureUnitName = row.Article.MeasureUnit.ShortName,
                    MeasureUnitScale = row.ReceiptWaybillRow.ArticleMeasureUnitScale.ToString(),
                    BatchName = row.ReceiptWaybillRow.BatchName,
                    ExpenditureWaybillDate = expenditureWaybill.Date.ToString(),
                    MarkupPercent = allowToViewPurchaseCost && salePrice.HasValue && row.ReceiptWaybillRow.PurchaseCost != 0M ?
                        Math.Round(((salePrice.Value - row.ReceiptWaybillRow.PurchaseCost) / row.ReceiptWaybillRow.PurchaseCost * 100M), 2).ForDisplay(ValueDisplayType.Percent) : "---",
                    MarkupSum = allowToViewPurchaseCost && salePrice.HasValue ? Math.Round((salePrice.Value - row.ReceiptWaybillRow.PurchaseCost), 2).ForDisplay(ValueDisplayType.Money) : "---",
                    PurchaseCost = allowToViewPurchaseCost ? row.ReceiptWaybillRow.PurchaseCost.ForDisplay(ValueDisplayType.Money) : "---",
                    SenderAccountingPrice = senderAccountingPrice.ForDisplay(ValueDisplayType.Money),
                    SenderStorageId = expenditureWaybill.SenderStorage.Id.ToString(),
                    SenderId = expenditureWaybill.Sender.Id.ToString(),
                    SellingCount = expenditureWaybillRow.SellingCount.ForEdit(),
                    DealQuotaDiscountPercent = expenditureWaybill.Quota.DiscountPercent.ForDisplay(ValueDisplayType.Percent),
                    ExpenditureWaybillId = expenditureWaybill.Id,
                    SalePriceString = salePrice.ForDisplay(ValueDisplayType.Money),
                    SalePriceValue = salePrice.ForEdit(ValueDisplayType.Money),
                    RoundSalePrice = expenditureWaybill.RoundSalePrice,

                    AvailableToReserveFromStorageCount = articleBatchAvailability.AvailableToReserveFromStorageCount.ForDisplay(),
                    AvailableToReserveFromPendingCount = articleBatchAvailability.AvailableToReserveFromPendingCount.ForDisplay(),
                    AvailableToReserveCount = articleBatchAvailability.AvailableToReserveCount.ForDisplay(),

                    ManualSourcesInfo = manualSourcesInfoString,

                    ValueAddedTaxList = valueAddedTaxService.GetList().GetParamComboBoxItemList(x => x.Name, x => x.Id.ToString(), x => x.Value.ForEdit(), emptyParamValue: "0"),
                    ValueAddedTaxId = expenditureWaybillRow.ValueAddedTax.Id,
                    ValueAddedTaxSum = valueAddedTaxSum.ForDisplay(ValueDisplayType.Money),
                    AllowToViewPurchaseCost = allowToViewPurchaseCost
                };

                return model;
            }
        }

        public object GetRowInfo(Guid waybillId, Guid batchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var waybill = expenditureWaybillService.CheckWaybillExistence(waybillId, user);

                var batch = receiptWaybillService.GetRowById(batchId);
                var article = batch.Article;
                var storage = waybill.SenderStorage;

                var articleBatchAvailability = articleAvailabilityService.GetExtendedArticleBatchAvailability(batch,
                    storage, waybill.Sender, DateTime.Now);

                var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);
                var allowToViewAccPrice = user.HasPermissionToViewStorageAccountingPrices(storage);

                var purchaseCost = allowToViewPurchaseCost ? batch.PurchaseCost : (decimal?)null;

                decimal? accountingPrice = null, salePrice = null, markupPercent = null, markupSum = null;
                if (allowToViewAccPrice)
                {
                    accountingPrice = articlePriceService.GetAccountingPrice(new List<short>() { storage.Id }, article.Id)[storage.Id];
                    ValidationUtils.NotNull(accountingPrice, String.Format("Не установлена учетная цена на товар «{0}».", article.FullName));

                    salePrice = accountingPrice - (accountingPrice * waybill.Quota.DiscountPercent / 100M);

                    if (allowToViewPurchaseCost)
                    {
                        markupSum = salePrice - purchaseCost;
                        markupPercent = purchaseCost != 0M ? markupSum / purchaseCost * 100M : null;
                    }
                }

                var model = new
                {
                    AvailableToReserveFromStorageCount = articleBatchAvailability.AvailableToReserveFromStorageCount.ForDisplay(),
                    AvailableToReserveFromPendingCount = articleBatchAvailability.AvailableToReserveFromPendingCount.ForDisplay(),
                    AvailableToReserveCount = articleBatchAvailability.AvailableToReserveCount.ForDisplay(),
                    PurchaseCost = purchaseCost.ForDisplay(ValueDisplayType.Money),
                    SenderAccountingPrice = accountingPrice.ForDisplay(ValueDisplayType.Money),

                    SalePrice = salePrice.ForDisplay(ValueDisplayType.Money),
                    SalePriceValue = salePrice.ForEdit(),

                    MarkupPercent = markupPercent.ForDisplay(ValueDisplayType.Percent),
                    MarkupSum = markupSum.ForDisplay(ValueDisplayType.Money)
                };

                return model;
            }
        }

        public object SaveRow(ExpenditureWaybillRowEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ValidationUtils.Assert(model.ExpenditureWaybillId != Guid.Empty, "Накладная реализации товаров не найдена. Возможно, она была удалена.");
                ValidationUtils.Assert(model.ReceiptWaybillRowId != Guid.Empty, "Партия товара не найдена. Возможно, она была удалена.");

                var expenditureWaybill = expenditureWaybillService.CheckWaybillExistence(model.ExpenditureWaybillId, user);

                var receiptWaybillRow = receiptWaybillService.GetRowById(model.ReceiptWaybillRowId);
                ValidationUtils.NotNull(receiptWaybillRow, "Партия товара не найдена. Возможно, она была удалена.");

                expenditureWaybillService.CheckPossibilityToEdit(expenditureWaybill, user);
                var valueAddedTax = valueAddedTaxService.CheckExistence(model.ValueAddedTaxId);

                ExpenditureWaybillRow row = null;
                var sellingCount = ValidationUtils.TryGetDecimal(model.SellingCount);

                // Добавление
                if (model.Id == Guid.Empty)
                {
                    row = new ExpenditureWaybillRow(receiptWaybillRow, sellingCount, valueAddedTax);

                    if (!String.IsNullOrEmpty(model.ManualSourcesInfo))
                    {
                        var sourcesInfo = DeserializeWaybillRowManualSourceInfo(model.ManualSourcesInfo);
                        expenditureWaybillService.AddRow(expenditureWaybill, row, sourcesInfo, user);
                    }
                    else
                    {
                        expenditureWaybillService.AddRow(expenditureWaybill, row, user);
                    }
                }
                // Редактирование
                else
                {
                    row = expenditureWaybill.Rows.FirstOrDefault(x => x.Id == model.Id).As<ExpenditureWaybillRow>();
                    ValidationUtils.NotNull(row, "Позиция накладной не найдена. Возможно, она была удалена.");

                    row.ReceiptWaybillRow = receiptWaybillRow; // Партию важно присваивать перед количеством, чтобы при смене товара проверялась точность количества уже по новому товару

                    row.SellingCount = sellingCount;
                    row.ValueAddedTax = valueAddedTax;

                    if (!String.IsNullOrEmpty(model.ManualSourcesInfo))
                    {
                        var sourcesInfo = DeserializeWaybillRowManualSourceInfo(model.ManualSourcesInfo);
                        expenditureWaybillService.SaveRow(expenditureWaybill, row, sourcesInfo, user);
                    }
                    else
                    {
                        expenditureWaybillService.SaveRow(expenditureWaybill, row, user);
                    }
                }

                uow.Commit();

                return GetMainChangeableIndicators(expenditureWaybill, user);
            }
        }

        public object DeleteRow(Guid expenditureWaybillId, Guid expenditureWaybillRowId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var expenditureWaybill = expenditureWaybillService.CheckWaybillExistence(expenditureWaybillId, user);

                var expenditureWaybillRow = expenditureWaybill.Rows.FirstOrDefault(x => x.Id == expenditureWaybillRowId);
                ValidationUtils.NotNull(expenditureWaybillRow, "Позиция накладной не найдена. Возможно, она была удалена.");

                expenditureWaybillService.DeleteRow(expenditureWaybill, expenditureWaybillRow.As<ExpenditureWaybillRow>(), user);

                uow.Commit();

                return GetMainChangeableIndicators(expenditureWaybill, user);
            }
        }

        /// <summary>
        /// Получение значений основных пересчитываемых показателей
        /// </summary>
        private object GetMainChangeableIndicators(ExpenditureWaybill waybill, User user)
        {
            var j = new
            {
                MainDetails = GetMainDetails(waybill, user),
                Permissions = new
                {
                    AllowToEdit = expenditureWaybillService.IsPossibilityToEdit(waybill, user),
                    AllowToDelete = expenditureWaybillService.IsPossibilityToDelete(waybill, user),
                    AllowToPrepareToAccept = expenditureWaybillService.IsPossibilityToPrepareToAccept(waybill, user),
                    IsPossibilityToPrepareToAccept = expenditureWaybillService.IsPossibilityToPrepareToAccept(waybill, user, true),
                    AllowToCancelReadinessToAccept = expenditureWaybillService.IsPossibilityToCancelReadinessToAccept(waybill, user),
                    AllowToAccept = expenditureWaybillService.IsPossibilityToAccept(waybill, user),
                    AllowToAcceptRetroactively = expenditureWaybillService.IsPossibilityToAcceptRetroactively(waybill, user),
                    IsPossibilityToAccept = expenditureWaybillService.IsPossibilityToAccept(waybill, user, true),
                    IsPossibilityToAcceptRetroactively = expenditureWaybillService.IsPossibilityToAcceptRetroactively(waybill, user, true),
                    AllowToCancelAcceptance = expenditureWaybillService.IsPossibilityToCancelAcceptance(waybill, user),
                    AllowToShip = expenditureWaybillService.IsPossibilityToShip(waybill, user),
                    AllowToShipRetroactively = expenditureWaybillService.IsPossibilityToShipRetroactively(waybill, user),
                    IsPossibilityToShip = expenditureWaybillService.IsPossibilityToShip(waybill, user, true),
                    IsPossibilityToShipRetroactively = expenditureWaybillService.IsPossibilityToShipRetroactively(waybill, user, true),
                    AllowToCancelShipping = expenditureWaybillService.IsPossibilityToCancelShipping(waybill, user),
                    AllowToPrintForms = expenditureWaybillService.IsPossibilityToPrintForms(waybill, user),
                    AllowToChangeCurator = expenditureWaybillService.IsPossibilityToChangeCurator(waybill, user)
                }
            };

            return j;
        }

        #endregion

        #region Подготовка / отмена готовности накладной к проводке

        public object PrepareToAccept(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(id, user);

                expenditureWaybillService.PrepareToAccept(waybill, user);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        public object CancelReadinessToAccept(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(id, user);

                expenditureWaybillService.CancelReadinessToAccept(waybill, user);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        #endregion

        #region Проводка / отмена проводки

        public object Accept(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(waybillId, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                expenditureWaybillService.Accept(waybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        public object CancelAcceptance(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(waybillId, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                expenditureWaybillService.CancelAcceptance(waybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        #region Проводка задним числом

        public DateTimeSelectViewModel GetRetroactivelyAcceptanceViewModel(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return new DateTimeSelectViewModel()
                {
                    Title = "Выбор даты проводки накладной",
                    OkButtonTitle = "Провести",
                    Time = "12:00:00"
                };
            }
        }

        public object AcceptRetroactively(Guid waybillId, DateTime acceptanceDate, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(waybillId, user);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                expenditureWaybillService.AcceptRetroactively(waybill, acceptanceDate, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        #endregion

        #endregion

        #region Отгрузка / отмена отгрузки

        public object Ship(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                expenditureWaybillService.Ship(waybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        public object CancelShipping(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                expenditureWaybillService.CancelShipping(waybill, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        #region Отгрузка задним числом

        public DateTimeSelectViewModel GetRetroactivelyShippingViewModel(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return new DateTimeSelectViewModel()
                {
                    Title = "Выбор даты отгрузки накладной",
                    OkButtonTitle = "Отгрузить",
                    Time = "12:00:00"
                };
            }
        }

        public object ShipRetroactively(Guid waybillId, DateTime shippingDate, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(waybillId, user);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                expenditureWaybillService.ShipRetroactively(waybill, shippingDate, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        #endregion

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

        #region Вспомогательные методы

        /// <summary>
        /// Получение списка всех мест хранения для выпадающего списка
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SelectListItem> GetStorageSelectListItems(Deal deal, User user, bool addEmptyItem = true)
        {
            return dealService.GetStorageList(deal, user)
                .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), addEmptyItem, false);
        }

        #endregion

        #region Добавление позиций списком

        public OutgoingWaybillAddRowsByListViewModel AddRowsByList(Guid waybillId, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(waybillId, user);

                expenditureWaybillService.CheckPossibilityToEdit(waybill, user);

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
                        ";ActionName=/ExpenditureWaybill/ShowArticlesForWaybillRowsAdditionByListGrid/",
                    Sort = "Number=Asc"
                }, user);

                model.RowGrid = GetExpenditureWaybillRowGridLocal(new GridState() { Parameters = "ExpenditureWaybillId=" + waybill.Id, Sort = "CreationDate=Asc" }, user);

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
                var waybill = expenditureWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid(deriveParams["WaybillId"].Value.ToString()), user);

                //убран вызов временно, чтобы фильтр нормально фильтровал в случае, когда открываем в одном окне добавление товаров списком, а в другом окне проводим накладную
                //expenditureWaybillService.CheckPossibilityToEdit(waybill, user);

                deriveParams["ArticleTakingsInfo"].Value = GetArticlesForWaybillRowsAdditionByListGridData(waybill);

                state.Parameters = deriveParams.ToString();

                return GetArticlesForWaybillRowsAdditionByListGridLocal(state, user);
            }
        }

        private string GetArticlesForWaybillRowsAdditionByListGridData(ExpenditureWaybill waybill)
        {
            // информация об уже имеющихся товарах в накладной с группировкой по товару
            return string.Concat(waybill.Rows.GroupBy(x => x.Article)
                .Select(x => string.Format("{0}_{1}:", x.Key.Id, x.Sum(y => y.SellingCount).ForEdit())));
        }

        public void AddRowSimply(Guid waybillId, int articleId, decimal count, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(waybillId, user);
                var article = articleService.CheckArticleExistence(articleId);

                expenditureWaybillService.AddRowSimply(waybill, article, count, user);

                uow.Commit();
            }
        }

        #endregion

        #region Печатные формы

        #region Товарный чек

        public CashMemoPrintingFormViewModel GetCashMemoPrintingForm(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(id, user);

                expenditureWaybillService.CheckPossibilityToPrintForms(waybill, user);

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

                    var article = row.As<ExpenditureWaybillRow>().Article;
                    printingFormRow.ArticleName = article.Number + " " + article.FullName;
                    printingFormRow.Count = row.SellingCount.ForDisplay();

                    decimal? salePrice = row.SalePrice;

                    printingFormRow.Price = salePrice.ForDisplay(ValueDisplayType.Money);
                    printingFormRow.PackSize = article.PackSize.ForDisplay();

                    var sum = salePrice.HasValue ?
                        (decimal?)(row.SellingCount * salePrice.Value) : null;

                    if (sum.HasValue)
                    {
                        totalSum += sum.Value;
                    }

                    printingFormRow.Sum = sum.ForDisplay(ValueDisplayType.Money);

                    printingFormRows.Add(printingFormRow);
                }

                model.Rows = printingFormRows;
                model.TotalSum = totalSum.ForDisplay(ValueDisplayType.Money);

                return model;
            }
        }
        #endregion

        #region Расходная накладная

        /// <summary>
        /// Получение модели параметров счета на оплату
        /// </summary>        
        public ExpenditureWaybillPrintingFormSettingsViewModel GetExpenditureWaybillPrintingFormSettings(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(waybillId, user);

                var model = new ExpenditureWaybillPrintingFormSettingsViewModel()
                {
                    ConsiderReturns = false
                };

                return model;
            }
        }

        /// <summary>
        /// Получение модели печатной формы
        /// </summary>
        /// <param name="settings">Параметры печатной формы</param>
        /// <returns>Модель печатной формы</returns>
        public ExpenditureWaybillPrintingFormViewModel GetExpenditureWaybillPrintingForm(ExpenditureWaybillPrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var id = settings.WaybillId;
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(id, user);

                expenditureWaybillService.CheckPossibilityToPrintForms(waybill, user);

                //Учитывать возвраты или нет
                var considerReturns = settings.ConsiderReturns.HasValue && settings.ConsiderReturns.Value;

                var model = new ExpenditureWaybillPrintingFormViewModel()
                {
                    OrganizationName = waybill.Sender.FullName,
                    Title = String.Format("Расходная накладная {0}", waybill.Name),
                    Date = DateTime.Now.ToShortDateString(),
                    RecepientName = GetOrganizationFullInfoForTORG12(waybill.Deal.Contract.ContractorOrganization, waybill),
                };

                decimal totalSalePrice = 0;
                decimal valueAddedTaxSum = 0;

                foreach (var row in waybill.Rows.OrderBy(x => x.CreationDate))
                {
                    var count = considerReturns ? row.SellingCount - row.ReceiptedReturnCount : row.SellingCount;
                    if (count > 0)
                    {
                        var article = row.As<ExpenditureWaybillRow>().Article;
                        var expenditureWaybillPrintingFormItem = new ExpenditureWaybillPrintingFormItemViewModel()
                        {
                            Id = article.Id.ForDisplay(),
                            Number = article.Number,
                            ArticleName = article.FullName,
                            Count = count.ForDisplay(),
                            SalePrice = row.SalePrice.ForDisplay(ValueDisplayType.Money),
                            SalePriceSum = row.SalePrice.HasValue ? (row.SalePrice * count).Value.ForDisplay(ValueDisplayType.Money) : "---",
                            PackSize = article.PackSize.ForDisplay(),
                            PackCount = (article.PackSize > 0 ? count / article.PackSize : 0).ForDisplay(ValueDisplayType.PackCount),
                            Weight = (row.Weight * count / row.SellingCount).ForDisplay(ValueDisplayType.Weight),
                            Volume = (row.Volume * count / row.SellingCount).ForDisplay(ValueDisplayType.Volume)
                        };

                        var rowCostWithVatSum = Math.Round(row.SalePrice.Value * count, 2);
                        var rowVatSum = VatUtils.CalculateVatSum(rowCostWithVatSum, row.ValueAddedTax.Value);

                        totalSalePrice += rowCostWithVatSum;
                        valueAddedTaxSum += rowVatSum;

                        model.Rows.Add(expenditureWaybillPrintingFormItem);
                        model.TotalCount += count;
                    }
                }

                model.TotalSalePrice = totalSalePrice;
                model.ValueAddedTaxSum = valueAddedTaxSum.ForDisplay(ValueDisplayType.Money);
                model.TotalSumWithoutValueAddedTax = (totalSalePrice - valueAddedTaxSum).ForDisplay(ValueDisplayType.Money);

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
                var waybill = expenditureWaybillService.CheckWaybillExistence(waybillId, user);

                var allowToViewSalePrices = expenditureWaybillService.IsPossibilityToPrintFormInSalePrices(waybill, user);
                var allowToViewPurchaseCosts = expenditureWaybillService.IsPossibilityToPrintFormInPurchaseCosts(waybill, user);

                ValidationUtils.Assert(allowToViewSalePrices || allowToViewPurchaseCosts,
                    "Нет прав на просмотр ни отпускных цен, ни закупочных цен.");

                var priceTypes = new List<PrintingFormPriceType>();
                if (allowToViewSalePrices) priceTypes.Add(PrintingFormPriceType.SalePrice);
                if (allowToViewPurchaseCosts) priceTypes.Add(PrintingFormPriceType.PurchaseCost);

                var model = new InvoicePrintingFormSettingsViewModel()
                {
                    PriceTypes = priceTypes.GetComboBoxItemList(s => s.GetDisplayName(), s => s.ValueToString(), false, false),
                    ConsiderReturns = false
                };

                return model;
            }
        }

        public InvoicePrintingFormViewModel GetInvoicePrintingForm(InvoicePrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(settings.WaybillId, user);

                var priceType = ValidationUtils.TryGetEnum<PrintingFormPriceType>(settings.PriceTypeId);

                // проверка возможности просмотра определенного типа цен
                CheckPriceTypePermissions(user, waybill, priceType);

                var model = new InvoicePrintingFormViewModel();
                model.PriceTypeId = settings.PriceTypeId;
                model.WaybillId = settings.WaybillId;
                model.ConsiderReturns = settings.ConsiderReturns;
                model.Title = "СЧЕТ-ФАКТУРА";
                model.Number = waybill.Number;
                model.Date = waybill.Date.ToShortDateString();

                var seller = waybill.Sender;
                var buyer = waybill.Deal.Contract.ContractorOrganization;

                model.SellerName = seller.ShortName;
                model.SellerAddress = String.IsNullOrWhiteSpace(seller.Address) ? "-" : seller.Address;

                if (seller.EconomicAgent.Type == EconomicAgentType.JuridicalPerson)
                {
                    var juridicalPerson = seller.EconomicAgent.As<JuridicalPerson>();
                    model.SellerINN_KPP = juridicalPerson.INN + " / " + juridicalPerson.KPP;
                }
                else
                {
                    var physicalPerson = seller.EconomicAgent.As<PhysicalPerson>();
                    model.SellerINN_KPP = physicalPerson.INN;
                }

                model.ArticleSenderInfo = "он же";

                model.ArticleRecipientInfo = waybill.Deal.Contract.ContractorOrganization.FullName;
                model.ArticleRecipientInfo += !String.IsNullOrEmpty(waybill.DeliveryAddress) ? String.Format(", {0}", waybill.DeliveryAddress) : "";
                model.ArticleRecipientInfo += !String.IsNullOrEmpty(waybill.Deal.Contract.ContractorOrganization.Phone) ? String.Format(", тел.: {0}", waybill.Deal.Contract.ContractorOrganization.Phone) : "";

                model.PaymentDocumentNumber = "-";

                model.BuyerName = buyer.ShortName;
                model.BuyerAddress = String.IsNullOrWhiteSpace(buyer.Address) ? "-" : buyer.Address;

                if (buyer.EconomicAgent.Type == EconomicAgentType.JuridicalPerson)
                {
                    var juridicalPerson = buyer.EconomicAgent.As<JuridicalPerson>();
                    model.BuyerINN_KPP =
                        (String.IsNullOrWhiteSpace(juridicalPerson.INN) ? "-" : juridicalPerson.INN)
                        + " / " +
                        (String.IsNullOrWhiteSpace(juridicalPerson.KPP) ? "-" : juridicalPerson.KPP);
                }
                else
                {
                    var physicalPerson = buyer.EconomicAgent.As<PhysicalPerson>();
                    model.BuyerINN_KPP = String.IsNullOrWhiteSpace(physicalPerson.INN) ? "-" : physicalPerson.INN;
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
                var waybill = expenditureWaybillService.CheckWaybillExistence(settings.WaybillId, user);

                var priceType = ValidationUtils.TryGetEnum<PrintingFormPriceType>(settings.PriceTypeId);

                //Учитывать возвраты или нет
                var considerReturns = settings.ConsiderReturns.HasValue && settings.ConsiderReturns.Value;

                // проверка возможности просмотра определенного типа цен
                CheckPriceTypePermissions(user, waybill, priceType);

                var model = new InvoicePrintingFormRowsViewModel();

                foreach (var row in waybill.Rows.OrderBy(x => x.CreationDate))
                {
                    var count = considerReturns ? (row.SellingCount - row.ReceiptedReturnCount) : row.SellingCount;
                    if (count > 0)
                    {
                        var item = new InvoicePrintingFormItemViewModel();

                        var batch = row.As<ExpenditureWaybillRow>().ReceiptWaybillRow;
                        var article = batch.Article;

                        item.ArticleName = article.Number + " " + article.FullName;
                        item.MeasureUnitName = article.MeasureUnit.ShortName;
                        item.MeasureUnitCode = article.MeasureUnit.NumericCode;

                        item.CountValue = count;
                        decimal price = 0M, valueAddedTaxSum = 0M;
                        switch (priceType)
                        {
                            case PrintingFormPriceType.SalePrice:
                                price = row.SalePrice ?? 0;
                                valueAddedTaxSum = considerReturns ? VatUtils.CalculateVatSum(row.SalePrice * count, row.ValueAddedTax.Value).Value : row.ValueAddedTaxSum;
                                break;
                            case PrintingFormPriceType.PurchaseCost:
                                price = row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.PurchaseCost;
                                valueAddedTaxSum = considerReturns ? VatUtils.CalculateVatSum(row.PurchaseCost * count, row.ValueAddedTax.Value) : row.As<ExpenditureWaybillRow>().PurchaseCostValueAddedTaxSum;
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

                        item.CustomsDeclarationNumber = String.IsNullOrEmpty(batch.CustomsDeclarationNumber) ? "-" : batch.CustomsDeclarationNumber;

                        var taxedCost = Math.Round(price * count, (priceType == PrintingFormPriceType.PurchaseCost ? 6 : 2));
                        var taxSum = valueAddedTaxSum;
                        var cost = taxedCost - taxSum;

                        item.TaxedCostValue = taxedCost;
                        item.CostValue = cost;
                        item.TaxSumValue = taxSum;

                        model.Rows.Add(item);
                    }
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

        #region ТОРГ 12

        /// <summary>
        /// Получение модели параметров ТОРГ12
        /// </summary>
        public TORG12PrintingFormSettingsViewModel GetTORG12PrintingFormSettings(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(waybillId, user);

                var allowToViewSalePrices = expenditureWaybillService.IsPossibilityToPrintFormInSalePrices(waybill, user);
                var allowToViewPurchaseCosts = expenditureWaybillService.IsPossibilityToPrintFormInPurchaseCosts(waybill, user);

                ValidationUtils.Assert(allowToViewSalePrices || allowToViewPurchaseCosts,
                    "Нет прав на просмотр ни отпускных цен, ни закупочных цен.");

                var priceTypes = new List<PrintingFormPriceType>();
                if (allowToViewSalePrices) priceTypes.Add(PrintingFormPriceType.SalePrice);
                if (allowToViewPurchaseCosts) priceTypes.Add(PrintingFormPriceType.PurchaseCost);

                var model = new TORG12PrintingFormSettingsViewModel()
                {
                    PriceTypes = priceTypes.GetComboBoxItemList(s => s.GetDisplayName(), s => s.ValueToString(), false, false),
                    ConsiderReturns = false
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
                var waybill = expenditureWaybillService.CheckWaybillExistence(settings.WaybillId, user);

                var priceType = ValidationUtils.TryGetEnum<PrintingFormPriceType>(settings.PriceTypeId);

                // проверка возможности просмотра определенного типа цен
                CheckPriceTypePermissions(user, waybill, priceType);

                var organizationJuridicalPerson = waybill.Sender.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                    waybill.Sender.EconomicAgent.As<JuridicalPerson>() : null;

                var payerJuridicalPerson = waybill.Deal.Contract.ContractorOrganization.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                    waybill.Deal.Contract.ContractorOrganization.EconomicAgent.As<JuridicalPerson>() : null;

                var recepientJuridicalPerson = waybill.Deal.Contract.ContractorOrganization.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                    waybill.Deal.Contract.ContractorOrganization.EconomicAgent.As<JuridicalPerson>() : null;

                var model = new TORG12PrintingFormViewModel()
                {
                    PriceTypeId = settings.PriceTypeId,
                    ConsiderReturns = settings.ConsiderReturns,
                    WaybillId = waybill.Id.ToString(),
                    OrganizationName = waybill.Sender.GetFullInfo(),
                    Date = waybill.Date.ToShortDateString(),
                    OrganizationOKPO = organizationJuridicalPerson != null ? organizationJuridicalPerson.OKPO : "",
                    Payer = waybill.Deal.Contract.ContractorOrganization.GetFullInfo(),
                    PayerOKPO = payerJuridicalPerson != null ? payerJuridicalPerson.OKPO : "",
                    Reason = waybill.Deal.Contract.FullName,
                    ReasonDate = "",
                    ReasonNumber = "",
                    Recepient = GetOrganizationFullInfoForTORG12(waybill.Deal.Contract.ContractorOrganization, waybill),
                    RecepientOKPO = recepientJuridicalPerson != null ? recepientJuridicalPerson.OKPO : "",
                    Sender = waybill.Sender.GetFullInfo(),
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
                var waybill = expenditureWaybillService.CheckWaybillExistence(settings.WaybillId, user);

                var priceType = ValidationUtils.TryGetEnum<PrintingFormPriceType>(settings.PriceTypeId);

                // проверка возможности просмотра определенного типа цен
                CheckPriceTypePermissions(user, waybill, priceType);

                var model = new TORG12PrintingFormRowsViewModel();

                //Для получения кодов ОКЕИ
                var measureUnitConv = new Dictionary<string, string>();
                measureUnitConv.Add("шт.", "796");

                //Учитывать возвраты или нет
                var considerReturns = settings.ConsiderReturns.HasValue && settings.ConsiderReturns.Value;

                dynamic query;

                // учитываем партии
                if (priceType == PrintingFormPriceType.PurchaseCost)
                {
                    query = waybill.Rows.Where(x => considerReturns ? (x.SellingCount - x.ReceiptedReturnCount) > 0 : x.SellingCount > 0)
                        .GroupBy(x => new { x.As<ExpenditureWaybillRow>().ReceiptWaybillRow, x.ValueAddedTax })
                        .Select(x => new
                        {
                            PurchaseCost = x.Key.ReceiptWaybillRow.PurchaseCost,
                            SalePrice = x.First().SalePrice,
                            SellingCount = x.Sum(y => considerReturns ? y.SellingCount - y.ReceiptedReturnCount : y.SellingCount),
                            Article = x.Key.ReceiptWaybillRow.Article,
                            CreationDate = x.Min(y => y.CreationDate),
                            ValueAddedTaxSum = x.Sum(y => considerReturns ?
                                    VatUtils.CalculateVatSum(y.SalePrice * (y.SellingCount - y.ReceiptedReturnCount), y.ValueAddedTax.Value) : y.ValueAddedTaxSum),
                            PurchaseCostValueAddedTaxSum = x.Sum(y => considerReturns ?
                                    VatUtils.CalculateVatSum(y.PurchaseCost * (y.SellingCount - y.ReceiptedReturnCount), y.ValueAddedTax.Value) : y.As<ExpenditureWaybillRow>().PurchaseCostValueAddedTaxSum),
                            ValueAddedTax = x.Key.ValueAddedTax
                        })
                        .OrderBy(x => x.CreationDate);
                }
                // партии не учитываем
                else
                {
                    query = waybill.Rows.Where(x => considerReturns ? (x.SellingCount - x.ReceiptedReturnCount) > 0 : x.SellingCount > 0)
                        .GroupBy(x => new { x.Article, x.ValueAddedTax })
                        .Select(x => new
                        {
                            SalePrice = x.First().SalePrice,
                            SellingCount = x.Sum(y => considerReturns ? y.SellingCount - y.ReceiptedReturnCount : y.SellingCount),
                            Article = x.Key.Article,
                            CreationDate = x.Min(y => y.CreationDate),
                            ValueAddedTaxSum = x.Sum(y => considerReturns ?
                                    VatUtils.CalculateVatSum(y.SalePrice * (y.SellingCount - y.ReceiptedReturnCount), y.ValueAddedTax.Value) : y.ValueAddedTaxSum),
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

                    priceSum = Math.Round(price * row.SellingCount, (priceType == PrintingFormPriceType.PurchaseCost ? 6 : 2));

                    var formItem = new TORG12PrintingFormItemViewModel(price, priceSum, row.SellingCount, valueAddedTaxSum, row.ValueAddedTax.Value)
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

        #region Счет на оплату

        /// <summary>
        /// Получение модели параметров счета на оплату
        /// </summary>        
        public PaymentInvoicePrintingFormSettingsViewModel GetPaymentInvoicePrintingFormSettings(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(waybillId, user);

                var model = new PaymentInvoicePrintingFormSettingsViewModel()
                {
                    ConsiderReturns = false
                };

                return model;
            }
        }

        /// <summary>
        /// Получение модели печатной формы счета на оплату
        /// </summary>
        /// <param name="settings">настройки печати</param>
        /// <param name="currentUser">информация о пользователе</param>
        public PaymentInvoicePrintingFormViewModel GetPaymentInvoicePrintingForm(PaymentInvoicePrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var id = settings.WaybillId;
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = expenditureWaybillService.CheckWaybillExistence(id, user);

                expenditureWaybillService.CheckPossibilityToPrintForms(waybill, user);

                //Учитывать возвраты или нет
                var considerReturns = settings.ConsiderReturns.HasValue && settings.ConsiderReturns.Value;

                var model = new PaymentInvoicePrintingFormViewModel();

                model.WaybillId = waybill.Id;
                model.Title = "Счет на оплату";
                model.Number = waybill.Number;
                model.Date = waybill.Date.ToShortDateString();

                var seller = waybill.Sender;
                var buyer = waybill.Deal.Contract.ContractorOrganization;

                model.SellerName = seller.FullName;

                switch (seller.EconomicAgent.Type)
                {
                    case EconomicAgentType.JuridicalPerson:
                        var jp = seller.EconomicAgent.As<JuridicalPerson>();
                        model.BookkeeperName = jp.MainBookkeeperName;
                        model.DirectorName = jp.DirectorName;
                        model.SellerINN = jp.INN;
                        model.SellerKPP = jp.KPP;
                        break;

                    case EconomicAgentType.PhysicalPerson:
                        var pp = seller.EconomicAgent.As<PhysicalPerson>();
                        model.DirectorName = pp.OwnerName;
                        model.BookkeeperName = pp.OwnerName;
                        model.SellerINN = pp.INN;
                        break;
                }

                var sellerBankAccount = seller.RussianBankAccounts.Any() ? seller.RussianBankAccounts.First() : null;
                model.SellerBankName = sellerBankAccount != null ? sellerBankAccount.Bank.Name : "---";
                model.SellerBankBIC = sellerBankAccount != null ? sellerBankAccount.Bank.As<RussianBank>().BIC : "---";
                model.SellerBankAccountNumber = sellerBankAccount != null ? sellerBankAccount.Bank.As<RussianBank>().CorAccount : "---";

                model.SellerAccountNumber = sellerBankAccount != null ? sellerBankAccount.Number : "---";

                model.SellerInfo = GetOrganizationFullInfoForPaymentInvoice(seller);
                model.BuyerInfo = GetOrganizationFullInfoForPaymentInvoice(buyer, waybill.DeliveryAddress);

                var rowList = new List<PaymentInvoicePrintingFormItemViewModel>();
                int rowNumber = 0;
                decimal totalCostWithVatSum = 0;
                decimal totalVatSum = 0;

                foreach (var item in waybill.Rows.OrderBy(x => x.CreationDate))
                {
                    var count = considerReturns ? item.SellingCount - item.ReceiptedReturnCount : item.SellingCount;
                    if (count > 0)
                    {
                        var paymentInvoiceRow = new PaymentInvoicePrintingFormItemViewModel();
                        rowNumber++;

                        paymentInvoiceRow.Number = rowNumber.ForDisplay();

                        decimal salePrice = expenditureWaybillService.CalculateRowSalePrice(item.As<ExpenditureWaybillRow>()) ?? 0;

                        var article = item.As<ExpenditureWaybillRow>().Article;
                        paymentInvoiceRow.ArticleName = article.Number + " " + article.FullName;
                        paymentInvoiceRow.MeasureUnitName = article.MeasureUnit.ShortName;

                        paymentInvoiceRow.Count = count.ForDisplay();
                        paymentInvoiceRow.Price = salePrice.ForDisplay(ValueDisplayType.Money);

                        var rowCostWithVatSum = Math.Round(salePrice * count, 2);
                        var rowVatSum = VatUtils.CalculateVatSum(rowCostWithVatSum, item.ValueAddedTax.Value);
                        paymentInvoiceRow.Cost = rowCostWithVatSum.ForDisplay(ValueDisplayType.Money);

                        totalCostWithVatSum += rowCostWithVatSum;
                        totalVatSum += rowVatSum;

                        rowList.Add(paymentInvoiceRow);
                    }
                }

                model.Rows = rowList;

                model.Total = totalCostWithVatSum.ForDisplay(ValueDisplayType.Money);
                model.TotalToPay = totalCostWithVatSum.ForDisplay(ValueDisplayType.Money);

                if (totalVatSum > 0M)
                {
                    model.TotalValueAddedTax = totalVatSum.ForDisplay(ValueDisplayType.Money);
                    model.TotalValueAddedTax_caption = "В том числе НДС";
                }
                else
                {
                    model.TotalValueAddedTax = "-";
                    model.TotalValueAddedTax_caption = "Без налога (НДС)";
                }

                model.RowsSumInSamples = SpelledOutCurrency.Get(totalCostWithVatSum);

                model.RowsCount = model.Rows.Count.ForDisplay();
                model.RowsSum = totalCostWithVatSum.ForDisplay(ValueDisplayType.Money);

                return model;
            }
        }

        #endregion

        /// <summary>
        /// Возвращает текстовое описание организации в виде Организация, ИНН, КПП, адрес, телефон
        /// Пустые, неуказанные поля в результат не включаются
        /// </summary>
        /// <returns>Описание организации</returns>
        private string GetOrganizationFullInfoForPaymentInvoice(Organization org, string address = "")
        {
            StringBuilder orgInfo = new StringBuilder();

            var inn = org.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                    org.EconomicAgent.As<JuridicalPerson>().INN : org.EconomicAgent.As<PhysicalPerson>().INN;

            var kpp = org.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ? org.EconomicAgent.As<JuridicalPerson>().KPP : "";

            orgInfo.Append(org.FullName);
            if (!String.IsNullOrEmpty(inn)) orgInfo.Append(String.Format(", ИНН {0}", inn));
            if (!String.IsNullOrEmpty(kpp)) orgInfo.Append(String.Format(", КПП {0}", kpp));
            if (address == "")
            {
                if (!String.IsNullOrEmpty(org.Address)) orgInfo.Append(String.Format(", {0}", org.Address));
            }
            else
            {
                orgInfo.Append(String.Format(", {0}", address));
            }
            if (!String.IsNullOrEmpty(org.Phone)) orgInfo.Append(String.Format(", тел.: {0}", org.Phone));

            return orgInfo.ToString();
        }

        /// <summary>
        /// Проверка возможности просмотра определенного типа цен
        /// </summary>        
        private void CheckPriceTypePermissions(User user, ExpenditureWaybill waybill, PrintingFormPriceType priceType)
        {
            switch (priceType)
            {
                case PrintingFormPriceType.SalePrice:
                    expenditureWaybillService.CheckPossibilityToPrintFormInSalePrices(waybill, user);
                    break;
                case PrintingFormPriceType.PurchaseCost:
                    expenditureWaybillService.CheckPossibilityToPrintFormInPurchaseCosts(waybill, user);
                    break;
                default:
                    throw new Exception("Неверное значение типа цен, в которых печается отчет.");
            }
        }

        /// <summary>
        /// Метод возвращает полное текстовое описание организации в виде Организация, адрес, телефон, ИНН, р/с, банк для накладной ТОРГ-12 (с учетом адреса доставки из накладной)
        /// </summary>
        /// <returns>Описание организации</returns>
        private string GetOrganizationFullInfoForTORG12(Organization org, ExpenditureWaybill waybill)
        {
            var sb = new StringBuilder();
            sb.Append(org.FullName);

            var inn = org.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                org.EconomicAgent.As<JuridicalPerson>().INN : org.EconomicAgent.As<PhysicalPerson>().INN;

            if (!String.IsNullOrEmpty(waybill.DeliveryAddress)) sb.Append(String.Format(", {0}", waybill.DeliveryAddress));
            if (!String.IsNullOrEmpty(org.Phone)) sb.Append(String.Format(", тел.: {0}", org.Phone));

            sb.Append(String.Format(", ИНН: {0}", inn));

            var bankAccounts = org.RussianBankAccounts.Where(x => x.IsMaster);
            if (bankAccounts.Count() > 1)
            {
                throw new Exception("Невозможно определить основной расчетный счет организации, так как ей назначено более одного основного расчетного счета.");
            }

            if (bankAccounts.Count() == 1)
            {
                var bankAccount = bankAccounts.Single().As<RussianBankAccount>();
                var bank = bankAccount.Bank.As<RussianBank>();

                sb.Append(String.Format(", р/с {0} в банке {1}, {2}, БИК {3}, корр/с {4} ", bankAccount.Number, bankAccount.Bank.Name, bankAccount.Bank.Address, bank.BIC, bank.CorAccount));
            }

            return sb.ToString();
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
                var waybill = expenditureWaybillService.CheckWaybillExistence(waybillId, user);

                var allowToViewSalePrices = expenditureWaybillService.IsPossibilityToPrintFormInSalePrices(waybill, user);
                ValidationUtils.Assert(allowToViewSalePrices, "Нет прав на просмотр отпускных цен.");

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
                var waybill = expenditureWaybillService.CheckWaybillExistence(settings.WaybillId, user);
                var currentDate = DateTimeUtils.GetCurrentDateTime();

                var allowToViewSalePrices = expenditureWaybillService.IsPossibilityToPrintFormInSalePrices(waybill, user);
                ValidationUtils.Assert(allowToViewSalePrices, "Нет прав на просмотр отпускных цен.");

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
                    Payer = waybill.Deal.Contract.ContractorOrganization.GetFullInfo(),
                    ArticleCountString = SpelledOutCurrency.Get(waybill.Rows.GroupBy(x => x.Article).Count(), true, "", "", "", "", "", "", false),
                    Recipient = waybill.Deal.Contract.ContractorOrganization.GetFullInfo(),
                    RowCountString = SpelledOutCurrency.Get(waybill.RowCount, true, "", "", "", "", "", "", false),
                    Sender = waybill.Deal.Contract.AccountOrganization.GetFullInfo(),
                    TotalSumSeniorString = SpelledOutCurrency.Get(waybill.SalePriceSum, true, "", "", "", "", "", "", false),
                    TotalSumJuniorString = ((waybill.SalePriceSum % 1) * 100).ToString("00"),   // Получаем копейки
                    TotalSumValue = waybill.SalePriceSum.ForDisplay(ValueDisplayType.Money),
                    ValueAddedTaxSum = waybill.Rows.Sum(x => x.ValueAddedTaxSum).ForDisplay(ValueDisplayType.Money),
                    ValueAddedTaxPercentage = waybill.Rows.GroupBy(x => x.ValueAddedTax).Count() == 1 ?
                        waybill.Rows.First().ValueAddedTax.Value.ForDisplay(ValueDisplayType.Percent) : "",
                    WaybillId = waybill.Id.ToString(),
                    TotalCount = waybill.Rows.Sum(x => x.SellingCount).ForDisplay(scale),
                    CountScale = scale.ToString(),  // точность вывода количества (для JS)
                    TotalWeight = (waybill.Weight / 1000).ForDisplay(ValueDisplayType.WeightWithZeroFractionPart), //Выводим в тоннах
                    TotalSum = waybill.Rows.Sum(x => (x.SalePrice * x.SellingCount)).ForDisplay(ValueDisplayType.MoneyWithZeroCopecks)
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
                var waybill = expenditureWaybillService.CheckWaybillExistence(settings.WaybillId, user);
                var currentDate = DateTimeUtils.GetCurrentDateTime();

                var allowToViewSalePrices = expenditureWaybillService.IsPossibilityToPrintFormInSalePrices(waybill, user);
                ValidationUtils.Assert(allowToViewSalePrices, "Нет прав на просмотр отпускных цен.");

                var model = new T1ProductSectionPrintingFormRowsViewModel();

                foreach (var row in waybill.Rows)
                {
                    model.Rows.Add(new T1ProductSectionPrintingFormItemViewModel()
                    {
                        Count = row.SellingCount.ForDisplay(),
                        ItemNumber = row.Article.Id.ForDisplay(),
                        MeasureUnit = row.Article.MeasureUnit.ShortName,
                        Name = row.Article.FullName,
                        Number = row.Article.Number,
                        ListPriseNumber = "-",
                        Price = row.SalePrice.ForDisplay(ValueDisplayType.MoneyWithZeroCopecks),
                        Sum = (row.SalePrice * row.SellingCount).ForDisplay(ValueDisplayType.MoneyWithZeroCopecks),
                        Weight = (row.Weight / 1000).ForDisplay(ValueDisplayType.WeightWithZeroFractionPart)   // переводим в тонны
                    });
                }

                return model;
            }
        }

        #endregion

        #endregion

        #endregion
    }
}