using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    public class DealPaymentToClientDetailsViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        public string PaymentId { get; set; }

        /// <summary>
        /// Название сделки, по которой делается оплата клиенту
        /// </summary>
        [DisplayName("Сделка")]
        public string DealName { get; set; }
        public string DealId { get; set; }
        public bool AllowToViewDealDetails { get; set; }

        /// <summary>
        /// Команда
        /// </summary>
        [DisplayName("Команда")]
        public string TeamName { get; set; }
        public string TeamId { get; set; }
        public bool AllowToViewTeamDetails { get; set; }

        /// <summary>
        /// Пользователь, вернувший оплату
        /// </summary>
        [DisplayName("Пользователь, вернувший оплату")]
        public string ReturnedByName { get; set; }
        public string ReturnedById { get; set; }
        public bool AllowToViewReturnedByDetails { get; set; }
        public bool AllowToChangeReturnedBy { get; set; }

        [DisplayName("№ платежного документа")]
        public string PaymentDocumentNumber { get; set; }

        [DisplayName("Дата оплаты")]
        public string Date { get; set; }

        [DisplayName("Сумма")]
        public string Sum { get; set; }

        /// <summary>
        /// Форма оплаты
        /// </summary>
        [DisplayName("Форма оплаты")]
        public string DealPaymentForm { get; set; }

        public bool AllowToDelete { get; set; }
    }
}
