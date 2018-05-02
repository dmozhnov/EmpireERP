using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.MovementWaybill
{
    public class MovementWaybillListViewModel
    {
        public FilterData FilterData { get; set; }
        
        public GridData ShippingPending { get; set; }
        public GridData Shipped { get; set; }
        public GridData Receipted { get; set; }

        public MovementWaybillListViewModel()
        {
            FilterData = new FilterData();
        }
    }
}