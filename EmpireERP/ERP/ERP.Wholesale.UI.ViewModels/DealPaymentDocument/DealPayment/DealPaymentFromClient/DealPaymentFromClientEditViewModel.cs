using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    public class DealPaymentFromClientEditViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Название клиента, к которому относится оплата
        /// </summary>
        [DisplayName("Клиент")]
        public string ClientName { get; set; }

        /// <summary>
        /// Код клиента, к которому относится оплата
        /// </summary>
        [Required(ErrorMessage = "Укажите клиента")]
        public string ClientId { get; set; }

        /// <summary>
        /// Название сделки, к которой относится оплата
        /// </summary>
        [DisplayName("Сделка")]
        public string DealName { get; set; }

        /// <summary>
        /// Код сделки, к которой относится оплата
        /// </summary>
        [Required(ErrorMessage = "Укажите сделку")]
        public string DealId { get; set; }

        /// <summary>
        /// № платежного документа
        /// </summary>
        [DisplayName("№ платежного документа")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PaymentDocumentNumber { get; set; }

        /// <summary>
        /// Дата оплаты
        /// </summary>
        [DisplayName("Дата оплаты")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string Date { get; set; }

        /// <summary>
        /// Сумма
        /// </summary>
        [DisplayName("Сумма")]
        [Required(ErrorMessage = "Укажите сумму оплаты")]
        [StringLength(19, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Неверный формат суммы")]
        [GreaterByConst(0, ErrorMessage = "Сумма оплаты должна быть больше 0")]
        public string Sum { get; set; }

        /// <summary>
        /// Форма оплаты
        /// </summary>
        [DisplayName("Форма оплаты")]
        [Required(ErrorMessage = "Укажите форму оплаты")]
        public string DealPaymentForm { get; set; }

        /// <summary>
        /// Перечень возможных форм оплаты
        /// </summary>
        public IEnumerable<SelectListItem> DealPaymentFormList { get; set; }

        /// <summary>
        /// Максимально возможная оплата наличными средствами
        /// </summary>
        public string MaxCashPaymentSum { get; set; }

        /// <summary>
        /// Контроллер, принимающий POST запрос для следующей формы (выбора документов для разнесения)
        /// </summary>
        public string DestinationDocumentSelectorControllerName { get; set; }

        /// <summary>
        /// Метод контроллера, принимающий POST запрос для следующей формы (выбора документов для разнесения)
        /// </summary>
        public string DestinationDocumentSelectorActionName { get; set; }

        /// <summary>
        /// Разрешено ли изменять дату
        /// </summary>
        public bool AllowToChangeDate { get; set; }
    }
}
