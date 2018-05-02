using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.Storage
{
    public class StorageDetailsViewModel
    {
        public GridData AccountOrganizationsGrid { get; set; }
        public GridData SectionsGrid { get; set; }
        public StorageMainDetailsViewModel MainDetails { get; set; }
        
        public string BackURL { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToDelete { get; set; }
        public bool AllowToCreateAccountingPriceList { get; set; }
    }
}