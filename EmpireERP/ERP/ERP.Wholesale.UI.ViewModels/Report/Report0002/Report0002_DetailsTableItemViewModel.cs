using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0002
{
    public class Report0002_DetailsTableItemViewModel
    {
        public string ArticleNumber { get; set; }
        public string ArticleId { get; set; }
        public string ArticleName { get; set; }
        public string BatchName { get; set; }

        public string PurchaseCostSum { get; set; }
        public string AvgPurchaseCostSum { get; set; }
        public string SalePriceSum { get; set; }
        public string AvgSalePriceSum { get; set; }
        public string AccountingPriceSum { get; set; }

        public string ReturnSalePriceSum { get; set; }
        public string ReturnAccountingPriceSum { get; set; }
        public string ReturnPurchaseCostSum { get; set; }
        public string ReturnedCount { get; set; }
        public string SoldCount { get; set; }

        /// <summary>
        /// Итоговая стоимость в отпускных ценах (с учетом возврата)
        /// </summary>
        public string ResultSalePriceSum { get; set; }

        public string MarkupSum { get; set; }
        public string MarkupPercent { get; set; }

        // Дополнительные столбцы
        public string PackSize { get; set; }
        public string ProductionCountryName { get; set; }
        public string CustomsDeclarationNumber { get; set; }

        //Данные для заголовочных строк
        public string HeaderSoldCount { get; set; }
        public string HeaderSaleSum { get; set; }
        public string HeaderPurchaseSum { get; set; }
        public string HeaderAccountingSum { get; set; }
        public string HeaderReturnPurchaseSum { get; set; }
        public string HeaderReturnSaleSum { get; set; }
        public string HeaderReturnAccountingSum { get; set; }
        public string HeaderSaleWithoutReturnSum { get; set; }
        public string HeaderMarkupSum { get; set; }
        public int HeaderOffset { get; set; }
        public string HeaderName { get; set; }
        public string HeaderValue { get; set; }
        public bool IsHeaderGroup { get; set; }
        public string HeaderReturnedCount { get; set; }
        /// <summary>
        /// Детализация реализации товара по МХ
        /// <remarks>Словарь: [Код МХ][Детализация]</remarks>
        /// </summary>
        public DynamicDictionary<short, Report0002_SeparationDetailsTableRowByStoragesViewModel> SeparationByStorages { get; set; }

        public Report0002_DetailsTableItemViewModel()
        {
            SeparationByStorages = new DynamicDictionary<short, Report0002_SeparationDetailsTableRowByStoragesViewModel>();
        }
    }
}
