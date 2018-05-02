using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;

namespace ERP.Infrastructure.Repositories
{
    /// <summary>
    /// Интерфейс репозитория
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    /// <typeparam name="IdT">Тип идентификатора сущности</typeparam>
    public interface IRepository<T, IdT>
    {
        /// <summary>
        /// Получение сущности по Id
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность</returns>
        T GetById(IdT id);

        /// <summary>
        /// Сохранение сущности
        /// </summary>
        /// <param name="entity">Сущность</param>
        void Save(T entity);

        /// <summary>
        /// Удаление записи
        /// </summary>
        /// <param name="entity">Сущность</param>
        void Delete(T entity);

        /// <summary>
        /// Выполнение запроса по критериям
        /// </summary>
        /// <typeparam name="T">Тип запрашиваемых данных</typeparam>
        /// <returns></returns>
        ICriteria<TResult> Query<TResult>(bool ignoreDeletedRows = true, string alias = "") where TResult : class;

        /// <summary>
        /// Подзапрос
        /// </summary>
        ISubCriteria<TResult> SubQuery<TResult>(bool ignoreDeletedRows = true) where TResult : class;
    }
}