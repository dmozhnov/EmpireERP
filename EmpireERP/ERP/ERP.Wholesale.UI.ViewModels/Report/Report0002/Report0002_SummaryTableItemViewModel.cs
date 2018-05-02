
namespace ERP.Wholesale.UI.ViewModels.Report.Report0002
{
    /// <summary>
    /// Строка сводной таблицы
    /// </summary>
    public class Report0002_SummaryTableItemViewModel
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Сумма реализации с учетом возвратов в закупочных ценах
        /// </summary>
        public string ResultPurchasePriceSum { get; set; }

        /// <summary>
        /// Сумма реализации с учетом возвратов в учетных ценах
        /// </summary>
        public string ResultAccountingPriceSum { get; set; }

        /// <summary>
        /// Сумма реализации с учетом возвратов в отпускных ценах
        /// </summary>
        public string ResultSalePriceSum { get; set; }

        /// <summary>
        /// Сумма возвратов в закупочных ценах
        /// </summary>
        public string ReturnsPurchaseCostSum { get; set; }

        /// <summary>
        /// Сумма возвратов в учетных ценах
        /// </summary>
        public string ReturnsAccountingPriceSum { get; set; }

        /// <summary>
        /// Сумма возвратов в отпускных ценах
        /// </summary>
        public string ReturnsSalePriceSum { get; set; }

        /// <summary>
        /// Сумма реализации в закупочных ценах
        /// </summary>
        public string ExpenditurePurchasePriceSum { get; set; }

        /// <summary>
        /// Сумма реализации в учетных ценах
        /// </summary>
        public string ExpenditureAccountingPriceSum { get; set; }

        /// <summary>
        /// Сумма реализации в отпускных ценах
        /// </summary>
        public string ExpenditureSalePriceSum { get; set; }

        /// <summary>
        /// Процент наценки
        /// </summary>
        public string MarkupPercentage { get; set; }

        /// <summary>
        /// Сумма наценки
        /// </summary>
        public string MarkupSum { get; set; }

        /// <summary>
        /// Количество проданного товара
        /// </summary>
        public string SoldArticleCount { get; set; }

    }
}
