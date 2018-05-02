using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class TaskTypeService : ITaskTypeService
    {
        #region Поля

        private readonly ITaskTypeRepository taskTypeRepository;

        #endregion

        #region Конструкторы

        public TaskTypeService(ITaskTypeRepository taskTypeRepository)
        {
            this.taskTypeRepository = taskTypeRepository;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Проверка существования типа задачи
        /// </summary>
        /// <param name="id">Идентификатор типа задачи</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <returns>Тип задачи</returns>
        public TaskType CheckTaskTypeExistence(short id, string message = "")
        {
            var val = taskTypeRepository.GetById(id);
            ValidationUtils.NotNull(val, String.IsNullOrEmpty(message) ? "Тип задачи не найден. Возможно, он был удален." : message);

            return val;
        }

        /// <summary>
        /// Получение всех типов задач
        /// </summary>
        /// <returns>Коллекция типов задач</returns>
        public IEnumerable<TaskType> GetAll()
        {
            return taskTypeRepository.GetAll();
        }

        /// <summary>
        /// Получение типа задачи по статусу исполнения
        /// </summary>
        /// <param name="state">Статус исполнения</param>
        /// <returns></returns>
        public TaskType GetTaskTypeByExecutionState(TaskExecutionState state)
        {
            return taskTypeRepository.GetTaskTypeByExecutionState(state.Id);
        }

        #endregion
    }
}
