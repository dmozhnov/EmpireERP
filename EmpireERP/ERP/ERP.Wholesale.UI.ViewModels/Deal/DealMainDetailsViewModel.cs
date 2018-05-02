using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Deal
{
    public class DealMainDetailsViewModel
    {
        /// <summary>
        /// Наименование
        /// </summary>
        [DisplayName("Наименование")]
        public string Name { get; set; }

        /// <summary>
        /// Куратор
        /// </summary>
        [DisplayName("Куратор")]
        public string CuratorName { get; set; }
        public string CuratorId { get; set; }
        public bool AllowToViewCuratorDetails { get; set; }

        /// <summary>
        /// Ожидаемый бюджет
        /// </summary>
        [DisplayName("Ожидаемый бюджет")]
        public string ExpectedBudget { get; set; }

        /// <summary>
        /// Дата начала сделки
        /// </summary>
        [DisplayName("Дата начала сделки")]
        public string StartDate { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        [DisplayName("Клиент")]
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        public bool AllowToViewClientDetails { get; set; }

        /// <summary>
        /// Этап
        /// </summary>
        [DisplayName("Этап")]
        public string StageName { get; set; }
        public string StageId { get; set; }

        /// <summary>
        /// Сумма реализации
        /// </summary>
        [DisplayName("Сумма реализации")]
        public string SaleSum { get; set; }

        /// <summary>
        /// Ожидается отгрузка
        /// </summary>
        [DisplayName("Ожидается отгрузка")]
        public string ShippingPendingSaleSum { get; set; }

        /// <summary>
        /// Сумма оплат
        /// </summary>
        [DisplayName("Сумма оплат")]
        public string PaymentSum { get; set; }

        /// <summary>
        /// Общее сальдо
        /// </summary>
        [DisplayName("Общее сальдо")]
        public string Balance { get; set; }

        /// <summary>
        /// Максимальная просрочка
        /// </summary>
        [DisplayName("Просрочка (срок | сумма)")]
        public string MaxPaymentDelayDuration { get; set; }

        /// <summary>
        /// Сумма просрочки
        /// </summary>
        public string PaymentDelaySum { get; set; }

        /// <summary>
        /// Дата начала этапа
        /// </summary>
        [DisplayName("Дата начала этапа")]
        public string StageStartDate { get; set; }
        public string StageDuration { get; set; }

        /// <summary>
        /// Договор по сделке
        /// </summary>
        [DisplayName("Договор по сделке")]
        public string ClientContractName { get; set; }
        public string ClientContractId { get; set; }

        [DisplayName("Организации")]
        public string ClientOrganizationName { get; set; }
        public string ClientOrganizationId { get; set; }
        public bool AllowToViewClientOrganizationDetails { get; set; }
        public string AccountOrganizationName { get; set; }
        public string AccountOrganizationId { get; set; }
        
        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        public string Comment { get; set; }
        
        /// <summary>
        /// Cумма возвратов
        /// </summary>
        [DisplayName("Сумма возвратов (прин. | оформ.)")]
        public string TotalReservedByReturnSum { get; set; }

        /// <summary>
        /// Общая сумма принятых возвратов
        /// </summary>
        public string TotalReturnedSum { get; set; }

        /// <summary>
        /// Сумма корректировок сальдо
        /// </summary>
        [DisplayName("Сумма корректировок сальдо")]
        public string InitialBalance { get; set; }

        public bool AllowToAddContract { get; set; }
        public bool AllowToChangeContract { get; set; }
        public bool AllowToChangeStage { get; set; }
    }
}