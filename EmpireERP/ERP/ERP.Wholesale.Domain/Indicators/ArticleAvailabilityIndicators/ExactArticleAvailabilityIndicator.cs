using System;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Показатель «Точное наличие товаров на складе»
    /// </summary>
    public class ExactArticleAvailabilityIndicator : ArticleAvailabilityIndicator
    {
        #region Конструкторы

        public ExactArticleAvailabilityIndicator()
        {
        }

        public ExactArticleAvailabilityIndicator(DateTime startDate, short storageId, int accountOrganizationId, int articleId, Guid batchId, decimal purchaseCost, decimal count)
            : base(startDate, storageId, accountOrganizationId, articleId, batchId, purchaseCost, count)
        {            
        }

        #endregion      
    }
}
