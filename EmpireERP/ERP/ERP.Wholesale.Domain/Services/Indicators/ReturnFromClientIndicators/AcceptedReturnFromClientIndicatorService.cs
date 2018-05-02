using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class AcceptedReturnFromClientIndicatorService : BaseReturnFromClientIndicatorService<AcceptedReturnFromClientIndicator>,
                                                            IAcceptedReturnFromClientIndicatorService
    {
        #region Конструкторы

        public AcceptedReturnFromClientIndicatorService(IAcceptedReturnFromClientIndicatorRepository acceptedReturnFromClientIndicatorRepository, IArticleRepository articleRepository)
            : base(acceptedReturnFromClientIndicatorRepository, articleRepository)
        {
        }

        #endregion
    }
}