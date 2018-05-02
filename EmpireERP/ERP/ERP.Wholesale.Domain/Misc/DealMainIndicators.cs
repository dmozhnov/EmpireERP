using ERP.Utils;

namespace ERP.Wholesale.Domain.Misc
{
    public class DealMainIndicators
    {
        /// <summary>
        /// Сумма реализации
        /// </summary>
        public decimal SaleSum
        {
            get
            {
                ValidationUtils.Assert(isSaleSumCalculated, "Сумма реализации не рассчитана.");

                return saleSum;
            }
            set
            {
                isSaleSumCalculated = true;
                saleSum = value;
            }
        }
        decimal saleSum;
        bool isSaleSumCalculated = false;

        /// <summary>
        /// Сумма реализации по накладным, ожидающим отгрузки
        /// </summary>
        public decimal ShippingPendingSaleSum
        {
            get
            {
                ValidationUtils.Assert(isShippingPendingSaleSumCalculated, "Сумма реализации по накладным, ожидающим отгрузки, не рассчитана.");

                return shippingPendingSaleSum;
            }
            set
            {
                isShippingPendingSaleSumCalculated = true;
                shippingPendingSaleSum = value;
            }
        }
        decimal shippingPendingSaleSum;
        bool isShippingPendingSaleSumCalculated = false;

        /// <summary>
        /// Период просрочки платежей (в днях)
        /// </summary>
        public int PaymentDelayPeriod
        {
            get
            {
                ValidationUtils.Assert(isPaymentDelayPeriodCalculated, "Период просрочки платежей не рассчитан.");

                return paymentDelayPeriod;
            }
            set
            {
                isPaymentDelayPeriodCalculated = true;
                paymentDelayPeriod = value;
            }
        }
        int paymentDelayPeriod;
        bool isPaymentDelayPeriodCalculated = false;

        /// <summary>
        /// Сумма просрочки платежей
        /// </summary>
        public decimal PaymentDelaySum
        {
            get
            {
                ValidationUtils.Assert(isPaymentDelaySumCalculated, "Сумма просрочки платежей не рассчитана.");

                return paymentDelaySum;
            }
            set
            {
                isPaymentDelaySumCalculated = true;
                paymentDelaySum = value;
            }
        }
        decimal paymentDelaySum;
        bool isPaymentDelaySumCalculated = false;
        
        /// <summary>
        /// Сумма принятых возвратов от клиентов
        /// </summary>
        public decimal ReturnedFromClientSum
        {
            get
            {
                ValidationUtils.Assert(isReturnedFromClientSumCalculated, "Сумма принятых возвратов от клиентов не рассчитана.");

                return returnedFromClientSum;
            }
            set
            {
                isReturnedFromClientSumCalculated = true;
                returnedFromClientSum = value;
            }
        }
        decimal returnedFromClientSum;
        bool isReturnedFromClientSumCalculated = false;
        
        /// <summary>
        /// Сумма оформленных возвратов от клиентов
        /// </summary>
        public decimal ReservedByReturnFromClientSum
        {
            get
            {
                ValidationUtils.Assert(isReservedByReturnFromClientSumCalculated, "Сумма оформленных возвратов от клиентов не рассчитана.");

                return reservedByReturnFromClientSum;
            }
            set
            {
                isReservedByReturnFromClientSumCalculated = true;
                reservedByReturnFromClientSum = value;
            }
        }
        decimal reservedByReturnFromClientSum;
        bool isReservedByReturnFromClientSumCalculated = false;

        /// <summary>
        /// Сальдо по сделке
        /// </summary>
        public decimal Balance
        {
            get
            {
                ValidationUtils.Assert(isBalanceCalculated, "Сальдо по отгруженным накладным с отсрочкой платежа не рассчитано.");

                return balance;
            }
            set
            {
                isBalanceCalculated = true;
                balance = value;
            }
        }
        decimal balance;
        bool isBalanceCalculated = false;

        /// <summary>
        /// Сумма корректировок сальдо
        /// </summary>
        public decimal InitialBalance
        {
            get
            {
                ValidationUtils.Assert(isInitialBalanceCalculated, "Сумма корректировок сальдо не рассчитана.");

                return initialBalance;
            }
            set
            {
                isInitialBalanceCalculated = true;
                initialBalance = value;
            }
        }
        decimal initialBalance;
        bool isInitialBalanceCalculated = false;
    }
}
