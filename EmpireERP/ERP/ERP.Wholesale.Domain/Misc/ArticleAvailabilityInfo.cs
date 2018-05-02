using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.Domain.Misc
{
    public class ArticleAvailabilityInfo
    {
        public short StorageId { get; set; }
        public int AccountOrganizationId { get; set; }
        public int ArticleId { get; set; }
        public Guid BatchId { get; set; }
        public decimal PurchaseCost { get; set; }

        /// <summary>
        /// Доступное на складе кол-во (точное наличие)
        /// </summary>
        public decimal AvailableInStorageCount { get; set; }

        /// <summary>
        /// Кол-во в ожидании (проведенное входящее начичие)
        /// </summary>
        public decimal PendingCount { get; set; }

        /// <summary>
        /// Кол-во, зарезервированное из точного наличия
        /// </summary>
        public decimal ReservedFromExactAvailabilityCount { get; set; }

        /// <summary>
        /// Кол-во, зарезервированное из ожидания (проведенного входящего наличия)
        /// </summary>
        public decimal ReservedFromIncomingAcceptedAvailabilityCount { get; set; }

        /// <summary>
        /// Общее зарезервированное кол-во
        /// </summary>
        public decimal ReservedCount
        {
            get { return ReservedFromExactAvailabilityCount + ReservedFromIncomingAcceptedAvailabilityCount; }
        }

        /// <summary>
        /// Доступно для резервирования из точного наличия (со склада)
        /// </summary>
        public decimal AvailableToReserveFromStorageCount
        {
            get { return AvailableInStorageCount - ReservedFromExactAvailabilityCount; }
        }

        /// <summary>
        /// Доступно для резервирования из ожидания
        /// </summary>
        public decimal AvailableToReserveFromPendingCount
        {
            get { return PendingCount - ReservedFromIncomingAcceptedAvailabilityCount; }
        }

        /// <summary>
        /// Общее доступное для резервирования кол-во
        /// </summary>
        public decimal AvailableToReserveCount
        {
            get { return AvailableToReserveFromStorageCount + AvailableToReserveFromPendingCount; }
        }

        public ArticleAvailabilityInfo(Guid batchId, int articleId, decimal purchaseCost, short storageId, int accountOrganizationId, decimal availableInStorageCount,
        decimal pendingCount, decimal reservedFromExactAvailabilityCount, decimal reservedFromIncomingAcceptedAvailabilityCount)
        {
            BatchId = batchId;
            ArticleId = articleId;
            PurchaseCost = purchaseCost;
            StorageId = storageId;
            AccountOrganizationId = accountOrganizationId;
            AvailableInStorageCount = availableInStorageCount;
            PendingCount = pendingCount;
            ReservedFromExactAvailabilityCount = reservedFromExactAvailabilityCount;
            ReservedFromIncomingAcceptedAvailabilityCount = reservedFromIncomingAcceptedAvailabilityCount;
        }
    }
}
