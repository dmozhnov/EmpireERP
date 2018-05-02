using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.EmployeePost;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class EmployeePostController : WholesaleController
    {
        #region Поля

        private readonly IEmployeePostPresenter employeePostPresenter;

        #endregion

        #region Конструктор

        public EmployeePostController(IEmployeePostPresenter employeePostPresenter)
        {
            this.employeePostPresenter = employeePostPresenter;
        }
        #endregion

        #region Просмотр списка

        public ActionResult List()
        {
            try
            {
                var model = employeePostPresenter.List(UserSession.CurrentUserInfo);

                return View("List", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowEmployeePostGrid(GridState state)
        {
            try
            {
                var model = employeePostPresenter.GetEmployeePostGrid(state, UserSession.CurrentUserInfo);

                return PartialView("EmployeePostGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult GetEmployeePosts()
        {
            return Json(employeePostPresenter.GetEmployeePosts(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Добавление

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var model = employeePostPresenter.Create(UserSession.CurrentUserInfo);

                return View("EmployeePostEdit", model);
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

                var model = employeePostPresenter.Edit(id.Value, UserSession.CurrentUserInfo);

                return View("EmployeePostEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Edit(EmployeePostEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model);

                var result = employeePostPresenter.Save(model, UserSession.CurrentUserInfo);

                return Content(result.ToString());

            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(EmployeePostEditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("EmployeePostEdit", model);
                }

                var obj = employeePostPresenter.Save(model, UserSession.CurrentUserInfo);

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

                employeePostPresenter.Delete(id.Value, UserSession.CurrentUserInfo);

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
                employeePostPresenter.CheckNameUniqueness(name, ValidationUtils.TryGetShort(id));

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
