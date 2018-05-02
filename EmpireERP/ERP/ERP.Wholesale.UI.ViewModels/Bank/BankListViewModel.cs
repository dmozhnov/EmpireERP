using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.Bank
{
    public class BankListViewModel
    {
        public FilterData Filter { get; set; }
        public GridData RussianBankGrid { get; set; }
        public GridData ForeignBankGrid { get; set; }

        public bool AllowToCreate { get; set; }
    }
}
