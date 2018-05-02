using System;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ProductionOrderPlannedPaymentRepository : BaseRepository, IProductionOrderPlannedPaymentRepository
    {
        public ProductionOrderPlannedPaymentRepository()
        {
        }

        public ProductionOrderPlannedPayment GetById(Guid id)
        {
            return CurrentSession.Get<ProductionOrderPlannedPayment>(id);
        }

        public void Save(ProductionOrderPlannedPayment entity)
        {
            CurrentSession.SaveOrUpdate(entity);   
        }

        public void Delete(ProductionOrderPlannedPayment entity)
        {
            CurrentSession.Delete(entity);
        }        

        public IList<ProductionOrderPlannedPayment> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrderPlannedPayment>(state, ignoreDeletedRows);
        }
        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        public IList<ProductionOrderPlannedPayment> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrderPlannedPayment>(state, parameterString, ignoreDeletedRows);
        }
    }
}
