using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    /// <summary>
    /// Служба индикаторов точной переоценки
    /// </summary>
    public class ExactArticleRevaluationIndicatorService : BaseArticleRevaluationIndicatorService<ExactArticleRevaluationIndicator>, IExactArticleRevaluationIndicatorService
    {
        #region Конструкторы

        public ExactArticleRevaluationIndicatorService(IExactArticleRevaluationIndicatorRepository exactArticleRevaluationIndicatorRepository)
            : base(exactArticleRevaluationIndicatorRepository)
        {
        }

        #endregion
    }
}
