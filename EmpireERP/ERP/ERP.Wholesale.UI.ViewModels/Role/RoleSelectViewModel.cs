using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.Role
{
    public class RoleSelectViewModel
    {
        public FilterData FilterData { get; set; }
        public GridData RolesGrid { get; set; }
        public string Title { get; set; }

        public RoleSelectViewModel()
        {
            FilterData = new FilterData();
        }
    }
}
