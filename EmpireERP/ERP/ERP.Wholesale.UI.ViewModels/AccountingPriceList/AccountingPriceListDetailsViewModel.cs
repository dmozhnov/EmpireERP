using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.AccountingPriceList
{
    public class AccountingPriceListDetailsViewModel
    {   
        public AccountingPriceListMainDetailsViewModel MainDetails { get; set; }

        public GridData ArticleGrid { get; set; }

        public GridData StorageGrid { get; set; }

        public string Message { get; set; }

        public string BackURL { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToAccept { get; set; }
        public bool AllowToCancelAcceptance { get; set; }
        public bool AllowToDelete { get; set; }

        /// <summary>
        /// Разрешено ли формировать печатные формы
        /// </summary>
        public bool AllowToPrintForms { get; set; }

        public AccountingPriceListDetailsViewModel()
        {            
            MainDetails = new AccountingPriceListMainDetailsViewModel();            
        }
    }
}