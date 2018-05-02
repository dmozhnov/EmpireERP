using System.Data;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Settings;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class SettingPresenter : ISettingPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IUserService userService;
        private readonly ISettingService settingService;

        #endregion

        #region Констурктор

        public SettingPresenter(IUnitOfWorkFactory unitOfWorkFactory, IUserService userService, ISettingService settingService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.userService = userService;
            this.settingService = settingService;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Получение деталей настроек
        /// </summary>
        /// <param name="backUrl">Адрес возврата</param>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <returns></returns>
        public SettingViewModel List(string backUrl, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                ValidationUtils.Assert(user.IsSystemAdmin, "Недостаточно прав для изменения настроек.");

                var setting = settingService.Get();

                return new SettingViewModel()
                {
                    Title = "Настройки аккаунта",
                    BackURL = backUrl,
                    GroupTitleForReadyToAcceptState = "Использовать статус «Готово к проводке»",
                    UseReadyToAcceptStateForChangeOwnerWaybill = setting.UseReadyToAcceptStateForChangeOwnerWaybill ? "1" : "0",
                    UseReadyToAcceptStateForExpenditureWaybill = setting.UseReadyToAcceptStateForExpenditureWaybill ? "1" : "0",
                    UseReadyToAcceptStateForMovementWaybill = setting.UseReadyToAcceptStateForMovementWaybill ? "1" : "0",
                    UseReadyToAcceptStateForReturnFromClientWaybill = setting.UseReadyToAcceptStateForReturnFromClientWaybill ? "1" : "0",
                    UseReadyToAcceptStateForWriteOffWaybill = setting.UseReadyToAcceptStateForWriteOffWaybill ? "1" : "0"
                };
            }
        }

        /// <summary>
        /// Сохранение настроек
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        public void Save(SettingViewModel model, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                ValidationUtils.Assert(user.IsSystemAdmin, "Недостаточно прав для изменения настроек.");
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                var setting = settingService.Get(); // Получаем объект настроек и бновляем поля, т.к. в нем содержатся и другие данные (которые не изменяются через интерфейс).

                setting.UseReadyToAcceptStateForChangeOwnerWaybill = ValidationUtils.TryGetBool(model.UseReadyToAcceptStateForChangeOwnerWaybill);
                setting.UseReadyToAcceptStateForExpenditureWaybill = ValidationUtils.TryGetBool(model.UseReadyToAcceptStateForExpenditureWaybill);
                setting.UseReadyToAcceptStateForMovementWaybill = ValidationUtils.TryGetBool(model.UseReadyToAcceptStateForMovementWaybill);
                setting.UseReadyToAcceptStateForReturnFromClientWaybill = ValidationUtils.TryGetBool(model.UseReadyToAcceptStateForReturnFromClientWaybill);
                setting.UseReadyToAcceptStateForWriteOffWaybill = ValidationUtils.TryGetBool(model.UseReadyToAcceptStateForWriteOffWaybill);

                uow.Commit();
            }
        }    

        #endregion
    }
}
