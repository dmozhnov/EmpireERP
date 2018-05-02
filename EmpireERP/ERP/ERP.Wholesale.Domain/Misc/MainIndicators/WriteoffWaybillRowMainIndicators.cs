using ERP.Utils;
using System.Linq;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Основные показатели для позиции накладной списания
    /// </summary>
    public class WriteoffWaybillRowMainIndicators
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
        
        ///// <summary>
        ///// Процент недополученной прибыли
        ///// </summary>
        //public decimal? ReceivelessProfitPercent
        //{
        //    get
        //    {
        //        ValidationUtils.Assert(isReceivelessProfitPercentCalculated, "Процент недополученной прибыли не рассчитан.");
        //        return receivelessProfitPercent;
        //    }
        //    set
        //    {
        //        isReceivelessProfitPercentCalculated = true;
        //        receivelessProfitPercent = value;
        //    }
        //}
        //decimal? receivelessProfitPercent;
        //bool isReceivelessProfitPercentCalculated = false;

        ///// <summary>
        ///// Сумма недополученной прибыли
        ///// </summary>
        //public decimal? ReceivelessProfitSum
        //{
        //    get
        //    {
        //        ValidationUtils.Assert(isReceivelessProfitSumCalculated, "Сумма недополученной прибыли не рассчитана.");
        //        return receivelessProfitSum;
        //    }
        //    set
        //    {
        //        isReceivelessProfitSumCalculated = true;
        //        receivelessProfitSum = value;
        //    }
        //}
        //decimal? receivelessProfitSum;
        //bool isReceivelessProfitSumCalculated = false;
    }
}
