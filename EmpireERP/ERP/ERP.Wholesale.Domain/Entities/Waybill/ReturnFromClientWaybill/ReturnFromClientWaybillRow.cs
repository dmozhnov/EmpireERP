using System;
using System.Linq;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Позиция накладной возврата от клиента
    /// </summary>
    public class ReturnFromClientWaybillRow : BaseIncomingWaybillRow
    {
        #region Свойства
        
        #region Основные
        
        /// <summary>
        /// Накладная возврата от клиента, частью которой является позиция
        /// </summary>
        public virtual ReturnFromClientWaybill ReturnFromClientWaybill { get; protected internal set; }

        /// <summary>
        /// Количество возвращаемого товара
        /// </summary>
        /// <remarks>вещественное (18, 6)</remarks>
        public virtual decimal ReturnCount
        {
            get { return returnCount; }
            set
            {
                ValidationUtils.Assert(value >= TotallyReservedCount,
                    String.Format("Количество товара не может быть меньше {0}, т.к. товар участвует в исходящих накладных.", TotallyReservedCount));

                ValidationUtils.CheckDecimalScale(value, SaleWaybillRow.ArticleMeasureUnitScale,
                    "Количество товара имеет слишком большое число цифр после запятой.");
                
                returnCount = value;
            }
        }
        private decimal returnCount;

        /// <summary>
        /// Позиция накладной реализации товара
        /// </summary>
        public virtual SaleWaybillRow SaleWaybillRow
        {
            get { return saleWaybillRow; }
            set
            {
                if (ReturnFromClientWaybill != null && saleWaybillRow != null &&
                    ReturnFromClientWaybill.Rows.Where(x => x.SaleWaybillRow.Id == value.Id && x.Id != this.Id).FirstOrDefault() != null)
                {
                    throw new Exception("Позиция накладной по данной партии и товару уже добавлена.");
                }

                saleWaybillRow = value;
                receiptWaybillRow = saleWaybillRow.As<ExpenditureWaybillRow>().ReceiptWaybillRow;
                article = value.Article;
            }
        }
        private SaleWaybillRow saleWaybillRow;

        /// <summary>
        /// Партия товара
        /// </summary>
        public virtual ReceiptWaybillRow ReceiptWaybillRow
        {
            get { return receiptWaybillRow; }
        }       
        private ReceiptWaybillRow receiptWaybillRow;

        /// <summary>
        /// Ставка НДС
        /// </summary>
        public virtual ValueAddedTax ValueAddedTax
        {
            get { return saleWaybillRow.ValueAddedTax; }
        }

        /// <summary>
        /// Сумма НДС в закупочной цене
        /// </summary>
        public virtual decimal PurchaseCostValueAddedTaxSum
        {
            get
            {
                return VatUtils.CalculateVatSum(ReceiptWaybillRow.PurchaseCost * ReturnCount, ValueAddedTax.Value);
            }
        }

        /// <summary>
        /// Сумма НДС получателя в отпускных ценах отправителя. Имеет смысл только в случае проведенной накладной (зафиксированы учетные цены)
        /// </summary>
        public virtual decimal ValueAddedTaxSum
        {
            get
            {
                ValidationUtils.Assert(ReturnFromClientWaybill.IsAccepted, "Зафиксированной учетной цены не существует, так как накладная еще не проведена.");

                return VatUtils.CalculateVatSum(SaleWaybillRow.SalePrice.Value * ReturnCount, ValueAddedTax.Value);
            }
        }

        /// <summary>
        /// Количество товара по позиции
        /// </summary>
        protected override decimal ArticleCount
        {
            get { return ReturnCount; }
        }

        #endregion

        #region Показатели
        
        /// <summary>
        /// Закупочная цена единицы товара
        /// </summary>
        public virtual decimal PurchaseCost
        {
            get { return SaleWaybillRow.PurchaseCost; }
        }

        /// <summary>
        /// Отпускная цена единицы товара
        /// </summary>
        public virtual decimal? SalePrice
        {
            get { return SaleWaybillRow.SalePrice; }
        }

        /// <summary>
        /// Позиция реестра цен, по которой сформирована учетная цена для этой позиции на отправителе
        /// </summary>
        public virtual ArticleAccountingPrice ArticleAccountingPrice
        {
            get { return articleAccountingPrice; }

            protected internal set
            {
                ValidationUtils.Assert(!ReturnFromClientWaybill.IsAccepted, "Реестр, по которому рассчитывается учетная цена, не может быть сменен.");
                
                articleAccountingPrice = value;
            }
        }
        private ArticleAccountingPrice articleAccountingPrice;

        #endregion

        #region Резервирование

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

            if (acceptedCount + shippedCount + finallyMovedCount > returnCount)
            {
                throw new Exception("Сумма проведенного, отгруженного и окончательно перемещенного товара не может быть больше кол-ва товара, указанного в позиции.");
            }

            this.acceptedCount = acceptedCount;
            this.shippedCount = shippedCount;
            this.finallyMovedCount = finallyMovedCount;

            this.AvailableToReserveCount = returnCount - acceptedCount - shippedCount - finallyMovedCount;
        }

        #endregion

        #endregion

        #region Конструкторы

        protected ReturnFromClientWaybillRow()
            : base(WaybillType.ReturnFromClientWaybill)
        {            
        }

        public ReturnFromClientWaybillRow(SaleWaybillRow saleWaybillRow, decimal returnCount) : this()
        {
            ValidationUtils.NotNull(saleWaybillRow, "Не указана позиция накладной реализации.");
            if (returnCount > saleWaybillRow.SellingCount)
            {
                throw new Exception("Невозможно вернуть количество товара большее, чем реализовано.");
            }
            if (returnCount < 0)
            {
                throw new Exception("Невозможно вернуть количество товара, меньшее нуля.");
            }

            SaleWaybillRow = saleWaybillRow; // Должно идти перед присваиванием количества, т.к. задает допустимое число его знаков после запятой
            ReturnCount = returnCount;
        }

        #endregion

        #region Методы

        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(ReturnFromClientWaybill.IsDraft,
                String.Format("Невозможно удалить позицию из накладной со статусом «{0}».", ReturnFromClientWaybill.State.GetDisplayName()));
            ValidationUtils.Assert(!AreOutgoingWaybills, "Невозможно удалить позицию, так как по ней уже есть исходящие накладные.");
        }

        #endregion
    }
}
