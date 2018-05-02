using System.Collections.Generic;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common
{
    public class InvoicePrintingFormRowsViewModel
    {
        public IList<InvoicePrintingFormItemViewModel> Rows { get; set; }

        public InvoicePrintingFormRowsViewModel()
        {
            Rows = new List<InvoicePrintingFormItemViewModel>();
        }
    }
}
