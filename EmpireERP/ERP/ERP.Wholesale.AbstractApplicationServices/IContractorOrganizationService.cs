using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IContractorOrganizationService
    {
        IEnumerable<ContractorOrganization> GetFilteredList(object state, ParameterString parameterString);
    }
}
