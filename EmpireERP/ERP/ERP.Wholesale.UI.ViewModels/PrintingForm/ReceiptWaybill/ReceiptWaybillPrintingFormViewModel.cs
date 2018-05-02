using System.Collections.Generic;
using System.ComponentModel;
using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.ReceiptWaybill
{
    public class ReceiptWaybillPrintingFormViewModel
    {
        [DisplayName("Организация")]
        public string OrganizationName { get; set; }

        [DisplayName("Отпечатано")]
        public string Date { get; set; }

        [DisplayName("Получатель")]
        public string ReceiptStorageName { get; set; }

        [DisplayName("Производитель")]
        public bool IsCreatedFromProductionOrderBatch { get; set; }

        [DisplayName("Поставщик")]
        public string ContractorName { get; set; }

        [DisplayName("Сопр. док")]
        public string AdditionDocs { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// Строки таблицы "Совпадающие позиции"
        /// </summary>
        public IList<ReceiptWaybillPrintingFormItemViewModel> MatchRows { get; set; }

        /// <summary>
        /// Строки таблицы "Позиции с расхождениями"
        /// </summary>
        public IList<ReceiptWaybillPrintingFormItemViewModel> DifRows { get; set; }

        /// <summary>
        /// Строки таблицы "Добавлено при приемке"
        /// </summary>
        public IList<ReceiptWaybillPrintingFormItemViewModel> AddedRows { get; set; }

        /// <summary>
        /// Параметры отображения формы
        /// </summary>
        public ReceiptWaybillPrintingFormSettingsViewModel Settings { get; set; }

        /// <summary>
        /// Общее количество по совпадающим позициям в единицах измерения
        /// </summary>
        public decimal TotalMatchRowsCount { get; set; }
        public string TotalMatchRowsCountString { get { return TotalMatchRowsCount.ForDisplay(); } }

        /// <summary>
        /// Общее ожидаемое количество по расходящимся позициям в единицах измерения
        /// </summary>
        public decimal TotalDifRowsPendingCount { get; set; }
        public string TotalDifRowsPendingCountString { get { return TotalDifRowsPendingCount.ForDisplay(); } }

        /// <summary>
        /// Общее принятое количество по расходящимся позициям в единицах измерения
        /// </summary>
        public decimal TotalDifRowsReceiptedCount { get; set; }
        public string TotalDifRowsReceiptedCountString { get { return TotalDifRowsReceiptedCount.ForDisplay(); } }

        /// <summary>
        /// Общее количество по добавленным позициям в единицах измерения
        /// </summary>
        public decimal TotalAddedRowsCount { get; set; }
        public string TotalAddedRowsCountString { get { return TotalAddedRowsCount.ForDisplay(); } }

        /// <summary>
        /// Общая сумма закупки
        /// </summary>
        public decimal TotalPurchaseSum { get; set; }
        public string TotalPurchaseSumString { get { return TotalPurchaseSum.ForDisplay(ValueDisplayType.Money); } }

        /// <summary>
        /// Общая учетная сумма
        /// </summary>
        public decimal TotalAccountingPriceSum { get; set; }
        public string TotalAccountingPriceSumString { get { return TotalAccountingPriceSum.ForDisplay(ValueDisplayType.Money); } }

        /// <summary>
        /// Общая сумма наценки
        /// </summary>
        public decimal TotalMarkupSum { get; set; }
        public string TotalMarkupSumString { get { return TotalMarkupSum.ForDisplay(ValueDisplayType.Money); } }

        /// <summary>
        /// НДС по закупочным ценам
        /// </summary>
        public string ValueAddedTaxSumString { get; set; }
        
        public ReceiptWaybillPrintingFormViewModel()
        {
            MatchRows = new List<ReceiptWaybillPrintingFormItemViewModel>();
            DifRows = new List<ReceiptWaybillPrintingFormItemViewModel>();
            AddedRows = new List<ReceiptWaybillPrintingFormItemViewModel>();
            TotalAddedRowsCount = TotalDifRowsPendingCount = TotalDifRowsReceiptedCount = TotalMatchRowsCount = 0;
            TotalPurchaseSum = 0;
            TotalAccountingPriceSum = 0;
            TotalMarkupSum = 0;
        }
    }
}
