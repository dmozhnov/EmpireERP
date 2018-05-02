using ERP.Infrastructure.Repositories;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IBaseDictionaryRepository<T> : IRepository<T, short>, IGetAllRepository<T>, IFilteredRepository<T> where T : BaseDictionary
    {
    }
}
