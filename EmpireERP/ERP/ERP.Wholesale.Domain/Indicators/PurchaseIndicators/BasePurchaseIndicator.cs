using System;

namespace ERP.Wholesale.Domain.Indicators.PurchaseIndicators
{
    /// <summary>
    /// Базовый показатель закупок
    /// </summary>
    public abstract class BasePurchaseIndicator : BaseIndicator
    {
        #region Свойства

        /// <summary>
        /// Код товара
        /// </summary>
        public virtual int ArticleId { get; set; }

        /// <summary>
        /// Код куратора накладной закупки
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
        /// Код организации поставщика/производителя
        /// </summary>
        public virtual int ContractorOrganizationId { get; set; }

        /// <summary>
        /// Код договора
        /// </summary>
        public virtual short ContractId { get; set; }

        /// <summary>
        /// Сумма в закупочных ценах
        /// </summary>
        public virtual decimal PurchaseCostSum { get; set; }

        /// <summary>
        /// Закупленное кол-во
        /// </summary>
        public virtual decimal Count { get; set; }

        /// <summary>
        /// Идентификатор предыдущей записи
        /// </summary>
        public virtual Guid? PreviousId { get; set; }

        #endregion

        #region Конструкторы

        protected internal BasePurchaseIndicator() : base()
        {
        }

        public BasePurchaseIndicator(DateTime startDate, int articleId, int userId, int contractorId, short storageId, int accountOrganizationId,
            int contractorOrganizationId, short contractId, decimal purchaseCostSum,  decimal count)
            : base(startDate)
        {
            StartDate = startDate;
            ArticleId = articleId;
            UserId = userId;
            ContractorId = contractorId;
            StorageId = storageId;
            AccountOrganizationId = accountOrganizationId;
            ContractorOrganizationId = contractorOrganizationId;
            ContractId = contractId;

            PurchaseCostSum = purchaseCostSum;
            Count = count;
        }

        #endregion

    }
}
