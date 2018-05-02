using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;
using ERP.Wholesale.UI.Web.Infrastructure;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ProviderTypeController : WholesaleController
    {
        #region Поля

        private readonly IProviderTypePresenter providerTypePresenter;

        #endregion

        #region Конструктор

        public ProviderTypeController(IProviderTypePresenter providerTypePresenter)
        {
            this.providerTypePresenter = providerTypePresenter;
        }

        #endregion

        #region Просмотр списка

        public ActionResult List()
        {
            try
            {
                var model = providerTypePresenter.List(UserSession.CurrentUserInfo);

                return View("List", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProviderTypeGrid(GridState state)
        {
            try
            {
                var model = providerTypePresenter.GetProviderTypeGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ProviderTypeGrid", model);
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
                var model = providerTypePresenter.Create(UserSession.CurrentUserInfo);

                return View("ProviderTypeEdit", model);
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

                var model = providerTypePresenter.Edit(id.Value, UserSession.CurrentUserInfo);

                return View("ProviderTypeEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Edit(BaseDictionaryEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model);

                var result = providerTypePresenter.Save(model, UserSession.CurrentUserInfo);

                return Content(result.ToString());

            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(BaseDictionaryEditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("ProviderTypeEdit", model);
                }

                var obj = providerTypePresenter.Save(model, UserSession.CurrentUserInfo);

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

                providerTypePresenter.Delete(id.Value, UserSession.CurrentUserInfo);

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
                providerTypePresenter.CheckNameUniqueness(name, ValidationUtils.TryGetShort(id));

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