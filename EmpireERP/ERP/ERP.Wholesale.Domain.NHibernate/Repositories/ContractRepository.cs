using System.Linq;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ContractRepository : BaseRepository, IContractRepository
    {
        /// <summary>
        /// Есть ли договора, связанные с данной собственной организацией
        /// </summary>
        public bool AnyContracts(AccountOrganization accountOrganization)
        {
            return CurrentSession.Query<Contract>().Any(x => x.AccountOrganization == accountOrganization);
        }
    }
}
