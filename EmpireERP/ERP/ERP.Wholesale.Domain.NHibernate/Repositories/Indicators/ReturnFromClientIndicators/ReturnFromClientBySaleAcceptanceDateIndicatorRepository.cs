using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ReturnFromClientBySaleAcceptanceDateIndicatorRepository : BaseReturnFromClientIndicatorRepository<ReturnFromClientBySaleAcceptanceDateIndicator>,
                                                                           IReturnFromClientBySaleAcceptanceDateIndicatorRepository
    {
        public ReturnFromClientBySaleAcceptanceDateIndicatorRepository() : base()
        {
        }
    }
}