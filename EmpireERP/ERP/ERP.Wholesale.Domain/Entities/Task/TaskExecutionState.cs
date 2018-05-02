using System;
using ERP.Infrastructure.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Статус исполения задачи
    /// </summary>
    public class TaskExecutionState : Entity<short>
    {
        #region Свойства

        /// <summary>
        /// Статус исполнения задачи
        /// </summary>
        /// <remarks>Максимум 100 символов</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Порядковый номер статуса (для сортировки статусов)
        /// </summary>
        public virtual short OrdinalNumber { get; set; }

        /// <summary>
        /// Логичский тип статуса исполнения задачи
        /// </summary>
        public virtual TaskExecutionStateType ExecutionStateType { get; set; }

        #endregion

        #region Конструкторы
        
        protected TaskExecutionState() {}

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название статуса</param>
        /// <param name="type">Логическй тип статуса</param>
        /// <param name="ordinalNumber">Порядковый номер при сортировке статусов</param>
        public TaskExecutionState(string name, TaskExecutionStateType type, short ordinalNumber)
        {
            ValidationUtils.Assert(!String.IsNullOrEmpty(name), "Необходимо указать статус.");

            Name = name;
            OrdinalNumber = ordinalNumber;
            ExecutionStateType = type;
        }

        #endregion
    }
}
