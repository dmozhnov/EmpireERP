using System.ComponentModel;
using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12
{
    public class TORG12PrintingFormItemViewModel
    {
        /// <summary>
        /// Номер строки
        /// </summary>
        public string RowNumber { get; set; }

        /// <summary>
        /// Наименование товара
        /// </summary>
        [DisplayName("наименование, характеристика, сорт, артикул товара")]
        public string ArticleName { get; set; }

        /// <summary>
        /// Код товара
        /// </summary>
        [DisplayName("код")]
        public string Id { get; set; }

        /// <summary>
        /// Количество
        /// </summary>
        public string Count { get { return CountValue.ForDisplay(); } }
        public decimal CountValue { get; set; }

        /// <summary>
        /// Единица измерения - наименование
        /// </summary>
        [DisplayName("Единица измерения - наименование")]
        public string MeasureUnit { get; set; }
        public byte MeasureUnitScale { get; set; }

        /// <summary>
        /// Единица измерения - наименование по ОКЕИ
        /// </summary>
        [DisplayName("Единица измерения - наименование по ОКЕИ")]
        public string MeasureUnitOKEI { get; set; }

        /// <summary>
        /// Вид упаковки
        /// </summary>
        [DisplayName("Вид упаковки")]
        public string PackType { get; set; }

        /// <summary>
        /// Количество в упаковке
        /// </summary>
        [DisplayName("Количество в упаковке")]
        public string PackVolume { get; set; }

        /// <summary>
        /// Количество упаковок
        /// </summary>
        [DisplayName("Количество упаковок")]
        public string PackCount { get { return PackCountValue.ForDisplay(); } }
        public decimal PackCountValue { get; set; }

        public string WithoutVatPrice { get; private set; }

        public string WithVatPriceSum { get { return WithVatPriceSumValue.ForDisplay(ValueDisplayType.MoneyWithZeroCopecks); } }
        public decimal WithVatPriceSumValue { get; set; }

        public string WeightBrutto { get; set; }
        public decimal WeightBruttoValue { get; set; }

        public string ValueAddedTax { get { return ValueAddedTaxValue.ForDisplay(ValueDisplayType.MoneyWithZeroCopecks); } }
        public decimal ValueAddedTaxValue { get; set; }

        public string ValueAddedTaxRate
        {
            get
            {
                if (ValueAddedTaxRateValue > 0)
                {
                    return ValueAddedTaxRateValue.ForDisplay(ValueDisplayType.Percent) + "%";
                }
                else
                    return "Без НДС";
            }
        }
        public decimal ValueAddedTaxRateValue { get; set; }

        public string SumWithoutValueAddedTax { get { return SumWithoutValueAddedTaxValue.ForDisplay(ValueDisplayType.MoneyWithZeroCopecks); } }
        public decimal SumWithoutValueAddedTaxValue { get; set; }

        public TORG12PrintingFormItemViewModel(decimal price, decimal priceSum, decimal count, decimal valueAddedTaxSum, decimal valueAddedTaxRate)
        {
            WithoutVatPrice = VatUtils.CalculateWithoutVatSum(price, valueAddedTaxRate).ForDisplay(ValueDisplayType.MoneyWithZeroCopecks);                
            CountValue = 0;
            PackCountValue = count;
            WithVatPriceSumValue = priceSum;
            ValueAddedTaxValue = valueAddedTaxSum;
            ValueAddedTaxRateValue = valueAddedTaxRate;
            SumWithoutValueAddedTaxValue = priceSum - ValueAddedTaxValue;
        }
    }
}