using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.MovementWaybill
{
    public class MovementWaybillSelectViewModel
    {
        public FilterData FilterData { get; set; }
        public GridData Data { get; set; }

        public MovementWaybillSelectViewModel()
        {
            FilterData = new FilterData();
        }
    }
}