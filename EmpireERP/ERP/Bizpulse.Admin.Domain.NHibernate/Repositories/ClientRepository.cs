using System.Collections.Generic;
using Bizpulse.Admin.Domain.Entities;
using Bizpulse.Admin.Domain.Repositories;

namespace Bizpulse.Admin.Domain.NHibernate.Repositories
{
    public class ClientRepository : BaseAdminRepository<Client, int>, IClientRepository
    {
        public IList<Client> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Client>(state, ignoreDeletedRows);
        }
    }
}
