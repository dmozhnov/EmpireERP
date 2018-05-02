using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IContractorService
    {
        Contractor CheckContractorExistence(int id, string message = "");
        IEnumerable<Contractor> GetContractorByUser(object state, User user);
    }
}
