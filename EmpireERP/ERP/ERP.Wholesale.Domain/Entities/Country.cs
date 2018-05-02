namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Страна
    /// </summary>
    public class Country : BaseDictionary
    {
        #region Свойства

        /// <summary>
        /// Цифровой код.
        /// </summary>
        public virtual string NumericCode { get; set; }

        #endregion

        #region Конструкторы

        public Country()
            : base()
        {
        }
        
        public Country(string name, string numericCode)
            : base(name)
        {
            NumericCode = numericCode;
        }

        #endregion
    }
}
