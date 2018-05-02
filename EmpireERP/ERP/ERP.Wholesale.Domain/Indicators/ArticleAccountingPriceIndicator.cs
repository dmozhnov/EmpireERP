using System;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Показатель «Учетная цена товара на месте хранения»
    /// </summary>
    public class ArticleAccountingPriceIndicator : BaseIndicator
    {
        #region Свойства
               
        /// <summary>
        /// Код места хранения
        /// </summary>
        public virtual short StorageId { get; set; }

        /// <summary>
        /// Код товара
        /// </summary>
        public virtual int ArticleId { get; set; }

        /// <summary>
        /// Код реестра цен
        /// </summary>
        public virtual Guid AccountingPriceListId { get; set; }

        /// <summary>
        /// Код позиции реестра цен
        /// </summary>
        public virtual Guid ArticleAccountingPriceId { get; set; }

        /// <summary>
        /// Значение учетной цены
        /// </summary>
        public virtual decimal AccountingPrice { get; set; }

        #endregion

        #region Конструкторы

        protected ArticleAccountingPriceIndicator() : base()
        {
        }

        public ArticleAccountingPriceIndicator(DateTime startDate, DateTime? endDate, short storageId, int articleId, Guid accountingPriceListId, Guid articleAccountingPriceId, decimal accountingPrice)
            : base(startDate)
        {
            StorageId = storageId;
            ArticleId = articleId;
            AccountingPriceListId = accountingPriceListId;
            ArticleAccountingPriceId = articleAccountingPriceId;
            AccountingPrice = accountingPrice;

            EndDate = endDate;
        }

        #endregion
        
    }
}
