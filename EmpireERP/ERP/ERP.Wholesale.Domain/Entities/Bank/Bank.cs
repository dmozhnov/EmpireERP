using System;
using ERP.Infrastructure.Entities;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Базовый банк
    /// </summary>
    public abstract class Bank: Entity<int>
    {
        #region Поля

        /// <summary>
        /// Название банка
        /// </summary>
        /// <remarks>250 символов</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Адрес банка
        /// </summary>
        /// <remarks>250 символов</remarks>
        public virtual string Address { get; set; }

        /// <summary>
        /// Тип банка
        /// </summary>
        public virtual BankType Type
        {
            get
            {
                return Is<RussianBank>() ? BankType.Bank : BankType.ForeignBank;
            }
        }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public virtual DateTime? DeletionDate { get; set; } 

        #endregion

        #region Конструкторы
        
        protected Bank() {}

        public Bank(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new Exception("Укажите название банка.");
            }

            Name = name;
        }

        #endregion

        #region Методы

        #endregion
    }
}
