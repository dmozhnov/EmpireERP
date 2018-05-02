using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Article;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ArticleController : WholesaleController
    {
        #region Поля

        private IArticlePresenter articlePresenter;

        #endregion

        #region Конструктор

        public ArticleController(IArticlePresenter articlePresenter)
        {
            this.articlePresenter = articlePresenter;
        }

        #endregion

        #region Методы
 
        #region Список

        public ActionResult List()
        {
            try
            {
                var model = articlePresenter.List(UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowActualArticleGrid(GridState state)
        {
            try
            {
                GridData data = articlePresenter.GetActualArticleGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ActualArticleGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowObsoleteArticleGrid(GridState state)
        {
            try
            {
                GridData data = articlePresenter.GetObsoleteArticleGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ObsoleteArticleGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Создание, редактирование, удаление, копирование

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var model = articlePresenter.Create(UserSession.CurrentUserInfo);

                return PartialView("ArticleEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            try
            {
                ValidationUtils.NotNull(id, "Неверное значение входного параметра.");

                var model = articlePresenter.Edit(id.Value, UserSession.CurrentUserInfo);
               
                return PartialView("ArticleEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(ArticleEditViewModel model)
        {
            try
            {
                articlePresenter.Save(model, UserSession.CurrentUserInfo);

                return Content("");
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
                articlePresenter.Delete(ValidationUtils.TryGetInt(id), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult Copy(int? articleId)
        {
            try
            {
                ValidationUtils.NotNull(articleId, "Неверное значение входного параметра.");

                var model = articlePresenter.Copy(articleId.Value, UserSession.CurrentUserInfo);

                return PartialView("ArticleEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion              

        #endregion


        #region Выбор товара

        #region Выбор товара из полного перечня

        [HttpGet]
        public ActionResult SelectArticle()
        {
            try
            {
                var model = articlePresenter.SelectArticle(UserSession.CurrentUserInfo);

                return PartialView("ArticleSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Форма выбора товара из расширенного наличия по складу
        /// </summary>
        /// <param name="storageId">Код склада</param>
        /// <param name="senderId">Код организации</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SelectArticleFromStorage(short storageId, int senderId)
        {
            try
            {
                var model = articlePresenter.SelectArticleFromStorage(storageId, senderId, UserSession.CurrentUserInfo);

                return PartialView("ArticleSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }            
        }

        /// <summary>
        /// Форма выбора товара из реализаций по сделке, команде и организации
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="recipientId">Код организации</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SelectArticleToReturn(int dealId, short teamId, int recipientId)
        {
            try
            {
                var model = articlePresenter.SelectArticleToReturn(dealId, teamId, recipientId, UserSession.CurrentUserInfo);

                return PartialView("ArticleSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        [HttpPost]
        public ActionResult ShowArticleSelectGrid(GridState state)
        {
            try
            {
                GridData data = articlePresenter.GetArticleSelectGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ArticleSelectGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        /// <summary>
        /// Получить словарь товаров по идентификатору группы товара
        /// </summary>
        [HttpGet]
        public ActionResult GetArticleFromArticleGroup(string id)
        {
            try
            {
                return Json(articlePresenter.GetArticleFromArticleGroup(ValidationUtils.TryGetShort(id), UserSession.CurrentUserInfo), 
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Выбор партии товара

        #region Для перемещения

        [HttpGet]
        public ActionResult SelectArticleBatch(int articleId, short senderStorageId, short recipientStorageId, int senderId, string date = "", 
            Guid? articleBatchToExcludeId = null)
        {
            try
            {
                var model = articlePresenter.SelectArticleBatch(articleId, senderStorageId, recipientStorageId, senderId, UserSession.CurrentUserInfo, 
                    date, articleBatchToExcludeId);
              
                return PartialView("ArticleBatchSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowArticleBatchSelectGrid(GridState state)
        {
            try
            {
                GridData data = articlePresenter.GetArticleBatchSelectGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ArticleBatchGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Для списания

        /// <summary>
        /// Получение списка партий товара по месту хранения (отличие от SelectArticleBatch в том, что принимает 1 склад, а не 2)
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="senderStorageId"></param>
        /// <param name="date"></param>
        /// <param name="articleBatchToExcludeId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SelectArticleBatchByStorage(int articleId, short storageId, int senderId, string date = "", Guid? articleBatchToExcludeId = null)
        {
            try
            {
                var model = articlePresenter.SelectArticleBatchByStorage(articleId, storageId, senderId, UserSession.CurrentUserInfo, date, articleBatchToExcludeId);

                return PartialView("ArticleBatchByStorageSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Выбор расходной партии для возврата

        /// <summary>
        /// Получение списка партий товара для возврата
        /// </summary>        
        [HttpGet]
        public ActionResult SelectArticleSale(int articleId, int dealId, short teamId, int recipientId, short storageId, string date = "",
            Guid? articleSaleToExcludeId = null)
        { 
            try
            {
                var model = articlePresenter.SelectArticleSale(articleId, dealId, teamId, recipientId, storageId, UserSession.CurrentUserInfo, 
                    articleSaleToExcludeId);

                return PartialView("ArticleSaleSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowArticleSaleSelectGrid(GridState state)
        {
            try
            {
                GridData data = articlePresenter.GetArticleSaleSelectGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ArticleSaleGrid", data);
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
