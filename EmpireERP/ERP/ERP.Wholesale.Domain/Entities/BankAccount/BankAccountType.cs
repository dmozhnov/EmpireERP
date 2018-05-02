using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.Domain.Entities
{
    public enum BankAccountType : byte
    {
        /// <summary>
        /// Счет в отечественном банке
        /// </summary>
        BankAccount = 1,

        /// <summary>
        /// Счет в зарубежном банке
        /// </summary>
        ForeignBankAccount
    }
}
