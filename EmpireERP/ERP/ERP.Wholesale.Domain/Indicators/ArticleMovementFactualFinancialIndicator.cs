using System;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Финансовый показатель по товародвижению
    /// </summary>
    public class ArticleMovementFactualFinancialIndicator : BaseIndicator
    {
        #region Свойства

        #region Ключ

        /// <summary>
        /// Код организации-отправителя
        /// </summary>
        public virtual int? SenderId { get; protected set; }

        /// <summary>
        /// Код МХ-отправителя
        /// </summary>
        public virtual short? SenderStorageId { get; protected set; }

        /// <summary>
        /// Код организации-получателя
        /// </summary>
        public virtual int? RecipientId { get; protected set; }

        /// <summary>
        /// Код МХ-получателя
        /// </summary>
        public virtual short? RecipientStorageId { get; protected set; }

        /// <summary>
        /// Тип операции товародвижения
        /// </summary>
        public virtual ArticleMovementOperationType ArticleMovementOperationType { get; set; }

        /// <summary>
        /// Идентификатор предыдущей записи
        /// </summary>
        public virtual Guid? PreviousId { get; set; }

        /// <summary>
        /// Код накладной, изменившей показатель
        /// </summary>
        public virtual Guid WaybillId { get; protected set; }

        #endregion

        #region Значение

        /// <summary>
        /// Сумма в закупочных ценах
        /// </summary>
        public virtual decimal PurchaseCostSum { get; protected internal set; }

        /// <summary>
        /// Сумма в учетных ценах
        /// </summary>
        public virtual decimal AccountingPriceSum { get; protected internal set; }

        /// <summary>
        /// Сумма в отпускных ценах
        /// </summary>
        public virtual decimal SalePriceSum { get; protected internal set; }

        #endregion

        #region Вычисляемые

        /// <summary>
        /// Проверяет значения текущего показателя на ноль.
        /// Реально используется как повод для удаления показателя (см. Find All References, а также метод ChangePurchaseCosts сервиса - проверку разности
        /// с предыдущим индикатором на 0).
        /// Нужен рефакторинг, т.к. этого условия недостаточно. Например, по приходу по заказу УЦ и ЗЦ могут быть одновременно равны 0
        /// (в таком случае надо также гарантировать создание индикатора, даже с 3 нулями по всем ценам).
        /// Лучше завести еще одно поле "показатель устарел" и ставить при отмене операций.
        /// </summary>
        /// <returns></returns>
        public virtual bool CountersAreZero
        {
            get
            {
                if (this.AccountingPriceSum != 0
                    || this.PurchaseCostSum != 0
                    || this.SalePriceSum != 0)
                    return false;

                return true;
            }
        }

        #endregion

        #endregion

        #region Конструкторы

        public ArticleMovementFactualFinancialIndicator() : base()
        {
        }

        public ArticleMovementFactualFinancialIndicator(DateTime startDate, int? senderId, short? senderStorageId, int? recipientId, 
            short? recipientStorageId, ArticleMovementOperationType articleMovementOperationType, Guid waybillId,
            decimal purchaseCostSum, decimal accountingPriceSum, decimal salePriceSum)
            : base(startDate)
        {
            #region Валидация параметров

            switch (articleMovementOperationType)
            {
                case ArticleMovementOperationType.Expenditure:
                case ArticleMovementOperationType.Writeoff:
                    ValidationUtils.Assert(senderId.HasValue, "В индикаторе необходимо указать отправителя.");
                    ValidationUtils.Assert(!recipientId.HasValue, "В индикаторе нельзя указать получателя.");
                    ValidationUtils.Assert(senderStorageId.HasValue, "В индикаторе необходимо указать место хранения отправителя.");
                    ValidationUtils.Assert(!recipientStorageId.HasValue, "В индикаторе нельзя указать место хранения получателя.");
                    break;

                case ArticleMovementOperationType.ReturnFromClient:
                case ArticleMovementOperationType.Receipt:
                    ValidationUtils.Assert(!senderId.HasValue, "В индикаторе нельзя указать отправителя.");
                    ValidationUtils.Assert(recipientId.HasValue, "В индикаторе необходимо указать получателя.");
                    ValidationUtils.Assert(!senderStorageId.HasValue, "В индикаторе нельзя указать место хранения отправителя.");
                    ValidationUtils.Assert(recipientStorageId.HasValue, "В индикаторе необходимо указать место хранения получателя.");
                    break;

                case ArticleMovementOperationType.IncomingMovement:
                case ArticleMovementOperationType.OutgoingMovement:
                    ValidationUtils.Assert(senderId.HasValue, "В индикаторе необходимо указать отправителя.");
                    ValidationUtils.Assert(recipientId.HasValue, "В индикаторе необходимо указать получателя.");
                    ValidationUtils.Assert(senderStorageId.HasValue, "В индикаторе необходимо указать место хранения отправителя.");
                    ValidationUtils.Assert(recipientStorageId.HasValue, "В индикаторе необходимо указать место хранения получателя.");
                    break;
            }

            #endregion

            SenderId = senderId;
            SenderStorageId = senderStorageId;
            RecipientId = recipientId;
            RecipientStorageId = recipientStorageId;

            PurchaseCostSum = purchaseCostSum;
            AccountingPriceSum = accountingPriceSum;
            SalePriceSum = salePriceSum;
            
            ArticleMovementOperationType = articleMovementOperationType;
            WaybillId = waybillId;
        }

        #endregion
    }
}