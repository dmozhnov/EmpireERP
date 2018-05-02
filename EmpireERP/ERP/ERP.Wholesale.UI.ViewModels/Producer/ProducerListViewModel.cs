using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.Producer
{
    public class ProducerListViewModel
    {
        public FilterData Filter { get; set; }

        public GridData ProducersGrid { get; set; }

        public ProducerListViewModel()
        {
            Filter = new FilterData();
        }
    }
}
