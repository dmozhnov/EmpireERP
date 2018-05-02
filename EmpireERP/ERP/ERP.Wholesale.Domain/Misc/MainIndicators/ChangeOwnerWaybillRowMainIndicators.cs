using ERP.Utils;
using System.Linq;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Основные показатели накладной позиции перемещения
    /// </summary>
    public class ChangeOwnerWaybillRowMainIndicators
    {
        /// <summary>
        /// Учетная цена
        /// </summary>
        public decimal? AccountingPrice
        {
            get
            {
                ValidationUtils.Assert(isAccountingPriceCalculated, "Учетная цена не рассчитана.");
                return accountingPrice;
            }
            set
            {
                isAccountingPriceCalculated = true;
                accountingPrice = value;
            }
        }
        decimal? accountingPrice;
        bool isAccountingPriceCalculated = false;

        /// <summary>
        /// Сумма НДС
        /// </summary>
        public decimal? ValueAddedTaxSum
        {
            get
            {
                ValidationUtils.Assert(isValueAddedTaxSumCalculated, "Сумма НДС е рассчитана.");
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
