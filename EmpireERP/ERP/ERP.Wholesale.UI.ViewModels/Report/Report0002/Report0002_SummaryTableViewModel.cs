using System.Collections.Generic;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0002
{
    /// <summary>
    /// Сводная таблица
    /// </summary>
    public class Report0002_SummaryTableViewModel
    {
        /// <summary>
        /// Название первого столбца таблицы
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Строки таблицы
        /// </summary>
        public List<Report0002_SummaryTableItemViewModel> Items { get; set; }

        /// <summary>
        /// Итоговая сумма реализаций с учетом возвратов в ЗЦ, УЦ, ОЦ
        /// </summary>
        public string ResultTotalSumInPurchasePrice { get; set; }
        public string ResultTotalSumInAccountPrice{ get; set; }
        public string ResultTotalSumInSalePrice { get; set; }

        /// <summary>
        /// Итоговая сумма возвратов в ЗЦ, УЦ, ОЦ
        /// </summary>
        public string ReturnsTotalSumInPurchasePrice{ get; set; }
        public string ReturnsTotalSumInAccountPrice { get; set; }
        public string ReturnsTotalSumInSalePrice{ get; set; }

        /// <summary>
        /// Итоговая сумма реализаций с учетом возвратов в ЗЦ, УЦ, ОЦ
        /// </summary>
        public string ExpenditureTotalSumInPurchasePrice { get; set; }
        public string ExpenditureTotalSumInAccountPrice { get; set; }
        public string ExpenditureTotalSumInSalePrice { get; set; }

        /// <summary>
        /// Итоговая наценка 
        /// </summary>
        public string MarkupTotal { get; set; }        

        /// <summary>
        /// Показывать закупочные цены?
        /// </summary>
        public bool InPurchaseCost { get; set; }

        /// <summary>
        /// Показывать учетные цены?
        /// </summary>
        public bool InAccountingPrice { get; set; }

        /// <summary>
        /// Показывать отпускные цены?
        /// </summary>
        public bool InSalePrice { get; set; }

        /// <summary>
        /// Посчитать наценку
        /// </summary>
        public bool CalculateMarkup { get; set; }

        /// <summary>
        /// Использовать возвраты
        /// </summary>
        public bool UseReturns { get; set; }

        /// <summary>
        /// Показывать количество проданного товара?
        /// </summary>
        public bool ShowSoldArticleCount { get; set; }

        /// <summary>
        /// Количество проданного товара
        /// </summary>
        public string SoldArticleCount { get; set; }

        public Report0002_SummaryTableViewModel()
        {
            Items = new List<Report0002_SummaryTableItemViewModel>();
        }
    }
}
