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
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;
using System.Web.Mvc;
using ERP.Wholesale.UI.ViewModels.User;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class DealPaymentPresenter : IDealPaymentPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IClientOrganizationService clientOrganizationService;
        private readonly IDealPaymentDocumentService dealPaymentDocumentService;
        private readonly IDealService dealService;
        private readonly IUserService userService;
        private readonly ITeamService teamService;

        private readonly IDealPaymentDocumentPresenterMediator dealPaymentDocumentPresenterMediator;

        #endregion

        #region Конструктор

        public DealPaymentPresenter(IUnitOfWorkFactory unitOfWorkFactory, IClientOrganizationService clientOrganizationService,
            IDealPaymentDocumentService dealPaymentDocumentService, IDealService dealService,
            IUserService userService,ITeamService teamService, IDealPaymentDocumentPresenterMediator dealPaymentDocumentPresenterMediator)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.clientOrganizationService = clientOrganizationService;
            this.dealPaymentDocumentService = dealPaymentDocumentService;
            this.dealService = dealService;
            this.userService = userService;
            this.teamService = teamService;

            this.dealPaymentDocumentPresenterMediator = dealPaymentDocumentPresenterMediator;
        }

        #endregion

        #region Методы

        #region Список

        public DealPaymentListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var model = new DealPaymentListViewModel();

                model.DealPaymentGrid = GetDealPaymentGridLocal(new GridState() { Sort = "Date=Desc;CreationDate=Desc" }, user);

                model.FilterData.Items.Add(new FilterTextEditor("PaymentDocumentNumber", "Номер документа"));
                model.FilterData.Items.Add(new FilterTextEditor("Deal_Contract_ContractorOrganization_ShortName", "Организация"));
                model.FilterData.Items.Add(new FilterDateRangePicker("Date", "Дата оплаты"));
                model.FilterData.Items.Add(new FilterTextEditor("Deal_Client_Name", "Клиент"));
                model.FilterData.Items.Add(new FilterComboBox("DealPaymentForm", "Форма оплаты", ComboBoxBuilder.GetComboBoxItemList<DealPaymentForm>(sort: false)));
                model.FilterData.Items.Add(new FilterTextEditor("Deal_Name", "Сделка"));
                model.FilterData.Items.Add(new FilterTextEditor("User_DisplayName", "Пользователь"));

                return model;
            }
        }

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

            user.CheckPermission(Permission.DealPayment_List_Details);

            GridData model = new GridData() { State = state, Title = "Оплаты" };

            model.ButtonPermissions["AllowToCreateDealPaymentFromClient"] = user.HasPermission(Permission.DealPaymentFromClient_Create_Edit);
            model.ButtonPermissions["AllowToCreateDealPaymentToClient"] = user.HasPermission(Permission.DealPaymentToClient_Create);

            model.AddColumn("Action", "Действие", Unit.Pixel(80));
            model.AddColumn("Date", "Дата", Unit.Pixel(54), align: GridColumnAlign.Center);
            model.AddColumn("PaymentDocumentNumber", "Номер документа", Unit.Pixel(90));
            model.AddColumn("ClientOrganizationName", "Организация", Unit.Percentage(28), GridCellStyle.Link);
            model.AddColumn("ClientOrganizationId", style: GridCellStyle.Hidden);
            model.AddColumn("ClientName", "Клиент", Unit.Percentage(28), GridCellStyle.Link);
            model.AddColumn("ClientId", style: GridCellStyle.Hidden);
            model.AddColumn("DealName", "Сделка", Unit.Percentage(28), GridCellStyle.Link);
            model.AddColumn("DealId", style: GridCellStyle.Hidden);
            model.AddColumn("DealPaymentForm", "Форма оплаты", Unit.Percentage(16));
            model.AddColumn("Sum", "Сумма оплаты", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Id", style: GridCellStyle.Hidden);

            var payments = dealPaymentDocumentService.GetDealPaymentFilteredList(state, user);

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
                    user.HasPermission(Permission.ClientOrganization_List_Details) ?
                        (GridCell)new GridLinkCell("ClientOrganizationName") { Value = payment.Deal.Contract.ContractorOrganization.ShortName } :
                        new GridLabelCell("ClientOrganizationName") { Value = payment.Deal.Contract.ContractorOrganization.ShortName },
                    new GridHiddenCell("ClientOrganizationId") { Value = payment.Deal.Contract.ContractorOrganization.Id.ToString() },
                    user.HasPermission(Permission.Client_List_Details) ?
                        (GridCell)new GridLinkCell("ClientName") { Value = payment.Deal.Client.Name } :
                        new GridLabelCell("ClientName") { Value = payment.Deal.Client.Name },
                    new GridHiddenCell("ClientId") { Value = payment.Deal.Client.Id.ToString() },
                    dealService.IsPossibilityToViewDetails(payment.Deal, user) ?
                        (GridCell)new GridLinkCell("DealName") { Value = payment.Deal.Name } :
                        new GridLabelCell("DealName") { Value = payment.Deal.Name },
                    new GridHiddenCell("DealId") { Value = payment.Deal.Id.ToString() },
                    new GridLabelCell("DealPaymentForm") { Value = payment.DealPaymentForm.GetDisplayName() },
                    new GridLabelCell("Sum") { Value = (payment.Type == DealPaymentDocumentType.DealPaymentFromClient ? payment.Sum : -payment.Sum).ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("Id") { Value = payment.Id.ToString() }
                    ) { Style = payment.Type == DealPaymentDocumentType.DealPaymentFromClient ? GridRowStyle.Normal : GridRowStyle.Warning });
            }

            return model;
        }

        #endregion

        #region Детали

        public DealPaymentFromClientDetailsViewModel DealPaymentFromClientDetails(Guid paymentId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var payment = dealPaymentDocumentService.CheckDealPaymentFromClientExistence(paymentId, user);

                user.CheckPermission(Permission.DealPayment_List_Details);

                var model = new DealPaymentFromClientDetailsViewModel()
                {
                    Title = "Детали оплаты",
                    PaymentId = payment.Id.ToString(),
                    PaymentDocumentNumber = payment.PaymentDocumentNumber,
                    DealName = payment.Deal.Name,
                    DealId = payment.Deal.Id.ToString(),
                    AllowToViewDealDetails = dealService.IsPossibilityToViewDetails(payment.Deal, user),
                    TeamName = payment.Team.Name,
                    TeamId = payment.Team.Id.ToString(),
                    AllowToViewTeamDetails = teamService.IsPossibilityToViewDetails(payment.Team, user),
                    Date = payment.Date.ToShortDateString(),
                    Sum = payment.Sum.ForDisplay(ValueDisplayType.Money),
                    UndistributedSum = payment.UndistributedSum.ForDisplay(ValueDisplayType.Money),
                    DistributedToDealDebitInitialBalanceCorrectionSum = payment.InitialBalancePaymentSum.ForDisplay(ValueDisplayType.Money),
                    DistributedToSaleWaybillSum = payment.DistributedToSaleWaybillSum.ForDisplay(ValueDisplayType.Money),
                    PaymentToClientSum = payment.PaymentToClientSum.ForDisplay(ValueDisplayType.Money),
                    PaymentFormName = payment.DealPaymentForm.GetDisplayName(),
                    AllowToDelete = dealPaymentDocumentService.IsPossibilityToDelete(payment, user),
                    TakenById = payment.User.Id.ToString(),
                    TakenByName = payment.User.DisplayName,
                    AllowToViewTakenByDetails = userService.IsPossibilityToViewDetails(payment.User, user),
                    AllowToChangeTakenBy = dealPaymentDocumentService.IsPossibilityToChangeTakenByInDealPaymentFromClient(payment, user),
                    
                    SaleWaybillGrid = dealPaymentDocumentPresenterMediator.GetSaleWaybillGridLocal(
                        new GridState() { Parameters = "DealPaymentId=" + paymentId + ";DocumentType=1", PageSize = 10 }, user),
                    DealDebitInitialBalanceCorrectionGrid = dealPaymentDocumentPresenterMediator.GetDealDebitInitialBalanceCorrectionGridLocal(
                        new GridState() { Parameters = "DealPaymentId=" + paymentId + ";DocumentType=1", PageSize = 10 }, user)
                };

                return model;
            }
        }

        /// <summary>
        /// Детали возврата оплаты клиенту
        /// </summary>
        public DealPaymentToClientDetailsViewModel DealPaymentToClientDetails(Guid paymentId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var payment = dealPaymentDocumentService.CheckDealPaymentToClientExistence(paymentId, user);

                user.CheckPermission(Permission.DealPayment_List_Details);

                var model = new DealPaymentToClientDetailsViewModel()
                {
                    Title = "Детали возврата оплаты клиенту",
                    PaymentId = paymentId.ToString(),
                    DealName = payment.Deal.Name,
                    DealId = payment.Deal.Id.ToString(),
                    AllowToViewDealDetails = dealService.IsPossibilityToViewDetails(payment.Deal, user),
                    TeamName = payment.Team.Name,
                    TeamId = payment.Team.Id.ToString(),
                    AllowToViewTeamDetails = teamService.IsPossibilityToViewDetails(payment.Team, user),
                    PaymentDocumentNumber = payment.PaymentDocumentNumber,
                    Date = payment.Date.ToShortDateString(),
                    DealPaymentForm = payment.DealPaymentForm.GetDisplayName(),
                    Sum = (-payment.Sum).ForDisplay(ValueDisplayType.Money),                    
                    AllowToDelete = dealPaymentDocumentService.IsPossibilityToDelete(payment, user),
                    ReturnedById = payment.User.Id.ToString(),
                    ReturnedByName = payment.User.DisplayName,
                    AllowToViewReturnedByDetails = userService.IsPossibilityToViewDetails(payment.User, user),
                    AllowToChangeReturnedBy = dealPaymentDocumentService.IsPossibilityToChangeReturnedByInDealPaymentToClient(payment, user),
                };

                return model;
            }
        }

        #endregion
       
        #region Удаление

        public void DeleteDealPaymentFromClient(Guid dealPaymentFromClientId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDate = DateTime.Now;

                var user = userService.CheckUserExistence(currentUser.Id);
                var dealPaymentFromClient = dealPaymentDocumentService.CheckDealPaymentFromClientExistence(dealPaymentFromClientId, user);

                dealPaymentDocumentService.Delete(dealPaymentFromClient, user, currentDate);

                uow.Commit();
            }
        }

        public void DeleteDealPaymentToClient(Guid dealPaymentToClientId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDate = DateTime.Now;

                var user = userService.CheckUserExistence(currentUser.Id);
                var dealPaymentToClient = dealPaymentDocumentService.CheckDealPaymentToClientExistence(dealPaymentToClientId, user);

                dealPaymentDocumentService.Delete(dealPaymentToClient, user, currentDate);

                uow.Commit();
            }
        }

        #endregion

        #region Создание и сохранение

        #region Оплаты от клиента

        /// <summary>
        /// По сделке
        /// </summary>
        /// <returns></returns>
        public DealPaymentFromClientEditViewModel CreateDealPaymentFromClient(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealPaymentFromClient_Create_Edit);    //Проверяем право на создание оплаты

                var model = new DealPaymentFromClientEditViewModel();

                model.Title = "Добавление оплаты по сделке";
                model.ClientName = "Выберите клиента";
                model.DealName = "Выберите сделку";
                model.Date = DateTime.Today.ToShortDateString();
                model.DealPaymentFormList = ComboBoxBuilder.GetComboBoxItemList<DealPaymentForm>(sort: false);
                model.MaxCashPaymentSum = dealService.GetMaxPossibleCashPaymentSum(null).ForEdit();
                model.DestinationDocumentSelectorControllerName = "DealPayment";
                model.DestinationDocumentSelectorActionName = "SaveDealPaymentFromClient";
                model.AllowToChangeDate = user.HasPermission(Permission.DealPayment_Date_Change);

                return model;
            }
        }

        public void SaveDealPaymentFromClient(DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var result = dealPaymentDocumentPresenterMediator.SaveDealPaymentFromClient<object>(model, currentUser);

                uow.Commit();
            }
        }      

        /// <summary>
        /// По организации клиента
        /// </summary>
        /// <returns></returns>
        public ClientOrganizationPaymentFromClientEditViewModel CreateClientOrganizationPaymentFromClient(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealPaymentFromClient_Create_Edit);    //Проверяем право на создание оплаты

                var model = new ClientOrganizationPaymentFromClientEditViewModel();

                model.Title = "Добавление оплаты по организации";
                model.ClientOrganizationName = "Выберите организацию";
                model.Date = DateTime.Today.ToShortDateString();
                model.DealPaymentFormList = ComboBoxBuilder.GetComboBoxItemList<DealPaymentForm>(sort: false);
                model.DestinationDocumentSelectorControllerName = "DealPayment";
                model.DestinationDocumentSelectorActionName = "SaveClientOrganizationPaymentFromClient";
                model.AllowToChangeDate = user.HasPermission(Permission.DealPayment_Date_Change);

                return model;
            }
        }

        public void SaveClientOrganizationPaymentFromClient(DestinationDocumentSelectForClientOrganizationPaymentFromClientDistributionViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                dealPaymentDocumentPresenterMediator.SaveClientOrganizationPaymentFromClient(model, currentUser);

                uow.Commit();
            }
        }

        #endregion

        #region Возврат оплаты клиенту

        public DealPaymentToClientEditViewModel CreateDealPaymentToClient(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealPaymentToClient_Create);

                var model = new DealPaymentToClientEditViewModel();
                model.ControllerName = "DealPayment";
                model.ActionName = "SaveDealPaymentToClient";
                model.Title = "Добавление возврата оплаты клиенту";
                model.Date = DateTime.Today.ToShortDateString();
                model.DealId = 0;
                model.DealName = "Выберите сделку";
                model.ClientId = 0;
                model.ClientName = "Выберите клиента";
                model.TeamId = 0;
                model.TeamList = ComboBoxBuilder.GetComboBoxItemList(new List<Team>(), x => x.Name, x => x.Id.ToString(), false);                
                model.DealPaymentFormList = ComboBoxBuilder.GetComboBoxItemList<DealPaymentForm>(sort: false);

                model.AllowToViewClientOrganization = false;
                model.AllowToViewClient = true;
                model.AllowToChooseClient = true;
                model.AllowToChooseDeal = true;
                model.IsDealSelectedByClient = true;
                model.AllowToChangeDate = user.HasPermission(Permission.DealPayment_Date_Change);

                return model;
            }
        }

        public void SaveDealPaymentToClient(DealPaymentToClientEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                dealPaymentDocumentPresenterMediator.SaveDealPaymentToClient<object>(model, currentUser);

                uow.Commit();
            }
        }

        /// <summary>
        /// Смена пользователя, вернувшего оплату клиенту
        /// </summary>
        public void ChangeReturnedByInPaymentToClient(Guid dealPaymentId, int newReturnedById, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var newReturnedBy = userService.CheckUserExistence(newReturnedById);
                var dealPaymentToClient = dealPaymentDocumentService.CheckDealPaymentToClientExistence(dealPaymentId, user);

                dealPaymentDocumentService.CheckPossibilityToChangeReturnedByInDealPaymentToClient(dealPaymentToClient, user);

                dealPaymentToClient.User = newReturnedBy;

                uow.Commit();
            }
        }

        /// <summary>
        /// Смена пользователя, принявшего оплату клиенту
        /// </summary>
        public void ChangeTakenByInPaymentFromClient(Guid dealPaymentId, int newTakenById, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var newTakenBy = userService.CheckUserExistence(newTakenById);
                var dealPaymentFromClient = dealPaymentDocumentService.CheckDealPaymentFromClientExistence(dealPaymentId, user);

                dealPaymentDocumentService.CheckPossibilityToChangeTakenByInDealPaymentFromClient(dealPaymentFromClient, user);

                dealPaymentFromClient.User = newTakenBy;

                uow.Commit();
            }
        }
        #endregion        

        #endregion

        

        #region Модальная форма выбора документов для ручного разнесения платежных документов

        /// <summary>
        /// Модальная форма выбора документов для ручного разнесения оплаты от клиента по организации клиента
        /// </summary>
        /// <param name="model">Модель, заполненная на предыдущем этапе (в форме параметров оплаты при создании)</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        public DestinationDocumentSelectForClientOrganizationPaymentFromClientDistributionViewModel SelectDestinationDocumentsForClientOrganizationPaymentFromClientDistribution(
            ClientOrganizationPaymentFromClientEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                ClientOrganization clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(ValidationUtils.TryGetInt(model.ClientOrganizationId),
                    user);
                var teamList = ComboBoxBuilder.GetComboBoxItemList<Team>(dealService.GetTeamListForDealDocumentByClientOrganization(clientOrganization, user),
                    x => x.Name, x => x.Id.ToString(), false);
                var defaultTeamId = teamList.First().Value; //т.к. пользователь входит по крайней мере в одну команду

                decimal sum = ValidationUtils.TryGetDecimal(model.Sum);

                var result = new DestinationDocumentSelectForClientOrganizationPaymentFromClientDistributionViewModel
                {
                    Title = "Выбор документов для оплаты",

                    DestinationDocumentSelectorControllerName = model.DestinationDocumentSelectorControllerName,
                    DestinationDocumentSelectorActionName = model.DestinationDocumentSelectorActionName,

                    TeamList = teamList,
                    TakenByList = userService.GetListByTeam(ValidationUtils.TryGetShort(defaultTeamId), false).GetComboBoxItemList<User>(x => x.DisplayName, x => x.Id.ToString(), true),
                    AllowToChangeTakenBy = true,
                    
                    ClientOrganizationId = clientOrganization.Id,
                    ClientOrganizationName = clientOrganization.ShortName,
                    CurrentOrdinalNumber = 1,
                    Date = model.Date,
                    DealPaymentForm = model.DealPaymentForm,
                    PaymentDocumentNumber = model.PaymentDocumentNumber,
                    PaymentFormName = ((DealPaymentForm)model.DealPaymentForm).GetDisplayName(),
                    SumString = sum.ForDisplay(ValueDisplayType.Money),
                    SumValue = sum.ForEdit(),
                    UndistributedSumString = sum.ForDisplay(ValueDisplayType.Money),
                    UndistributedSumValue = sum.ForEdit(),
                    SaleWaybillGridData = dealPaymentDocumentPresenterMediator.GetSaleWaybillSelectGridLocal(
                        new GridState { Parameters = "ClientOrganizationId=" + model.ClientOrganizationId + ";IsDealPayment=true;TeamId=" + defaultTeamId }, user),
                    DealDebitInitialBalanceCorrectionGridData = dealPaymentDocumentPresenterMediator.GetDealDebitInitialBalanceCorrectionSelectGridLocal(
                        new GridState { Parameters = "ClientOrganizationId=" + model.ClientOrganizationId + ";IsDealPayment=true;TeamId=" + defaultTeamId }, user)
                };

                return result;
            }
        }

        /// <summary>
        /// Получение грида реализаций для организации клиента для разнесения оплаты
        /// </summary>
        /// <param name="clientOrganizationId">Код организации клиента</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        public GridData ShowDestinationSaleGridForClientOrganizationPaymentFromClientDistribution(int clientOrganizationId, short teamId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(clientOrganizationId, user);
                var team = teamService.CheckTeamExistence(teamId);

                var teamList = dealService.GetTeamListForDealDocumentByClientOrganization(clientOrganization, user);
                // TODO Сформулировать сообщение информативнее
                ValidationUtils.Assert(teamList.Contains(team), String.Format("Невозможно создать оплату для команды «0».", team.Name));

                return dealPaymentDocumentPresenterMediator.GetSaleWaybillSelectGridLocal(
                       new GridState { Parameters = "ClientOrganizationId=" + clientOrganizationId.ToString() + ";IsDealPayment=true;TeamId=" + teamId.ToString() }, user);
            }
        }

        /// <summary>
        /// Получение грида платежных документов для организации клиента для разнесения оплаты
        /// </summary>
        /// <param name="clientOrganizationId">Код организации клиента</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        public GridData ShowDestinationPaymentDocumentGridForClientOrganizationPaymentFromClientDistribution(int clientOrganizationId, short teamId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(clientOrganizationId, user);
                var team = teamService.CheckTeamExistence(teamId);

                var teamList = dealService.GetTeamListForDealDocumentByClientOrganization(clientOrganization, user);
                // TODO Сформулировать сообщение информативнее
                ValidationUtils.Assert(teamList.Contains(team), String.Format("Невозможно создать оплату для команды «0».", team.Name));

                return dealPaymentDocumentPresenterMediator.GetDealDebitInitialBalanceCorrectionSelectGridLocal(
                        new GridState { Parameters = "ClientOrganizationId=" + clientOrganizationId.ToString() + ";IsDealPayment=true;TeamId=" + teamId.ToString() }, user);
            }
        }

        /// <summary>
        /// Модальная форма выбора документов для ручного разнесения оплаты от клиента по сделке
        /// </summary>
        /// <param name="model">Модель, заполненная на предыдущем этапе (в форме параметров оплаты при создании)</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        public DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel SelectDestinationDocumentsForDealPaymentFromClientDistribution(
            DealPaymentFromClientEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                var deal = dealService.CheckDealExistence(ValidationUtils.TryGetInt(model.DealId), user);
                dealPaymentDocumentService.CheckPossibilityToCreateDealPaymentFromClient(deal, user);

                var teamList = ComboBoxBuilder.GetComboBoxItemList<Team>(dealService.GetTeamListForDealDocumentByDeal(deal, user), x => x.Name, x => x.Id.ToString(), false);
                var defaultTeamId = teamList.First().Value; //т.к. пользователь входит по крайней мере в одну команду

                decimal sum = ValidationUtils.TryGetDecimal(model.Sum);

                var result = new DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel
                {
                    Title = "Выбор документов для оплаты",
                    IsNew = true,

                    DestinationDocumentSelectorControllerName = model.DestinationDocumentSelectorControllerName,
                    DestinationDocumentSelectorActionName = model.DestinationDocumentSelectorActionName,

                    CurrentOrdinalNumber = 1,
                    Date = model.Date,
                    DealId = deal.Id,
                    DealName = deal.Name,
                    TeamList = teamList,
                    TakenByList = userService.GetListByTeam(ValidationUtils.TryGetShort(defaultTeamId), false).GetComboBoxItemList<User>(x => x.DisplayName, x => x.Id.ToString(), true),
                    AllowToChangeTakenBy = true,
                    DealPaymentForm = ValidationUtils.TryGetByte(model.DealPaymentForm),
                    PaymentDocumentNumber = model.PaymentDocumentNumber,
                    PaymentFormName = ((DealPaymentForm)ValidationUtils.TryGetByte(model.DealPaymentForm)).GetDisplayName(),
                    SumString = sum.ForDisplay(ValueDisplayType.Money),
                    SumValue = sum.ForEdit(),
                    UndistributedSumString = sum.ForDisplay(ValueDisplayType.Money),
                    UndistributedSumValue = sum.ForEdit(),
                    SaleWaybillGridData = dealPaymentDocumentPresenterMediator.GetSaleWaybillSelectGridLocal(
                        new GridState { Parameters = "DealId=" + deal.Id + ";IsDealPayment=true;TeamId=" + defaultTeamId }, user),
                    DealDebitInitialBalanceCorrectionGridData = dealPaymentDocumentPresenterMediator.GetDealDebitInitialBalanceCorrectionSelectGridLocal(
                            new GridState { Parameters = "DealId=" + deal.Id + ";IsDealPayment=true;TeamId=" + defaultTeamId }, user)
                };

                return result;
            }
        }

        /// <summary>
        /// Получение модели для грида реализаций при ручном разнесении оплаты
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        public GridData ShowDestinationSaleGridForDealPaymentFromClientDistribution(int dealId, short teamId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var team = teamService.CheckTeamExistence(teamId);

                var deal = dealService.CheckDealExistence(dealId, user);
                dealPaymentDocumentService.CheckPossibilityToCreateDealPaymentFromClient(deal, user);

                var teamList = dealService.GetTeamListForDealDocumentByDeal(deal, user);
                // TODO Сформулировать сообщение информативнее
                ValidationUtils.Assert(teamList.Contains(team), String.Format("Невозможно создать оплату для команды «0».", team.Name));

                return dealPaymentDocumentPresenterMediator.GetSaleWaybillSelectGridLocal(
                    new GridState { Parameters = "DealId=" + deal.Id + ";IsDealPayment=true;TeamId=" + team.Id }, user);
            }
        }

        /// <summary>
        /// Получение модели для грида платежных документов при ручном разнесении оплаты
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        public GridData ShowDestinationPaymentDocumentGridForDealPaymentFromClientDistribution(int dealId, short teamId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var team = teamService.CheckTeamExistence(teamId);

                var deal = dealService.CheckDealExistence(dealId, user);
                dealPaymentDocumentService.CheckPossibilityToCreateDealPaymentFromClient(deal, user);

                var teamList = dealService.GetTeamListForDealDocumentByDeal(deal, user);
                // TODO Сформулировать сообщение информативнее
                ValidationUtils.Assert(teamList.Contains(team), String.Format("Невозможно создать оплату для команды «0».", team.Name));

                return dealPaymentDocumentPresenterMediator.GetDealDebitInitialBalanceCorrectionSelectGridLocal(
                    new GridState { Parameters = "DealId=" + deal.Id + ";IsDealPayment=true;TeamId=" + team.Id }, user);
            }
        }

        /// <summary>
        /// Модальная форма выбора документов для ручного переразнесения оплаты от клиента по сделке
        /// </summary>
        /// <param name="dealPaymentFromClientId">Код оплаты от клиента по сделке</param>
        /// <param name="destinationDocumentSelectorControllerName">Название контроллера, который будет вызываться при submit формы разнесения</param>
        /// <param name="destinationDocumentSelectorActionName">Название метода контроллера, который будет вызываться при submit формы разнесения</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        public DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel SelectDestinationDocumentsForDealPaymentFromClientRedistribution(
            Guid dealPaymentFromClientId, string destinationDocumentSelectorControllerName, string destinationDocumentSelectorActionName, UserInfo currentUser)
        {
            var user = userService.CheckUserExistence(currentUser.Id);
            var dealPaymentFromClient = dealPaymentDocumentService.CheckDealPaymentFromClientExistence(dealPaymentFromClientId, user);
            var team = dealPaymentFromClient.Team;
            var teamList = new List<SelectListItem> { new SelectListItem() { Text = team.Name, Value = team.Id.ToString() } };

            dealPaymentDocumentService.CheckPossibilityToRedistribute(dealPaymentFromClient, user);

            var result = new DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel
            {
                Title = "Выбор документов для оплаты",
                IsNew = false,

                DestinationDocumentSelectorControllerName = destinationDocumentSelectorControllerName,
                DestinationDocumentSelectorActionName = destinationDocumentSelectorActionName,

                TeamId = team.Id,
                TeamList = teamList,
                TakenById = dealPaymentFromClient.User.Id.ToString(),
                TakenByList = userService.GetListByTeam(team.Id, false).GetComboBoxItemList<User>(x => x.DisplayName, x => x.Id.ToString(), false),
                AllowToChangeTakenBy = dealPaymentDocumentService.IsPossibilityToChangeTakenByInDealPaymentFromClient(dealPaymentFromClient, user),

                CurrentOrdinalNumber = 1,
                Date = dealPaymentFromClient.Date.ToShortDateString(),
                DealId = dealPaymentFromClient.Deal.Id,
                DealName = dealPaymentFromClient.Deal.Name,
                DealPaymentDocumentId = dealPaymentFromClientId.ToString(),
                DealPaymentForm = (byte)dealPaymentFromClient.DealPaymentForm,
                PaymentDocumentNumber = dealPaymentFromClient.PaymentDocumentNumber,
                PaymentFormName = dealPaymentFromClient.DealPaymentForm.GetDisplayName(),
                SumString = dealPaymentFromClient.Sum.ForDisplay(ValueDisplayType.Money),
                SumValue = dealPaymentFromClient.Sum.ForEdit(),
                UndistributedSumString = dealPaymentFromClient.UndistributedSum.ForDisplay(ValueDisplayType.Money),
                UndistributedSumValue = dealPaymentFromClient.UndistributedSum.ForEdit(),
                SaleWaybillGridData = dealPaymentDocumentPresenterMediator.GetSaleWaybillSelectGridLocal(
                    new GridState { Parameters = "DealId=" + dealPaymentFromClient.Deal.Id + ";IsDealPayment=true;TeamId=" + team.Id.ToString() }, user),
                DealDebitInitialBalanceCorrectionGridData = dealPaymentDocumentPresenterMediator.GetDealDebitInitialBalanceCorrectionSelectGridLocal(
                    new GridState { Parameters = "DealId=" + dealPaymentFromClient.Deal.Id + ";IsDealPayment=true;TeamId=" + team.Id.ToString() }, user)
            };

            return result;
        }

        #endregion

        #endregion
    }
}