using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Ставка НДС
    /// </summary>
    public class ValueAddedTax : BaseDictionary
    {
        #region Свойства
        
        /// <summary>
        /// Значение в %
        /// </summary>
        public virtual decimal Value
        { 
            get { return _value; } 
            protected set
            {
                ValidationUtils.Assert(value >= 0 && value <= 100, "Значение ставки НДС должно быть от 0 до 100.");
                _value = value;
            }
        }
        private decimal _value;

        /// <summary>
        /// Используется по умолчанию
        /// </summary>
        public virtual bool IsDefault { get; set; }

        #endregion

        #region Конструкторы

        public ValueAddedTax()
        {
            Name = "";
            IsDefault = false;
        }

        public ValueAddedTax(string name, decimal value) : base(name)
	    {
            Value = value;
        }

        public ValueAddedTax(string name, decimal value, bool isDefault) : this(name, value)
        {
            IsDefault = isDefault;            
        }

        #endregion
    }
}
