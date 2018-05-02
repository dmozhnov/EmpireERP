using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Организация контрагента
    /// </summary>
    public abstract class ContractorOrganization : Organization
    {
        #region Свойства

        #region Список контрагентов

        /// <summary>
        /// Список контрагентов
        /// </summary>
        public virtual IEnumerable<Contractor> Contractors
        {
            get { return new ImmutableSet<Contractor>(contractors); }
        }
        private Iesi.Collections.Generic.ISet<Contractor> contractors;

        /// <summary>
        /// Количество связанных контрагентов
        /// </summary>
        public virtual int ContractorCount
        {
            get { return contractors.Count; }
        }

        #endregion

        #region Список договоров

        /// <summary>
        /// Список договоров
        /// </summary>
        public virtual IEnumerable<Contract> Contracts
        {
            get { return new ImmutableSet<Contract>(contracts); }
        }
        private Iesi.Collections.Generic.ISet<Contract> contracts;

        /// <summary>
        /// Количество связанных договоров
        /// </summary>
        public virtual int ContractCount
        {
            get { return contracts.Count; }
        }

        #endregion

        #endregion

        #region Конструкторы

        protected ContractorOrganization()
        {
        }

        protected internal ContractorOrganization(string shortName, string fullName, EconomicAgent economicAgent, OrganizationType type)
            : base(shortName, fullName, economicAgent, type)
        {
            contractors = new HashedSet<Contractor>();
            contracts = new HashedSet<Contract>();
        }

        #endregion

        #region Методы

        protected internal virtual void AddContractor(Contractor contractor)
        {
            if (contractors.Contains(contractor))
            {
                throw new Exception("Данный контрагент уже связан с этой организацией.");
            }

            contractors.Add(contractor);
        }

        protected internal virtual void RemoveContractor(Contractor contractor)
        {
            if (!contractors.Contains(contractor))
            {
                throw new Exception("Данный контрагент не связан с этой организацией. Возможно, он был удален.");
            }

            contractors.Remove(contractor);
        }

        protected internal virtual void AddContract(Contract contract)
        {
            if (contracts.Contains(contract))
            {
                throw new Exception("Данный договор уже связан с этой организацией.");
            }

            contracts.Add(contract);
        }

        public virtual void RemoveContract(Contract contract)
        {
            if (!contracts.Contains(contract))
            {
                throw new Exception("Данный договор не связан с этой организацией. Возможно, он был удален.");
            }

            contracts.Remove(contract);
        }

        #endregion
    }
}
