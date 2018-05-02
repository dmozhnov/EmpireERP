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
    public class WriteoffReasonController : WholesaleController
    {
        #region Поля

        private readonly IWriteoffReasonPresenter writeoffReasonPresenter;

        #endregion 

        #region Конструктор

        public WriteoffReasonController(IWriteoffReasonPresenter writeoffReasonPresenter)
        {
            this.writeoffReasonPresenter = writeoffReasonPresenter;
        }
        #endregion

        #region Просмотр списка

        public ActionResult List()
        {
            try
            {
                var model = writeoffReasonPresenter.List(UserSession.CurrentUserInfo);

                return View("List", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowWriteoffReasonGrid(GridState state)
        {
            try
            {
                var model = writeoffReasonPresenter.GetWriteoffReasonGrid(state, UserSession.CurrentUserInfo);

                return PartialView("WriteoffReasonGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult GetWriteoffReasons()
        {
            return Json(writeoffReasonPresenter.GetWriteoffReasons(), JsonRequestBehavior.AllowGet);
        } 

        #endregion

        #region Добавление 

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var model = writeoffReasonPresenter.Create(UserSession.CurrentUserInfo);

                return View("WriteoffReasonEdit", model);
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

                var model = writeoffReasonPresenter.Edit(id.Value, UserSession.CurrentUserInfo);

                return View("WriteoffReasonEdit", model);
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

                var result = writeoffReasonPresenter.Save(model, UserSession.CurrentUserInfo);

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
                    return PartialView("WriteoffReasonEdit", model);
                }

                var obj = writeoffReasonPresenter.Save(model, UserSession.CurrentUserInfo);

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

                writeoffReasonPresenter.Delete(id.Value, UserSession.CurrentUserInfo);

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
                writeoffReasonPresenter.CheckNameUniqueness(name, ValidationUtils.TryGetShort(id));

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
