using System.ComponentModel;
using ERP.Wholesale.UI.ViewModels.BaseWaybill;

namespace ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybill
{
    public class ChangeOwnerWaybillMainDetailsViewModel : BaseWaybillMainDetailsViewModel
    {
        /// <summary>
        /// Отправитель
        /// </summary>
        [DisplayName("Отправитель")]
        public string SenderName { get; set; }

        /// <summary>
        /// Идентификатор отправителя
        /// </summary>
        public string SenderId { get; set; }

        /// <summary>
        /// Получатель
        /// </summary>
        [DisplayName("Получатель")]
        public string RecipientName { get; set; }

        /// <summary>
        /// Идентификатор получателя
        /// </summary>
        public string RecipientId { get; set; }

        /// <summary>
        /// Место хранения
        /// </summary>
        [DisplayName("Место хранения")]
        public string StorageName { get; set; }

        /// <summary>
        /// Идентификатор места хранения
        /// </summary>
        public string StorageId { get; set; }

        /// <summary>
        /// Пользователь, осуществивший приемку
        /// </summary>
        [DisplayName("Смена собственника")]
        public string ChangedOwnerByName { get; set; }
        public string ChangedOwnerById { get; set; }
        public bool AllowToViewChangedOwnerByDetails { get; set; }
        public string ChangeOwnerDate { get; set; }

        /// <summary>
        /// Сумма в закупочных ценах
        /// </summary>
        [DisplayName("Сумма в закупочных ценах")]
        public string PurchaseCostSum { get; set; }

        /// <summary>
        /// Сумма в учетных ценах
        /// </summary>
        [DisplayName("Сумма в учетных ценах")]
        public string AccountingPriceSum { get; set; }

        /// <summary>
        /// Количество позиций
        /// </summary>
        [DisplayName("Кол-во позиций и отгрузка")]
        public string RowCount { get; set; }

        /// <summary>
        /// Отгрузка
        /// </summary>
        public string ShippingPercent { get; set; }

        /// <summary>
        /// "Сумма НДС"
        /// </summary>
        [DisplayName("Сумма НДС")]
        public string ValueAddedTaxString { get; set; }

        /// <summary>
        /// Признак разрешения изменения получателя
        /// </summary>
        public bool AllowToChangeRecipient { get; set; }
    }
}
