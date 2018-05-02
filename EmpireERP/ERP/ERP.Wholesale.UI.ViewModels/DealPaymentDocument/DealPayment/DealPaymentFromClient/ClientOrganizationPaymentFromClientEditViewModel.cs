using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    public class ClientOrganizationPaymentFromClientEditViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Код организации клиента, на которую разносится оплата
        /// </summary>
        [Required(ErrorMessage = "Укажите организацию клиента")]
        [GreaterByConst(0, ErrorMessage = "Укажите организацию клиента")]
        public string ClientOrganizationId { get; set; }

        /// <summary>
        /// Название организации клиента, на которую разносится оплата
        /// </summary>
        [DisplayName("Организация клиента")]
        public string ClientOrganizationName { get; set; }

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
        [StringLength(19, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Неверный формат суммы")]
        [Required(ErrorMessage = "Укажите сумму")]
        public string Sum { get; set; }

        /// <summary>
        /// Форма оплаты
        /// </summary>
        [DisplayName("Форма оплаты")]
        [Required(ErrorMessage = "Укажите форму оплаты")]
        public byte DealPaymentForm { get; set; }

        /// <summary>
        /// Перечень возможных форм оплаты
        /// </summary>
        public IEnumerable<SelectListItem> DealPaymentFormList { get; set; }

        /// <summary>
        /// Контроллер, принимающий POST запрос для следующей формы (выбора накладных для разнесения оплаты)
        /// </summary>
        public string DestinationDocumentSelectorControllerName { get; set; }

        /// <summary>
        /// Метод контроллера, принимающий POST запрос для следующей формы (выбора накладных для разнесения оплаты)
        /// </summary>
        public string DestinationDocumentSelectorActionName { get; set; }

        /// <summary>
        /// Разрешено ли изменять дату
        /// </summary>
        public bool AllowToChangeDate { get; set; }
    }
}