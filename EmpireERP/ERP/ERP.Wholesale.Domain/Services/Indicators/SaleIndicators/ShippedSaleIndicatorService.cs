using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class ShippedSaleIndicatorService : BaseSaleIndicatorService<ShippedSaleIndicator>,
                                               IShippedSaleIndicatorService
    {
        #region Конструкторы

        public ShippedSaleIndicatorService(IShippedSaleIndicatorRepository shippedSaleIndicatorRepository, IArticleRepository articleRepository, IDealRepository dealRepository)
            : base(shippedSaleIndicatorRepository, articleRepository, dealRepository)
        {
        }

        #endregion
    }
}
