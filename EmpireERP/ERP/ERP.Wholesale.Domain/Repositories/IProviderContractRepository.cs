using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using ERP.Infrastructure.Repositories;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IProviderContractRepository : IRepository<ProviderContract, short>, IGetAllRepository<ProviderContract>
    {
        /// <summary>
        /// Есть ли по указанному договору какие-нибудь приходы
        /// </summary>
        bool AnyReceipts(ProviderContract contract);
    }
}
