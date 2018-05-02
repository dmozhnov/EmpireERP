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
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;
using ERP.Wholesale.UI.ViewModels.ClientContract;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ClientContractPresenter : IClientContractPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IClientContractService clientContractService;
        private readonly IDealService dealService;
        private readonly IUserService userService;
        private readonly IClientService clientService;

        private readonly IClientContractPresenterMediator clientContractPresenterMediator;
       
        #endregion

        #region Конструктор

        public ClientContractPresenter(IUnitOfWorkFactory unitOfWorkFactory, IUserService userService, IClientContractService clientContractService, IDealService dealService,
            IClientService clientService,  IClientContractPresenterMediator clientContractPresenterMediator)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.userService = userService;
            this.clientContractService = clientContractService;
            this.dealService = dealService;
            this.clientService = clientService;

            this.clientContractPresenterMediator = clientContractPresenterMediator;
        }

        #endregion

        #region Методы

        #region Выбор контракта из грида

        /// <summary>
        /// Выбор договора с организацией клиента.
        /// </summary>
        /// <param name="dealId">Идентификатор сделки, для которой выбирается договор.</param>
        /// <param name="currentUser">Информация о текущем пользователе.</param>
        /// <returns>ViewModel для модальной формы выбора договора с организацией клиента.</returns>
        public ClientContractSelectViewModel Select(int dealId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = new ClientContractSelectViewModel();

                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);

                dealService.CheckPossibilityToSetContract(deal, user);

                model.FilterData.Items.Add(new FilterTextEditor("Number", "Номер"));
                model.FilterData.Items.Add(new FilterTextEditor("Name", "Название"));
                model.FilterData.Items.Add(new FilterDateRangePicker ("Date", "Дата"));
                model.FilterData.Items.Add(new FilterTextEditor("AccountOrganization_ShortName", "Собственная организация"));
                model.FilterData.Items.Add(new FilterTextEditor("ContractorOrganization_ShortName", "Организация клиента"));                

                model.Title = "Выбор договора с клиентом";
                model.ClientContractGrid = GetSelectGridLocal(
                    new GridState { Parameters = "ClientId=" + deal.Client.Id.ToString(), Sort = "CreationDate=Desc" }, user);

                model.AllowToCreateContract = dealService.IsPossibilityToCreateContractFromDeal(deal, user);

                return model;
            }
        }

        /// <summary>
        /// Грид списка договоров с клиентом для выбора.
        /// </summary>
        /// <param name="state">Состояние грида.</param>        
        public GridData GetSelectGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetSelectGridLocal(state, user);
            }
        }

        /// <summary>
        /// Формирование грида договоров с клиентом для модальной формы выбора.
        /// </summary>
        /// <param name="state">Состояние грида.</param>
        private GridData GetSelectGridLocal(GridState state, User user)
        {
            if (state == null) state = new GridState();

            var model = new GridData { State = state };
            model.AddColumn("Action", "Действие", Unit.Pixel(60));
            model.AddColumn("Name", "Название", Unit.Pixel(250));
            model.AddColumn("AccountOrganizationName", "Собственная организация", Unit.Percentage(50));
            model.AddColumn("ClientOrganizationName", "Организация клиента", Unit.Percentage(50));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("AccountOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ClientOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveFilter = new ParameterString(state.Filter);

            var dp = new ParameterString(state.Parameters);
            var clientId = Convert.ToInt32(dp["ClientId"].Value);
            var client = clientService.CheckClientExistence(clientId, user);
            var clientOrganizationIds = client.Organizations.Select(x => x.Id.ToString());

            deriveFilter.Add("Contract.ContractorOrganization", ParameterStringItem.OperationType.OneOf, clientOrganizationIds.Any() ? clientOrganizationIds : new List<string> {"0"});

            var contracts = clientContractService.GetFilteredList(state, deriveFilter, user, Permission.Deal_List_Details);

            var actions = new GridActionCell("Action");
            actions.AddAction("Выбрать", "linkClientContractSelect");

            foreach (var contract in contracts)
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Name") { Value = contract.FullName },
                    new GridLinkCell("AccountOrganizationName") { Value = contract.AccountOrganization.ShortName },
                    new GridLinkCell("ClientOrganizationName") { Value = contract.ContractorOrganization.ShortName },
                    new GridHiddenCell("Id") { Value = contract.Id.ToString() },
                    new GridHiddenCell("AccountOrganizationId") { Value = contract.AccountOrganization.Id.ToString() },
                    new GridHiddenCell("ClientOrganizationId") { Value = contract.ContractorOrganization.Id.ToString() }
                    ));
            }

            model.Title = "Договоры с клиентами";

            return model;
        }   

        #endregion

        #region Редактирование

        public ClientContractEditViewModel EditContract(short contractId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var contract = clientContractService.CheckClientContractExistence(contractId, user);

                clientContractService.CheckPossibilityToEdit(contract, user);

                var model = new ClientContractEditViewModel();
                model.Id = contract.Id.ToString();
                model.Name = contract.Name;
                model.Number = contract.Number;
                model.Date = contract.Date.ToShortDateString();

                model.AccountOrganizationId = contract.AccountOrganization.Id.ToString();
                model.AccountOrganizationName = contract.AccountOrganization.ShortName;
                model.ClientOrganizationId = contract.ContractorOrganization.Id.ToString();
                model.ClientOrganizationName = contract.ContractorOrganization.ShortName;

                model.Title = "Изменение договора по сделке";
                model.AllowToEditOrganization = clientContractService.IsPossibilityToEditOrganization(contract, user);
                model.AllowToEditClientOrganization = false; //так как пока договор можно редактировать только со страницы деталей организации клиента, то пусть пока будет false (там выбирать организацию клиента не надо, она и так уже известна)

                model.PostControllerName = "ClientContract";
                model.PostActionName = "EditContract";

                return model;
            }
        }

        #endregion

        #region Сохранение

        public object SaveContract(ClientContractEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var clientContract = clientContractPresenterMediator.SaveContract(model, currentUser);

                var user = userService.CheckUserExistence(currentUser.Id);

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

        #endregion

        public bool IsUsedBySingleDeal(short clientContractId, int dealId, UserInfo currentUser)
        {
            var user = userService.CheckUserExistence(currentUser.Id);

            var clientContract = clientContractService.CheckClientContractExistence(clientContractId, user);
            var deal = dealService.CheckDealExistence(dealId, user);

            return clientContractService.IsUsedBySingleDeal(clientContract, deal);
        }

        #endregion
    }
}