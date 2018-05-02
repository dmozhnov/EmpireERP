using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.Organization
{
    public class ForeignBankAccountEditViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        [DisplayName("Валюта")]
        [Required(ErrorMessage = "Укажите валюту")]
        public short CurrencyId { get; set; }
        public IEnumerable<SelectListItem> CurrencyList { get; set; }

        /// <summary>
        /// Номер счета
        /// </summary>
        [DisplayName("Номер счета")]
        [Required(ErrorMessage = "Укажите номер счета")]
        public string BankAccountNumber { get; set; }

        /// <summary>
        /// Клиринговый код
        /// </summary>
        [DisplayName("Клиринговый код")]
        public string ClearingCode { get; set; }

        /// <summary>
        /// Тип клирингового кода
        /// </summary>
        public string ClearingCodeType { get; set; }

        /// <summary>
        /// Банк
        /// </summary>
        [DisplayName("Название банка")]
        public string BankName { get; set; }

        /// <summary>
        /// Адрес банка
        /// </summary>
        [DisplayName("Адрес банка")]
        public string BankAddress { get; set; }

        /// <summary>
        /// Является основным
        /// </summary>
        [DisplayName("Является основным")]
        public string IsMaster { get; set; }

        /// <summary>
        /// Номер счета европейского стандарта
        /// </summary>
        [DisplayName("IBAN")]
        [StringLength(34, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull=false)]
        public string IBAN { get; set; }

        /// <summary>
        /// SWIFT-код
        /// </summary>
        [DisplayName("SWIFT-код")]
        [StringLength(34, ErrorMessage = "Не более {1} символов")]
        [Required(ErrorMessage = "Укажите SWIFT-код")]
        [RegularExpression("[^^]{8}([^^]{3})?", ErrorMessage = "SWIFT-код должен содержать 8 или 11 символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SWIFT { get; set; }


        #region Вызываемые в представлении методы

        /// <summary>
        /// Метод контроллера, вызываемый по методу POST
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Имя контроллера, вызываемого по методу POST
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Имя функции Javascript, вызываемой в случае успешного срабатывания метода POST
        /// </summary>
        public string SuccessFunctionName { get; set; }

        #endregion

        /// <summary>
        /// Идентификатор организации, которой принадлежит счет
        /// </summary>
        public int OrganizationId { get; set; }

        /// <summary>
        /// Идентификатор расчетного счета
        /// </summary>
        public int BankAccountId { get; set; }

        public bool AllowToEdit { get; set; }
    }
}