using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип правила определения учетной цены
    /// </summary>
    public enum AccountingPriceDeterminationRuleType : byte
    {
        [EnumDisplayName("По средней учетной стоимости среди")]
        ByAverageAccountingPrice = 1,

        [EnumDisplayName("По минимальной учетной цене среди указанных")]
        ByMinimalAccountingPrice,

        [EnumDisplayName("По максимальной учетной цене среди указанных")]
        ByMaximalAccountingPrice,

        [EnumDisplayName("По учетной цене на указанном МХ")]
        ByAccountingPriceOnStorage
    }
}
