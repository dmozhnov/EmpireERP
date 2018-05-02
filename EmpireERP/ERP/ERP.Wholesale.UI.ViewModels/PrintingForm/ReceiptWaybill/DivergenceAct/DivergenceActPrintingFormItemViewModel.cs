using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.ReceiptWaybill.DivergenceAct
{
    public class DivergenceActPrintingFormItemViewModel
    {
        [DisplayName("№ п/п")]
        public string Number { get; set; }

        [DisplayName("Наименование товара")]
        public string ArticleName { get; set; }

        [DisplayName("Ожидаемое кол-во")]
        public string PendingCount { get; set; }

        [DisplayName("Кол-во по накладной")]
        public string ProviderCount { get; set; }

        [DisplayName("Кол-во принятое")]
        public string ReceiptedCount { get; set; }

        [DisplayName("Недостача (-)")]
        public string ShortageCount { get; set; }

        [DisplayName("Излишки (+)")]
        public string ExcessCount { get; set; }

        [DisplayName("Ожидаемая сумма")]
        public string PendingSum { get; set; }

        [DisplayName("Cумма по накладной")]
        public string ProviderSum { get; set; }
    }
}
