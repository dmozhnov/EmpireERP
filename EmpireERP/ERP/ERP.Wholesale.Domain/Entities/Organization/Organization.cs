using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using Iesi.Collections.Generic;
using System.Text;

namespace ERP.Wholesale.Domain.Entities
{
    public abstract class Organization : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Хозяйствующий субъект
        /// </summary>
        public virtual EconomicAgent EconomicAgent { get; protected set; }

        /// <summary>
        /// Тип организации
        /// </summary>
        public virtual OrganizationType Type { get; protected set; }

        /// <summary>
        /// Краткое название
        /// </summary>
        /// <remarks>не более 100 символов, обязательное</remarks>
        public virtual string ShortName { get; set; }

        /// <summary>
        /// Полное название
        /// </summary>
        /// <remarks>не более 250 символов, обязательное</remarks>
        public virtual string FullName { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        /// <remarks>не более 250 символов, необязательное</remarks>
        public virtual string Address { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        /// <remarks>не более 20 символов, необязательное</remarks>
        public virtual string Phone { get; set; }

        /// <summary>
        /// Номер факса
        /// </summary>
        /// <remarks>не более 20 символов, необязательное</remarks>
        public virtual string Fax { get; set; }

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
                if (deletionDate == null && value != null) // запрещаем повторную пометку об удалении
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
        /// Список расчетных счетов организации
        /// </summary>
        public virtual IEnumerable<RussianBankAccount> RussianBankAccounts
        {
            get { return new ImmutableSet<RussianBankAccount>(russianBankAccounts); }
        }
        private Iesi.Collections.Generic.ISet<RussianBankAccount> russianBankAccounts;

        /// <summary>
        /// Список иностранных расчетных счетов организации
        /// </summary>
        public virtual IEnumerable<ForeignBankAccount> ForeignBankAccounts
        {
            get { return new ImmutableSet<ForeignBankAccount>(foreignBankAccounts); }
        }
        private Iesi.Collections.Generic.ISet<ForeignBankAccount> foreignBankAccounts;

        #endregion

        #region Конструкторы

        protected Organization()
        {            
        }

        protected internal Organization(string shortName, string fullName, EconomicAgent economicAgent, OrganizationType type)
        {
            ShortName = shortName;
            FullName = fullName;
            EconomicAgent = economicAgent;
            Type = type;
                        
            Address = String.Empty;
            Comment = String.Empty;
            CreationDate = DateTime.Now;
            russianBankAccounts = new HashedSet<RussianBankAccount>();
            foreignBankAccounts = new HashedSet<ForeignBankAccount>();
        }

        #endregion

        #region Методы

        #region Работа с расчетными счетами

        /// <summary>
        /// Добавление расчетного счета организации
        /// </summary>
        /// <param name="bankAccount">Расчетный счет</param>
        public virtual void AddRussianBankAccount(RussianBankAccount bankAccount)
        {
            russianBankAccounts.Add(bankAccount);
            bankAccount.Organization = this;
        }

        /// <summary>
        /// Добавление расчетного счета организации
        /// </summary>
        /// <param name="bankAccount">Расчетный счет</param>
        public virtual void AddForeignBankAccount(ForeignBankAccount bankAccount)
        {
            foreignBankAccounts.Add(bankAccount);
            bankAccount.Organization = this;
        }

        /// <summary>
        /// Удаление расчетного счета организации
        /// </summary>
        /// <param name="bankAccount">Расчетный счет</param>
        public virtual void DeleteRussianBankAccount(RussianBankAccount bankAccount)
        {
            russianBankAccounts.Remove(bankAccount);
            bankAccount.DeletionDate = DateTime.Now;
        }

        /// <summary>
        /// Удаление расчетного счета организации в иностранном банке
        /// </summary>
        /// <param name="bankAccount">Расчетный счет</param>
        public virtual void DeleteForeignBankAccount(ForeignBankAccount bankAccount)
        {
            foreignBankAccounts.Remove(bankAccount);
            bankAccount.DeletionDate = DateTime.Now;
        }

        #endregion

        /// <summary>
        /// Получение полного текстового описания организации в виде Организация, адрес, телефон, ИНН, р/с, банк
        /// </summary>
        /// <returns>Описание организации</returns>
        public virtual string GetFullInfo()
        {
            var sb = new StringBuilder();
            sb.Append(FullName);

            var INN = EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                EconomicAgent.As<JuridicalPerson>().INN : EconomicAgent.As<PhysicalPerson>().INN;

            if (!String.IsNullOrEmpty(Address)) sb.Append(String.Format(", {0}", Address));
            if (!String.IsNullOrEmpty(Phone)) sb.Append(String.Format(", тел.: {0}", Phone));

            sb.Append(String.Format(", ИНН: {0}", INN));

            var bankAccounts = RussianBankAccounts.Where(x => x.IsMaster);
            if (bankAccounts.Count() > 1)
                throw new Exception("Невозможно определить основной расчетный счет организации, так как ей назначено более одного основного расчетного счета.");

            if (bankAccounts.Count() == 1)
            {
                var bankAccount = bankAccounts.Single().As<RussianBankAccount>();
                var bank = bankAccount.Bank.As<RussianBank>();

                sb.Append(String.Format(", р/с {0} в банке {1}, {2}, БИК {3}, корр/с {4} ", bankAccount.Number, bankAccount.Bank.Name, bankAccount.Bank.Address, bank.BIC, bank.CorAccount));
            }

            return sb.ToString();
        }

        #endregion
    }
}
