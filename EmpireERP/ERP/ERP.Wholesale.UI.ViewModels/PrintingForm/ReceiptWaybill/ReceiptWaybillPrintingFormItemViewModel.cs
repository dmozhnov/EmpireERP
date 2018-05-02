using System.Collections.Generic;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.ReceiptWaybill
{
    public class ReceiptWaybillPrintingFormItemViewModel
    {        
        public string Id { get; set; }
        public string Number { get; set; }
        public string ArticleName { get; set; }
        public string Count { get; set; }
        public string PendingCount { get; set; }
        public string ReceiptedCount { get; set; }
        public string PackSize { get; set; }
        public string PackCount { get; set; }
        public string MeasureUnit { get; set; }
        public string Weight { get; set; }
        public string Volume { get; set; }
        public string PurchaseCost { get; set; }
        public string PurchaseSum { get; set; }
        public string AccountingPrice { get; set; }
        public string AccountingPriceSum { get; set; }
        public string MarkupCost { get; set; }
        public string MarkupSum { get; set; }

        /// <summary>
        /// Учетная сумма строки. Во внутреннем представлении округляется до 6 знаков, при выводе до 2
        /// </summary>
        public decimal AccountingPriceSumValue { get; set; }
        
        /// <summary>
        /// Сумма наценки строки. Во внутреннем представлении округляется до 6 знаков, при выводе до 2
        /// </summary>
        public decimal MarkupSumValue { get; set; }
    }
}
