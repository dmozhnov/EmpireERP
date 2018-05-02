using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.Client
{
    public class ClientSelectViewModel
    {
        public FilterData FilterData { get; set; }
        public GridData ClientsGrid { get; set; }
        public string Title { get; set; }

        public ClientSelectViewModel()
        {
            FilterData = new FilterData();
        }
    }
}
