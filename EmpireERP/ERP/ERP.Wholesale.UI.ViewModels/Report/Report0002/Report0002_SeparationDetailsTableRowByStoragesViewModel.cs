
namespace ERP.Wholesale.UI.ViewModels.Report.Report0002
{
    /// <summary>
    /// Детализация реализации товара по МХ
    /// </summary>
    public class Report0002_SeparationDetailsTableRowByStoragesViewModel
    {
        /// <summary>
        /// Реализованное количество товара с МХ 
        /// </summary>
        public string SoldCount { get; set; }

        /// <summary>
        /// Сумма реализации в ЗЦ
        /// </summary>
        public string PurchaseCostSum { get; set; }

        /// <summary>
        /// Сумма реализации в ОЦ
        /// </summary>
        public string SalePriceSum { get; set; }

        /// <summary>
        /// Сумма реализованного товара в УЦ МХ
        /// </summary>
        public string StoredAccountingPriceSum { get; set; }

        /// <summary>
        /// Учетная цена реализованного товара на МХ
        /// </summary>
        public string StoredAccountingPrice { get; set; }

        /// <summary>
        /// Доступное количество товара на МХ
        /// </summary>
        public string AvailableArticleCount { get; set; }

        /// <summary>
        /// Возвращенное количество товара на МХ 
        /// </summary>
        public string ReturnedCount { get; set; }
    }
}
