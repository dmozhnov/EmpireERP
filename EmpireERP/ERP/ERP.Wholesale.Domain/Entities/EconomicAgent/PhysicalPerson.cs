using System;
using ERP.Wholesale.Domain.ValueObjects;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Хозяйствующий субъект - физическое лицо
    /// </summary>
    public class PhysicalPerson : EconomicAgent
    {
        #region Свойства

        /// <summary>
        /// ФИО физического лица
        /// </summary>
        /// <remarks>не более 100 символов, не обязательное</remarks>
        public virtual string OwnerName { get; set; }

        /// <summary>
        /// Паспортные данные физического лица
        /// </summary>
        public virtual PassportInfo Passport { get; set; }

        /// <summary>
        /// ИНН физического лица
        /// </summary>
        /// <remarks>12 символов, не обязательное</remarks>
        public virtual string INN { get; set; }

        /// <summary>
        /// ОГРНИП
        /// </summary>
        /// <remarks>15 символов, не обязательное</remarks>
        public virtual string OGRNIP { get; set; }

        #endregion

        #region Конструкторы

        protected PhysicalPerson()
        {
        }

        public PhysicalPerson(LegalForm legalForm)
            : base(EconomicAgentType.PhysicalPerson, legalForm)
        {
            Passport = new PassportInfo();
            OwnerName = String.Empty;
            INN = String.Empty;
        }

        #endregion
    }
}
