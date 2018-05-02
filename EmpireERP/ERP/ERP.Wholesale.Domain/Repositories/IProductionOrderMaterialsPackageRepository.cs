using System;
using ERP.Infrastructure.Repositories;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IProductionOrderMaterialsPackageRepository : IRepository<ProductionOrderMaterialsPackage,Guid>, IFilteredRepository<ProductionOrderMaterialsPackage>, IGetAllRepository<ProductionOrderMaterialsPackage>
    {

    }
}
