using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Team;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class TeamController : WholesaleController
    {
        #region Поля

        private readonly ITeamPresenter teamPresenter;

        #endregion

        #region Конструкторы

        public TeamController(ITeamPresenter teamPresenter)
        {
            this.teamPresenter = teamPresenter;
        }

        #endregion

        #region Методы

        #region Список
        
        public ActionResult List()
        {
            try
            {
                return View(teamPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowTeamsGrid(GridState state)
        {
            try
            {
                return PartialView("TeamsGrid", teamPresenter.GetTeamsGrid(state, UserSession.CurrentUserInfo));
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
                return View("Edit", teamPresenter.Create(backURL, UserSession.CurrentUserInfo));
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
                short teamId = ValidationUtils.TryGetShort(id);

                return View("Edit", teamPresenter.Edit(teamId, backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(TeamEditViewModel model)
        {
            try
            {                
                return Content(teamPresenter.Save(model, UserSession.CurrentUserInfo).ToString());
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление

        [HttpPost]
        public ActionResult Delete(string teamId)
        {
            try
            {
                teamPresenter.Delete(ValidationUtils.TryGetShort(teamId), UserSession.CurrentUserInfo);

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
                short teamId = ValidationUtils.TryGetShort(id);

                return View(teamPresenter.Details(teamId, backURL, UserSession.CurrentUserInfo));
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
                return PartialView("UsersGrid", teamPresenter.GetUsersGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowDealsGrid(GridState state)
        {
            try
            {
                return PartialView("DealsGrid", teamPresenter.GetDealsGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowStoragesGrid(GridState state)
        {
            try
            {
                return PartialView("StoragesGrid", teamPresenter.GetStoragesGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProductionOrdersGrid(GridState state)
        {
            try
            {
                return PartialView("ProductionOrdersGrid", teamPresenter.GetProductionOrdersGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Выбор команды

        public ActionResult SelectTeam(string userId)
        {
            try
            {
                return PartialView("TeamSelector", teamPresenter.SelectTeam(ValidationUtils.TryGetInt(userId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowTeamSelectGrid(GridState state)
        {
            try
            {
                return PartialView("TeamSelectGrid", teamPresenter.GetSelectTeamGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / удаление пользователя

        [HttpPost]
        public ActionResult AddUser(string teamId, string userId)
        {
            try
            {
                return Json(teamPresenter.AddUser(ValidationUtils.TryGetShort(teamId), ValidationUtils.TryGetInt(userId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult RemoveUser(string teamId, string userId)
        {
            try
            {
                return Json(teamPresenter.RemoveUser(ValidationUtils.TryGetShort(teamId), ValidationUtils.TryGetInt(userId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / удаление сделки

        [HttpPost]
        public ActionResult AddDeal(string teamId, string dealId)
        {
            try
            {
                return Json(teamPresenter.AddDeal(ValidationUtils.TryGetShort(teamId), ValidationUtils.TryGetInt(dealId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult RemoveDeal(string teamId, string dealId)
        {
            try
            {
                return Json(teamPresenter.RemoveDeal(ValidationUtils.TryGetShort(teamId), ValidationUtils.TryGetInt(dealId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / удаление места хранения

        [HttpGet]
        public ActionResult StoragesList(string teamId)
        {
            try
            {
                return PartialView("StorageSelectList", teamPresenter.GetStoragesList(ValidationUtils.TryGetShort(teamId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult AddStorage(string teamId, string storageId)
        {
            try
            {
                return Json(teamPresenter.AddStorage(ValidationUtils.TryGetShort(teamId), ValidationUtils.TryGetShort(storageId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult RemoveStorage(string teamId, string storageId)
        {
            try
            {
                return Json(teamPresenter.RemoveStorage(ValidationUtils.TryGetShort(teamId), ValidationUtils.TryGetShort(storageId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }        

        #endregion

        #region Добавление / удаление заказа на производство
               
        [HttpPost]
        public ActionResult AddProductionOrder(string teamId, string orderId)
        {
            try
            {
                return Json(teamPresenter.AddProductionOrder(
                    ValidationUtils.TryGetShort(teamId), ValidationUtils.TryGetGuid(orderId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult RemoveProductionOrder(string teamId, string orderId)
        {
            try
            {
                return Json(teamPresenter.RemoveProductionOrder(
                    ValidationUtils.TryGetShort(teamId), ValidationUtils.TryGetGuid(orderId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
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
