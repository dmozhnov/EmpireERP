using System.Collections.Generic;
using System.Linq;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class TaskPriorityRepository : BaseNHRepository, ITaskPriorityRepository
    {
        /// <summary>
        /// Получение приоритета по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Приоритет с указанным идентификатором</returns>
        public TaskPriority GetById(short id)
        {
            return CurrentSession.Get<TaskPriority>(id);
        }

        /// <summary>
        /// Сохранение приоритета
        /// </summary>
        /// <param name="entity">Сохраняемый приоритет</param>
        public void Save(TaskPriority entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Удаление приоритета
        /// </summary>
        /// <param name="entity">Удаляемый приоритет</param>
        public void Delete(TaskPriority entity)
        {
            CurrentSession.Delete(entity);
        }

        /// <summary>
        /// Получение перечня всех имеющихся приоритетов
        /// </summary>
        /// <returns>Коллекция приоритетов</returns>
        public IEnumerable<TaskPriority> GetAll()
        {
            return CurrentSession.Query<TaskPriority>().ToList();
        }
    }
}
