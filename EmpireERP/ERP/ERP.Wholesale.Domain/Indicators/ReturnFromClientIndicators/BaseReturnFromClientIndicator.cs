using System;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Базовый показатель возврата товаров от клиентов
    /// </summary>
    public abstract class BaseReturnFromClientIndicator : BaseIndicator
    {
        #region Свойства

        /// <summary>
        /// Код товара
        /// </summary>
        public virtual int ArticleId { get; set; }

        /// <summary>
        /// Код куратора накладной реализации
        /// </summary>
        public virtual int SaleWaybillCuratorId { get; set; }

        /// <summary>
        /// Код куратора накладной возврата от клиента
        /// </summary>
        public virtual int ReturnFromClientWaybillCuratorId { get; set; }

        /// <summary>
        /// Код поставщика/производителя
        /// </summary>
        public virtual int ContractorId { get; set; }

        /// <summary>
        /// Код МХ, с которого была выполнена реализация
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
        /// Код сделки
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
        /// Идентификатор накладной реализации, с которой происходит возврат
        /// </summary>
        public virtual Guid SaleWaybillId { get; set; }

        /// <summary>
        /// Идентификатор партии товара
        /// </summary>
        public virtual Guid BatchId { get; set; }

        /// <summary>
        /// Возвращенное кол-во
        /// </summary>
        public virtual decimal ReturnedCount { get; set; }

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
                if (PurchaseCostSum == 0 && AccountingPriceSum == 0 && SalePriceSum == 0 && ReturnedCount == 0)
                    return true;

                return false;
            }
        }

        #endregion

        #region Конструкторы

        protected internal BaseReturnFromClientIndicator() : base()
        {
        }

        protected internal BaseReturnFromClientIndicator(DateTime startDate, int articleId, int saleWaybillCuratorId, int returnFromClientWaybillCuratorId, 
            int contractorId, short storageId, int accountOrganizationId, int clientId, int dealId, int clientOrganizationId, short teamId, Guid saleWaybillId, 
            Guid batchId, decimal purchaseCostSum, decimal accountingPriceSum, decimal salePriceSum, decimal returnedCount)
            : base(startDate)
        {
            AccountingPriceSum = accountingPriceSum;
            ArticleId = articleId;
            SaleWaybillCuratorId = saleWaybillCuratorId;
            ReturnFromClientWaybillCuratorId = returnFromClientWaybillCuratorId;
            ContractorId = contractorId;
            StorageId = storageId;
            AccountOrganizationId = accountOrganizationId;
            ClientId = clientId;
            DealId = dealId;
            ClientOrganizationId = clientOrganizationId;
            TeamId = teamId;
            PurchaseCostSum = purchaseCostSum;
            SalePriceSum = salePriceSum;
            SaleWaybillId = saleWaybillId;
            BatchId = batchId;
            ReturnedCount = returnedCount;
        }

        #endregion
    }
}
