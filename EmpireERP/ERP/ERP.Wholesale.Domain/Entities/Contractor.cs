using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Контрагент
    /// </summary>
    public abstract class Contractor : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Тип контрагента
        /// </summary>
        public virtual ContractorType ContractorType { get; protected set; }

        /// <summary>
        /// Рейтинг
        /// </summary>
        /// <remarks>от 0 до 10</remarks>
        public virtual byte Rating
        {
            get { return rating; }
            set
            {
                if (value < 0 || value > 10)
                {
                    throw new Exception("Значение рейтинга должно попадать в интервал [0-10].");
                }

                rating = value;
            }
        }
        protected byte rating;

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
        /// Список связанных с контрагентом договоров
        /// </summary>
        public virtual IEnumerable<Contract> Contracts
        {
            get { return new ImmutableSet<Contract>(contracts); }
        }
        protected Iesi.Collections.Generic.ISet<Contract> contracts;

        /// <summary>
        /// Количество связанных договоров
        /// </summary>
        public virtual int ContractCount
        {
            get { return contracts.Count; }
        }

        /// <summary>
        /// Список связанных организаций
        /// </summary>
        public virtual IEnumerable<ContractorOrganization> Organizations
        {
            get { return new ImmutableSet<ContractorOrganization>(organizations); }
        }
        private Iesi.Collections.Generic.ISet<ContractorOrganization> organizations;

        /// <summary>
        /// Количество связанных организаций
        /// </summary>
        public virtual int OrganizationCount
        {
            get { return organizations.Count; }
        }

        /// <summary>
        /// Комментарий
        /// </summary>
        /// <remarks>не более 4000 символов</remarks>
        public virtual string Comment { get; set; }

        #endregion

        #region Конструкторы

        protected Contractor()
        {
        }

        protected Contractor(string name)
        {
            CreationDate = DateTime.Now;
            contracts = new HashedSet<Contract>();
            organizations = new HashedSet<ContractorOrganization>();
            Comment = String.Empty;
            Rating = 0;
            Name = name;
        }

        #endregion

        #region Методы

        #region Организации контрагента

        /// <summary>
        /// Добавление организации в список организаций контрагента
        /// </summary>
        public virtual void AddContractorOrganization(ContractorOrganization contractorOrganization)
        {
            if (organizations.Contains(contractorOrganization))
            {
                throw new Exception("Данная организация уже содержится в списке организаций контрагента.");
            }

            organizations.Add(contractorOrganization);
            if (!contractorOrganization.Contractors.Contains(this))
            {
                contractorOrganization.AddContractor(this);
            }
        }

        /// <summary>
        /// Удаление организации из списка организаций контрагента
        /// </summary>
        public virtual void RemoveContractorOrganization(ContractorOrganization contractorOrganization)
        {
            if (!organizations.Contains(contractorOrganization))
            {
                throw new Exception("Данная организация не содержится в списке организаций контрагента. Возможно, она была удалена.");
            }

            organizations.Remove(contractorOrganization);
            if (contractorOrganization.Contractors.Contains(this))
            {
                contractorOrganization.RemoveContractor(this);
            }
        }

        #endregion

        #endregion
    }
}
