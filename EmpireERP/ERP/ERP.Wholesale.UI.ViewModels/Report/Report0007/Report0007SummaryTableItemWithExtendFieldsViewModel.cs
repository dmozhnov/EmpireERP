using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0007
{
    public class Report0007SummaryTableItemWithExtendFieldsViewModel: Report0007SummaryTableItemViewModel
    {
        /// <summary>
        /// Неразнесенная сумма оплат
        /// </summary>
        [DisplayName("Неразнесенная сумма оплат")]
        public decimal UndistributionPaymentSum { get; set; }

        /// </summary>
        /// Максимальная просрочка в днях
        /// </summary>
        [DisplayName("Максимальная просрочка (дни)")]
        public string DelayPaymentDays { get; set; }
    }
}
