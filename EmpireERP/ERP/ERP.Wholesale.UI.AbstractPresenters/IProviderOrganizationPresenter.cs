using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;
using ERP.Wholesale.UI.ViewModels.Organization;
using ERP.Wholesale.UI.ViewModels.ProviderOrganization;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IProviderOrganizationPresenter
    {
        ProviderOrganizationDetailsViewModel Details(int id, string backURL, UserInfo currentUser);
        ProviderOrganizationMainDetailsViewModel GetMainDetails(int orgId, UserInfo currentUser);

        GridData GetReceiptWaybillGrid(GridState state, UserInfo currentUser);
        GridData GetProviderContractGrid(GridState state, UserInfo currentUser);
        GridData GetRussianBankAccountGrid(GridState state, UserInfo currentUser);
        GridData GetForeignBankAccountGrid(GridState state, UserInfo currentUser);

        RussianBankAccountEditViewModel AddRussianBankAccount(int providerOrganizationId, UserInfo currentUser);
        ForeignBankAccountEditViewModel AddForeignBankAccount(int providerOrganizationId, UserInfo currentUser);

        RussianBankAccountEditViewModel EditRussianBankAccount(int providerOrganizationId, int bankAccountId, UserInfo currentUser);
        ForeignBankAccountEditViewModel EditForeignBankAccount(int providerOrganizationId, int bankAccountId, UserInfo currentUser);

        void SaveRussianBankAccount(RussianBankAccountEditViewModel model, UserInfo currentUser);
        void SaveForeignBankAccount(ForeignBankAccountEditViewModel model, UserInfo currentUser);
        void RemoveRussianBankAccount(int providerOrganizationId, int bankAccountId, UserInfo currentUser);
        void RemoveForeignBankAccount(int providerOrganizationId, int bankAccountId, UserInfo currentUser);

        object Edit(int providerOrganizationId, UserInfo currentUser);
        void SaveJuridicalPerson(JuridicalPersonEditViewModel model, UserInfo currentUser);
        void SavePhysicalPerson(PhysicalPersonEditViewModel model, UserInfo currentUser);
        void Delete(int providerOrganizationId, UserInfo currentUser);
    }
}
