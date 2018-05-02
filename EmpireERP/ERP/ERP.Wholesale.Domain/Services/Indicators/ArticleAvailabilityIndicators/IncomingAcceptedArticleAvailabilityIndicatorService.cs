using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class IncomingAcceptedArticleAvailabilityIndicatorService : BaseArticleAvailabilityIndicatorService<IncomingAcceptedArticleAvailabilityIndicator>,
        IIncomingAcceptedArticleAvailabilityIndicatorService
    {
        #region Конструкторы

        public IncomingAcceptedArticleAvailabilityIndicatorService(IIncomingAcceptedArticleAvailabilityIndicatorRepository incomingAcceptedArticleAvailabilityIndicatorRepository) :
            base(incomingAcceptedArticleAvailabilityIndicatorRepository)
        {            
        }

        #endregion

    }
}
