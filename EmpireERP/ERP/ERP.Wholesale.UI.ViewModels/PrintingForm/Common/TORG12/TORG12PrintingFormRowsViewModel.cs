using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12
{
    public class TORG12PrintingFormRowsViewModel
    {
        /// <summary>
        /// строки таблицы
        /// </summary>
        public IList<TORG12PrintingFormItemViewModel> Rows { get; set; }
        
        /// <summary>
        /// Общая сумма товаров прописью
        /// </summary>
        [DisplayName("Всего отпущено на сумму (прописью)")]
        public string TotalSalePriceString { get { return SpelledOutCurrency.Get(Rows.Sum(x => x.WithVatPriceSumValue)); } }

        /// <summary>
        /// Количество позиций
        /// </summary>
        public string RowsCountString { get { return SpelledOutNumber.Get(Rows.Count, false, "", "", ""); } }

        /// <summary>
        /// Погрузочных мест
        /// </summary>
        [DisplayName("Всего мест")]
        public string PlaceCount { get { return Rows.Sum(x => x.PackCountValue).ForDisplay(); } }

        /// <summary>
        /// Масса груза (нетто)
        /// </summary>
        [DisplayName("Масса груза (нетто)")]
        public string WeightNetto { get { return Rows.Sum(x => x.CountValue).ForDisplay(ValueDisplayType.Weight); } }

        /// <summary>
        /// Масса груза (брутто)
        /// </summary>
        [DisplayName("Масса груза (брутто)")]
        public string WeightBrutto { get { return Rows.Sum(x => x.WeightBruttoValue).ForDisplay(ValueDisplayType.Weight); } }

        /// <summary>
        /// Масса груза (брутто) прописью
        /// </summary>
        [DisplayName("Масса груза (брутто) (прописью)")]
        public string WeightBruttoString
        {
            get { return SpelledOutCurrency.Get(Rows.Sum(x => x.WeightBruttoValue), true, "килограмм", "килограмма", "килограмм", "", "", "", false); }
        }

        /// <summary>
        /// Общее количество товара
        /// </summary>
        public string TotalCount { get { return Rows.Sum(x => x.CountValue).ForDisplay(); } }

        /// <summary>
        /// Общая сумма НДС
        /// </summary>
        public string TotalValueAddedTax { get { return Rows.Sum(x => x.ValueAddedTaxValue).ForDisplay(ValueDisplayType.MoneyWithZeroCopecks); } }

        /// <summary>
        /// Общая сумма без НДС
        /// </summary>
        public string TotalSumWithoutValueAddedTax { get { return Rows.Sum(x => x.SumWithoutValueAddedTaxValue).ForDisplay(ValueDisplayType.MoneyWithZeroCopecks); } }

        public TORG12PrintingFormRowsViewModel()
        {
            Rows = new List<TORG12PrintingFormItemViewModel>();
        }
    }
}
