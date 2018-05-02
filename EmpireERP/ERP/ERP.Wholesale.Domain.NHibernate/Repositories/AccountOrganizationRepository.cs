using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class AccountOrganizationRepository : BaseRepository, IAccountOrganizationRepository
    {
        public AccountOrganizationRepository()
            : base()
        {
        }

        public AccountOrganization GetById(int id)
        {
            return CurrentSession.Get<AccountOrganization>(id);
        }

        public void Save(AccountOrganization Value)
        {
            CurrentSession.SaveOrUpdate(Value);
        }

        public void Delete(AccountOrganization Value)
        {
            CurrentSession.SaveOrUpdate(Value);
        }

        public IEnumerable<AccountOrganization> GetAll()
        {
            return CurrentSession.CreateCriteria(typeof(AccountOrganization)).List<AccountOrganization>();
        }

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        public IList<AccountOrganization> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<AccountOrganization>(state, ignoreDeletedRows);
        }
        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        public IList<AccountOrganization> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<AccountOrganization>(state, parameterString, ignoreDeletedRows);
        }

        /// <summary>
        /// Получить список собственных организаций по подзапросу сделок (выбираются организации из договоров этих сделок)
        /// </summary>
        /// <param name="dealSubQuery">Подзапрос сделок</param>
        public IDictionary<int, AccountOrganization> GetDealAccountOrganizationList(ISubQuery dealSubQuery)
        {
            var contractSubQuery = SubQuery<Deal>()
                .PropertyIn(x => x.Id, dealSubQuery)
                .Select(x => x.Contract.Id);

            var accountOrganizationSubQuery = SubQuery<ClientContract>()
                .PropertyIn(x => x.Id, contractSubQuery)
                .Select(x => x.AccountOrganization.Id);

            return Query<AccountOrganization>()
                .PropertyIn(x => x.Id, accountOrganizationSubQuery)
                .ToList<AccountOrganization>()
                .Distinct()
                .ToDictionary(x => x.Id);
        }
    }
}
