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
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class DealInitialBalanceCorrectionPresenter : IDealInitialBalanceCorrectionPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IDealPaymentDocumentService dealPaymentDocumentService;
        private readonly IDealService dealService;
        private readonly IUserService userService;
        private readonly ITeamService teamService;

        private readonly IDealPaymentDocumentPresenterMediator dealPaymentDocumentPresenterMediator;

        #endregion

        #region Конструктор

        public DealInitialBalanceCorrectionPresenter(IUnitOfWorkFactory unitOfWorkFactory,
            IDealPaymentDocumentService dealPaymentDocumentService, IDealService dealService,
            IUserService userService, ITeamService teamService, IDealPaymentDocumentPresenterMediator dealPaymentDocumentPresenterMediator)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.dealPaymentDocumentService = dealPaymentDocumentService;
            this.dealService = dealService;
            this.userService = userService;
            this.teamService = teamService;

            this.dealPaymentDocumentPresenterMediator = dealPaymentDocumentPresenterMediator;
        }

        #endregion

        #region Методы

        #region Список

        public DealInitialBalanceCorrectionListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var model = new DealInitialBalanceCorrectionListViewModel();

                model.DealInitialBalanceCorrectionGrid = GetDealInitialBalanceCorrectionGridLocal(new GridState() { Sort = "Date=Desc;CreationDate=Desc" }, user);

                model.FilterData.Items.Add(new FilterTextEditor("CorrectionReason", "Причина корректировки"));
                model.FilterData.Items.Add(new FilterTextEditor("Deal_Contract_ContractorOrganization_ShortName", "Организация"));
                model.FilterData.Items.Add(new FilterDateRangePicker("Date", "Дата корректировки"));
                model.FilterData.Items.Add(new FilterTextEditor("Deal_Client_Name", "Клиент"));
                model.FilterData.Items.Add(new FilterTextEditor("Deal_Name", "Сделка"));

                return model;
            }
        }

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
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            user.CheckPermission(Permission.DealInitialBalanceCorrection_List_Details);

            GridData model = new GridData() { State = state, Title = "Корректировки сальдо" };

            model.ButtonPermissions["AllowToCreateDealDebitInitialBalanceCorrection"] = user.HasPermission(Permission.DealDebitInitialBalanceCorrection_Create);
            model.ButtonPermissions["AllowToCreateDealCreditInitialBalanceCorrection"] = user.HasPermission(Permission.DealCreditInitialBalanceCorrection_Create_Edit);

            model.AddColumn("Action", "Действие", Unit.Pixel(80));
            model.AddColumn("Date", "Дата", Unit.Pixel(54), align: GridColumnAlign.Center);
            model.AddColumn("Reason", "Причина корректировки", Unit.Percentage(28));
            model.AddColumn("ClientOrganizationName", "Организация", Unit.Percentage(24), GridCellStyle.Link);
            model.AddColumn("ClientOrganizationId", style: GridCellStyle.Hidden);
            model.AddColumn("ClientName", "Клиент", Unit.Percentage(24), GridCellStyle.Link);
            model.AddColumn("ClientId", style: GridCellStyle.Hidden);
            model.AddColumn("DealName", "Сделка", Unit.Percentage(24), GridCellStyle.Link);
            model.AddColumn("DealId", style: GridCellStyle.Hidden);
            model.AddColumn("Sum", "Сумма корректировки", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("CorrectionId", style: GridCellStyle.Hidden);

            var dealInitialBalanceCorrections = dealPaymentDocumentService.GetDealInitialBalanceCorrectionFilteredList(state, user);

            // Подгружаем коллекции разнесений документов, т.к. они обязательно будут использоваться
            dealPaymentDocumentService.LoadDealPaymentDocumentDistributions(dealInitialBalanceCorrections);

            foreach (var dealInitialBalanceCorrection in dealInitialBalanceCorrections)
            {
                var actions = new GridActionCell("Action");
                if (dealInitialBalanceCorrection.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection)
                {
                    actions.AddAction("Дет.", "linkDealDebitInitialBalanceCorrectionDetails");

                    if (dealPaymentDocumentService.IsPossibilityToDelete(dealInitialBalanceCorrection, user))
                    {
                        actions.AddAction("Удал.", "linkDealDebitInitialBalanceCorrectionDelete");
                    }
                }
                else if (dealInitialBalanceCorrection.Type == DealPaymentDocumentType.DealCreditInitialBalanceCorrection)
                {
                    actions.AddAction("Дет.", "linkDealCreditInitialBalanceCorrectionDetails");

                    if (dealPaymentDocumentService.IsPossibilityToRedistribute(dealInitialBalanceCorrection.As<DealCreditInitialBalanceCorrection>(), user))
                    {
                        actions.AddAction("Ред.", "linkDealCreditInitialBalanceCorrectionEdit");
                    }

                    if (dealPaymentDocumentService.IsPossibilityToDelete(dealInitialBalanceCorrection, user))
                    {
                        actions.AddAction("Удал.", "linkDealCreditInitialBalanceCorrectionDelete");
                    }
                }

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Date") { Value = dealInitialBalanceCorrection.Date.ToShortDateString() },
                    new GridLabelCell("Reason") { Value = dealInitialBalanceCorrection.CorrectionReason },
                    user.HasPermission(Permission.ClientOrganization_List_Details) ?
                        (GridCell)new GridLinkCell("ClientOrganizationName") { Value = dealInitialBalanceCorrection.Deal.Contract.ContractorOrganization.ShortName } :
                        new GridLabelCell("ClientOrganizationName") { Value = dealInitialBalanceCorrection.Deal.Contract.ContractorOrganization.ShortName },
                    new GridHiddenCell("ClientOrganizationId") { Value = dealInitialBalanceCorrection.Deal.Contract.ContractorOrganization.Id.ToString() },
                    user.HasPermission(Permission.Client_List_Details) ?
                        (GridCell)new GridLinkCell("ClientName") { Value = dealInitialBalanceCorrection.Deal.Client.Name } :
                        new GridLabelCell("ClientName") { Value = dealInitialBalanceCorrection.Deal.Client.Name },
                    new GridHiddenCell("ClientId") { Value = dealInitialBalanceCorrection.Deal.Client.Id.ToString() },
                    dealService.IsPossibilityToViewDetails(dealInitialBalanceCorrection.Deal, user) ?
                        (GridCell)new GridLinkCell("DealName") { Value = dealInitialBalanceCorrection.Deal.Name } :
                        new GridLabelCell("DealName") { Value = dealInitialBalanceCorrection.Deal.Name },
                    new GridHiddenCell("DealId") { Value = dealInitialBalanceCorrection.Deal.Id.ToString() },
                    new GridLabelCell("Sum") { Value = (dealInitialBalanceCorrection.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection ? dealInitialBalanceCorrection.Sum : -dealInitialBalanceCorrection.Sum).ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("CorrectionId") { Value = dealInitialBalanceCorrection.Id.ToString() }
                    ) { Style = dealInitialBalanceCorrection.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection ? GridRowStyle.Normal : GridRowStyle.Warning });
            }

            return model;
        }

        #endregion

        #region Детали

        /// <summary>
        /// Кредитовая корректировка
        /// </summary>
        public DealCreditInitialBalanceCorrectionDetailsViewModel DealCreditInitialBalanceCorrectionDetails(Guid correctionId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealInitialBalanceCorrection_List_Details); // Проверяем право на просмотр

                var correction = dealPaymentDocumentService.CheckDealCreditInitialBalanceCorrectionExistence(correctionId, user);
                var deal = correction.Deal;
                var client = deal.Client;

                var model = new DealCreditInitialBalanceCorrectionDetailsViewModel();

                model.Title = "Детали кредитовой корректировки";
                model.DealCreditInitialBalanceCorrectionId = correction.Id.ToString();
                model.DealId = deal.Id.ToString();
                model.DealName = deal.Name;
                model.AllowToViewDealDetails = dealService.IsPossibilityToViewDetails(deal, user);

                model.TeamName = correction.Team.Name;
                model.TeamId = correction.Team.Id.ToString();
                model.AllowToViewTeamDetails = teamService.IsPossibilityToViewDetails(correction.Team, user);

                model.ClientName = client.Name;
                model.Date = correction.Date.ToShortDateString();

                model.CorrectionReason = correction.CorrectionReason;

                model.UndistributedSum = correction.UndistributedSum.ForDisplay(ValueDisplayType.Money);
                model.Sum = correction.Sum.ForDisplay(ValueDisplayType.Money);
                model.PaymentToClientSum = correction.PaymentToClientSum.ForDisplay(ValueDisplayType.Money);
                model.DistributedToDealDebitInitialBalanceCorrectionSum = correction.InitialBalancePaymentSum.ForDisplay(ValueDisplayType.Money);
                model.DistributedToSaleWaybillSum = correction.DistributedToSaleWaybillSum.ForDisplay(ValueDisplayType.Money);

                model.AllowToDelete = dealPaymentDocumentService.IsPossibilityToDelete(correction, user);

                model.SaleWaybillGrid = dealPaymentDocumentPresenterMediator.GetSaleWaybillGridLocal(
                    new GridState() { Parameters = "DealCreditInitialBalanceCorrectionId=" + correctionId + ";DocumentType=2", PageSize = 10 }, user);
                model.DealDebitInitialBalanceCorrectionGrid = dealPaymentDocumentPresenterMediator.GetDealDebitInitialBalanceCorrectionGridLocal(
                    new GridState() { Parameters = "DealCreditInitialBalanceCorrectionId=" + correctionId + ";DocumentType=2", PageSize = 10 }, user);

                return model;
            }
        }

        /// <summary>
        /// Дебетовая корректировка
        /// </summary>
        public DealDebitInitialBalanceCorrectionDetailsViewModel DealDebitInitialBalanceCorrectionDetails(Guid correctionId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealInitialBalanceCorrection_List_Details); // Проверяем право на просмотр

                var correction = dealPaymentDocumentService.CheckDealDebitInitialBalanceCorrectionExistence(correctionId, user);
                var deal = correction.Deal;
                var client = deal.Client;

                var model = new DealDebitInitialBalanceCorrectionDetailsViewModel();

                model.Title = "Детали дебетовой корректировки";
                model.DealDebitInitialBalanceCorrectionId = correction.Id.ToString();
                model.DealName = deal.Name;
                model.DealId = deal.Id.ToString();
                model.AllowToViewDealDetails = dealService.IsPossibilityToViewDetails(deal, user);
                model.ClientName = client.Name;

                model.TeamName = correction.Team.Name;
                model.TeamId = correction.Team.Id.ToString();
                model.AllowToViewTeamDetails = teamService.IsPossibilityToViewDetails(correction.Team, user);

                model.Date = correction.Date.ToShortDateString();

                model.CorrectionReason = correction.CorrectionReason;

                model.Sum = correction.Sum.ForDisplay(ValueDisplayType.Money);

                model.AllowToDelete = dealPaymentDocumentService.IsPossibilityToDelete(correction, user);

                return model;
            }
        }

        #endregion

        #region Удаление

        public void DeleteDealCreditInitialBalanceCorrection(Guid correctionId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDate = DateTime.Now;

                var user = userService.CheckUserExistence(currentUser.Id);
                var correction = dealPaymentDocumentService.CheckDealCreditInitialBalanceCorrectionExistence(correctionId, user);

                dealPaymentDocumentService.Delete(correction, user, currentDate);

                uow.Commit();
            }
        }

        public void DeleteDealDebitInitialBalanceCorrection(Guid correctionId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDate = DateTime.Now;

                var user = userService.CheckUserExistence(currentUser.Id);
                var correction = dealPaymentDocumentService.CheckDealDebitInitialBalanceCorrectionExistence(correctionId, user);

                dealPaymentDocumentService.Delete(correction, user, currentDate);

                uow.Commit();
            }
        }

        #endregion

        #region Создание

        /// <summary>
        /// Кредитовая корректировка
        /// </summary>
        /// <returns></returns>
        public DealCreditInitialBalanceCorrectionEditViewModel CreateDealCreditInitialBalanceCorrection(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealCreditInitialBalanceCorrection_Create_Edit);    //Проверяем право на создание корректировки

                var model = new DealCreditInitialBalanceCorrectionEditViewModel();

                model.Title = "Добавление кредитовой корректировки";
                model.ClientName = "Выберите клиента";
                model.DealName = "Выберите сделку";
                model.Date = DateTime.Today.ToShortDateString();

                model.DestinationDocumentSelectorControllerName = "DealInitialBalanceCorrection";
                model.DestinationDocumentSelectorActionName = "SaveDealCreditInitialBalanceCorrection";

                model.AllowToViewClientOrganization = false;
                model.AllowToViewClient = true;
                model.AllowToChooseClient = true;
                model.AllowToChooseDeal = true;
                model.IsDealSelectedByClient = true;

                model.AllowToChangeDate = user.HasPermission(Permission.DealInitialBalanceCorrection_Date_Change);

                return model;
            }
        }

        /// <summary>
        /// Дебетовая корректировка
        /// </summary>
        public DealDebitInitialBalanceCorrectionEditViewModel CreateDealDebitInitialBalanceCorrection(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealDebitInitialBalanceCorrection_Create);    //Проверяем право на создание корректировки

                var model = new DealDebitInitialBalanceCorrectionEditViewModel();

                model.Title = "Добавление дебетовой корректировки";
                model.ClientName = "Выберите клиента";
                model.DealName = "Выберите сделку";
                model.Date = DateTime.Today.ToShortDateString();

                model.ControllerName = "DealInitialBalanceCorrection";
                model.ActionName = "SaveDealDebitInitialBalanceCorrection";

                model.AllowToViewClientOrganization = false;
                model.AllowToViewClient = true;
                model.AllowToChooseClient = true;
                model.AllowToChooseDeal = true;
                model.IsDealSelectedByClient = true;

                model.AllowToChangeDate = user.HasPermission(Permission.DealInitialBalanceCorrection_Date_Change);

                model.TeamId = 0;
                model.TeamList = ComboBoxBuilder.GetComboBoxItemList(new List<Team>(), x => x.Name, x => x.Id.ToString(), false);

                return model;
            }
        }

        #endregion

        #region Сохранение

        public void SaveDealDebitInitialBalanceCorrection(DealDebitInitialBalanceCorrectionEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                dealPaymentDocumentPresenterMediator.SaveDealDebitInitialBalanceCorrection<object>(model, currentUser);

                uow.Commit();
            }
        }

        public void SaveDealCreditInitialBalanceCorrection(DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                dealPaymentDocumentPresenterMediator.SaveDealCreditInitialBalanceCorrection<object>(model, currentUser);

                uow.Commit();
            }
        }

        #endregion

        #region Модальная форма выбора документов для ручного разнесения платежных документов

        /// <summary>
        /// Модальная форма выбора документов для ручного разнесения кредитовой корректировки сальдо по сделке
        /// </summary>
        /// <param name="model">Модель, заполненная на предыдущем этапе (в форме параметров кредитовой корректировки при создании)</param>
        /// <param name="currentUser">Пользователь</param>
        public DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel SelectDestinationDocumentsForDealCreditInitialBalanceCorrectionDistribution(
            DealCreditInitialBalanceCorrectionEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                var deal = dealService.CheckDealExistence(model.DealId, user);
                dealPaymentDocumentService.CheckPossibilityToCreateDealCreditInitialBalanceCorrection(deal, user);
                var teamList = ComboBoxBuilder.GetComboBoxItemList<Team>(dealService.GetTeamListForDealDocumentByDeal(deal, user), 
                    x => x.Name, x => x.Id.ToString(), false);
                var defaultTeamId = teamList.First().Value; // т.к. пользователь должен входить по крайней мере в одну команду

                decimal sum = ValidationUtils.TryGetDecimal(model.Sum);

                var result = new DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel
                {
                    Title = "Выбор документов для кредитовой корректировки сальдо",
                    IsNew = true,

                    DestinationDocumentSelectorControllerName = model.DestinationDocumentSelectorControllerName,
                    DestinationDocumentSelectorActionName = model.DestinationDocumentSelectorActionName,

                    TeamList = teamList,
                    CorrectionReason = model.CorrectionReason,
                    CurrentOrdinalNumber = 1,
                    Date = model.Date,
                    DealId = deal.Id,
                    DealName = deal.Name,
                    SumString = sum.ForDisplay(ValueDisplayType.Money),
                    SumValue = sum.ForEdit(),
                    UndistributedSumString = sum.ForDisplay(ValueDisplayType.Money),
                    UndistributedSumValue = sum.ForEdit(),
                    SaleWaybillGridData = dealPaymentDocumentPresenterMediator.GetSaleWaybillSelectGridLocal(
                        new GridState { Parameters = "DealId=" + deal.Id + ";IsDealPayment=false;TeamId=" + defaultTeamId }, user),
                    DealDebitInitialBalanceCorrectionGridData = dealPaymentDocumentPresenterMediator.GetDealDebitInitialBalanceCorrectionSelectGridLocal(
                        new GridState { Parameters = "DealId=" + deal.Id + ";IsDealPayment=false;TeamId=" + defaultTeamId }, user)
                };

                return result;
            }
        }

        /// <summary>
        /// Грид реализаций для оплаты кредитовой корректировкой
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        public GridData ShowDestinationSaleGridForDealCreditInitialBalanceCorrectionDistribution(int dealId, short teamId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);
                var team = teamService.CheckTeamExistence(teamId);

                var teamList = dealService.GetTeamListForDealDocumentByDeal(deal, user);
                // TODO Сформулировать сообщение информативнее
                ValidationUtils.Assert(teamList.Contains(team), String.Format("Невозможно создать корректировку для команды «0».", team.Name));

                return dealPaymentDocumentPresenterMediator.GetSaleWaybillSelectGridLocal(
                        new GridState { Parameters = "DealId=" + deal.Id + ";IsDealPayment=false;TeamId=" + team.Id.ToString() }, user);
            }
        }

        /// <summary>
        /// Грид документов для оплаты кредитовой корректировкой
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        public GridData ShowDestinationDocumentGridForDealCreditInitialBalanceCorrectionDistribution(int dealId, short teamId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);
                var team = teamService.CheckTeamExistence(teamId);

                var teamList = dealService.GetTeamListForDealDocumentByDeal(deal, user);
                // TODO Сформулировать сообщение информативнее
                ValidationUtils.Assert(teamList.Contains(team), String.Format("Невозможно создать корректировку для команды «0».", team.Name));

                return dealPaymentDocumentPresenterMediator.GetDealDebitInitialBalanceCorrectionSelectGridLocal(
                        new GridState { Parameters = "DealId=" + deal.Id + ";IsDealPayment=false;TeamId=" + team.Id.ToString() }, user);
            }
        }

        /// <summary>
        /// Модальная форма выбора документов для ручного переразнесения кредитовой корректировки сальдо по сделке
        /// </summary>
        /// <param name="dealCreditInitialBalanceCorrectionId">Код кредитовой корректировки сальдо по сделке</param>
        /// <param name="destinationDocumentSelectorControllerName">Название контроллера, который будет вызываться при submit формы разнесения</param>
        /// <param name="destinationDocumentSelectorActionName">Название метода контроллера, который будет вызываться при submit формы разнесения</param>
        /// <param name="currentUser">Пользователь</param>
        public DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel SelectDestinationDocumentsForDealCreditInitialBalanceCorrectionRedistribution(
            Guid dealCreditInitialBalanceCorrectionId, string destinationDocumentSelectorControllerName, string destinationDocumentSelectorActionName, UserInfo currentUser)
        {
            var user = userService.CheckUserExistence(currentUser.Id);
            var dealCreditInitialBalanceCorrection = dealPaymentDocumentService.CheckDealCreditInitialBalanceCorrectionExistence(dealCreditInitialBalanceCorrectionId, user);
            dealPaymentDocumentService.CheckPossibilityToRedistribute(dealCreditInitialBalanceCorrection, user);
            var team = dealCreditInitialBalanceCorrection.Team;
            var teamList = new List<SelectListItem> { new SelectListItem() { Text = team.Name, Value = team.Id.ToString() } };

            var result = new DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel
            {
                Title = "Выбор документов для кредитовой корректировки сальдо",
                IsNew = false,

                DestinationDocumentSelectorControllerName = destinationDocumentSelectorControllerName,
                DestinationDocumentSelectorActionName = destinationDocumentSelectorActionName,

                TeamId = team.Id,
                TeamList = teamList,
                CorrectionReason = dealCreditInitialBalanceCorrection.CorrectionReason,
                CurrentOrdinalNumber = 1,
                Date = dealCreditInitialBalanceCorrection.Date.ToShortDateString(),
                DealId = dealCreditInitialBalanceCorrection.Deal.Id,
                DealName = dealCreditInitialBalanceCorrection.Deal.Name,
                DealPaymentDocumentId = dealCreditInitialBalanceCorrectionId.ToString(),
                SumString = dealCreditInitialBalanceCorrection.Sum.ForDisplay(ValueDisplayType.Money),
                SumValue = dealCreditInitialBalanceCorrection.Sum.ForEdit(),
                UndistributedSumString = dealCreditInitialBalanceCorrection.UndistributedSum.ForDisplay(ValueDisplayType.Money),
                UndistributedSumValue = dealCreditInitialBalanceCorrection.UndistributedSum.ForEdit(),
                SaleWaybillGridData = dealPaymentDocumentPresenterMediator.GetSaleWaybillSelectGridLocal(
                    new GridState { Parameters = "DealId=" + dealCreditInitialBalanceCorrection.Deal.Id + ";IsDealPayment=false;TeamId=" + team.Id.ToString() }, user),
                DealDebitInitialBalanceCorrectionGridData = dealPaymentDocumentPresenterMediator.GetDealDebitInitialBalanceCorrectionSelectGridLocal(
                    new GridState { Parameters = "DealId=" + dealCreditInitialBalanceCorrection.Deal.Id + ";IsDealPayment=false;TeamId=" + team.Id.ToString() }, user)
            };

            return result;
        }

        #endregion

        #endregion
    }
}