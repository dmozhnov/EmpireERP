using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class IncomingAcceptedArticleAvailabilityIndicatorRepository : BaseIndicatorRepository<IncomingAcceptedArticleAvailabilityIndicator>, IIncomingAcceptedArticleAvailabilityIndicatorRepository
    {
        public IncomingAcceptedArticleAvailabilityIndicatorRepository() : base()
        {
        }
    }
}
