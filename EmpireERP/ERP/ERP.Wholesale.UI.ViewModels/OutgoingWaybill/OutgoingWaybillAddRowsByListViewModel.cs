using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.OutgoingWaybill
{
    public class OutgoingWaybillAddRowsByListViewModel
    {        
        public FilterData Filter { get; set; }

        public GridData ArticleGrid { get; set; }

        public GridData RowGrid { get; set; }

        public string Id { get; set; }
        
        public string Name { get; set; }

        public string BackURL { get; set; }

        public string StorageId { get; set; }
        public string AccountOrganizationId { get; set; }

        public OutgoingWaybillAddRowsByListViewModel()
        {
            Filter = new FilterData();
            ArticleGrid = new GridData();
            RowGrid = new GridData();
        }
    }
}
