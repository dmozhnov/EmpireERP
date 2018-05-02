using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.Role
{
    public class RoleListViewModel
    {
        public FilterData FilterData { get; set; }
        
        public GridData RolesGrid { get; set; }

        public RoleListViewModel()
        {
            FilterData = new FilterData();
        }
    }
}
