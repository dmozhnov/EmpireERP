using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizpulse.Admin.Domain.ValueObjects;

namespace Bizpulse.Admin.Domain.Entities
{
    /// <summary>
    /// Клиент - физическое лицо
    /// </summary>
    public class PhysicalPerson : Client
    {
        #region Свойства

        /// <summary>
        /// Фамилия
        /// </summary>
        /// <remarks>не более 100</remarks>
        public virtual string LastName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        /// <remarks>не более 100</remarks>
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        /// <remarks>не более 100</remarks>
        public virtual string Patronymic { get; set; }

        /// <summary>
        /// ИНН физического лица
        /// </summary>
        /// <remarks>12 символов</remarks>
        public virtual string INN { get; set; }

        /// <summary>
        /// ОГРНИП
        /// </summary>
        /// <remarks>15 символов, не обязательное</remarks>
        public virtual string OGRNIP { get; set; }

        /// <summary>
        /// Адрес регистрации
        /// </summary>
        public virtual Address RegistrationAddress { get; set; }

        /// <summary>
        /// Отображаемое имя
        /// </summary>        
        public override string DisplayName 
        { 
            get 
            {
                return string.Format("ИП {0} {1}.{2}.", LastName, FirstName.Substring(0, 1), Patronymic.Substring(0, 1)); 
            }
        }

        #endregion

        #region Конструкторы

        protected PhysicalPerson()
        {            
        }

        public PhysicalPerson(string lastName, string firstName, string patronymic, string inn, Address registrationAddress, 
            Address postalAddress, DateTime currentDate)
            : base(ClientType.PhysicalPerson, postalAddress, currentDate)
        {
            LastName = lastName;
            FirstName = firstName;
            Patronymic = patronymic;
            INN = inn;
            RegistrationAddress = registrationAddress;
        }

        #endregion

        #region Методы

        #endregion
    }
}
