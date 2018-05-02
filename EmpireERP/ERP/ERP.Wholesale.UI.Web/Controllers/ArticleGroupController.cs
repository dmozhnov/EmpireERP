using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ArticleGroup;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ArticleGroupController : WholesaleController
    {
        #region Поля

        private readonly IArticleGroupPresenter articleGroupPresenter;

        #endregion

        #region Конструктор


        public ArticleGroupController(IArticleGroupPresenter articleGroupPresenter)
        {
            this.articleGroupPresenter = articleGroupPresenter;
        }

        #endregion

        #region Список

        public ActionResult List()
        {
            try
            {
                var model = articleGroupPresenter.List(UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }


        #endregion

        #region Добавление / редактирование

        [HttpGet]
        public ActionResult Create(short? parentGroupId)
        {
            try
            {
                var model = articleGroupPresenter.Create(parentGroupId, UserSession.CurrentUserInfo);

                return PartialView("ArticleGroupDetailsForEdit", model);
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

                var model = articleGroupPresenter.Edit(id.Value, UserSession.CurrentUserInfo);

                return PartialView("ArticleGroupDetailsForEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Edit(ArticleGroupEditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("ArticleGroupDetailsForEdit", model);
                }

                var j = articleGroupPresenter.Save(model, UserSession.CurrentUserInfo);

                return Json(j, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление

        [HttpPost]
        public ActionResult Delete(short? id)
        {
            try
            {
                ValidationUtils.NotNull(id, "Группа товаров не найдена. Возможно, она была удалена.");

                articleGroupPresenter.Delete(id.Value, UserSession.CurrentUserInfo);

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
        public ActionResult Details(short? id)
        {
            try
            {
                ValidationUtils.NotNull(id, "Неверное значение входного параметра.");

                var model = articleGroupPresenter.Details(id.Value, UserSession.CurrentUserInfo);

                return PartialView("ArticleGroupDetails", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Выбор группы товара

        public ActionResult SelectArticleGroup()
        {
            try
            {
                return PartialView("ArticleGroupSelector", articleGroupPresenter.GetArticleGroupTree(""));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }

        }

        #endregion

        #region Получение информации о группе товара

        [HttpGet]
        public ActionResult GetArticleGroupInfo(string id)
        {
            try
            {
                return Json(articleGroupPresenter.GetArticleGroupInfo(ValidationUtils.TryGetShort(id), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

    }
}
