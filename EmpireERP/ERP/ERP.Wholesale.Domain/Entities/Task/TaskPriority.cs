using System;
using ERP.Infrastructure.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Приоритет задачи
    /// </summary>
    public class TaskPriority: Entity<short>
    {
        #region Свойства

        /// <summary>
        /// Название
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Порядковый номер в списке
        /// </summary>
        public virtual short OrdinalNumber { get; set; }

        #endregion

        #region Конструкторы

        protected TaskPriority() {}

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Приоритет</param>
        /// <param name="ordinalNumber">Порядковый номер (для сортировки)</param>
        public TaskPriority(string name, short ordinalNumber)
        {
            ValidationUtils.Assert(!String.IsNullOrEmpty(name), "Необходимо указать название приоритета задачи.");

            Name = name;
            OrdinalNumber = ordinalNumber;
        }

        #endregion
    }
}
