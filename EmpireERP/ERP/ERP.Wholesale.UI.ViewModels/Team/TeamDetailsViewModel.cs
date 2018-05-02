using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.Team
{
    public class TeamDetailsViewModel
    {
        public int Id { get; set; }
        public string BackURL { get; set; }
        public string Name { get; set; }
        
        public TeamMainDetailsViewModel MainDetails { get; set; }

        public GridData UsersGrid { get; set; }

        public GridData DealsGrid { get; set; }

        public GridData StoragesGrid { get; set; }

        public GridData ProductionOrdersGrid { get; set; }

        public bool AllowToDelete { get; set; }
        public bool AllowToEdit { get; set; }

        public bool AllowToViewUserList {get;set;}
        public bool AllowToViewDealList {get;set;}
        public bool AllowToViewStorageList {get;set;}
        public bool AllowToViewProductionOrderList { get; set; }
    }
}
