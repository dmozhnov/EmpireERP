using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class AcceptedSaleIndicatorRepository : BaseSaleIndicatorRepository<AcceptedSaleIndicator>,
                                                   IAcceptedSaleIndicatorRepository
    {
        public AcceptedSaleIndicatorRepository() : base()
        {
        }
    }
}
