using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип правила расчета учетной цены
    /// </summary>
    public enum AccountingPriceCalcRuleType : byte
    {
        [EnumDisplayName("Рассчитывать от закупочной цены + % наценки")]
        ByPurchaseCost = 1,

        [EnumDisplayName("Рассчитывать от действующей учетной цены + % наценки (- % скидки)")]
        ByCurrentAccountingPrice
    }
}
