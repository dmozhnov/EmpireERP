using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.AccountingPriceList
{
    public class AccountingPriceListPrintingFormArticleViewModel
    {
        public string Id { get; set; }
        /// <summary>
        /// Артикул
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string ArticleName { get; set; }
        /// <summary>
        /// Назначенная учетная цена
        /// </summary>
        public string AccountingPrice { get; set; }
        /// <summary>
        /// Назначенная учетная цена
        /// </summary>
        public decimal AccountingPriceValue { get; set; }

        public int ArticleId { get; set; }
    }
}
