using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.Team
{
    public class TeamSelectViewModel
    {
        public FilterData FilterData { get; set; }
        public GridData TeamsGrid { get; set; }
        public string Title { get; set; }

        public TeamSelectViewModel()
        {
            FilterData = new FilterData();
        }
    }
}
