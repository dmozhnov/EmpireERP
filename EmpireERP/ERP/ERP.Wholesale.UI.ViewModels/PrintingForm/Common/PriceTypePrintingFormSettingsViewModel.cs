using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common
{
    public class PriceTypePrintingFormSettingsViewModel : BasePrintingFormSettingsViewModel
    {
        /// <summary>
        /// В каких ценах будет печататься отчет
        /// </summary>
        [DisplayName("Печатать форму в")]
        [GreaterByConst(0, ErrorMessage = "Укажите цены, в которых печатать отчет")]
        [Required(ErrorMessage = "Укажите цены, в которых печатать отчет")]
        public string PriceTypeId { get; set; }
        public IEnumerable<SelectListItem> PriceTypes { get; set; }
    }
}
