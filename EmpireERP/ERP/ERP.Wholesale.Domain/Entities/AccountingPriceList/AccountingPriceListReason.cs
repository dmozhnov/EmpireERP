using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Основание для создания реестра цен
    /// </summary>
    public enum AccountingPriceListReason : byte
    {
        /// <summary>
        /// Приход
        /// </summary>
        [EnumDisplayName("Приход №{0} от {1}")]
        ReceiptWaybill = 1,

        /// <summary>
        /// Переоценка
        /// </summary>
        [EnumDisplayName("Переоценка")]
        Revaluation,

        /// <summary>
        /// По месту хранения
        /// </summary>
        [EnumDisplayName("По месту хранения")]
        Storage
    }
}
