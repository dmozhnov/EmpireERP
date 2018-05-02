using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.WriteoffWaybill
{
    public class WriteoffWaybillListViewModel
    {
        public FilterData FilterData { get; set; }

        public GridData WriteoffPendingGrid { get; set; }
        public GridData WrittenoffGrid { get; set; }

        public WriteoffWaybillListViewModel()
        {
            FilterData = new FilterData();
        }
    }
}