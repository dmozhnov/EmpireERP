using System;
using System.Linq;
using ERP.Infrastructure.Security;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;
using ERP.Wholesale.UI.ViewModels.ClientContract;

namespace ERP.Wholesale.UI.LocalPresenters.Mediators
{
    public class ClientContractPresenterMediator : IClientContractPresenterMediator
    {
        private readonly IUserService userService;
        private readonly IDealService dealService;
        private readonly IClientService clientService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly IClientOrganizationService clientOrganizationService;
        private readonly IClientContractService clientContractService;

        public ClientContractPresenterMediator(IUserService userService, IDealService dealService, IClientService clientService,
            IAccountOrganizationService accountOrganizationService, IClientOrganizationService clientOrganizationService, IClientContractService clientContractService)
        {
            this.userService = userService;
            this.dealService = dealService;
            this.clientService = clientService;
            this.accountOrganizationService = accountOrganizationService;
            this.clientOrganizationService = clientOrganizationService;
            this.clientContractService = clientContractService;
        }

        public ClientContract SaveContract(ClientContractEditViewModel model, UserInfo currentUser)
        {
            var user = userService.CheckUserExistence(currentUser.Id);

            ClientContract clientContract;

            if (String.IsNullOrEmpty(model.Id) || model.Id == "0")
            {
                var deal = dealService.CheckDealExistence(ValidationUtils.TryGetInt(model.DealId), user);

                dealService.CheckPossibilityToCreateContractFromDeal(deal, user);

                var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(ValidationUtils.TryGetInt(model.AccountOrganizationId));
                var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(ValidationUtils.TryGetInt(model.ClientOrganizationId), user);

                clientContract = new ClientContract(accountOrganization, clientOrganization,
                    model.Name, model.Number, DateTime.Parse(model.Date), DateTime.Parse(model.Date));

                clientContractService.Save(clientContract, user);
            }
            else
            {
                clientContract = clientContractService.CheckClientContractExistence(ValidationUtils.TryGetShort(model.Id), user);

                clientContractService.CheckPossibilityToEdit(clientContract, user);

                clientContract.Name = model.Name;
                clientContract.Number = model.Number;
                clientContract.Date = ValidationUtils.TryGetDate(model.Date);
                
                var accountOrganizationId = ValidationUtils.TryGetInt(model.AccountOrganizationId);
                var clientOrganizationId = ValidationUtils.TryGetInt(model.ClientOrganizationId);

                if (accountOrganizationId != clientContract.AccountOrganization.Id || clientOrganizationId != clientContract.ContractorOrganization.Id)
                {
                    clientContractService.CheckPossibilityToEditOrganization(clientContract, user);

                    var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(accountOrganizationId);
                    var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(clientOrganizationId, user);

                    clientContract.AccountOrganization = accountOrganization;
                    clientContract.ContractorOrganization = clientOrganization;

                }
            }

            return clientContract;            
        }
    }
}
