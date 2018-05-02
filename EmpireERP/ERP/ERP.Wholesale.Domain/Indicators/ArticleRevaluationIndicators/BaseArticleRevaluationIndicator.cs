using System;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Базовый показатель переоценки товаров
    /// </summary>
    public abstract class BaseArticleRevaluationIndicator : BaseIndicator
    {
        #region Свойства

        /// <summary>
        /// Код МХ, по которому происходит изменение УЦ
        /// </summary>
        public virtual short StorageId { get; protected internal set; }

        /// <summary>
        /// Код собственной организации, по которой происходит изменение УЦ
        /// </summary>
        public virtual int AccountOrganizationId { get; protected internal set; }

        /// <summary>
        /// Сумма переоценки
        /// </summary>
        public virtual decimal RevaluationSum { get; protected internal set; }

        /// <summary>
        /// Идентификатор предыдущей записи
        /// </summary>
        public virtual Guid? PreviousId { get; protected internal set; }

        #endregion

        #region Конструкторы

        protected internal BaseArticleRevaluationIndicator()
        {
        }

        protected internal BaseArticleRevaluationIndicator(DateTime startDate, short storageId, int accountOrganizationId, decimal revaluationSum) 
            : base(startDate)
        {
            StorageId = storageId;
            AccountOrganizationId = accountOrganizationId;
            RevaluationSum = revaluationSum;
        }

        #endregion
    }
}
