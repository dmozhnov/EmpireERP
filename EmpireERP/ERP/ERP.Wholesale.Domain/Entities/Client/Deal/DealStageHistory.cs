using System;
using ERP.Infrastructure.Entities;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// История этапов сделки
    /// </summary>
    public class DealStageHistory : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Сделка
        /// </summary>
        public virtual Deal Deal { get; protected internal set; }

        /// <summary>
        /// Дата начала этапа
        /// </summary>
        public virtual DateTime StartDate { get; protected internal set; }

        /// <summary>
        /// Дата окончания этапа
        /// </summary>
        public virtual DateTime? EndDate { get; protected internal set; }

        /// <summary>
        /// Длительность этапа
        /// </summary>
        public virtual TimeSpan? StageDuration
        {
            get { return (EndDate == null ? (TimeSpan?)null : EndDate.Value.Subtract(StartDate)); }
        }

        /// <summary>
        /// Этап сделки
        /// </summary>
        public virtual DealStage DealStage { get; protected set; }

        #endregion

        #region Конструкторы

        protected DealStageHistory() {}

        public DealStageHistory(DealStage dealStage)
	    {
            DealStage = dealStage;
            StartDate = DateTime.Now;
	    }

        #endregion

        #region Методы

        #endregion
    }
}
