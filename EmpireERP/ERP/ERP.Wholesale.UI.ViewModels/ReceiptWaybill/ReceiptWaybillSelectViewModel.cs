using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ReceiptWaybill
{
    public class ReceiptWaybillSelectViewModel
    {
        public FilterData FilterData { get; set; }
        public GridData Data { get; set; }

        public ReceiptWaybillSelectViewModel()
        {
            FilterData = new FilterData();
        }
    }
}