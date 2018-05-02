using System.ComponentModel;
using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common
{
    public class InvoicePrintingFormItemViewModel
    {
        [DisplayName("Наименование товара (описание выполненных работ, оказанных услуг), имущественного права")]
        public string ArticleName { get; set; }

        [DisplayName("Единица измерения")]
        public string MeasureUnitInfo { get; set; }

        [DisplayName("Условное обозначение (национальное)")]
        public string MeasureUnitName { get; set; }

        [DisplayName("Код")]
        public string MeasureUnitCode { get; set; }

        [DisplayName("Количество (объем)")]
        public string Count { get { return CountValue.ForDisplay(); } }
        public decimal CountValue { get; set; }
        
        [DisplayName("Цена (тариф) за единицу измерения")]
        public string Price { get; set; }

        [DisplayName("Стоимость товаров (работ, услуг), имущественных прав без налога - всего")]
        public string Cost { get { return CostValue.ForDisplay(ValueDisplayType.MoneyWithZeroCopecks); } }
        public decimal CostValue { get; set; }

        [DisplayName("В том числе сумма акциза")]
        public string ExciseValue { get; set; }

        [DisplayName("Налоговая ставка")]
        public string TaxValue { get; set; }

        [DisplayName("Сумма налога, предъявляемая покупателю")]
        public string TaxSum { get { return TaxSumValue.ForDisplay(ValueDisplayType.MoneyWithZeroCopecks); } }
        public decimal TaxSumValue { get; set; }

        [DisplayName("Стоимость товаров (работ, услуг), имущественных прав с налогом - всего")]
        public string TaxedCost { get { return TaxedCostValue.ForDisplay(ValueDisplayType.MoneyWithZeroCopecks); } }
        public decimal TaxedCostValue { get; set; }

        [DisplayName("Страна происхождения товара")]
        public string CountryInfo { get; set; }

        [DisplayName("Цифровой код")]
        public string CountryCode { get; set; }

        [DisplayName("Краткое наименование")]
        public string CountryName { get; set; }

        [DisplayName("Номер таможенной декларации")]
        public string CustomsDeclarationNumber { get; set; }
    }
}
