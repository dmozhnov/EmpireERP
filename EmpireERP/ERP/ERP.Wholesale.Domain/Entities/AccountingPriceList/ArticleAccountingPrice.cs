using System;
using ERP.Infrastructure.Entities;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Товар с ценой (элемент реестра цен)
    /// </summary>
    public class ArticleAccountingPrice : Entity<Guid>
    {
        #region Свойства

        /// <summary>
        /// Реестр, к которому относится товар
        /// </summary>
        public virtual AccountingPriceList AccountingPriceList { get; protected internal set; }

        /// <summary>
        /// Товар
        /// </summary>
        public virtual Article Article
        {
            get { return article; }
            set
            {
                if (value == null)
                {
                    throw new Exception("Товар не указан.");
                }
                article = value;
            }
        }
        private Article article;

        /// <summary>
        /// Учетная цена
        /// </summary>
        public virtual decimal AccountingPrice
        {
            get { return accountingPrice; }
            set
            {
                accountingPrice = Math.Round(value, 6);
            }
        }
        private decimal accountingPrice;

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public virtual DateTime? DeletionDate
        {
            get { return deletionDate; }
            set { if (deletionDate == null && value != null) deletionDate = value; }   // запрещаем повторную пометку об удалении
        }
        protected DateTime? deletionDate;

        /// <summary>
        /// true, если заданное правило расчета учетной цены применить не удалось и было использовано правило по умолчанию
        /// </summary>
        public virtual bool ErrorAccountingPriceCalculation { get; set; }

        /// <summary>
        /// true, если заданное правило расчета последней цифры применить не удалось и было использовано правило по умолчанию
        /// </summary>
        public virtual bool ErrorLastDigitCalculation { get; set; }

        /// <summary>
        /// Порядковый номер для сортировки
        /// </summary>
        public virtual int OrdinalNumber { get; protected set; }

        /// <summary>
        /// Признак: была ли перекрыта данная позиция РЦ при переоценке на конец периода действия РЦ
        /// </summary>
        public virtual bool IsOverlappedOnEnd { get; protected internal set; }

        #endregion

        #region Конструкторы

        protected ArticleAccountingPrice()
        {
        }

        public ArticleAccountingPrice(Article article, decimal accountingPrice, int ordinalNumber)
        {
            CreationDate = DateTime.Now;
            Article = article;
            AccountingPrice = accountingPrice;
            OrdinalNumber = ordinalNumber;
            IsOverlappedOnEnd = false;
        }

        public ArticleAccountingPrice(Article article, decimal accountingPrice) : this(article, accountingPrice, int.MaxValue)
        {
        }

        #endregion

        #region Методы

        #region Проверки на возможность выполнения операций

        #region Удаление

        public virtual void CheckPossibilityToDelete()
        {
            AccountingPriceList.CheckPossibilityToDeleteRow();
        }

        public virtual void CheckPossibilityToEdit()
        {
            AccountingPriceList.CheckPossibilityToEditRow();
        }

        #endregion

        #endregion

        #endregion
    }
}
