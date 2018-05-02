using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Infrastructure.Repositories;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IProducerRepository : IRepository<Producer, int>, IFilteredRepository<Producer>
    {
        IList<ProductionOrderPayment> GetPaymentsFilteredList(object state, ParameterString parameterString);
        IList<ProductionOrder> GetProductionOrders(Producer producer);
    }
}
