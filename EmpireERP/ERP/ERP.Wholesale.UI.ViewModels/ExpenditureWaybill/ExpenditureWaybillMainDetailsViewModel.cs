using System.ComponentModel;
using ERP.Wholesale.UI.ViewModels.BaseWaybill;

namespace ERP.Wholesale.UI.ViewModels.ExpenditureWaybill
{
    public class ExpenditureWaybillMainDetailsViewModel : BaseWaybillMainDetailsViewModel
    {
        /// <summary>
        /// Форма взаиморасчетов
        /// </summary>
        [DisplayName("Форма взаиморасчетов")]
        public string PaymentType { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        [DisplayName("Клиент")]
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        public bool AllowToViewClientDetails { get; set; }

        /// <summary>
        /// Сделка
        /// </summary>
        [DisplayName("Сделка")]
        public string DealName { get; set; }
        public string DealId { get; set; }
        public bool AllowToViewDealDetails { get; set; }

        /// <summary>
        /// Квота
        /// </summary>
        [DisplayName("Квота")]
        public string DealQuotaName { get; set; }

        /// <summary>
        /// Место хранения
        /// </summary>
        [DisplayName("Место хранения")]
        public string SenderStorageName { get; set; }
        public string SenderStorageId { get; set; }
        public bool AllowToViewSenderStorageDetails { get; set; }

        /// <summary>
        /// Организация-собственник
        /// </summary>
        [DisplayName("Организация-собственник")]
        public string AccountOrganizationName { get; set; }
        public string AccountOrganizationId { get; set; }

        /// <summary>
        /// Команда
        /// </summary>
        [DisplayName("Команда")]
        public string TeamName { get; set; }
        public string TeamId { get; set; }
        public bool AllowToViewTeamDetails { get; set;}

        /// <summary>
        /// Пользователь, отгрузивший накладную
        /// </summary>
        [DisplayName("Отгрузка")]
        public string ShippedByName { get; set; }
        public string ShippedById { get; set; }
        public bool AllowToViewShippedByDetails { get; set; }
        public string ShippingDate { get; set; }

        /// <summary>
        /// Сумма в ЗЦ
        /// </summary>
        [DisplayName("Сумма в ЗЦ")]
        public string PurchaseCostSum { get; set; }

        /// <summary>
        /// Сумма в УЦ и ОЦ
        /// </summary>
        [DisplayName("Сумма в УЦ | ОЦ")]
        public string SenderAccountingAndSalePriceSum { get; set; }
        public string SenderAccountingPriceSum { get; set; }

        /// <summary>
        /// Сумма в отпускных ценах
        /// </summary>
        [DisplayName("Сумма в отпускных ценах")]
        public string SalePriceSum { get; set; }

        /// <summary>
        /// Сумма возвратов
        /// </summary>
        [DisplayName("Сумма возвратов (прин. | оформ.)")]
        public string TotalReturnedSum { get; set; }

        /// <summary>
        /// Общая сумма зарезервированного для возвратов
        /// </summary>
        [DisplayName("Общая сумма зарезервированного для возвратов")]
        public string TotalReservedByReturnSum { get; set; }

        /// <summary>
        /// Потери от возвратов (уже принятых)
        /// </summary>
        [DisplayName("Потери от возвратов (прин. | оформ.)")]
        public string ReturnLostProfitSum { get; set; }

        /// <summary>
        /// Потери от возвратов (всех)
        /// </summary>
        public string ReservedByReturnLostProfitSum { get; set; }
        
        /// <summary>
        /// Скидка по квоте
        /// </summary>
        [DisplayName("Итоговая скидка")]
        public string TotalDiscountPercent { get; set; }
        public string TotalDiscountSum { get; set; }

        /// <summary>
        /// Наценка от закупки
        /// </summary>
        [DisplayName("Наценка от закупки")]
        public string MarkupPercent { get; set; }
        public string MarkupSum { get; set; }

        /// <summary>
        /// Оплачено
        /// </summary>
        [DisplayName("Оплачено")]
        public string PaymentPercent { get; set; }
        public string PaymentSum { get; set; }

        /// <summary>
        /// Количество позиций
        /// </summary>
        [DisplayName("Кол-во позиций")]
        public string RowCount { get; set; }

        /// <summary>
        /// Сумма НДС
        /// </summary>
        [DisplayName("Сумма НДС")]
        public string ValueAddedTaxString { get; set; }

        /// <summary>
        /// Адрес доставки
        /// </summary>
        [DisplayName("Адрес доставки")]
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// Можно ли сменить квоту
        /// </summary>
        public bool AllowToChangeDealQuota { get; set; }
    }
}