using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ReceiptWaybill
{
    public class ReceiptWaybillListViewModel
    {
        public FilterData FilterData { get; set; }

        public GridData DeliveryPendingGrid { get; set; }
        public GridData DivergenceWaybillGrid { get; set; }
        public GridData ApprovedWaybillGrid { get; set; }
        
        public ReceiptWaybillListViewModel()
        {
            FilterData = new FilterData();
        }
    }
}