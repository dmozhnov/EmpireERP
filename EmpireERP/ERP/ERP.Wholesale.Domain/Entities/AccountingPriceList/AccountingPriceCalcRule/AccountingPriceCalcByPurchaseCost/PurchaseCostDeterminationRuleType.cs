using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Правило определения закупочной цены
    /// </summary>
    public enum PurchaseCostDeterminationRuleType
    {
        [EnumDisplayName("По средней закупочной стоимости")]
        ByAveragePurchasePrice = 1,

        [EnumDisplayName("По минимальной закупочной цене на товар")]
        ByMinimalPurchaseCost,

        [EnumDisplayName("По максимальной закупочной цене на товар")]
        ByMaximalPurchaseCost,

        [EnumDisplayName("По последней закупочной цене")]
        ByLastPurchaseCost
    }
}
