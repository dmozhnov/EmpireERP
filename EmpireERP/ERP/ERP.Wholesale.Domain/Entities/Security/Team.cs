using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities.Security
{
    /// <summary>
    /// Команда пользователей
    /// </summary>
    public class Team : Entity<short>
    {
        #region Свойства
        
        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string Name { get; set; }
        
        /// <summary>
        /// Список пользователей команды
        /// </summary>
        public virtual IEnumerable<User> Users
        {
            get { return new ImmutableSet<User>(users); }
        }
        private Iesi.Collections.Generic.ISet<User> users;

        /// <summary>
        /// Список мест хранения команды
        /// </summary>
        public virtual IEnumerable<Storage> Storages
        {
            get { return new ImmutableSet<Storage>(storages); }
        }
        private Iesi.Collections.Generic.ISet<Storage> storages;

        /// <summary>
        /// Список сделок хранения команды
        /// </summary>
        public virtual IEnumerable<Deal> Deals
        {
            get { return new ImmutableSet<Deal>(deals); }
        }
        private Iesi.Collections.Generic.ISet<Deal> deals;

        /// <summary>
        /// Список заказов на производство команды
        /// </summary>
        public virtual IEnumerable<ProductionOrder> ProductionOrders
        {
            get { return new ImmutableSet<ProductionOrder>(productionOrders); }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrder> productionOrders;

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Кто создал
        /// </summary>
        public virtual User CreatedBy { get; protected set; }

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

        #endregion

        #region Конструкторы

        protected Team() {}

        public Team(string name, User createdBy)
        {
            CreationDate = DateTime.Now;
            users = new HashedSet<User>();
            storages = new HashedSet<Storage>();
            deals = new HashedSet<Deal>();
            productionOrders = new HashedSet<ProductionOrder>();
            
            Name = name;
            CreatedBy = createdBy;
        }

        #endregion

        #region Методы

        #region Добавление / удаление пользователя

        /// <summary>
        /// Добавление пользователя в команду
        /// </summary>
        /// <param name="user"></param>
        public virtual void AddUser(User user)
        {
            if (users.Any(x => x.Id == user.Id))
            {
                throw new Exception("Пользователь уже входит в состав данной команды.");
            }

            users.Add(user);
            if (!user.Teams.Any(x => x.Id == Id))
            {
                user.AddTeam(this);
            }
        }

        /// <summary>
        /// Удаление пользователя из команды
        /// </summary>
        /// <param name="user"></param>
        public virtual void RemoveUser(User user)
        {
            if (!users.Any(x => x.Id == user.Id))
            {
                throw new Exception("Пользователь не является участником данной команды.");
            }

            users.Remove(user);
            if (user.Teams.Any(x => x.Id == Id))
            {
                user.RemoveTeam(this);
            }
        }

        /// <summary>
        /// Удаление всех пользователей из команды
        /// </summary>
        public virtual void RemoveAllUsers()
        {
            users.Clear();
        }

        #endregion

        #region Добавление / удаление сделки

        /// <summary>
        /// Добавление сделки в команду
        /// </summary>
        /// <param name="deal">Сделка</param>
        public virtual void AddDeal(Deal deal)
        {
            if (deals.Any(x => x.Id == deal.Id))
            {
                throw new Exception("Сделка уже входит в область видимости данной команды.");
            }

            deals.Add(deal);
        }

        /// <summary>
        /// Удаление сделки из команды
        /// </summary>
        /// <param name="deal">Сделка</param>
        public virtual void RemoveDeal(Deal deal)
        {
            if (!Deals.Any(x => x.Id == deal.Id))
            {
                throw new Exception("Сделка не входит в область видимости данной команды.");
            }

            deals.Remove(deal);
        }

        /// <summary>
        /// Удаление всех сделок из команды
        /// </summary>
        public virtual void RemoveAllDeals()
        {
            deals.Clear();
        }
        #endregion

        #region Добавление / удаление места хранения

        /// <summary>
        /// Добавление места хранения в команду
        /// </summary>
        /// <param name="deal">Место хранения</param>
        public virtual void AddStorage(Storage storage)
        {
            if (storages.Any(x => x.Id == storage.Id))
            {
                throw new Exception("Место хранения уже входит в область видимости данной команды.");
            }

            storages.Add(storage);
        }

        /// <summary>
        /// Удаление места хранения из команды
        /// </summary>
        /// <param name="deal">Место хранения</param>
        public virtual void RemoveStorage(Storage storage)
        {
            if (!storages.Any(x => x.Id == storage.Id))
            {
                throw new Exception("Место хранения не входит в область видимости данной команды.");
            }

            storages.Remove(storage);
        }

        /// <summary>
        /// Удаление всех мест хранения из команды
        /// </summary>
        public virtual void RemoveAllStorages()
        {
            storages.Clear();
        }
        #endregion

        #region Добавление / удаление заказа на производство

        /// <summary>
        /// Добавление заказа на производство в команду
        /// </summary>
        /// <param name="deal">Заказ на производство</param>
        public virtual void AddProductionOrder(ProductionOrder order)
        {
            if (productionOrders.Any(x => x.Id == order.Id))
            {
                throw new Exception("Заказ на производство уже входит в область видимости данной команды.");
            }

            productionOrders.Add(order);
        }

        /// <summary>
        /// Удаление заказа на производство из команды
        /// </summary>
        /// <param name="deal">Заказ на производство</param>
        public virtual void RemoveProductionOrder(ProductionOrder order)
        {
            if (!productionOrders.Any(x => x.Id == order.Id))
            {
                throw new Exception("Заказ на производство не входит в область видимости данной команды.");
            }

            productionOrders.Remove(order);
        }

        /// <summary>
        /// Удаление всех заказов на производство из команды
        /// </summary>
        public virtual void RemoveAllProductionOrders()
        {
            productionOrders.Clear();
        }
        #endregion
        
        #endregion

    }
}
