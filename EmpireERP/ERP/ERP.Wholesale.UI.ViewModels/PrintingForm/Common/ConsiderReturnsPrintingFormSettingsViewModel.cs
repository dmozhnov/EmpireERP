using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common
{
    public class ConsiderReturnsPrintingFormSettingsViewModel : BasePrintingFormSettingsViewModel
    {
        /// <summary>
        /// Учитывать возвраты при печати формы
        /// </summary>
        [DisplayName("Учитывать возвраты")]
        public bool? ConsiderReturns { get; set; }
    }
}
