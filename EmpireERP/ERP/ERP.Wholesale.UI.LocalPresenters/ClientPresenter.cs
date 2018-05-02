using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
using ERP.Wholesale.UI.ViewModels.Client;
using ERP.Wholesale.UI.ViewModels.ContractorOrganization;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ClientPresenter : IClientPresenter
    {
        #region Поля

        private readonly IClientService clientService;
        private readonly IClientTypeService clientTypeService;
        private readonly IClientServiceProgramService clientServiceProgramService;
        private readonly IClientRegionService regionService;
        private readonly ILegalFormService legalFormService;
        private readonly IExpenditureWaybillService expenditureWaybillService;
        private readonly IClientOrganizationService clientOrganizationService;
        private readonly IDealService dealService;
        private readonly IStorageService storageService;
        private readonly IUserService userService;
        private readonly IDealPaymentDocumentService dealPaymentDocumentService;
        private readonly IReturnFromClientWaybillService returnFromClientWaybillService;
        private readonly IReturnFromClientWaybillMainIndicatorService returnFromClientWaybillIndicatorService;
        private readonly ITaskPresenterMediator taskPresenterMediator;

        private readonly IDealPaymentDocumentPresenterMediator dealPaymentDocumentPresenterMediator;

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        #endregion

        #region Конструктор

        public ClientPresenter(IUnitOfWorkFactory unitOfWorkFactory, IClientService clientService, IClientTypeService clientTypeService, IClientServiceProgramService clientServiceProgramService,
            IClientRegionService regionService, ILegalFormService legalFormService, IExpenditureWaybillService expenditureWaybillService,
            IClientOrganizationService clientOrganizationService, IDealService dealService, IStorageService storageService, IUserService userService,
            IDealPaymentDocumentService dealPaymentDocumentService, IReturnFromClientWaybillService returnFromClientWaybillService,
            IReturnFromClientWaybillMainIndicatorService returnFromClientWaybillIndicatorService, ITaskPresenterMediator taskPresenterMediator,
            IDealPaymentDocumentPresenterMediator dealPaymentDocumentPresenterMediator)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.clientService = clientService;
            this.clientTypeService = clientTypeService;
            this.clientServiceProgramService = clientServiceProgramService;
            this.regionService = regionService;
            this.legalFormService = legalFormService;
            this.expenditureWaybillService = expenditureWaybillService;
            this.clientOrganizationService = clientOrganizationService;
            this.dealService = dealService;
            this.userService = userService;
            this.storageService = storageService;
            this.dealPaymentDocumentService = dealPaymentDocumentService;
            this.returnFromClientWaybillService = returnFromClientWaybillService;
            this.returnFromClientWaybillIndicatorService = returnFromClientWaybillIndicatorService;
            
            this.taskPresenterMediator = taskPresenterMediator;
            this.dealPaymentDocumentPresenterMediator = dealPaymentDocumentPresenterMediator;
        }

        #endregion

        #region Методы

        #region Список

        public ClientListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Client_List_Details);

                var model = new ClientListViewModel();
                model.ClientGrid = GetClientGridLocal(new GridState() { PageSize = 25, Sort = "Name=Asc" }, user);

                model.Filter.Items.Add(new FilterTextEditor("Name", "Наименование"));
                model.Filter.Items.Add(new FilterComboBox("Rating", "Рейтинг",
                    Enumerable.Range(0, 11).GetComboBoxItemList<int>(x => x.ToString(), x => x.ToString(), sort: false)));
                model.Filter.Items.Add(new FilterComboBox("Type", "Тип",
                    ComboBoxBuilder.GetComboBoxItemList(clientTypeService.GetList(user), x => x.Name, x => x.Id.ToString())));
                model.Filter.Items.Add(new FilterComboBox("Loyalty", "Лояльность", ComboBoxBuilder.GetComboBoxItemList<ClientLoyalty>(sort: false)));
                model.Filter.Items.Add(new FilterComboBox("Region", "Регион",
                    ComboBoxBuilder.GetComboBoxItemList(regionService.GetList(user), x => x.Name, x => x.Id.ToString())));
                model.Filter.Items.Add(new FilterComboBox("ServiceProgram", "Программа обслуживания",
                     ComboBoxBuilder.GetComboBoxItemList(clientServiceProgramService.GetList(user), x => x.Name, x => x.Id.ToString())));

                return model;
            }
        }

        public GridData GetClientGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetClientGridLocal(state, user);
            }
        }

        private GridData GetClientGridLocal(GridState state, User user)
        {
            var model = new GridData();
            model.AddColumn("Name", "Название", Unit.Percentage(45), GridCellStyle.Link);
            model.AddColumn("Type", "Тип клиента", Unit.Percentage(15), GridCellStyle.Link);
            model.AddColumn("ServiceProgram", "Программа обслуживания", Unit.Percentage(15), GridCellStyle.Link);
            model.AddColumn("Loyalty", "Лояльность", Unit.Pixel(120));
            model.AddColumn("Rating", "Рейтинг", Unit.Pixel(40), GridCellStyle.Label, GridColumnAlign.Center);
            model.AddColumn("SalesVolume", "Сумма продаж", Unit.Pixel(100), GridCellStyle.Label, GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.Client_Create);

            var clients = clientService.GetFilteredList(state, user);

            foreach (var client in clients)
            {
                model.AddRow(new GridRow(
                    new GridLinkCell("Name") { Value = client.Name },
                    new GridLabelCell("Type") { Value = client.Type.Name },
                    new GridLabelCell("ServiceProgram") { Value = client.ServiceProgram.Name },
                    new GridLabelCell("Loyalty") { Value = client.Loyalty.GetDisplayName() },
                    new GridLabelCell("Rating") { Value = client.Rating.ToString() },
                    new GridLabelCell("SalesVolume") { Value = clientService.CalculateSaleSum(client, user).ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("Id") { Value = client.Id.ToString() }));
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Добавление / Редактирование

        public ClientEditViewModel Create(string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Client_Create);

                var model = new ClientEditViewModel();
                model.BackURL = backURL;
                model.Title = "Добавление клиента";                

                model.TypeList = ComboBoxBuilder.GetComboBoxItemList(clientTypeService.GetList(user), x => x.Name, x => x.Id.ToString());
                model.ServiceProgramList = ComboBoxBuilder.GetComboBoxItemList(clientServiceProgramService.GetList(user), x => x.Name, x => x.Id.ToString());
                model.RegionList = ComboBoxBuilder.GetComboBoxItemList(regionService.GetList(user), x => x.Name, x => x.Id.ToString());
                model.LoyaltyList = ComboBoxBuilder.GetComboBoxItemList<ClientLoyalty>(sort: false);
                model.RatingList = Enumerable.Range(0, 11).GetComboBoxItemList(x => x.ToString(), x => x.ToString(), false, sort: false);

                model.AllowToAddClientType = user.HasPermission(Permission.ClientType_Create);
                model.AllowToAddClientServiceProgram = user.HasPermission(Permission.ClientServiceProgram_Create);
                model.AllowToAddClientRegion = user.HasPermission(Permission.ClientRegion_Create);                

                return model;
            }
        }

        public ClientEditViewModel Edit(int id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Client_Edit);

                var client = clientService.CheckClientExistence(id, user);

                var model = new ClientEditViewModel();
                model.BackURL = backURL;
                model.Title = "Редактирование клиента";

                model.Id = client.Id;
                model.Name = client.Name;
                model.TypeId = client.Type.Id;
                model.Loyalty = (byte)client.Loyalty;
                model.ServiceProgramId = client.ServiceProgram.Id;
                model.RegionId = client.Region.Id;
                model.Comment = client.Comment;
                model.Rating = client.Rating.ToString();                
                model.FactualAddress = client.FactualAddress;
                model.ContactPhone = client.ContactPhone;

                model.TypeList = ComboBoxBuilder.GetComboBoxItemList(clientTypeService.GetList(user), x => x.Name, x => x.Id.ToString());
                model.ServiceProgramList = ComboBoxBuilder.GetComboBoxItemList(clientServiceProgramService.GetList(user), x => x.Name, x => x.Id.ToString());
                model.RegionList = ComboBoxBuilder.GetComboBoxItemList(regionService.GetList(user), x => x.Name, x => x.Id.ToString());
                model.LoyaltyList = ComboBoxBuilder.GetComboBoxItemList<ClientLoyalty>(sort: false);
                model.RatingList = Enumerable.Range(0, 11).GetComboBoxItemList(x => x.ToString(), x => x.ToString(), false, sort: false);

                model.AllowToAddClientType = user.HasPermission(Permission.ClientType_Create);
                model.AllowToAddClientServiceProgram = user.HasPermission(Permission.ClientServiceProgram_Create);
                model.AllowToAddClientRegion = user.HasPermission(Permission.ClientRegion_Create);               

                return model;
            }
        }

        public object Save(ClientEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                Client client = null;                

                if (model.Id == 0)
                {
                    user.CheckPermission(Permission.Client_Create);                    

                    //Создаем нового клиента
                    client = new Client(
                        model.Name,
                        clientTypeService.CheckExistence(model.TypeId, user),
                        (ClientLoyalty)model.Loyalty,
                        clientServiceProgramService.CheckExistence(model.ServiceProgramId, user),
                        regionService.CheckExistence(model.RegionId, user),
                        Convert.ToByte(model.Rating));
                }
                else
                {
                    user.CheckPermission(Permission.Client_Edit);

                    //Редактируем
                    client = clientService.CheckClientExistence(model.Id, user);                    

                    client.Type = clientTypeService.CheckExistence(model.TypeId, user);
                    client.Loyalty = (ClientLoyalty)model.Loyalty;
                    client.Rating = Convert.ToByte(model.Rating);
                    client.Region = regionService.CheckExistence(model.RegionId, user);
                    client.ServiceProgram = clientServiceProgramService.CheckExistence(model.ServiceProgramId, user);
                    client.Name = model.Name;
                }

                client.FactualAddress = model.FactualAddress;
                client.ContactPhone = model.ContactPhone;
                client.Comment = StringUtils.ToHtml(model.Comment);                

                clientService.Save(client);

                uow.Commit();

                return new { Id = client.Id.ToString(), BackURL = model.BackURL };
            }
        }

        public object GetClientRegionList(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new { List = ComboBoxBuilder.GetComboBoxItemList(regionService.GetList(user), x => x.Name, x => x.Id.ToString(), false) };

                return model;
            }
        }

        public object GetClientServiceProgramList(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new { List = ComboBoxBuilder.GetComboBoxItemList(clientServiceProgramService.GetList(user), x => x.Name, x => x.Id.ToString(), false) };

                return model;
            }
        }

        public object GetClientTypeList(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new { List = ComboBoxBuilder.GetComboBoxItemList(clientTypeService.GetList(user), x => x.Name, x => x.Id.ToString(), false) };

                return model;
            }
        }

        #endregion

        #region Детали клиента

        #region Детали общие

        public ClientDetailsViewModel Details(int id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var client = clientService.CheckClientExistence(id, user);

                var allowToViewActiveDealList = user.HasPermission(Permission.Deal_List_Details);
                var allowToViewClientOrganizationList = user.HasPermission(Permission.ClientOrganization_List_Details);
                var allowToViewSaleList = user.HasPermission(Permission.ExpenditureWaybill_List_Details);
                var allowToViewDealPaymentList = user.HasPermission(Permission.DealPayment_List_Details);
                var allowToViewDealInitialBalanceCorrectionList = user.HasPermission(Permission.DealInitialBalanceCorrection_List_Details);
                var allowToViewReturnFromClientList = user.HasPermission(Permission.ReturnFromClientWaybill_List_Details);

                var model = new ClientDetailsViewModel()
                {
                    BackURL = backURL,
                    Id = client.Id.ToString(),
                    MainDetails = GetMainDetails(client, user),

                    AllowToEdit = user.HasPermission(Permission.Client_Edit),
                    AllowToDelete = user.HasPermission(Permission.Client_Delete),

                    AllowToViewActiveDealList = allowToViewActiveDealList,

                    AllowToViewClientOrganizationList = allowToViewClientOrganizationList,
                    AllowToViewSaleList = allowToViewSaleList,
                    AllowToViewPaymentList = allowToViewDealPaymentList,
                    AllowToViewReturnFromClientList = allowToViewReturnFromClientList,
                    AllowToViewDealInitialBalanceCorrectionList = allowToViewDealInitialBalanceCorrectionList
                };

                if (allowToViewActiveDealList)
                {
                    model.DealGrid = GetDealGridLocal(new GridState { PageSize = 5, Parameters = "ClientId=" + client.Id, Sort = "IsClosed=Asc;CreationDate=Desc" }, user);
                }
                if (allowToViewSaleList)
                {
                    model.SalesGrid = GetSalesGridLocal(new GridState() { PageSize = 5, Parameters = "ClientId=" + client.Id, Sort = "Date=Desc;CreationDate=Desc" }, user);
                }
                if (allowToViewDealPaymentList)
                {
                    model.PaymentGrid = GetDealPaymentGridLocal(new GridState() { PageSize = 5, Parameters = "ClientId=" + client.Id, Sort = "Date=Desc;CreationDate=Desc" }, user);
                }

                if (allowToViewDealInitialBalanceCorrectionList)
                {
                    model.DealInitialBalanceCorrectionGrid = GetInitialBalanceCorrectionGridLocal(
                        new GridState() 
                        {
                            Parameters = "ClientId=" + client.Id.ToString(),
                            PageSize = 5,
                            Sort = "Date=Desc;CreationDate=Desc"
                        }, user);
                }

                if (allowToViewReturnFromClientList)
                {
                    model.ReturnFromClientGrid = GetReturnFromClientGridLocal(new GridState() { PageSize = 5, Parameters = "ClientId=" + client.Id, Sort = "Date=Desc;CreationDate=Desc" }, user);
                }
                if (allowToViewClientOrganizationList)
                {
                    model.OrganizationGrid = GetOrganizationGridLocal(new GridState() { PageSize = 5, Parameters = "ClientId=" + client.Id }, user);
                }
                model.TaskGrid = taskPresenterMediator.GetTaskGridForClient(client, user);

                return model;
            }
        }

        private ClientMainDetailsViewModel GetMainDetails(Client client, User user)
        {
            decimal saleSum = 0, shippingPendingSaleSum = 0, paymentSum = 0, balance = 0, paymentDelayPeriod = 0, paymentDelaySum = 0,
                returnedSum = 0M, reservedByReturnSum = 0M, initialBalance = 0M;

            clientService.CalculateMainIndicators(client, ref saleSum, ref shippingPendingSaleSum, ref paymentSum, ref balance,
                ref paymentDelayPeriod, ref paymentDelaySum, ref returnedSum, ref reservedByReturnSum, ref initialBalance, user);

            var allowToViewDealPaymentList = user.HasPermission(Permission.DealPayment_List_Details);
            var allowToViewDealInitialBalanceCorrectionList = user.HasPermission(Permission.DealInitialBalanceCorrection_List_Details);
            var allowToViewReturnsFromClient = user.HasPermission(Permission.ReturnFromClientWaybill_List_Details);

            var model = new ClientMainDetailsViewModel()
            {
                IsBlockedManually = client.IsBlockedManually ? "1" : "0",
                Comment = client.Comment,

                LoyaltyName = client.Loyalty.GetDisplayName(),
                Name = client.Name,
                FactualAddress = client.FactualAddress,
                ContactPhone = client.ContactPhone,
                Rating = client.Rating.ToString(),
                RegionName = client.Region.Name,
                ServiceProgramName = client.ServiceProgram.Name,

                SaleSum = saleSum.ForDisplay(ValueDisplayType.Money),
                ShippingPendingSaleSum = shippingPendingSaleSum.ForDisplay(ValueDisplayType.Money),
                PaymentSum = allowToViewDealPaymentList ? paymentSum.ForDisplay(ValueDisplayType.Money) : "---",
                Balance = allowToViewDealPaymentList ? balance.ForDisplay(ValueDisplayType.Money) : "---",
                InitialBalance = allowToViewDealInitialBalanceCorrectionList ? initialBalance.ForDisplay(ValueDisplayType.Money) : "---",                

                TypeName = client.Type.Name,
                PaymentDelayPeriod = paymentDelayPeriod.ForDisplay(),
                PaymentDelaySum = paymentDelaySum.ForDisplay(ValueDisplayType.Money),
                TotalReturnedSum = allowToViewReturnsFromClient ? returnedSum.ForDisplay(ValueDisplayType.Money) : "---",
                TotalReservedByReturnSum = allowToViewReturnsFromClient ? reservedByReturnSum.ForDisplay(ValueDisplayType.Money) : "---",

                AllowToBlock = user.HasPermission(Permission.Client_Block)
            };

            return model;
        }

        /// <summary>
        /// Получение значений основных пересчитываемых показателей
        /// </summary>
        private object GetMainChangeableIndicators(Client client, User user)
        {
            var j = new
            {
                MainDetails = GetMainDetails(client, user)
            };

            return j;
        }

        #endregion

        #region Сделки

        public GridData GetDealGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetDealGridLocal(state, user);
            }
        }

        private GridData GetDealGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData();
            model.AddColumn("IsClosed", "Закрыта", Unit.Pixel(50));
            model.AddColumn("Name", "Название", Unit.Percentage(34));
            model.AddColumn("CreationDate", "Дата создания", Unit.Pixel(60));
            model.AddColumn("StageName", "Этап", Unit.Pixel(130));
            model.AddColumn("ClientOrganizationName", "Организация клиента", Unit.Percentage(33));
            model.AddColumn("AccountOrganizationName", "Собственная организация", Unit.Percentage(33));
            model.AddColumn("ExpectedBudget", "Ожидаемый бюджет", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("SaleSum", "Сумма реализации", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ClientOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("AccountOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreateDeal"] = user.HasPermission(Permission.Deal_Create_Edit);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var clientId = ValidationUtils.TryGetInt(deriveParams["ClientId"].Value.ToString());
            clientService.CheckClientExistence(clientId, user);

            ParameterString param = new ParameterString("");
            param.Add("Client", ParameterStringItem.OperationType.Eq, clientId.ToString());

            var list = dealService.GetFilteredList(state, param, user, Permission.Deal_List_Details);

            foreach (var deal in list)
            {
                var contract = deal.Contract;
                var clientOrganization = contract != null ? deal.Contract.ContractorOrganization : null;
                var accountOrganization = contract != null ? deal.Contract.AccountOrganization : null;

                GridCell clientOrganizationCell, accountOrganizationCell;

                if (clientOrganization != null)
                {
                    if (user.HasPermission(Permission.ClientOrganization_List_Details))
                    {
                        clientOrganizationCell = new GridLinkCell("ClientOrganizationName") { Value = clientOrganization.ShortName };
                    }
                    else
                    {
                        clientOrganizationCell = new GridLabelCell("ClientOrganizationName") { Value = clientOrganization.ShortName };
                    }
                }
                else
                {
                    clientOrganizationCell = new GridLabelCell("ClientOrganizationName") { Value = "---" };
                }

                if (accountOrganization != null)
                {
                    accountOrganizationCell = new GridLinkCell("AccountOrganizationName") { Value = accountOrganization.ShortName };
                }
                else
                {
                    accountOrganizationCell = new GridLabelCell("AccountOrganizationName") { Value = "---" };
                }

                decimal saleSum = dealService.CalculateSaleSum(deal, user);

                model.AddRow(new GridRow(
                    new GridLabelCell("IsClosed") { Value = deal.IsClosed ? "З" : "" },
                    new GridLinkCell("Name") { Value = deal.Name },
                    new GridLabelCell("CreationDate") { Value = deal.CreationDate.ToShortDateString() },
                    new GridLabelCell("StageName") { Value = deal.Stage.GetDisplayName() },
                    clientOrganizationCell,
                    accountOrganizationCell,
                    new GridLabelCell("ExpectedBudget") { Value = deal.ExpectedBudget != null ? deal.ExpectedBudget.Value.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("SaleSum") { Value = saleSum.ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("Id") { Value = deal.Id.ToString() },
                    new GridHiddenCell("ClientOrganizationId") { Value = clientOrganization != null ? clientOrganization.Id.ToString() : "" },
                    new GridHiddenCell("AccountOrganizationId") { Value = accountOrganization != null ? accountOrganization.Id.ToString() : "" }
               ));
            }
            model.State = state;

            return model;
        }

        #endregion

        #region Организации клиента

        #region Грид организаций клиента в деталях клиента

        public GridData GetOrganizationGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetOrganizationGridLocal(state, user);
            }
        }

        private GridData GetOrganizationGridLocal(GridState state, User user)
        {
            var allowToRemoveOrganization = user.HasPermission(Permission.Client_ClientOrganization_Remove);

            GridData model = new GridData();
            if (allowToRemoveOrganization) { model.AddColumn("Action", "Действие", Unit.Pixel(90), GridCellStyle.Action); }
            model.AddColumn("INN", "ИНН", Unit.Pixel(75));
            model.AddColumn("ShortName", "Краткое название", Unit.Percentage(100), GridCellStyle.Link);
            model.AddColumn("LegalForm", "Юр. форма", Unit.Pixel(60));
            model.AddColumn("TotalSaleSum", "Сумма продаж организации", Unit.Pixel(120), align: GridColumnAlign.Right);
            model.AddColumn("ClientOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToAddClientOrganization"] = user.HasPermission(Permission.Client_ClientOrganization_Add);

            ParameterString dp = new ParameterString(state.Parameters);
            Client client = clientService.CheckClientExistence(Convert.ToInt32(dp["ClientId"].Value), user);

            GridActionCell action = new GridActionCell("Action");
            if (allowToRemoveOrganization)
            {
                action.AddAction("Удал. из списка", "removeClientOrganization");
            }

            foreach (var org in GridUtils.GetEntityRange(client.Organizations.OrderBy(x => x.ShortName), state))
            {
                ClientOrganization clientOrganization = org.As<ClientOrganization>();

                decimal saleSum = clientOrganizationService.CalculateSaleSum(clientOrganization, client, user);

                model.AddRow(new GridRow(
                    action,
                    new GridLabelCell("INN") { Value = clientOrganization.EconomicAgent.As<EconomicAgent>().Type == EconomicAgentType.JuridicalPerson ? ((clientOrganization.EconomicAgent.As<JuridicalPerson>())).INN : ((clientOrganization.EconomicAgent.As<PhysicalPerson>())).INN },
                    new GridLinkCell("ShortName") { Value = clientOrganization.ShortName },
                    new GridLabelCell("LegalForm") { Value = clientOrganization.EconomicAgent.LegalForm.Name },
                    new GridLabelCell("TotalSaleSum") { Value = saleSum.ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("ClientOrganizationId") { Value = clientOrganization.Id.ToString() }));
            }
            model.State = state;

            return model;
        }

        #endregion

        #region Выбор существующей организации клиента

        /// <summary>
        /// Формирование модели грида доступных организаций контрагента
        /// </summary>
        /// <param name="state">состояние грида</param>
        public GridData GetClientOrganizationSelectGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetClientOrganizationSelectGridLocal(state, user);
            }
        }

        /// <summary>
        /// Формирование модели грида доступных организаций контрагента
        /// </summary>
        /// <param name="state">состояние грида</param>
        private GridData GetClientOrganizationSelectGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(45));
            model.AddColumn("INN", "ИНН", Unit.Pixel(75));
            model.AddColumn("ShortName", "Название", Unit.Percentage(100));
            model.AddColumn("LegalForm", "Юр. форма", Unit.Pixel(80));
            model.AddColumn("Id", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            int currentPage = state.CurrentPage.Value;
            int pageSize = state.PageSize;

            ParameterString deriveParams = new ParameterString(state.Parameters);

            string mode = "";
            Client client = null;

            if (deriveParams["mode"] != null)
            {
                mode = deriveParams["mode"].Value as string;
                ValidationUtils.NotNull(mode, "Неверное значение входного параметра.");

                int clientId;

                if (deriveParams["clientId"] == null || !Int32.TryParse(deriveParams["clientId"].Value as string, out clientId))
                {
                    throw new Exception("Неверное значение входного параметра.");
                }

                client = clientService.CheckClientExistence(clientId, user);
            }

            IEnumerable<ClientOrganization> clientOrganizationsOnThisPage = null;

            if (deriveParams["mode"] != null)
            {
                switch (mode)
                {
                    case "includeclient":
                        model.Title = "Организации клиента";

                        var clientOrganizationList = new List<ClientOrganization>();
                        if (user.HasPermission(Permission.ClientOrganization_List_Details))
                        {
                            clientOrganizationList = client.Organizations.Cast<ClientOrganization>().OrderBy(x => x.ShortName).ToList<ClientOrganization>();
                        }

                        clientOrganizationsOnThisPage = GridUtils.GetEntityRange(clientOrganizationList, state);
                        break;

                    case "excludeclient":
                        IEnumerable<int> clientOrganizationIdList = client.Organizations.Select(x => x.Id);
                        model.Title = "Организации, доступные для клиентов";
                        deriveParams = new ParameterString("");
                        deriveParams.Add("Id", ParameterStringItem.OperationType.NotOneOf);
                        deriveParams["Id"].Value = clientOrganizationIdList.ToList().ConvertAll<string>(x => x.ToString());
                        state.Sort = "ShortName=Asc;";

                        clientOrganizationsOnThisPage = clientOrganizationService.GetFilteredList(state, user, deriveParams);
                        break;

                    default:
                        throw new Exception("Неверное значение входного параметра.");
                }
            }
            else
            {
                clientOrganizationsOnThisPage = clientOrganizationService.GetFilteredList(state, user);
                model.Title = "Организации клиентов";
            }
            model.State = state;

            foreach (var item in clientOrganizationsOnThisPage)
            {
                var organization = (Organization)item;

                string innString = "";
                switch (organization.EconomicAgent.As<EconomicAgent>().Type)
                {
                    case EconomicAgentType.JuridicalPerson:
                        JuridicalPerson juridicalPerson = organization.EconomicAgent.As<JuridicalPerson>();
                        innString = juridicalPerson.INN;
                        break;
                    case EconomicAgentType.PhysicalPerson:
                        PhysicalPerson physicalPerson = organization.EconomicAgent.As<PhysicalPerson>();
                        innString = physicalPerson.INN;
                        break;
                    default:
                        throw new Exception(String.Format("Обнаружена организация ({0}) неизвестного типа.", organization.Id));
                };

                model.AddRow(new GridRow(
                    new GridActionCell("Action", new GridActionCell.Action("Выбрать", "linkOrganizationSelect")),
                    new GridLabelCell("INN") { Value = innString },
                    new GridLabelCell("ShortName") { Value = organization.ShortName, Key = "organizationShortName" },
                    new GridLabelCell("LegalForm") { Value = organization.EconomicAgent.LegalForm.Name },
                    new GridHiddenCell("Id") { Value = organization.Id.ToString(), Key = "organizationId" }
                ));
            }

            model.GridPartialViewAction = "/Client/ShowClientOrganizationSelectGrid/";

            return model;
        }

        public ContractorOrganizationSelectViewModel SelectClientOrganization(int? clientId, string mode, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Client_ClientOrganization_Add);

                GridState state;
                var model = new ContractorOrganizationSelectViewModel();

                if (clientId != null)
                {
                    mode = mode.ToLower();

                    if (mode != "includeclient" && mode != "excludeclient")
                    {
                        throw new Exception("Неверное значение входного параметра.");
                    }

                    state = new GridState { Parameters = "clientId=" + clientId + ";mode=" + mode };
                    model.Title = "Добавление связанной организации";

                    model.NewOrganizationLinkName = "Создать новую организацию и связать ее с данным клиентом";
                }
                else
                {
                    state = new GridState();
                    model.Title = "Выбор организации";
                }

                model.ContractorId = clientId.ToString();
                model.ControllerName = "Client";
                model.ActionName = "CreateContractorOrganization";
                // если клиент не задан, то создавать новую организацию нельзя
                model.AllowToCreateNewOrganization = clientId != null && user.HasPermission(Permission.ClientOrganization_Create);

                model.GridData = GetClientOrganizationSelectGridLocal(state, user);
                model.Filter = new FilterData()
                {
                    Items = new List<FilterItem>()
                {
                    new FilterTextEditor("ShortName", "Название"),
                    new FilterTextEditor("INN", "ИНН"),
                    new FilterTextEditor("Address", "Адрес"),
                    new FilterTextEditor("OGRN", "ОГРН"),
                    new FilterTextEditor("OKPO", "ОКПО"),
                    new FilterTextEditor("KPP", "КПП")
                }
                };

                return model;
            }
        }

        public void AddClientOrganization(int clientId, int organizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var client = clientService.CheckClientExistence(clientId, user);
                var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(organizationId, user);

                clientService.AddClientOrganization(client, clientOrganization, user);

                uow.Commit();
            }
        }

        public void RemoveClientOrganization(int clientId, int clientOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var client = clientService.CheckClientExistence(clientId, user);

                var clientOrganization = client.Organizations.SingleOrDefault(x => x.Id == clientOrganizationId);
                ValidationUtils.NotNull(clientOrganization, "Организация клиента не найдена. Возможно, она была удалена или больше не принадлежит данному клиенту.");

                clientService.RemoveClientOrganization(client, clientOrganization, user);

                uow.Commit();
            }
        }
        #endregion

        #region Создание новой организации

        public EconomicAgentTypeSelectorViewModel CreateContractorOrganization(int contractorId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = new EconomicAgentTypeSelectorViewModel();
                model.Title = "Добавление новой организации";
                model.ActionNameForJuridicalPerson = "EditJuridicalPerson";
                model.ActionNameForPhysicalPerson = "EditPhysicalPerson";
                model.ControllerName = "Client";
                model.SuccessFunctionName = "OnSuccessOrganizationEdit";
                model.ContractorId = contractorId.ToString();

                return model;
            }
        }

        public object SaveJuridicalPerson(JuridicalPersonEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ClientOrganization_Create);

                if (model.OrganizationId != 0) // если пытаются отредактировать организацию (метод предназначен только для создания, а не для редактирования)
                {
                    throw new Exception("Неверное значение входного параметра.");
                }

                var legalForm = legalFormService.CheckExistence(ValidationUtils.TryGetShort(model.LegalFormId));
                var client = clientService.CheckClientExistence(ValidationUtils.TryGetInt(model.ContractorId), user);

                var jp = new JuridicalPerson(legalForm);
                jp.INN = model.INN;
                jp.KPP = model.KPP;
                jp.OGRN = model.OGRN;
                jp.OKPO = model.OKPO;
                jp.LegalForm = legalForm;
                jp.DirectorName = model.DirectorName;
                jp.DirectorPost = model.DirectorPost;
                jp.MainBookkeeperName = model.Bookkeeper;
                jp.CashierName = model.Cashier;

                var clientOrganization = new ClientOrganization(model.ShortName, model.FullName, jp);
                clientOrganization.Address = model.Address;
                clientOrganization.Comment = StringUtils.ToHtml(model.Comment);
                clientOrganization.Phone = model.Phone;
                clientOrganization.Fax = model.Fax;

                clientOrganizationService.Save(clientOrganization); //Саша: Не комментировать. Иначе не будет выполнена проверка уникальности
                clientService.AddClientOrganization(client, clientOrganization, user);

                uow.Commit();

                var result = new { organizationId = clientOrganization.Id.ToString(), organizationShortName = clientOrganization.ShortName };

                return result;
            }
        }

        public object SavePhysicalPerson(PhysicalPersonEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ClientOrganization_Create);

                if (model.OrganizationId != 0) // если пытаются отредактировать организацию (метод предназначен только для создания, а не для редактирования)
                {
                    throw new Exception("Неверное значение входного параметра.");
                }

                var legalForm = legalFormService.CheckExistence(ValidationUtils.TryGetShort(model.LegalFormId));
                var client = clientService.CheckClientExistence(ValidationUtils.TryGetInt(model.ContractorId), user);

                var pp = new PhysicalPerson(legalForm);
                pp.OwnerName = model.FIO;
                pp.INN = model.INN;
                pp.Passport.Series = model.Series;
                pp.Passport.Number = model.Number;
                pp.Passport.IssuedBy = model.IssuedBy;
                pp.Passport.IssueDate = model.IssueDate == null ? (DateTime?)null : Convert.ToDateTime(model.IssueDate);
                pp.OGRNIP = model.OGRNIP;
                pp.LegalForm = legalForm;

                var clientOrganization = new ClientOrganization(model.ShortName, model.FullName, pp);

                clientOrganization.Address = model.Address;
                clientOrganization.Comment = StringUtils.ToHtml(model.Comment);
                clientOrganization.Phone = model.Phone;
                clientOrganization.Fax = model.Fax;

                clientOrganizationService.Save(clientOrganization); //Саша: Не комментировать. Иначе не будет выполнена проверка уникальности
                clientService.AddClientOrganization(client, clientOrganization, user);

                uow.Commit();

                var result = new { organizationId = clientOrganization.Id.ToString(), organizationShortName = clientOrganization.ShortName };

                return result;
            }
        }

        #endregion

        #endregion

        #region Накладные реализации

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

            var model = new GridData();
            model.AddColumn("Number", "Номер документа", Unit.Pixel(80), GridCellStyle.Link);
            model.AddColumn("SaleId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Date", "Дата", Unit.Pixel(40));
            model.AddColumn("AccountingPriceSum", "Сумма в УЦ", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Discount", "Скидка", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("SalePriceSum", "Итоговая сумма", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("ReturnFromClientSum", "Сумма возвращенного товара", Unit.Pixel(140), align: GridColumnAlign.Right);
            model.AddColumn("DealName", "Сделка", Unit.Percentage(30), GridCellStyle.Link);
            model.AddColumn("DealId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("StateName", "Статус", Unit.Percentage(30));
            model.AddColumn("StorageName", "Место хранения", Unit.Percentage(40), GridCellStyle.Link);
            model.AddColumn("StorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("PaymentPercent", "Оплата", Unit.Pixel(50), align: GridColumnAlign.Right);

            model.ButtonPermissions["AllowToCreateExpenditureWaybill"] = user.HasPermission(Permission.ExpenditureWaybill_Create_Edit);

            ParameterString dp = new ParameterString(state.Parameters);
            var clientId = ValidationUtils.TryGetInt(dp["ClientId"].Value.ToString());
            var client = clientService.CheckClientExistence(clientId, user);

            ParameterString param = new ParameterString("");
            param.Add("Deal.Client", ParameterStringItem.OperationType.Eq, clientId.ToString());

            var sales = expenditureWaybillService.GetFilteredList(state, user, param);

            foreach (var sale in sales)
            {
                decimal? senderAccountingPriceSum = 0M, salePriceSum = 0M, paymentPercent = 0M, totalDiscountPercent = 0M, totalReturnedSum = 0M;

                var allowToViewAccPrices = user.HasPermissionToViewStorageAccountingPrices(sale.SenderStorage);

                string stateName = "---", storageName = "---", storageId = "";

                if (sale.Is<ExpenditureWaybill>())
                {
                    var expenditureWaybill = sale.As<ExpenditureWaybill>();
                    var allowToViewPayments = expenditureWaybillService.IsPossibilityToViewPayments(expenditureWaybill, user);

                    var ind = expenditureWaybillService.GetMainIndicators(expenditureWaybill,
                        calculateSenderAccountingPriceSum: allowToViewAccPrices, calculateSalePriceSum: true, calculatePaymentPercent: allowToViewPayments,
                        calculateTotalDiscount: allowToViewAccPrices, calculateTotalReturnedSum: true);

                    senderAccountingPriceSum = allowToViewAccPrices ? ind.SenderAccountingPriceSum : (decimal?)null;
                    salePriceSum = ind.SalePriceSum;
                    paymentPercent = allowToViewPayments ? ind.PaymentPercent : (decimal?)null;
                    totalDiscountPercent = allowToViewAccPrices ? ind.TotalDiscountPercent : (decimal?)null;
                    totalReturnedSum = ind.TotalReturnedSum;

                    stateName = expenditureWaybill.State.GetDisplayName();
                    storageName = expenditureWaybill.SenderStorage.Name;
                    storageId = expenditureWaybill.SenderStorage.Id.ToString();
                }

                model.AddRow(new GridRow(
                    new GridLinkCell("Number") { Value = sale.Number.PadLeftZeroes(8) },
                    new GridLabelCell("SaleId") { Value = sale.Id.ToString() },
                    new GridLabelCell("Date") { Value = sale.Date.ToShortDateString() },
                    new GridLabelCell("AccountingPriceSum")
                    {
                        Value = user.HasPermissionToViewStorageAccountingPrices(sale.SenderStorage) ?
                            senderAccountingPriceSum.ForDisplay(ValueDisplayType.Money) : "---"
                    },
                    new GridLabelCell("Discount") { Value = totalDiscountPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridLabelCell("SalePriceSum") { Value = salePriceSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("ReturnFromClientSum") { Value = totalReturnedSum.ForDisplay(ValueDisplayType.Money) },
                    dealService.IsPossibilityToViewDetails(sale.Deal, user) ?
                        (GridCell)new GridLinkCell("DealName") { Value = sale.Deal.Name } :
                        new GridLabelCell("DealName") { Value = sale.Deal.Name },
                    new GridLabelCell("DealId") { Value = sale.Deal.Id.ToString() },
                    new GridLabelCell("StateName") { Value = stateName },
                    storageService.IsPossibilityToViewDetails(sale.SenderStorage, user) ?
                        (GridCell)new GridLinkCell("StorageName") { Value = storageName } :
                        new GridLabelCell("StorageName") { Value = storageName },
                    new GridLabelCell("StorageId") { Value = storageId },
                    new GridLabelCell("PaymentPercent") { Value = (paymentPercent.ForDisplay(ValueDisplayType.Percent) + (paymentPercent.HasValue ? " %" : "")) }
                    ));
            }
            model.State = state;
            model.Title = "Реализации по сделкам";
            model.GridPartialViewAction = "/Client/ShowSalesGrid/";

            return model;
        }

        #endregion

        #region Оплаты

        #region Список

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

            var model = new GridData() { State = state };
            model.AddColumn("Action", "Действие", Unit.Pixel(70), GridCellStyle.Action);
            model.AddColumn("Date", "Дата", Unit.Pixel(54), align: GridColumnAlign.Center);
            model.AddColumn("Number", "Номер документа", Unit.Pixel(110));
            model.AddColumn("DealName", "Сделка", Unit.Percentage(60));
            model.AddColumn("DealPaymentForm", "Форма оплаты", Unit.Percentage(40));
            model.AddColumn("Sum", "Сумма оплаты", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("DealId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreateDealPaymentFromClient"] = user.HasPermission(Permission.DealPaymentFromClient_Create_Edit); // не используем службу, т.к. заранее не знаем сделку
            model.ButtonPermissions["AllowToCreateDealPaymentToClient"] = user.HasPermission(Permission.DealPaymentToClient_Create);

            ParameterString dp = new ParameterString(state.Parameters);
            var clientId = ValidationUtils.TryGetInt(dp["ClientId"].Value.ToString());
            var client = clientService.CheckClientExistence(clientId, user);

            var ps = new ParameterString("");            
            ps.Add("Deal.Client", ParameterStringItem.OperationType.Eq, client.Id.ToString());

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
                    new GridLabelCell("Number") { Value = payment.PaymentDocumentNumber },
                    dealService.IsPossibilityToViewDetails(payment.Deal, user) ?
                        (GridCell)new GridLinkCell("DealName") { Value = payment.Deal.Name } :
                        new GridLabelCell("DealName") { Value = payment.Deal.Name },
                    new GridLabelCell("DealPaymentForm") { Value = payment.DealPaymentForm.GetDisplayName() },
                    new GridLabelCell("Sum") { Value = (payment.Type == DealPaymentDocumentType.DealPaymentFromClient ? payment.Sum : -payment.Sum).ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("Id") { Value = payment.Id.ToString(), Key = "PaymentId" },
                    new GridHiddenCell("DealId") { Value = payment.Deal.Id.ToString() }
                    ) { Style = payment.Type == DealPaymentDocumentType.DealPaymentToClient ? GridRowStyle.Warning : GridRowStyle.Normal });
            }

            model.Title = "Оплаты по сделкам";

            return model;
        }
        #endregion

        #region Оплаты от клиента

        public DealPaymentFromClientEditViewModel CreateDealPaymentFromClient(int clientId, UserInfo userInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(userInfo.Id);
                user.CheckPermission(Permission.DealPaymentFromClient_Create_Edit);
                var client = clientService.CheckClientExistence(clientId, user);

                var model = new DealPaymentFromClientEditViewModel()
                {
                    Title = "Добавление оплаты",
                    ClientName = client.Name,
                    ClientId = client.Id.ToString(),
                    DealName = "Выберите сделку",
                    Date = DateTime.Today.ToShortDateString(),
                    DealPaymentFormList = ComboBoxBuilder.GetComboBoxItemList<DealPaymentForm>(sort: false),
                    MaxCashPaymentSum = dealService.GetMaxPossibleCashPaymentSum(null).ForEdit(),
                    DestinationDocumentSelectorControllerName = "Client",
                    DestinationDocumentSelectorActionName = "SaveDealPaymentFromClient",
                    AllowToChangeDate = user.HasPermission(Permission.DealPayment_Date_Change)
                };

                return model;
            }
        }

        public object SaveDealPaymentFromClient(DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var result = dealPaymentDocumentPresenterMediator.SaveDealPaymentFromClient(model, currentUser, (deal, user) => GetMainChangeableIndicators(deal.Client, user));

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
                var client = clientService.CheckClientExistence(dealPaymentFromClient.Deal.Client.Id, user);

                dealPaymentDocumentService.Delete(dealPaymentFromClient, user, currentDate);

                uow.Commit();

                return GetMainChangeableIndicators(client, user);
            }
        }

        #endregion

        #region Возвраты оплат клиенту

        public DealPaymentToClientEditViewModel CreateDealPaymentToClient(int clientId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealPaymentToClient_Create);

                var client = clientService.CheckClientExistence(clientId, user);

                var model = new DealPaymentToClientEditViewModel();
                model.ControllerName = "Client";
                model.ActionName = "SaveDealPaymentToClient";
                model.Title = "Добавление возврата оплаты клиенту";
                model.Date = DateTime.Today.ToShortDateString();
                model.DealId = 0;
                model.DealName = "Выберите сделку";
                model.ClientId = clientId;
                model.ClientName = client.Name;
                model.TeamList = ComboBoxBuilder.GetComboBoxItemList(new List<Team>(), x => x.Name, x => x.Id.ToString(), false);
                model.DealPaymentFormList = ComboBoxBuilder.GetComboBoxItemList<DealPaymentForm>(sort: false);

                model.AllowToViewClientOrganization = false;
                model.AllowToViewClient = true;
                model.AllowToChooseClient = false;
                model.AllowToChooseDeal = true;
                model.IsDealSelectedByClient = true;
                model.AllowToChangeDate = user.HasPermission(Permission.DealPayment_Date_Change);

                return model;
            }
        }

        public object SaveDealPaymentToClient(DealPaymentToClientEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var result = dealPaymentDocumentPresenterMediator.SaveDealPaymentToClient(model, currentUser, (deal, user) => GetMainChangeableIndicators(deal.Client, user));

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
                var client = clientService.CheckClientExistence(dealPaymentToClient.Deal.Client.Id, user);

                dealPaymentDocumentService.Delete(dealPaymentToClient, user, currentDate);

                uow.Commit();

                return GetMainChangeableIndicators(client, user);
            }
        }

        #endregion

        #endregion

        #region Возвраты

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
            model.AddColumn("DealName", "Сделка", Unit.Percentage(33));
            model.AddColumn("DealId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("RecipientStorageName", "Место хранения-приемщик", Unit.Percentage(33));
            model.AddColumn("RecipientStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("RecipientName", "Организация-приемщик", Unit.Percentage(34));
            model.AddColumn("ShippingPercent", "Отгрузка", Unit.Pixel(50), align: GridColumnAlign.Right);
            model.AddColumn("RecipientId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.ReturnFromClientWaybill_Create_Edit);

            ParameterString dp = new ParameterString(state.Parameters);
            var clientId = ValidationUtils.TryGetInt(dp["ClientId"].Value.ToString());
            var client = clientService.CheckClientExistence(clientId, user);

            var ps = new ParameterString("");
            ps.Add("Deal.Client", ParameterStringItem.OperationType.Eq, client.Id.ToString());

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
                    dealService.IsPossibilityToViewDetails(row.Deal, user) ?
                        (GridCell)new GridLinkCell("DealName") { Value = row.Deal.Name } :
                        new GridLabelCell("DealName") { Value = row.Deal.Name },
                    new GridHiddenCell("DealId") { Value = row.Deal.Id.ToString() },
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

        #region Корректировки сальдо

        #region Грид корректировок

        public GridData GetInitialBalanceCorrectionGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetInitialBalanceCorrectionGridLocal(state, user);
            }
        }

        private GridData GetInitialBalanceCorrectionGridLocal(GridState state, User user)
        {
            user.CheckPermission(Permission.DealInitialBalanceCorrection_List_Details);

            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            var model = new GridData() { State = state };
            model.AddColumn("Action", "Действие", Unit.Pixel(70), GridCellStyle.Action);
            model.AddColumn("Date", "Дата", Unit.Pixel(54), align: GridColumnAlign.Center);
            model.AddColumn("Reason", "Причина корректировки", Unit.Percentage(60));
            model.AddColumn("DealName", "Сделка", Unit.Percentage(40));
            model.AddColumn("Sum", "Сумма корректировки", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("PaymentPercent", "Процент оплаты", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("DealId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreateDealDebitInitialBalanceCorrection"] = user.HasPermission(Permission.DealDebitInitialBalanceCorrection_Create);
            model.ButtonPermissions["AllowToCreateDealCreditInitialBalanceCorrection"] = user.HasPermission(Permission.DealCreditInitialBalanceCorrection_Create_Edit);

            ParameterString dp = new ParameterString(state.Parameters);
            var clientId = ValidationUtils.TryGetInt(dp["ClientId"].Value.ToString());
            var client = clientService.CheckClientExistence(clientId, user);

            var ps = new ParameterString("");            
            ps.Add("Deal.Client", ParameterStringItem.OperationType.Eq, client.Id.ToString());

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
                     dealService.IsPossibilityToViewDetails(correction.Deal, user) ?
                         (GridCell)new GridLinkCell("DealName") { Value = correction.Deal.Name } :
                         new GridLabelCell("DealName") { Value = correction.Deal.Name },
                     new GridLabelCell("Sum") { Value = (correction.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection ? correction.Sum : -correction.Sum).ForDisplay(ValueDisplayType.Money) },
                     new GridLabelCell("PaymentPercent") { Value = correction.PaymentPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                     new GridHiddenCell("Id") { Value = correction.Id.ToString(), Key = "CorrectionId" },                     
                     new GridHiddenCell("DealId") { Value = correction.Deal.Id.ToString(), Key = "DealId" }
                     ) { Style = correction.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection ? GridRowStyle.Normal : GridRowStyle.Warning });
            }

            model.Title = "Корректировки сальдо по сделкам";

            return model;
        }

        #endregion

        #region Создание
        
        /// <summary>
        /// Кредитовая корректировка
        /// </summary>
        public DealCreditInitialBalanceCorrectionEditViewModel CreateDealCreditInitialBalanceCorrection(int clientId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealCreditInitialBalanceCorrection_Create_Edit);    //Проверяем право на создание корректировки

                var client = clientService.CheckClientExistence(clientId, user);

                var model = new DealCreditInitialBalanceCorrectionEditViewModel();

                model.Title = "Добавление кредитовой корректировки";
                model.DealName = "Выберите сделку";
                model.ClientName = client.Name;
                model.ClientId = client.Id;
                model.Date = DateTime.Today.ToShortDateString();

                model.DestinationDocumentSelectorControllerName = "Client";
                model.DestinationDocumentSelectorActionName = "SaveDealCreditInitialBalanceCorrection";

                model.AllowToViewClientOrganization = false;
                model.AllowToViewClient = true;
                model.AllowToChooseClient = false;
                model.AllowToChooseDeal = true;
                model.IsDealSelectedByClient = true;
                model.AllowToChangeDate = user.HasPermission(Permission.DealInitialBalanceCorrection_Date_Change);

                return model;
            }
        }

        /// <summary>
        /// Дебетовая корректировка
        /// </summary>
        public DealDebitInitialBalanceCorrectionEditViewModel CreateDealDebitInitialBalanceCorrection(int clientId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealDebitInitialBalanceCorrection_Create);    //Проверяем право на создание корректировки

                var client = clientService.CheckClientExistence(clientId, user);

                var model = new DealDebitInitialBalanceCorrectionEditViewModel();

                model.Title = "Добавление дебетовой корректировки";
                model.DealName = "Выберите сделку";
                model.ClientName = client.Name;
                model.ClientId = client.Id;
                model.TeamList = ComboBoxBuilder.GetComboBoxItemList(new List<Team>(), x => x.Name, x => x.Id.ToString(), false);
                model.Date = DateTime.Today.ToShortDateString();

                model.ControllerName = "Client";
                model.ActionName = "SaveDealDebitInitialBalanceCorrection";

                model.AllowToViewClientOrganization = false;
                model.AllowToViewClient = true;
                model.AllowToChooseClient = false;
                model.AllowToChooseDeal = true;
                model.IsDealSelectedByClient = true;
                model.AllowToChangeDate = user.HasPermission(Permission.DealInitialBalanceCorrection_Date_Change);

                return model;
            }
        }

        #endregion

        #region Сохранение

        public object SaveDealDebitInitialBalanceCorrection(DealDebitInitialBalanceCorrectionEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var result = dealPaymentDocumentPresenterMediator.SaveDealDebitInitialBalanceCorrection(model, currentUser, (deal, user) => GetMainChangeableIndicators(deal.Client, user));

                uow.Commit();

                return result;
            }
        }

        public object SaveDealCreditInitialBalanceCorrection(DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var result = dealPaymentDocumentPresenterMediator.SaveDealCreditInitialBalanceCorrection(model, currentUser, (deal, user) => GetMainChangeableIndicators(deal.Client, user));

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
                                
                var client = clientService.CheckClientExistence(correction.Deal.Client.Id, user);

                dealPaymentDocumentService.Delete(correction, user, currentDate);

                uow.Commit();

                return GetMainChangeableIndicators(client, user);
            }
        }

        public object DeleteDealDebitInitialBalanceCorrection(Guid correctionId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDate = DateTime.Now;

                var user = userService.CheckUserExistence(currentUser.Id);
                var correction = dealPaymentDocumentService.CheckDealDebitInitialBalanceCorrectionExistence(correctionId, user);

                var client = clientService.CheckClientExistence(correction.Deal.Client.Id, user);

                dealPaymentDocumentService.Delete(correction, user, currentDate);

                uow.Commit();

                return GetMainChangeableIndicators(client, user);
            }
        }

        #endregion

        #endregion

        #region Блокировка клиента

        public void SetClientBlockingValue(int clientId, byte blockingValue, UserInfo currentUser)
        {

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                if (blockingValue != 0 && blockingValue != 1)
                {
                    throw new Exception("Неверное значение входного параметра.");
                }

                var user = userService.CheckUserExistence(currentUser.Id);
                var client = clientService.CheckClientExistence(clientId, user);

                clientService.SetClientBlockingValue(client, blockingValue, user);

                uow.Commit();
            }
        }

        #endregion

        #region Удаление клиента

        public void Delete(int clientId, UserInfo currentUser)
        {

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var client = clientService.CheckClientExistence(clientId, user);

                clientService.Delete(client, user);

                uow.Commit();
            }
        }

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
            return taskPresenterMediator.GetTaskGridForClient(state, user);
        }

        #endregion

        #endregion

        #region Выбор клиента

        public ClientSelectViewModel SelectClient(UserInfo currentUser)
        {

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new ClientSelectViewModel();
                model.Title = "Выбор клиента";
                model.ClientsGrid = GetSelectClientGridLocal(new GridState() { PageSize = 5, Parameters = "", Sort = "Name=Asc" }, user);

                model.FilterData.Items.Add(new FilterTextEditor("Name", "Наименование"));
                model.FilterData.Items.Add(new FilterComboBox("Rating", "Рейтинг",
                    Enumerable.Range(0, 11).GetComboBoxItemList<int>(x => x.ToString(), x => x.ToString(), sort: false)));
                model.FilterData.Items.Add(new FilterComboBox("Type", "Тип",
                    ComboBoxBuilder.GetComboBoxItemList(clientTypeService.GetList(user), x => x.Name, x => x.Id.ToString())));
                model.FilterData.Items.Add(new FilterComboBox("Loyalty", "Лояльность", ComboBoxBuilder.GetComboBoxItemList<ClientLoyalty>(sort: false)));

                return model;
            }
        }

        public GridData GetSelectClientGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetSelectClientGridLocal(state, user);
            }
        }

        private GridData GetSelectClientGridLocal(GridState state, User user)
        {
            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(60));
            model.AddColumn("Name", "Наименование", Unit.Percentage(100));
            model.AddColumn("Loyalty", "Лояльность", Unit.Pixel(200));
            model.AddColumn("Rating", "Рейтинг", Unit.Pixel(50), GridCellStyle.Label, GridColumnAlign.Center);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var clients = clientService.GetFilteredList(state, user);
            foreach (var client in clients)
            {
                var actions = new GridActionCell("Action");
                actions.AddAction("Выбрать", "select_client");

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Name") { Value = client.Name },
                    new GridLabelCell("Loyalty") { Value = client.Loyalty.GetDisplayName() },
                    new GridLabelCell("Rating") { Value = client.Rating.ToString() },
                    new GridHiddenCell("Id") { Value = client.Id.ToString() }
                ));
            }

            model.State = state;
            model.Title = "Список клиентов";

            return model;
        }

        /// <summary>
        /// Получение фактического адреса клиента
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public object GetFactualAddress(int clientId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var client = clientService.CheckClientExistence(clientId, user);

                return client.FactualAddress;
            }
        }

        #endregion

        #endregion
    }
}