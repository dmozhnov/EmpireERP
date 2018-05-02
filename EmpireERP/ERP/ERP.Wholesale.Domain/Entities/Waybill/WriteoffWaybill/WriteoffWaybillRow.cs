using System;
using System.Linq;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Позиция накладной списания
    /// </summary>
    public class WriteoffWaybillRow : BaseOutgoingWaybillRow
    {
        #region Свойства

        /// <summary>
        /// Накладная списания, частью которой является позиция
        /// </summary>
        public virtual WriteoffWaybill WriteoffWaybill { get; protected internal set; }

        /// <summary>
        /// Статус позиции исходящей накладной
        /// </summary>
        public virtual OutgoingWaybillRowState OutgoingWaybillRowState
        {
            get { return outgoingWaybillRowState; }
            protected internal set
            {
                ValidationUtils.NotNull(WriteoffWaybill, "Не установлено значение накладной.");

                // если накладная не отгружена и значение статуса позиции стало другим
                if (WriteoffWaybill.State != WriteoffWaybillState.Writtenoff && outgoingWaybillRowState != value)
                {
                    outgoingWaybillRowState = value;

                    // пересчет статуса накладной имеет смысл только для проведенной накладной
                    if (WriteoffWaybill.IsAccepted)
                    {
                        WriteoffWaybill.UpdateStateByRowsState();
                    }
                }
            }
        }
        private OutgoingWaybillRowState outgoingWaybillRowState;

        /// <summary>
        /// Позиция реестра цен, по которой сформирована учетная цена для этой позиции на отправителе
        /// </summary>
        public virtual ArticleAccountingPrice SenderArticleAccountingPrice
        {
            get { return senderArticleAccountingPrice; }
            protected internal set
            {
                ValidationUtils.Assert(!WriteoffWaybill.IsAccepted, "Реестр, по которому рассчитывается учетная цена, не может быть сменен.");                
                
                senderArticleAccountingPrice = value;
            }
        }
        private ArticleAccountingPrice senderArticleAccountingPrice;

        /// <summary>
        /// Партия товара
        /// </summary>
        public virtual ReceiptWaybillRow ReceiptWaybillRow
        {
            get { return receiptWaybillRow; }
            set
            {
                if (WriteoffWaybill != null && receiptWaybillRow != null &&
                    WriteoffWaybill.Rows.Where(x => x.ReceiptWaybillRow.Id == value.Id && x.Id != this.Id).FirstOrDefault() != null)
                {
                    throw new Exception("Позиция накладной по данной партии и товару уже добавлена.");
                }

                receiptWaybillRow = value;
                article = value.Article;
            }
        }
        private ReceiptWaybillRow receiptWaybillRow;

        /// <summary>
        /// Количество списываемого товара
        /// </summary>
        /// <remarks>вещественное (18, 6)</remarks>
        public virtual decimal WritingoffCount
        {
            get { return writingoffCount; }
            set
            {
                ValidationUtils.CheckDecimalScale(value, ReceiptWaybillRow.ArticleMeasureUnitScale,
                    "Количество товара имеет слишком большое число цифр после запятой.");
                writingoffCount = value;
            }
        }
        private decimal writingoffCount;

        /// <summary>
        /// Определены ли источники для этой позиции (установлены вручную при добавлении или назначены автоматически при проводке).
        /// </summary>
        public virtual bool AreSourcesDetermined
        {
            get
            {
                return IsUsingManualSource || WriteoffWaybill.IsAccepted;
            }
        }

        /// <summary>
        /// Количество товара по позиции
        /// </summary>
        protected override decimal ArticleCount
        {
            get { return writingoffCount; }
        }

        #endregion

        #region Конструкторы

        protected WriteoffWaybillRow()
            : base(WaybillType.WriteoffWaybill)
        {
        }

        public WriteoffWaybillRow(ReceiptWaybillRow receiptWaybillRow, decimal writingoffCount) : this()
        {
            ValidationUtils.NotNull(receiptWaybillRow, "Не указана партия товара.");
            ValidationUtils.Assert(writingoffCount > 0, "Количество списываемого товара должно быть положительным числом.");

            outgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
            ReceiptWaybillRow = receiptWaybillRow; // Должно идти перед присваиванием количества, т.к. задает допустимое число его знаков после запятой
            WritingoffCount = writingoffCount;
        }

        #endregion

        #region Методы

        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(WriteoffWaybill.IsDraft,
                String.Format("Невозможно удалить позицию из накладной со статусом «{0}».", WriteoffWaybill.State.GetDisplayName()));            
        }

        #endregion
    }
}
