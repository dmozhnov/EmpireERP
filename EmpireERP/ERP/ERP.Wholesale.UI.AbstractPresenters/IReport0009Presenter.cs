using ERP.Wholesale.UI.ViewModels.Report.Report0009;
using ERP.Infrastructure.Security;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IReport0009Presenter
    {
        ///<summary>
        /// Настройка отчета
        /// </summary>
        Report0009SettingsViewModel Report0009Settings(string backUrl, UserInfo currentUser);

        /// <summary>
        /// Отчет
        /// </summary>
        Report0009ViewModel Report0009(Report0009SettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Выгрузка в Excel
        /// </summary>
        byte[] Report0009ExportToExcel(Report0009SettingsViewModel settings, UserInfo currentUser);
    }
}
