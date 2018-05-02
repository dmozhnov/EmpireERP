using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.Currency
{
    public class CurrencyListViewModel
    {
        public string Title { get; set; }

        public GridData CurrencyGrid { get; set; }
    }
}
