using ERP.Utils;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Основные показатели позиции накладной реализации товаров
    /// </summary>
    public class ExpenditureWaybillRowMainIndicators
    {
        /// <summary>
        /// Учетная цена отправителя
        /// </summary>
        public decimal? SenderAccountingPrice
        {
            get
            {
                ValidationUtils.Assert(isSenderAccountingPriceCalculated, "Учетная цена отправителя не рассчитана.");

                return senderAccountingPrice;
            }
            set
            {
                isSenderAccountingPriceCalculated = true;
                senderAccountingPrice = value;
            }
        }
        decimal? senderAccountingPrice;
        bool isSenderAccountingPriceCalculated = false;

        /// <summary>
        /// Отпускная цена
        /// </summary>
        public decimal? SalePrice
        {
            get
            {
                ValidationUtils.Assert(isSalePriceCalculated, "Отпускная цена не рассчитана.");

                return salePrice;
            }
            set
            {
                isSalePriceCalculated = true;
                salePrice = value;
            }
        }
        decimal? salePrice;
        bool isSalePriceCalculated = false;

        /// <summary>
        /// Сумма НДС
        /// </summary>
        public decimal? ValueAddedTaxSum
        {
            get
            {
                ValidationUtils.Assert(isValueAddedTaxSumCalculated, "Сумма НДС не рассчитана.");

                return valueAddedTaxSum;
            }
            set
            {
                isValueAddedTaxSumCalculated = true;
                valueAddedTaxSum = value;
            }
        }
        decimal? valueAddedTaxSum;
        bool isValueAddedTaxSumCalculated = false;
    }
}
