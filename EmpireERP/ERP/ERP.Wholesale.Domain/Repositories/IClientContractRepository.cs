using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IClientContractRepository : IRepository<ClientContract, short>, IFilteredRepository<ClientContract>
    {
        /// <summary>
        /// Получить все сделки, использующие данный договор.
        /// </summary>
        /// <param name="clientContract">Договор с клиентом.</param>
        /// <returns>Все сделки, использующие данный договор.</returns>
        IEnumerable<Deal> GetDeals(ClientContract clientContract);
        
        /// <summary>
        /// Получение договоров по подзапросу сделок, которые активны в указанный период (есть пересечение временных интервалов)
        /// </summary>
        /// <param name="subQuery">Подзапрос сделок</param>
        /// <returns></returns>
        IEnumerable<ClientContract> GetList(DateTime startDate, DateTime endDate, ISubCriteria<Deal> subQuery);

        /// <summary>
        /// Получить подзапрос всех сделок, использующих данный договор.
        /// </summary>
        /// <param name="clientContract">Договор с клиентом.</param>
        /// <returns>Подзапрос всех сделок, использующих данный договор.</returns>
        ISubQuery GetDealsSubQuery(ClientContract clientContract);

        /// <summary>
        /// Получить подзапрос всех накладных реализации, использующих данный договор.
        /// </summary>
        /// <param name="clientContract">Договор с клиентом.</param>
        /// <returns>Подзапрос всех накладных реализации, использующих данный договор.</returns>
        ISubQuery GetSalesSubQuery(ClientContract clientContract);

        /// <summary>
        /// Является ли номер договора уникальным в системе.
        /// </summary>
        /// <param name="clientContract">Договор с клиентом.</param>
        /// <returns>true, если номер договора уникален.</returns>
        bool IsNumberUnique(ClientContract clientContract);

        /// <summary>
        /// Проверка, что договор используется одной и только одной сделкой.
        /// </summary>
        /// <param name="contract">Договор с клиентом.</param>
        /// <param name="deal">Сделка.</param>
        /// <returns>true, если договор используется только этой сделкой.</returns>
        bool IsUsedBySingleDeal(ClientContract contract, Deal deal);

        /// <summary>
        /// Получить список договоров с клиентами по подзапросу сделок (выбираются договора с клиентами из этих сделок)
        /// </summary>
        /// <param name="dealSubQuery">подкритерий сделок</param>
        IDictionary<short, ClientContract> GetDealClientContractList(ISubQuery dealSubQuery);
    }
}
