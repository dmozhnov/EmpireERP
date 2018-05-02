using ERP.Infrastructure.Security;
using ERP.Wholesale.UI.ViewModels.Export._1C;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    /// <summary>
    /// Презентер для выгрузки операций в 1С
    /// </summary>
    public interface IExportTo1CPresenter
    {
        /// <summary>
        /// Получение модели страницы настроек экспорта в 1С
        /// </summary>
        ExportTo1CSettingsViewModel ExportTo1CSettings(UserInfo currentUser);
        
        /// <summary>
        /// Выгрузка операций в 1С в виде файла Excel
        /// </summary>
        byte[] ExportOperationsTo1C(ExportTo1CSettingsViewModel settings, UserInfo currentUser);

        ExportTo1CSettingsSelectorViewModel GetCommissionaireOrganizationsList();
        ExportTo1CSettingsSelectorViewModel GetReturnsFromCommissionaireOrganizationsList();
        ExportTo1CSettingsSelectorViewModel GetReturnsAcceptedByCommissionaireOrganizationsList();
        ExportTo1CSettingsSelectorViewModel GetConsignorOrganizationsList();
    }
}
