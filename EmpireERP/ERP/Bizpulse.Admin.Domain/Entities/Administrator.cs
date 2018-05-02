using System;
using ERP.Infrastructure.Entities;
using ERP.Utils;

namespace Bizpulse.Admin.Domain.Entities
{
    /// <summary>
    /// Администратор системы
    /// </summary>
    public class Administrator : Entity<short>
    {
        #region Свойства

        /// <summary>
        /// Фамилия
        /// </summary>
        /// <remarks>не более 100</remarks>
        public virtual string LastName { get; protected set; }

        /// <summary>
        /// Имя
        /// </summary>
        /// <remarks>не более 100</remarks>
        public virtual string FirstName { get; protected set; }

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public virtual string DisplayName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        /// <summary>
        /// Логин
        /// </summary>
        /// <remarks>не более 30</remarks>
        public virtual string Login { get; protected set; }

        /// <summary>
        /// Хэш пароля
        /// </summary>        
        public virtual string PasswordHash { get; protected set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        #endregion

        #region Конструкторы

        protected Administrator() {}

        public Administrator(string lastName, string firstName, string login, string password, DateTime currentDate)
        {
            LastName = lastName;
            FirstName = firstName;
            Login = login;
            PasswordHash = CryptographyUtils.ComputeHash(password);
            CreationDate = currentDate;
        }

        #endregion

        #region Методы

        #endregion

        
    }
}
