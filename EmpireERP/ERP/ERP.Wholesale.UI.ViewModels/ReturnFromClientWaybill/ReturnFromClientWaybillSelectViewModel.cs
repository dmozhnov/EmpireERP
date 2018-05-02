using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ReturnFromClientWaybill
{
    public class ReturnFromClientWaybillSelectViewModel
    {
        public FilterData FilterData { get; set; }
        public GridData Data { get; set; }

        public ReturnFromClientWaybillSelectViewModel()
        {
            FilterData = new FilterData();
        }
    }
}