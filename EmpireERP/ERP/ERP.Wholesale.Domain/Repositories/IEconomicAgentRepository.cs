using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IEconomicAgentRepository
    {
        /// <summary>
        /// Получить список хоз. субъектов по подзапросу сделок (выбираются субъекты из организаций договоров этих сделок)
        /// </summary>
        void GetDealEconomicAgentList(ISubQuery dealSubQuery, out IDictionary<int, JuridicalPerson> juridicalPersonList,
            out IDictionary<int, PhysicalPerson> physicalPersonList);
    }
}
