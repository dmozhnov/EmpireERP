
namespace ERP.Wholesale.UI.ViewModels.PrintingForm.AccountingPriceList
{
    public class AccountingPriceListPrintingFormItemViewModel
    {
        /// <summary>
        /// Товар с новой назначенной ценой
        /// </summary>
        public AccountingPriceListPrintingFormArticleViewModel AccountingPriceListArticle { get; set; }
        /// <summary>
        /// Старая учетная цена
        /// </summary>
        public string OldAccountingPrice { get; set; }
        /// <summary>
        /// Изменение учетной цены
        /// </summary>
        public string DifferenceInAccountingPrice { get; set; }

        public AccountingPriceListPrintingFormItemViewModel(AccountingPriceListPrintingFormArticleViewModel accountingPriceListArticle)
        {
            AccountingPriceListArticle = accountingPriceListArticle;
        }
    }
}
