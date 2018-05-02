using System;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ProductionOrderExtraExpensesSheetRepository : BaseRepository, IProductionOrderExtraExpensesSheetRepository
    {
        public IList<ProductionOrderExtraExpensesSheet> GetFilteredList(object state, ParameterString parameterString)
        {
            return GetBaseFilteredList<ProductionOrderExtraExpensesSheet>(state, parameterString);
        }

        public ProductionOrderExtraExpensesSheet GetById(Guid id)
        {
            return Query<ProductionOrderExtraExpensesSheet>().Where(x => x.Id == id).FirstOrDefault<ProductionOrderExtraExpensesSheet>();
        }

        public void Save(ProductionOrderExtraExpensesSheet entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(ProductionOrderExtraExpensesSheet entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public IEnumerable<ProductionOrderExtraExpensesSheet> GetAll()
        {
            return CurrentSession.CreateCriteria(typeof(ProductionOrderExtraExpensesSheet)).List<ProductionOrderExtraExpensesSheet>();
        }

        public IList<ProductionOrderExtraExpensesSheet> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrderExtraExpensesSheet>(state, ignoreDeletedRows);
        }

        public IList<ProductionOrderExtraExpensesSheet> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrderExtraExpensesSheet>(state, parameterString, ignoreDeletedRows);
        }
    }
}
