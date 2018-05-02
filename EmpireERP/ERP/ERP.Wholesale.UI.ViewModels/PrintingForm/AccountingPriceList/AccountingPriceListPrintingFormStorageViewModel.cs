using System.Collections.Generic;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.AccountingPriceList
{
    public class AccountingPriceListPrintingFormStorageViewModel
    {

        /// <summary>
        /// Набор товаров с ценами
        /// </summary>
        public IList<AccountingPriceListPrintingFormItemViewModel> AccountingPriceListItem { get; set; }

        /// <summary>
        /// Место хранения
        /// </summary>
        public string StorageName { get; set; }

        public AccountingPriceListPrintingFormStorageViewModel(string storageName)
        {
            StorageName = storageName;
            AccountingPriceListItem = new List<AccountingPriceListPrintingFormItemViewModel>();
        }
    }
}