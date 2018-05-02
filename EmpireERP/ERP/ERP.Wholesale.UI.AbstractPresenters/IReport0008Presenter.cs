using ERP.Infrastructure.Security;
using ERP.Wholesale.UI.ViewModels.Report.Report0008;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IReport0008Presenter
    {
        #region Настройка отчета
        
        /// <summary>
        /// Настройка отчета
        /// </summary>
        Report0008SettingsViewModel Report0008Settings(string backUrl, UserInfo currentUser);

        /// <summary>
        /// Получение списка возможных вариантов задания статусов накладных для вывода в отчет
        /// </summary>
        object GetWaybillOptionList(string waybillTypeId, string dateTypeId, UserInfo currentUser);

        /// <summary>
        /// Получение списка возможных вариантов задания сортировки для вывода в отчет
        /// </summary>
        /// <param name="waybillTypeId">Код типа накландой</param>
        /// <param name="waybillOptionId">Значение поля «Выводить накладные»</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Список возможных вариантов задания сортировки</returns>
        object GetWaybillSortDateTypeList(string waybillTypeId, string waybillOptionId, UserInfo currentUser);

        /// <summary>
        /// Получение формы для выбора клиента
        /// </summary>
        Report0008_ClientSelector GetClientSelector(UserInfo currentUser);

        /// <summary>
        /// Получение формы для выбора поставщиков
        /// </summary>
        Report0008_ProviderSelectorViewModel GetProviderSelector(UserInfo currentUser);

        /// <summary>
        /// Получение списка возможных группировок
        /// </summary>
        object GetWaybillGroupingTypeList(string waybillTypeId, UserInfo currentUser);

        /// <summary>
        /// Получение списка возможных типов дат
        /// </summary>
        object GetWaybillDateTypeList(string waybillTypeId, UserInfo currentUser);

        #endregion

        #region Построение отчета
        
        /// <summary>
        /// Построение отчета
        /// </summary>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <returns>Модель отчета</returns>
        Report0008ViewModel Report0008(Report0008SettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Экспорт отчета в Excel
        /// </summary>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="currentUser">Текущий пользователь</param>
        byte[] Report0008ExportToExcel(Report0008SettingsViewModel settings, UserInfo currentUser);

        #endregion
    }
}
