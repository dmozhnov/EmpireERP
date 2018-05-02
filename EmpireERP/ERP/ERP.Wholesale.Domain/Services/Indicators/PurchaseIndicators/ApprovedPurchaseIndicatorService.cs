using ERP.Wholesale.Domain.AbstractServices.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.Repositories.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services.Indicators.PurchaseIndicators
{
    public class ApprovedPurchaseIndicatorService : BasePurchaseIndicatorService<ApprovedPurchaseIndicator>, IApprovedPurchaseIndicatorService
    {
        #region Конструкторы

        public ApprovedPurchaseIndicatorService(IApprovedPurchaseIndicatorRepository approvedPurchaseIndicatorRepository, IReceiptWaybillRepository receiptWaybillRepository)
            : base(approvedPurchaseIndicatorRepository, receiptWaybillRepository)
        {
        }

        #endregion
    }    
}
