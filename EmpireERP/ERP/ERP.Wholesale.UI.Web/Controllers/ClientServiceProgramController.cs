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
    public class ClientServiceProgramController : WholesaleController
    {
        #region Поля

        private readonly IClientServiceProgramPresenter clientServiceProgramPresenter;

        #endregion

        #region Конструктор

        public ClientServiceProgramController(IClientServiceProgramPresenter clientServiceProgramPresenter)
        {
            this.clientServiceProgramPresenter = clientServiceProgramPresenter;
        }
        #endregion

        #region Просмотр списка

        public ActionResult List()
        {
            try
            {
                var model = clientServiceProgramPresenter.List(UserSession.CurrentUserInfo);

                return View("List", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowClientServiceProgramGrid(GridState state)
        {
            try
            {
                var model = clientServiceProgramPresenter.GetClientServiceProgramGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ClientServiceProgramGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult GetClientServicePrograms()
        {
            return Json(clientServiceProgramPresenter.GetClientServicePrograms(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Добавление

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var model = clientServiceProgramPresenter.Create(UserSession.CurrentUserInfo);

                return View("ClientServiceProgramEdit", model);
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

                var model = clientServiceProgramPresenter.Edit(id.Value, UserSession.CurrentUserInfo);

                return View("ClientServiceProgramEdit", model);
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

                var result = clientServiceProgramPresenter.Save(model, UserSession.CurrentUserInfo);

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
                    return PartialView("ClientServiceProgramEdit", model);
                }

                var obj = clientServiceProgramPresenter.Save(model, UserSession.CurrentUserInfo);

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

                clientServiceProgramPresenter.Delete(id.Value, UserSession.CurrentUserInfo);

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
                clientServiceProgramPresenter.CheckNameUniqueness(name, ValidationUtils.TryGetShort(id));

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
