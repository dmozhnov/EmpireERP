using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.ClientContract
{
    public class ClientContractSelectViewModel
    {
        public FilterData FilterData { get; set; }
        public GridData ClientContractGrid { get; set; }
        public string Title { get; set; }

        public bool AllowToCreateContract { get; set; }

        public ClientContractSelectViewModel()
        {
            FilterData = new FilterData();
        }
    }
}
