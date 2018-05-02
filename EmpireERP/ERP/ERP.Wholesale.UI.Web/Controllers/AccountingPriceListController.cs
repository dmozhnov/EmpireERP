using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.AccountingPriceList;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class AccountingPriceListController : WholesaleController
    {
        // TODO: по всему документу используется AccountingPrice вместо AccountingPriceList. Даже в параметрах грида - AccountingPriceId. Надо заменить

        #region Поля

        private readonly IAccountingPriceListPresenter accountingPriceListPresenter;

        #endregion

        #region Конструктор

        public AccountingPriceListController(IAccountingPriceListPresenter accountingPriceListPresenter)            
        {            
            this.accountingPriceListPresenter = accountingPriceListPresenter;
        }

        #endregion

        #region Список реестров цен

        /// <summary>
        /// Получить список реестров
        /// </summary>
        [HttpGet]
        public ActionResult List()
        {
            try
            {
                var model = accountingPriceListPresenter.List(UserSession.CurrentUserInfo);
                
                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowNewAccountingPriceListGrid(GridState state)
        {
            try
            {
                GridData data = accountingPriceListPresenter.GetNewAccountingPriceListGrid(state, UserSession.CurrentUserInfo);

                return PartialView("AccountingPriceListNewGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }        

        [HttpPost]
        public ActionResult ShowAcceptedAccountingPriceListGrid(GridState state)
        {
            try
            {
                GridData data = accountingPriceListPresenter.GetAcceptedAccountingPriceListGrid(state, UserSession.CurrentUserInfo);

                return PartialView("AccountingPriceListAcceptedGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Детали реестра цен

        [HttpGet]
        public ActionResult Details(string id, string message = "", string backUrl = "")
        {
            try
            {
                var priceListId = ValidationUtils.TryGetGuid(id);

                var model = accountingPriceListPresenter.Details(priceListId, UserSession.CurrentUserInfo, message, backUrl);

                if(!String.IsNullOrEmpty(model.Message)) TempData["Message"] = model.Message;

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }        

        [HttpPost]
        public ActionResult ShowAccountingPriceArticlesGrid(GridState state)
        {
            try
            {
                var data = accountingPriceListPresenter.GetAccountingPriceArticlesGrid(state, UserSession.CurrentUserInfo);

                return PartialView("AccountingPriceArticlesGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowAccountingPriceStoragesGrid(GridState state)
        {
            try
            {
                var data = accountingPriceListPresenter.GetAccountingPriceStoragesGrid(state, UserSession.CurrentUserInfo);

                return PartialView("AccountingPriceStoragesGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion       

        #region Изменения товаров в реестре цен

        public ActionResult AddArticleAccountingPriceSet(string id, string backURL)
        {
            
            try
            {
                return View(accountingPriceListPresenter.AddArticleAccountingPriceSet(ValidationUtils.TryGetGuid(id), backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        [HttpPost]
        public ActionResult SaveArticleAccountingPriceSet(ArticleAccountingPriceSetAddViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Значение входного параметра не задано.");

                var result = accountingPriceListPresenter.SaveArticleAccountingPriceSet(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }   

        [HttpGet]
        public ActionResult AddArticle(string accountingPriceListId)
        {
            try
            {
                var priceListId = ValidationUtils.TryGetGuid(accountingPriceListId);

                var model = accountingPriceListPresenter.AddArticle(priceListId, UserSession.CurrentUserInfo);

                return PartialView("ArticleAccountingPriceEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult EditArticle(Guid? accountingPriceListId, Guid? articleAccountingPriceId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(accountingPriceListId, "Неверное значение входного параметра.");
                ValidationUtils.NotNullOrDefault(articleAccountingPriceId, "Неверное значение входного параметра.");

                var model = accountingPriceListPresenter.EditArticle(accountingPriceListId.Value, articleAccountingPriceId.Value, UserSession.CurrentUserInfo);                

                return PartialView("ArticleAccountingPriceEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult EditArticle(ArticleAccountingPriceEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Значение входного параметра не задано.");

                var result = accountingPriceListPresenter.SaveArticle(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }        

        /// <summary>
        /// Удаление товара из реестра цен
        /// </summary>
        /// <param name="accountingPriceListId">Идентификатор реестра цен</param>
        /// <param name="articleAccountingPriceId">Идентификатор товара с ценой (элемента реестра цен)</param>
        [HttpPost]
        public ActionResult DeleteArticle(Guid? accountingPriceListId, Guid? articleAccountingPriceId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(accountingPriceListId, "Неверное значение входного параметра.");
                ValidationUtils.NotNullOrDefault(articleAccountingPriceId, "Неверное значение входного параметра.");

                var result = accountingPriceListPresenter.DeleteArticle(accountingPriceListId.Value, articleAccountingPriceId.Value, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);                
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }        

        #endregion

        #region Редактирование и создание нового реестра цен

        /// <summary>
        /// Создание нового реестра
        /// </summary>
        /// <param name="additionalId">Идентификатор (склада, если создается по умолчанию, накладной - если создается по приходу, реестра - если переоценка)</param>
        /// <param name="reasonCode">Код основания</param>
        /// <param name="backURL">Обратный адрес</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create(string additionalId, AccountingPriceListReason reasonCode, string backURL = "")
        {
            try
            {
                var model = accountingPriceListPresenter.Create(additionalId, reasonCode, UserSession.CurrentUserInfo, backURL);                

                return View("Edit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование и удаление реестра цен
        /// </summary>
        /// <param name="backURL">Обратный адрес</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(Guid? accountingPriceListId, string backURL = "")
        {
            try
            {
                ValidationUtils.NotNullOrDefault(accountingPriceListId, "Неверное значение входного параметра.");

                var model = accountingPriceListPresenter.Edit(accountingPriceListId.Value, UserSession.CurrentUserInfo, backURL);              

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }       
        
        [HttpPost]
        public ActionResult Edit(AccountingPriceListEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Значение входного параметра не задано.");
                
                if (!ModelState.IsValid)
                {
                    // TODO: сделать валидацию на форме. А то приходит невалидная модель. Да и возврат модели не приводит к появлению страницы.
                    //FillConstantsForAccountingPriceListEdit(model);

                    //return View(model);
                }

                var result = accountingPriceListPresenter.Save(model, UserSession.CurrentUserInfo);

                if (!String.IsNullOrEmpty(result.Value)) TempData["Message"] = result.Value;

                return Json(result.Key, JsonRequestBehavior.AllowGet);                
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
                var accountingPriceListId = ValidationUtils.TryGetGuid(id);

                accountingPriceListPresenter.Delete(accountingPriceListId, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Правила для расчета учетной цены и последней цифры       

        #endregion

        #region Проводка и отмена проводки

        [HttpPost]
        public ActionResult Accept(string id)
        {
            try
            {                
                var accountingPriceListId = ValidationUtils.TryGetGuid(id);

                var result = accountingPriceListPresenter.Accept(accountingPriceListId, UserSession.CurrentUserInfo);

                if(!String.IsNullOrEmpty(result)) TempData["Message"] = result;

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelAcceptance(string id)
        {
            try
            {
                var accountingPriceListId = ValidationUtils.TryGetGuid(id);

                accountingPriceListPresenter.CancelAcceptance(accountingPriceListId, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion     

        #region Расчет подсказок для редактирования строки реестра

        /// <summary>
        /// Расчет подсказок, выводимых на форму редактирования строки реестра
        /// Для расчета учетной цены методу необходимо знать, какие заданы правила расчета учетной цены и формирования последней цифры, поэтому передается идентификатор реестра
        /// Метод вызывается из представления, поэтому передаются идентификаторы
        /// </summary>
        /// <param name="accountingPriceListId"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetTipsForArticle(Guid accountingPriceListId, int articleId)
        {
            var model = accountingPriceListPresenter.GetTipsForArticle(accountingPriceListId, articleId, UserSession.CurrentUserInfo);
            
            return Json(model, JsonRequestBehavior.AllowGet);
        }        

        #endregion

        #region Вспомогательные функции

        [HttpGet]
        public bool IsNumberUnique(string number)
        {
            bool isUnique = accountingPriceListPresenter.IsNumberUnique(number);

            return isUnique;
        }      


        #endregion

        #region Добавление места хранения

        [HttpGet]
        public ActionResult StoragesList(Guid priceListId)
        {
            try
            {
                var model = accountingPriceListPresenter.StoragesList(priceListId, UserSession.CurrentUserInfo);                

                return PartialView("AccountingPriceListAddStorage", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult StoragesList(AccountingPriceListAddStorageViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Значение входного параметра не задано.");
                
                if (!ModelState.IsValid)
                {
                    //throw new Exception("Модель не валидна.");
                    //return View(model);
                }

                var result = accountingPriceListPresenter.StoragesList(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public JsonResult GetListOfStorages(Guid priceListId)
        {
            var result = accountingPriceListPresenter.GetListOfStorages(priceListId, UserSession.CurrentUserInfo);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult StoragesAddAll(Guid priceListId)
        {
            try
            {
                var result = accountingPriceListPresenter.StoragesAddAll(priceListId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult StoragesAddTradePoint(Guid priceListId)
        {
            try
            {
                var result = accountingPriceListPresenter.StoragesAddTradePoint(priceListId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление места хранения

        public ActionResult DeleteStorage(Guid? accPriceListId, short? storageId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(accPriceListId);
                ValidationUtils.NotNullOrDefault(storageId);

                var result = accountingPriceListPresenter.DeleteStorage(accPriceListId.Value, storageId.Value, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Печатные формы

        /// <summary>
        /// Получение печатной формы расходной накладной
        /// </summary>
        /// <param name="settings">Параметры печатных форм</param>
        public ActionResult ShowAccountingPriceListPrintingForm(string accountingPriceListId, string expMode)
        {
            try
            {
                var detailedMode = false;
                if (!bool.TryParse(expMode, out detailedMode))
                {
                    detailedMode = false;
                }
                var accountingPriceListGuid = ValidationUtils.TryGetGuid(accountingPriceListId);
                var model = accountingPriceListPresenter.GetAccountingPriceListPrintingForm(accountingPriceListGuid, detailedMode, UserSession.CurrentUserInfo);

                return View("~/Views/PrintingForm/AccountingPriceList/AccountingPriceListPrintingForm.aspx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
    }
}
