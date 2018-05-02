using System;
using System.Linq;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Позиция накладной перемещения
    /// </summary>
    public class MovementWaybillRow : BaseIncomingAndOutgoingWaybillRow
    {
        #region Свойства

        /// <summary>
        /// Накладная перемещения, частью которой является позиция
        /// </summary>
        public virtual MovementWaybill MovementWaybill { get; protected internal set; }

        /// <summary>
        /// Статус позиции исходящей накладной
        /// </summary>
        public virtual OutgoingWaybillRowState OutgoingWaybillRowState
        {
            get { return outgoingWaybillRowState; }
            protected internal set
            {
                ValidationUtils.NotNull(MovementWaybill, "Не установлено значение накладной.");

                // если накладная не отгружена и значение статуса позиции стало другим
                if (!MovementWaybill.IsShipped && outgoingWaybillRowState != value)
                {
                    outgoingWaybillRowState = value;

                    // пересчет статуса накладной имеет смысл только для проведенной накладной
                    if (MovementWaybill.IsAccepted)
                    {
                        MovementWaybill.UpdateStateByRowsState();
                    }
                }
            }
        }
        private OutgoingWaybillRowState outgoingWaybillRowState;

        /// <summary>
        /// Ставка НДС
        /// </summary>
        public virtual ValueAddedTax ValueAddedTax
        {
            get { return valueAddedTax; }
            set
            {
                ValidationUtils.Assert(value.Value == 0M || MovementWaybill.Sender != MovementWaybill.Recipient,
                    "Организации-отправитель и получатель совпадают. Невозможно установить ненулевой НДС для позиции.");
                valueAddedTax = value;
            }
        }
        private ValueAddedTax valueAddedTax;

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
        /// Сумма НДС отправителя. Имеет смысл только в случае проведенной накладной (зафиксированы учетные цены)
        /// </summary>
        public virtual decimal SenderValueAddedTaxSum
        {
            get
            {
                ValidationUtils.Assert(MovementWaybill.IsAccepted, "Зафиксированной учетной цены не существует, так как накладная еще не проведена.");

                return VatUtils.CalculateVatSum(senderArticleAccountingPrice.AccountingPrice * MovingCount, ValueAddedTax.Value);
            }
        }

        /// <summary>
        /// Сумма НДС получателя. Имеет смысл только в случае проведенной накладной (зафиксированы учетные цены)
        /// </summary>
        public virtual decimal RecipientValueAddedTaxSum
        {
            get
            {
                ValidationUtils.Assert(MovementWaybill.IsAccepted, "Зафиксированной учетной цены не существует, так как накладная еще не проведена.");

                return VatUtils.CalculateVatSum(recipientArticleAccountingPrice.AccountingPrice * MovingCount, ValueAddedTax.Value);
            }
        }

        /// <summary>
        /// Позиция реестра цен, по которой сформирована учетная цена для этой позиции на получателе
        /// </summary>
        public virtual ArticleAccountingPrice RecipientArticleAccountingPrice
        {
            get { return recipientArticleAccountingPrice; }
            protected internal set
            {
                ValidationUtils.Assert(!MovementWaybill.IsAccepted, "Реестр, по которому рассчитывается учетная цена, не может быть сменен.");
                
                recipientArticleAccountingPrice = value;
            }
        }
        private ArticleAccountingPrice recipientArticleAccountingPrice;

        /// <summary>
        /// Позиция реестра цен, по которой сформирована учетная цена для этой позиции на отправителе
        /// </summary>
        public virtual ArticleAccountingPrice SenderArticleAccountingPrice
        {
            get { return senderArticleAccountingPrice; }
            protected internal set
            {
                ValidationUtils.Assert(!MovementWaybill.IsAccepted, "Реестр, по которому рассчитывается учетная цена, не может быть сменен.");

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
                if (MovementWaybill != null && receiptWaybillRow != null &&
                    MovementWaybill.Rows.Where(x => x.ReceiptWaybillRow.Id == value.Id && x.Id != this.Id).FirstOrDefault() != null)
                {
                    throw new Exception("Позиция накладной по данной партии и товару уже добавлена.");
                }

                if (receiptWaybillRow != null && receiptWaybillRow.Id != value.Id && AreOutgoingWaybills)
                {
                    throw new Exception("Невозможно сменить партию, т.к. по данной позиции есть зарезервированные товары.");
                }

                receiptWaybillRow = value;
                article = value.Article;
            }
        }
        private ReceiptWaybillRow receiptWaybillRow;

        /// <summary>
        /// Количество перемещаемого товара
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

        /// <summary>
        /// Определены ли источники для этой позиции (установлены вручную при добавлении или назначены автоматически при проводке)
        /// </summary>
        public virtual bool AreSourcesDetermined
        { 
            get
            {
                return IsUsingManualSource || MovementWaybill.IsAccepted;
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

        protected MovementWaybillRow()
            : base(WaybillType.MovementWaybill)
        {
        }

        public MovementWaybillRow(ReceiptWaybillRow receiptWaybillRow, decimal movingCount, ValueAddedTax valueAddedTax)
            : this()
        {
            ReceiptWaybillRow = receiptWaybillRow; // Должно идти перед присваиванием количества, т.к. задает допустимое число его знаков после запятой
            MovingCount = movingCount;
            this.valueAddedTax = valueAddedTax;
            outgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
        }

        #endregion

        #region Методы

        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(MovementWaybill.IsDraft,
                String.Format("Невозможно удалить позицию из накладной со статусом «{0}».", MovementWaybill.State.GetDisplayName()));

            ValidationUtils.Assert(!AreOutgoingWaybills, "Невозможно удалить позицию, так как по ней уже создана позиция другой исходящей накладной.");
        }

        #endregion
    }
}