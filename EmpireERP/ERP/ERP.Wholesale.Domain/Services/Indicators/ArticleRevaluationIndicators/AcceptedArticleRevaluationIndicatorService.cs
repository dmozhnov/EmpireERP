using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Services;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    /// <summary>
    /// Служба индикаторов проведенной переоценки
    /// </summary>
    public class AcceptedArticleRevaluationIndicatorService : BaseArticleRevaluationIndicatorService<AcceptedArticleRevaluationIndicator>, 
        IAcceptedArticleRevaluationIndicatorService
    {
        #region Конструкторы

        public AcceptedArticleRevaluationIndicatorService(IAcceptedArticleRevaluationIndicatorRepository acceptedArticleRevaluationIndicatorRepository)
            : base(acceptedArticleRevaluationIndicatorRepository)
        {
        }

        #endregion
    }
}
