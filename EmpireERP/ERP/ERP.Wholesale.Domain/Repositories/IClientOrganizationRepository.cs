using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IClientOrganizationRepository : IRepository<ClientOrganization, int>, IFilteredRepository<ClientOrganization>, IGetAllRepository<ClientOrganization>
    {
        /// <summary>
        /// Получение списка организаций клиента по Id
        /// </summary>
        IEnumerable<ClientOrganization> GetList(IEnumerable<int> idList);

        /// <summary>
        /// Получить список организаций клиента по подзапросу сделок (выбираются организации из договоров этих сделок)
        /// </summary>
        IDictionary<int, ClientOrganization> GetDealClientOrganizationList(ISubQuery dealSubQuery);
    }
}
