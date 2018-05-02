using System;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class DealPaymentFromClientRepository : BaseRepository, IDealPaymentFromClientRepository
    {
        public DealPaymentFromClientRepository()
        {
        }

        public DealPaymentFromClient GetById(Guid id)
        {
            return Query<DealPaymentFromClient>().Where(x => x.Id == id).FirstOrDefault<DealPaymentFromClient>();
        }

        public void Save(DealPaymentFromClient value)
        {
            CurrentSession.SaveOrUpdate(value);
            CurrentSession.Flush();
        }

        public void Delete(DealPaymentFromClient value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public IList<DealPaymentFromClient> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealPaymentFromClient>(state, ignoreDeletedRows);
        }
        public IList<DealPaymentFromClient> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealPaymentFromClient>(state, parameterString, ignoreDeletedRows);
        }
    }
}
