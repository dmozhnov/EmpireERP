using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;
using System;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Краткая информация о наличии товара по партиям
    /// </summary>
    public class ArticleBatchAvailabilityShortInfo
    {
        /// <summary>
        /// Код партии товара
        /// </summary>
        public Guid BatchId { get; private set; }

        /// <summary>
        /// Код собственной организации
        /// </summary>
        public int AccountOrganizationId { get; private set; }

        /// <summary>
        /// Код товара
        /// </summary>
        public int ArticleId { get; private set; }

        /// <summary>
        /// Код места хранения, на котором имеется в наличии товар из данной партии
        /// </summary>
        public short StorageId { get; private set; }

        /// <summary>
        /// Количество товара в наличии
        /// </summary>
        public decimal Count { get; set; }

        public ArticleBatchAvailabilityShortInfo(Guid batchId, int articleId, short storageId, int accountOrganizationId, decimal count)
        {
            BatchId = batchId;
            ArticleId = articleId;
            StorageId = storageId;
            AccountOrganizationId = accountOrganizationId;
            Count = count;
        }

    }
}
