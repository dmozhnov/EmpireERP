using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class AcceptedSaleIndicatorService : BaseSaleIndicatorService<AcceptedSaleIndicator>,
                                                IAcceptedSaleIndicatorService
    {
        #region Конструкторы

        public AcceptedSaleIndicatorService(IAcceptedSaleIndicatorRepository acceptedSaleIndicatorRepository, IArticleRepository articleRepository, IDealRepository dealRepository)
            : base(acceptedSaleIndicatorRepository, articleRepository, dealRepository)
        {
        }

        #endregion
    }
}
