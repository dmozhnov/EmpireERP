using ERP.Utils;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Информация об основных показателях для заказа
    /// </summary>
    public class ProductionOrderMainIndicators
    {
        #region Свойства

        #region Плановые затраты

        /// <summary>
        /// Сумма плановых затрат в базовой валюте
        /// </summary>
        public decimal? PlannedExpensesSumInBaseCurrency;

        /// <summary>
        /// Плановые затраты на производство в базовой валюте
        /// </summary>
        public decimal? PlannedProductionExpensesInBaseCurrency;

        /// <summary>
        /// Плановые затраты на транспортировку в базовой валюте
        /// </summary>
        public decimal? PlannedTransportationExpensesInBaseCurrency;

        /// <summary>
        /// Плановые дополнительные расходы в базовой валюте
        /// </summary>
        public decimal? PlannedExtraExpensesInBaseCurrency;

        /// <summary>
        /// Плановые таможенные расходы в базовой валюте
        /// </summary>
        public decimal? PlannedCustomsExpensesInBaseCurrency;

        #endregion

        #region Плановые оплаты

        /// <summary>
        /// Плановые затраты на производство
        /// </summary>
        public decimal? PlannedProductionPaymentsInBaseCurrency;

        /// <summary>
        /// Плановые затраты на транспортировку
        /// </summary>
        public decimal? PlannedTransportationPaymentsInBaseCurrency;

        /// <summary>
        /// Плановые дополнительные затраты
        /// </summary>
        public decimal? PlannedExtraExpensesPaymentsInBaseCurrency;

        /// <summary>
        /// Плановые таможенные затраты
        /// </summary>
        public decimal? PlannedCustomsPaymentsInBaseCurrency;

        #endregion

        #region Фактическая стоимость

        /// <summary>
        /// Фактическая стоимость заказа (полная) в валюте заказа
        /// </summary>
        public decimal ActualCostSumInCurrency
        {
            get
            {
                ValidationUtils.NotNull(actualCostSumInCurrency, "Показатель заказа не рассчитан.");

                return actualCostSumInCurrency.Value;
            }
            set { actualCostSumInCurrency = value; }
        }
        private decimal? actualCostSumInCurrency;

        /// <summary>
        /// Фактическая стоимость заказа (полная) в базовой валюте
        /// </summary>
        public decimal ActualCostSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(actualCostSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return actualCostSumInBaseCurrency.Value;
            }
            set { actualCostSumInBaseCurrency = value; }
        }
        private decimal? actualCostSumInBaseCurrency;

        /// <summary>
        /// Фактическая стоимость транспортировки заказа в базовой валюте
        /// </summary>
        public decimal ActualTransportationCostSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(actualTransportationCostSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return actualTransportationCostSumInBaseCurrency.Value;
            }
            set { actualTransportationCostSumInBaseCurrency = value; }
        }
        private decimal? actualTransportationCostSumInBaseCurrency;

        /// <summary>
        /// Фактическая стоимость дополнительных расходов заказа в базовой валюте
        /// </summary>
        public decimal ActualExtraExpensesCostSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(actualExtraExpensesCostSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return actualExtraExpensesCostSumInBaseCurrency.Value;
            }
            set { actualExtraExpensesCostSumInBaseCurrency = value; }
        }
        private decimal? actualExtraExpensesCostSumInBaseCurrency;

        #region Фактическая стоимость таможенных расходов

        /// <summary>
        /// Фактическая стоимость таможенных расходов заказа (всех) в базовой валюте
        /// </summary>
        public decimal ActualCustomsExpensesCostSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(actualCustomsExpensesCostSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return actualCustomsExpensesCostSumInBaseCurrency.Value;
            }
            set { actualCustomsExpensesCostSumInBaseCurrency = value; }
        }
        private decimal? actualCustomsExpensesCostSumInBaseCurrency;

        /// <summary>
        /// Фактическая стоимость ввозных таможенных пошлин заказа в базовой валюте
        /// </summary>
        public decimal ActualImportCustomsDutiesSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(actualImportCustomsDutiesSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return actualImportCustomsDutiesSumInBaseCurrency.Value;
            }
            set { actualImportCustomsDutiesSumInBaseCurrency = value; }
        }
        private decimal? actualImportCustomsDutiesSumInBaseCurrency;

        /// <summary>
        /// Фактическая стоимость вывозных таможенных пошлин заказа в базовой валюте
        /// </summary>
        public decimal ActualExportCustomsDutiesSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(actualExportCustomsDutiesSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return actualExportCustomsDutiesSumInBaseCurrency.Value;
            }
            set { actualExportCustomsDutiesSumInBaseCurrency = value; }
        }
        private decimal? actualExportCustomsDutiesSumInBaseCurrency;

        /// <summary>
        /// Фактическая стоимость НДС заказа в базовой валюте
        /// </summary>
        public decimal ActualValueAddedTaxSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(actualValueAddedTaxSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return actualValueAddedTaxSumInBaseCurrency.Value;
            }
            set { actualValueAddedTaxSumInBaseCurrency = value; }
        }
        private decimal? actualValueAddedTaxSumInBaseCurrency;

        /// <summary>
        /// Фактическая стоимость акцизов заказа в базовой валюте
        /// </summary>
        public decimal ActualExciseSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(actualExciseSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return actualExciseSumInBaseCurrency.Value;
            }
            set { actualExciseSumInBaseCurrency = value; }
        }
        private decimal? actualExciseSumInBaseCurrency;

        /// <summary>
        /// Фактическая стоимость таможенных сборов заказа в базовой валюте
        /// </summary>
        public decimal ActualCustomsFeesSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(actualCustomsFeesSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return actualCustomsFeesSumInBaseCurrency.Value;
            }
            set { actualCustomsFeesSumInBaseCurrency = value; }
        }
        private decimal? actualCustomsFeesSumInBaseCurrency;

        /// <summary>
        /// Фактическая стоимость КТС заказа в базовой валюте
        /// </summary>
        public decimal ActualCustomsValueCorrectionSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(actualCustomsValueCorrectionSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return actualCustomsValueCorrectionSumInBaseCurrency.Value;
            }
            set { actualCustomsValueCorrectionSumInBaseCurrency = value; }
        }
        private decimal? actualCustomsValueCorrectionSumInBaseCurrency;

        #endregion

        #endregion

        #region Показатели по оплатам

        /// <summary>
        /// Сумма оплат со всеми назначениями в валюте заказа
        /// </summary>
        public decimal PaymentSumInCurrency
        {
            get
            {
                ValidationUtils.NotNull(paymentSumInCurrency, "Показатель заказа не рассчитан.");

                return paymentSumInCurrency.Value;
            }
            set { paymentSumInCurrency = value; }
        }
        private decimal? paymentSumInCurrency;

        /// <summary>
        /// Сумма оплат со всеми назначениями в базовой валюте
        /// </summary>
        public decimal PaymentSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(paymentSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return paymentSumInBaseCurrency.Value;
            }
            set { paymentSumInBaseCurrency = value; }
        }
        private decimal? paymentSumInBaseCurrency;

        /// <summary>
        /// Сумма оплат за производство в валюте заказа
        /// </summary>
        public decimal PaymentProductionSumInCurrency
        {
            get
            {
                ValidationUtils.NotNull(paymentProductionSumInCurrency, "Показатель заказа не рассчитан.");

                return paymentProductionSumInCurrency.Value;
            }
            set { paymentProductionSumInCurrency = value; }
        }
        private decimal? paymentProductionSumInCurrency;

        /// <summary>
        /// Сумма оплат за производство в базовой валюте
        /// </summary>
        public decimal PaymentProductionSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(paymentProductionSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return paymentProductionSumInBaseCurrency.Value;
            }
            set { paymentProductionSumInBaseCurrency = value; }
        }
        private decimal? paymentProductionSumInBaseCurrency;

        /// <summary>
        /// Сумма оплат за транспортировку в базовой валюте
        /// </summary>
        public decimal PaymentTransportationSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(paymentTransportationSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return paymentTransportationSumInBaseCurrency.Value;
            }
            set { paymentTransportationSumInBaseCurrency = value; }
        }
        private decimal? paymentTransportationSumInBaseCurrency;

        /// <summary>
        /// Сумма оплат за дополнительные расходы в базовой валюте
        /// </summary>
        public decimal PaymentExtraExpensesSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(paymentExtraExpensesSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return paymentExtraExpensesSumInBaseCurrency.Value;
            }
            set { paymentExtraExpensesSumInBaseCurrency = value; }
        }
        private decimal? paymentExtraExpensesSumInBaseCurrency;

        #region Суммы оплат за таможенные расходы

        /// <summary>
        /// Сумма оплат таможенных расходов заказа (всех) в базовой валюте
        /// </summary>
        public decimal PaymentCustomsExpensesSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(paymentCustomsExpensesSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return paymentCustomsExpensesSumInBaseCurrency.Value;
            }
            set { paymentCustomsExpensesSumInBaseCurrency = value; }
        }
        private decimal? paymentCustomsExpensesSumInBaseCurrency;

        /// <summary>
        /// Сумма оплат ввозных таможенных пошлин заказа в базовой валюте
        /// </summary>
        public decimal PaymentImportCustomsDutiesSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(paymentImportCustomsDutiesSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return paymentImportCustomsDutiesSumInBaseCurrency.Value;
            }
            set { paymentImportCustomsDutiesSumInBaseCurrency = value; }
        }
        private decimal? paymentImportCustomsDutiesSumInBaseCurrency;

        /// <summary>
        /// Сумма оплат вывозных таможенных пошлин заказа в базовой валюте
        /// </summary>
        public decimal PaymentExportCustomsDutiesSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(paymentExportCustomsDutiesSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return paymentExportCustomsDutiesSumInBaseCurrency.Value;
            }
            set { paymentExportCustomsDutiesSumInBaseCurrency = value; }
        }
        private decimal? paymentExportCustomsDutiesSumInBaseCurrency;

        /// <summary>
        /// Сумма оплат НДС заказа в базовой валюте
        /// </summary>
        public decimal PaymentValueAddedTaxSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(paymentValueAddedTaxSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return paymentValueAddedTaxSumInBaseCurrency.Value;
            }
            set { paymentValueAddedTaxSumInBaseCurrency = value; }
        }
        private decimal? paymentValueAddedTaxSumInBaseCurrency;

        /// <summary>
        /// Сумма оплат акцизов заказа в базовой валюте
        /// </summary>
        public decimal PaymentExciseSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(paymentExciseSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return paymentExciseSumInBaseCurrency.Value;
            }
            set { paymentExciseSumInBaseCurrency = value; }
        }
        private decimal? paymentExciseSumInBaseCurrency;

        /// <summary>
        /// Сумма оплат таможенных сборов заказа в базовой валюте
        /// </summary>
        public decimal PaymentCustomsFeesSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(paymentCustomsFeesSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return paymentCustomsFeesSumInBaseCurrency.Value;
            }
            set { paymentCustomsFeesSumInBaseCurrency = value; }
        }
        private decimal? paymentCustomsFeesSumInBaseCurrency;

        /// <summary>
        /// Сумма оплат КТС заказа в базовой валюте
        /// </summary>
        public decimal PaymentCustomsValueCorrectionSumInBaseCurrency
        {
            get
            {
                ValidationUtils.NotNull(paymentCustomsValueCorrectionSumInBaseCurrency, "Показатель заказа не рассчитан.");

                return paymentCustomsValueCorrectionSumInBaseCurrency.Value;
            }
            set { paymentCustomsValueCorrectionSumInBaseCurrency = value; }
        }
        private decimal? paymentCustomsValueCorrectionSumInBaseCurrency;

        #endregion

        /// <summary>
        /// Процент оплаты
        /// </summary>
        public decimal PaymentPercent
        {
            get
            {
                ValidationUtils.NotNull(paymentPercent, "Показатель заказа не рассчитан.");

                return paymentPercent.Value;
            }
            set { paymentPercent = value; }
        }
        private decimal? paymentPercent;

        #endregion

        #region Показатели по учетным ценам

        /// <summary>
        /// Сумма заказа в учетных ценах
        /// </summary>
        public decimal AccountingPriceSum
        {
            get
            {
                ValidationUtils.NotNull(accountingPriceSum, "Показатель заказа не рассчитан.");

                return accountingPriceSum.Value;
            }
            set { accountingPriceSum = value; }
        }
        private decimal? accountingPriceSum;

        /// <summary>
        /// Ожидаемая прибыль в учетных ценах
        /// </summary>
        public decimal MarkupPendingSum
        {
            get
            {
                ValidationUtils.NotNull(markupPendingSum, "Показатель заказа не рассчитан.");

                return markupPendingSum.Value;
            }
            set { markupPendingSum = value; }
        }
        private decimal? markupPendingSum;

        #endregion

        #endregion
    }
}
