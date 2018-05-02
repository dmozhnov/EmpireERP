using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.User
{
    public class UserSelectViewModel
    {
        public FilterData FilterData { get; set; }
        public GridData UsersGrid { get; set; }
        public string Title { get; set; }

        public UserSelectViewModel()
        {
            FilterData = new FilterData();
        }
    }
}
