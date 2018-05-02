using ERP.Utils;
using System.Linq;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Основные показатели накладной перемещения
    /// </summary>
    public class ChangeOwnerWaybillMainIndicators
    {
        /// <summary>
        /// Сумма в учетных ценах
        /// </summary>
        public decimal? AccountingPriceSum
        {
            get
            {
                ValidationUtils.Assert(isAccountingPriceSumCalculated, "Сумма в учетных ценах не рассчитана.");
                return accountingPriceSum;
            }
            set
            {
                isAccountingPriceSumCalculated = true;
                accountingPriceSum = value;
            }
        }
        decimal? accountingPriceSum;
        bool isAccountingPriceSumCalculated = false;        
       
        /// <summary>
        /// Строка с информацией об НДС
        /// </summary>
        public ILookup<decimal, decimal> VatInfoList
        {
            get
            {
                ValidationUtils.Assert(isVatInfoListCalculated, "Ставки НДС не рассчитаны.");
                return vatInfoList;
            }
            set
            {
                isVatInfoListCalculated = true;
                vatInfoList = value;
            }
        }
        ILookup<decimal, decimal> vatInfoList;
        bool isVatInfoListCalculated = false;
    }
}
