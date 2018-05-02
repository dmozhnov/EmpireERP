using ERP.Infrastructure.Security;
using ERP.Wholesale.UI.ViewModels.Report.Report0004;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IReport0004Presenter
    {
        Report0004SettingsViewModel Report0004Settings(string backURL, UserInfo currentUser);
        Report0004ViewModel Report0004(Report0004SettingsViewModel settings, UserInfo currentUser);
        byte[] Report0004ExportToExcel(Report0004SettingsViewModel settings, UserInfo currentUser);
    }
}
