using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.DealQuota;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IDealQuotaPresenter
    {
        DealQuotaListViewModel List(UserInfo currentUser);
        GridData GetActiveDealQuotaGrid(GridState state, UserInfo currentUser);
        GridData GetInactiveDealQuotaGrid(GridState state, UserInfo currentUser);

        DealQuotaEditViewModel Create(UserInfo currentUser);
        DealQuotaEditViewModel Edit(int id, UserInfo currentUser);
        object Save(DealQuotaEditViewModel model, UserInfo currentUser);
        void Delete(int id, UserInfo currentUser);

        DealQuotaSelectViewModel SelectDealQuota(int dealId, UserInfo currentUser, string mode);
        GridData GetDealQuotaSelectGrid(GridState state, UserInfo currentUser);
    }
}
