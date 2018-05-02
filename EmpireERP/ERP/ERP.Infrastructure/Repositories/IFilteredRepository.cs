using System.Collections.Generic;
using ERP.Utils;

namespace ERP.Infrastructure.Repositories
{
    public interface IFilteredRepository<T>
    {
        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>        
        IList<T> GetFilteredList(object state, bool ignoreDeletedRows = true);

        IList<T> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true);
      
    }
}
