using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities.Security
{
    /// <summary>
    /// Пользователь системы
    /// </summary>
    public class User : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Сотрудник, связанный с пользователем
        /// </summary>
        public virtual Employee Employee { get; protected set; }

        /// <summary>
        /// Логин
        /// </summary>
        /// <remarks>не более 30</remarks>
        public virtual string Login { get; set; }

        /// <summary>
        /// Хэш пароля
        /// </summary>        
        public virtual string PasswordHash { get; set; }

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        /// <remarks>не более 100</remarks>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Шаблон формирования отображаемого имени
        /// </summary>
        /// <remarks>3 символа</remarks>
        public virtual string DisplayNameTemplate { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Кто создал
        /// </summary>
        public virtual User CreatedBy { get; protected set; }

        /// <summary>
        /// Дата блокировки
        /// </summary>
        public virtual DateTime? BlockingDate { get; protected set; }

        /// <summary>
        /// Кто заблокировал
        /// </summary>
        public virtual User Blocker { get; protected set; }

        /// <summary>
        /// Список ролей пользователя
        /// </summary>
        public virtual IEnumerable<Role> Roles
        {
            get { return new ImmutableSet<Role>(roles); }
        }
        private Iesi.Collections.Generic.ISet<Role> roles;

        /// <summary>
        /// Список команд пользователя
        /// </summary>
        public virtual IEnumerable<Team> Teams
        {
            get { return new ImmutableSet<Team>(teams); }
        }
        private Iesi.Collections.Generic.ISet<Team> teams;

        /// <summary>
        /// Список командных мест хранения пользователя
        /// </summary>
        public virtual IEnumerable<Storage> Storages
        {
            get { return Teams.SelectMany(x => x.Storages).Distinct(); }
        }

        /// <summary>
        /// Заблокирован ли пользователь
        /// </summary>
        public virtual bool IsBlocked
        {
            get { return BlockingDate != null; }
        }

        /// <summary>
        /// Является ли пользователь системным администратором
        /// </summary>
        public virtual bool IsSystemAdmin
        {
            get { return roles.Any(x => x.IsSystemAdmin); }
        }
        
        #endregion

        #region Конструкторы

        protected User()
        {
            CreationDate = DateTime.Now;
            roles = new HashedSet<Role>();
            teams = new HashedSet<Team>();
            DisplayNameTemplate = "LF";
        }

        public User(Employee employee, string displayName, string login, string password, Team team, User createdBy) : this()
        {
            Employee = employee;
            employee.User = this;

            DisplayName = displayName;

            Login = login;
            PasswordHash = CryptographyUtils.ComputeHash(password);

            CreatedBy = createdBy;

            DisplayNameTemplate = "LF";

            AddTeam(team);
        }

        #endregion

        #region Методы

        #region Блокировка / снятие блокировки

        /// <summary>
        /// Блокировка пользователя
        /// </summary>
        /// <param name="blocker"></param>
        public virtual void Block(User blocker)
        {
            if (BlockingDate != null)
            {
                throw new Exception("Пользователь уже заблокирован.");
            }

            if (this.Id == blocker.Id)
            {
                throw new Exception("Невозможно заблокировать самого себя.");
            }

            BlockingDate = DateTime.Now;
            Blocker = blocker;
        }

        /// <summary>
        /// Снятие блокировки пользователя
        /// </summary>
        /// <param name="blocker"></param>
        public virtual void UnBlock()
        {
            if (BlockingDate == null)
            {
                throw new Exception("Пользователь не заблокирован.");
            }

            BlockingDate = null;
            Blocker = null;
        }
        #endregion

        #region Добавление / удаление роли

        /// <summary>
        /// Добавление роли пользователю
        /// </summary>
        /// <param name="role"></param>
        public virtual void AddRole(Role role)
        {
            if (roles.Any(x => x.Id == role.Id))
            {
                throw new Exception("Пользователь уже обладает данной ролью.");
            }

            roles.Add(role);
            if (!role.Users.Any(x => x.Id == Id))
            {
                role.AddUser(this);
            }
        }

        /// <summary>
        /// Удаление роли у пользователя
        /// </summary>
        /// <param name="role"></param>
        public virtual void RemoveRole(Role role)
        {
            if (!roles.Any(x => x.Id == role.Id))
            {
                throw new Exception("Пользователь не обладает данной ролью.");
            }

            roles.Remove(role);
            if (role.Users.Any(x => x.Id == Id))
            {
                role.RemoveUser(this);
            }
        }
        #endregion

        #region Добавление / удаление команды

        /// <summary>
        /// Добавление пользователя в команду
        /// </summary>
        /// <param name="team"></param>
        public virtual void AddTeam(Team team)
        {
            if (teams.Any(x => x.Id == team.Id))
            {
                throw new Exception("Пользователь уже входит в состав данной команды.");
            }

            teams.Add(team);
            if (!team.Users.Any(x => x.Id == Id))
            {
                team.AddUser(this);
            }
        }

        /// <summary>
        /// Удаление пользователя из команды
        /// </summary>
        /// <param name="team"></param>
        public virtual void RemoveTeam(Team team)
        {
            ValidationUtils.Assert(teams.Any(x => x.Id == team.Id), "Пользователь не является участником данной команды.");
            ValidationUtils.Assert(teams.Count != 1, "Пользователь должен быть участником хотя бы одной команды.");

            teams.Remove(team);
            if (team.Users.Any(x => x.Id == Id))
            {
                team.RemoveUser(this);
            }
        }
        #endregion

        #region Смена пароля

        public virtual void ChangePassword(string currentPassword, string newPassword)
        {
            var currentPasswordHash = CryptographyUtils.ComputeHash(currentPassword);

            ValidationUtils.Assert(PasswordHash == currentPasswordHash, "Указан неверный пароль.");

            PasswordHash = CryptographyUtils.ComputeHash(newPassword);            
        }

        #endregion

        #region Установка нового пароля

        public virtual void ResetPassword(string newPassword)
        {            
            PasswordHash = CryptographyUtils.ComputeHash(newPassword);
        }
                
        #endregion

        #region Права пользователя
        
        /// <summary>
        /// Проверка на наличие у пользователя права (с областью видимости кроме None)
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public virtual bool HasPermission(Permission permission)
        {
            if (Roles.SelectMany(x => x.PermissionDistributions).Any(x => x.Permission == permission && x.Type != PermissionDistributionType.None))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Проверка на наличие у пользователя права (с областью видимости кроме None) с генерацией исключения
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public virtual void CheckPermission(Permission permission)
        {
            if (!Roles.SelectMany(x => x.PermissionDistributions).Any(x => x.Permission == permission && x.Type != PermissionDistributionType.None))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        /// <summary>
        /// Получение максимального типа распространения права пользователя
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public virtual PermissionDistributionType GetPermissionDistributionType(Permission permission)
        {
            PermissionDistributionType type = PermissionDistributionType.None;

            var list = Roles.SelectMany(x => x.PermissionDistributions).Where(x => x.Permission == permission);

            if (list.Any())
            {
                type = list.Max(x => x.Type);
            }

            return type;
        }

        /// <summary>
        /// Проверка доступности указанного места хранения текущему пользователю
        /// </summary>
        /// <param name="storage"></param>
        public virtual void CheckStorageAvailability(Storage storage, Permission permission)
        {
            switch (GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    throw new Exception(String.Format("Нет доступа к месту хранения «{0}».", storage.Name));

                case PermissionDistributionType.Personal:
                case PermissionDistributionType.Teams:
                    if (!Teams.SelectMany(x => x.Storages).Contains(storage))
                    {
                        throw new Exception(String.Format("Нет доступа к месту хранения «{0}».", storage.Name));
                    }
                    break;
                
                default:
                    break;
            }
        }

        #endregion

        #region Проверка возможности просмотра учетных цен на некомандных местах хранения

        public virtual bool HasPermissionToViewStorageAccountingPrices(Storage storage)
        {
            return this.HasPermission(Permission.AccountingPrice_NotCommandStorage_View) || this.Teams.SelectMany(x => x.Storages).Contains(storage);
        }

        public virtual void CheckPermissionToViewNotCommandStorageAccountingPrices(Storage storage)
        {
            if(!this.Teams.SelectMany(x => x.Storages).Contains(storage))
            {
                this.CheckPermission(Permission.AccountingPrice_NotCommandStorage_View);
            }
        }

        #endregion

        #region Проверка возможности просмотра закупочных цен на некомандных местах хранения

        /// <summary>
        /// Проверка возможности просмотра закупочных цен на некомандных местах хранения
        /// </summary>
        /// <param name="storage">МХ</param>
        /// <param name="user">Куратор приходной накладной</param>
        /// <returns>true - просмотр возможен</returns>
        public virtual bool HasPermissionToViewStoragePurchasePrices(Storage storage, User user)
        {
            switch (this.GetPermissionDistributionType(Permission.PurchaseCost_View_ForReceipt))
            {
                case PermissionDistributionType.All:
                    return true;

                case PermissionDistributionType.Personal:
                    return this.Teams.SelectMany(x => x.Storages).Contains(storage) && this == user;

                case PermissionDistributionType.Teams:
                    return this.Teams.SelectMany(x => x.Storages).Contains(storage);

                case PermissionDistributionType.None:
                    return false;

                default:
                    throw new Exception("Неизвестный тип распространения права.");
            }
        }

        #endregion

        #endregion
    }
}
