using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Producer
{
    public class ProducerMainDetailsViewModel
    {
        /// <summary>
        /// Куратор
        /// </summary>
        [DisplayName("Куратор")]
        public string CuratorName { get; set; }

        /// <summary>
        /// Идентификатор куратора
        /// </summary>
        public string CuratorId { get; set; }

        /// <summary>
        /// Он же изготовитель
        /// </summary>
        [DisplayName("Он же изготовитель")]
        public string IsManufacturerName { get; set; }

        /// <summary>
        /// Организация
        /// </summary>
        [DisplayName("Организация")]
        public string OrganizationName { get; set; }

        /// <summary>
        /// Общая сумма заказов
        /// </summary>
        [DisplayName("Общая сумма заказов")]
        public string OrderSum { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        [DisplayName("Адрес")]
        public string Address { get; set; }

        /// <summary>
        /// Сумма открытых заказов
        /// </summary>
        [DisplayName("Сумма открытых заказов")]
        public string OpenOrderSum { get; set; }

        /// <summary>
        /// VAT No
        /// </summary>
        [DisplayName("VAT No")]
        public string VATNo { get; set; }

        /// <summary>
        /// Общая сумма производства
        /// </summary>
        [DisplayName("Общая сумма производства")]
        public string ProductionSum { get; set; }

        /// <summary>
        /// Руководитель
        /// </summary>
        [DisplayName("Руководитель")]
        public string DirectorName { get; set; }

        /// <summary>
        /// Общая сумма оплат
        /// </summary>
        [DisplayName("Общая сумма оплат")]
        public string PaymentSum { get; set; }

        /// <summary>
        /// Менеджер
        /// </summary>
        [DisplayName("Менеджер")]
        public string ManagerName { get; set; }
        
        /// <summary>
        /// Контакты
        /// </summary>
        [DisplayName("Контакты")]
        public string Contacts { get; set; }

        /// <summary>
        /// E-mail
        /// </summary>
        [DisplayName("E-mail")]
        public string Email { get; set; }

        /// <summary>
        /// Мобильный телефон
        /// </summary>
        [DisplayName("Моб. тел.")]
        public string MobilePhone { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        [DisplayName("Тел.")]
        public string Phone { get; set; }

        /// <summary>
        /// Факс
        /// </summary>
        [DisplayName("Факс")]
        public string Fax { get; set; }

        /// <summary>
        /// Skype
        /// </summary>
        [DisplayName("Skype")]
        public string Skype { get; set; }

        /// <summary>
        /// MSN
        /// </summary>
        [DisplayName("MSN")]
        public string MSN { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        public bool AllowToViewCuratorDetails { get; set; }
    }
}
