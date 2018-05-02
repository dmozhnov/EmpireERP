using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Repositories;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ProductionOrderBatchRepository : BaseRepository, IProductionOrderBatchRepository
    {
        public ProductionOrderBatchRepository()
        {
        }

        public ProductionOrderBatch GetById(Guid id)
        {
            return CurrentSession.Get<ProductionOrderBatch>(id);
        }

        public void Save(ProductionOrderBatch entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(ProductionOrderBatch entity)
        {
            CurrentSession.Delete(entity);
        }
    }
}
