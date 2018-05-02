using System;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ProductionOrderPaymentRepository : BaseRepository, IProductionOrderPaymentRepository
    {
        // методы Save и Delete не реализованы, т.к. за это отвечает производитель!!!
        // Этот репозиторий используется для списка оплат

        public ProductionOrderPayment GetById(Guid id)
        {
            return Query<ProductionOrderPayment>().Where(x => x.Id == id).FirstOrDefault<ProductionOrderPayment>();
        }

        public IEnumerable<ProductionOrderPayment> GetAll()
        {
            return CurrentSession.CreateCriteria(typeof(ProductionOrderPayment)).List<ProductionOrderPayment>();
        }

        public IList<ProductionOrderPayment> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrderPayment>(state, ignoreDeletedRows);
        }

        public IList<ProductionOrderPayment> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrderPayment>(state, parameterString, ignoreDeletedRows);
        }
    }
}
