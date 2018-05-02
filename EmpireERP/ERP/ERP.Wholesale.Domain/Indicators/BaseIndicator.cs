using System;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Базовый класс показателей
    /// </summary>
    public abstract class BaseIndicator
    {
        #region Свойства

        /// <summary>
        /// Код
        /// </summary>
        public virtual Guid Id { get; protected set; }

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public virtual DateTime StartDate { get; protected internal set; }

        /// <summary>
        /// Дата окончания действия
        /// </summary>
        public virtual DateTime? EndDate { get; protected internal set; }

        #endregion

        #region Конструкторы

        protected internal BaseIndicator()
        {
        }

        protected internal BaseIndicator(DateTime startDate)
        {
            StartDate = startDate;
        }

        #endregion       
    }
}
