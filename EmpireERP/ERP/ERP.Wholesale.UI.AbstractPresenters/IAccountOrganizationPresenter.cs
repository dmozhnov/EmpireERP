using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.AccountOrganization;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;
using ERP.Wholesale.UI.ViewModels.Organization;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IAccountOrganizationPresenter
    {
        AccountOrganizationListViewModel List(UserInfo currentUser);
        GridData GetAccountOrganizationGrid(GridState state, UserInfo currentUser);

        AccountOrganizationDetailsViewModel Details(int id, string backURL, UserInfo currentUser);
        AccountOrganizationMainDetailsViewModel MainDetails(int accountOrganizationId);

        GridData GetRussianBankAccountGrid(GridState state, UserInfo currentUser);
        GridData GetForeignBankAccountGrid(GridState state, UserInfo currentUser);
        GridData GetStorageGrid(GridState state, UserInfo currentUser);

        EconomicAgentTypeSelectorViewModel Create(UserInfo currentUser);                
        object Edit(int accountOrganizationId, UserInfo currentUser);
        int SaveJuridicalPerson(JuridicalPersonEditViewModel model, UserInfo currentUser);
        int SavePhysicalPerson(PhysicalPersonEditViewModel model, UserInfo currentUser);

        void Delete(int accountOrganizationId, UserInfo currentUser);

        LinkedStorageListViewModel GetStorageListForAddition(int orgId, UserInfo currentUser);
        void AddStorage(LinkedStorageListViewModel model, UserInfo currentUser);
        void RemoveStorage(int accountOrganizationId, short storageId, UserInfo currentUser);

        RussianBankAccountEditViewModel CreateRussianBankAccount(int accountOrganizationId, UserInfo currentUser);
        ForeignBankAccountEditViewModel CreateForeignBankAccount(int accountOrganizationId, UserInfo currentUser);
        RussianBankAccountEditViewModel EditRussianBankAccount(int accountOrganizationId, int bankAccountId, UserInfo currentUser);
        ForeignBankAccountEditViewModel EditForeignBankAccount(int accountOrganizationId, int bankAccountId, UserInfo currentUser);
        void SaveRussianBankAccount(RussianBankAccountEditViewModel model, UserInfo currentUser);
        void SaveForeignBankAccount(ForeignBankAccountEditViewModel model, UserInfo currentUser);
        void DeleteRussianBankAccount(int accountOrganizationId, int bankAccountId, UserInfo currentUser);
        void DeleteForeignBankAccount(int accountOrganizationId, int bankAccountId, UserInfo currentUser);        

        AccountOrganizationSelectViewModel SelectAccountOrganization();
        AccountOrganizationSelectViewModel SelectAccountOrganizationForStorage(short storageId);
        AccountOrganizationSelectGridViewModel GetAccountOrganizationSelectGrid(GridState state = null);
    }
}
