using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Назначение оплаты по заказу (влияет на то, на какой документ она может быть отнесена)
    /// </summary>
    public enum ProductionOrderPaymentType : byte
    {
        /// <summary>
        /// Оплата за производство (фактически по самому заказу, вернее, по сумме его партий)
        /// </summary>
        [EnumDisplayName("За производство")]
        ProductionOrderProductionPayment = 1,

        /// <summary>
        /// Транспортный лист
        /// </summary>
        [EnumDisplayName("За транспортные листы")]
        ProductionOrderTransportSheetPayment,

        /// <summary>
        /// Лист дополнительных расходов
        /// </summary>
        [EnumDisplayName("За листы дополнительных расходов")]
        ProductionOrderExtraExpensesSheetPayment,

        /// <summary>
        /// Таможенный лист
        /// </summary>
        [EnumDisplayName("За таможенные листы")]
        ProductionOrderCustomsDeclarationPayment
    }
}
