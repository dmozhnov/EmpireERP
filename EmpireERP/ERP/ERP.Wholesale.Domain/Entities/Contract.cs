using System;
using System.Collections.Generic;
using ERP.Infrastructure.Entities;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Договор
    /// </summary>
    public /*abstract*/ class Contract : Entity<short>
    {
        #region Свойства

        /// <summary>
        /// Номер
        /// </summary>
        /// <remarks>не более 50 символов</remarks>
        public virtual string Number { get; set; }

        /// <summary>
        /// Дата договора
        /// </summary>
        public virtual DateTime Date { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Собственная организация
        /// </summary>
        public virtual AccountOrganization AccountOrganization
        {
            get { return accountOrganization; }
            set { accountOrganization = value; }            
        }
        private AccountOrganization accountOrganization;

        /// <summary>
        /// Организация контрагента
        /// </summary>
        public virtual ContractorOrganization ContractorOrganization
        {
            get { return contractorOrganization; }
            set { contractorOrganization = value; }
        }
        private ContractorOrganization contractorOrganization;

        /// <summary>
        /// Список контрагентов, связанных с договором
        /// </summary>
        public virtual IEnumerable<Contractor> Contractors
        {
            get { return new ImmutableSet<Contractor>(contractors); }
        }
        private Iesi.Collections.Generic.ISet<Contractor> contractors;

        /// <summary>
        /// Количество контрагентов, связанных с договором
        /// </summary>
        public virtual int ContractorCount
        {
            get { return contractors.Count;  }
        }

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
            set { if (deletionDate == null && value != null) deletionDate = value; }   // запрещаем повторную пометку об удалении
        }
        protected DateTime? deletionDate;

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public virtual DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания действия
        /// </summary>
        public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        /// <remarks>не более 4000 символов</remarks>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Полное название договора
        /// </summary>
        public virtual string FullName
        {
            get
            {
                return !String.IsNullOrEmpty(Number) ? String.Format("{0} № {1} от {2}", Name, Number, Date.ToShortDateString()) :
                    String.Format("{0} от {1}", Name, Date.ToShortDateString());
            }
        }

        #endregion

        #region Конструкторы

        protected Contract()
        {            
        }

        protected internal Contract(AccountOrganization accountOrganization, string name, string number, DateTime date, DateTime startDate)
        {
            CreationDate = DateTime.Now;
            contractors = new HashedSet<Contractor>();
            Comment = String.Empty;
            
            accountOrganization.AddContract(this);

            StartDate = startDate.Date;
            Date = date.Date;
            Number = number;
            Name = name;
        }

        #endregion

        #region Методы

        protected internal virtual void AddContractor(Contractor contractor)
        {
            if (contractors.Contains(contractor))
            {
                throw new Exception("Данный контрагент уже связан с этим договором.");
            }

            contractors.Add(contractor);
        }

        protected internal virtual void RemoveContractor(Contractor contractor)
        {
            if (!contractors.Contains(contractor))
            {
                throw new Exception("Данный контрагент не связан с этим договором. Возможно, он был удален.");
            }

            contractors.Remove(contractor);
        }

        #endregion
    }
}
