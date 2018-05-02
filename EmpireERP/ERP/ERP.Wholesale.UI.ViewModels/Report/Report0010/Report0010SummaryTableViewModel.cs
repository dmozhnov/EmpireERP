using System.Collections.Generic;
using System.Linq;
using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0010
{
    public class Report0010SummaryTableViewModel
    {
        /// <summary>
        /// Строки таблицы
        /// </summary>
        public IEnumerable<Report0010SummaryTableRowViewModel> Rows { get; set; }

        /// <summary>
        /// Итоговая сумма принятых платежей
        /// </summary>                
        public string TotalDealPaymentFromClientSumString { get { return Rows.Sum(x => x.DealPaymentFromClientSum).ForDisplay(); } }

        /// <summary>
        /// Итого в т.ч. наличный расчет
        /// </summary>
        public string TotalDealPaymentFromClientCashSumString { get { return Rows.Sum(x => x.DealPaymentFromClientCashSum).ForDisplay(); } }

        /// <summary>
        /// Итого в т.ч. безналичный расчет
        /// </summary>
        public string TotalDealPaymentFromClientCashlessSumString { get { return Rows.Sum(x => x.DealPaymentFromClientCashlessSum).ForDisplay(); } }

        /// <summary>
        /// Итоговая сумма возвращенных платежей
        /// </summary>
        public string TotalDealPaymentToClientSumString { get { return Rows.Sum(x => x.DealPaymentToClientSum).ForDisplay(); } }

        /// <summary>
        /// Итого в т.ч. наличный расчет
        /// </summary>
        public string TotalDealPaymentToClientCashSumString { get { return Rows.Sum(x => x.DealPaymentToClientCashSum).ForDisplay(); } }

        /// <summary>
        /// Итого в т.ч. безналичный расчет
        /// </summary>
        public string TotalDealPaymentToClientCashlessSumString { get { return Rows.Sum(x => x.DealPaymentToClientCashlessSum).ForDisplay(); } }


        public Report0010SummaryTableViewModel()
        {
            Rows = new List<Report0010SummaryTableRowViewModel>();
        }
    }
}
