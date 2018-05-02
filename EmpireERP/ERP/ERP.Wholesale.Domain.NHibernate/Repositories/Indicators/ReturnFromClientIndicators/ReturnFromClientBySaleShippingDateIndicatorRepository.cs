using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ReturnFromClientBySaleShippingDateIndicatorRepository : BaseReturnFromClientIndicatorRepository<ReturnFromClientBySaleShippingDateIndicator>,
                                                                         IReturnFromClientBySaleShippingDateIndicatorRepository
    {
        public ReturnFromClientBySaleShippingDateIndicatorRepository() : base()
        {
        }
    }
}