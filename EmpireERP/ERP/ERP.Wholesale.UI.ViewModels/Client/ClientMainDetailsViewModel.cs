using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Client
{
    public class ClientMainDetailsViewModel
    {
        /// <summary>
        /// Наименование
        /// </summary>
        [DisplayName("Наименование")]
        public string Name { get; set; }

        /// <summary>
        /// Фактический адрес
        /// </summary>
        [DisplayName("Фактический адрес")]
        public string FactualAddress { get; set; }

        /// <summary>
        /// Контактный телефон
        /// </summary>
        [DisplayName("Контактный телефон")]
        public string ContactPhone { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        [DisplayName("Тип")]
        public string TypeName { get; set; }

        /// <summary>
        /// Лояльность
        /// </summary>
        [DisplayName("Лояльность")]
        public string LoyaltyName { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        [DisplayName("Регион")]
        public string RegionName { get; set; }

        /// <summary>
        /// Программа обслуживания
        /// </summary>
        [DisplayName("Программа обслуживания")]
        public string ServiceProgramName { get; set; }

        /// <summary>
        /// Рейтинг
        /// </summary>
        [DisplayName("Рейтинг")]
        public string Rating { get; set; }

        /// <summary>
        /// Сумма продаж
        /// </summary>
        [DisplayName("Сумма продаж")]
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
        /// Сальдо по сделкам
        /// </summary>
        [DisplayName("Сальдо по сделкам")]
        public string Balance { get; set; }

        /// <summary>
        /// Просрочка, сумма
        /// </summary>
        public string PaymentDelaySum { get; set; }

        /// <summary>
        /// Просрочка, срок
        /// </summary>
        [DisplayName("Просрочка (срок | сумма)")]
        public string PaymentDelayPeriod { get; set; }

        /// <summary>
        /// Заблокирован ли аккаунт?
        /// </summary>
        [DisplayName("Заблокирован?")]
        public string IsBlockedManually { get; set; }

        /// <summary>
        /// Разрешение на блокировку клиента
        /// </summary>
        public bool AllowToBlock { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        /// <summary>
        /// Общая сумма возвратов
        /// </summary>
        public string TotalReturnedSum { get; set; }

        /// <summary>
        /// Cумма возвратов
        /// </summary>
        [DisplayName("Сумма возвратов (прин. | оформ.)")]
        public string TotalReservedByReturnSum { get; set; }

        /// <summary>
        /// Сумма корректировок сальдо
        /// </summary>
        [DisplayName("Сумма корректировок сальдо")]
        public string InitialBalance { get; set; }
    }
}