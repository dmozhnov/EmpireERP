using System;
using System.Linq;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Позиция накладной смены собственника
    /// </summary>
    public class ChangeOwnerWaybillRow : BaseIncomingAndOutgoingWaybillRow
    {
        #region Свойства

        /// <summary>
        /// Накладная смены собственника
        /// </summary>
        public virtual ChangeOwnerWaybill ChangeOwnerWaybill { get; protected internal set; }

        /// <summary>
        /// Статус позиции исходящей накладной
        /// </summary>
        public virtual OutgoingWaybillRowState OutgoingWaybillRowState
        {
            get { return outgoingWaybillRowState; }
            protected internal set
            {
                ValidationUtils.NotNull(ChangeOwnerWaybill, "Не установлено значение накладной.");

                // если накладная не отгружена и значение статуса позиции стало другим
                if (!ChangeOwnerWaybill.IsOwnerChanged && outgoingWaybillRowState != value)
                {
                    outgoingWaybillRowState = value;

                    // пересчет статуса накладной имеет смысл только для проведенной накладной
                    if (ChangeOwnerWaybill.IsAccepted)
                    {
                        ChangeOwnerWaybill.UpdateStateByRowsState();
                    }
                }
            }
        }
        private OutgoingWaybillRowState outgoingWaybillRowState;

        /// <summary>
        /// Позиция реестра цен, по которой сформирована учетная цена для этой позиции
        /// </summary>
        public virtual ArticleAccountingPrice ArticleAccountingPrice
        {
            get { return articleAccountingPrice; }
            protected internal set
            {
                ValidationUtils.Assert(!ChangeOwnerWaybill.IsAccepted, "Реестр, по которому рассчитывается учетная цена, не может быть сменен.");

                articleAccountingPrice = value;
            }
        }
        private ArticleAccountingPrice articleAccountingPrice;

        /// <summary>
        /// Партия товара
        /// </summary>
        public virtual ReceiptWaybillRow ReceiptWaybillRow
        {
            get { return receiptWaybillRow; }
            set
            {
                if (ChangeOwnerWaybill != null && receiptWaybillRow != null &&
                    ChangeOwnerWaybill.Rows.Where(x => x.ReceiptWaybillRow.Id == value.Id && x.Id != this.Id).FirstOrDefault() != null)
                {
                    throw new Exception("Позиция накладной по данной партии товара уже добавлена.");
                }

                receiptWaybillRow = value;
                article = value.Article;
            }
        }
        private ReceiptWaybillRow receiptWaybillRow;

        /// <summary>
        /// Перемещаемое количество
        /// </summary>
        public virtual decimal MovingCount
        {
            get { return movingCount; }
            set
            {
                ValidationUtils.Assert(value >= TotallyReservedCount,
                    String.Format("Количество товара не может быть меньше {0}, т.к. товар участвует в исходящих накладных.", TotallyReservedCount));

                ValidationUtils.CheckDecimalScale(value, ReceiptWaybillRow.ArticleMeasureUnitScale,
                    "Количество товара имеет слишком большое число цифр после запятой.");
                movingCount = value;
            }
        }
        private decimal movingCount;

        /// <summary>
        /// Ставка НДС
        /// </summary>
        public virtual ValueAddedTax ValueAddedTax { get; set; }

        /// <summary>
        /// Сумма НДС. Имеет смысл только в случае проведенной накладной (зафиксированы учетные цены)
        /// </summary>
        public virtual decimal ValueAddedTaxSum
        {
            get
            {
                ValidationUtils.Assert(ChangeOwnerWaybill.IsAccepted, "Зафиксированной учетной цены не существует, так как накладная еще не проведена.");

                return VatUtils.CalculateVatSum(ArticleAccountingPrice.AccountingPrice * MovingCount, ValueAddedTax.Value);
            }
        }

        /// <summary>
        /// Сумма НДС в закупочной цене
        /// </summary>
        public virtual decimal PurchaseCostValueAddedTaxSum
        {
            get
            {
                return VatUtils.CalculateVatSum(ReceiptWaybillRow.PurchaseCost * MovingCount, ValueAddedTax.Value);
            }
        }

        /// <summary>
        /// Определены ли источники для этой позиции (установлены вручную при добавлении или назначены автоматически при проводке).
        /// </summary>
        public virtual bool AreSourcesDetermined
        {
            get
            {
                return IsUsingManualSource || ChangeOwnerWaybill.IsAccepted;
            }
        }

        /// <summary>
        /// Количество товара по позиции
        /// </summary>
        protected override decimal ArticleCount
        {
            get { return movingCount; }
        }

        #endregion

        #region Конструкторы

        protected ChangeOwnerWaybillRow()
            : base(WaybillType.ChangeOwnerWaybill)
        {
        }

        public ChangeOwnerWaybillRow(ReceiptWaybillRow receiptWaybillRow, decimal movingCount, ValueAddedTax valueAddedTax)
            : this()
        {
            outgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
            ReceiptWaybillRow = receiptWaybillRow; // Должно идти перед присваиванием количества, т.к. задает допустимое число его знаков после запятой
            MovingCount = movingCount;
            ValueAddedTax = valueAddedTax;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Одновременная установка количеств исходящего товара (зарезервированного, отгруженного и окончательно перемещенного).
        /// Если они некорректно заданы, происходит исключение.
        /// </summary>
        public virtual void SetOutgoingArticleCount(decimal acceptedCount, decimal shippedCount, decimal finallyMovedCount)
        {
            if (acceptedCount < 0 || shippedCount < 0 || finallyMovedCount < 0)
            {
                throw new Exception("Количество проведенного, отгруженного или окончательно перемещенного товара не может быть меньше 0.");
            }

            if (acceptedCount + shippedCount + finallyMovedCount > movingCount)
            {
                throw new Exception("Сумма проведенного, отгруженного и окончательно перемещенного товара не может быть больше кол-ва товара, указанного в позиции.");
            }

            this.acceptedCount = acceptedCount;
            this.shippedCount = shippedCount;
            this.finallyMovedCount = finallyMovedCount;

            this.AvailableToReserveCount = movingCount - acceptedCount - shippedCount - finallyMovedCount;
        }

        #region Проверки на возможность выполнения операций

        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(ChangeOwnerWaybill.IsDraft, String.Format("Невозможно удалить позицию из накладной со статусом «{0}».", ChangeOwnerWaybill.State.GetDisplayName()));

            ValidationUtils.Assert(!AreOutgoingWaybills, "Невозможно удалить позицию, которая участвует в дальнейшем товародвижении.");
        }

        public virtual void CheckPossibilityToEdit()
        {
            ChangeOwnerWaybill.CheckPossibilityToEditRow();
        }

        #endregion

        #endregion
    }
}
