using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ReceiptedReturnFromClientIndicatorRepository : BaseReturnFromClientIndicatorRepository<ReceiptedReturnFromClientIndicator>, 
                                                                IReceiptedReturnFromClientIndicatorRepository
    {
        public ReceiptedReturnFromClientIndicatorRepository() : base()
        {
        }
    }
}
