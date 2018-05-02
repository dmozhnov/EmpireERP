using System;
using ERP.Infrastructure.Repositories;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IProductionOrderPlannedPaymentRepository : IRepository<ProductionOrderPlannedPayment, Guid>, IFilteredRepository<ProductionOrderPlannedPayment>
    {
    }
}
