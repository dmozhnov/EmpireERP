namespace ERP.Wholesale.UI.ViewModels.Report.Report0009
{
    public class Report0009ViewModel
    {
        /// <summary>
        /// Автор отчета
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Дата создания отчета
        /// </summary>
        public string CreationDate { get; set; }

        /// <summary>
        /// Настройки отчета
        /// </summary>
        public Report0009SettingsViewModel Settings { get; set; }

        /// <summary>
        /// Сводная таблица приходов по местам хранения
        /// </summary>
        public Report0009SummaryTableViewModel StorageSummaryTable { get; set; }

        /// <summary>
        /// Название типа даты
        /// </summary>
        public string DateTypeName { get; set; }

        /// <summary>
        /// Сводная таблица приходов по организациям приемщикам
        /// </summary>
        public Report0009SummaryTableViewModel AccountOrganizationSummaryTable { get; set; }

        /// <summary>
        /// Сводная таблица приходов по поставщикам
        /// </summary>
        public Report0009SummaryTableViewModel ProviderSummaryTable { get; set; }

        /// <summary>
        /// Сводная таблица приходов по организациям поставщика
        /// </summary>
        public Report0009SummaryTableViewModel ProviderOrganizationSummaryTable { get; set; }

        /// <summary>
        /// Сводная таблица приходов по группам товаров
        /// </summary>
        public Report0009SummaryTableViewModel ArticleGroupSummaryTable { get; set; }

        /// <summary>
        /// Сводная таблица приходов по пользователям (кураторам приходов) 
        /// </summary>
        public Report0009SummaryTableViewModel UserSummaryTable { get; set; }

        /// <summary>
        /// Разрешено ли просматривать закупочные цены
        /// </summary>
        public bool AllowToViewPurchaseCosts { get; set; }

        /// <summary>
        /// Развернутая таблица
        /// </summary>
        public Report0009DetailTableViewModel ReceiptWaybillRowDetailTable { get; set; }
       
        /// <summary>
        /// Развернутая таблица с расхождениями
        /// </summary>
        public Report0009DetailTableViewModel ReceiptWaybillRowWithDivergencesAfterReceiptDetailTable { get; set; }

        public Report0009ViewModel()
        {
            ReceiptWaybillRowWithDivergencesAfterReceiptDetailTable = new Report0009DetailTableViewModel();
            ReceiptWaybillRowDetailTable = new Report0009DetailTableViewModel();

            StorageSummaryTable = new Report0009SummaryTableViewModel();
            AccountOrganizationSummaryTable = new Report0009SummaryTableViewModel();
            ProviderSummaryTable = new Report0009SummaryTableViewModel();
            ProviderOrganizationSummaryTable = new Report0009SummaryTableViewModel();
            ArticleGroupSummaryTable = new Report0009SummaryTableViewModel();
            UserSummaryTable = new Report0009SummaryTableViewModel();
        }
    }
}
