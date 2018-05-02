using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.NHibernate.Repositories.Indicators;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ExactArticleRevaluationIndicatorRepository : BaseArticleRevaluationIndicatorRepository<ExactArticleRevaluationIndicator>, 
        IExactArticleRevaluationIndicatorRepository
    {
        public ExactArticleRevaluationIndicatorRepository() : base()
        {
        }
    }
}
