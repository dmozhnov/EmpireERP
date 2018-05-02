using Bizpulse.Admin.Domain.Entities;

namespace Bizpulse.Admin.Domain.ValueObjects
{
    /// <summary>
    /// Адрес
    /// </summary>
    public class Address
    {
        #region Свойства

        /// <summary>
        /// Город
        /// </summary>
        public virtual City City { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        public virtual Region Region
        { 
            get 
            {
                return this.City.Region;
            }
        }

        /// <summary>
        /// Почтовый индекс
        /// </summary>
        /// <remarks>6 символов</remarks>
        public virtual string PostalIndex { get; set; }

        /// <summary>
        /// Название улицы, дом, квартира
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string LocalAddress { get; set; }

        #endregion

        #region Конструкторы

        protected Address() {}

        public Address(City city, string postalIndex, string localAddress)
        {
            this.City = city;
            this.PostalIndex = postalIndex;
            this.LocalAddress = localAddress;
        }

        #endregion
    }
}
