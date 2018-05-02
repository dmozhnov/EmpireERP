using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ReturnFromClientWaybill
{
    public class ReturnFromClientWaybillListViewModel
    {
        public FilterData FilterData { get; set; }

        public GridData NewAndAcceptedReturnFromClientWaybillGrid { get; set; }
        public GridData ReceiptedReturnFromClientWaybillGrid { get; set; }

        public ReturnFromClientWaybillListViewModel()
        {
            FilterData = new FilterData();
        }
    }
}