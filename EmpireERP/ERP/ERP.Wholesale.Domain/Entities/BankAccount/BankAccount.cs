using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Infrastructure.Entities;

namespace ERP.Wholesale.Domain.Entities
{
    public abstract class BankAccount : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Признак - является ли счет основным
        /// </summary>
        public virtual bool IsMaster { get; set; }

        /// <summary>
        /// Валюта
        /// </summary>
        public virtual Currency Currency { get; set; }

        /// <summary>
        /// Банк, в котором заведен Р/с
        /// </summary>
        public virtual Bank Bank { get; set; }

        /// <summary>
        /// Номер счета
        /// </summary>
        /// <remarks>У отечественного - 20, у иностранного - ?</remarks>
        public virtual string Number { get; set; }

        /// <summary>
        /// Организация, которой принадлежит р/с
        /// </summary>
        public virtual Organization Organization { get; set; }

        /// <summary>
        /// Дата создания счета
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Дата удаления счета
        /// </summary>
        public virtual DateTime? DeletionDate
        {
            get { return deletionDate; }
            set { if (deletionDate == null && value != null) deletionDate = value; }   // запрещаем повторную пометку об удалении
        }
        protected DateTime? deletionDate;

        /// <summary>
        /// Тип расчетного счета
        /// </summary>
        public virtual BankAccountType Type { get; set; }

        #endregion

        #region Конструкторы
        
        protected BankAccount() {}

        public BankAccount(Bank bank, string number, Currency currency)
        {
            Bank = bank;
            Number = number;
            Currency = currency;
            CreationDate = DateTime.Now;
        }

        #endregion

        #region Методы

        #endregion
    }
}
