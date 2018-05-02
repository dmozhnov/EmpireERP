using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IProviderContractService
    {
        IEnumerable<ProviderContract> GetList();
        void Save(ProviderContract providerContract);
        ProviderContract CheckProviderContractExistence(short id);

        /*IEnumerable<ProviderContract> GetListForReceiptWaybillCreation(Provider provider, DateTime date);
        IEnumerable<ProviderContract> GetListForReceiptWaybillCreation(Provider provider, AccountOrganization accountOrganization, DateTime date);*/
    }
}
