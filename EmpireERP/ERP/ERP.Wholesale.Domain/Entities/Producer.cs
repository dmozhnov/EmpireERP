using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Производитель
    /// </summary>
    public class Producer : Contractor
    {
        #region Свойства

        #region Основные

        #region Свойства организации

        /// <summary>
        /// Организация производителя (единственная)
        /// </summary>
        public virtual ProducerOrganization Organization { get { return Organizations.First().As<ProducerOrganization>(); } }

        /// <summary>
        /// Название организации
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string OrganizationName
        {
            get { return Organization.FullName; }
            set { Organization.FullName = Organization.ShortName = value; }
        }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public override DateTime? DeletionDate
        {
            get { return base.DeletionDate; }
            set
            {
                base.DeletionDate = value;

                // Уничтожаем список связанных изготовителей (связь ManyToMany)
                manufacturers.Clear();
            }
        }

        #endregion

        /// <summary>
        /// Индивидуальный номер европейской компании (VAT No)
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string VATNo { get; set; }

        /// <summary>
        /// Куратор
        /// </summary>
        public virtual User Curator { get; set; }

        /// <summary>
        /// Менеджер
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string ManagerName { get; set; }

        /// <summary>
        /// E-mail
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string Email { get; set; }

        /// <summary>
        /// Мобильный телефон
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string MobilePhone { get; set; }

        /// <summary>
        /// Скайп
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string Skype { get; set; }

        /// <summary>
        /// MSN
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string MSN { get; set; }

        /// <summary>
        /// Список связанных изготовителей
        /// </summary>
        public virtual IEnumerable<Manufacturer> Manufacturers
        {
            get { return new ImmutableSet<Manufacturer>(manufacturers); }
        }
        private Iesi.Collections.Generic.ISet<Manufacturer> manufacturers;

        #endregion

        #endregion

        #region Конструкторы

        protected Producer()
        {
        }

        public Producer(string name, string organizationName, byte rating, User curator, bool isManufacturer) : base(name)
        {
            manufacturers = new HashedSet<Manufacturer>();

            ContractorType = ContractorType.Producer;

            Rating = rating;
            Curator = curator;

            var org = new ProducerOrganization(organizationName, isManufacturer);

            AddContractorOrganization(org);
        }

        #endregion

        #region Методы

        #region Работа с коллекциями

        /// <summary>
        /// Добавление связанной фабрики-изготовителя
        /// </summary>
        /// <param name="manufacturer"></param>
        public virtual void AddManufacturer(Manufacturer manufacturer)
        {
            if (manufacturers.Contains(manufacturer))
            {
                throw new Exception(String.Format("Фабрика-изготовитель «{0}» уже связана с данным производителем.", manufacturer.Name));
            }

            manufacturers.Add(manufacturer);
        }

        /// <summary>
        /// Удаление связанной фабрики-изготовителя
        /// </summary>
        /// <param name="manufacturer"></param>
        public virtual void RemoveManufacturer(Manufacturer manufacturer)
        {
            if (Organization.HasManufacturer && Organization.Manufacturer == manufacturer)
            {
                throw new Exception("Связь не может быть разорвана, так как производитель является данной фабрикой-изготовителем.");
            }

            if (!manufacturers.Contains(manufacturer))
            {
                throw new Exception(String.Format("Фабрика-изготовитель «{0}» не связана с данным производителем.", manufacturer.Name));
            }

            manufacturers.Remove(manufacturer);
        }

        /// <summary>
        /// Добавление организации в список организаций
        /// </summary>
        public override void AddContractorOrganization(ContractorOrganization contractorOrganization)
        {
            if (OrganizationCount > 0)
            {
                throw new Exception("Невозможно добавить больше одной организации производителя.");
            }

            base.AddContractorOrganization(contractorOrganization);
        }

        /// <summary>
        /// Удаление организации из списка организаций
        /// </summary>
        public override void RemoveContractorOrganization(ContractorOrganization contractorOrganization)
        {
            throw new Exception("Невозможно удалить организацию производителя.");
        }

        #endregion

        #endregion
    }
}
