using ERP.Infrastructure.Security;
using ERP.Wholesale.UI.ViewModels.Settings;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface ISettingPresenter
    {
        /// <summary>
        /// Получение деталей настроек
        /// </summary>
        /// <param name="backUrl"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        SettingViewModel List(string backUrl, UserInfo currentUser);

        /// <summary>
        /// Сохранение настроек
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        void Save(SettingViewModel model, UserInfo currentUser);
    }
}
