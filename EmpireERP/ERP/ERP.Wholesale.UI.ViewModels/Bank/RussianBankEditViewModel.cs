using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.Bank
{
    public class RussianBankEditViewModel
    {
        /// <summary>
        /// Идентификатор банка
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Флаг разрешения редактирования
        /// </summary>
        public bool AllowToEdit { get; set; }

        /// <summary>
        /// Название банка
        /// </summary>
        [DisplayName("Название")]
        [StringLength(250, ErrorMessage="Не более {1} символов")]
        [Required(ErrorMessage="Укажите название банка")]
        public string Name { get; set; }

        /// <summary>
        /// Адрес банка
        /// </summary>
        [DisplayName("Адрес банка")]
        [StringLength(250, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Address { get; set; }

        /// <summary>
        /// БИК
        /// </summary>
        [DisplayName("БИК")]        
        [RegularExpression("[0-9]{9}", ErrorMessage = "Введите 9 цифр")]
        [Required(ErrorMessage = "Укажите БИК")]
        public string BIC { get; set; }

        /// <summary>
        /// Корреспондентский счет
        /// </summary>
        [DisplayName("Кор./счет")]
        [StringLength(20, ErrorMessage = "Не более {1} символов")]
        [RegularExpression("[0-9]{20}", ErrorMessage = "Введите 20 цифр")]
        [Required(ErrorMessage = "Укажите корреспондентский счет")]
        public string CorAccount { get; set; }
    }
}
