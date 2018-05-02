using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    /// <summary>
    /// Модель модальной формы для ручного разнесения оплаты от клиента по сделке
    /// Все поля, которые приходят с предыдущей формы (редактирования), здесь не валидируются, ставится только ConvertEmptyStringToNull
    /// </summary>
    public class DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel : BaseDestinationDocumentSelectViewModel
    {
        /// <summary>
        /// Код сделки, к которой относится оплата (при разнесении на одну сделку)
        /// </summary>
        public int DealId { get; set; }

        /// <summary>
        /// Код пользователя, принявшего оплату от клиента
        /// </summary>
        [DisplayName("Пользователь")]
        [Required(ErrorMessage = "Укажите пользователя, принявшего оплату")]
        public string TakenById { get; set; }
        public IEnumerable<SelectListItem> TakenByList { get; set; }
        public bool AllowToChangeTakenBy { get; set; }

        /// <summary>
        /// Признак создания новой оплаты. Если false - переразносится уже созданная.
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Название сделки, к которой относится оплата (при разнесении на одну сделку)
        /// </summary>
        [DisplayName("Сделка")]
        public string DealName { get; set; }

        [DisplayName("№ платежного документа")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PaymentDocumentNumber { get; set; }

        [DisplayName("Дата оплаты")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Date { get; set; }

        /// <summary>
        /// Форма оплаты
        /// </summary>
        public byte DealPaymentForm { get; set; }

        /// <summary>
        /// Наименование формы оплаты
        /// </summary>
        [DisplayName("Форма оплаты")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PaymentFormName { get; set; }

        [DisplayName("Сумма")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SumString { get; set; }

        [DisplayName("Неразнесенная сумма оплаты")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string UndistributedSumString { get; set; }
    }
}