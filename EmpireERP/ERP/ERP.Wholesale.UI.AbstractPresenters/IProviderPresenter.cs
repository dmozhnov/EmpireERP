using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.ViewModels.ContractorOrganization;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;
using ERP.Wholesale.UI.ViewModels.Provider;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IProviderPresenter
    {
        ProviderListViewModel List(UserInfo currentUser);
        GridData GetProviderGrid(GridState state, UserInfo currentUser);

        ProviderDetailsViewModel Details(int id, string backURL, UserInfo currentUser);
        GridData GetReceiptWaybillGrid(GridState state, UserInfo currentUser);
        GridData GetProviderOrganizationGrid(GridState state, UserInfo currentUser);
        GridData GetProviderContractGrid(GridState state, UserInfo currentUser);
        GridData GetTaskGrid(GridState state, UserInfo currentUser);

        ProviderEditViewModel Create(string backURL, UserInfo currentUser);
        ProviderEditViewModel Edit(int id, string backURL, UserInfo currentUser);
        int Save(ProviderEditViewModel model, UserInfo currentUser);
        object GetProviderTypes();
        
        EconomicAgentTypeSelectorViewModel CreateContractorOrganization(int contractorId);
        object SaveJuridicalPerson(JuridicalPersonEditViewModel model, UserInfo currentUser);
        object SavePhysicalPerson(PhysicalPersonEditViewModel model, UserInfo currentUser);

        object AddContractorOrganization(int providerId, int organizationId, UserInfo currentUser);
        object RemoveProviderOrganization(int providerId, int providerOrganizationId, UserInfo currentUser);

        ProviderContractEditViewModel CreateContract(int providerId, UserInfo currentUser);
        ProviderContractEditViewModel EditContract(int providerId, short contractId, UserInfo currentUser);
        object SaveContract(ProviderContractEditViewModel model, UserInfo currentUser);
        object DeleteContract(int providerId, short contractId, UserInfo currentUser);

        ContractorOrganizationSelectViewModel SelectContractorOrganization(int providerId, string mode, UserInfo currentUser);
        GridData GetProviderOrganizationSelectGrid(GridState state = null);

        void Delete(int providerId, UserInfo currentUser);
    }
}
