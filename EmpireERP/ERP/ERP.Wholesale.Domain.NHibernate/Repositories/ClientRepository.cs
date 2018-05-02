using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ClientRepository : BaseNHRepository, IClientRepository
    {
        public ClientRepository()
        {
        }

        public Client GetById(int id)
        {
            return CurrentSession.Get<Client>(id);
        }

        public void Save(Client entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(Client entity)
        {
            CurrentSession.Delete(entity);
        }

        public IEnumerable<Client> GetAll()
        {
            return CurrentSession.CreateCriteria(typeof(Client)).List<Client>();
        }

        /// <summary>
        /// Получение списка клиентов по Id
        /// </summary>
        /// <param name="idList">Список кодов клиентов</param>
        public IEnumerable<Client> GetList(IEnumerable<int> idList)
        {
            return base.GetList<int, Client>(idList).Values.ToList<Client>();
        }

        public IList<Client> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Client>(state, ignoreDeletedRows);
        }
        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        public IList<Client> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Client>(state, parameterString, ignoreDeletedRows);
        }

        /// <summary>
        /// Получить список клиентов по подзапросу сделок (выбираются клиенты этих сделок)
        /// </summary>
        /// <param name="dealSubQuery">подкритерий сделок</param>
        public IDictionary<int, Client> GetDealClientList(ISubQuery dealSubQuery)
        {
            var subQuery = SubQuery<Deal>()
                .PropertyIn(x => x.Id, dealSubQuery)
                .Select(x => x.Client.Id);

            return Query<Client>()
                .PropertyIn(x => x.Id, subQuery)
                .ToList<Client>()
                .Distinct()
                .ToDictionary(x => x.Id);
        }
    }
}
