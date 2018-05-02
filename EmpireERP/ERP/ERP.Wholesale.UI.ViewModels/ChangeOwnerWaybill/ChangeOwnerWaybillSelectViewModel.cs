using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybill
{
    public class ChangeOwnerWaybillSelectViewModel
    {
        public FilterData FilterData { get; set; }
        public GridData Data { get; set; }

        public ChangeOwnerWaybillSelectViewModel()
        {
            FilterData = new FilterData();
        }
    }
}