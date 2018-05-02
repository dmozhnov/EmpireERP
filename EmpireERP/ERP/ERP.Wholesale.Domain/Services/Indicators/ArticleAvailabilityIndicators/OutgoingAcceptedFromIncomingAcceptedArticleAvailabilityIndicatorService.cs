using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService : 
        BaseArticleAvailabilityIndicatorService<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>,
        IOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService
    {
        #region Конструкторы

        public OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService(IOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorRepository outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorRepository) :
            base(outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorRepository)
        {
        }

        #endregion
    }
}
