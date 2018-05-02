using System.ComponentModel;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    public class DealPaymentFromClientDetailsViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Код оплаты
        /// </summary>
        public string PaymentId { get; set; }

        /// <summary>
        /// Номер документа
        /// </summary>
        [DisplayName("Номер документа")]
        public string PaymentDocumentNumber { get; set; }

        /// <summary>
        /// Дата оплаты
        /// </summary>
        [DisplayName("Дата оплаты")]
        public string Date { get; set; }

        /// <summary>
        /// Неразнесенный остаток по данной оплате
        /// </summary>
        [DisplayName("Неразнесенный остаток")]
        public string UndistributedSum { get; set; }

        /// <summary>
        /// Сделка
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
        /// Пользователь, принявший оплату
        /// </summary>
        [DisplayName("Пользователь")]
        public string TakenByName { get; set; }
        public string TakenById { get; set; }
        public bool AllowToViewTakenByDetails { get; set; }
        public bool AllowToChangeTakenBy { get; set; }

        /// <summary>
        /// Сумма оплаты
        /// </summary>
        [DisplayName("Сумма оплаты")]
        public string Sum { get; set; }

        /// <summary>
        /// Форма оплаты
        /// </summary>
        [DisplayName("Форма оплаты")]
        public string PaymentFormName { get; set; }

        /// <summary>
        /// Возвращено средств
        /// </summary>
        [DisplayName("Возвращено средств")]
        public string PaymentToClientSum { get; set; }

        /// <summary>
        /// Разнесено (накл.|корр.) 
        /// </summary>
        [DisplayName("Разнесено (накл. | корр.)")]
        public string DistributedToSaleWaybillSum { get; set; }
        public string DistributedToDealDebitInitialBalanceCorrectionSum { get; set; }

        /// <summary>
        /// Разрешено ли удаление оплаты из модальной формы
        /// </summary>
        public bool AllowToDelete { get; set; }

        /// <summary>
        /// Накладные реализации
        /// </summary>
        public GridData SaleWaybillGrid { get; set; }

        /// <summary>
        /// Накладные дебетов корректировок ПС
        /// </summary>
        public GridData DealDebitInitialBalanceCorrectionGrid { get; set; }
    }
}