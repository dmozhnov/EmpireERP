using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;
using ERP.Wholesale.UI.Web.Infrastructure;
using ERP.Wholesale.UI.ViewModels.Country;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class CountryController : WholesaleController
    {
        #region Поля

        private readonly ICountryPresenter countryPresenter;

        #endregion

        #region Конструктор

        public CountryController(ICountryPresenter countryPresenter)
        {
            this.countryPresenter = countryPresenter;
        }
        #endregion

        #region Просмотр списка

        public ActionResult List()
        {
            try
            {
                var model = countryPresenter.List(UserSession.CurrentUserInfo);

                return View("List", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowCountryGrid(GridState state)
        {
            try
            {
                var model = countryPresenter.GetCountryGrid(state, UserSession.CurrentUserInfo);

                return PartialView("CountryGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult GetList()
        {
            return Json(countryPresenter.GetList(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Добавление

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var model = countryPresenter.Create(UserSession.CurrentUserInfo);

                return View("CountryEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult Edit(short? id)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(id, "Неверное значение входного параметра.");

                var model = countryPresenter.Edit(id.Value, UserSession.CurrentUserInfo);

                return View("CountryEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Edit(CountryEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model);

                var result = countryPresenter.Save(model, UserSession.CurrentUserInfo);

                return Content(result.ToString());

            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(CountryEditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("CountryEdit", model);
                }

                var obj = countryPresenter.Save(model, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Delete(short? id)
        {
            try
            {
                ValidationUtils.NotNull(id, "Неверное значение входного параметра.");

                countryPresenter.Delete(id.Value, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult CheckNameUniqueness(string name, string id)
        {
            try
            {
                countryPresenter.CheckNameUniqueness(name, ValidationUtils.TryGetShort(id));

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
