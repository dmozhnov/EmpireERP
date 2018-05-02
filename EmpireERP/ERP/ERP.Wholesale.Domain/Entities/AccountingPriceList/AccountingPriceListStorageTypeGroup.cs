using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип места хранения для правила определения учетной цены
    /// </summary>
    public enum AccountingPriceListStorageTypeGroup : byte
    {
        /// <summary>
        /// Всех мест хранения
        /// </summary>
        [EnumDisplayName("Всех мест хранения")]
        All = 1,

        /// <summary>
        /// Распределительных центров
        /// </summary>
        [EnumDisplayName("Распределительных центров")]
        DistributionCenters,

        /// <summary>
        /// Торговых точек
        /// </summary>
        [EnumDisplayName("Торговых точек")]
        TradePoints,

        /// <summary>
        /// Дополнительных складов
        /// </summary>
        [EnumDisplayName("Дополнительных складов")]
        ExtraStorages
    }
}
