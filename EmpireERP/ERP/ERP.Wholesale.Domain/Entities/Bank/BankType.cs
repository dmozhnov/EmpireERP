using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.Domain.Entities
{
    public enum BankType: byte
    {
        /// <summary>
        /// Отечественный банк
        /// </summary>
        Bank = 1,

        /// <summary>
        /// Зарубежный банк
        /// </summary>
        ForeignBank
    }
}
