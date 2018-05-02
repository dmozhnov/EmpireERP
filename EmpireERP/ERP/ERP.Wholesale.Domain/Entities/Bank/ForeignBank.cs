using System;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Иностранный банк
    /// </summary>
    public class ForeignBank : Bank
    {
        #region Свойства
        
        /// <summary>
        /// Тип клирингового счета
        /// </summary>
        public virtual ClearingCodeType? ClearingCodeType { get; set; }

        /// <summary>
        /// Клиринговый код
        /// </summary>
        public virtual string ClearingCode { get; set; }

        /// <summary>
        /// SWIFT-код
        /// </summary>
        public virtual string SWIFT { get; set; }

        #endregion

        #region Конструкторы
        
        protected ForeignBank() {}

        public ForeignBank(string name, string swift)
            :base(name)
        {
            if (String.IsNullOrEmpty(swift))
            {
                throw new Exception("Укажите SWIFT-код.");
            }

            SWIFT = swift;
        }

        #endregion

        #region Методы

        #endregion
    }
}
