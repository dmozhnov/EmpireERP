using System;
using ERP.Infrastructure.Entities;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Хозяйствующий субъект
    /// </summary>
    public abstract class EconomicAgent : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Тип хозяйствующего субъекта
        /// </summary>
        public virtual EconomicAgentType Type { get; protected set; }

        /// <summary>
        /// Организационно-правовая форма
        /// </summary>
        public virtual LegalForm LegalForm
        {
            get { return legalForm; }
            set
            {
                if (value.EconomicAgentType != Type)
                {
                    throw new Exception("Организационно-правовая форма не соответствует указанному типу хозяйствующего субъекта.");
                }

                legalForm = value;
            }
        }
        protected LegalForm legalForm;

        /*public virtual EconomicAgent Prop {
            get
            {
                return (EconomicAgentType == EconomicAgentType.JuridicalPerson ? (EconomicAgent)JuridicalPerson : (EconomicAgent)PhysicalPerson);
            }
        }*/

        //public virtual JuridicalPerson JuridicalPerson { get; set; }
        //public virtual PhysicalPerson PhysicalPerson { get; set; }

        #endregion

        #region Конструкторы

        protected EconomicAgent()
        {
        }

        protected EconomicAgent(EconomicAgentType type, LegalForm legalForm)
        {
            Type = type; // Type должно быть установлено перед LegalForm
            LegalForm = legalForm;
        }

        #endregion
    }
}
