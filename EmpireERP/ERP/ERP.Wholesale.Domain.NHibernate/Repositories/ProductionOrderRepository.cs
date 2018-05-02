using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Repositories;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Utils;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ProductionOrderRepository : BaseRepository, IProductionOrderRepository
    {
        public ProductionOrderRepository()
        {
        }

        public ProductionOrder GetById(Guid id)
        {
            return CurrentSession.Get<ProductionOrder>(id);
        }

        public void Save(ProductionOrder entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(ProductionOrder entity)
        {
            CurrentSession.Delete(entity);
        }
        
        public IList<ProductionOrder> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrder>(state, ignoreDeletedRows);
        }

        public IList<ProductionOrder> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrder>(state, parameterString, ignoreDeletedRows);
        }
    }
}
