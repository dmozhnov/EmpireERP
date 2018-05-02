using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип места хранения
    /// </summary>
    public enum StorageType : byte
    {
        /// <summary>
        /// Распределительный центр
        /// </summary>
        [EnumDisplayName("Распределительный центр")]
        DistributionCenter = 1,

        /// <summary>
        /// Торговая точка
        /// </summary>
        [EnumDisplayName("Торговая точка")]
        TradePoint,

        /// <summary>
        /// Дополнительный склад
        /// </summary>
        [EnumDisplayName("Дополнительный склад")]
        ExtraStorage
    }
}
