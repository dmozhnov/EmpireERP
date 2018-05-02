using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class TaskTypeRepository : BaseNHRepository, ITaskTypeRepository
    {
        public TaskType GetById(short id)
        {
            return CurrentSession.Get<TaskType>(id);
        }


        public void Save(TaskType entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(TaskType entity)
        {
            //реализован из-за интерфейса. Логика метода пока не известна.
            throw new NotImplementedException();
        }

        public IEnumerable<TaskType> GetAll()
        {
            return CurrentSession.Query<TaskType>().ToList();
        }

        /// <summary>
        /// Получение типа задачи по статусу исполнения
        /// </summary>
        /// <param name="stateId">Идентификатор статуса исполнения</param>
        /// <returns></returns>
        public TaskType GetTaskTypeByExecutionState(short stateId)
        {
            return CurrentSession.Query<TaskType>()
                .Where(x => x.States.Any(y => y.Id == stateId))
                .FirstOrDefault();
        }
    }
}
