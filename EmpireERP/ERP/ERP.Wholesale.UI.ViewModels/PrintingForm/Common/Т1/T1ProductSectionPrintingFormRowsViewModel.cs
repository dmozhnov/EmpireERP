using System.Collections.Generic;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common.Т1
{
    /// <summary>
    /// Модель позиций накладной для Т1 (ТТН)
    /// </summary>
    public class T1ProductSectionPrintingFormRowsViewModel
    {
        /// <summary>
        ///  Позиции
        /// </summary>
        public List<T1ProductSectionPrintingFormItemViewModel> Rows { get; set; }

        public T1ProductSectionPrintingFormRowsViewModel()
        {
            Rows = new List<T1ProductSectionPrintingFormItemViewModel>();
        }
    }
}
