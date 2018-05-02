using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.Team
{
    public class TeamListViewModel
    {
        public FilterData FilterData { get; set; }
        
        public GridData TeamsGrid { get; set; }

        public TeamListViewModel()
        {
            FilterData = new FilterData();
        }
    }
}
