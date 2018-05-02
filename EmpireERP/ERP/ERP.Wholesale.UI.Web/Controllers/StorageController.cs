using System;
using System.Collections;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Storage;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class StorageController : WholesaleController
    {
        #region Поля
       
        private readonly IStoragePresenter storagePresenter;

        #endregion

        #region Конструктор

        public StorageController(IStoragePresenter storagePresenter)
          
        {
            this.storagePresenter = storagePresenter;
        }

        #endregion

        #region Список

        public ActionResult List()
        {
            try
            {
                return View(storagePresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowStorageGrid(GridState state)
        {
            try
            {
                GridData data = storagePresenter.GetStorageGrid(state, UserSession.CurrentUserInfo);

                return PartialView("StorageGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Создание, редактирование, удаление

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var model = storagePresenter.Create(UserSession.CurrentUserInfo);

                return PartialView("StorageEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        [HttpGet]
        public ActionResult Edit(short id)
        {
            try
            {
                var model = storagePresenter.Edit(id, UserSession.CurrentUserInfo);

                return PartialView("StorageEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(StorageEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(storagePresenter.Save(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                storagePresenter.Delete(ValidationUtils.TryGetShort(id), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Детали

        [HttpGet]
        public ActionResult Details(string id, string backURL)
        {
            try
            {
                var model = storagePresenter.Details(ValidationUtils.TryGetShort(id), backURL, UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }            
        }

        #endregion

        #region Список секций

        public ActionResult ShowStorageSectionGrid(GridState state)
        {
            try
            {
                GridData data = storagePresenter.GetStorageSectionGrid(state, UserSession.CurrentUserInfo);

                return PartialView("StorageSectionGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Добавление секции места хранения
        /// </summary>
        [HttpGet]
        public ActionResult AddSection(short storageId)
        {
            try
            {
                var model = storagePresenter.CreateSection(storageId, UserSession.CurrentUserInfo);

                return PartialView("StorageSectionEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult EditSection(string storageSectionId, short storageId)
        {
            try
            {
                var model = storagePresenter.EditSection(ValidationUtils.TryGetShort(storageSectionId), storageId, UserSession.CurrentUserInfo);

                return PartialView("StorageSectionEdit", model);                
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveSection(StorageSectionEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(storagePresenter.SaveSection(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }    

        [HttpPost]
        public ActionResult DeleteStorageSection(short storageSectionId, short storageId)
        {
            try
            {
                return Json(storagePresenter.DeleteSection(storageSectionId, storageId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Список связанных организаций

        public ActionResult ShowAccountOrganizationGrid(GridState state)
        {
            try
            {
                GridData data = storagePresenter.GetAccountOrganizationGrid(state, UserSession.CurrentUserInfo);

                return PartialView("StorageAccountOrganizationGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Добавление связанной организации
        /// </summary>
        [HttpPost]
        public ActionResult AddAccountOrganization(AccountOrganizationSelectList model)
        {
            try
            {                
                return Json(storagePresenter.AddAccountOrganization(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Удаление связанной организации
        /// </summary>
        [HttpPost]
        public ActionResult DeleteAccountOrganization(int accountOrganizationId, short storageId)
        {            
            try
            {
                return Json(storagePresenter.DeleteAccountOrganization(accountOrganizationId, storageId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult GetAvailableAccountOrganizations(short storageId)
        {
            try
            {
                var model = storagePresenter.GetAvailableAccountOrganizations(storageId, UserSession.CurrentUserInfo);               

                return PartialView("AccountOrganizationSelectList", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

    }
}
