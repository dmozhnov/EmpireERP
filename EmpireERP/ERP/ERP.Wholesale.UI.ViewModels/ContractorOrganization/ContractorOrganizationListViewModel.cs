using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ContractorOrganization
{
    public class ContractorOrganizationListViewModel
    {
        public GridData ContractorOrganizationGrid { get; set; }
        public FilterData Filter { get; set; }

        public ContractorOrganizationListViewModel()
        {
            Filter = new FilterData();
        }
    }
}