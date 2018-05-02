using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Форма оплаты
    /// </summary>
    public enum ProductionOrderPaymentForm : byte
    {
        /// <summary>
        /// Наличными денежными средствами
        /// </summary>
        [EnumDisplayName("Наличными денежными средствами")]
        Cash = 1,

        /// <summary>
        /// По безналичному расчету
        /// </summary>
        [EnumDisplayName("По безналичному расчету")]
        Cashless
    }
}
