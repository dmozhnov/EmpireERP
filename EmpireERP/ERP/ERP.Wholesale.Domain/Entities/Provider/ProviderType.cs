namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип поставщика
    /// </summary>
    public class ProviderType : BaseDictionary
    {
        #region Свойства

        #endregion

        #region Конструкторы


        public ProviderType()
            : base()
        {
        }

        public ProviderType(string name)
            : base(name)
        {
        }

        #endregion
    }
}