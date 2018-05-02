using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.Storage;
using ERP.Infrastructure.Security;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IStoragePresenter
    {
        StorageListViewModel List(UserInfo currentUser);
        GridData GetStorageGrid(GridState state, UserInfo currentUser);

        StorageEditViewModel Create(UserInfo currentUser);
        StorageEditViewModel Edit(short id, UserInfo currentUser);
        object Save(StorageEditViewModel model, UserInfo currentUser);
        void Delete(short id, UserInfo currentUser);

        StorageDetailsViewModel Details(short id, string backURL, UserInfo currentUser);
        GridData GetStorageSectionGrid(GridState state, UserInfo currentUser);
        GridData GetAccountOrganizationGrid(GridState state, UserInfo currentUser);
        
        StorageSectionEditViewModel CreateSection(short storageId, UserInfo currentUser);
        StorageSectionEditViewModel EditSection(short storageSectionId, short storageId, UserInfo currentUser);
        object SaveSection(StorageSectionEditViewModel model, UserInfo currentUser);
        object DeleteSection(short sectionId, short storageId, UserInfo currentUser);

        object AddAccountOrganization(AccountOrganizationSelectList model, UserInfo currentUser);
        object DeleteAccountOrganization(int accountOrganizationId, short storageId, UserInfo currentUser);

        AccountOrganizationSelectList GetAvailableAccountOrganizations(short storageId, UserInfo currentUser);
    }
}
