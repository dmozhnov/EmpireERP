using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Infrastructure.NHibernate.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    /// <summary>
    /// Базовый класс для репозиториев
    /// </summary>
    public class BaseNHRepository : BaseRepository
    {
        /// <summary>
        /// Получение списка сущностей по Id
        /// </summary>        
        public Dictionary<TId, T> GetList<TId, T>(IEnumerable<TId> idList) where T : Entity<TId>
        {
            var result = new Dictionary<TId, T>();
            var listToLoad = new List<TId>();

            for (int i = 1; i <= idList.Count(); i++)
            {
                listToLoad.Add(idList.ElementAt(i - 1));

                // делаем выборку 100 строк
                if (i % 100 == 0)
                {
                    result = result.Concat(Query<T>().OneOf(x => x.Id, listToLoad).ToList<T>().ToDictionary(x => x.Id, x => x)).ToDictionary(x => x.Key, x => x.Value);                    

                    listToLoad.Clear();
                }
            }

            // добавляем оставшиеся
            result = result.Concat(Query<T>().OneOf(x => x.Id, listToLoad).ToList<T>().ToDictionary(x => x.Id, x => x)).ToDictionary(x => x.Key, x => x.Value);

            return result;
        }

        
    }
}
