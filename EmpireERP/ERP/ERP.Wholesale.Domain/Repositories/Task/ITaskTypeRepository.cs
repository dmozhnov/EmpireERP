using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface ITaskTypeRepository : IRepository<TaskType, short>, IGetAllRepository<TaskType>
    {
        /// <summary>
        /// Получение типа задачи по статусу исполнения
        /// </summary>
        /// <param name="stateId">Идентификатор статуса исполнения</param>
        /// <returns></returns>
        TaskType GetTaskTypeByExecutionState(short stateId);
    }
}
