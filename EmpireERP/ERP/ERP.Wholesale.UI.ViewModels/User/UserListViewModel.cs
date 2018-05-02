using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.User
{
    public class UserListViewModel
    {
        public FilterData FilterData { get; set; }
        
        public GridData ActiveUsersGrid { get; set; }
        public GridData BlockedUsersGrid { get; set; }

        public UserListViewModel()
        {
            FilterData = new FilterData();
        }
    }
}
