namespace ERP.Wholesale.Domain.Entities
{
    public class ForeignBankAccount: BankAccount
    {
        #region Свойства

        /// <summary>
        /// Номер счета европейского стандарта
        /// </summary>
        public virtual string IBAN { get; set; }

        #endregion

        #region Конструкторы
        
        protected ForeignBankAccount() {}

        public ForeignBankAccount(ForeignBank bank, string number, Currency currency)
            : base(bank, number, currency)
        {
        }

        #endregion

        #region Методы
        #endregion
    }
}
