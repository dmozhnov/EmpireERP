using System;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ProductionOrderTransportSheetRepository : BaseRepository, IProductionOrderTransportSheetRepository
    {
        public IList<ProductionOrderTransportSheet> GetFilteredList(object state, ParameterString parameterString)
        {
            return GetBaseFilteredList<ProductionOrderTransportSheet>(state, parameterString);
        }

        public ProductionOrderTransportSheet GetById(Guid id)
        {
            return Query<ProductionOrderTransportSheet>().Where(x => x.Id == id).FirstOrDefault<ProductionOrderTransportSheet>();
        }

        public void Save(ProductionOrderTransportSheet entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(ProductionOrderTransportSheet entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public IEnumerable<ProductionOrderTransportSheet> GetAll()
        {
            return CurrentSession.CreateCriteria(typeof(ProductionOrderTransportSheet)).List<ProductionOrderTransportSheet>();
        }

        public IList<ProductionOrderTransportSheet> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrderTransportSheet>(state, ignoreDeletedRows);
        }

        public IList<ProductionOrderTransportSheet> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrderTransportSheet>(state, parameterString, ignoreDeletedRows);
        }
    }
}
