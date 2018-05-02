using System;
using System.Linq;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    public class ProducerOrganization : ContractorOrganization
    {
        #region Свойства

        /// <summary>
        /// ФИО директора
        /// <remarks>не более 100 символов, не обязательное</remarks>
        /// </summary>
        public virtual string DirectorName { get; set; }

        /// <summary>
        /// Производитель (единственный)
        /// </summary>
        public virtual Producer Producer { get { return Contractors.FirstOrDefault().As<Producer>(); } }

        /// <summary>
        /// Является изготовителем (признак, редактируемый пользователем)
        /// </summary>
        public virtual bool IsManufacturer { get; set; }

        /// <summary>
        /// Имеет связанного изготовителя, которым является (может и не иметь при этом признак "Является изготовителем")
        /// </summary>
        public virtual bool HasManufacturer
        {
            get { return Manufacturer != null; }
        }

        /// <summary>
        /// Фабрика-изготовитель, которой является производитель
        /// </summary>
        public virtual Manufacturer Manufacturer { get; set; }

        /// <summary>
        /// Установка изготовителя и связывание его с производителем, если еще не связан
        /// </summary>
        public virtual void SetManufacturer(Manufacturer manufacturer)
        {
            ValidationUtils.NotNull(manufacturer, "Изготовитель не указан.");
            Manufacturer = manufacturer;

            if (!Producer.Manufacturers.Contains(manufacturer))
            {
                Producer.AddManufacturer(manufacturer);
            }
        }

        #endregion

        #region Конструкторы

        protected ProducerOrganization()
        {
        }

        public ProducerOrganization(string organizationName, bool isManufacturer) : base(organizationName, organizationName, null, OrganizationType.ProducerOrganization)
        {
            IsManufacturer = isManufacturer;
        }

        #endregion
    }
}
