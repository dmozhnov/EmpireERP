using System;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Общий класс для элемента истории
    /// </summary>
    public abstract class BaseTaskHistoryItem : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Пользователь, внесший изменение выполенения задачи
        /// </summary>
        public virtual User CreatedBy { get; protected set; }

        /// <summary>
        /// Дата изменения исполнения
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Тип истории
        /// </summary>
        public abstract TaskHistoryItemType HistoryItemType { get; }

        #endregion

        #region Конструкторы

        protected BaseTaskHistoryItem() {}

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="creationDate">Дата совершения изменений</param>
        /// <param name="createdBy">Пользователь, внесший изменения</param>
        protected BaseTaskHistoryItem(DateTime creationDate, User createdBy)
        {
            ValidationUtils.NotNull(createdBy, "Необходимо указать пользователя.");

            CreationDate = creationDate;
            CreatedBy = createdBy;
        }

        #endregion
    }
}
