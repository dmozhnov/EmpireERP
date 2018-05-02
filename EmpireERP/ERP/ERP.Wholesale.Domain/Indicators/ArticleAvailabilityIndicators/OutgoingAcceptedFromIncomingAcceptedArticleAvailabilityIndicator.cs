using System;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Показатель «Исходящее проведенное (из входящего проведенного) наличие товаров на складе»
    /// </summary>
    /// <remarks>Содержит информацию о кол-ве товара по проведенным (из проведенных входящих накладных) незавершенным исходящим накладным</remarks>
    public class OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator : ArticleAvailabilityIndicator
    {
        #region Конструкторы

        public OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator()
        {
        }

        public OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(DateTime startDate, short storageId, int accountOrganizationId, int articleId, Guid batchId, decimal purchaseCost, decimal count)
            : base(startDate, storageId, accountOrganizationId, articleId, batchId, purchaseCost, count)
        {
        }

        #endregion
    }
}
