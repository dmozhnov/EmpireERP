using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrderMaterialsPackage
{
    public class ProductionOrderMaterialsPackageDetailsViewModel
    {
        public string Title { get; set; }

        public string BackURL { get; set; }

        public string PackageName { get; set; }

        public ProductionOrderMaterialsPackageMainDetailsViewModel MainDetails { get; set; }

        public GridData Grid { get; set; }

        public string Id { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToDelete { get; set; }
    }
}
