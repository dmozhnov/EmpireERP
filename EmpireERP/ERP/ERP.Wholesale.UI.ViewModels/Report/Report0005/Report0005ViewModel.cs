using System.Collections.Generic;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0005
{
    public class Report0005ViewModel
    {
        public string CreatedBy { get; set; }
        public Report0005SettingsViewModel Settings { get; set; }
        public string ReportName { get; set; }

        public string ArticleName { get; set; }

        public Report0005Permissions Permissions { get; set; }
        public List<Report0005ItemViewModel> Items { get; set; }

        public Report0005ViewModel()
        {
            Permissions = new Report0005Permissions();
            Settings = new Report0005SettingsViewModel();
        }

        public Report0005ViewModel(Report0005SettingsViewModel settings)
        {
            Permissions = new Report0005Permissions();
            Settings = settings;
        }
    }
}