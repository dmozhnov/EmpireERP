using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface ITaskService
    {
        /// <summary>
        /// Проверка существования задачи
        /// </summary>
        /// <param name="id">Идентификатор задачи</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <returns>Задача</returns>
        Task CheckTaskExistence(int id, User user, string message = "");

        /// <summary>
        /// Сохранение задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <returns>Идентификатор сохраненной задачи</returns>
        int Save(Task task, DateTime currentDate, User user);

        /// <summary>
        /// Удаление задачи
        /// </summary>
        /// <param name="task">Удаляемая задача</param>
        void Delete(Task task, DateTime currentDate, User user);

        /// <summary>
        /// Получение отфильтрованного списка задач
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="parameterString">Параметры грида</param>
        /// <param name="user">Пользователь</param>
        /// <param name="ignoreDeletedRows">Флаг, нужно ли игнорировать удаленные задачи</param>
        /// <returns>Коллекция задач</returns>
        IList<Task> GetFilteredList(object state, ParameterString parameterString, User user, bool ignoreDeletedRows = true);

        ///// <summary>
        ///// Добавление связанной задачи
        ///// </summary>
        ///// <param name="task">Задача</param>
        ///// <param name="relatedTask">Связываемеая задача</param>
        ///// <param name="user">Пользователь</param>
        //void AddRelatedTask(Task task, Task relatedTask, User user);

        ///// <summary>
        ///// Удаление связанной задачи
        ///// </summary>
        ///// <param name="task">Задача</param>
        ///// <param name="relatedTask">Связанная задача</param>
        ///// <param name="user">Пользователь</param> 
        //void RemoveRelatedTask(Task task, Task relatedTask, User user);

        
        /// <summary>
        /// Получение истории изменений
        /// </summary>
        /// <param name="task">Задача</param>
        /// <returns></returns>
        IEnumerable<BaseTaskHistoryItem> GetChangeHistory(Task task);

        #region Права

        /// <summary>
        /// Получение права, которым регламентируется видимость ответственных лиц при их назначении
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        Permission GetPermissionForExecutedByList(User user);

        /// <summary>
        /// Проверка права на создание задачи
        /// </summary>
        /// <param name="user"></param>
        void CheckPossibilityToCreateTask(User user);

        /// <summary>
        /// Проверка права на создание задачи
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Признак разрешения на создание задачи (true - можно создавать)</returns>
        bool IsPossibilityToCreateTask(User user);

        /// <summary>
        /// Проверка права на редактирование задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        void CheckPossibilityToEditTask(Task task, User user);

        /// <summary>
        /// Проверка права на редактирование задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        bool IsPossibilityToEditTask(Task task, User user);

        /// <summary>
        /// Проверка права на удаление задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        void CheckPossibilityToDeleteTask(Task task, User user);

        /// <summary>
        /// Проверка права на удаление задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        bool IsPossibilityToDeleteTask(Task task, User user);

        /// <summary>
        /// Проверка возможности просмотра автора задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        void CheckPossibilityToViewCreatedBy(Task task, User user);

        /// <summary>
        /// Проверка возможности просмотра автора задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        bool IsPossibilityToViewCreatedBy(Task task, User user);

        /// <summary>
        /// Проверка возможности просмотра исполнителя задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        void CheckPossibilityToViewExecutedBy(Task task, User user);

        /// <summary>
        /// Проверка возможности просмотра исполнителя задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        bool IsPossibilityToViewExecutedBy(Task task, User user);

        /// <summary>
        /// Проверка возможности изменения даты исполнения задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        void CheckPossibilityToViewTaskDetails(Task task, User user);

        /// <summary>
        /// Проверка возможности изменения даты исполнения задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        bool IsPossibilityToViewTaskDetails(Task task, User user);

        /// <summary>
        /// Проверка возможности выбора пользователя как исполнителя задачи
        /// </summary>
        /// <param name="executedBy">Проверяемый пользователь</param>
        /// <param name="user">Пользователь</param>
        void CheckPossibilityToSetByExecutedBy(User executedBy, User user);
        
        /// <summary>
        /// Проверка возможности выбора пользователя как исполнителя задачи
        /// </summary>
        /// <param name="executedBy">Проверяемый пользователь</param>
        /// <param name="user">Пользователь</param>
        bool IsPossibilityToSetByExecutedBy(User executedBy, User user);

        #endregion
    }
}
