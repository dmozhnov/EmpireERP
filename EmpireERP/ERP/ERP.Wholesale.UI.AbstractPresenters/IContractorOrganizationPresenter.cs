using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.ContractorOrganization;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IContractorOrganizationPresenter
    {
        ContractorOrganizationListViewModel List(UserInfo currentUser);
        GridData GetContractorOrganizationGrid(GridState state, UserInfo currentUser);
    }
}
