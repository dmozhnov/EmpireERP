using System;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ProductionOrderCustomsDeclarationRepository : BaseRepository, IProductionOrderCustomsDeclarationRepository
    {
        public IList<ProductionOrderCustomsDeclaration> GetFilteredList(object state, ParameterString parameterString)
        {
            return GetBaseFilteredList<ProductionOrderCustomsDeclaration>(state, parameterString);
        }

        public ProductionOrderCustomsDeclaration GetById(Guid id)
        {
            return Query<ProductionOrderCustomsDeclaration>().Where(x => x.Id == id).FirstOrDefault<ProductionOrderCustomsDeclaration>();
        }

        public void Save(ProductionOrderCustomsDeclaration entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(ProductionOrderCustomsDeclaration entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public IEnumerable<ProductionOrderCustomsDeclaration> GetAll()
        {
            return CurrentSession.CreateCriteria(typeof(ProductionOrderCustomsDeclaration)).List<ProductionOrderCustomsDeclaration>();
        }

        public IList<ProductionOrderCustomsDeclaration> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrderCustomsDeclaration>(state, ignoreDeletedRows);
        }

        public IList<ProductionOrderCustomsDeclaration> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrderCustomsDeclaration>(state, parameterString, ignoreDeletedRows);
        }
    }
}
