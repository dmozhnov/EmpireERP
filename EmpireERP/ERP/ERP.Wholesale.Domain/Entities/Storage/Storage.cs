using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Место хранения
    /// </summary>
    public class Storage : Entity<short>
    {
        #region Свойства

        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public virtual StorageType Type { get; set; }

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
                // запрещаем повторную пометку об удалении
                if (deletionDate == null && value != null)
                {
                    deletionDate = value;

                    foreach (var section in Sections)
                    {
                        if (section.DeletionDate == null)
                        {
                            section.DeletionDate = deletionDate;
                        }
                    }
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
        /// Секции места хранения
        /// </summary>
        public virtual IEnumerable<StorageSection> Sections
        {
            get { return new ImmutableSet<StorageSection>(sections); }
        }
        private Iesi.Collections.Generic.ISet<StorageSection> sections;

        /// <summary>
        /// Количество секций места хранения
        /// </summary>
        public virtual int SectionCount
        {
            get { return sections.Count; }
        }

        /// <summary>
        /// Связанные организации
        /// </summary>
        public virtual IEnumerable<AccountOrganization> AccountOrganizations
        {
            get { return new ImmutableSet<AccountOrganization>(accountOrganizations); }
        }
        private Iesi.Collections.Generic.ISet<AccountOrganization> accountOrganizations;

        /// <summary>
        /// Количество связанных организаций
        /// </summary>
        public virtual int AccountOrganizationCount
        {
            get { return accountOrganizations.Count; }
        }

        #endregion

        #region Конструкторы

        protected Storage()
        {
        }

        public Storage(string name, StorageType type)
        {
            sections = new HashedSet<StorageSection>();
            accountOrganizations = new HashedSet<AccountOrganization>();
            CreationDate = DateTime.Now;
            Comment = String.Empty;
            
            Type = type;
            Name = name;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление секции места хранения
        /// </summary>
        /// <param name="section">Секция места хранения</param>
        public virtual void AddSection(StorageSection section)
        {
            sections.Add(section);
            section.Storage = this;
        }

        /// <summary>
        /// Удаление секции места хранения
        /// </summary>
        /// <param name="section">Секция места хранения</param>
        public virtual void RemoveSection(StorageSection section)
        {
            section.DeletionDate = DateTime.Now;
            sections.Remove(section);
        }

        /// <summary>
        /// Добавление связанной собственной организации
        /// </summary>
        /// <param name="accountOrganization">Собственная организация</param>
        public virtual void AddAccountOrganization(AccountOrganization accountOrganization)
        {
            if (accountOrganizations.Contains(accountOrganization))
            {
                throw new Exception(String.Format("Организация «{0}» уже связана с местом хранения «{1}».", accountOrganization.ShortName, this.Name));
            }

            accountOrganizations.Add(accountOrganization);
            if (!accountOrganization.Storages.Contains(this))
            {
                accountOrganization.AddStorage(this);
            }
        }

        /// <summary>
        /// Удаление связанной собственной организации
        /// </summary>
        /// <param name="accountOrganization">Собственная организация</param>
        public virtual void RemoveAccountOrganization(AccountOrganization accountOrganization)
        {
            if (!accountOrganizations.Contains(accountOrganization))
            {
                throw new Exception("Данная организация не связана с этим местом хранения. Возможно, она была удалена.");
            }

            accountOrganizations.Remove(accountOrganization);
            if (accountOrganization.Storages.Contains(this))
            {
                accountOrganization.RemoveStorage(this);
            }
        }


        #endregion
    }
}
