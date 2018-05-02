using System.Collections.Generic;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.WriteoffWaybill
{
    public class WriteoffWaybillPrintingFormItemViewModel
    {        
        public string Id { get; set; }
        public string Number { get; set; }
        public string ArticleName { get; set; }
        public string Count { get; set; }
        public string PurchaseCost { get; set; }
        public string PurchaseSum { get; set; }
        public string AccountingPrice { get; set; }
        public string AccountingPriceSum { get; set; }
        public string BatchName { get; set; }
        public string PackSize { get; set; }
        public string PackCount { get; set; }
        public string Weight{ get; set; }
        public string Volume { get; set; }
    }
}
