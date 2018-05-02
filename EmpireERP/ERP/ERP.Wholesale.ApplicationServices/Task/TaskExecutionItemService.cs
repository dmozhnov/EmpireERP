using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class TaskExecutionItemService : ITaskExecutionItemService
    {
        #region Поля

        private readonly ITaskExecutionItemRepository taskExecutionItemRepository;

        #endregion
         
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="taskExecutionItemRepository"></param>
        public TaskExecutionItemService(ITaskExecutionItemRepository taskExecutionItemRepository)
        {
            this.taskExecutionItemRepository = taskExecutionItemRepository;
        }

        #endregion

        #region Методы
         
        #region Основные операции
        
        /// <summary>
        /// Проверка существования исполнения
        /// </summary>
        /// <param name="id">Идентификатор исполнения</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <returns>Исполнение</returns>
        public TaskExecutionItem CheckTaskExecutionItemExistence(int id, string message = "")
        {
            var val = taskExecutionItemRepository.GetById(id);
            ValidationUtils.NotNull(val, String.IsNullOrEmpty(message) ? "Исполнение не найдено. Возможно, оно было удалено." : message);

            return val;
        }

        /// <summary>
        /// Сохранение исполнения
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение</param>
        /// <param name="user">Пользователь, сохраняющий исполнение</param>
        /// <returns>Идентификатор сохраненного исполнения</returns>
        public int Save(TaskExecutionItem taskExecutionItem, DateTime currentDate, User user)
        {
            if (taskExecutionItem.Id == 0)  // Если исполнение сохраняется, то
            {
                if (taskExecutionItem.IsCreatedByUser)  //Если исполнение создано пользователем, то
                {
                    // проверяем право на создание
                    CheckPossibilityToCreateTaskExecution(taskExecutionItem.Task, user);
                }// иначе право на редактирование задачи уже проверено презентером
            }
            else
            {
                // Иначе на редактирование
                CheckPossibilityToEditTaskExecution(taskExecutionItem, user);
            }

            taskExecutionItemRepository.Save(taskExecutionItem);
            taskExecutionItem.SaveHistory(currentDate, user);   //Сохраняем историю

            return taskExecutionItem.Id;
        }

        /// <summary>
        /// Удаление исполнения
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение</param>
        /// <param name="currentDate">Текущая дата</param>
        /// <param name="user">Пользователь, удаляющий исполнение</param>
        public void Delete(TaskExecutionItem taskExecutionItem, DateTime currentDate, User user)
        {
            taskExecutionItem.DeletionDate = currentDate;
            taskExecutionItemRepository.Delete(taskExecutionItem);
            taskExecutionItem.SaveHistory(currentDate, user);    //Сохраняем историю изменений исполнения
        }

        
        #endregion
        
        #region Права

        /// <summary>
        /// Проверка разрешения
        /// </summary>
        /// <param name="checkingUser">Проверяемый пользователь</param>
        /// <param name="user">Пользователь, совершающий операцию</param>
        /// <param name="permission">Разрешение</param>
        /// <returns></returns>
        private bool IsPermissionToPerformOperation(User checkingUser, User user, Permission permission)
        {
            bool result = false;
            var distribution = user.GetPermissionDistributionType(permission);

            switch (distribution)
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                    result = checkingUser == user;
                    break;

                case PermissionDistributionType.Teams:
                    result = user.Teams.SelectMany(x => x.Users).Contains(checkingUser);
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Проверка разрешения
        /// </summary>
        /// <param name="checkingUser">Проверяемый пользователь</param>
        /// <param name="user">Пользователь, совершающий операцию</param>
        /// <param name="permission">Разрешение</param>
        /// <returns></returns>
        private void CheckPermissionToPerformOperation(User checkingUser, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(checkingUser, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        /// <summary>
        /// Проверка права на создание исполнения задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        public void CheckPossibilityToCreateTaskExecution(Task task, User user)
        {
            CheckPermissionToPerformOperation(task.ExecutedBy, user, Permission.TaskExecutionItem_Create);
        }

        /// <summary>
        /// Проверка права на создание исполнения задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        public bool IsPossibilityToCreateTaskExecution(Task task, User user)
        {
            try
            {
                CheckPossibilityToCreateTaskExecution(task, user);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка задачи на возможность редактирование/удаления исполнений
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        private void CheckTaskOnPossibilityToEditTaskExecution(Task task, User user)
        {
            CheckPermissionToPerformOperation(task.CreatedBy, user, Permission.Task_TaskExecutionItem_Edit_Delete);
        }

        /// <summary>
        /// Проверка права на редактирование исполнения задачи
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение задачи</param>
        /// <param name="user">Пользователь</param>
        public void CheckPossibilityToEditTaskExecution(TaskExecutionItem taskExecutionItem, User user)
        {
            CheckTaskOnPossibilityToEditTaskExecution(taskExecutionItem.Task, user);
            CheckPermissionToPerformOperation(taskExecutionItem.CreatedBy, user, Permission.TaskExecutionItem_Edit);
        }

        /// <summary>
        /// Проверка права на редактирование исполнения задачи
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение задачи</param>
        /// <param name="user">Пользователь</param>
        public bool IsPossibilityToEditTaskExecution(TaskExecutionItem taskExecutionItem, User user)
        {
            try
            {
                CheckPossibilityToEditTaskExecution(taskExecutionItem, user);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка права на удаление исполнения задачи
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение задачи</param>
        /// <param name="user">Пользователь</param>
        public void CheckPossibilityToDeleteTaskExecution(TaskExecutionItem taskExecutionItem, User user)
        {
            CheckTaskOnPossibilityToEditTaskExecution(taskExecutionItem.Task, user);
            CheckPermissionToPerformOperation(taskExecutionItem.CreatedBy, user, Permission.TaskExecutionItem_Delete);
        }

        /// <summary>
        /// Проверка права на удаление исполнения задачи
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение задачи</param>
        /// <param name="user">Пользователь</param>
        public bool IsPossibilityToDeleteTaskExecution(TaskExecutionItem taskExecutionItem, User user)
        {
            try
            {
                CheckPossibilityToDeleteTaskExecution(taskExecutionItem, user);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка возможности изменения даты исполнения задачи
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение. Если NULL, то в качестве автора исполнения используется текущий пользователь.</param>
        /// <param name="user">Пользователь</param>
        public void CheckPossibilityToEditExecutionDate(TaskExecutionItem taskExecutionItem, User user)
        {
            CheckPermissionToPerformOperation(taskExecutionItem != null ? taskExecutionItem.CreatedBy : user, user, Permission.TaskExecutionItem_EditExecutionDate);
        }

        /// <summary>
        /// Проверка возможности изменения даты исполнения задачи
        /// </summary>
        /// <param name="taskExecutionItem">Исполнение. Если NULL, то в качестве автора исполнения используется текущий пользователь.</param>
        /// <param name="user">Пользователь</param>
        public bool IsPossibilityToEditExecutionDate(TaskExecutionItem taskExecutionItem, User user)
        {
            try
            {
                CheckPossibilityToEditExecutionDate(taskExecutionItem, user);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #endregion
    }
}
