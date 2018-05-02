using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class ReturnFromClientBySaleShippingDateIndicatorService : BaseReturnFromClientIndicatorService<ReturnFromClientBySaleShippingDateIndicator>,
                                                                      IReturnFromClientBySaleShippingDateIndicatorService
    {
        #region Конструкторы

        public ReturnFromClientBySaleShippingDateIndicatorService(IReturnFromClientBySaleShippingDateIndicatorRepository returnFromClientBySaleShippingDateIndicatorRepository, 
            IArticleRepository articleRepository)
            : base(returnFromClientBySaleShippingDateIndicatorRepository, articleRepository)
        {
        }

        #endregion
    }
}