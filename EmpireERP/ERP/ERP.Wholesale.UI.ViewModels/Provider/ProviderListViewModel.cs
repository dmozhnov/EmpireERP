using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.Provider
{
    public class ProviderListViewModel
    {
        public GridData ProviderGrid { get; set; }
        public FilterData Filter { get; set; }

        public ProviderListViewModel()
        {
            Filter = new FilterData();
        }
    }
}