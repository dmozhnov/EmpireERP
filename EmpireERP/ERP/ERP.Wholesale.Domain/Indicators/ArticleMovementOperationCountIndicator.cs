using System;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Показатель «Количество операций по товародвижению»
    /// </summary>
    public class ArticleMovementOperationCountIndicator : BaseIndicator
    {
        #region Свойства

        /// <summary>
        /// Тип операции товародвижения
        /// </summary>
        public virtual ArticleMovementOperationType ArticleMovementOperationType { get; protected set; }

        /// <summary>
        /// Склад, по которому произведена операция
        /// </summary>
        public virtual short StorageId { get; protected set; }

        /// <summary>
        /// Кол-во операций
        /// </summary>
        public virtual int Count { get; protected internal set; }

        /// <summary>
        /// Идентификатор предыдущей записи
        /// </summary>
        public virtual Guid? PreviousId { get; protected internal set; }

        #endregion

        #region Конструкторы

        public ArticleMovementOperationCountIndicator() : base()
        {
        }

        public ArticleMovementOperationCountIndicator(DateTime startDate, ArticleMovementOperationType articleMovementOperationType, short storageId, int count)
            : base(startDate)
        {
            ArticleMovementOperationType = articleMovementOperationType;
            StorageId = storageId;
            Count = count;
        }

        #endregion

    }
}
