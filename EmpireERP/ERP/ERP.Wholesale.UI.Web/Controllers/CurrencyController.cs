using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.Web.Infrastructure;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.ViewModels.Currency;
using System.Web.UI;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class CurrencyController : WholesaleController
    {
        #region Поля

        private readonly ICurrencyPresenter currencyPresenter;

        #endregion

        #region Конструкторы

        public CurrencyController(ICurrencyPresenter currencyPresenter)
        {
            this.currencyPresenter = currencyPresenter;
        }

        #endregion

        #region Методы

        // TODO: заменить, где надо, View на PartialView

        #region Список
        
        /// <summary>
        /// Список валют
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            try
            {
                return View(currencyPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Отрисовка грида валют
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShowCurrencyGrid(GridState state)
        {
            try
            {
                ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

                return View("CurrencyGrid", currencyPresenter.GetCurrencyGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        

        public ActionResult Create()
        {
            try
            {
                return View("CurrencyEdit", currencyPresenter.Create(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Edit(string currencyId)
        {
            try
            {
                var id = ValidationUtils.TryGetShort(currencyId);

                return View("CurrencyEdit", currencyPresenter.Edit(id, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowCurrencyRateGrid(GridState state)
        {
            try
            {
                ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

                return View("CurrencyRateGrid", currencyPresenter.GetCurrencyRateGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(CurrencyEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                var obj = currencyPresenter.Save(model, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Delete(string currencyId)
        {
            try
            {
                currencyPresenter.Delete(ValidationUtils.TryGetShort(currencyId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление курса

        public ActionResult CreateRate(string currencyId)
        {
            try
            {
                return View("CurrencyrateEdit", currencyPresenter.CreateRate(ValidationUtils.TryGetShort(currencyId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование курса валюты
        /// </summary>
        /// <param name="currencyRateId">Код курса валюты</param>
        /// <returns></returns>
        public ActionResult EditRate(string currencyRateId)
        {
            try
            {
                return View("CurrencyRateEdit", currencyPresenter.EditRate(ValidationUtils.TryGetInt(currencyRateId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveRate(CurrencyRateEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model,"Неверное значение входного параметра.");

                currencyPresenter.SaveRate(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Удаление курса валюты
        /// </summary>
        /// <param name="currencyRateId">Код курса валюты</param>
        /// <returns></returns>
        public ActionResult DeleteRate(string currencyRateId)
        {
            try
            {
                var rateId = ValidationUtils.TryGetInt(currencyRateId);
                currencyPresenter.DeleteRate(rateId, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /* Метод пока не используется
        public ActionResult ImportCurrencyRate(string currencyId)
        {
            try
            {
                return View("CurrencyRateEdit", currencyPresenter.ImportCurrencyRate(ValidationUtils.TryGetShort(currencyId)));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }*/

        public ActionResult SelectCurrencyRate(string currencyId, string selectFunctionName)
        {
            try
            {
                ValidationUtils.NotNull(currencyId, "Неверное значение входного параметра.");
                if (currencyId.Length == 0)
                {
                    throw new Exception("Укажите валюту.");
                }

                var model = currencyPresenter.SelectCurrencyRate(ValidationUtils.TryGetShort(currencyId), selectFunctionName);
                
                return View("CurrencyRateSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowCurrencyRateSelectGrid(GridState state)
        {
            try
            {
                ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

                return View("CurrencyRateSelectGrid", currencyPresenter.GetSelectCurrencyRateGrid(state));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Получение курса валюты

        public ActionResult GetCurrentCurrencyRate(string currencyId)
        {
            try
            {
                return Json(currencyPresenter.GetCurrentCurrencyRate(ValidationUtils.TryGetShort(currencyId)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult GetCurrencyRate(string currencyRateId)
        {
            try
            {
                ValidationUtils.NotNull(currencyRateId, "Неверное значение входного параметра.");

                return Json(currencyPresenter.GetCurrencyRate(ValidationUtils.TryGetInt(currencyRateId)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion
    }
}
