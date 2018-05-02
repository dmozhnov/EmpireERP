using System;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Вспомогательная сущность, используемая для передачи информации о выбранных источниках для позиции исходящей накладной
    /// </summary>
    public class WaybillRowManualSource
    {
        /// <summary>
        /// Количество, отружаемое из входящей позиции
        /// </summary>
        public decimal Count { get; set; }

        /// <summary>
        /// Идентификатор источника (позиции входящей накладной)
        /// </summary>
        public Guid WaybillRowId { get; set; }

        /// <summary>
        /// Тип входящей накладной-источника
        /// </summary>
        public WaybillType WaybillType { get; set; }
    }
}
