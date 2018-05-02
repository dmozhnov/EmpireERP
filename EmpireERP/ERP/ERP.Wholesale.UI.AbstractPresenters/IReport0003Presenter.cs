using ERP.Infrastructure.Security;
using ERP.Wholesale.UI.ViewModels.Report.Report0003;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IReport0003Presenter
    {
        Report0003SettingsViewModel Report0003Settings(string backURL, UserInfo currentUser);
        Report0003ViewModel Report0003(Report0003SettingsViewModel settings, UserInfo currentUser);

        byte[] Report0003ExportToExcel(Report0003SettingsViewModel settings, UserInfo currentUser);
    }
}
