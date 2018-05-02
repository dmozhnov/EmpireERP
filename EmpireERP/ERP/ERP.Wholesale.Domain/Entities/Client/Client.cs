using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Клиент
    /// </summary>
    public class Client : Contractor
    {
        #region Свойства

        /// <summary>
        /// Тип клиента
        /// </summary>
        public virtual ClientType Type { get; set; }

        /// <summary>
        /// Лояльность
        /// </summary>
        public virtual ClientLoyalty Loyalty { get; set; }

        /// <summary>
        /// Программа обслуживания клиента
        /// </summary>
        public virtual ClientServiceProgram ServiceProgram { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        public virtual ClientRegion Region { get; set; }

        /// <summary>
        /// Сделки клиента
        /// </summary>
        public virtual IEnumerable<Deal> Deals
        {
            get { return new ImmutableSet<Deal>(deals); }
        }
        private Iesi.Collections.Generic.ISet<Deal> deals;
        
        /// <summary>
        /// Количество сделок клиента
        /// </summary>
        public virtual int DealCount
        {
            get { return deals.Count; }
        }

        /// <summary>
        /// Заблокирован ли клиент вручную
        /// </summary>
        public virtual bool IsBlockedManually
        {
            get { return ManualBlockingDate != null; }
        }

        /// <summary>
        /// Дата блокировки
        /// </summary>
        public virtual DateTime? ManualBlockingDate { get; set; }

        /// <summary>
        /// Пользователь, заблокировавший клиента вручную
        /// </summary>
        public virtual User ManualBlocker { get; set; }

        /// <summary>
        /// Сумма корректировок сальдо
        /// </summary>
        public virtual decimal InitialBalance
        {
            get
            {
                return deals.Sum(x => x.InitialBalance);
            }
        }        

        /// <summary>
        /// Фактический адрес
        /// </summary>
        public virtual string FactualAddress { get; set; }

        /// <summary>
        /// Контактный телефон
        /// </summary>
        public virtual string ContactPhone { get; set; }

        #endregion

        #region Конструкторы

        protected Client()
        {            
        }

        public Client(string name, ClientType type, ClientLoyalty loyalty, ClientServiceProgram serviceProgram, ClientRegion region, byte rating) : base(name)
        {
            ContractorType = ContractorType.Client;

            Type = type;
            Loyalty = loyalty;
            ServiceProgram = serviceProgram;
            Region = region;
            Rating = rating;

            deals = new HashedSet<Deal>();
        }

        #endregion

        #region Методы

        #region Работа со сделками

        /// <summary>
        /// Добавление сделки
        /// </summary>
        /// <param name="deal">Сделка</param>
        public virtual void AddDeal(Deal deal)
        {
            deals.Add(deal);
            deal.Client = this;
        }

        #endregion

        #region Блокировка / снятие блокировки

        /// <summary>
        /// Блокировка клиента вручную
        /// </summary>
        /// <param name="blocker">Пользователь, заблокировавший клиента вручную</param>
        public virtual void Block(User blocker)
        {
            ValidationUtils.NotNull(blocker, "Пользователь, заблокировавший клиента, не указан.");

            if (ManualBlockingDate != null)
            {
                throw new Exception("Клиент уже заблокирован.");
            }

            ManualBlockingDate = DateTime.Now;
            ManualBlocker = blocker;
        }

        /// <summary>
        /// Снятие ручной блокировки клиента
        /// </summary>
        /// <param name="blocker"></param>
        public virtual void Unblock()
        {
            if (ManualBlockingDate == null)
            {
                throw new Exception("Клиент не заблокирован.");
            }

            ManualBlockingDate = null;
            ManualBlocker = null;
        }

        #endregion

        #endregion
    }
}
