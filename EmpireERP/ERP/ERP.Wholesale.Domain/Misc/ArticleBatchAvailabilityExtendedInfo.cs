using System;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Наличие по партии товара на указанном МХ и организации
    /// </summary>
    public class ArticleBatchAvailabilityExtendedInfo
    {
        public short StorageId { get; private set; }
        public int AccountOrganizationId { get; private set; }
        public int ArticleId { get; private set; }
        
        public Guid ArticleBatchId { get; private set; }
        public string BatchName { get; private set; }
        public DateTime BatchDate { get; private set; }
        
        public decimal PurchaseCost { get; private set; }

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

        public byte ArticleMeasureUnitScale { get; set; }

        public ArticleBatchAvailabilityExtendedInfo(ReceiptWaybillRow articleBatch, short storageId, int accountOrganizationId, decimal availableInStorageCount,
            decimal pendingCount, decimal reservedFromExactAvailabilityCount, decimal reservedFromIncomingAcceptedAvailabilityCount)
        {
            ArticleBatchId = articleBatch.Id;
            ArticleId = articleBatch.Article.Id;
            BatchName = articleBatch.BatchName;
            PurchaseCost = articleBatch.PurchaseCost;
            BatchDate = articleBatch.ReceiptWaybill.Date;
            ArticleMeasureUnitScale = articleBatch.ArticleMeasureUnitScale;
            StorageId = storageId;
            AccountOrganizationId = accountOrganizationId;
            AvailableInStorageCount = availableInStorageCount;
            PendingCount = pendingCount;
            ReservedFromExactAvailabilityCount = reservedFromExactAvailabilityCount;
            ReservedFromIncomingAcceptedAvailabilityCount = reservedFromIncomingAcceptedAvailabilityCount;
        }
    }
}
