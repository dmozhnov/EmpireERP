using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class TaskPriorityService : ITaskPriorityService
    {
        #region Поля

        private readonly ITaskPriorityRepository taskPriorityRepository;

        #endregion 

        #region Конструкторы

        public TaskPriorityService(ITaskPriorityRepository taskPriorityRepository)
        {
            this.taskPriorityRepository = taskPriorityRepository;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Проверка существования приоритета задачи
        /// </summary>
        /// <param name="id">Идентификатор приоритета задачи</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <returns>Приоритет задачи</returns>
        public TaskPriority CheckTaskPriorityExistence(short id, string message="")
        {
            var val = taskPriorityRepository.GetById(id);
            ValidationUtils.NotNull(val, String.IsNullOrEmpty(message) ? "Пироритет задачи не найден. Возможно, он был удален." : message);

            return val;
        }

        /// <summary>
        /// Получение всех приоритетов задачи
        /// </summary>
        /// <returns>Коллекция приоритетов задачи</returns>
        public IEnumerable<TaskPriority> GetAll()
        {
            return taskPriorityRepository.GetAll();
        }

        #endregion
    }
}
