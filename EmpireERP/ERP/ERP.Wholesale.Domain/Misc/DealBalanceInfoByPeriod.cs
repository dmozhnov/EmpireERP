using System;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Информация о сальдо по сделке (группе сделок) за период
    /// </summary>
    public class DealBalanceInfoByPeriod
    {
        /// <summary>
        /// Сальдо на начало периода
        /// </summary>
        public decimal StartingBalance { get; set; }

        /// <summary>
        /// Сальдо на конец периода
        /// </summary>
        public decimal EndingBalance
        {
            get
            {
                return StartingBalance + TotalDebit - TotalCredit;
            }
        }

        /// <summary>
        /// Отгрузка товаров
        /// </summary>
        public decimal SaleWaybillSum { get; set; }

        /// <summary>
        /// Возвраты товаров
        /// </summary>
        public decimal ReturnFromClientWaybillSum { get; set; }

        /// <summary>
        /// Оплаты за наличный расчет
        /// </summary>
        public decimal DealPaymentFromClientCashSum { get; set; }

        /// <summary>
        /// Возвраты оплат за наличный расчет
        /// </summary>
        public decimal DealPaymentToClientCashSum { get; set; }

        /// <summary>
        /// Оплаты за безналичный расчет
        /// </summary>
        public decimal DealPaymentFromClientCashlessSum { get; set; }

        /// <summary>
        /// Возвраты оплат за безналичный расчет
        /// </summary>
        public decimal DealPaymentToClientCashlessSum { get; set; }

        /// <summary>
        /// Оплаты, принятые от третьих лиц
        /// </summary>
        public decimal DealPaymentFromClientThirdPartyCashlessSum { get; set; }

        /// <summary>
        /// Возвраты оплат, принятые третьими лицами
        /// </summary>
        public decimal DealPaymentToClientThirdPartyCashlessSum { get; set; }

        /// <summary>
        /// Дебеты корректировок сальдо
        /// </summary>
        public decimal DealDebitInitialBalanceCorrectionSum { get; set; }

        /// <summary>
        /// Кредиты корректировок сальдо
        /// </summary>
        public decimal DealCreditInitialBalanceCorrectionSum { get; set; }

        /// <summary>
        /// Итого обороты (дебет) за период
        /// </summary>
        public decimal TotalDebit
        {
            get
            {
                return SaleWaybillSum + DealPaymentToClientCashSum + DealPaymentToClientCashlessSum +
                    DealPaymentToClientThirdPartyCashlessSum + DealDebitInitialBalanceCorrectionSum;
            }
        }

        /// <summary>
        /// Итого обороты (кредит) за период
        /// </summary>
        public decimal TotalCredit
        {
            get
            {
                return ReturnFromClientWaybillSum + DealPaymentFromClientCashSum + DealPaymentFromClientCashlessSum +
                    DealPaymentFromClientThirdPartyCashlessSum + DealCreditInitialBalanceCorrectionSum;
            }
        }
    }
}
