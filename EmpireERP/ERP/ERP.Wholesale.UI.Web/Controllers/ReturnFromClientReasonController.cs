using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ReturnFromClientReasonController : WholesaleController
    {
        #region Поля

        private readonly IReturnFromClientReasonPresenter returnFromClientReasonPresenter;

        #endregion

        #region Конструктор

        public ReturnFromClientReasonController(IReturnFromClientReasonPresenter returnFromClientReasonPresenter)
        {
            this.returnFromClientReasonPresenter = returnFromClientReasonPresenter;
        }
        #endregion

        #region Просмотр списка

        public ActionResult List()
        {
            try
            {
                var model = returnFromClientReasonPresenter.List(UserSession.CurrentUserInfo);

                return View("List", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowReturnFromClientReasonGrid(GridState state)
        {
            try
            {
                var model = returnFromClientReasonPresenter.GetReturnFromClientReasonGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReturnFromClientReasonGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult GetReturnFromClientReasons()
        {
            return Json(returnFromClientReasonPresenter.GetReturnFromClientReasons(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Добавление

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var model = returnFromClientReasonPresenter.Create(UserSession.CurrentUserInfo);

                return View("ReturnFromClientReasonEdit", model);
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

                var model = returnFromClientReasonPresenter.Edit(id.Value, UserSession.CurrentUserInfo);

                return View("ReturnFromClientReasonEdit", model);
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

                var result = returnFromClientReasonPresenter.Save(model, UserSession.CurrentUserInfo);

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
                    return PartialView("ReturnFromClientReasonEdit", model);
                }

                var obj = returnFromClientReasonPresenter.Save(model, UserSession.CurrentUserInfo);

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

                returnFromClientReasonPresenter.Delete(id.Value, UserSession.CurrentUserInfo);

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
                returnFromClientReasonPresenter.CheckNameUniqueness(name, ValidationUtils.TryGetShort(id));

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
