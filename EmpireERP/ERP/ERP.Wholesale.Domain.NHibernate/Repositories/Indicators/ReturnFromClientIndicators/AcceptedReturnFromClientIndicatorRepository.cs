using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class AcceptedReturnFromClientIndicatorRepository : BaseReturnFromClientIndicatorRepository<AcceptedReturnFromClientIndicator>,
                                                               IAcceptedReturnFromClientIndicatorRepository
    {
        public AcceptedReturnFromClientIndicatorRepository() : base()
        {
        }
    }
}