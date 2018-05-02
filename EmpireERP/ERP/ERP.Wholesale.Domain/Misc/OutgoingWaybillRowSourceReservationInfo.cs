using System;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Класс, содержащий информацию и кол-ве товара в проведенном входящем и точном наличии, резервируемом исходящей накладной
    /// </summary>
    public class OutgoingWaybillRowSourceReservationInfo
    {
        /// <summary>
        /// Код позиции исходящей накладной
        /// </summary>
        public Guid RowId { get; protected set; }

        /// <summary>
        /// Кол-во товара, зарезервированного из проведенного входящего наличия (ожидания)
        /// </summary>
        public decimal ReservedFromIncomingAcceptedArticleAvailabilityCount { get; internal set; }

        /// <summary>
        /// Кол-во товара, зарезервированного из точного наличия
        /// </summary>
        public decimal ReservedFromExactArticleAvailabilityCount { get; internal set; }


        public OutgoingWaybillRowSourceReservationInfo(Guid rowId, decimal reservedFromIncomingAcceptedAvailabilityCount, decimal reservedFromExactAvailabilityCount)
        {
            RowId = rowId;
            ReservedFromIncomingAcceptedArticleAvailabilityCount = reservedFromIncomingAcceptedAvailabilityCount;
            ReservedFromExactArticleAvailabilityCount = reservedFromExactAvailabilityCount;
        }
    }
}
