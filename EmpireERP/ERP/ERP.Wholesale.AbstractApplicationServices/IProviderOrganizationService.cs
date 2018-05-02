using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IProviderOrganizationService
    {
        void Save(ProviderOrganization providerOrganization);

        ProviderOrganization CheckProviderOrganizationExistence(int id, string message = "");
        IEnumerable<ProviderOrganization> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true);

        void Delete(ProviderOrganization providerOrganization, User user);
        void DeleteRussianBankAccount(ProviderOrganization providerOrganization, RussianBankAccount bankAccount);
        void DeleteForeignBankAccount(ProviderOrganization providerOrganization, ForeignBankAccount bankAccount);
    }
}
