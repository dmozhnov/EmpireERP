using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Класс, используемый для построения отчета Report0004, хранящий два количества товара (в начале периода и в конце)
    /// </summary>
    public class StartEndAvailabilityItem
    {        
        public decimal StartCount { get; set; }

        public decimal EndCount { get; set; }

        public StartEndAvailabilityItem()
        {
            StartCount = 0;
            EndCount = 0;
        }
    }
}
