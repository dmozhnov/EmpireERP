using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.DealQuota
{
    public class DealQuotaListViewModel
    {
        public GridData ActiveDealQuotaGrid { get; set; }
        public GridData InactiveDealQuotaGrid { get; set; }

        public FilterData Filter { get; set; }

        public DealQuotaListViewModel()
        {
            Filter = new FilterData();
        }
    }
}