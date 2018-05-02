using System;
using ERP.Infrastructure.Entities;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Секция места хранения
    /// </summary>
    public class StorageSection : Entity<short>
    {
        #region Свойства

        /// <summary>
        /// Название
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Площадь (м2)
        /// </summary>
        public virtual decimal Square { get; set; }

        /// <summary>
        /// Объем (м3)
        /// </summary>
        public virtual decimal Volume { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public virtual DateTime? DeletionDate
        {
            get { return deletionDate; }
            set { if (deletionDate == null && value != null) deletionDate = value; }   // запрещаем повторную пометку об удалении
        }
        protected DateTime? deletionDate;

        /// <summary>
        /// Место хранения, к которому относится секция
        /// </summary>
        public virtual Storage Storage { get; protected internal set; }

        #endregion

        #region Конструкторы
        
        protected StorageSection() {}

        public StorageSection(string name) : this()
        {
            Name = name;
            Square = 0.0M;
            Volume = 0.0M;

            CreationDate = DateTime.Now;
        }

        #endregion
    }
}
