using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Role;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class RoleController : WholesaleController
    {
        #region Поля

        private readonly IRolePresenter rolePresenter;

        #endregion

        #region Конструкторы

        public RoleController(IRolePresenter rolePresenter)
        {
            this.rolePresenter = rolePresenter;
        }

        #endregion

        #region Методы

        #region Список

        public ActionResult List()
        {
            try
            {
                return View(rolePresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowRolesGrid(GridState state)
        {
            try
            {
                return PartialView("RolesGrid", rolePresenter.GetRolesGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / редактирование

        public ActionResult Create(string backURL)
        {
            try
            {
                return View("Edit", rolePresenter.Create(backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Edit(string id, string backURL)
        {
            try
            {
                short roleId = ValidationUtils.TryGetShort(id);

                return View("Edit", rolePresenter.Edit(roleId, backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(RoleEditViewModel model)
        {
            try
            {
                return Content(rolePresenter.Save(model, UserSession.CurrentUserInfo).ToString());
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление

        [HttpPost]
        public ActionResult Delete(string roleId)
        {
            try
            {
                rolePresenter.Delete(ValidationUtils.TryGetShort(roleId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Детали

        public ActionResult Details(string id, string backURL)
        {
            try
            {
                short roleId = ValidationUtils.TryGetShort(id);

                return View(rolePresenter.Details(roleId, backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowUsersGrid(GridState state)
        {
            try
            {
                return PartialView("UsersGrid", rolePresenter.GetUsersGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #region Страницы групп прав

        #region Общие права

        public ActionResult GetCommonPermissions(string roleId)
        {
            try
            {
                return PartialView("CommonPermissions", rolePresenter.GetCommonPermissions(ValidationUtils.TryGetShort(roleId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveCommonPermissions(CommonPermissionsViewModel model)
        {
            try
            {
                rolePresenter.SaveCommonPermissions(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Товародвижение

        public ActionResult GetArticleDistributionPermissions(string roleId)
        {
            try
            {
                return PartialView("ArticleDistributionPermissions", rolePresenter.GetArticleDistributionPermissions(ValidationUtils.TryGetShort(roleId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveArticleDistributionPermissions(ArticleDistributionPermissionsViewModel model)
        {
            try
            {
                rolePresenter.SaveArticleDistributionPermissions(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Производство

        public ActionResult GetProductionPermissions(string roleId)
        {
            try
            {
                return PartialView("ProductionPermissions", rolePresenter.GetProductionPermissions(ValidationUtils.TryGetShort(roleId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveProductionPermissions(ProductionPermissionsViewModel model)
        {
            try
            {
                rolePresenter.SaveProductionPermissions(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Реализации

        public ActionResult GetSalesPermissions(string roleId)
        {
            try
            {
                return PartialView("SalesPermissions", rolePresenter.GetSalesPermissions(ValidationUtils.TryGetShort(roleId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveSalesPermissions(SalesPermissionsViewModel model)
        {
            try
            {
                rolePresenter.SaveSalesPermissions(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Справочники

        public ActionResult GetDirectoriesPermissions(string roleId)
        {
            try
            {
                return PartialView("DirectoriesPermissions", rolePresenter.GetDirectoriesPermissions(ValidationUtils.TryGetShort(roleId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveDirectoriesPermissions(DirectoriesPermissionsViewModel model)
        {
            try
            {
                rolePresenter.SaveDirectoriesPermissions(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Пользователи

        public ActionResult GetUsersPermissions(string roleId)
        {
            try
            {
                return PartialView("UsersPermissions", rolePresenter.GetUsersPermissions(ValidationUtils.TryGetShort(roleId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveUsersPermissions(UsersPermissionsViewModel model)
        {
            try
            {
                rolePresenter.SaveUsersPermissions(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Отчеты

        public ActionResult GetReportsPermissions(string roleId)
        {
            try
            {
                return PartialView("ReportsPermissions", rolePresenter.GetReportsPermissions(ValidationUtils.TryGetShort(roleId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveReportsPermissions(ReportsPermissionsViewModel model)
        {
            try
            {
                rolePresenter.SaveReportsPermissions(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Задачи

        public ActionResult GetTaskDistributionPermissions(string roleId)
        {
            try
            {
                return PartialView("TaskDistributionPermissions", rolePresenter.GetTaskDistributionPermissions(ValidationUtils.TryGetShort(roleId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveTaskDistributionPermissions(TaskDistributionPermissionsViewModel model)
        {
            try
            {
                rolePresenter.SaveTaskDistributionPermissions(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #endregion

        #region Выбор роли

        public ActionResult SelectRole(string userId)
        {
            try
            {
                return PartialView("RoleSelector", rolePresenter.SelectRole(ValidationUtils.TryGetInt(userId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowRoleSelectGrid(GridState state)
        {
            try
            {
                return PartialView("RoleSelectGrid", rolePresenter.GetSelectRoleGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / удаление пользователя

        [HttpPost]
        public ActionResult AddUser(string roleId, string userId)
        {
            try
            {
                return Json(rolePresenter.AddUser(ValidationUtils.TryGetShort(roleId), ValidationUtils.TryGetInt(userId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult RemoveUser(string roleId, string userId)
        {
            try
            {
                return Json(rolePresenter.RemoveUser(ValidationUtils.TryGetShort(roleId), ValidationUtils.TryGetInt(userId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
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
