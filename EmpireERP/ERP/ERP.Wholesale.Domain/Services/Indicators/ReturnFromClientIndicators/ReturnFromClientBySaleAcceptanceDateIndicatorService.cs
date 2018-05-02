using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class ReturnFromClientBySaleAcceptanceDateIndicatorService : BaseReturnFromClientIndicatorService<ReturnFromClientBySaleAcceptanceDateIndicator>,
                                                                        IReturnFromClientBySaleAcceptanceDateIndicatorService
    {
        #region Конструкторы

        public ReturnFromClientBySaleAcceptanceDateIndicatorService(IReturnFromClientBySaleAcceptanceDateIndicatorRepository returnFromClientBySaleAcceptanceDateIndicatorRepository, 
            IArticleRepository articleRepository)
            : base(returnFromClientBySaleAcceptanceDateIndicatorRepository, articleRepository)
        {
        }

        #endregion
    }
}