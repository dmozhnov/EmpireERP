using System;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ProductionOrderMaterialsPackageRepository : BaseRepository, IProductionOrderMaterialsPackageRepository
    {
        public ProductionOrderMaterialsPackage GetById(Guid id)
        {
            return Query<ProductionOrderMaterialsPackage>().Where(x => x.Id == id).FirstOrDefault<ProductionOrderMaterialsPackage>();
        }

        public void Save(ProductionOrderMaterialsPackage entity)
        {
            CurrentSession.SaveOrUpdate(entity);
            CurrentSession.Flush();
        }

        public void Delete(ProductionOrderMaterialsPackage entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public IEnumerable<ProductionOrderMaterialsPackage> GetAll()
        {
            return CurrentSession.CreateCriteria(typeof(ProductionOrderMaterialsPackage)).List<ProductionOrderMaterialsPackage>();
        }

        public IList<ProductionOrderMaterialsPackage> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrderMaterialsPackage>(state, ignoreDeletedRows);
        }

        public IList<ProductionOrderMaterialsPackage> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrderMaterialsPackage>(state, parameterString, ignoreDeletedRows);
        }
    }
}
