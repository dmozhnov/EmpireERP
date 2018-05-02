namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Организационно-правовая форма
    /// </summary>
    public class LegalForm : BaseDictionary
    {
        #region Свойства

        /// <summary>
        /// Тип хозяйствующего субъекта
        /// </summary>
        public virtual EconomicAgentType EconomicAgentType { get; set; }

        #endregion

        #region Конструкторы

        public LegalForm()
        {
        }

        public LegalForm(string name, EconomicAgentType economicAgentType): 
            base(name)
        {
            EconomicAgentType = economicAgentType;
        }

        #endregion
    }
}
