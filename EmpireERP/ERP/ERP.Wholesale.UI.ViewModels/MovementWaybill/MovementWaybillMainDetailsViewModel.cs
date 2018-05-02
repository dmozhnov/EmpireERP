using System.ComponentModel;
using ERP.Wholesale.UI.ViewModels.BaseWaybill;

namespace ERP.Wholesale.UI.ViewModels.MovementWaybill
{
    public class MovementWaybillMainDetailsViewModel : BaseWaybillMainDetailsViewModel
    {
        /// <summary>
        /// Номер накладной
        /// </summary>
        [DisplayName("Номер")]
        public string Number { get; set; }

        /// <summary>
        /// Дата накладной
        /// </summary>
        [DisplayName("Дата")]
        public string Date { get; set; }

        [DisplayName("Отправитель")]
        public string SenderStorageName { get; set; }
        public string SenderStorageId { get; set; }

        [DisplayName("Организация отправителя")]
        public string SenderName { get; set; }
        public string SenderId { get; set; }

        [DisplayName("Получатель")]
        public string RecipientStorageName { get; set; }
        public string RecipientStorageId { get; set; }

        [DisplayName("Организация получателя")]
        public string RecipientName { get; set; }
        public string RecipientId { get; set; }

        /// <summary>
        /// Пользователь, отгрузивший накладную
        /// </summary>
        [DisplayName("Отгрузка")]
        public string ShippedByName { get; set; }
        public string ShippedById { get; set; }
        public bool AllowToViewShippedByDetails { get; set; }
        public string ShippingDate { get; set; }

        /// <summary>
        /// Пользователь, осуществивший приемку
        /// </summary>
        [DisplayName("Приемка")]
        public string ReceiptedByName { get; set; }
        public string ReceiptedById { get; set; }
        public bool AllowToViewReceiptedByDetails { get; set; }
        public string ReceiptDate { get; set; }


        [DisplayName("Сумма в ЗЦ")]
        public string PurchaseCostSum { get; set; }

        [DisplayName("Сумма в УЦ отправит.")]
        public string SenderAccountingPriceSum { get; set; }

        [DisplayName(" | получ.")]
        public string RecipientAccountingPriceSum { get; set; }

        [DisplayName("Наценка при перемещении")]
        public string MovementMarkupPercent { get; set; }
        public string MovementMarkupSum { get; set; }

        [DisplayName("Кол-во позиций и отгрузка")]
        public string RowCount { get; set; }
        public string ShippingPercent { get; set; }

        [DisplayName("Сумма НДС отправителя")]
        public string SenderValueAddedTaxString { get; set; }
        [DisplayName("Сумма НДС получателя")]
        public string RecipientValueAddedTaxString { get; set; }

        public bool AllowToViewSenderStorageDetails { get; set; }
        public bool AllowToViewRecipientStorageDetails { get; set; }

    }
}