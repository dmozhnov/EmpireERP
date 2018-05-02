namespace ERP.Wholesale.UI.ViewModels.Report.Report0010
{
    public class Report0010ViewModel : BaseReportViewModel
    {
        /// <summary>
        /// Настройки отчета
        /// </summary>
        public Report0010SettingsViewModel Settings { get; set; }

        /// <summary>
        /// Сводная информация по клиентам
        /// </summary>
        public Report0010SummaryTableViewModel ClientSummary { get; set; }

        /// <summary>
        /// Сводная информация по организациям клиентов
        /// </summary>
        public Report0010SummaryTableViewModel ClientOrganizationSummary { get; set; }

        /// <summary>
        /// Сводная информация по собственным организациям
        /// </summary>
        public Report0010SummaryTableViewModel AccountOrganizationSummary { get; set; }

        /// <summary>
        /// Сводная информация по договорам с клиентами
        /// </summary>
        public Report0010SummaryTableViewModel ClientContractSummary { get; set; }

        /// <summary>
        /// Сводная информация по командам
        /// </summary>
        public Report0010SummaryTableViewModel TeamSummary { get; set; }

        /// <summary>
        /// Сводная информация по пользователям
        /// </summary>
        public Report0010SummaryTableViewModel UserSummary { get; set; }

        /// <summary>
        /// Оплаты наличными
        /// </summary>
        public Report0010DetailsTableViewModel CashPaymentsDetailsTable { get; set; }
        
        /// <summary>
        /// Оплаты безналичными
        /// </summary>
        public Report0010DetailsTableViewModel CashlessPaymentsDetailsTable { get; set; }
        
        /// <summary>
        /// Оплаты безналичными от третьих лиц
        /// </summary>
        public Report0010DetailsTableViewModel ThirdPartyCashlessPaymentsDetailsTable { get; set; }

        /// <summary>
        /// Все оплаты
        /// </summary>
        public Report0010DetailsTableViewModel AllPaymentsDetailsTable { get; set; }


        public Report0010ViewModel()
        {
            ClientSummary = new Report0010SummaryTableViewModel();
            ClientOrganizationSummary = new Report0010SummaryTableViewModel();
            AccountOrganizationSummary = new Report0010SummaryTableViewModel();
            ClientContractSummary = new Report0010SummaryTableViewModel();
            TeamSummary = new Report0010SummaryTableViewModel();
            UserSummary = new Report0010SummaryTableViewModel();

            CashPaymentsDetailsTable = new Report0010DetailsTableViewModel();
            CashlessPaymentsDetailsTable = new Report0010DetailsTableViewModel();
            ThirdPartyCashlessPaymentsDetailsTable = new Report0010DetailsTableViewModel();
            AllPaymentsDetailsTable = new Report0010DetailsTableViewModel();
        }
    }
}
