using System;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Базовый показатель реализаций
    /// </summary>
    public abstract class BaseSaleIndicator : BaseIndicator
    {
        #region Свойства

        /// <summary>
        /// Код товара
        /// </summary>
        public virtual int ArticleId { get; set; }

        /// <summary>
        /// Код партии товара 
        /// </summary>
        public virtual Guid BatchId { get; set; }

        /// <summary>
        /// Код куратора накладной реализации
        /// </summary>
        public virtual int UserId { get; set; }

        /// <summary>
        /// Код поставщика/производителя
        /// </summary>
        public virtual int ContractorId { get; set; }

        /// <summary>
        /// Код МХ
        /// </summary>
        public virtual short StorageId { get; set; }

        /// <summary>
        /// Код собственной организации
        /// </summary>
        public virtual int AccountOrganizationId { get; set; }

        /// <summary>
        /// Код клиента
        /// </summary>
        public virtual int ClientId { get; set; }

        /// <summary>
        /// Код сделки, в рамках которой совершена реализация
        /// </summary>
        public virtual int DealId { get; set; }

        /// <summary>
        /// Код организации клиента
        /// </summary>
        public virtual int ClientOrganizationId { get; set; }

        /// <summary>
        /// Код команды
        /// </summary>
        public virtual short TeamId { get; set; }

        /// <summary>
        /// Сумма в закупочных ценах
        /// </summary>
        public virtual decimal PurchaseCostSum { get; set; }

        /// <summary>
        /// Сумма в учетных ценах
        /// </summary>
        public virtual decimal AccountingPriceSum { get; set; }

        /// <summary>
        /// Сумма в отпускных ценах
        /// </summary>
        public virtual decimal SalePriceSum { get; set; }

        /// <summary>
        /// Реализованное количество
        /// </summary>
        public virtual decimal SoldCount { get; set; }

        /// <summary>
        /// Идентификатор предыдущей записи
        /// </summary>
        public virtual Guid? PreviousId { get; set; }

        /// <summary>
        /// Равны ли все суммы показателя 0
        /// </summary>
        /// <returns></returns>
        public virtual bool CountersAreZero
        {
            get
            {
                if (PurchaseCostSum == 0 && AccountingPriceSum == 0 && SalePriceSum == 0 && SoldCount == 0)
                    return true;

                return false;
            }
        }

        #endregion

        #region Конструкторы

        protected internal BaseSaleIndicator() : base()
        {
        }

        protected internal BaseSaleIndicator(DateTime startDate, int articleId, int userId, int contractorId, short storageId, int accountOrganizationId, 
            int clientId, int dealId, int clientOrganizationId, short teamId, Guid batchId, decimal purchaseCostSum, decimal accountingPriceSum, 
            decimal salePriceSum, decimal soldCount) : base(startDate)
        {
            
            ArticleId = articleId;
            BatchId = batchId;
            UserId = userId;
            ContractorId = contractorId;
            StorageId = storageId;
            AccountOrganizationId = accountOrganizationId;
            ClientId = clientId;
            DealId = dealId;
            ClientOrganizationId = clientOrganizationId;
            TeamId = teamId;

            PurchaseCostSum = purchaseCostSum;
            AccountingPriceSum = accountingPriceSum;
            SalePriceSum = salePriceSum;
            SoldCount = soldCount;
        }

        #endregion
    }
}
