namespace ERP.Wholesale.UI.ViewModels.Report
{
    public abstract class BaseReportViewModel
    {
        /// <summary>
        /// Заголовок отчета
        /// </summary>
        public string ReportName { get; set; }

        /// <summary>
        /// Кто создал отчет
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Дата составления отчета
        /// </summary>
        public string CreationDate { get; set; }
    }
}
