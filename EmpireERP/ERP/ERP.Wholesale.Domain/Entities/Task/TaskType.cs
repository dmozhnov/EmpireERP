using System;
using System.Collections.Generic;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип задачи
    /// </summary>
    public class TaskType : Entity<short>
    {
        #region Свойтсва

        /// <summary>
        /// Название
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Возможные статусы задачи
        /// </summary>
        public virtual IEnumerable<TaskExecutionState> States
        {
            get { return states; }
        }
        private Iesi.Collections.Generic.ISet<TaskExecutionState> states;

        #endregion

        #region Конструкторы
        
        protected TaskType() {}

        /// <summary>
        /// Конструктор типа задачи
        /// </summary>
        /// <param name="name">Тип задачи</param>
        public TaskType(string name)
            :this()
        {
            ValidationUtils.Assert(!String.IsNullOrEmpty(name), "Необходимо указать название типа задачи.");
            Name = name;

            states = new HashedSet<TaskExecutionState>();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление статуса
        /// </summary>
        /// <param name="state">Добавляемый статус</param>
        public virtual void AddState(TaskExecutionState state)
        {
            ValidationUtils.NotNull(state, "Необходимо указать статус задачи.");
            states.Add(state);
        }

        /// <summary>
        /// Удаление статуса
        /// </summary>
        /// <param name="state">Удаляемый статус</param>
        public virtual void DeleteState(TaskExecutionState state)
        {
            ValidationUtils.NotNull(state, "Необходимо указать статус задачи.");
            states.Remove(state);
        }

        #endregion
    }
}
