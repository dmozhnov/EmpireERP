using System;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class DealPaymentToClientRepository : BaseNHRepository, IDealPaymentToClientRepository
    {
        public DealPaymentToClientRepository()
        {
        }

        public DealPaymentToClient GetById(Guid id)
        {
            return Query<DealPaymentToClient>().Where(x => x.Id == id).FirstOrDefault<DealPaymentToClient>();
        }

        public void Save(DealPaymentToClient value)
        {
            CurrentSession.SaveOrUpdate(value);
            CurrentSession.Flush();
        }

        public void Delete(DealPaymentToClient value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public IList<DealPaymentToClient> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealPaymentToClient>(state, ignoreDeletedRows);
        }
        public IList<DealPaymentToClient> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealPaymentToClient>(state, parameterString, ignoreDeletedRows);
        }
    }
}
