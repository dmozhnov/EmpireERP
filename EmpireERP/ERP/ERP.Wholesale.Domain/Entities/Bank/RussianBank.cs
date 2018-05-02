using System;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Отечественный банк
    /// </summary>
    public class RussianBank : Bank
    {
        #region Свойства

        /// <summary>
        /// БИК
        /// </summary>
        /// <remarks>9 цифр</remarks>
        public virtual string BIC { get; set; }

        /// <summary>
        /// К/с
        /// </summary>
        /// <remarks>20 цифр</remarks>
        public virtual string CorAccount { get; set; }

        #endregion

        #region Конструкторы
        
        protected RussianBank() {}

        public RussianBank(string name, string bic, string corAccount)
            :base(name)
        {
            if (string.IsNullOrEmpty(bic))
            {
                throw new Exception("Укажите БИК.");
            }

            if (string.IsNullOrEmpty(corAccount))
            {
                throw new Exception("Укажите корреспондентский счет.");
            }

            BIC = bic;
            CorAccount = corAccount;
        }

        #endregion


    }

}
