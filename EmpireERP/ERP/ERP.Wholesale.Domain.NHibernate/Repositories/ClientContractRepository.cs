using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Repositories;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Utils;
using ERP.Infrastructure.Repositories.Criteria;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ClientContractRepository : BaseRepository, IClientContractRepository
    {
        public ClientContractRepository()
        {
        }

        public ClientContract GetById(short id)
        {
            return CurrentSession.Get<ClientContract>(id);
        }

        public void Save(ClientContract entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(ClientContract entity)
        {
            entity.DeletionDate = DateTime.Now;
            CurrentSession.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Получение договоров по подзапросу сделок, которые активны в указанный период (есть пересечение временных интервалов)
        /// </summary>
        /// <param name="subQuery">Подзапрос сделок</param>
        /// <returns></returns>
        public IEnumerable<ClientContract> GetList(DateTime startDate, DateTime endDate, ISubCriteria<Deal> subQuery)
        {
            ISubCriteria<Deal> sq = null; 
            if (subQuery != null)
            {
                sq = SubQuery<Deal>().PropertyIn(x => x.Id, subQuery).Select(x => x.Contract.Id);
            }

            var result = Query<ClientContract>()
                .Where(x => (x.StartDate >= startDate && x.StartDate <= endDate) || (x.EndDate >= startDate && x.EndDate <= endDate) ||
                    (x.StartDate < startDate && x.EndDate == null));
            if (sq != null)
            {
                result = result.PropertyIn(x => x.Id, sq);
            }

            return result.ToList<ClientContract>();
        }

        public IList<ClientContract> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ClientContract>(state, ignoreDeletedRows);
        }

        public IList<ClientContract> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ClientContract>(state, parameterString, ignoreDeletedRows);
        }

        /// <summary>
        /// Получить все сделки, использующие данный договор.
        /// </summary>
        /// <param name="clientContract">Договор с клиентом.</param>
        /// <returns>Все сделки, использующие данный договор.</returns>
        public IEnumerable<Deal> GetDeals(ClientContract clientContract)
        {
            return Query<Deal>().Where(x => x.Contract == clientContract).ToList<Deal>();
        }

        /// <summary>
        /// Получить подзапрос всех сделок, использующих данный договор.
        /// </summary>
        /// <param name="clientContract">Договор с клиентом.</param>
        /// <returns>Подзапрос всех сделок, использующих данный договор.</returns>
        public ISubQuery GetDealsSubQuery(ClientContract clientContract)
        {
            return SubQuery<Deal>().Where(x => x.Contract == clientContract).Select(x => x.Id);
        }

        /// <summary>
        /// Получить подзапрос всех накладных реализации, использующих данный договор.
        /// </summary>
        /// <param name="clientContract">Договор с клиентом.</param>
        /// <returns>Подзапрос всех накладных реализации, использующих данный договор.</returns>
        public ISubQuery GetSalesSubQuery(ClientContract clientContract)
        {
            return SubQuery<SaleWaybill>().PropertyIn(x => x.Deal, GetDealsSubQuery(clientContract)).Select(x => x.Id);
        }

        /// <summary>
        /// Является ли номер договора уникальным в системе.
        /// </summary>
        /// <param name="clientContract">Договор с клиентом.</param>
        /// <returns>true, если номер договора уникален.</returns>
        public bool IsNumberUnique(ClientContract clientContract)
        {
            return Query<ClientContract>().Where(x => x.Number == clientContract.Number && x.Id != clientContract.Id).Count() == 0;
        }

        /// <summary>
        /// Проверка, что договор используется одной и только одной сделкой.
        /// </summary>
        /// <param name="contract">Договор с клиентом.</param>
        /// <param name="deal">Сделка.</param>
        /// <returns>true, если договор используется только этой сделкой.</returns>
        public bool IsUsedBySingleDeal(ClientContract contract, Deal deal)
        {
            var deals = GetDeals(contract);

            return deals.Count() == 1 && deals.All(x => x == deal);
        }

        /// <summary>
        /// Получить список договоров с клиентами по подзапросу сделок (выбираются договора с клиентами из этих сделок)
        /// </summary>
        /// <param name="dealSubQuery">подкритерий сделок</param>
        public IDictionary<short, ClientContract> GetDealClientContractList(ISubQuery dealSubQuery)
        {
            var subQuery = SubQuery<Deal>()
                .PropertyIn(x => x.Id, dealSubQuery)
                .Select(x => x.Contract.Id);

            return Query<ClientContract>()
                .PropertyIn(x => x.Id, subQuery)
                .ToList<ClientContract>()
                .Distinct()
                .ToDictionary(x => x.Id);
        }
    }
}
