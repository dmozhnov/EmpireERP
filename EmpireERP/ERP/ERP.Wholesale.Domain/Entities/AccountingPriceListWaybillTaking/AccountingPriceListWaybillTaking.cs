using System;
using ERP.Infrastructure.Entities;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Связь между позицией накладной и позицией РЦ для учета переоценкой
    /// </summary>
    public class AccountingPriceListWaybillTaking : Entity<Guid>
    {
        #region Свойства

        /// <summary>
        /// Дата создания связи
        /// </summary>
        public virtual DateTime TakingDate { get; protected set; }
        
        /// <summary>
        /// Тип накладной (входящая или исходящая), с позицией которой связана данная связь
        /// </summary>
        public virtual bool IsWaybillRowIncoming { get; protected internal set; }

        /// <summary>
        /// Код позиции РЦ
        /// </summary>
        public virtual Guid ArticleAccountingPriceId { get; protected set; }

        /// <summary>
        /// Тип накладной, к которой относится позиция
        /// </summary>
        public virtual WaybillType WaybillType { get; protected set; }

        /// <summary>
        /// Код позиции накладной
        /// </summary>
        public virtual Guid WaybillRowId { get; protected set; }

        /// <summary>
        /// Код товара
        /// </summary>
        public virtual int ArticleId { get; protected set; }

        /// <summary>
        /// Код МХ
        /// </summary>
        public virtual short StorageId { get; protected set; }

        /// <summary>
        /// Код собственной организации
        /// </summary>
        public virtual int AccountOrganizationId { get; protected set; }

        /// <summary>
        /// УЦ из позиции РЦ (для упрощения расчета)
        /// </summary>
        public virtual decimal AccountingPrice { get; protected set; }

        /// <summary>
        /// Является ли связью на начало действия РЦ
        /// </summary>
        public virtual bool IsOnAccountingPriceListStart { get; protected set; }

        /// <summary>
        /// Количество товара для учета РЦ
        /// </summary>
        public virtual decimal Count { get; protected internal set; }

        /// <summary>
        /// Дата учета данной связи точной переоценкой
        /// </summary>
        public virtual DateTime? RevaluationDate { get; protected internal set; }

        #endregion

        #region Конструкторы

        protected AccountingPriceListWaybillTaking()
        {
        }

        public AccountingPriceListWaybillTaking(DateTime takingDate, bool isWaybillRowIncoming, Guid articleAccountingPriceId, WaybillType waybillType,
            Guid waybillRowId, int articleId, short storageId, int accountOrganizationId, decimal accountingPrice, bool isOnAccountingPriceListStart, decimal count)
        {
            TakingDate = takingDate;
            IsWaybillRowIncoming = isWaybillRowIncoming;
            ArticleAccountingPriceId = articleAccountingPriceId;
            WaybillType = waybillType;
            WaybillRowId = waybillRowId;
            ArticleId = articleId;
            StorageId = storageId;
            AccountOrganizationId = accountOrganizationId;
            AccountingPrice = accountingPrice;
            IsOnAccountingPriceListStart = isOnAccountingPriceListStart;
            Count = count;
        }

        #endregion
    }
}
