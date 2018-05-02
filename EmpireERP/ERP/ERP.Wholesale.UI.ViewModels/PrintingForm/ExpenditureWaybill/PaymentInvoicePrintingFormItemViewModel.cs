using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.ExpenditureWaybill
{
    public class PaymentInvoicePrintingFormItemViewModel
    {
        [DisplayName("№")]
        public string Number { get; set; }

        [DisplayName("Товары (работы, услуги)")]
        public string ArticleName { get; set; }

        [DisplayName("Количество")]
        public string Count { get; set; }

        [DisplayName("Единица")]
        public string MeasureUnitName { get; set; }

        [DisplayName("Цена")]
        public string Price { get; set; }

        [DisplayName("Сумма")]
        public string Cost { get; set; }
    }
}
