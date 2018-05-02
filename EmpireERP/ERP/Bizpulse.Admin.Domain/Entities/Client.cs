using System;
using System.Collections.Generic;
using System.Linq;
using Bizpulse.Admin.Domain.ValueObjects;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using Iesi.Collections.Generic;

namespace Bizpulse.Admin.Domain.Entities
{
    /// <summary>
    /// Клиент системы
    /// </summary>
    public class Client : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Номер
        /// </summary>
        public virtual int Number { get { return Id; } }
        
        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Тип клиента
        /// </summary>
        public virtual ClientType Type { get; protected set; }

        /// <summary>
        /// Почтовый адрес
        /// </summary>
        public virtual Address PostalAddress { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        /// <remarks>не более 20 символов, необязательное</remarks>
        public virtual string Phone { get; set; }

        /// <summary>
        /// E-mail администратора системы
        /// </summary>
        /// <remarks>не более 50 символов</remarks>
        public virtual string AdminEmail { get; set; }

        /// <summary>
        /// Промо-код
        /// </summary>
        /// <remarks>не более 10 символов, необязательное</remarks>
        public virtual string PromoCode { get; set; }

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public virtual string DisplayName { get { return ""; } }

        /// <summary>
        /// Комментарий
        /// </summary>
        /// <remarks>не более 4000 символов, необязательное</remarks>
        public virtual string Comment { get; set; }
        
        /// <summary>
        /// Имя сервера БД
        /// </summary>
        public virtual string DBServerName { get; set; }

        /// <summary>
        /// Название БД
        /// </summary>
        public virtual string DBName { get; set; }

        /// <summary>
        /// Дата блокировки клиента
        /// </summary>
        public virtual DateTime? BlockingDate { get; set; }

        /// <summary>
        /// Администратор, заблокировавший клиента 
        /// </summary>
        public virtual Administrator BlockedBy { get; set; }

        /// <summary>
        /// Заблокирован ли клиент
        /// </summary>
        public virtual bool IsBlocked
        {
            get { return BlockingDate != null; }
        }

        /// <summary>
        /// Дата удаления клиента
        /// </summary>
        public virtual DateTime? DeletionDate { get; set; }

        /// <summary>
        /// Администратор, удаливший клиента 
        /// </summary>
        public virtual Administrator DeletedBy { get; set; }

        /// <summary>
        /// Пользователи аккаунта клиента
        /// </summary>
        public virtual IEnumerable<ClientUser> Users
        {
            get { return new ImmutableSet<ClientUser>(users); }
        }
        private Iesi.Collections.Generic.ISet<ClientUser> users = new HashedSet<ClientUser>();

        /// <summary>
        /// Наборы услуг клиента
        /// </summary>
        public virtual IEnumerable<ServiceSet> ServiceSets
        {
            get { return new ImmutableSet<ServiceSet>(serviceSets); }
        }
        private Iesi.Collections.Generic.ISet<ServiceSet> serviceSets = new HashedSet<ServiceSet>();

        /// <summary>
        /// Текущий аванс
        /// </summary>
        public virtual decimal PrepaymentSum { get; protected internal set; }

        /// <summary>
        /// Время последней активности на сайте (время последнего входа в аккаунт)
        /// </summary>
        public virtual DateTime LastActivityDate { get; set; }

        #endregion

        #region Конструкторы

        protected Client() {}

        public Client(DateTime currentDate)
        {
            Type = ClientType.Undefined;
            CreationDate = LastActivityDate = currentDate;
            Comment = "";
            DBName = "";
        }

        protected Client(ClientType type, Address postalAddress, DateTime currentDate)
            : this(currentDate)
        {
            Type = type;
            PostalAddress = postalAddress;
        }

        #endregion

        #region Методы

        #region Добавление / удаление пользователя
        
        /// <summary>
        /// Добавление пользователя в аккаунт клиента
        /// </summary>
        /// <param name="clientUser"></param>
        public virtual void AddUser(ClientUser clientUser)
        {
            ValidationUtils.Assert(!Users.Any(x => x.Login == clientUser.Login), "Пользователь с данным логином уже принадлежит аккаунту клиента.");

            users.Add(clientUser);
            clientUser.Client = this;
        }

        /// <summary>
        /// Удаление пользователя из аккаунта клиента
        /// </summary>
        public virtual void DeleteUser(ClientUser clientUser)
        {
            users.Remove(clientUser);
            clientUser.DeletionDate = DateTime.Now;
        } 
        
        #endregion

        #region Блокировка / отмена блокировки
        
        /// <summary>
        /// Блокировка клиента
        /// </summary>
        /// <param name="currentDate">Текущая дата</param>
        /// <param name="blockedBy">Администратор, блокирующий клиента</param>
        public virtual void Block(DateTime currentDate, Administrator blockedBy)
        {
            BlockingDate = currentDate;
            BlockedBy = blockedBy;
        }

        /// <summary>
        /// Отмена блокировки клиента
        /// </summary>
        public virtual void UnBlock()
        {
            BlockingDate = null;
            BlockedBy = null;
        } 
        
        #endregion

        #region Добавление набора услуг

        /// <summary>
        /// Добавление набора услуг
        /// </summary>
        public virtual void AddServiceSet(ServiceSet serviceSet)
        {
            serviceSets.Add(serviceSet);
            serviceSet.Client = this;
        }

        /// <summary>
        /// Удаление набора услуг
        /// </summary>
        public virtual void DeleteServiceSet(ServiceSet serviceSet)
        {
            serviceSets.Remove(serviceSet);
            serviceSet.DeletionDate = DateTime.Now;
        }

        /// <summary>
        /// Добавление первого набора услуг и первой услуги в набор
        /// </summary>
        /// <param name="client">Клиент</param>
        /// <param name="rate">Тариф</param>
        /// <param name="userCount">Максимальное кол-во пользователей на аккаунт</param>
        /// <param name="currentDateTime">Текущие дата и время</param>
        public virtual void CreateInitialServiceSet(Client client, Rate rate, short extraActiveUserCount, short extraTeamCount, 
            short extraStorageCount, short extraAccountOrganizationCount, short extraGigabyteCount, DateTime currentDateTime)
        {
            var configuration = new ServiceSetConfiguration(rate, extraActiveUserCount, extraTeamCount, 
                extraStorageCount, extraAccountOrganizationCount, extraGigabyteCount);                       
            var serviceSet = new ServiceSet(configuration, 1, 0, 0, currentDateTime);

            client.AddServiceSet(serviceSet);
            serviceSet.Activate(currentDateTime);
            serviceSet.AddNewService(currentDateTime);
        }

        /// <summary>
        /// Добавление первого набора услуг и первой услуги в набор при быстрой регистрации версии
        /// </summary>
        /// <param name="client">Клиент</param>
        /// <param name="rate">Тариф</param>        
        /// <param name="currentDateTime">Текущие дата и время</param>
        public virtual void CreateInitialStandardServiceSet(Client client, Rate rate, DateTime currentDateTime)
        {
            var configuration = new ServiceSetConfiguration(rate, 0, 0, 0, 0, 0);
            var serviceSet = new ServiceSet(configuration, 1, 0, 0, currentDateTime);

            client.AddServiceSet(serviceSet);
            serviceSet.Activate(currentDateTime);
            serviceSet.AddNewService(currentDateTime);
        }

        #endregion

        #endregion
    }
}
