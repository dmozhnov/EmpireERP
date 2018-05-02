using System.ComponentModel;
using ERP.Wholesale.UI.ViewModels.BaseWaybill;

namespace ERP.Wholesale.UI.ViewModels.ReturnFromClientWaybill
{
    public class ReturnFromClientWaybillMainDetailsViewModel : BaseWaybillMainDetailsViewModel
    {
        /// <summary>
        /// Место хранения
        /// </summary>
        [DisplayName("Место хранения")]
        public string RecipientStorageName { get; set; }
        public string RecipientStorageId { get; set; }
        public bool AllowToViewRecipientStorageDetails { get; set; }

        // <summary>
        /// Организация
        /// </summary>
        [DisplayName("Организация")]
        public string RecipientName { get; set; }
        public string RecipientId { get; set; }

        /// <summary>
        /// Основание
        /// </summary>
        [DisplayName("Основание")]
        public string ReasonName { get; set; }

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
        /// Команда
        /// </summary>
        [DisplayName("Команда")]
        public string TeamName { get; set; }
        public string TeamId { get; set; }
        public bool AllowToViewTeamDetails { get; set; }
        

        /// <summary>
        /// Пользователь, осуществивший приемку
        /// </summary>
        [DisplayName("Приемка")]
        public string ReceiptedByName { get; set; }
        public string ReceiptedById { get; set; }
        public bool AllowToViewReceiptedByDetails { get; set; }
        public string ReceiptDate { get; set; }

        /// <summary>
        /// Сумма в ЗЦ
        /// </summary>
        [DisplayName("Сумма в ЗЦ")]
        public string PurchaseCostSum { get; set; }

        /// <summary>
        /// Сумма в ОЦ
        /// </summary>
        [DisplayName("Сумма в ОЦ")]
        public string SalePriceSum { get; set; }

        /// <summary>
        /// Сумма в УЦ приемки
        /// </summary>
        [DisplayName("Сумма в УЦ приемки")]
        public string AccountingPriceSum { get; set; }

        /// <summary>
        /// Количество позиций
        /// </summary>
        [DisplayName("Кол-во позиций и отгрузка")]
        public string RowCount { get; set; }

        [DisplayName("Отгрузка")]
        public string ShippingPercent { get; set; }
    }
}