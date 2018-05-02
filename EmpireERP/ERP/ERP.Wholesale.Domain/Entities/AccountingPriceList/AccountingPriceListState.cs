using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Статус реестра цен
    /// </summary>
    public enum AccountingPriceListState : byte
    {
        /// <summary>
        /// Новый
        /// </summary>
        [EnumDisplayName("Новый")]
        New = 1,

        /// <summary>
        /// Проведен
        /// </summary>
        [EnumDisplayName("Проведен")]
        Accepted
    }
}
