using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.ExpenditureWaybill
{
    public class ExpenditureWaybillListViewModel
    {
        public FilterData FilterData { get; set; }

        public GridData NewAndAcceptedGrid { get; set; }
        public GridData ShippedGrid { get; set; }

        public ExpenditureWaybillListViewModel()
        {
            FilterData = new FilterData();
        }
    }
}