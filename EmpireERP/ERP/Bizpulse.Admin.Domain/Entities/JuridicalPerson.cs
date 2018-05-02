using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizpulse.Admin.Domain.ValueObjects;

namespace Bizpulse.Admin.Domain.Entities
{
    /// <summary>
    /// Клиент - юридическое лицо
    /// </summary>
    public class JuridicalPerson : Client
    {
        #region Свойства

        /// <summary>
        /// Краткое наименование организации
        /// </summary>
        /// <remarks>100 символов</remarks>
        public virtual string ShortName { get; set; }
        
        /// <summary>
        /// Юридический адрес
        /// </summary>
        public virtual Address JuridicalAddress { get; set; }
        
        /// <summary>
        /// ИНН
        /// </summary>
        /// <remarks>10 символов</remarks>
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
        /// Должность руководителя
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string DirectorPost { get; set; }

        /// <summary>
        /// ФИО руководителя
        /// <remarks>не более 100 символов</remarks>
        /// </summary>
        public virtual string DirectorName { get; set; }

        /// <summary>
        /// E-mail руководителя
        /// </summary>
        /// <remarks>не более 50 символов, не обязательное</remarks>
        public virtual string DirectorEmail { get; set; }

        /// <summary>
        /// Отображаемое имя
        /// </summary>        
        public override string DisplayName { get { return ShortName; } }

        #endregion

        #region Конструкторы

        protected JuridicalPerson()
        {
        }

        public JuridicalPerson(string shortName, Address juridicalAddress, Address postalAddress, string inn, string directorPost, 
            string directorName, DateTime currentDate)
            : base(ClientType.JuridicalPerson, postalAddress, currentDate)
        {
            ShortName = shortName;
            JuridicalAddress = juridicalAddress;
            INN = inn;
            DirectorPost = directorPost;
            DirectorName = directorName;
        }

        #endregion

        #region Методы

        #endregion
    }
}
