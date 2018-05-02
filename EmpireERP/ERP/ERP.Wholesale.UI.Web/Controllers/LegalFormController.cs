using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;
using ERP.Wholesale.UI.ViewModels.LegalForm;
using ERP.Wholesale.UI.Web.Infrastructure;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class LegalFormController : WholesaleController
    {
        #region Поля

        private readonly ILegalFormPresenter legalFormPresenter;

        #endregion

        #region Конструктор

        public LegalFormController(ILegalFormPresenter legalFormPresenter)
        {
            this.legalFormPresenter = legalFormPresenter;
        }

        #endregion

        #region Просмотр списка

        public ActionResult List()
        {
            try
            {
                var model = legalFormPresenter.List(UserSession.CurrentUserInfo);

                return View("List", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowLegalFormGrid(GridState state)
        {
            try
            {
                var model = legalFormPresenter.GetLegalFormGrid(state, UserSession.CurrentUserInfo);

                return PartialView("LegalFormGrid", model);
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
                var model = legalFormPresenter.Create(UserSession.CurrentUserInfo);

                return View("LegalFormEdit", model);
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

                var model = legalFormPresenter.Edit(id.Value, UserSession.CurrentUserInfo);

                return View("LegalFormEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Edit(LegalFormEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model);

                var result = legalFormPresenter.Save(model, UserSession.CurrentUserInfo);

                return Content(result.ToString());

            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(LegalFormEditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("LegalFormEdit", model);
                }

                var obj = legalFormPresenter.Save(model, UserSession.CurrentUserInfo);

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

                legalFormPresenter.Delete(id.Value, UserSession.CurrentUserInfo);

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
                legalFormPresenter.CheckNameUniqueness(name, ValidationUtils.TryGetShort(id));

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