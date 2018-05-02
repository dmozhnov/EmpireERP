using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    public class DealPaymentListViewModel
    {
        public FilterData FilterData { get; set; }
        
        public GridData DealPaymentGrid { get; set; }

        public DealPaymentListViewModel()
        {
            FilterData = new FilterData();
        }
    }
}
