using System;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Показатель «Окончательно принятые возвраты товаров от клиентов»
    /// </summary>
    public class ReceiptedReturnFromClientIndicator : BaseReturnFromClientIndicator
    {
        #region Конструкторы

        public ReceiptedReturnFromClientIndicator() : base()
        {
        }

        public ReceiptedReturnFromClientIndicator(DateTime startDate, int articleId, int saleWaybillCuratorId, int returnFromClientWaybillCuratorId, 
            int contractorId, short storageId, int accountOrganizationId, int clientId, int dealId, int clientOrganizationId, short teamId, Guid saleWaybillId, 
            Guid batchId, decimal purchaseCostSum, decimal accountingPriceSum, decimal salePriceSum, decimal returnedCount)
            
            : base(startDate, articleId, saleWaybillCuratorId, returnFromClientWaybillCuratorId, contractorId, storageId, accountOrganizationId, clientId, dealId, 
                clientOrganizationId, teamId, saleWaybillId, batchId, purchaseCostSum, accountingPriceSum, salePriceSum, returnedCount)
        {
        }

        #endregion
    }
}
