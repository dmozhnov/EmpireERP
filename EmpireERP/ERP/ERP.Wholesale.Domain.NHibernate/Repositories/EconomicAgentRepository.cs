using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Infrastructure.NHibernate.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class EconomicAgentRepository : BaseRepository, IEconomicAgentRepository
    {
        /// <summary>
        /// Получить список хоз. субъектов по подзапросу сделок (выбираются субъекты из организаций договоров этих сделок)
        /// </summary>
        public void GetDealEconomicAgentList(ISubQuery dealSubQuery, out IDictionary<int, JuridicalPerson> juridicalPersonList,
            out IDictionary<int, PhysicalPerson> physicalPersonList)
        {
            var contractSubQuery = SubQuery<Deal>()
                .PropertyIn(x => x.Id, dealSubQuery)
                .Select(x => x.Contract.Id);

            var accountOrganizationSubQuery = SubQuery<ClientContract>()
                .PropertyIn(x => x.Id, contractSubQuery)
                .Select(x => x.AccountOrganization.Id);

            var economicAgentSubQuery = SubQuery<AccountOrganization>()
                .PropertyIn(x => x.Id, accountOrganizationSubQuery)
                .Select(x => x.EconomicAgent.Id);

            juridicalPersonList = Query<JuridicalPerson>()
                .PropertyIn(x => x.Id, economicAgentSubQuery)
                .ToList<JuridicalPerson>()
                .Distinct()
                .ToDictionary(x => x.Id);

            physicalPersonList = Query<PhysicalPerson>()
                .PropertyIn(x => x.Id, economicAgentSubQuery)
                .ToList<PhysicalPerson>()
                .Distinct()
                .ToDictionary(x => x.Id);
        }
    }
}
