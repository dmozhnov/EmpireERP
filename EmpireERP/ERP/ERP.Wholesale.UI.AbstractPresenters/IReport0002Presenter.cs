using ERP.Infrastructure.Security;
using ERP.Wholesale.UI.ViewModels.Report.Report0002;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IReport0002Presenter
    {
        Report0002SettingsViewModel Report0002Settings(string backURL, UserInfo currentUser);
        Report0002ViewModel Report0002(Report0002SettingsViewModel settings, UserInfo currentUser);

        byte[] Report0002ExportToExcel(Report0002SettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Получение списка МХ для построения отчета
        /// </summary>
        /// <param name="inAccountingPrice">Признак вывода УЦ</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель списка МХ для выбора</returns>
        Report0002StorageSelectorViewModel GetStorageSelector(string inAccountingPrice, UserInfo currentUser);

    }
}
