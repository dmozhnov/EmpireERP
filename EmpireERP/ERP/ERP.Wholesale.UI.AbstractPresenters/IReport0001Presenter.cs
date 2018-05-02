using ERP.Infrastructure.Security;
using ERP.Wholesale.UI.ViewModels.Report.Report0001;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IReport0001Presenter
    {        
        /// <summary>
        /// Настройки отчета "Наличие товаров на местах хранения".
        /// </summary>        
        Report0001SettingsViewModel Report0001Settings(string backURL, UserInfo currentUser);

        /// <summary>
        /// Отчет "Наличие товаров на местах хранения".
        /// </summary>  
        Report0001ViewModel Report0001(Report0001SettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Выгрузка отчета "Наличие товаров на местах хранения" в Excel.
        /// </summary>  
        byte[] Report0001ExportToExcel(Report0001SettingsViewModel settings, UserInfo currentUser);
    }
}
