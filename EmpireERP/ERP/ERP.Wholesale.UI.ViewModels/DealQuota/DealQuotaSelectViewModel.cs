using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.DealQuota
{
    public class DealQuotaSelectViewModel
    {
        public FilterData FilterData { get; set; }
        public GridData DealQuotasGrid { get; set; }
        
        public string Title { get; set; }

        public DealQuotaSelectViewModel()
        {
            FilterData = new FilterData();
        }
    }
}
