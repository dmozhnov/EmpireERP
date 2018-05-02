using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ExactArticleAvailabilityIndicatorRepository : BaseIndicatorRepository<ExactArticleAvailabilityIndicator>, IExactArticleAvailabilityIndicatorRepository
    {
        public ExactArticleAvailabilityIndicatorRepository() : base()
        {
        }
    }
}
