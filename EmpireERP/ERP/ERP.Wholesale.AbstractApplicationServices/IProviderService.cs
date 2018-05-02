using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IProviderService
    {        
        Provider CheckProviderExistence(int id, string message = "");
        void Delete(Provider provider, User user);

        IEnumerable<Provider> GetList(User user);
        IList<Provider> GetFilteredList(object state, bool ignoreDeletedRows = true);
                
        void Save(Provider provider);
        void AddContractorOrganization(Provider provider, ProviderOrganization providerOrganization);
        void RemoveProviderOrganization(Provider provider, ProviderOrganization providerOrganization);
        void AddProviderContract(Provider provider, ProviderContract providerContract);
        void DeleteProviderContract(Provider provider, ProviderContract providerContract, User user);               

        bool IsPossibilityToEditOrganization(Provider provider, ProviderContract contract, User user);
        bool IsPossibilityToDeleteContract(Provider provider, ProviderContract contract, User user);

        void CheckPossibilityToEditOrganization(Provider provider, ProviderContract contract, User user);
        void CheckPossibilityToDeleteContract(Provider provider, ProviderContract providerContract, User user);
    }
}
