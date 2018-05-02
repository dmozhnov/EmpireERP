using System;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Базовый класс для показателей наличия товаров на складе
    /// </summary>
    public abstract class ArticleAvailabilityIndicator : BaseIndicator
    {
        #region Свойства

        /// <summary>
        /// Код МХ
        /// </summary>
        public virtual short StorageId { get; set; }

        /// <summary>
        /// Код собственной организации
        /// </summary>
        public virtual int AccountOrganizationId { get; set; }

        /// <summary>
        /// Код товара
        /// </summary>
        public virtual int ArticleId { get; set; }

        /// <summary>
        /// Код партии товара 
        /// </summary>
        public virtual Guid BatchId { get; set; }

        /// <summary>
        /// Закупочная цена
        /// </summary>
        public virtual decimal PurchaseCost { get; set; }

        /// <summary>
        /// Кол-во товара
        /// </summary>
        public virtual decimal Count { get; set; }

        /// <summary>
        /// Идентификатор предыдущей записи
        /// </summary>
        public virtual Guid? PreviousId { get; set; }

        #endregion

        #region Конструкторы

        protected internal ArticleAvailabilityIndicator() : base()
        {
        }

        protected internal ArticleAvailabilityIndicator(DateTime startDate, short storageId, int accountOrganizationId, int articleId, Guid batchId, decimal purchaseCost, decimal count)
            : base(startDate)
        {
            ValidationUtils.Assert(startDate <= DateTime.Now, "Дата начала действия показателя не может быть больше текущей даты.");

            StorageId = storageId;
            AccountOrganizationId = accountOrganizationId;
            ArticleId = articleId;
            BatchId = batchId;
            PurchaseCost = purchaseCost;
            Count = count;            
        }

        #endregion      
    }
}
