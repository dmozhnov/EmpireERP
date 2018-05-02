using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;
using ERP.Wholesale.UI.ViewModels.Organization;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ProviderOrganizationController : WholesaleController
    {
        #region Поля

        private readonly IProviderOrganizationPresenter providerOrganizationPresenter;

        #endregion

        #region Конструктор

        public ProviderOrganizationController(IProviderOrganizationPresenter providerOrganizationPresenter)
        {   
          this.providerOrganizationPresenter = providerOrganizationPresenter;
        }

        #endregion

        #region Вывод основных деталей

        /// <summary>
        /// Вывод страницы деталей организации
        /// </summary>
        [HttpGet]
        public ActionResult Details(int id, string backURL)
        {
            try
            {
                var model = providerOrganizationPresenter.Details(id, backURL, UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Вывод главных деталей организации
        /// </summary>
        /// <param name="providerOrganizationId">Идентификатор организации</param>
        [HttpGet]
        public ActionResult ShowMainDetails(int? providerOrganizationId)
        {
            try
            {
                ValidationUtils.NotNull(providerOrganizationId, "Организация указана неверно.");

                return PartialView("ProviderOrganizationMainDetails", providerOrganizationPresenter.GetMainDetails(providerOrganizationId.Value, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Создание гридов

        #region Грид "Закупки у организации"

        /// <summary>
        /// Получение грида "Закупки у поставщика"
        /// </summary>
        /// <param name="state">Состояние грида</param>
        [HttpPost]
        public ActionResult ShowReceiptWaybillGrid(GridState state)
        {
            try
            {
                var grid = providerOrganizationPresenter.GetReceiptWaybillGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ProviderOrganizationReceiptWaybillGrid", grid);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Грид "Договоры с организацией"

        /// <summary>
        /// Получение грида "Договоры с организацией"
        /// </summary>
        /// <param name="state">Состояние грида</param>
        [HttpPost]
        public ActionResult ShowProviderContractGrid(GridState state)
        {
            try
            {
                var grid = providerOrganizationPresenter.GetProviderContractGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ProviderOrganizationProviderContractGrid", grid);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Грид расчетных счетов

        public ActionResult ShowRussianBankAccountGrid(GridState state)
        {
            try
            {
                var model = providerOrganizationPresenter.GetRussianBankAccountGrid(state, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/RussianBankAccountGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowForeignBankAccountGrid(GridState state)
        {
            try
            {
                var model = providerOrganizationPresenter.GetForeignBankAccountGrid(state, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/ForeignBankAccountGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Работа с расчетным счетом

        /// <summary>
        /// Создание нового счета
        /// </summary>
        [HttpGet]
        public ActionResult AddRussianBankAccount(int providerOrganizationId)
        {
            try
            {
                var model = providerOrganizationPresenter.AddRussianBankAccount(providerOrganizationId, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/RussianBankAccountEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }            
        }

        /// <summary>
        /// Создание нового счета в иностранном банке
        /// </summary>
        [HttpGet]
        public ActionResult AddForeignBankAccount(int providerOrganizationId)
        {
            try
            {
                var model = providerOrganizationPresenter.AddForeignBankAccount(providerOrganizationId, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/ForeignBankAccountEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }            
        }

        /// <summary>
        /// Редактирование счета
        /// </summary>
        [HttpGet]
        public ActionResult EditRussianBankAccount(int providerOrganizationId, int bankAccountId)
        {
            try
            {
                var model = providerOrganizationPresenter.EditRussianBankAccount(providerOrganizationId, bankAccountId, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/RussianBankAccountEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование счета в иностранном банке
        /// </summary>
        [HttpGet]
        public ActionResult EditForeignBankAccount(int providerOrganizationId, int bankAccountId)
        {
            try
            {
                var model = providerOrganizationPresenter.EditForeignBankAccount(providerOrganizationId, bankAccountId, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/ForeignBankAccountEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование расчетного счета
        /// </summary>        
        [HttpPost]
        public ActionResult EditRussianBankAccount(RussianBankAccountEditViewModel model)
        {
            try
            {
                providerOrganizationPresenter.SaveRussianBankAccount(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование расчетного счета
        /// </summary>        
        [HttpPost]
        public ActionResult EditForeignBankAccount(ForeignBankAccountEditViewModel model)
        {
            try
            {
                providerOrganizationPresenter.SaveForeignBankAccount(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Удаление расчетного счета
        /// </summary>
        /// <param name="providerOrganizationId">Код организации поставщика</param>
        /// <param name="bankAccountId">Код расчетного счета</param>
        [HttpPost]
        public ActionResult RemoveRussianBankAccount(int providerOrganizationId, int bankAccountId)
        {
            try
            {
                providerOrganizationPresenter.RemoveRussianBankAccount(providerOrganizationId, bankAccountId, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Удаление расчетного счета в иностранном банке
        /// </summary>
        /// <param name="providerOrganizationId">Код организации поставщика</param>
        /// <param name="bankAccountId">Код расчетного счета</param>
        [HttpPost]
        public ActionResult RemoveForeignBankAccount(int providerOrganizationId, int bankAccountId)
        {
            try
            {
                providerOrganizationPresenter.RemoveForeignBankAccount(providerOrganizationId, bankAccountId, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Редактирование организации

        /// <summary>
        /// Редактирование деталей организации
        /// </summary>
        [HttpGet]
        public ActionResult Edit(int? providerOrganizationId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(providerOrganizationId, "Неверное значение входного параметра.");

                var model = providerOrganizationPresenter.Edit(providerOrganizationId.Value, UserSession.CurrentUserInfo);

                if (model is JuridicalPersonEditViewModel)
                {
                    return PartialView("~/Views/EconomicAgent/JuridicalPersonEdit.ascx", model);
                }
                else
                {
                    return PartialView("~/Views/EconomicAgent/PhysicalPersonEdit.ascx", model);
                }
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование деталей организации
        /// </summary>
        [HttpPost]
        public ActionResult EditJuridicalPerson(JuridicalPersonEditViewModel model)
        {
            try
            {
                providerOrganizationPresenter.SaveJuridicalPerson(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование деталей организации
        /// </summary>
        [HttpPost]
        public ActionResult EditPhysicalPerson(PhysicalPersonEditViewModel model)
        {
            try
            {
                providerOrganizationPresenter.SavePhysicalPerson(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление организации

        /// <summary>
        /// Удаление организации поставщика
        /// </summary>
        /// <param name="providerOrganizationId">Код организации поставщика</param>
        [HttpPost]
        public ActionResult Delete(int? providerOrganizationId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(providerOrganizationId, "Неверное значение входного параметра.");

                providerOrganizationPresenter.Delete(providerOrganizationId.Value, UserSession.CurrentUserInfo);
                
                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
    }
}
