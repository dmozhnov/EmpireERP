using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип правила определения % наценки
    /// </summary>
    public enum MarkupPercentDeterminationRuleType
    {
        [EnumDisplayName("% наценки, заданный для товара")]
        ByArticle = 1,

        [EnumDisplayName("% наценки, заданный по группам")]
        ByArticleGroup,

        [EnumDisplayName("указанный % наценки")]
        Custom
    }
}
