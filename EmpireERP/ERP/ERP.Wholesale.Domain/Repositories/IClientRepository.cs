using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IClientRepository : IRepository<Client, int>, IFilteredRepository<Client>, IGetAllRepository<Client>
    {
        /// <summary>
        /// Получение списка клиентов по Id
        /// </summary>
        /// <param name="idList">Список кодов клиентов</param>
        IEnumerable<Client> GetList(IEnumerable<int> idList);

        /// <summary>
        /// Получить список клиентов по подзапросу сделок (выбираются клиенты этих сделок)
        /// </summary>
        /// <param name="dealSubQuery">подкритерий сделок</param>
        IDictionary<int, Client> GetDealClientList(ISubQuery dealSubQuery);
    }
}
