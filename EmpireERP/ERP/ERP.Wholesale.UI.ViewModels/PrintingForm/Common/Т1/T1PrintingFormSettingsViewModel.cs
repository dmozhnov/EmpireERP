using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common.Т1
{
    /// <summary>
    ///  Настройка печати формы Т1 (ТТН)
    /// </summary>
    public class T1PrintingFormSettingsViewModel : BasePrintingFormSettingsViewModel
    {
        [DisplayName("Печатать раздел")]
        public bool IsPrintProductSection { get; set; }

        /// <summary>
        /// Тип учетных цен
        /// </summary>
        [DisplayName("Печатать форму в")]
        public string PriceTypeId { get; set; }

        /// <summary>
        /// Типы учетных цен
        /// </summary>
        public IEnumerable<SelectListItem> PriceTypes { get; set; }

        /// <summary>
        /// Признак необходимости выбора типа учетных цен
        /// </summary>
        public bool IsNeedSelectPriceType { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public T1PrintingFormSettingsViewModel()
        {
            PriceTypes = new List<SelectListItem>();
        }
    }
}
