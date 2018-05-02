using System;
using ERP.Infrastructure.Entities;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Расчетный счет в банке
    /// </summary>
    public class RussianBankAccount : BankAccount
    {
        #region Свойства

        #endregion

        #region Конструкторы

        protected RussianBankAccount()
        {
        }

        public RussianBankAccount(RussianBank bank, string number, Currency currency)
            : base(bank, number, currency)
        {
            CreationDate = DateTime.Now;
        }

        #endregion

        #region Методы

        #endregion
    }
}
