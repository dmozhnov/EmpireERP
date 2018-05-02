using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface ITaskExecutionItemService
    {
        /// <summary>
        /// Проверка существования исполнения
        /// </summary>
        /// <param name="id">Идентификатор исполнения</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <returns>Исполнение</returns>
        TaskExecutionItem CheckTaskExecutionItemExistence(int id, string message = "");

        /// <summary>
        /// Сохранение исполнения
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение</param>
        /// <param name="user">Пользователь, сохраняющий исполнение</param>
        /// <returns>Идентификатор сохраненного исполнения</returns>
        int Save(TaskExecutionItem taskExecutionItem, DateTime currentDate, User user);

        /// <summary>
        /// Удаление исполнения
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение</param>
        /// <param name="currentDate">Текущая дата</param>
        /// <param name="user">Пользователь, удаляющий исполнение</param>
        void Delete(TaskExecutionItem taskExecutionItem, DateTime currentDate, User user);

        #region Права

        /// <summary>
        /// Проверка права на создание исполнения задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        void CheckPossibilityToCreateTaskExecution(Task task, User user);

        /// <summary>
        /// Проверка права на создание исполнения задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        bool IsPossibilityToCreateTaskExecution(Task task, User user);

        /// <summary>
        /// Проверка права на редактирование исполнения задачи
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение задачи</param>
        /// <param name="user">Пользователь</param>
        void CheckPossibilityToEditTaskExecution(TaskExecutionItem taskExecutionItem, User user);

        /// <summary>
        /// Проверка права на редактирование исполнения задачи
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение задачи</param>
        /// <param name="user">Пользователь</param>
        bool IsPossibilityToEditTaskExecution(TaskExecutionItem taskExecutionItem, User user);

        /// <summary>
        /// Проверка права на удаление исполнения задачи
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение задачи</param>
        /// <param name="user">Пользователь</param>
        void CheckPossibilityToDeleteTaskExecution(TaskExecutionItem taskExecutionItem, User user);

        /// <summary>
        /// Проверка права на удаление исполнения задачи
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение задачи</param>
        /// <param name="user">Пользователь</param>
        bool IsPossibilityToDeleteTaskExecution(TaskExecutionItem taskExecutionItem, User user);

        /// <summary>
        /// Проверка возможности изменения даты исполнения задачи
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение. Если NULL, то в качестве автора исполнения используется текущий пользователь.</param>
        /// <param name="user">Пользователь</param>
        void CheckPossibilityToEditExecutionDate(TaskExecutionItem taskExecutionItem, User user);

        /// <summary>
        /// Проверка возможности изменения даты исполнения задачи
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение. Если NULL, то в качестве автора исполнения используется текущий пользователь.</param>
        /// <param name="user">Пользователь</param>
        bool IsPossibilityToEditExecutionDate(TaskExecutionItem taskExecutionItem, User user);

        #endregion
    }
}
