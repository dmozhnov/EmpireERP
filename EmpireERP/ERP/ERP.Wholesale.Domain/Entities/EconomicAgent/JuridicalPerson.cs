using System;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Хозяйствующий субъект - юридическое лицо
    /// </summary>
    public class JuridicalPerson : EconomicAgent
    {
        #region Свойства

        /// <summary>
        /// ИНН
        /// </summary>
        /// <remarks>10 символов, не обязательное</remarks>
        public virtual string INN { get; set; }

        /// <summary>
        /// КПП
        /// </summary>
        /// <remarks>9 символов, не обязательное</remarks>
        public virtual string KPP { get; set; }

        /// <summary>
        /// ОГРН
        /// </summary>
        /// <remarks>13 символов, не обязательное</remarks>
        public virtual string OGRN { get; set; }

        /// <summary>
        /// ОКПО
        /// </summary>
        /// <remarks>8 или 10 символов, не обязательное</remarks>
        public virtual string OKPO { get; set; }

        /// <summary>
        /// ФИО директора
        /// <remarks>не более 100 символов, не обязательное</remarks>
        /// </summary>
        public virtual string DirectorName { get; set; }

        /// <summary>
        /// Должность директора
        /// </summary>
        /// <remarks>не более 100 символов, не обязательное</remarks>
        public virtual string DirectorPost { get; set; }

        /// <summary>
        /// ФИО гл. бухгалтера
        /// </summary>
        /// <remarks>не более 100 символов, не обязательное</remarks>
        public virtual string MainBookkeeperName { get; set; }

        /// <summary>
        /// ФИО кассира
        /// </summary>
        /// <remarks>не более 100 символов, не обязательное</remarks>
        public virtual string CashierName { get; set; }

        #endregion

        #region Конструкторы

        protected JuridicalPerson()
        {
        }

        public JuridicalPerson(LegalForm legalForm)
            : base(EconomicAgentType.JuridicalPerson, legalForm)
        {
            INN = String.Empty;
            KPP = String.Empty;
            OGRN = String.Empty;
            OKPO = String.Empty;
            DirectorName = String.Empty;
            DirectorPost = String.Empty;
            MainBookkeeperName = String.Empty;
            CashierName = String.Empty;
        }

        #endregion
    }
}
