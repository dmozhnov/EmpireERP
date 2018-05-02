using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrderMaterialsPackage
{
    public class ProductionOrderMaterialsPackageListViewModel
    {
        public string Title { get; set; }

        public GridData MaterialsPackageGrid { get; set; }

        public FilterData Filter { get; set; }
    }
}
