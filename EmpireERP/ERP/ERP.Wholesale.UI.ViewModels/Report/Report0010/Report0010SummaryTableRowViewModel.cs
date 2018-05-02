using System.ComponentModel;
using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0010
{
    public class Report0010SummaryTableRowViewModel
    {
        /// <summary>
        /// Наименование
        /// </summary>
        [DisplayName("Наименование")]
        public string Name { get; set; }

        /// <summary>
        /// Сумма принятых платежей
        /// </summary>
        [DisplayName("Сумма принятых платежей")]
        public string DealPaymentFromClientSumString { get { return DealPaymentFromClientSum.ForDisplay(ValueDisplayType.Money); } }
        public decimal DealPaymentFromClientSum { get; set; }

        /// <summary>
        /// В т.ч. наличный расчет
        /// </summary>
        [DisplayName("В т.ч. наличный расчет")]
        public string DealPaymentFromClientCashSumString { get { return DealPaymentFromClientCashSum.ForDisplay(ValueDisplayType.Money); } }
        public decimal DealPaymentFromClientCashSum { get; set; }

        /// <summary>
        /// В т.ч. безналичный расчет
        /// </summary>
        [DisplayName("В т.ч. безналичный расчет")]
        public string DealPaymentFromClientCashlessSumString { get { return DealPaymentFromClientCashlessSum.ForDisplay(ValueDisplayType.Money); } }
        public decimal DealPaymentFromClientCashlessSum { get; set; }

        /// <summary>
        /// Сумма возвращенных платежей
        /// </summary>
        [DisplayName("Сумма возвращенных платежей")]
        public string DealPaymentToClientSumString { get { return DealPaymentToClientSum.ForDisplay(ValueDisplayType.Money); } }
        public decimal DealPaymentToClientSum { get; set; }

        /// <summary>
        /// В т.ч. наличный расчет
        /// </summary>
        [DisplayName("В т.ч. наличный расчет")]
        public string DealPaymentToClientCashSumString { get { return DealPaymentToClientCashSum.ForDisplay(ValueDisplayType.Money); } }
        public decimal DealPaymentToClientCashSum { get; set; }

        /// <summary>
        /// В т.ч. безналичный расчет
        /// </summary>
        [DisplayName("В т.ч. безналичный расчет")]
        public string DealPaymentToClientCashlessSumString { get { return DealPaymentToClientCashlessSum.ForDisplay(ValueDisplayType.Money); } }
        public decimal DealPaymentToClientCashlessSum { get; set; }
    }
}
