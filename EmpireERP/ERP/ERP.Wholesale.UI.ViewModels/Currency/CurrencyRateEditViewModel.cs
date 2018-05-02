using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.Currency
{
    public class CurrencyRateEditViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Курс
        /// </summary>
        [DisplayName("Курс")]
        [Required(ErrorMessage="Укажите курс")]
        [RegularExpression("[0-9]{1,12}([.,][0-9]{1,6})?", ErrorMessage="Укажите число, содержащее не более 6 знаков после запятой")]
        public string Rate { get; set; }

        /// <summary>
        /// Дата начала действия курса
        /// </summary>
        [DisplayName("Дата начала действия")]
        [Required(ErrorMessage="Укажите дату начала действия курса")]
        [IsDate(ErrorMessage="Укажите корректную дату")]
        public string StartDate { get; set; }

        /// <summary>
        /// Идентификатор валюты, для которой задается курс
        /// </summary>
        public string CurrencyId { get; set; }

        /// <summary>
        /// Код курса валюты
        /// </summary>
        public string CurrencyRateId { get; set; }

        /// <summary>
        /// Разрешение редактировать курс валюты
        /// </summary>
        public bool AllowToEditCurrencyRate { get; set; }
    }
}
