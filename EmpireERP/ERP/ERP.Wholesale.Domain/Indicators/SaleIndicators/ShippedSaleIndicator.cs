
using System;
namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Показатель «Отгруженные реализации»
    /// </summary>
    public class ShippedSaleIndicator : BaseSaleIndicator
    {
        #region Конструкторы

        public ShippedSaleIndicator() : base()
        {
        }

        public ShippedSaleIndicator(DateTime startDate, int articleId, int userId, int contractorId, short storageId, int accountOrganizationId, 
            int clientId, int dealId, int clientOrganizationId, short teamId, Guid batchId, decimal purchaseCostSum, decimal accountingPriceSum, 
            decimal salePriceSum, decimal soldCount) : 
            base(startDate, articleId, userId, contractorId, storageId, accountOrganizationId, clientId, dealId, clientOrganizationId, teamId, batchId, 
                purchaseCostSum, accountingPriceSum, salePriceSum, soldCount)
        {            
        }

        #endregion
    }
}
