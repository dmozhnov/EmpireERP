using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using ERP.Infrastructure.Repositories;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IContractorRepository : IRepository<Contractor, int>, IFilteredRepository<Contractor>
    {
    }
}
