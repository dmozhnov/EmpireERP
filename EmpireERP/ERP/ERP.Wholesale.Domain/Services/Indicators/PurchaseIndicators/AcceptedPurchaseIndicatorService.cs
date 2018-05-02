using ERP.Wholesale.Domain.AbstractServices.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.Repositories.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services.Indicators.PurchaseIndicators
{
    public class AcceptedPurchaseIndicatorService : BasePurchaseIndicatorService<AcceptedPurchaseIndicator>, IAcceptedPurchaseIndicatorService
    {
        #region Конструкторы

        public AcceptedPurchaseIndicatorService(IAcceptedPurchaseIndicatorRepository acceptedPurchaseIndicatorRepository, IReceiptWaybillRepository receiptWaybillRepository)
            : base(acceptedPurchaseIndicatorRepository, receiptWaybillRepository)
        {
        }

        #endregion
    }    
}
