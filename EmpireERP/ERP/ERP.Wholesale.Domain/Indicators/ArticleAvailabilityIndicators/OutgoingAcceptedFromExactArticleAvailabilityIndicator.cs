using System;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Показатель «Исходящее проведенное (из точного) наличие товаров на складе»
    /// </summary>
    /// <remarks>Содержит информацию о кол-ве товара по проведенным (из завершенных накладных) незавершенным исходящим накладным</remarks>
    public class OutgoingAcceptedFromExactArticleAvailabilityIndicator : ArticleAvailabilityIndicator
    {
        #region Конструкторы

        public OutgoingAcceptedFromExactArticleAvailabilityIndicator()
        {
        }

        public OutgoingAcceptedFromExactArticleAvailabilityIndicator(DateTime startDate, short storageId, int accountOrganizationId, int articleId, Guid batchId, decimal purchaseCost, decimal count)
            : base(startDate, storageId, accountOrganizationId, articleId, batchId, purchaseCost, count)
        {            
        }

        #endregion      
    }
}
