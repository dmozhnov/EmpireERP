using System;
using System.Collections.Generic;
using System.Linq;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Собственная организация
    /// </summary>
    public class AccountOrganization : Organization
    {
        #region Свойства
        /// <summary>
        /// Связанные места хранения
        /// </summary>
        public virtual IEnumerable<Storage> Storages
        {
            get { return new ImmutableSet<Storage>(storages); }
        }
        private Iesi.Collections.Generic.ISet<Storage> storages;

        /// <summary>
        /// Количество связанных мест хранения
        /// </summary>
        public virtual int StorageCount
        {
            get { return storages.Count; }
        }

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

        /// <summary>
        /// Последние номера документов для организации
        /// </summary>
        protected virtual Iesi.Collections.Generic.ISet<AccountOrganizationDocumentNumbers> documentNumbers { get; set; }
        /// <summary>
        /// Реализует ли организация собственный товар или нет
        /// </summary>
        public virtual bool SalesOwnArticle { get; set; }

        #endregion

        #region Конструкторы

        protected AccountOrganization()
        {
        }

        public AccountOrganization(string shortName, string fullName, EconomicAgent economicAgent) :
            base(shortName, fullName, economicAgent, OrganizationType.AccountOrganization)
        {
            storages = new HashedSet<Storage>();
            contracts = new HashedSet<Contract>();
            documentNumbers = new HashedSet<AccountOrganizationDocumentNumbers>();
            SalesOwnArticle = false;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление договора
        /// </summary>
        /// <param name="newContract">Добавляемый договор</param>
        public virtual void AddContract(Contract contract)
        {
            if (contracts.Contains(contract))
            {
                throw new Exception("Данный договор уже связан с этой организацией.");
            }

            contracts.Add(contract);
            contract.AccountOrganization = this;
        }

        /// <summary>
        /// Добавление места хранения
        /// </summary>
        /// <param name="storage"></param>
        public virtual void AddStorage(Storage storage)
        {
            if (storages.Contains(storage))
            {
                throw new Exception("Данное место хранения уже связано с этой организацией.");
            }

            storages.Add(storage);
            if (!storage.AccountOrganizations.Contains(this))
            {
                storage.AddAccountOrganization(this);
            }
        }

        /// <summary>
        /// Удаление места хранения
        /// </summary>
        /// <param name="storage"></param>
        public virtual void RemoveStorage(Storage storage)
        {
            if (!storages.Contains(storage))
            {
                throw new Exception("Данное место хранения не связано с этой организацией. Возможно, оно было удалено.");
            }

            storages.Remove(storage);
            if (storage.AccountOrganizations.Contains(this))
            {
                storage.RemoveAccountOrganization(this);
            }
        }

        /// <summary>
        /// Получить последние номера документов для организации собственника
        /// </summary>
        /// <param name="year">год, для которого получаем номера</param>
        public virtual AccountOrganizationDocumentNumbers GetLastDocumentNumbers(int year)
        {
            var lastDocumentNumbers = this.documentNumbers.Where(w => w.Year == year).FirstOrDefault();
            if (lastDocumentNumbers == null)
            {
                lastDocumentNumbers = new AccountOrganizationDocumentNumbers(year, this);
                documentNumbers.Add(lastDocumentNumbers);
            }

            return lastDocumentNumbers;
        }
        #endregion
    }
}
