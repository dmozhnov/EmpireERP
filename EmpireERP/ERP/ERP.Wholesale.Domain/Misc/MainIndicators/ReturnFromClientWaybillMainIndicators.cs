using ERP.Utils;
using System.Linq;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Основные показатели накладной возврата от клиента
    /// </summary>
    public class ReturnFromClientWaybillMainIndicators
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
       
    }
}
