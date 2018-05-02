using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;
using ERP.Wholesale.UI.ViewModels.ValueAddedTax;
using ERP.Wholesale.UI.Web.Infrastructure;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ValueAddedTaxController : WholesaleController
    {
        #region Поля

        private readonly IValueAddedTaxPresenter valueAddedTaxPresenter;

        #endregion

        #region Конструктор

        public ValueAddedTaxController(IValueAddedTaxPresenter valueAddedTaxPresenter)
        {
            this.valueAddedTaxPresenter = valueAddedTaxPresenter;
        }

        #endregion

        #region Просмотр списка

        public ActionResult List()
        {
            try
            {
                var model = valueAddedTaxPresenter.List(UserSession.CurrentUserInfo);

                return View("List", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowValueAddedTaxGrid(GridState state)
        {
            try
            {
                var model = valueAddedTaxPresenter.GetValueAddedTaxGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ValueAddedTaxGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление типа поставщика

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var model = valueAddedTaxPresenter.Create(UserSession.CurrentUserInfo);

                return View("ValueAddedTaxEdit", model);
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

                var model = valueAddedTaxPresenter.Edit(id.Value, UserSession.CurrentUserInfo);

                return View("ValueAddedTaxEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Edit(ValueAddedTaxEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model);

                var result = valueAddedTaxPresenter.Save(model, UserSession.CurrentUserInfo);

                return Content(result.ToString());

            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(ValueAddedTaxEditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("ValueAddedTaxEdit", model);
                }

                var obj = valueAddedTaxPresenter.Save(model, UserSession.CurrentUserInfo);

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

                valueAddedTaxPresenter.Delete(id.Value, UserSession.CurrentUserInfo);

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
                valueAddedTaxPresenter.CheckNameUniqueness(name, ValidationUtils.TryGetShort(id));

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