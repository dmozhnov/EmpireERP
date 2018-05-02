using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    /// <summary>
    /// Модель модальной формы для ручного разнесения оплаты от клиента по организации клиента
    /// Все поля, которые приходят с предыдущей формы (редактирования), здесь не валидируются, ставится только ConvertEmptyStringToNull
    /// </summary>
    public class DestinationDocumentSelectForClientOrganizationPaymentFromClientDistributionViewModel : BaseDestinationDocumentSelectViewModel
    {
        /// <summary>
        /// Код организации клиента, к которой относится оплата
        /// </summary>
        public int ClientOrganizationId { get; set; }

        /// <summary>
        /// Код пользователя, принявшего оплату от клиента
        /// </summary>
        [DisplayName("Пользователь")]
        [Required(ErrorMessage = "Укажите пользователя, принявшего оплату")]
        public string TakenById { get; set; }
        public IEnumerable<SelectListItem> TakenByList { get; set; }
        public bool AllowToChangeTakenBy { get; set; }

        /// <summary>
        /// Название организации клиента, к которой относится оплата
        /// </summary>
        [DisplayName("Организация клиента")]
        public string ClientOrganizationName { get; set; }

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