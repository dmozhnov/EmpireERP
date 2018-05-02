using System.Collections.Generic;
using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0002
{
    public class Report0002_DetailsTableViewModel
    {
        /// <summary>
        /// Позиции таблицы
        /// </summary>
        public List<Report0002_DetailsTableItemViewModel> Items { get; set; }

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
        /// Признак разделения партий
        /// </summary>
        public bool IsDevideByBatch { get; set; }

        /// <summary>
        /// Признак расчета средних цен
        /// </summary>
        public bool CalculateAveragePrice { get; set; }

        /// <summary>
        /// Вывод дополнительных столбцов
        /// </summary>
        public bool ShowAdditionColumns { get; set; }
        
        /// <summary>
        /// Итоговая сумма реализаций с учетом возвратов в ОЦ
        /// </summary>
         public string ResultTotalSumInSalePriceSum { get; set; }

        /// <summary>
        /// Итоговая сумма возвратов в ЗЦ, УЦ, ОЦ
        /// </summary>
        public string ReturnsTotalSumInPurchasePriceSum { get; set; }
        public string ReturnsTotalSumInAccountPriceSum { get; set; }
        public string ReturnsTotalSumInSalePriceSum { get; set; }

        /// <summary>
        /// Итоговая сумма реализаций с учетом возвратов в ЗЦ, УЦ, ОЦ
        /// </summary>
        public string ExpenditureTotalSumInPurchasePriceSum { get; set; }
        public string ExpenditureTotalSumInAccountPriceSum { get; set; }
        public string ExpenditureTotalSumInSalePriceSum { get; set; }

        /// <summary>
        /// Итоговая наценка 
        /// </summary>
        public string TotalMarkup { get; set; }

        /// <summary>
        /// Итоговое количество проданного товара
        /// </summary>
        public string TotalSoldCount { get; set; }

        /// <summary>
        /// Итоговое количество возвращенного товара
        /// </summary>
        public string TotalReturnedCount { get; set; }

        /// <summary>
        /// Количество группировок
        /// </summary>
        public int GroupCount { get; set; }

        /// <summary>
        /// Список МХ отчета
        /// <remarks>Необходим для построения отчета с выводом МХ в столбцах</remarks>
        /// </summary>
        public List<Report0002_StorageInfoItem> Storages { get; set; }

        /// <summary>
        /// Итого для МХ в столбцах
        /// </summary>
        public DynamicDictionary<short, Report0002_SeparationDetailsTableRowByStoragesViewModel> SeparationByStorages { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Report0002_DetailsTableViewModel()
        {
            Items = new List<Report0002_DetailsTableItemViewModel>();
            Storages = new List<Report0002_StorageInfoItem>();
            SeparationByStorages = new DynamicDictionary<short, Report0002_SeparationDetailsTableRowByStoragesViewModel>();
        }
    }
}
