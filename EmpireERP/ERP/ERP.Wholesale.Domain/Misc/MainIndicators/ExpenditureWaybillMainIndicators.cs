using System.Linq;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Основные показатели накладной реализации товаров
    /// </summary>
    public class ExpenditureWaybillMainIndicators
    {
        /// <summary>
        /// Сумма в учетных ценах отправителя
        /// </summary>
        public decimal SenderAccountingPriceSum
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
        decimal senderAccountingPriceSum;
        bool isSenderAccountingPriceSumCalculated = false;

        /// <summary>
        /// Сумма в отпускных ценах
        /// </summary>
        public decimal SalePriceSum
        {
            get
            {
                ValidationUtils.Assert(isSalePriceSumCalculated, "Сумма в отпускных ценах не рассчитана.");

                return salePriceSum;
            }
            set 
            {
                isSalePriceSumCalculated = true;
                salePriceSum = value; 
            }
        }
        decimal salePriceSum;
        bool isSalePriceSumCalculated = false;

        /// <summary>
        /// Сумма оплат
        /// </summary>
        public decimal PaymentSum
        {
            get
            {
                ValidationUtils.Assert(isPaymentSumCalculated, "Сумма оплат не рассчитана.");

                return paymentSum;
            }
            set 
            {
                isPaymentSumCalculated = true;
                paymentSum = value; 
            }
        }
        decimal paymentSum;
        bool isPaymentSumCalculated = false;

        /// <summary>
        /// Сумма неоплаченного остатка по накладной
        /// </summary>
        public decimal DebtRemainder
        {
            get
            {
                ValidationUtils.Assert(isDebtRemainderCalculated, "Сумма неоплаченного остатка не рассчитана.");

                return debtRemainder;
            }
            set 
            {
                isDebtRemainderCalculated = true;
                debtRemainder = value; 
            }
        }
        decimal debtRemainder;
        bool isDebtRemainderCalculated = false;

        /// <summary>
        /// Процент оплаты по накладной
        /// </summary>
        public decimal PaymentPercent
        {
            get
            {
                ValidationUtils.Assert(isPaymentPercentCalculated, "Процент оплаты не рассчитан.");

                return paymentPercent;
            }
            set 
            {
                isPaymentPercentCalculated = true;
                paymentPercent = value; 
            }
        }
        decimal paymentPercent;
        bool isPaymentPercentCalculated = false;

        /// <summary>
        /// Информация об НДС
        /// </summary>
        public ILookup<decimal, decimal> VatInfoList
        {
            get
            {
                ValidationUtils.Assert(isVatInfoListCalculated, "Информация об НДС не установлена.");

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

        /// <summary>
        /// Процент скидки
        /// </summary>
        public decimal TotalDiscountPercent
        {
            get
            {
                ValidationUtils.Assert(isTotalDiscountPercentCalculated, "Процент скидки не рассчитан.");

                return totalDiscountPercent;
            }
            set 
            {
                isTotalDiscountPercentCalculated = true;
                totalDiscountPercent = value; 
            }
        }
        decimal totalDiscountPercent;
        bool isTotalDiscountPercentCalculated = false;

        /// <summary>
        /// Сумма скидки
        /// </summary>
        public decimal TotalDiscountSum
        {
            get
            {
                ValidationUtils.Assert(isTotalDiscountSumCalculated, "Сумма скидки не рассчитана.");

                return totalDiscountSum;
            }
            set 
            {
                isTotalDiscountSumCalculated = true;
                totalDiscountSum = value; 
            }
        }
        decimal totalDiscountSum;
        bool isTotalDiscountSumCalculated = false;

        /// <summary>
        /// Процент прибыли
        /// </summary>
        public decimal ProfitPercent
        {
            get
            {
                ValidationUtils.Assert(isProfitPercentCalculated, "Процент прибыли не рассчитан.");

                return profitPercent;
            }
            set 
            {
                isProfitPercentCalculated = true;
                profitPercent = value; 
            }
        }
        decimal profitPercent;
        bool isProfitPercentCalculated = false;

        /// <summary>
        /// Сумма прибыли
        /// </summary>
        public decimal ProfitSum
        {
            get
            {
                ValidationUtils.Assert(isProfitSumCalculated, "Процент прибыли не рассчитан.");

                return profitSum;
            }
            set 
            {
                isProfitSumCalculated = true;
                profitSum = value; 
            }
        }
        decimal profitSum;
        bool isProfitSumCalculated = false;

        /// <summary>
        /// Сумма потерь от всех возвратов
        /// </summary>
        public decimal ReturnLostProfitSum
        {
            get
            {
                ValidationUtils.Assert(isReturnLostProfitSumCalculated, "Сумма потерь от всех возвратов не рассчитана.");

                return returnLostProfitSum;
            }
            set
            {
                isReturnLostProfitSumCalculated = true;
                returnLostProfitSum = value;
            }
        }
        decimal returnLostProfitSum;
        bool isReturnLostProfitSumCalculated = false;

        /// <summary>
        /// Сумма потерь от принятых возвратов
        /// </summary>
        public decimal ReservedByReturnLostProfitSum
        {
            get
            {
                ValidationUtils.Assert(isReservedByReturnLostProfitSumCalculated, "Сумма потерь от принятых возвратов не рассчитана.");

                return reservedByReturnLostProfitSum;
            }
            set
            {
                isReservedByReturnLostProfitSumCalculated = true;
                reservedByReturnLostProfitSum = value;
            }
        }
        decimal reservedByReturnLostProfitSum;
        bool isReservedByReturnLostProfitSumCalculated = false;

        /// <summary>
        /// Общая сумма принятых возвратов по накладной реализации
        /// </summary>
        public decimal TotalReturnedSum
        {
            get
            {
                ValidationUtils.Assert(isTotalReturnedSumCalculated, "Общая сумма принятых возвратов по накладной реализации не рассчитана.");

                return totalReturnedSum;
            }
            set
            {
                isTotalReturnedSumCalculated = true;
                totalReturnedSum = value;
            }
        }
        decimal totalReturnedSum;
        bool isTotalReturnedSumCalculated = false;

        /// <summary>
        /// Общая сумма всех возвратов по накладной реализации
        /// </summary>
        public decimal TotalReservedByReturnSum
        {
            get
            {
                ValidationUtils.Assert(isTotalReservedByReturnSumCalculated, "Общая сумма всех возвратов по накладной реализации не рассчитана.");

                return totalReservedByReturnSum;
            }
            set
            {
                isTotalReservedByReturnSumCalculated = true;
                totalReservedByReturnSum = value;
            }
        }
        decimal totalReservedByReturnSum;
        bool isTotalReservedByReturnSumCalculated = false;

    }
}
