using ERP.Infrastructure.Entities;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Базовый класс для типового справочника
    /// </summary>
    public abstract class BaseDictionary : Entity<short>
    {
        #region Свойства

        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string Name { get; set; }

        #endregion

        #region Конструкторы

        protected BaseDictionary()
        {
        }

        protected BaseDictionary(string name)
        {
            Name = name;
        }

        #endregion
    }
}
