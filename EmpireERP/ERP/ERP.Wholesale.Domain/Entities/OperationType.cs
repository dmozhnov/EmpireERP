using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип операции
    /// </summary>
    public enum OperationType
    {
        // операции с приходной накладной
        ReceiptWaybillAdding = 1,
        ReceiptWaybillEditing,
        ReceiptWaybillDeleting
    }
}
