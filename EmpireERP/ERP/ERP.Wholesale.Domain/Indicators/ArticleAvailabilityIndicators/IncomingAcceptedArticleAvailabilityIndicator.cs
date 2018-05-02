using System;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Показатель «Входящее проведенное наличие товаров на складе»
    /// </summary>
    /// <remarks>Содержит информацию о кол-ве товара по проведенным незавершенным входящим накладным</remarks>
    public class IncomingAcceptedArticleAvailabilityIndicator : ArticleAvailabilityIndicator
    {
        #region Конструкторы

        public IncomingAcceptedArticleAvailabilityIndicator()
        {
        }

        public IncomingAcceptedArticleAvailabilityIndicator(DateTime startDate, short storageId, int accountOrganizationId, int articleId, Guid batchId, decimal purchaseCost, decimal count)
            : base(startDate, storageId, accountOrganizationId, articleId, batchId, purchaseCost, count)
        {            
        }

        #endregion      
    }
}
