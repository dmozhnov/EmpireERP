using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.MeasureUnit;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class MeasureUnitController : WholesaleController
    {
        #region Поля
        
        private readonly IMeasureUnitPresenter measureUnitPresenter;
        
        #endregion

        #region Конструктор
        
        public MeasureUnitController(IMeasureUnitPresenter measureUnitPresenter)
        {
            this.measureUnitPresenter = measureUnitPresenter;
        }
        
        #endregion

        #region Методы

        public ActionResult SelectMeasureUnit()
        {
            try
            {
                var model = measureUnitPresenter.SelectMeasureUnit(UserSession.CurrentUserInfo);

                return View("MeasureUnitSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowSelectMeasureUnitGrid(GridState state)
        {
            try
            {
                var model = measureUnitPresenter.GetMeasureUnitSelectGrid(state);

                return View("MeasureUnitSelectGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult List()
        {
            try
            {
                return View(measureUnitPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowMeasureUnitGrid(GridState state)
        {
            try
            {
                return PartialView("MeasureUnitGrid", measureUnitPresenter.GetMeasureUnitGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                return PartialView("MeasureUnitEdit", measureUnitPresenter.Create(UserSession.CurrentUserInfo));
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
                ValidationUtils.NotNull(id, "Неверное значение входного параметра.");

                return PartialView("MeasureUnitEdit", measureUnitPresenter.Edit(id.Value, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(MeasureUnitEditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("MeasureUnitEdit", model);
                }

                var obj = measureUnitPresenter.Save(model, UserSession.CurrentUserInfo);

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

                measureUnitPresenter.Delete(id.Value, UserSession.CurrentUserInfo);

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
