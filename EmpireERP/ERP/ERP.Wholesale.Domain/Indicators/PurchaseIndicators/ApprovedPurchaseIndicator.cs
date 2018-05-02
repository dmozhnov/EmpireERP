using System;

namespace ERP.Wholesale.Domain.Indicators.PurchaseIndicators
{
    /// <summary>
    /// Показатель "Согласованные закупки"
    /// </summary>
    public class ApprovedPurchaseIndicator : BasePurchaseIndicator
    {
        #region Конструкторы

        public ApprovedPurchaseIndicator()
            : base()
        {
        }

        public ApprovedPurchaseIndicator(DateTime startDate, int articleId, int userId, int contractorId, short storageId, int accountOrganizationId,
            int contractorOrganizationId, short contractId, decimal purchaseCostSum, decimal count) :
            base(startDate, articleId, userId, contractorId, storageId, accountOrganizationId,
                contractorOrganizationId, contractId, purchaseCostSum, count)
        {
        }

        #endregion
    }
}
