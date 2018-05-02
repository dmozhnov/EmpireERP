using System.Collections.Generic;

namespace ERP.Infrastructure.Repositories
{
    public interface IGetAllRepository<T>
    {
        /// <summary>
        /// Получение списка сущностей
        /// </summary>
        /// <returns>Список сущностей</returns>
        IEnumerable<T> GetAll();        
    }
}
