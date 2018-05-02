using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.NHibernate.Repositories.Indicators;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class AcceptedArticleRevaluationIndicatorRepository : BaseArticleRevaluationIndicatorRepository<AcceptedArticleRevaluationIndicator>, 
        IAcceptedArticleRevaluationIndicatorRepository
    {
        public AcceptedArticleRevaluationIndicatorRepository() : base()
        {
        }
    }
}
