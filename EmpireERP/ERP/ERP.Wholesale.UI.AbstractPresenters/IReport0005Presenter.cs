using ERP.Infrastructure.Security;
using ERP.Wholesale.UI.ViewModels.Report.Report0005;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IReport0005Presenter
    {
        Report0005SettingsViewModel Report0005Settings(string backURL, UserInfo currentUser);
        Report0005ViewModel Report0005(Report0005SettingsViewModel settings, UserInfo currentUser);
    }
}
