using System;
using ERP.Infrastructure.Entities;

namespace Bizpulse.Admin.Domain.Entities
{
    /// <summary>
    /// Услуга клиента
    /// </summary>
    public class Service : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Набор, в который входит услуга
        /// </summary>
        public virtual ServiceSet ServiceSet { get; protected internal set; }

        /// <summary>
        /// Название (выстявляется при закрытии услуги)
        /// </summary>
        /// <remarks>Не более 200 символов</remarks>
        public virtual string Name { get; protected set; }

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public virtual DateTime StartDate { get; protected internal set; }

        /// <summary>
        /// Дата окончания действия (закрытия)
        /// </summary>
        public virtual DateTime? EndDate { get; protected internal set; }

        /// <summary>
        /// Является ли услуга текущей (активной)
        /// </summary>
        public virtual bool IsCurrent
        {
            get { return StartDate <= DateTime.Now && EndDate == null; }
        }

        /// <summary>
        /// Реальная стоимость услуги
        /// </summary>
        public virtual decimal? FactualCost { get; protected set; }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public virtual DateTime? DeletionDate { get; protected internal set; }

        #endregion

        #region Конструкторы

        protected Service() {}

        protected internal Service(DateTime startDate)
        {            
            StartDate = startDate;
            Name = "";
        }

        #endregion

        #region Методы

        #endregion
    }
}
