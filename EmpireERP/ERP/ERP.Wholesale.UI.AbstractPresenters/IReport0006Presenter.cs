using ERP.Infrastructure.Security;
using ERP.Wholesale.UI.ViewModels.Report.Report0006;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IReport0006Presenter
    {
        /// <summary>
        /// Создание настроек 6 отчета
        /// </summary>
        /// <param name="backURL">Адрес возврата</param>
        /// <param name="currentUser">Пользователь</param>
        Report0006SettingsViewModel Report0006Settings(string backURL, UserInfo currentUser);

        /// <summary>
        /// Создание настроек печатной формы
        /// </summary>
        /// <param name="clientId">Код клиента (если строится из деталей клиента)</param>
        /// <param name="clientOrganizationId">Код организации клиента (если строится из деталей организации клиента)</param>
        /// <param name="currentUser">Пользователь</param>
        Report0006PrintingFormSettingsViewModel Report0006PrintingFormSettings(int? clientId, int? clientOrganizationId, UserInfo currentUser);

        /// <summary>
        /// Построение 6 отчета "Отчет по взаиморасчетам с клиентами (организациями клиента)"
        /// </summary>
        /// <param name="settings">Настройки</param>
        /// <param name="currentUser">Пользователь</param>
        Report0006ViewModel Report0006(Report0006SettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Выгрузка отчета "Отчет по взаиморасчетам с клиентами (организациями клиента)" в Excel
        /// </summary>
        /// <param name="settings">Настройки</param>
        /// <param name="currentUser">Пользователь</param>
        byte[] Report0006ExportToExcel(Report0006SettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Построение печатной формы "Акт сверки взаиморасчетов"
        /// </summary>
        /// <param name="settings">Настройки</param>
        /// <param name="currentUser">Пользователь</param>
        Report0006PrintingFormListViewModel Report0006PrintingForm(Report0006PrintingFormSettingsViewModel settings, UserInfo currentUser);
    }
}
