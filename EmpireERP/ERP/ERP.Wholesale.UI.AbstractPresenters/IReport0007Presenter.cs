using ERP.Infrastructure.Security;
using ERP.Wholesale.UI.ViewModels.Report.Report0007;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IReport0007Presenter
    {
        /// <summary>
        /// Настройка отчета
        /// </summary>
        /// <param name="backUrl"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        Report0007SettingsViewModel Report0007Settings(string backUrl, UserInfo currentUser);

        /// <summary>
        /// Построение отчета
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        Report0007ViewModel Report0007(Report0007SettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Выгрузка  отчета в Excel
        /// </summary>
        byte[] Report0007ExportToExcel(Report0007SettingsViewModel settings, UserInfo currentUser);
    }
}
