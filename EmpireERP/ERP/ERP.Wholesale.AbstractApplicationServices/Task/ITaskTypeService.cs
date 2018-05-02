using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface ITaskTypeService
    {
        /// <summary>
        /// Проверка существования типа задачи
        /// </summary>
        /// <param name="id">Идентификатор типа задачи</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <returns>Тип задачи</returns>
        TaskType CheckTaskTypeExistence(short id, string message = "");

        /// <summary>
        /// Получение всех типов задач
        /// </summary>
        /// <returns>Коллекция типов задач</returns>
        IEnumerable<TaskType> GetAll();

        /// <summary>
        /// Получение типа задачи по статусу исполнения
        /// </summary>
        /// <param name="state">Статус исполнения</param>
        /// <returns></returns>
        TaskType GetTaskTypeByExecutionState(TaskExecutionState state);
    }
}
