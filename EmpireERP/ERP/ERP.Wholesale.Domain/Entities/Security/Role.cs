using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities.Security
{
    /// <summary>
    /// Роль
    /// </summary>
    public class Role : Entity<short>
    {
        #region Свойства
        
        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string Name { get; set; }
        
        /// <summary>
        /// Список распространений прав роли
        /// </summary>
        public virtual IEnumerable<PermissionDistribution> PermissionDistributions 
        {
            get { return new ImmutableSet<PermissionDistribution>(permissionDistributions); }
        }
        private Iesi.Collections.Generic.ISet<PermissionDistribution> permissionDistributions;

        /// <summary>
        /// Список пользователей с данной ролью
        /// </summary>
        public virtual IEnumerable<User> Users
        {
            get { return new ImmutableSet<User>(users); }
        }
        private Iesi.Collections.Generic.ISet<User> users;

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
            set
            {
                if (deletionDate == null && value != null)
                {
                    deletionDate = value;
                }
            }
        }
        protected DateTime? deletionDate;

        /// <summary>
        /// Комментарий
        /// </summary>
        /// <remarks>не более 4000 символов</remarks>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Является ли администратором
        /// </summary>
        public virtual bool IsSystemAdmin { get; protected set; }

        #endregion

        #region Конструкторы

        protected Role()
        {
            CreationDate = DateTime.Now;
            permissionDistributions = new HashedSet<PermissionDistribution>();
            users = new HashedSet<User>();
            IsSystemAdmin = false;
        }

        public Role(string name) : this()
        {
            Name = name;            
        }
        #endregion

        #region Методы

        #region Добавление / удаление пользователя
        
        /// <summary>
        /// Добавление пользователя в роль
        /// </summary>
        /// <param name="user"></param>
        public virtual void AddUser(User user)
        {
            if (users.Any(x => x.Id == user.Id))
            {
                throw new Exception("Пользователь уже обладает данной ролью.");
            }

            users.Add(user);
            if (!user.Roles.Any(x => x.Id == Id))
            {
                user.AddRole(this);
            }
        }

        /// <summary>
        /// Удаление пользователя из роли
        /// </summary>
        /// <param name="user"></param>
        public virtual void RemoveUser(User user)
        {
            if (!users.Any(x => x.Id == user.Id))
            {
                throw new Exception("Пользователь не обладает данной ролью.");
            }

            users.Remove(user);
            if (user.Roles.Any(x => x.Id == Id))
            {
                user.RemoveRole(this);
            }
        } 
        #endregion

        #region Добавление / удаление распространения

        /// <summary>
        /// Добавление распространения права роли
        /// </summary>
        public virtual void AddPermissionDistribution(PermissionDistribution permissionDistribution)
        {
            if (PermissionDistributions.Any(x => x.Permission == permissionDistribution.Permission))
            {
                throw new Exception(String.Format("Роль «{0}» уже обладает правом «{1}».", Name, permissionDistribution.Permission.GetDisplayName()));
            }

            permissionDistributions.Add(permissionDistribution);
            permissionDistribution.Role = this;
        }

        /// <summary>
        /// Удаление права роли
        /// </summary>
        public virtual void RemovePermissionDistribution(PermissionDistribution permissionDistribution)
        {
            if (!PermissionDistributions.Any(x => x.Permission == permissionDistribution.Permission))
            {
                throw new Exception(String.Format("Роль «{0}» не обладает правом «{1}».", Name, permissionDistribution.Permission.GetDisplayName()));
            }

            permissionDistributions.Remove(permissionDistribution);
        }

        #endregion

        #region Проверки возможности совершения операций

        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(!Users.Any(), "Невозможно удалить роль, принадлежащую пользователям системы.");

            ValidationUtils.Assert(!IsSystemAdmin, "Невозможно удалить роль администратора.");
        }

        public virtual void CheckPossibilityToEdit()
        {
            ValidationUtils.Assert(!IsSystemAdmin, "Невозможно редактировать роль администратора.");
        }


        #endregion

        #endregion
    }
}
