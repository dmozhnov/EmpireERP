using ERP.Infrastructure.Repositories;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IRoleRepository : IRepository<Role, short>, IFilteredRepository<Role>, IGetAllRepository<Role>
    {
    }
}
