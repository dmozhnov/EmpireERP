using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Infrastructure.Entities;

namespace Bizpulse.Admin.Domain.Entities
{
    /// <summary>
    /// Пользователь системы (от клиента)
    /// </summary>
    public class ClientUser : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Клиент, к которому относится пользователь
        /// </summary>
        public virtual Client Client { get; protected internal set; }

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
        /// Отчество
        /// </summary>
        /// <remarks>не более 100</remarks>
        public virtual string Patronymic { get; set; }

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public virtual string DisplayName
        {
            get { return LastName + " " + FirstName + (!string.IsNullOrEmpty(Patronymic) ? " " + Patronymic : ""); }
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
        /// Является ли пользователь администратором аккаунта клиента
        /// </summary>
        public virtual bool IsClientAdmin { get; protected set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime? DeletionDate { get; set; }

        /// <summary>
        /// Заблокирован ли пользователь
        /// </summary>
        public virtual bool IsBlocked { get; protected set; }

        #endregion

        #region Конструкторы

        protected ClientUser() {}

        public ClientUser(string lastName, string firstName, string login, string passwordHash, bool isClientAdmin, DateTime currentDate)
        {
            LastName = lastName;
            FirstName = firstName;
            Patronymic = "";
            Login = login;
            PasswordHash = passwordHash;
            IsClientAdmin = isClientAdmin;
            CreationDate = currentDate;
        }

        #endregion

        #region Методы

        #endregion
    }
}
