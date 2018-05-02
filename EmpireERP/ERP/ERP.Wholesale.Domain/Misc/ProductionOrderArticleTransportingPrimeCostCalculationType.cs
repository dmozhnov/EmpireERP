using ERP.Utils;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Возможные способы подсчета себестоимости транспортировки
    /// </summary>
    public enum ProductionOrderArticleTransportingPrimeCostCalculationType : byte
    {
        /// <summary>
        /// По объему товара в позиции заказа
        /// </summary>
        [EnumDisplayName("По объему позиции заказа")]
        Volume = 1,

        /// <summary>
        /// По весу товара в позиции заказа
        /// </summary>
        [EnumDisplayName("По весу позиции заказа")]
        Weight,

        /// <summary>
        /// Не указано
        /// </summary>
        [EnumDisplayName("Не указано")]
        Undefined
    }
}
