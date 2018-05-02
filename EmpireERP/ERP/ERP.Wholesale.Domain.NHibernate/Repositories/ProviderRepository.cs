using System;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate;
using NHibernate.Criterion;
using ERP.Utils;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ProviderRepository : BaseRepository, IProviderRepository
    {
        public ProviderRepository() : base()
        {
        }

        public Provider GetById(int id)
        {
            return Query<Provider>().Where(x => x.Id == id).FirstOrDefault<Provider>();
        }

        public void Save(Provider entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(Provider entity)
        {
            entity.DeletionDate = DateTime.Now;
            CurrentSession.SaveOrUpdate(entity);
        }

        public IEnumerable<Provider> GetAll()
        {
            return CurrentSession.CreateCriteria(typeof(Provider)).List<Provider>();
        }

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        public IList<Provider> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Provider>(state, ignoreDeletedRows);
        }
        public IList<Provider> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Provider>(state, parameterString, ignoreDeletedRows);
        }

    }
}