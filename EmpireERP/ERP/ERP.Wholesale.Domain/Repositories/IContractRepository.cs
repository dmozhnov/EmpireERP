using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IContractRepository
    {
        /// <summary>
        /// Есть ли договора, связанные с данной собственной организацией
        /// </summary>
        bool AnyContracts(AccountOrganization accountOrganization);
    }
}
