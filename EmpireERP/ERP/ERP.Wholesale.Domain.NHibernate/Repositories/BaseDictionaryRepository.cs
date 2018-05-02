using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    /// <summary>
    /// Базовый репозиторий для словарей
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseDictionaryRepository<T> : BaseRepository, IBaseDictionaryRepository<T> where T : BaseDictionary
    {
        public BaseDictionaryRepository()
            : base()
        {
        }

        public T GetById(short id)
        {
            return CurrentSession.Get<T>(id);
        }

        public void Save(T entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(T entity)
        {
            CurrentSession.Delete(entity);
        }

        public IEnumerable<T> GetAll()
        {
            return Query<T>().OrderByAsc(x => x.Name).ToList<T>();            
        }

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        public IList<T> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<T>(state, ignoreDeletedRows);
        }

        public IList<T> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<T>(state, parameterString, ignoreDeletedRows);
        }

    }
}
