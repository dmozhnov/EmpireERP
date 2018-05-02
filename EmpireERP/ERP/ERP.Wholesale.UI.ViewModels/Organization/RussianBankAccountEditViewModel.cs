using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.Organization
{
    public class RussianBankAccountEditViewModel
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
        [RegularExpression(@"[1-9]([0-9]{19})", ErrorMessage = "Введите 20 цифр")]
        [Required(ErrorMessage = "Укажите номер счета")]
        public string BankAccountNumber { get; set; }

        /// <summary>
        /// БИК
        /// </summary>
        [DisplayName("БИК")]
        [RegularExpression(@"[0-9]{9}", ErrorMessage = "Введите 9 цифр")]
        [Required(ErrorMessage = "Укажите БИК")]
        public string BIC { get; set; }

        /// <summary>
        /// Банк
        /// </summary>
        [DisplayName("Банк")]
        [StringLength(250, ErrorMessage = "Не более {1} символов")]
        [Required(ErrorMessage = "Укажите название банка")]
        public string BankName { get; set; }

        /// <summary>
        /// Кор. счет
        /// </summary>
        [DisplayName("Кор. счет")]
        [RegularExpression(@"[1-9]([0-9]{19})", ErrorMessage = "Введите 20 цифр")]
        [Required(ErrorMessage = "Укажите корреспондентский счет")]
        public string CorAccount { get; set; }

        /// <summary>
        /// Является основным
        /// </summary>
        [DisplayName("Является основным")]
        public string IsMaster { get; set; }
        
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