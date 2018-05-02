using System.Collections.Generic;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0003
{
    public class Report0003ViewModel
    {
        public string CreatedBy { get; set; }
        public Report0003SettingsViewModel Settings { get; set; }
        public int IncomingDocumentCount { get; set; }
        public int OutgoingDocumentCount { get; set; }
        
        /// <summary>
        /// Показатели отчета по товародвижению
        /// </summary>
        public IList<Report0003ItemViewModel> ReportItems { get; set; }

        /// <summary>
        /// Показатели отчета по товародвижению - приходы
        /// </summary>
        public IList<Report0003ItemViewModel> IncomingItems { get; set; }

        /// <summary>
        /// Показатели отчета по товародвижению- расходы
        /// </summary>
        public IList<Report0003ItemViewModel> OutgoingItems { get; set; }

        /// <summary>
        /// Показатели отчета по реестрам цен
        /// </summary>
        public IList<Report0003ItemViewModel> ArticleAccountingPriceChangeItems { get; set; }
        
        /// <summary>
        /// Места хранения
        /// </summary>
        [DisplayName("Места хранения")]
        public string StorageNames { get; set; }

        /// <summary>
        /// Сальдо на начало периода
        /// </summary>
        [DisplayName("Сальдо на начало периода")]
        public Report0003ItemViewModel StartBalance { get; set; }

        /// <summary>
        /// Сальдо на конец периода
        /// </summary>
        [DisplayName("Сальдо на конец периода")]
        public Report0003ItemViewModel EndBalance { get; set; }

        public bool AllowToViewPurchaseCosts { get; set; }

        public Report0003ViewModel()
        {
            Settings = new Report0003SettingsViewModel();
            IncomingItems = new List<Report0003ItemViewModel>();
            OutgoingItems = new List<Report0003ItemViewModel>();
            ArticleAccountingPriceChangeItems = new List<Report0003ItemViewModel>();            
        }
    }
}
