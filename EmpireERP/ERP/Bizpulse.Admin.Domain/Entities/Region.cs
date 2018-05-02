using ERP.Infrastructure.Entities;

namespace Bizpulse.Admin.Domain.Entities
{
    /// <summary>
    /// Регион (субъект) РФ
    /// </summary>
    public class Region : Entity<short>
    {
        #region Свойства

        /// <summary>
        /// Наименование
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Номер региона
        /// </summary>
        /// <remarks>не более 2 символов</remarks>
        public virtual string Code { get; set; }

        /// <summary>
        /// Порядок сортировки при выводе
        /// </summary>        
        public virtual short SortOrder { get; set; }

        #endregion

        #region Конструкторы

        protected Region()
        {
        }

        #endregion

    }
}
