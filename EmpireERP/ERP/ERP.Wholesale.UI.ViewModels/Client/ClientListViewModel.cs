using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.Client
{
    public class ClientListViewModel
    {
        public GridData ClientGrid { get; set; }
        public FilterData Filter { get; set; }

        public ClientListViewModel()
        {
            Filter = new FilterData();
        }
    }
}