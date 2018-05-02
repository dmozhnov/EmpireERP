using ERP.Utils;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Возможные способы подсчета себестоимости
    /// </summary>
    public enum ProductionOrderArticlePrimeCostCalculationType : byte
    {
        /// <summary>
        /// По плановой стоимости
        /// </summary>
        [EnumDisplayName("По плановой стоимости")]
        PlannedExpenses = 1,

        /// <summary>
        /// По текущей фактической стоимости
        /// </summary>
        [EnumDisplayName("По текущей фактической стоимости")]
        ActualExpenses,

        /// <summary>
        /// По текущим оплатам
        /// </summary>
        [EnumDisplayName("По текущим оплатам")]
        PaymentSum
    }
}
