using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ContractorRepository : BaseRepository, IContractorRepository
    {
        public ContractorRepository() : base()
        {
        }

        public Contractor GetById(int id)
        {
            return Query<Contractor>().Where(x => x.Id == id).FirstOrDefault<Contractor>();
        }

        public void Save(Contractor entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(Contractor entity)
        {
            entity.DeletionDate = DateTime.Now;
            CurrentSession.SaveOrUpdate(entity);
        }

        public IList<Contractor> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return base.GetBaseFilteredList<Contractor>(state, ignoreDeletedRows);
        }

        public IList<Contractor> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return base.GetBaseFilteredList<Contractor>(state, parameterString, ignoreDeletedRows);
        }
    }
}