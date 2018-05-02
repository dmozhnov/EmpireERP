using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.ChangeOwnerWaybill
{
    public class ChangeOwnerWaybillPrintingFormItemViewModel
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public string ArticleName { get; set; }
        public string Count { get; set; }
        public string PackSize { get; set; }
        public string PackCount { get; set; }
        public string MeasureUnit { get; set; }
        public string Weight { get; set; }
        public string Volume { get; set; }
        public string PurchaseCost { get; set; }
        public string PurchaseSum { get; set; }
        public string Price { get; set; }
        public string PriceSum { get; set; }
        public string ContractorName { get; set; }
        public string BatchName { get; set; }
        public string Markup { get; set; }
    }
}
