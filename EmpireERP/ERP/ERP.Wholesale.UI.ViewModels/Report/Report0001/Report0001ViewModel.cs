using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0001
{
    public class Report0001ViewModel
    {
        public Report0001SettingsViewModel Settings { get; set; }
        
        // для Report0001_1
        public Report0001_1SummaryTableViewModel StorageSummary { get; set; }
        public Report0001_1SummaryTableViewModel ArticleGroupSummary { get; set; }
        public Report0001_1SummaryTableViewModel AccountOrganizationSummary { get; set; }

        public List<Report0001_1ItemViewModel> Report0001_1Items { get; set; }

        // для Report0001_2
        public IEnumerable<Report0001_2ItemViewModel> Report0001_2Items { get; set; }


        /// <summary>
        /// Средние закупочные цены по каждому товару
        /// </summary>
        public Dictionary<int, decimal> AverageArticlePurchaseCosts { get; set; }
        
        /// <summary>
        /// Средние учетные цены по каждому товару
        /// </summary>
        public Dictionary<int, decimal?> AverageArticleAccountingPrices { get; set; }        


        public string CreatedBy { get; set; }


        public Report0001ViewModel()
        {
            StorageSummary = new Report0001_1SummaryTableViewModel();
            ArticleGroupSummary = new Report0001_1SummaryTableViewModel();
            AccountOrganizationSummary = new Report0001_1SummaryTableViewModel();

            AverageArticlePurchaseCosts = new Dictionary<int, decimal>();
            AverageArticleAccountingPrices = new Dictionary<int, decimal?>();

            Report0001_1Items = new List<Report0001_1ItemViewModel>();
            Report0001_2Items = new List<Report0001_2ItemViewModel>();
        }
    }
}
