using ERP.Infrastructure.Security;
using ERP.Wholesale.UI.ViewModels.Report.Report0010;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IReport0010Presenter
    {
        Report0010SettingsViewModel Report0010Settings(string backUrl, UserInfo currentUser);

        Report0010ViewModel Report0010(Report0010SettingsViewModel settings, UserInfo currentUser);

        byte[] Report0010ExportToExcel(Report0010SettingsViewModel settings, UserInfo currentUser);
    }
}
