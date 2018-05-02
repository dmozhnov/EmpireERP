using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.Deal
{
    public class DealSelectViewModel
    {
        public FilterData FilterData { get; set; }
        public GridData DealsGrid { get; set; }
        
        public string Title { get; set; }

        public DealSelectViewModel()
        {
            FilterData = new FilterData();
        }
    }
}
