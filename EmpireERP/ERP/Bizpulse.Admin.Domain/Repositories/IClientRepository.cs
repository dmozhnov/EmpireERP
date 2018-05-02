using Bizpulse.Admin.Domain.Entities;
using ERP.Infrastructure.Repositories;
using System.Collections.Generic;

namespace Bizpulse.Admin.Domain.Repositories
{
    public interface IClientRepository : IRepository<Client, int>
    {
        IList<Client> GetFilteredList(object state, bool ignoreDeletedRows = true);
    }
}
