using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IAccountOrganizationRepository : IRepository<AccountOrganization, int>, IFilteredRepository<AccountOrganization>, IGetAllRepository<AccountOrganization>
    {
        /// <summary>
        /// Получить список собственных организаций по подзапросу сделок (выбираются организации из договоров этих сделок)
        /// </summary>
        IDictionary<int, AccountOrganization> GetDealAccountOrganizationList(ISubQuery dealSubQuery);
    }
}
