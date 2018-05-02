using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    public class DealPaymentToClientEditViewModel : BaseDealPaymentDocumentEditViewModel
    {
        [DisplayName("№ платежного документа")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PaymentDocumentNumber { get; set; }

        [DisplayName("Дата оплаты")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string Date { get; set; }

        [DisplayName("Сумма")]
        [StringLength(19, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Неверный формат суммы")]
        [GreaterByConst(0, ErrorMessage = "Сумма оплаты должна быть больше 0")]
        [Required(ErrorMessage = "Укажите сумму")]
        public string Sum { get; set; }

        /// <summary>
        /// Форма оплаты
        /// </summary>
        [DisplayName("Форма оплаты")]
        [Required(ErrorMessage = "Укажите форму оплаты")]
        public byte DealPaymentForm { get; set; }

        /// <summary>
        /// Код пользователя, вернувшего оплату клиенту
        /// </summary>
        [DisplayName("Пользователь, вернувший оплату")]
        [Required(ErrorMessage = "Укажите пользователя, вернувшего оплату")]
        public string ReturnedById { get; set; }
        public IEnumerable<SelectListItem> ReturnedByList { get; set; }

        /// <summary>
        /// Перечень возможных форм оплаты
        /// </summary>
        public IEnumerable<SelectListItem> DealPaymentFormList { get; set; }

        /// <summary>
        /// Контроллер, принимающий POST запрос 
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Метод контроллера, принимающий POST запрос 
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Разрешено ли изменять дату
        /// </summary>
        public bool AllowToChangeDate { get; set; }

        public DealPaymentToClientEditViewModel()
        {
            ReturnedByList = new List<SelectListItem>();
        }
    }

}
