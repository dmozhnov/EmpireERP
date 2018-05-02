using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Позиция накладной реализации
    /// </summary>
    public abstract class SaleWaybillRow : BaseWaybillRow
    {
        #region Свойства
        
        /// <summary>
        /// Накладная реализации, частью которой является позиция
        /// </summary>
        public virtual SaleWaybill SaleWaybill { get; protected internal set; }

        /// <summary>
        /// Ставка НДС
        /// </summary>
        public virtual ValueAddedTax ValueAddedTax { get; set; }

        /// <summary>
        /// Сумма НДС. Имеет смысл только в случае проведенной накладной (зафиксированы учетные цены)
        /// </summary>
        public abstract decimal ValueAddedTaxSum { get; }

        /// <summary>
        /// Закупочная цена товара
        /// </summary>
        public abstract decimal PurchaseCost { get; }

        /// <summary>
        /// Количество реализуемого товара
        /// </summary>
        /// <remarks>вещественное (18, 6)</remarks>
        public virtual decimal SellingCount
        {
            get { return sellingCount; }
            set
            {
                ValidationUtils.CheckDecimalScale(value, ArticleMeasureUnitScale, "Количество товара имеет слишком большое число цифр после запятой.");
                sellingCount = value;
            }
        }
        private decimal sellingCount;

        /// <summary>
        /// Отпускная цена товара
        /// </summary>
        public abstract decimal? SalePrice { get; protected set; }
        protected decimal? salePrice;

        /// <summary>
        /// Кол-во знаков после запятой (из единицы измерения товара)
        /// </summary>
        public abstract byte ArticleMeasureUnitScale { get; }

        /// <summary>
        /// Кол-во проведенного (но еще не возвращенного) возвращаемого товара по позиции накладной
        /// </summary>
        public virtual decimal AcceptedReturnCount
        {
            get { return acceptedReturnCount; }
            protected set 
            {
                ValidationUtils.Assert(value >= 0, "Кол-во возвращаемого товара не может быть меньше 0.");

                acceptedReturnCount = value; 
            }
        }
        private decimal acceptedReturnCount;

        /// <summary>
        /// Кол-во принятого возвращенного товара по позиции накладной
        /// </summary>
        public virtual decimal ReceiptedReturnCount
        {
            get { return receiptedReturnCount; }
            set
            {
                ValidationUtils.Assert(value >= 0, "Кол-во возвращаемого товара не может быть меньше 0.");
                
                receiptedReturnCount = value;
            }
        }
        private decimal receiptedReturnCount;

        /// <summary>
        /// Кол-во, доступное для возвратов по данной позиции
        /// </summary>
        public virtual decimal AvailableToReturnCount
        {
            get { return availableToReturnCount; }
            protected internal set { availableToReturnCount = value; }
        }
        private decimal availableToReturnCount;

        /// <summary>
        /// Общее кол-во проведенных и принятых возвратов
        /// </summary>
        public virtual decimal ReservedByReturnCount
        {
            get { return AcceptedReturnCount + ReceiptedReturnCount; }
        }

        #endregion

        #region Конструктор

        protected SaleWaybillRow()
        {

        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="waybillType">Тип накладной, которой принадлежит позиция</param>
        protected SaleWaybillRow(WaybillType waybillType)
            :base(waybillType)
        {

        }

        #endregion

        #region Методы

        /// <summary>
        /// Установка количеств возвращенного товара
        /// </summary>
        /// <param name="acceptedReturnCount">Кол-во проведенных возвратов</param>
        /// <param name="receiptedReturnCount">Кол-во окончательно принятых возвратов</param>
        protected internal virtual void SetReturnCounts(decimal acceptedReturnCount, decimal receiptedReturnCount)
        {
            ValidationUtils.Assert(acceptedReturnCount >= 0 && receiptedReturnCount >= 0, 
                "Кол-во возвращаемого товара не может быть меньше 0.");

            ValidationUtils.Assert(SellingCount >= acceptedReturnCount + receiptedReturnCount,
                "Невозможно вернуть товара больше, чем было реализовано.");

            AcceptedReturnCount = acceptedReturnCount;
            ReceiptedReturnCount = receiptedReturnCount;

            AvailableToReturnCount = SellingCount - AcceptedReturnCount - ReceiptedReturnCount;
        }

        #endregion
    }
}
