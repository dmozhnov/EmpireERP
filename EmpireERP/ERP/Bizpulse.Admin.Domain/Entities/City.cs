using ERP.Infrastructure.Entities;

namespace Bizpulse.Admin.Domain.Entities
{
    /// <summary>
    /// Город
    /// </summary>
    public class City : Entity<short>
    {
        #region Свойства

        /// <summary>
        /// Регион, которому принадлежит город
        /// </summary>
        public virtual Region Region { get; protected internal set; }
        
        /// <summary>
        /// Наименование
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Порядок сортировки при выводе
        /// </summary>        
        public virtual short SortOrder { get; set; }

        #endregion

        #region Конструкторы

        protected City() {}

        #endregion

    }
}
