using System;

namespace ERP.Wholesale.Domain.Indicators.PurchaseIndicators
{
    /// <summary>
    /// Показатель «Проведенные закупки»
    /// </summary>
    public class AcceptedPurchaseIndicator : BasePurchaseIndicator
    {
        #region Конструкторы

        public AcceptedPurchaseIndicator()
            : base()
        {
        }

        public AcceptedPurchaseIndicator(DateTime startDate, int articleId, int userId, int contractorId, short storageId, int accountOrganizationId,
            int contractorOrganizationId, short contractId, decimal purchaseCostSum, decimal count) :
            base(startDate, articleId, userId, contractorId, storageId, accountOrganizationId,
                contractorOrganizationId, contractId, purchaseCostSum, count)
        {
        }

        #endregion
    }
}
