using System;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Показатель "Возвраты от клиентов на дату проводки реализации-источника"
    /// </summary>
    public class ReturnFromClientBySaleAcceptanceDateIndicator : BaseReturnFromClientIndicator
    {
        #region Конструкторы

        public ReturnFromClientBySaleAcceptanceDateIndicator() : base()
        {
        }

        public ReturnFromClientBySaleAcceptanceDateIndicator(DateTime startDate, int articleId, int saleWaybillCuratorId, int returnFromClientWaybillCuratorId, int contractorId, short storageId, int accountOrganizationId, 
            int clientId, int dealId, int clientOrganizationId, short teamId, Guid saleWaybillId, Guid batchId, decimal purchaseCostSum, 
            decimal accountingPriceSum, decimal salePriceSum, decimal returnedCount)
            
            : base(startDate, articleId, saleWaybillCuratorId, returnFromClientWaybillCuratorId, contractorId, storageId, accountOrganizationId, clientId, dealId, 
                clientOrganizationId, teamId, saleWaybillId, batchId, purchaseCostSum, accountingPriceSum, salePriceSum, returnedCount)
        {            
        }

        #endregion
    }
}
