using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.Currency
{
    public class CurrencyEditViewModel
    {
        /// <summary>
        /// Идентификатор валюты
        /// </summary>
        public string CurrencyId { get; set; }

        /// <summary>
        /// Является ли валюта новой
        /// </summary>
        public bool IsNew
        {
            get
            {
                return String.IsNullOrEmpty(CurrencyId) || CurrencyId == "0";
            }
        }

        /// <summary>
        /// Название валюты
        /// </summary>
        [DisplayName("Название")]
        [StringLength(20, ErrorMessage = "Не более {1} символов")]
        [Required(ErrorMessage = "Укажите название валюты")]
        public string Name { get; set; }

        /// <summary>
        /// Сокращение названия валюты
        /// </summary>
        [DisplayName("Сокращенное название")]
        [StringLength(3, ErrorMessage = "Не более {1} символов")]
        [Required(ErrorMessage = "Укажите сокращенное название валюты")]
        [RegularExpression("[A-Z]{1,3}", ErrorMessage = "Допустимы только прописные латинские буквы")]
        public string LiteralCode { get; set; }

        /// <summary>
        /// Цифровой код валюты
        /// </summary>
        [DisplayName("Код")]
        [StringLength(3, ErrorMessage = "Не более {1} символов")]
        [Required(ErrorMessage = "Укажите код валюты")]
        [RegularExpression("[0-9]{1,3}", ErrorMessage = "Код валюты может содержать только цифры")]
        public string NumericCode { get; set; }

        /// <summary>
        /// Курсы валюты
        /// </summary>
        public GridData CurrencyRateGrid { get; set; }

        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        public bool AllowToEdit { get; set; }
    }
}
