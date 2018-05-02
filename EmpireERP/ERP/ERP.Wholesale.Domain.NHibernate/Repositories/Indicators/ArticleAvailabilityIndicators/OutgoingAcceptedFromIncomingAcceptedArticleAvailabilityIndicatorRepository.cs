using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorRepository : BaseIndicatorRepository<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>, 
        IOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorRepository
    {
        public OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorRepository() : base()
        {
        }
    }
}
