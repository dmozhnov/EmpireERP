using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.Bank
{
    public class ForeignBankEditViewModel
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
        [StringLength(250, ErrorMessage = "Не более {1} символов")]
        [Required(ErrorMessage = "Укажите название банка")]
        public string Name { get; set; }

        /// <summary>
        /// Адрес банка
        /// </summary>
        [DisplayName("Адрес банка")]
        [StringLength(250, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Address { get; set; }

        /// <summary>
        /// SWIFT
        /// </summary>
        [DisplayName("SWIFT-код")]
        [StringLength(11, ErrorMessage = "Не более {1} символов")]
        [RegularExpression("((.){11})?((.){8})?", ErrorMessage = "SWIFT-код должен содержать 8 или 11 символов")]
        [Required(ErrorMessage = "Укажите SWIFT-код")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SWIFT { get; set; }

        /// <summary>
        /// Клиринговый код
        /// </summary>
        [DisplayName("Клиринговый код")]
        [RegularExpression("((.){9})?", ErrorMessage = "Указано неверное количество символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "Укажите клиринговый код")]
        public string ClearingCode { get; set; }
        
        [DisplayName("Тип клирингового кода")]
        public string ClearingCodeType { get; set; }

        public IEnumerable<SelectListItem> ClearingCodeTypeList { get; set; }
    }
}
