using ERP.Utils;
using System.Linq;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Основные показатели для накладной списания
    /// </summary>
    public class WriteoffWaybillMainIndicators
    {
        /// <summary>
        /// Сумма в учетных ценах отправителя
        /// </summary>
        public decimal? SenderAccountingPriceSum
        {
            get
            {
                ValidationUtils.Assert(isSenderAccountingPriceSumCalculated, "Сумма в учетных ценах отправителя не рассчитана.");
                return senderAccountingPriceSum;
            }
            set
            {
                isSenderAccountingPriceSumCalculated = true;
                senderAccountingPriceSum = value;
            }
        }
        decimal? senderAccountingPriceSum;
        bool isSenderAccountingPriceSumCalculated = false;
        
        /// <summary>
        /// Процент недополученной прибыли
        /// </summary>
        public decimal? ReceivelessProfitPercent
        {
            get
            {
                ValidationUtils.Assert(isReceivelessProfitPercentCalculated, "Процент недополученной прибыли не рассчитан.");
                return receivelessProfitPercent;
            }
            set
            {
                isReceivelessProfitPercentCalculated = true;
                receivelessProfitPercent = value;
            }
        }
        decimal? receivelessProfitPercent;
        bool isReceivelessProfitPercentCalculated = false;

        /// <summary>
        /// Сумма недополученной прибыли
        /// </summary>
        public decimal? ReceivelessProfitSum
        {
            get
            {
                ValidationUtils.Assert(isReceivelessProfitSumCalculated, "Сумма недополученной прибыли не рассчитана.");
                return receivelessProfitSum;
            }
            set
            {
                isReceivelessProfitSumCalculated = true;
                receivelessProfitSum = value;
            }
        }
        decimal? receivelessProfitSum;
        bool isReceivelessProfitSumCalculated = false;
    }
}
