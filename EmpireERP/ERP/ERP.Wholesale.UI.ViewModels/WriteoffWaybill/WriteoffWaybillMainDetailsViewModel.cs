using System.ComponentModel;
using ERP.Wholesale.UI.ViewModels.BaseWaybill;

namespace ERP.Wholesale.UI.ViewModels.WriteoffWaybill
{
    public class WriteoffWaybillMainDetailsViewModel : BaseWaybillMainDetailsViewModel
    {
        /// <summary>
        /// Место хранения
        /// </summary>
        [DisplayName("Место хранения")]
        public string SenderStorageName { get; set; }

        /// <summary>
        /// Организация
        /// </summary>
        [DisplayName("Организация")]
        public string SenderName { get; set; }

        /// <summary>
        /// Основание списания
        /// </summary>
        [DisplayName("Основание")]
        public string WriteoffReasonName { get; set; }


        /// <summary>
        /// Пользователь, осуществивший списание
        /// </summary>
        [DisplayName("Списание")]
        public string WrittenoffByName { get; set; }
        public string WrittenoffById { get; set; }
        public bool AllowToViewWrittenoffByDetails { get; set; }
        public string WriteoffDate { get; set; }

        /// <summary>
        /// Сумма в ЗЦ
        /// </summary>
        [DisplayName("Сумма в ЗЦ")]
        public string PurchaseCostSum { get; set; }

        /// <summary>
        /// Сумма в УЦ
        /// </summary>
        [DisplayName("Сумма в УЦ")]
        public string SenderAccountingPriceSum { get; set; }

        /// <summary>
        /// Недополученная прибыль
        /// </summary>
        [DisplayName("Недополученная прибыль")]
        public string ReceivelessProfitPercent { get; set; }
        public string ReceivelessProfitSum { get; set; }

        /// <summary>
        /// Количество позиций
        /// </summary>
        [DisplayName("Кол-во позиций")]
        public string RowCount { get; set; }

        public string SenderStorageId { get; set; }
        public string SenderId { get; set; }
        public bool AllowToViewSenderStorageDetails { get; set; }

    }
}