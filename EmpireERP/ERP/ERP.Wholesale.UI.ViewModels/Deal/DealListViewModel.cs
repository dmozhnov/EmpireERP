using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.Deal
{
    public class DealListViewModel
    {
        public GridData ClosedDealGrid { get; set; }
        public GridData ActiveDealGrid { get; set; }

        public FilterData Filter { get; set; }

        public DealListViewModel()
        {
            Filter = new FilterData();
        }
    }
}