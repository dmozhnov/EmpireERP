using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class TaskService : ITaskService
    {
        #region Поля

        private readonly ITaskRepository taskRepository;
        private readonly ITaskExecutionItemRepository taskExecutionItemRepository;

        #endregion

        #region Конструкторы

        public TaskService(ITaskRepository taskRepository, ITaskExecutionItemRepository taskExecutionItemRepository)
        {
            this.taskRepository = taskRepository;
            this.taskExecutionItemRepository = taskExecutionItemRepository;
        }

        #endregion

        #region Методы

        #region Основные операции
        
        /// <summary>
        /// Проверка существования задачи
        /// </summary>
        /// <param name="id">Идентификатор задачи</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <returns>Задача</returns>
        public Task CheckTaskExistence(int id, User user, string message = "")
        {
            var val = taskRepository.GetById(id);
            if (val != null)
            {
                // Проверка прав на просмотр задачи
                if (!IsPossibilityToViewTaskDetails(val, user))
                {
                    val = null;
                }
            }
            ValidationUtils.NotNull(val, String.IsNullOrEmpty(message) ? "Задача не найдена. Возможно, она была удалена." : message);

            return val;
        }

        /// <summary>
        /// Сохранение задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <returns>Идентификатор сохраненной задачи</returns>
        public int Save(Task task, DateTime currentDate, User user)
        {
            ValidationUtils.NotNull(task, "Необходимо указать задачу.");
            // Проверка прав на сохранение/редактирование
            if (task.Id == 0)
            {
                CheckPossibilityToCreateTask(user);
            }
            else
            {
                CheckPossibilityToEditTask(task, user); 
            }

            taskRepository.Save(task);
            task.SaveHistory(currentDate, user);    // Сохраняем историю изменений для задачи

            return task.Id;
        }

        /// <summary>
        /// Удаление задачи
        /// </summary>
        /// <param name="task">Удаляемая задача</param>
        public void Delete(Task task, DateTime currentDate, User user)
        {
            ValidationUtils.NotNull(task, "Необходимо указать задачу.");
            CheckPossibilityToDeleteTask(task, user);   // Проверка прав на удаление

            task.DeletionDate = currentDate;
            // Сбрасываем связи задачи
            task.Contractor = null;
            task.Deal = null;
            task.ProductionOrder = null;

            taskRepository.Delete(task);

            task.SaveHistory(currentDate, user);
        }

        /// <summary>
        /// Получение отфильтрованного списка задач
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="parameterString">Параметры грида</param>
        /// <param name="user">Пользователь</param>
        /// <param name="ignoreDeletedRows">Флаг, нужно ли игнорировать удаленные задачи</param>
        /// <returns>Коллекция задач</returns>
        public IList<Task> GetFilteredList(object state, ParameterString parameterString, User user,bool ignoreDeletedRows = true)
        {
            // Права на просмотр
            var r1 = RestrictOnUserByPermission("CreatedBy", user, Permission.Task_CreatedBy_List_Details);
            var r2 = RestrictOnUserByPermission("ExecutedBy", user, Permission.Task_ExecutedBy_List_Details);
            
            parameterString.Add(ParameterStringItem.OperationType.Or, r1, r2);

            return taskRepository.GetFilteredList(state, parameterString, ignoreDeletedRows);
        }

        /// <summary>
        /// Ограничение выборки задач по пользователям
        /// </summary>
        /// <param name="ps">Параметры фильтрации</param>
        /// <param name="key">имя поля</param>
        /// <param name="user">Пользователь</param>
        /// <param name="permission">Право, по корому ограничивается выборка</param>
        private ParameterStringItem RestrictOnUserByPermission(string key, User user, Permission permission)
        {
            ParameterStringItem result = null;
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.All:
                    result = new ParameterStringItem("Id", ParameterStringItem.OperationType.NotEq, "0");   // Заведомо истинное условие
                    break;

                case PermissionDistributionType.None:
                    result = new ParameterStringItem(key, ParameterStringItem.OperationType.Eq, "0");
                    break;

                case PermissionDistributionType.Personal:
                    result = new ParameterStringItem(key, ParameterStringItem.OperationType.Eq, user.Id.ToString());
                    break;

                case PermissionDistributionType.Teams:
                    var list = user.Teams.SelectMany(x => x.Users).Select(x => x.Id.ToString());
                    if (list.Count() > 0)
                    {
                        result = new ParameterStringItem(key, ParameterStringItem.OperationType.OneOf, list);
                    }
                    else
                    {
                        result = new ParameterStringItem(key, ParameterStringItem.OperationType.Eq, user.Id.ToString());
                    }
                    break;

                default:
                    throw new Exception(String.Format("Неизвестное распространение права «{0}».", permission.GetDisplayName()));
            }

            return result;
        }

        /// <summary>
        /// Получение истории изменений
        /// </summary>
        /// <param name="task">Задача</param>
        /// <returns></returns>
        public IEnumerable<BaseTaskHistoryItem> GetChangeHistory(Task task)
        {
            return taskRepository.GetChangeHistory(task.Id);
        }

        #endregion

        #region Права

        /// <summary>
        /// Выбор права с наименьшим разрешением
        /// </summary>
        /// <param name="user">Пользователь, для которого определяется разрешение по правам</param>
        /// <param name="first">Первое право</param>
        /// <param name="second">Второе право</param>
        /// <returns></returns>
        private Permission GetMinimalPermission(User user, Permission first, Permission second)
        {
            var pd1 = user.GetPermissionDistributionType(first);
            var pd2 = user.GetPermissionDistributionType(second);
            
            return pd1 > pd2 ? second : first;
        }

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
        /// Получение права, которым регламентируется видимость ответственных лиц при их назначении
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        public Permission GetPermissionForExecutedByList(User user)
        {
            return GetMinimalPermission(user, Permission.Task_Create, Permission.User_List_Details);
        }

        /// <summary>
        /// Проверка права на создание задачи
        /// </summary>
        /// <param name="user"></param>
        public void CheckPossibilityToCreateTask(User user)
        {
            CheckPermissionToPerformOperation(user, user, Permission.Task_Create);
        }

        /// <summary>
        /// Проверка права на создание задачи
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Признак разрешения на создание задачи (true - можно создавать)</returns>
        public bool IsPossibilityToCreateTask(User user)
        {
            try
            {
                CheckPossibilityToCreateTask(user);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка права на редактирование задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        public void CheckPossibilityToEditTask(Task task, User user)
        {
            CheckPermissionToPerformOperation(task.CreatedBy, user, Permission.Task_Edit);
        }

        /// <summary>
        /// Проверка права на редактирование задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        public bool IsPossibilityToEditTask(Task task, User user)
        {
            try
            {
                CheckPossibilityToEditTask(task, user);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка права на удаление задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        public void CheckPossibilityToDeleteTask(Task task, User user)
        {
            CheckPermissionToPerformOperation(task.CreatedBy, user, Permission.Task_Delete);
        }

        /// <summary>
        /// Проверка права на удаление задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        public bool IsPossibilityToDeleteTask(Task task, User user)
        {
            try
            {
                CheckPossibilityToDeleteTask(task, user);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка возможности просмотра автора задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        public void CheckPossibilityToViewCreatedBy(Task task, User user)
        {
            CheckPermissionToPerformOperation(task.CreatedBy, user, GetMinimalPermission(user, Permission.Task_CreatedBy_List_Details, Permission.User_List_Details));
        }

        /// <summary>
        /// Проверка возможности просмотра автора задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        public bool IsPossibilityToViewCreatedBy(Task task, User user)
        {
            try
            {
                CheckPossibilityToViewCreatedBy(task, user);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка возможности просмотра исполнителя задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        public void CheckPossibilityToViewExecutedBy(Task task, User user)
        {
            CheckPermissionToPerformOperation(task.ExecutedBy, user, GetMinimalPermission(user, Permission.Task_ExecutedBy_List_Details, Permission.User_List_Details));
        }

        /// <summary>
        /// Проверка возможности просмотра исполнителя задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        public bool IsPossibilityToViewExecutedBy(Task task, User user)
        {
            try
            {
                CheckPossibilityToViewExecutedBy(task, user);

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
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        public void CheckPossibilityToViewTaskDetails(Task task, User user)
        {
            bool canViewExecutedBy = true, canViewCreatedBy = true;
            try
            {
                CheckPermissionToPerformOperation(task.ExecutedBy, user, Permission.Task_ExecutedBy_List_Details);
            }
            catch (Exception)
            {
                canViewExecutedBy = false;
            }
            try
            {
            CheckPermissionToPerformOperation(task.CreatedBy, user, Permission.Task_CreatedBy_List_Details);
            }
            catch (Exception)
            {
                canViewCreatedBy = false;
            }
            if (!canViewExecutedBy && !canViewCreatedBy)
            {
                throw new Exception("Недостаточно прав для выполнения операции «Просмотр списка и деталей задачи».");
            }
        }

        /// <summary>
        /// Проверка возможности изменения даты исполнения задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="user">Пользователь</param>
        public bool IsPossibilityToViewTaskDetails(Task task, User user)
        {
            try
            {
                CheckPossibilityToViewTaskDetails(task, user);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка возможности выбора пользователя как исполнителя задачи
        /// </summary>
        /// <param name="executedBy">Проверяемый пользователь</param>
        /// <param name="user">Пользователь</param>
        public void CheckPossibilityToSetByExecutedBy(User executedBy, User user)
        {
            CheckPermissionToPerformOperation(executedBy, user, Permission.Task_Create);
        }

        /// <summary>
        /// Проверка возможности выбора пользователя как исполнителя задачи
        /// </summary>
        /// <param name="executedBy">Проверяемый пользователь</param>
        /// <param name="user">Пользователь</param>
        public bool IsPossibilityToSetByExecutedBy(User executedBy, User user)
        {
            try
            {
                CheckPossibilityToSetByExecutedBy(executedBy, user);

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
