using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0002
{
    public class Report0002ViewModel
    {
        // Модели итоговых таблиц
        public Report0002_SummaryTableViewModel StorageSummaryTable { get; set; }
        public Report0002_SummaryTableViewModel AccountOrganizationSummaryTable { get; set; }
        public Report0002_SummaryTableViewModel ClientSummaryTable { get; set; }
        public Report0002_SummaryTableViewModel ClientOrganizationSummaryTable { get; set; }
        public Report0002_SummaryTableViewModel ArticleGroupSummaryTable { get; set; }
        public Report0002_SummaryTableViewModel TeamSummaryTable { get; set; }
        public Report0002_SummaryTableViewModel UserSummaryTable { get; set; }
        public Report0002_SummaryTableViewModel ContractorSummaryTable { get; set; }
        
        /// <summary>
        /// Развернутая таблица
        /// </summary>
        public Report0002_DetailsTableViewModel DetailsTable { get; set; }

        public bool ShowStorageSummaryTable { get; set; }
        public bool ShowAccountOrganizationSummaryTable { get; set; }
        public bool ShowClientSummaryTable { get; set; }
        public bool ShowClientOrganizationSummaryTable { get; set; }
        public bool ShowArticleGroupSummaryTable { get; set; }
        public bool ShowTeamSummaryTable { get; set; }
        public bool ShowUserSummaryTable { get; set; }
        public bool ShowContractorSummaryTable { get; set; }

        public bool ShowDetailsTable { get; set; }

        public bool ShowShortDetailsTable { get; set; }

        /// <summary>
        /// Дата начала интервала отчета
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Дата завершения интервала  отчета
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Пользователь, построивший отчет
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Признак вывода МХ в столбцах
        /// </summary>
        public bool AreStoragesInColumns { get; set; }

        public Report0002ViewModel()
        {
            StorageSummaryTable = new Report0002_SummaryTableViewModel();
            AccountOrganizationSummaryTable = new Report0002_SummaryTableViewModel();
            ClientSummaryTable = new Report0002_SummaryTableViewModel();
            ClientOrganizationSummaryTable = new Report0002_SummaryTableViewModel();
            ArticleGroupSummaryTable = new Report0002_SummaryTableViewModel();
            TeamSummaryTable = new Report0002_SummaryTableViewModel();
            UserSummaryTable = new Report0002_SummaryTableViewModel();
            ContractorSummaryTable = new Report0002_SummaryTableViewModel();

            DetailsTable = new Report0002_DetailsTableViewModel();
        }
    }
}
