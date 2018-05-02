using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ArticleCertificate;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ArticleCertificateController : WholesaleController
    {
        #region Поля
        
        private readonly IArticleCertificatePresenter articleCertificatePresenter;
        
        #endregion

        #region Конструктор
        
        public ArticleCertificateController(IArticleCertificatePresenter articleCertificatePresenter)
        {
            this.articleCertificatePresenter = articleCertificatePresenter;
        }
        
        #endregion

        #region Методы

        public ActionResult SelectArticleCertificate()
        {
            try
            {
                var model = articleCertificatePresenter.SelectArticleCertificate(UserSession.CurrentUserInfo);

                return View("ArticleCertificateSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowArticleCertificateSelectGrid(GridState state)
        {
            try
            {
                var model = articleCertificatePresenter.GetArticleCertificateSelectGrid(state);

                return PartialView("ArticleCertificateSelectGrid", model);
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
                return View(articleCertificatePresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowArticleCertificateGrid(GridState state)
        {
            try
            {
                return PartialView("ArticleCertificateGrid", articleCertificatePresenter.GetArticleCertificateGrid(state, UserSession.CurrentUserInfo));
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
                return PartialView("ArticleCertificateEdit", articleCertificatePresenter.Create(UserSession.CurrentUserInfo));
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

                return PartialView("ArticleCertificateEdit", articleCertificatePresenter.Edit(id.Value, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(ArticleCertificateEditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("ArticleCertificateEdit", model);
                }

                var obj = articleCertificatePresenter.Save(model, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            try
            {
                ValidationUtils.NotNull(id, "Неверное значение входного параметра.");

                articleCertificatePresenter.Delete(id.Value, UserSession.CurrentUserInfo);

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
