using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface ITaskPriorityService
    {
        /// <summary>
        /// Проверка существования приоритета задачи
        /// </summary>
        /// <param name="id">Идентификатор приоритета задачи</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <returns>Приоритет задачи</returns>
        TaskPriority CheckTaskPriorityExistence(short id, string message = "");

        /// <summary>
        /// Получение всех приоритетов задачи
        /// </summary>
        /// <returns>Коллекция приоритетов задачи</returns>
        IEnumerable<TaskPriority> GetAll();
    }
}
