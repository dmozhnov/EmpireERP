using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    public class DealInitialBalanceCorrectionListViewModel
    {
        public FilterData FilterData { get; set; }

        public GridData DealInitialBalanceCorrectionGrid { get; set; }

        public DealInitialBalanceCorrectionListViewModel()
        {
            FilterData = new FilterData();
        }
    }
}
