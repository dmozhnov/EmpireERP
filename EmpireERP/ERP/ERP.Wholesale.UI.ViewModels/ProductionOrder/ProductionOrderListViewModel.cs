using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderListViewModel
    {
        public FilterData FilterData { get; set; }

        public GridData ActiveProductionOrderGrid { get; set; }
        public GridData ClosedProductionOrderGrid { get; set; }

        public ProductionOrderListViewModel()
        {
            FilterData = new FilterData();
        }
    }
}