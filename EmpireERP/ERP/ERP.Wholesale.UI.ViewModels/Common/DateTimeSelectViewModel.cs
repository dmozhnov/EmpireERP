using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.Common
{
    public class DateTimeSelectViewModel
    {
        public string Title { get; set; }

        public string OkButtonTitle { get; set; }
        
        [DisplayName("Дата и время")]
        [Required(ErrorMessage = "Укажите дату")]
        [IsDate(ErrorMessage = "Укажите корректную дату")]
        public string Date { get; set; }

        [Required(ErrorMessage = "Укажите время")]
        [RegularExpression("([0-1][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]", ErrorMessage = "Укажите корректное время")]
        public string Time { get; set; }
    }
}
