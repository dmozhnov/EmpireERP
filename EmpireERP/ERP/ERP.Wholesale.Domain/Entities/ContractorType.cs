using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип контрагента
    /// </summary>
    public enum ContractorType : byte
    {
        /// <summary>
        /// Поставщик
        /// </summary>
        [EnumDisplayName("Поставщик")]
        Provider = 1,

        /// <summary>
        /// Клиент
        /// </summary>
        [EnumDisplayName("Клиент")]
        Client,

        /// <summary>
        /// Производитель
        /// </summary>
        [EnumDisplayName("Производитель")]
        Producer
    }
}
