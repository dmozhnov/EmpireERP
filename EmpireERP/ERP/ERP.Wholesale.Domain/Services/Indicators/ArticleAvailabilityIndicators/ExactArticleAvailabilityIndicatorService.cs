using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class ExactArticleAvailabilityIndicatorService : BaseArticleAvailabilityIndicatorService<ExactArticleAvailabilityIndicator>, 
        IExactArticleAvailabilityIndicatorService
    {
        #region Конструкторы

        public ExactArticleAvailabilityIndicatorService(IExactArticleAvailabilityIndicatorRepository exactArticleAvailabilityIndicatorRepository) :
            base(exactArticleAvailabilityIndicatorRepository)
        {
        }

        #endregion

       
    }
}
