using System;
using System.Linq;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Позиция накладной реализации товаров
    /// </summary>
    public class ExpenditureWaybillRow : SaleWaybillRow
    {
        #region Свойства

        /// <summary>
        /// Накладная реализация товаров, частью которой является позиция
        /// </summary>
        public virtual ExpenditureWaybill ExpenditureWaybill 
        {
            get { return SaleWaybill == null ? null : SaleWaybill.As<ExpenditureWaybill>(); }
            protected internal set
            {
                SaleWaybill = value;
            }
        }

        /// <summary>
        /// Статус позиции исходящей накладной
        /// </summary>
        public virtual OutgoingWaybillRowState OutgoingWaybillRowState
        {
            get { return outgoingWaybillRowState; }
            protected internal set
            {
                ValidationUtils.NotNull(ExpenditureWaybill, "Не установлено значение накладной.");
                
                // если накладная не отгружена и значение статуса позиции стало другим
                if (!ExpenditureWaybill.IsShipped && outgoingWaybillRowState != value)
                {
                    outgoingWaybillRowState = value;
                    
                    // пересчет статуса накладной имеет смысл только для проведенной накладной
                    if (ExpenditureWaybill.IsAccepted)
                    {
                        ExpenditureWaybill.UpdateStateByRowsState();
                    }
                }
            }
        }
        private OutgoingWaybillRowState outgoingWaybillRowState;

        /// <summary>
        /// Сумма НДС в закупочной цене
        /// </summary>
        public virtual decimal PurchaseCostValueAddedTaxSum
        {
            get
            {
                return VatUtils.CalculateVatSum(ReceiptWaybillRow.PurchaseCost * SellingCount, ValueAddedTax.Value);
            }
        }

        /// <summary>
        /// Сумма НДС. Имеет смысл только в случае проведенной накладной (зафиксированы учетные цены)
        /// </summary>
        public override decimal ValueAddedTaxSum
        {
            get
            {
                ValidationUtils.Assert(ExpenditureWaybill.IsAccepted, "Зафиксированной учетной цены не существует, так как накладная еще не проведена.");

                return VatUtils.CalculateVatSum(SalePrice.Value * SellingCount, ValueAddedTax.Value);
            }
        }

        /// <summary>
        /// Позиция реестра цен, по которой сформирована учетная цена для этой позиции на отправителе
        /// </summary>
        public virtual ArticleAccountingPrice SenderArticleAccountingPrice
        {
            get { return senderArticleAccountingPrice; }
            protected internal set
            {
                ValidationUtils.Assert(!ExpenditureWaybill.IsAccepted, "Реестр, по которому рассчитывается учетная цена, не может быть сменен.");
                                
                senderArticleAccountingPrice = value;

                if (senderArticleAccountingPrice != null)
                {
                    salePrice = CalculateSalePrice(SenderArticleAccountingPrice.AccountingPrice);
                }
                else
                {
                    salePrice = null;
                }
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
                if (ExpenditureWaybill != null && receiptWaybillRow != null &&
                    ExpenditureWaybill.Rows.Where(x => x.As<ExpenditureWaybillRow>().ReceiptWaybillRow.Id == value.Id && x.Id != this.Id).FirstOrDefault() != null)
                {
                    throw new Exception("Позиция накладной по данной партии и товару уже добавлена.");
                }

                receiptWaybillRow = value;
                article = value.Article;
            }
        }
        private ReceiptWaybillRow receiptWaybillRow;

        /// <summary>
        /// Кол-во знаков после запятой (из единицы измерения товара)
        /// </summary>
        public override byte ArticleMeasureUnitScale
        {
            get { return ReceiptWaybillRow.ArticleMeasureUnitScale; }
        }

        /// <summary>
        /// Закупочная цена единицы товара
        /// </summary>
        public override decimal PurchaseCost 
        {
            get { return ReceiptWaybillRow.PurchaseCost; } 
        }

        /// <summary>
        /// Рассчитать "незафиксированную" отпускную цену позиции. Использовать для непроведенных накладных.
        /// </summary>
        /// <param name="senderAccountingPrice">Учетная цена товара на складе-отправителе.</param>
        /// <returns>"Незафиксированная" отпускная цена.</returns>
        public virtual decimal CalculateSalePrice(decimal senderAccountingPrice)
        {
            var salePrice = senderAccountingPrice - (senderAccountingPrice * ExpenditureWaybill.Quota.DiscountPercent / 100);
            return RoundSalePrice(salePrice);
        }

        /// <summary>
        /// Округлить отпускную цену до 2 знаков или до целого числа, в зависимости от флага ExpenditureWaybill.RoundSalePrice.
        /// </summary>
        /// <param name="salePrice">Отпускная цена, которую нужно округлить.</param>
        /// <returns>Округленная отпускная цена.</returns>
        private decimal RoundSalePrice(decimal salePrice)
        {
            return ExpenditureWaybill.RoundSalePrice ? Math.Round(salePrice) : Math.Round(salePrice, 2); 
        }

        /// <summary>
        /// Отпускная цена единицы товара
        /// </summary>
        public override decimal? SalePrice
        {
            get { return salePrice; }
            protected set { salePrice = value.HasValue ? RoundSalePrice(value.Value) : (decimal?)null; }
        }

        /// <summary>
        /// Источник указан вручную (пока здесь, т.к. зависит не напрямую от BaseOutgoingWaybillRow )
        /// </summary>
        public virtual bool IsUsingManualSource { get; set; }

        /// <summary>
        /// Определены ли источники для этой позиции (установлены вручную при добавлении или назначены автоматически при проводке).
        /// </summary>
        public virtual bool AreSourcesDetermined
        {
            get
            {
                return IsUsingManualSource || ExpenditureWaybill.IsAccepted;
            }
        }

        /// <summary>
        /// Количество товара по позиции
        /// </summary>
        protected override decimal ArticleCount
        {
            get { return SellingCount; }
        }

        #endregion

        #region Конструкторы

        protected ExpenditureWaybillRow()
            : base(WaybillType.ExpenditureWaybill)
        {
        }

        public ExpenditureWaybillRow(ReceiptWaybillRow receiptWaybillRow, decimal sellingCount, ValueAddedTax valueAddedTax)
            : this()
        {
            ValidationUtils.NotNull(receiptWaybillRow, "Не указана партия товара.");
            ValidationUtils.Assert(sellingCount > 0, "Количество реализуемого товара должно быть положительным числом.");

            outgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
            ReceiptWaybillRow = receiptWaybillRow; // Должно идти перед присваиванием количества, т.к. задает допустимое число его знаков после запятой
            SellingCount = sellingCount;
            ValueAddedTax = valueAddedTax;
        }

        #endregion

        #region Методы

        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(ExpenditureWaybill.IsDraft, 
               String.Format("Невозможно удалить позицию из накладной со статусом «{0}».", ExpenditureWaybill.State.GetDisplayName()));
        }

        #endregion
    }
}
