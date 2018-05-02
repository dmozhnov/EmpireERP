using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.Utils;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;
using ERP.Wholesale.UI.ViewModels.ClientContract;
using ERP.Wholesale.UI.ViewModels.Deal;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class DealPresenter : IDealPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IDealService dealService;
        private readonly IDealQuotaService dealQuotaService;
        private readonly IStorageService storageService;
        private readonly ITeamService teamService;
        private readonly IClientService clientService;
        private readonly IDealPaymentDocumentService dealPaymentDocumentService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly IUserService userService;
        private readonly IExpenditureWaybillService expenditureWaybillService;
        private readonly IReturnFromClientWaybillService returnFromClientWaybillService;
        private readonly IReturnFromClientWaybillMainIndicatorService returnFromClientWaybillIndicatorService;
        private readonly IClientContractService clientContractService;
        
        private readonly ITaskPresenterMediator taskPresenterMediator;
        private readonly IDealPaymentDocumentPresenterMediator dealPaymentDocumentPresenterMediator;
        private readonly IClientContractPresenterMediator clientContractPresenterMediator;

        #endregion

        #region Конструктор

        public DealPresenter(IUnitOfWorkFactory unitOfWorkFactory, IDealService dealService, IStorageService storageService, ITeamService teamService, IExpenditureWaybillService expenditureWaybillService,
            IClientService clientService, IDealPaymentDocumentService dealPaymentDocumentService,
            IAccountOrganizationService accountOrganizationService, IUserService userService, IDealQuotaService dealQuotaService,
            IReturnFromClientWaybillService returnFromClientWaybillService, IReturnFromClientWaybillMainIndicatorService returnFromClientWaybillIndicatorService,
            IClientContractService clientContractService, ITaskPresenterMediator taskPresenterMediator, IDealPaymentDocumentPresenterMediator dealPaymentDocumentPresenterMediator, 
            IClientContractPresenterMediator clientContractPresenterMediator)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.dealService = dealService;
            this.storageService = storageService;
            this.teamService = teamService;
            this.expenditureWaybillService = expenditureWaybillService;
            this.clientService = clientService;
            this.dealPaymentDocumentService = dealPaymentDocumentService;
            this.accountOrganizationService = accountOrganizationService;
            this.userService = userService;
            this.dealQuotaService = dealQuotaService;
            this.returnFromClientWaybillService = returnFromClientWaybillService;
            this.returnFromClientWaybillIndicatorService = returnFromClientWaybillIndicatorService;
            this.clientContractService = clientContractService;

            this.taskPresenterMediator = taskPresenterMediator;
            this.dealPaymentDocumentPresenterMediator = dealPaymentDocumentPresenterMediator;
            this.clientContractPresenterMediator = clientContractPresenterMediator;
        }

        #endregion

        #region Методы

        #region Список

        public DealListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Deal_List_Details);

                var model = new DealListViewModel();
                model.ActiveDealGrid = GetActiveDealGridLocal(new GridState { Sort = "CreationDate=Desc" }, user);
                model.ClosedDealGrid = GetClosedDealGridLocal(new GridState { Sort = "CreationDate=Desc" }, user);

                model.Filter.Items.Add(new FilterTextEditor("Name", "Наименование"));
                model.Filter.Items.Add(new FilterComboBox("Stage", "Этап", ComboBoxBuilder.GetComboBoxItemList<DealStage>(sort: false)));
                model.Filter.Items.Add(new FilterTextEditor("Client_Name", "Клиент"));
                model.Filter.Items.Add(new FilterTextEditor("Curator_DisplayName", "Куратор"));

                return model;
            }
        }

        public GridData GetActiveDealGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetActiveDealGridLocal(state, user);
            }
        }

        private GridData GetActiveDealGridLocal(GridState state, User user)
        {
            if (state == null) state = new GridState();

            var model = new GridData { State = state };
            model.AddColumn("Name", "Название", Unit.Percentage(38));
            model.AddColumn("CuratorName", "Куратор", Unit.Percentage(17));
            model.AddColumn("ClientName", "Клиент", Unit.Percentage(32));
            model.AddColumn("StageName", "Этап", Unit.Pixel(150));
            model.AddColumn("ExpectedBudget", "Ожидаемый бюджет", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("SalesVolume", "Сумма реализации", Unit.Percentage(13), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("CuratorId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ClientId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            string filter = state.Filter;
            ParameterString deriveFilter = new ParameterString(state.Filter);

            var filterStringList = new List<String> { ((byte)DealStage.ClientInvestigation).ToString(),
                                    ((byte)DealStage.CommercialProposalPreparation).ToString(),
                                    ((byte)DealStage.Negotiations).ToString(),
                                    ((byte)DealStage.ContractSigning).ToString(),
                                    ((byte)DealStage.ContractExecution).ToString(),
                                    ((byte)DealStage.ContractClosing).ToString(),
                                    ((byte)DealStage.DecisionMakerSearch).ToString()};

            if (deriveFilter["Stage"] != null && deriveFilter["Stage"].Value as string != "")
            {
                deriveFilter["Stage"].Value = filterStringList.Contains<string>(deriveFilter["Stage"].Value as string) ? deriveFilter["Stage"].Value as string : "0";
            }
            else
            {
                if (deriveFilter["Stage"] != null)
                    deriveFilter.Delete("Stage");

                deriveFilter.Add("Stage", ParameterStringItem.OperationType.OneOf, filterStringList);
            }

            var deals = dealService.GetFilteredList(state, deriveFilter, user, Permission.Deal_List_Details);

            foreach (var deal in deals)
            {
                model.AddRow(new GridRow(
                    new GridLinkCell("Name") { Value = deal.Name },
                    userService.IsPossibilityToViewDetails(deal.Curator, user) ?
                        (GridCell)new GridLinkCell("CuratorName") { Value = deal.Curator.DisplayName } :
                        new GridLabelCell("CuratorName") { Value = deal.Curator.DisplayName },
                    user.HasPermission(Permission.Client_List_Details) ?
                        (GridCell)new GridLinkCell("ClientName") { Value = deal.Client.Name } :
                        new GridLabelCell("ClientName") { Value = deal.Client.Name },
                    new GridLabelCell("StageName") { Value = deal.Stage.GetDisplayName() },
                    new GridLabelCell("ExpectedBudget") { Value = deal.ExpectedBudget != null ? deal.ExpectedBudget.Value.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("SalesVolume") { Value = dealService.CalculateSaleSum(deal, user).ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("Id") { Value = deal.Id.ToString() },
                    new GridHiddenCell("CuratorId") { Value = deal.Curator.Id.ToString() },
                    new GridHiddenCell("ClientId") { Value = deal.Client.Id.ToString() }
                    ));
            }

            model.Title = "Активные сделки";

            return model;
        }

        public GridData GetClosedDealGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetClosedDealGridLocal(state, user);
            }
        }

        private GridData GetClosedDealGridLocal(GridState state, User user)
        {
            if (state == null) state = new GridState();

            var model = new GridData { State = state };
            model.AddColumn("Name", "Название", Unit.Percentage(38));
            model.AddColumn("CuratorName", "Куратор", Unit.Percentage(17));
            model.AddColumn("ClientName", "Клиент", Unit.Percentage(32));
            model.AddColumn("StageName", "Этап", Unit.Pixel(150));
            model.AddColumn("ExpectedBudget", "Ожидаемый бюджет", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("SalesVolume", "Сумма реализации", Unit.Percentage(13), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("CuratorId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ClientId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            string filter = state.Filter;
            ParameterString deriveFilter = new ParameterString(state.Filter);

            var filterStringList = new List<String> { ((byte)DealStage.ContractAbrogated).ToString(),
                                    ((byte)DealStage.DealRejection).ToString(),
                                    ((byte)DealStage.SuccessfullyClosed).ToString()};

            if (deriveFilter["Stage"] != null && !String.IsNullOrEmpty(deriveFilter["Stage"].Value as string))
            {
                deriveFilter["Stage"].Value = filterStringList.Contains<string>(deriveFilter["Stage"].Value as string) ? deriveFilter["Stage"].Value as string : "0";
            }
            else
            {
                if (deriveFilter["Stage"] != null)
                    deriveFilter.Delete("Stage");

                deriveFilter.Add("Stage", ParameterStringItem.OperationType.OneOf, filterStringList);
            }

            var deals = dealService.GetFilteredList(state, deriveFilter, user, Permission.Deal_List_Details);

            foreach (var deal in deals)
            {
                model.AddRow(new GridRow(
                    new GridLinkCell("Name") { Value = deal.Name },
                    userService.IsPossibilityToViewDetails(deal.Curator, user) ?
                        (GridCell)new GridLinkCell("CuratorName") { Value = deal.Curator.DisplayName } :
                        new GridLabelCell("CuratorName") { Value = deal.Curator.DisplayName },
                    user.HasPermission(Permission.Client_List_Details) ?
                        (GridCell)new GridLinkCell("ClientName") { Value = deal.Client.Name } :
                        new GridLabelCell("ClientName") { Value = deal.Client.Name },
                    new GridLabelCell("StageName") { Value = deal.Stage.GetDisplayName() },
                    new GridLabelCell("ExpectedBudget") { Value = deal.ExpectedBudget != null ? deal.ExpectedBudget.Value.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("SalesVolume") { Value = dealService.CalculateSaleSum(deal, user).ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("Id") { Value = deal.Id.ToString() },
                    new GridHiddenCell("CuratorId") { Value = deal.Curator.Id.ToString() },
                    new GridHiddenCell("ClientId") { Value = deal.Client.Id.ToString() }
                    ));
            }

            model.Title = "Закрытые сделки";

            return model;
        }

        #endregion

        #region Добавление / редактирование сделки

        /* clientId имеет тип int? т.к. этот параметер передается не всегда. 
         * Например, при добавлении сделки из списка сделок, клиент выбирается на форме создания сделки. */
        public DealEditViewModel Create(int? clientId, string BackURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Deal_Create_Edit);

                Client client = null;
                if (clientId != null)
                {
                    client = clientService.CheckClientExistence(clientId.Value, user);
                }

                var model = new DealEditViewModel();
                model.BackURL = BackURL;
                model.Title = "Добавление сделки";

                model.ClientName = client != null ? client.Name : "Выберите клиента";
                model.ClientId = clientId != null ? clientId.Value.ToString() : "";
                model.CuratorName = user.DisplayName;
                model.CuratorId = user.Id.ToString();

                model.StageName = DealStage.ClientInvestigation.GetDisplayName();               

                return model;
            }
        }

        public DealEditViewModel Edit(int id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Deal_Create_Edit);
                var deal = dealService.CheckDealExistence(id, user);

                var model = new DealEditViewModel();
                model.ClientId = deal.Client.Id.ToString();
                model.ClientName = deal.Client.Name;
                model.Comment = deal.Comment;
                model.CuratorName = deal.Curator.DisplayName;
                model.CuratorId = deal.Curator.Id.ToString();
                model.ExpectedBudget = deal.ExpectedBudget != null ? deal.ExpectedBudget.Value.ForEdit() : "";
                model.Id = deal.Id;
                model.Name = deal.Name;
                model.StageName = deal.Stage.GetDisplayName();
                model.Title = "Редактирование сделки";

                model.BackURL = backURL;

                return model;
            }
        }

        public int Save(DealEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var curator = userService.CheckUserExistence(ValidationUtils.TryGetInt(model.CuratorId));
                var client = clientService.CheckClientExistence(ValidationUtils.TryGetInt(model.ClientId), user);
                
                Deal deal = null;

                decimal? expectedBudget = (!String.IsNullOrEmpty(model.ExpectedBudget)) ?
                    ValidationUtils.TryGetDecimal(model.ExpectedBudget, "Введите ожидаемый бюджет в правильном формате или оставьте пустым.") :
                    (decimal?)null;

                // Добавление 
                if (model.Id == 0)
                {
                    user.CheckPermission(Permission.Deal_Create_Edit);
                    
                    deal = new Deal(model.Name, curator);
                    client.AddDeal(deal);
                }
                // Редактирование
                else
                {
                    deal = dealService.CheckDealExistence(model.Id, user);

                    dealService.CheckPossibilityToEdit(deal, user);


                    deal.Name = model.Name;
                }

                deal.ExpectedBudget = expectedBudget;
                deal.Comment = StringUtils.ToHtml(model.Comment);
                
                dealService.Save(deal, user);

                uow.Commit();

                return deal.Id;
            }
        }

        #endregion

        #region Детали сделки

        #region Детали общие

        public DealDetailsViewModel Details(int id, string BackURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(id, user);

                dealService.CheckPossibilityToViewDetails(deal, user);

                var model = new DealDetailsViewModel();

                model.Id = id.ToString();
                model.Name = deal.Name;
                model.MainDetails = GetMainDetails(deal, user);

                var allowToViewSaleList = dealService.IsPossibilityToViewSales(deal, user);
                var allowToViewReturnFromClientList = dealService.IsPossibilityToViewReturnsFromClient(deal, user);
                var allowToViewDealPaymentList = dealService.IsPossibilityToViewDealPayments(deal, user);
                var allowToViewDealInitialBalanceCorrectionList = dealPaymentDocumentService.IsPossibilityToViewDealInitialBalanceCorrections(deal, user);
                var allowToViewDealQuotaList = dealService.IsPossibilityToViewDealQuotaList(deal, user);

                if (allowToViewSaleList)
                {
                    model.SalesGrid = GetSalesGridLocal(new GridState { Parameters = "DealId=" + deal.Id.ToString(), PageSize = 5, Sort = "Date=Desc;CreationDate=Desc" }, user);
                }
                if (allowToViewReturnFromClientList)
                {
                    model.ReturnFromClientGrid = GetReturnFromClientGridLocal(new GridState() { PageSize = 5, Parameters = "DealId=" + deal.Id.ToString(), Sort = "Date=Desc;CreationDate=Desc" }, user);
                }
                if (allowToViewDealPaymentList)
                {
                    model.PaymentGrid = GetDealPaymentGridLocal(new GridState { Parameters = "DealId=" + deal.Id.ToString(), PageSize = 5, Sort = "Date=Desc;CreationDate=Desc" }, user);
                }

                if (allowToViewDealInitialBalanceCorrectionList)
                {
                    model.DealInitialBalanceCorrectionGrid = GetDealInitialBalanceCorrectionGridLocal(
                        new GridState()
                        {
                            Parameters = "DealId=" + deal.Id.ToString(),
                            PageSize = 5,
                            Sort = "Date=Desc;CreationDate=Desc"
                        }, user);
                }

                if (allowToViewDealQuotaList)
                {
                    model.QuotaGrid = GetQuotaGridLocal(new GridState { Parameters = "DealId=" + deal.Id.ToString(), PageSize = 10, Sort = "CreationDate=Desc" }, user);
                }

                model.TaskGrid = taskPresenterMediator.GetTaskGridForDeal(deal, user);

                model.AllowToEdit = dealService.IsPossibilityToEdit(deal, user);
                model.AllowToViewDealQuotaList = allowToViewDealQuotaList;
                model.AllowToViewSaleList = allowToViewSaleList;
                model.AllowToViewPaymentList = allowToViewDealPaymentList;
                model.AllowToViewReturnFromClientList = allowToViewReturnFromClientList;
                model.AllowToViewDealInitialBalanceCorrectionList = allowToViewDealInitialBalanceCorrectionList;

                return model;
            }
        }

        private DealMainDetailsViewModel GetMainDetails(Deal deal, User user)
        {
            var contract = deal.Contract;

            var allowToViewSaleSum = dealService.IsPossibilityToViewSales(deal, user);
            var allowToViewDealPaymentSum = dealService.IsPossibilityToViewDealPayments(deal, user);
            var allowToViewDealInitialBalanceCorrectionSum = dealPaymentDocumentService.IsPossibilityToViewDealInitialBalanceCorrections(deal, user);
            var allowToViewReturnFromClientSum = dealService.IsPossibilityToViewReturnsFromClient(deal, user);
            var allowToViewBalance = dealService.IsPossibilityToViewBalance(deal, user);

            var ind = dealService.CalculateMainIndicators(deal, calculateSaleSum: allowToViewSaleSum, calculateShippingPendingSaleSum: allowToViewSaleSum,
                calculateBalance: true, calculatePaymentDelayPeriod: allowToViewDealPaymentSum, calculatePaymentDelaySum: allowToViewDealPaymentSum,
                calculateReservedByReturnFromClientSum: allowToViewReturnFromClientSum, calculateReturnedFromClientSum: allowToViewReturnFromClientSum);

            var model = new DealMainDetailsViewModel()
            {
                AccountOrganizationId = contract != null ? deal.Contract.AccountOrganization.Id.ToString() : "",
                AccountOrganizationName = contract != null ? deal.Contract.AccountOrganization.ShortName : "---",
                ClientId = deal.Client.Id.ToString(),
                ClientName = deal.Client.Name,
                ClientOrganizationId = contract != null ? deal.Contract.ContractorOrganization.Id.ToString() : "",
                ClientOrganizationName = contract != null ? deal.Contract.ContractorOrganization.ShortName : "---",
                Comment = deal.Comment,
                ClientContractId = contract != null ? deal.Contract.Id.ToString() : "",
                ClientContractName = contract != null ? deal.Contract.FullName : "---",
                StartDate = deal.StartDate.ToShortDateString(),
                CuratorName = deal.Curator.DisplayName,
                CuratorId = deal.Curator.Id.ToString(),
                ExpectedBudget = deal.ExpectedBudget != null ? deal.ExpectedBudget.Value.ForDisplay(ValueDisplayType.Money) : "---",
                InitialBalance = allowToViewDealInitialBalanceCorrectionSum ? deal.InitialBalance.ForDisplay(ValueDisplayType.Money) : "---",
                MaxPaymentDelayDuration = (allowToViewDealPaymentSum ? ind.PaymentDelayPeriod.ForDisplay() : "---"),
                Name = deal.Name,
                PaymentDelaySum = (allowToViewDealPaymentSum ? ind.PaymentDelaySum.ForDisplay(ValueDisplayType.Money) : "---"),
                Balance = (allowToViewBalance ? ind.Balance.ForDisplay(ValueDisplayType.Money) : "---"),
                ShippingPendingSaleSum = (allowToViewSaleSum ? ind.ShippingPendingSaleSum.ForDisplay(ValueDisplayType.Money) : "---"),
                SaleSum = (allowToViewSaleSum ? ind.SaleSum.ForDisplay(ValueDisplayType.Money) : "---"),
                StageStartDate = deal.StageDate.ToShortDateString(),
                StageDuration = deal.CurrentStageDuration.ForDisplay(),
                StageName = deal.Stage.GetDisplayName(),
                StageId = deal.Stage.ValueToString(),
                PaymentSum = (allowToViewDealPaymentSum ? deal.DealPaymentSum.ForDisplay(ValueDisplayType.Money) : "---"),
                TotalReturnedSum = (allowToViewReturnFromClientSum ? ind.ReturnedFromClientSum.ForDisplay(ValueDisplayType.Money) : "---"),
                TotalReservedByReturnSum = (allowToViewReturnFromClientSum ? ind.ReservedByReturnFromClientSum.ForDisplay(ValueDisplayType.Money) : "---"),

                AllowToAddContract = dealService.IsPossibilityToAddContract(deal, user),
                AllowToChangeContract = dealService.IsPossibilityToChangeContract(deal, user),
                AllowToViewCuratorDetails = userService.IsPossibilityToViewDetails(deal.Curator, user),
                AllowToViewClientDetails = user.HasPermission(Permission.Client_List_Details),
                AllowToViewClientOrganizationDetails = user.HasPermission(Permission.ClientOrganization_List_Details),
                AllowToChangeStage = dealService.IsPossibilityToChangeStage(deal, user)
            };

            return model;
        }

        #endregion

        #region Гриды

        #region Квоты по сделке

        public GridData GetQuotaGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetQuotaGridLocal(state, user);
            }
        }

        private GridData GetQuotaGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            var model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(90));
            model.AddColumn("QuotaName", "Название квоты", Unit.Percentage(100));
            model.AddColumn("Date", "Дата", Unit.Pixel(70));
            model.AddColumn("TypeName", "Тип", Unit.Pixel(95));
            model.AddColumn("PostPaymentDays", "Отсрочка платежа", Unit.Pixel(110), align: GridColumnAlign.Right);
            model.AddColumn("CreditLimit", "Кредитный лимит", Unit.Pixel(120), align: GridColumnAlign.Right);
            model.AddColumn("DiscountPercent", "Скидка", Unit.Pixel(65), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var deal = dealService.CheckDealExistence(ValidationUtils.TryGetInt(deriveParams["DealId"].Value.ToString()), user);

            model.ButtonPermissions["AllowToAddQuota"] = dealService.IsPossibilityToAddQuota(deal, user);

            foreach (var quota in GridUtils.GetEntityRange(deal.Quotas.OrderByDescending(x => x.CreationDate), state))
            {
                var actions = new GridActionCell("Action");
                if (dealService.IsPossibilityToRemoveQuota(deal, quota, user))
                {
                    actions.AddAction("Удал. из списка", "delete_link");
                }

                model.AddRow(new GridRow(
                    actions.ActionCount > 0 ? (GridCell)actions : new GridLabelCell("Action"),
                    new GridLabelCell("QuotaName") { Value = quota.Name },
                    new GridLabelCell("Date") { Value = quota.StartDate.ToShortDateString() },
                    new GridLabelCell("TypeName") { Value = quota.IsActive ? "Действует" : "Закрыта" },
                    new GridLabelCell("PostPaymentDays") { Value = quota.PostPaymentDays.HasValue ? quota.PostPaymentDays.Value.ForDisplay() : "---" },
                    new GridLabelCell("CreditLimit") { Value = quota.CreditLimitSum.HasValue ? quota.CreditLimitSum.Value.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("DiscountPercent") { Value = quota.DiscountPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = quota.Id.ToString(), Key = "quotaId" }
                    ));
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Оплаты по сделке

        public GridData GetDealPaymentGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetDealPaymentGridLocal(state, user);
            }
        }

        private GridData GetDealPaymentGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            var model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("Date", "Дата", Unit.Pixel(54), align: GridColumnAlign.Center);
            model.AddColumn("PaymentDocumentNumber", "Номер документа", Unit.Percentage(50));
            model.AddColumn("DealPaymentForm", "Форма оплаты", Unit.Percentage(50));
            model.AddColumn("Sum", "Сумма оплаты", Unit.Pixel(125), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var dealId = ValidationUtils.TryGetInt(deriveParams["DealId"].Value.ToString());
            var deal = dealService.CheckDealExistence(dealId, user);

            model.ButtonPermissions["AllowToCreateDealPaymentFromClient"] = dealPaymentDocumentService.IsPossibilityToCreateDealPaymentFromClient(deal, user);
            model.ButtonPermissions["AllowToCreateDealPaymentToClient"] = dealPaymentDocumentService.IsPossibilityToCreateDealPaymentToClient(deal, user);

            var ps = new ParameterString("");            
            ps.Add("Deal", ParameterStringItem.OperationType.Eq, deal.Id.ToString());

            var payments = dealPaymentDocumentService.GetDealPaymentFilteredList(state, ps, user);

            // Подгружаем коллекции разнесений документов, т.к. они обязательно будут использоваться
            dealPaymentDocumentService.LoadDealPaymentDocumentDistributions(payments);

            foreach (var payment in payments)
            {
                var actions = new GridActionCell("Action");
                if (payment.Type == DealPaymentDocumentType.DealPaymentFromClient)
                {
                    actions.AddAction("Дет.", "linkPaymentFromClientDetails");

                    if (dealPaymentDocumentService.IsPossibilityToRedistribute(payment.As<DealPaymentFromClient>(), user))
                    {
                        actions.AddAction("Ред.", "linkPaymentFromClientEdit");
                    }

                    if (dealPaymentDocumentService.IsPossibilityToDelete(payment.As<DealPaymentFromClient>(), user))
                    {
                        actions.AddAction("Удал.", "linkPaymentFromClientDelete");
                    }
                }
                else if (payment.Type == DealPaymentDocumentType.DealPaymentToClient)
                {
                    actions.AddAction("Дет.", "linkPaymentToClientDetails");

                    if (dealPaymentDocumentService.IsPossibilityToDelete(payment.As<DealPaymentToClient>(), user))
                    {
                        actions.AddAction("Удал.", "linkPaymentToClientDelete");
                    }
                }

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Date") { Value = payment.Date.ToShortDateString() },
                    new GridLabelCell("PaymentDocumentNumber") { Value = payment.PaymentDocumentNumber },
                    new GridLabelCell("DealPaymentForm") { Value = payment.DealPaymentForm.GetDisplayName() },
                    new GridLabelCell("Sum") { Value = (payment.Type == DealPaymentDocumentType.DealPaymentFromClient ? payment.Sum : -payment.Sum).ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("Id") { Value = payment.Id.ToString(), Key = "PaymentId" }
                    ) { Style = payment.Type == DealPaymentDocumentType.DealPaymentToClient ? GridRowStyle.Warning : GridRowStyle.Normal });
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Реализации по сделке

        public GridData GetSalesGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetSalesGridLocal(state, user);
            }
        }

        private GridData GetSalesGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            var model = new GridData { State = state };
            model.AddColumn("Number", "Номер документа", Unit.Pixel(80), GridCellStyle.Link);
            model.AddColumn("Date", "Дата", Unit.Pixel(40));
            model.AddColumn("AccountingPriceSum", "Сумма в УЦ", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("DiscountPercent", "Скидка", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("SalePriceSum", "Итоговая сумма", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("ReturnFromClientSum", "Сумма возвращенного товара", Unit.Pixel(130), align: GridColumnAlign.Right);
            model.AddColumn("StateName", "Статус", Unit.Percentage(30));
            model.AddColumn("StorageName", "Место хранения", Unit.Percentage(70), GridCellStyle.Link);
            model.AddColumn("PaymentPercent", "Оплата", Unit.Pixel(50), align: GridColumnAlign.Right);
            model.AddColumn("ExpenditureWaybillId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("StorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString dp = new ParameterString(state.Parameters);
            var dealId = ValidationUtils.TryGetInt(dp["DealId"].Value.ToString());
            var deal = dealService.CheckDealExistence(dealId, user);

            ParameterString param = new ParameterString("");
            param.Add("Deal", ParameterStringItem.OperationType.Eq, dealId.ToString());

            var sales = expenditureWaybillService.GetFilteredList(state, user, param);

            string stateName, storageName;
            decimal? senderAccountingPriceSum = 0M, salePriceSum = 0M, paymentPercent = 0M, totalDiscountPercent = 0M, totalReturnedSum = 0M;

            model.ButtonPermissions["AllowToCreateExpenditureWaybill"] = dealService.IsPossibilityToCreateExpenditureWaybill(deal, user);
            model.ButtonPermissions["IsPossibilityToCreateExpenditureWaybill"] = dealService.IsPossibilityToCreateExpenditureWaybill(deal, user, false);

            foreach (var sale in sales)
            {
                stateName = "---"; storageName = "---";
                var expenditureWaybill = sale.As<ExpenditureWaybill>();

                var allowToViewAccPrices = user.HasPermissionToViewStorageAccountingPrices(expenditureWaybill.SenderStorage);

                if (sale.Is<ExpenditureWaybill>())
                {
                    var allowToViewPayments = expenditureWaybillService.IsPossibilityToViewPayments(expenditureWaybill, user);
                    var ind = expenditureWaybillService.GetMainIndicators(expenditureWaybill, calculateSenderAccountingPriceSum: allowToViewAccPrices,
                    calculateSalePriceSum: true, calculatePaymentPercent: allowToViewPayments, calculateTotalDiscount: allowToViewAccPrices, calculateTotalReturnedSum: true);

                    senderAccountingPriceSum = allowToViewAccPrices ? ind.SenderAccountingPriceSum : (decimal?)null;
                    salePriceSum = ind.SalePriceSum;
                    paymentPercent = allowToViewPayments ? ind.PaymentPercent : (decimal?)null;
                    totalDiscountPercent = allowToViewAccPrices ? ind.TotalDiscountPercent : (decimal?)null;
                    totalReturnedSum = ind.TotalReturnedSum;

                    stateName = expenditureWaybill.State.GetDisplayName();
                    storageName = expenditureWaybill.SenderStorage.Name;
                }

                model.AddRow(new GridRow(
                    new GridLinkCell("Number") { Value = sale.Number.PadLeftZeroes(8) },
                    new GridLabelCell("Date") { Value = sale.Date.ToShortDateString() },
                    new GridLabelCell("AccountingPriceSum")
                    {
                        Value = (user.HasPermissionToViewStorageAccountingPrices(sale.SenderStorage) ?
                            senderAccountingPriceSum.ForDisplay(ValueDisplayType.Money) : "---")
                    },
                    new GridLabelCell("DiscountPercent") { Value = totalDiscountPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridLabelCell("SalePriceSum") { Value = salePriceSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("ReturnFromClientSum") { Value = totalReturnedSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("StateName") { Value = stateName },
                    storageService.IsPossibilityToViewDetails(sale.SenderStorage, user) ?
                        (GridCell)new GridLinkCell("StorageName") { Value = storageName } :
                        new GridLabelCell("StorageName") { Value = storageName },
                    new GridLabelCell("PaymentPercent") { Value = (paymentPercent.ForDisplay(ValueDisplayType.Percent) + (paymentPercent.HasValue ? " %" : "")) },
                    new GridHiddenCell("ExpenditureWaybillId") { Value = expenditureWaybill.Id.ToString() },
                    new GridHiddenCell("StorageId") { Value = expenditureWaybill.SenderStorage.Id.ToString() }));
            }

            model.Title = "Реализации по сделке";
            model.GridPartialViewAction = "/Deal/ShowSalesGrid/";

            return model;
        }

        #endregion

        #region Возвраты по сделке

        public GridData GetReturnFromClientGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetReturnFromClientGridLocal(state, user);
            }
        }

        private GridData GetReturnFromClientGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            var model = new GridData();
            model.AddColumn("Number", "Номер", Unit.Pixel(60), GridCellStyle.Link);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Date", "Дата", Unit.Pixel(54));
            model.AddColumn("SalePriceSum", "Сумма в ОЦ", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("AccountingPriceSum", "Сумма в УЦ приемщика", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("StateName", "Статус", Unit.Pixel(120));
            model.AddColumn("RecipientStorageName", "Место хранения-приемщик", Unit.Percentage(50));
            model.AddColumn("RecipientStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("RecipientName", "Организация-приемщик", Unit.Percentage(50));
            model.AddColumn("ShippingPercent", "Отгрузка", Unit.Pixel(50), align: GridColumnAlign.Right);
            model.AddColumn("RecipientId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString dp = new ParameterString(state.Parameters);
            var dealId = ValidationUtils.TryGetInt(dp["DealId"].Value.ToString());
            var deal = dealService.CheckDealExistence(dealId, user, Permission.ReturnFromClientWaybill_List_Details);

            model.ButtonPermissions["AllowToCreateReturnFromClientWaybill"] = dealService.IsPossibilityToCreateReturnFromClientWaybill(deal, user);
            model.ButtonPermissions["IsPossibilityToCreateReturnFromClientWaybill"] = dealService.IsPossibilityToCreateReturnFromClientWaybill(deal, user, false);

            var ps = new ParameterString("");
            ps.Add("Deal", ParameterStringItem.OperationType.Eq, deal.Id.ToString());

            var list = returnFromClientWaybillService.GetFilteredList(state, user, ps);

            foreach (var row in list)
            {
                decimal? accountingPriceSum = null;

                if (user.HasPermissionToViewStorageAccountingPrices(row.RecipientStorage))
                {
                    accountingPriceSum = returnFromClientWaybillIndicatorService.CalculateAccountingPriceSum(row);
                }

                model.AddRow(new GridRow(
                    new GridLinkCell("Number") { Value = StringUtils.PadLeftZeroes(row.Number, 8) },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("SalePriceSum") { Value = row.SalePriceSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("AccountingPriceSum") { Value = accountingPriceSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("StateName") { Value = row.State.GetDisplayName() },
                    storageService.IsPossibilityToViewDetails(row.RecipientStorage, user) ?
                        (GridCell)new GridLinkCell("RecipientStorageName") { Value = row.RecipientStorage.Name } :
                        new GridLabelCell("RecipientStorageName") { Value = row.RecipientStorage.Name },
                    new GridHiddenCell("RecipientStorageId") { Value = row.RecipientStorage.Id.ToString() },
                    new GridLinkCell("RecipientName") { Value = row.Recipient.ShortName },
                    new GridLabelCell("ShippingPercent") { Value = returnFromClientWaybillIndicatorService.CalculateShippingPercent(row).ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("RecipientId") { Value = row.Recipient.Id.ToString() }
                ));
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Корректировки сальдо по сделке

        #region Грид

        public GridData GetDealInitialBalanceCorrectionGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetDealInitialBalanceCorrectionGridLocal(state, user);
            }
        }

        private GridData GetDealInitialBalanceCorrectionGridLocal(GridState state, User user)
        {
            user.CheckPermission(Permission.DealInitialBalanceCorrection_List_Details);

            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            var model = new GridData() { State = state, Title = "Корректировки сальдо по сделке" };

            model.AddColumn("Action", "Действие", Unit.Pixel(70), GridCellStyle.Action);
            model.AddColumn("Date", "Дата", Unit.Pixel(54), align: GridColumnAlign.Center);
            model.AddColumn("Reason", "Причина корректировки", Unit.Percentage(100));           
            model.AddColumn("Sum", "Сумма корректировки", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("PaymentPercent", "Процент оплаты", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("CorrectionId", style: GridCellStyle.Hidden);

            ParameterString dp = new ParameterString(state.Parameters);
            var dealId = ValidationUtils.TryGetInt(dp["DealId"].Value.ToString());
            var deal = dealService.CheckDealExistence(dealId, user);

            model.ButtonPermissions["AllowToCreateDealDebitInitialBalanceCorrection"] = dealPaymentDocumentService.IsPossibilityToCreateDealDebitInitialBalanceCorrection(deal, user);
            model.ButtonPermissions["AllowToCreateDealCreditInitialBalanceCorrection"] = dealPaymentDocumentService.IsPossibilityToCreateDealCreditInitialBalanceCorrection(deal, user);

            var ps = new ParameterString("");
            ps.Add("Deal", ParameterStringItem.OperationType.Eq, deal.Id.ToString());

            var corrections = dealPaymentDocumentService.GetDealInitialBalanceCorrectionFilteredList(state, ps, user);

            // Подгружаем коллекции разнесений документов, т.к. они обязательно будут использоваться
            dealPaymentDocumentService.LoadDealPaymentDocumentDistributions(corrections);

            foreach (var correction in corrections)
            {
                var actions = new GridActionCell("Action");
                if (correction.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection)
                {
                    actions.AddAction("Дет.", "linkDealDebitInitialBalanceCorrectionDetails");

                    if (dealPaymentDocumentService.IsPossibilityToDelete(correction, user))
                    {
                        actions.AddAction("Удал.", "linkDealDebitInitialBalanceCorrectionDelete");
                    }
                }
                else if (correction.Type == DealPaymentDocumentType.DealCreditInitialBalanceCorrection)
                {
                    actions.AddAction("Дет.", "linkDealCreditInitialBalanceCorrectionDetails");

                    if (dealPaymentDocumentService.IsPossibilityToRedistribute(correction.As<DealCreditInitialBalanceCorrection>(), user))
                    {
                        actions.AddAction("Ред.", "linkDealCreditInitialBalanceCorrectionEdit");
                    }

                    if (dealPaymentDocumentService.IsPossibilityToDelete(correction, user))
                    {
                        actions.AddAction("Удал.", "linkDealCreditInitialBalanceCorrectionDelete");
                    }
                }

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Date") { Value = correction.Date.ToShortDateString() },
                    new GridLabelCell("Reason") { Value = correction.CorrectionReason },
                    new GridLabelCell("Sum") { Value = (correction.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection ? correction.Sum : -correction.Sum).ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PaymentPercent") { Value = correction.PaymentPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("CorrectionId") { Value = correction.Id.ToString() }                     
                    ) { Style = correction.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection ? GridRowStyle.Normal : GridRowStyle.Warning });
            }

            return model;
        }

        #endregion

        #region Создание

        /// <summary>
        /// Кредитовая корректировка
        /// </summary>
        public DealCreditInitialBalanceCorrectionEditViewModel CreateDealCreditInitialBalanceCorrection(int dealId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealCreditInitialBalanceCorrection_Create_Edit);    //Проверяем право на создание корректировки

                var deal = dealService.CheckDealExistence(dealId, user);
                var client = deal.Client;

                var model = new DealCreditInitialBalanceCorrectionEditViewModel();

                model.Title = "Добавление кредитовой корректировки";
                model.DealName = deal.Name;
                model.DealId = deal.Id;
                model.ClientName = client.Name;
                model.ClientId = client.Id;
                model.Date = DateTime.Today.ToShortDateString();

                model.DestinationDocumentSelectorControllerName = "Deal";
                model.DestinationDocumentSelectorActionName = "SaveDealCreditInitialBalanceCorrection";

                model.AllowToViewClientOrganization = false;
                model.AllowToViewClient = true;
                model.AllowToChooseClient = false;
                model.AllowToChooseDeal = false;
                model.IsDealSelectedByClient = true;
                model.AllowToChangeDate = user.HasPermission(Permission.DealInitialBalanceCorrection_Date_Change);

                return model;
            }
        }

        /// <summary>
        /// Дебетовая корректировка
        /// </summary>
        public DealDebitInitialBalanceCorrectionEditViewModel CreateDealDebitInitialBalanceCorrection(int dealId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealDebitInitialBalanceCorrection_Create);    //Проверяем право на создание корректировки

                var deal = dealService.CheckDealExistence(dealId, user);
                var client = deal.Client;

                var model = new DealDebitInitialBalanceCorrectionEditViewModel();

                model.Title = "Добавление дебетовой корректировки";
                model.DealName = deal.Name;
                model.DealId = deal.Id;

                var teamList = dealService.GetTeamListForDealDocumentByDeal(deal, user);
                model.TeamList = ComboBoxBuilder.GetComboBoxItemList(teamList, x => x.Name, x => x.Id.ToString(), teamList.Count() != 1);

                model.ClientName = client.Name;
                model.ClientId = client.Id;
                model.Date = DateTime.Today.ToShortDateString();

                model.ControllerName = "Deal";
                model.ActionName = "SaveDealDebitInitialBalanceCorrection";

                model.AllowToViewClientOrganization = false;
                model.AllowToViewClient = true;
                model.AllowToChooseClient = false;
                model.AllowToChooseDeal = false;
                model.IsDealSelectedByClient = true;
                model.AllowToChangeDate = user.HasPermission(Permission.DealInitialBalanceCorrection_Date_Change);

                return model;
            }
        }

        #endregion

        #endregion

        #region Задачи

        /// <summary>
        /// Получение модели для грида задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public GridData GetTaskGrid(GridState state, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetTaskGridLocal(state, user);
            }
        }

        /// <summary>
        /// Формирование грида задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private GridData GetTaskGridLocal(GridState state, User user)
        {
            return taskPresenterMediator.GetTaskGridForDeal(state, user);
        }

        #endregion

        #endregion

        #endregion

        #region Изменение этапа сделки

        public DealChangeStageViewModel ChangeStage(int dealId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);

                dealService.CheckPossibilityToChangeStage(deal, user);

                var model = new DealChangeStageViewModel()
                {
                    Title = "Изменение этапа сделки",
                    DealId = dealId.ToString(),

                    CurrentStageName = deal.Stage.GetDisplayName(),
                    CurrentStageId = deal.Stage.ValueToString(),
                    CurrentStageStartDate = deal.StageDate.ToShortDateString(),
                    CurrentStageDuration = deal.CurrentStageDuration.ForDisplay(),

                    NextStageName = deal.NextStage.HasValue ? deal.NextStage.Value.GetDisplayName() : "---",
                    PreviousStageName = deal.PreviousStage.HasValue ? deal.PreviousStage.Value.GetDisplayName() : "---",
                    UnsuccessfulClosingStageName = deal.UnsuccessfulClosingStage.HasValue ? deal.UnsuccessfulClosingStage.Value.GetDisplayName() :
                         (deal.Stage.ContainsIn(DealStage.SuccessfullyClosed, DealStage.ContractAbrogated)) ? DealStage.ContractAbrogated.GetDisplayName() :
                         DealStage.DealRejection.GetDisplayName(),
                    DecisionMakerSearchStageName = DealStage.DecisionMakerSearch.GetDisplayName(),

                    AllowToMoveToNextStage = dealService.IsPossibilityToMoveToNextStage(deal, user),
                    AllowToMoveToPreviousStage = dealService.IsPossibilityToMoveToPreviousStage(deal, user),
                    AllowToMoveToUnsuccessfulClosingStage = dealService.IsPossibilityToMoveToUnsuccessfulClosingStage(deal, user),
                    AllowToMoveToDecisionMakerSearchStage = dealService.IsPossibilityToMoveToDecisionMakerSearchStage(deal, user)
                };

                return model;
            }
        }

        public object MoveToNextStage(int dealId, byte currentStageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);

                DealStage currentStage = (DealStage)currentStageId;
                ValidationUtils.Assert(Enum.IsDefined(typeof(DealStage), currentStage), "Текущий этап сделки не указан.");
                ValidationUtils.Assert(deal.Stage == currentStage,
                    String.Format("Невозможно перейти на следующий этап, так как текущий этап «{0}».", deal.Stage.GetDisplayName()));

                dealService.MoveToNextStage(deal, user);

                uow.Commit();

                return GetMainChangeableIndicators(deal, user);
            }
        }

        public object MoveToPreviousStage(int dealId, byte currentStageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);

                DealStage currentStage = (DealStage)currentStageId;
                ValidationUtils.Assert(Enum.IsDefined(typeof(DealStage), currentStage), "Текущий этап сделки не указан.");
                ValidationUtils.Assert(deal.Stage == currentStage,
                    String.Format("Невозможно перейти на предыдущий этап, так как текущий этап «{0}».", deal.Stage.GetDisplayName()));

                dealService.MoveToPreviousStage(deal, user);

                uow.Commit();

                return GetMainChangeableIndicators(deal, user);
            }
        }

        public object MoveToUnsuccessfulClosingStage(int dealId, byte currentStageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);

                // Проверка на существование этапа, чтобы правильно вывести сообщение об ошибке
                dealService.CheckPossibilityToMoveToUnsuccessfulClosingStage(deal, user);

                DealStage currentStage = (DealStage)currentStageId;
                DealStage unsuccessfulClosingStage = deal.UnsuccessfulClosingStage.Value;
                ValidationUtils.Assert(Enum.IsDefined(typeof(DealStage), currentStage), "Текущий этап сделки не указан.");
                ValidationUtils.Assert(deal.Stage == currentStage,
                    String.Format("Невозможно перейти на этап «{0}», так как текущий этап «{1}».",
                    unsuccessfulClosingStage.GetDisplayName(), deal.Stage.GetDisplayName()));

                dealService.MoveToUnsuccessfulClosingStage(deal, user);

                uow.Commit();

                return GetMainChangeableIndicators(deal, user);
            }
        }

        public object MoveToDecisionMakerSearchStage(int dealId, byte currentStageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);

                DealStage currentStage = (DealStage)currentStageId;
                ValidationUtils.Assert(Enum.IsDefined(typeof(DealStage), currentStage), "Текущий этап сделки не указан.");
                ValidationUtils.Assert(deal.Stage == currentStage,
                    String.Format("Невозможно перейти на этап «{0}», так как текущий этап «{1}».",
                    DealStage.DecisionMakerSearch.GetDisplayName(), deal.Stage.GetDisplayName()));

                dealService.MoveToDecisionMakerSearchStage(deal, user);

                uow.Commit();

                return GetMainChangeableIndicators(deal, user);
            }
        }

        #endregion

        #region Работа с квотами

        public object AddQuota(int dealId, int dealQuotaId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);
                var dealQuota = dealQuotaService.CheckDealQuotaExistence(dealQuotaId, user);

                dealService.AddQuota(deal, dealQuota, user);

                uow.Commit();

                return GetMainChangeableIndicators(deal, user);
            }
        }

        public object AddAllQuotas(int dealId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);

                var dealQuotas = dealQuotaService.GetActiveDealQuotasList(user);

                var addedCount = dealQuotas.Except(deal.Quotas).Count();

                dealService.AddQuotas(deal, dealQuotas, user);

                uow.Commit();

                return new { Indicators = GetMainChangeableIndicators(deal, user), AddedCount = addedCount };
            }
        }

        public object RemoveQuota(int dealId, int quotaId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);
                var quota = dealService.CheckDealQuotaExistence(deal, quotaId);

                dealService.RemoveQuota(deal, quota, user);

                uow.Commit();

                return GetMainChangeableIndicators(deal, user);
            }
        }

        #endregion

        #region Выбор сделки

        public DealSelectViewModel SelectDealByClient(int clientId, string mode, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new DealSelectViewModel();
                model.Title = "Выбор сделки";

                model.FilterData.Items.Add(new FilterTextEditor("Name", "Название"));
                model.FilterData.Items.Add(new FilterComboBox("Stage", "Этап", ComboBoxBuilder.GetComboBoxItemList<DealStage>(sort: false)));
                model.FilterData.Items.Add(new FilterTextEditor("Client_Name", "Клиент"));
                model.FilterData.Items.Add(new FilterTextEditor("Curator_DisplayName", "Куратор"));

                model.DealsGrid = GetSelectDealGridLocal(new GridState() { PageSize = 5, Parameters = "ClientId=" + clientId.ToString() + ";Mode=" + mode, Sort = "Name=Asc" }, user);

                return model;
            }
        }

        public DealSelectViewModel SelectDealByTeam(short teamId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var team = teamService.CheckTeamExistence(teamId, user);

                teamService.CheckPossibilityToAddDeal(team, user);

                var model = new DealSelectViewModel();
                model.Title = "Добавление сделки в область видимости команды";

                model.FilterData.Items.Add(new FilterTextEditor("Name", "Отображаемое имя"));
                model.FilterData.Items.Add(new FilterComboBox("Stage", "Этап", ComboBoxBuilder.GetComboBoxItemList<DealStage>(sort: false)));
                model.FilterData.Items.Add(new FilterTextEditor("Client_Name", "Клиент"));
                model.FilterData.Items.Add(new FilterTextEditor("Curator_DisplayName", "Куратор"));

                model.DealsGrid = GetSelectDealGridLocal(new GridState() { PageSize = 5, Parameters = "TeamId=" + teamId.ToString(), Sort = "Name=Asc" }, user);

                return model;
            }
        }

        public DealSelectViewModel SelectDeal(bool activeOnly, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var ps = new ParameterString("");
                ps.Add("ShowOption", ParameterStringItem.OperationType.Eq, activeOnly? "Active":"All");

                var model = new DealSelectViewModel();
                model.Title = "Выбор сделки";

                model.FilterData.Items.Add(new FilterTextEditor("Name", "Отображаемое имя"));
                model.FilterData.Items.Add(new FilterComboBox("Stage", "Этап", ComboBoxBuilder.GetComboBoxItemList<DealStage>(sort: false)));
                model.FilterData.Items.Add(new FilterTextEditor("Client_Name", "Клиент"));
                model.FilterData.Items.Add(new FilterTextEditor("Curator_DisplayName", "Куратор"));

                model.DealsGrid = GetSelectDealGridLocal(new GridState() { PageSize = 5, Parameters = ps.ToString(), Sort = "Name=Asc" }, user);

                return model;
            }
        }

        public DealSelectViewModel SelectDealByClientOrganization(int clientOrganizationId, string mode, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new DealSelectViewModel();
                model.Title = "Выбор сделки";

                model.FilterData.Items.Add(new FilterTextEditor("Name", "Отображаемое имя"));
                model.FilterData.Items.Add(new FilterComboBox("Stage", "Этап", ComboBoxBuilder.GetComboBoxItemList<DealStage>(sort: false)));
                model.FilterData.Items.Add(new FilterTextEditor("Client_Name", "Клиент"));
                model.FilterData.Items.Add(new FilterTextEditor("Curator_DisplayName", "Куратор"));

                model.DealsGrid = GetSelectDealGridLocal(new GridState() { PageSize = 5, Parameters = "ClientOrganizationId=" + clientOrganizationId.ToString() + ";Mode=" + mode, Sort = "Name=Asc" }, user);

                return model;
            }
        }

        private GridData GetSelectDealGridLocal(GridState state, User user)
        {
            IEnumerable<Deal> rows;
            GridData model = new GridData() { State = state, GridPartialViewAction = "/Deal/ShowDealSelectGrid" };

            model.AddColumn("Action", "Действие", Unit.Pixel(60));
            model.AddColumn("Name", "Название", Unit.Percentage(40));
            model.AddColumn("ContractName", "Договор", Unit.Percentage(60));
            model.AddColumn("AccountOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("AccountOrganizationName", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var deriveParams = new ParameterString(state.Parameters);

            string gridTitle = "Список сделок";
            var deriveFilter = new ParameterString(state.Filter);

            var listPermission = Permission.Deal_List_Details;

            if (deriveParams["TeamId"] != null)
            {
                var team = teamService.CheckTeamExistence(ValidationUtils.TryGetShort(deriveParams["TeamId"].Value.ToString()), user);
                gridTitle = "Сделки, доступные для добавления в область видимости команды";

                if (team != null && team.Deals.Count() > 0)
                {
                    deriveFilter.Add("Id", ParameterStringItem.OperationType.NotOneOf);
                    var ignoreValue = new List<string>();
                    if (team != null)
                    {
                        foreach (var u in team.Deals)
                        {
                            ignoreValue.Add(u.Id.ToString());
                        }
                    }

                    deriveFilter["Id"].Value = ignoreValue;
                }
            }
            if (deriveParams["ClientId"] != null)
            {
                var client = clientService.CheckClientExistence(ValidationUtils.TryGetInt(deriveParams["ClientId"].Value.ToString()), user);
                gridTitle = "Сделки клиента";
                deriveFilter.Add("Client", ParameterStringItem.OperationType.Eq, client.Id.ToString());

                listPermission = ProcessDealSelectionMode(deriveParams, deriveFilter, listPermission);
            }
            if (deriveParams["ClientOrganizationId"] != null)
            {
                deriveFilter.Add("Contract_ContractorOrganization_Id", ParameterStringItem.OperationType.Eq, deriveParams["ClientOrganizationId"].Value as string);

                listPermission = ProcessDealSelectionMode(deriveParams, deriveFilter, listPermission);
            }
            if (deriveParams["ShowOption"] != null)
            {
                if (deriveParams["ShowOption"].Value.ToString() == "Active")
                {
                    deriveFilter.Add("IsClosed", ParameterStringItem.OperationType.Eq, "false");
                }
                listPermission = ProcessDealSelectionMode(deriveParams, deriveFilter, listPermission);
            }
            rows = dealService.GetFilteredList(state, deriveFilter, user, listPermission);

            var actions = new GridActionCell("Action");
            actions.AddAction("Выбрать", "select_deal");

            foreach (var row in rows)
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Name") { Value = row.Name },
                    new GridLabelCell("ContractName")
                    {
                        Value = row.Contract == null ? "---" :
                            row.Contract.FullName + ", " + row.Contract.AccountOrganization.ShortName + ", " + row.Contract.ContractorOrganization.ShortName
                    },
                    new GridHiddenCell("AccountOrganizationId") { Value = row.Contract != null ? row.Contract.AccountOrganization.Id.ToString() : "0" },
                    new GridHiddenCell("AccountOrganizationName") { Value = row.Contract != null ? row.Contract.AccountOrganization.ShortName : "---" },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                ));
            }

            model.Title = gridTitle;

            return model;
        }

        public GridData GetSelectDealGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                
                return GetSelectDealGridLocal(state, user);
            }
        }


        /// <summary>
        /// Обработка параметра Mode - режима фильтрации сделок
        /// </summary>        
        private static Permission ProcessDealSelectionMode(ParameterString deriveParams, ParameterString deriveFilter, Permission listPermission)
        {
            if (deriveParams["Mode"] != null)
            {
                string modeString = deriveParams["Mode"].Value.ToString();

                switch (modeString)
                {
                    case "ForPaymentFromClient":
                        listPermission = Permission.DealPaymentFromClient_Create_Edit;
                        break;
                    case "ForPaymentToClient":
                        listPermission = Permission.DealPaymentToClient_Create;
                        break;
                    case "ForDealDebitInitialBalanceCorrection":
                        listPermission = Permission.DealDebitInitialBalanceCorrection_Create;
                        break;
                    case "ForDealCreditInitialBalanceCorrection":
                        listPermission = Permission.DealCreditInitialBalanceCorrection_Create_Edit;
                        break;
                    case "ForSaleToClient":
                        listPermission = Permission.ExpenditureWaybill_Create_Edit;
                        break;
                    case "ForReturnFromClient":
                        listPermission = Permission.ReturnFromClientWaybill_Create_Edit;
                        break;
                    case "ForTask":
                        listPermission = Permission.Deal_List_Details;
                        break;
                    default:
                        throw new Exception("Неизвестный режим выбора сделки.");
                };

                switch (modeString)
                {
                    case "ForPaymentFromClient":
                    case "ForPaymentToClient":
                    case "ForDealDebitInitialBalanceCorrection":
                    case "ForDealCreditInitialBalanceCorrection":
                        // выбираем только сделки со статусом «Исполнение договора» или «Закрытие договора»
                        deriveFilter.Add("Stage", ParameterStringItem.OperationType.OneOf,
                            new List<string>() { DealStage.ContractExecution.ValueToString(), DealStage.ContractClosing.ValueToString() });
                        break;
                    case "ForSaleToClient":
                        // выбираем только сделки со статусом «Исполнение договора»
                        deriveFilter.Add("Stage", ParameterStringItem.OperationType.Eq, DealStage.ContractExecution.ValueToString());
                        break;
                    case "ForReturnFromClient":
                        // выбираем только сделки со статусом «Исполнение договора», «Закрытие договора», «Успешно закрыто», «Договор расторгнут»
                        deriveFilter.Add("Stage", ParameterStringItem.OperationType.OneOf,
                            new List<string>() { DealStage.ContractExecution.ValueToString(), DealStage.ContractClosing.ValueToString(), 
                            DealStage.SuccessfullyClosed.ValueToString(), DealStage.ContractAbrogated.ValueToString() });
                        break;
                    case "ForTask":
                        // выбираем только сделки со статусом «Исполнение договора» или «Закрытие договора»
                        deriveFilter.Add("Stage", ParameterStringItem.OperationType.NotOneOf,
                            new List<string>() { DealStage.ContractAbrogated.ValueToString(), DealStage.DealRejection.ValueToString(), 
                                DealStage.SuccessfullyClosed.ValueToString() });
                        break;
                    default:
                        throw new Exception("Неизвестный режим выбора сделки.");
                };
            }

            return listPermission;
        }

        /// <summary>
        /// Получение данных по сделке (список МХ, адрес организации покупателя, максимально допустимая сумма оплат наличными денежными средствами).
        /// (Необходим при создании накладной реализации товаров из деталей клиента, а также при вводе оплаты от клиента наличными денежными средствами по сделке)
        /// </summary>
        /// <param name="dealId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public object GetDealInfo(int dealId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);

                dealService.CheckPossibilityToCreateExpenditureWaybill(deal, user);

                // получаем список команд, в которые можно дабавить накладную
                var teamList = dealService.GetTeamListForDealDocumentByDeal(deal, user);
                // вычисляем максимально допустимый объем новых оплат наличными денежными средствами по сделке
                decimal maxPossibleCashPaymentSum = dealService.GetMaxPossibleCashPaymentSum(deal);

                var teamSelectListItems = teamList.GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), addEmptyItem: teamList.Count() != 1);

                // сумма оплат наличными по договору, связанному с данной сделкой
                var dealContractCashPaymentSum = clientContractService.CalculateDealContractCashPaymentSum(deal.Contract);

                return new
                {
                    StorageList = new { List = GetStorageSelectListItems(deal, user, false), SelectedOption = (SelectListItem)null },
                    TeamList = new { List = teamSelectListItems, SelectedOption = (SelectListItem)teamSelectListItems.FirstOrDefault() },
                    OrganizationDeliveryAddress = deal.Contract.ContractorOrganization.Address,
                    MaxPossibleCashPaymentSum = maxPossibleCashPaymentSum.ForEdit(ValueDisplayType.Money),
                    DealContractCashPaymentSum = dealContractCashPaymentSum.ForDisplay()
                };
            }
        }

        #endregion

        #region Добавление / редактирование договора

        public ClientContractEditViewModel CreateContract(int dealId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);

                if (deal.Contract == null)
                {
                    dealService.CheckPossibilityToAddContract(deal, user);
                }
                else
                {
                    dealService.CheckPossibilityToChangeContract(deal, user);
                }

                var model = new ClientContractEditViewModel();
                model.Title = "Добавление договора по сделке";
                model.DealId = dealId.ToString();
                model.ClientId = deal.Client.Id.ToString();
                model.Date = DateTime.Now.ToShortDateString();
                model.AllowToEditOrganization = true;
                model.AllowToEditClientOrganization = true;

                model.PostControllerName = "Deal";
                model.PostActionName = "EditContract";

                return model;
            }
        }        

        public object SaveContract(ClientContractEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var clientContract = clientContractPresenterMediator.SaveContract(model, currentUser);

                var user = userService.CheckUserExistence(currentUser.Id);

                var deal = dealService.CheckDealExistence(ValidationUtils.TryGetInt(model.DealId), user);

                dealService.SetContract(deal, clientContract, user);

                uow.Commit();

                return new
                {
                    ClientContractName = clientContract.FullName,
                    ClientContractId = clientContract.Id.ToString(),
                    AccountOrganizationName = clientContract.AccountOrganization.ShortName,
                    AccountOrganizationId = clientContract.AccountOrganization.Id.ToString(),
                    ClientOrganizationName = clientContract.ContractorOrganization.ShortName,
                    ClientOrganizationId = clientContract.ContractorOrganization.Id.ToString()
                };
            }
        }

        /// <summary>
        /// Назначить сделке договор с клиентом.
        /// </summary>
        /// <param name="dealId">Идентификатор сделки.</param>
        /// <param name="clientContractId">Идентификатор договора с клиентом.</param>
        /// <param name="currentUser">Информация о пользователе, выполняющем операцию.</param>
        public void SetContract(int dealId, short clientContractId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);               

                var clientContract = clientContractService.CheckClientContractExistence(clientContractId, user);

                dealService.SetContract(deal, clientContract, user);

                uow.Commit();
            }
        }

        /// <summary>
        /// Проверка возможности установить (или сменить) договор.
        /// </summary>
        /// <param name="dealId">Идентификатор сделки.</param>
        /// <param name="currentUser">Пользователь, совершающий операцию.</param>
        public void CheckPossibilityToSetContract(int dealId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);

                dealService.CheckPossibilityToSetContract(deal, user);
            }
        }
        
        #endregion

        #region Добавление / удаление оплаты

        #region Оплаты клиента

        public DealPaymentFromClientEditViewModel CreateDealPaymentFromClient(int dealId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealPaymentFromClient_Create_Edit);
                var deal = dealService.CheckDealExistence(dealId, user);

                dealPaymentDocumentService.CheckPossibilityToCreateDealPaymentFromClient(deal, user);

                var model = new DealPaymentFromClientEditViewModel();
                model.DestinationDocumentSelectorControllerName = "Deal";
                model.DestinationDocumentSelectorActionName = "SaveDealPaymentFromClient";                
                model.Title = "Добавление новой оплаты";
                model.ClientName = deal.Client.Name;
                model.ClientId = deal.Client.Id.ToString();
                model.DealName = deal.Name;
                model.DealId = dealId.ToString();
                model.Date = DateTime.Today.ToShortDateString();
                model.DealPaymentFormList = ComboBoxBuilder.GetComboBoxItemList<DealPaymentForm>(sort: false);
                model.MaxCashPaymentSum = dealService.GetMaxPossibleCashPaymentSum(deal).ForEdit();
                model.AllowToChangeDate = user.HasPermission(Permission.DealPayment_Date_Change);

                return model;
            }
        }

        public object SaveDealPaymentFromClient(DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var result = dealPaymentDocumentPresenterMediator.SaveDealPaymentFromClient(model, currentUser, (deal, user) => GetMainChangeableIndicators(deal, user));

                uow.Commit();

                return result;
            }
        }        

        public object DeleteDealPaymentFromClient(Guid dealPaymentFromClientId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDate = DateTime.Now;

                var user = userService.CheckUserExistence(currentUser.Id);
                var dealPaymentFromClient = dealPaymentDocumentService.CheckDealPaymentFromClientExistence(dealPaymentFromClientId, user);
                var deal = dealService.CheckDealExistence(dealPaymentFromClient.Deal.Id, user);

                dealPaymentDocumentService.Delete(dealPaymentFromClient, user, currentDate);

                uow.Commit();

                return GetMainChangeableIndicators(deal, user);
            }
        }

        #endregion

        #region Возврат оплаты клиенту

        public DealPaymentToClientEditViewModel CreateDealPaymentToClient(int dealId, UserInfo currentUser)
        {

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);

                var clientId = deal.Client.Id;
                var clientName = deal.Client.Name;

                dealPaymentDocumentService.CheckPossibilityToCreateDealPaymentToClient(deal, user);

                var model = new DealPaymentToClientEditViewModel();
                model.ControllerName = "Deal";
                model.ActionName = "SaveDealPaymentToClient";
                model.Title = "Добавление возврата оплаты клиенту";
                model.DealId = dealId;
                model.DealName = deal.Name;

                var teamList = dealService.GetTeamListForDealDocumentByDeal(deal, user);
                model.TeamList = ComboBoxBuilder.GetComboBoxItemList(teamList, x => x.Name, x => x.Id.ToString(), teamList.Count() != 1);
                
                model.ClientId = clientId;
                model.ClientName = clientName;
                model.Date = DateTime.Today.ToShortDateString();
                model.DealPaymentFormList = ComboBoxBuilder.GetComboBoxItemList<DealPaymentForm>(sort: false);

                model.AllowToViewClientOrganization = false;
                model.AllowToViewClient = true;
                model.AllowToChooseClient = false;
                model.AllowToChooseDeal = false;
                model.IsDealSelectedByClient = true;
                model.AllowToChangeDate = user.HasPermission(Permission.DealPayment_Date_Change);

                return model;
            }
        }

        public object SaveDealPaymentToClient(DealPaymentToClientEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var result = dealPaymentDocumentPresenterMediator.SaveDealPaymentToClient(model, currentUser, (deal, user) => GetMainChangeableIndicators(deal, user));

                uow.Commit();

                return result;
            }
        }  

        public object DeleteDealPaymentToClient(Guid dealPaymentToClientId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDate = DateTime.Now;

                var user = userService.CheckUserExistence(currentUser.Id);
                var dealPaymentToClient = dealPaymentDocumentService.CheckDealPaymentToClientExistence(dealPaymentToClientId, user);
                var deal = dealService.CheckDealExistence(dealPaymentToClient.Deal.Id, user);

                dealPaymentDocumentService.Delete(dealPaymentToClient, user, currentDate);

                uow.Commit();

                return GetMainChangeableIndicators(deal, user);
            }
        }

        #endregion

        #region Получение списка команд для документов сделки

        /// <summary>
        /// Получение списка команд для документов сделки (для combobox-а)
        /// </summary>
        /// <param name="dealId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public object GetTeamListForDealDocument(int dealId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId,user);

                var list = dealService.GetTeamListForDealDocumentByDeal(deal, user);
                var itemList = ComboBoxBuilder.GetComboBoxItemList(list, x => x.Name, x => x.Id.ToString(), list.Count() != 1);
                var selectItem = (list.Count() == 1 ? list.First() : null);

                return new { List = itemList, SelectedOption = selectItem != null ? selectItem.Id.ToString() : "" };
            }
        }

        #endregion

        #endregion

        #region Корректировки сальдо

        #region Сохранение

        public object SaveDealDebitInitialBalanceCorrection(DealDebitInitialBalanceCorrectionEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var result = dealPaymentDocumentPresenterMediator.SaveDealDebitInitialBalanceCorrection(model, currentUser, (deal, user) => GetMainChangeableIndicators(deal, user));

                uow.Commit();

                return result;
            }
        }

        public object SaveDealCreditInitialBalanceCorrection(DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var result = dealPaymentDocumentPresenterMediator.SaveDealCreditInitialBalanceCorrection(model, currentUser, (deal, user) => GetMainChangeableIndicators(deal, user));

                uow.Commit();

                return result;
            }
        }

        #endregion

        #region Удаление

        public object DeleteDealCreditInitialBalanceCorrection(Guid correctionId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDate = DateTime.Now;

                var user = userService.CheckUserExistence(currentUser.Id);
                var correction = dealPaymentDocumentService.CheckDealCreditInitialBalanceCorrectionExistence(correctionId, user);

                var deal = dealService.CheckDealExistence(correction.Deal.Id, user);

                dealPaymentDocumentService.Delete(correction, user, currentDate);

                uow.Commit();

                return GetMainChangeableIndicators(deal, user);
            }
        }

        public object DeleteDealDebitInitialBalanceCorrection(Guid correctionId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDate = DateTime.Now;

                var user = userService.CheckUserExistence(currentUser.Id);
                var correction = dealPaymentDocumentService.CheckDealDebitInitialBalanceCorrectionExistence(correctionId, user);

                var deal = dealService.CheckDealExistence(correction.Deal.Id, user);

                dealPaymentDocumentService.Delete(correction, user, currentDate);

                uow.Commit();

                return GetMainChangeableIndicators(deal, user);
            }
        }

        #endregion

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получение значений основных пересчитываемых показателей
        /// </summary>
        private object GetMainChangeableIndicators(Deal deal, User user)
        {
            var j = new
            {
                MainDetails = GetMainDetails(deal, user),
                Permissions = new
                {
                    AllowToCreateExpenditureWaybill = dealService.IsPossibilityToCreateExpenditureWaybill(deal, user),
                    IsPossibilityToCreateExpenditureWaybill = dealService.IsPossibilityToCreateExpenditureWaybill(deal, user, false),

                    AllowToCreateReturnFromClientWaybill = dealService.IsPossibilityToCreateReturnFromClientWaybill(deal, user),
                    IsPossibilityToCreateReturnFromClientWaybill = dealService.IsPossibilityToCreateReturnFromClientWaybill(deal, user, false),

                    AllowToAddQuota = dealService.IsPossibilityToAddQuota(deal, user),
                    AllowToEdit = dealService.IsPossibilityToEdit(deal, user), //пока полностью не избавились от Permissions в этой сущности, придется так
                    AllowToCreateDealPaymentFromClient = dealPaymentDocumentService.IsPossibilityToCreateDealPaymentFromClient(deal, user),
                    AllowToCreateDealPaymentToClient = dealPaymentDocumentService.IsPossibilityToCreateDealPaymentToClient(deal, user),
                    AllowToCreateDealCreditInitialBalanceCorrection = dealPaymentDocumentService.IsPossibilityToCreateDealCreditInitialBalanceCorrection(deal, user),
                    AllowToCreateDealDebitInitialBalanceCorrection = dealPaymentDocumentService.IsPossibilityToCreateDealDebitInitialBalanceCorrection(deal, user)
                }
            };

            return j;
        }

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

        #endregion
    }
}