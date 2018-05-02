using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.Web.Infrastructure;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.ViewModels.ProductionOrderMaterialsPackage;
using System.IO;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ProductionOrderMaterialsPackageController : WholesaleController
    {
        #region Поля

        private readonly IProductionOrderMaterialsPackagePresenter productionOrderMaterialsPackagePresenter;

        #endregion

        #region Конструктор

        public ProductionOrderMaterialsPackageController(IProductionOrderMaterialsPackagePresenter productionOrderMaterialsPackagePresenter)
        {
            this.productionOrderMaterialsPackagePresenter = productionOrderMaterialsPackagePresenter;
        }

        #endregion

        #region Методы

        #region Список

        public ActionResult List()
        {
            try
            {
                return View(productionOrderMaterialsPackagePresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowMaterialsPackageGrid(GridState state)
        {
            try
            {
                ValidationUtils.NotNull(state,"Неверное значение входного параметра.");

                return PartialView("ProductionOrderMaterialsPackageGrid", productionOrderMaterialsPackagePresenter.GetProductionOrderMaterialsPackageGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Создание/редактирование

        public ActionResult Create(string productionOrderId, string backURL)
        {
            try
            {
                Guid? id = null;
                if (productionOrderId != null && productionOrderId.Length > 0)
                {
                    id = ValidationUtils.TryGetGuid(productionOrderId);
                }

                return View("Edit", productionOrderMaterialsPackagePresenter.Create(id, backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(ProductionOrderMaterialsPackageEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model);

                return Json(productionOrderMaterialsPackagePresenter.Save(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
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
                var materialsPackageId = ValidationUtils.TryGetGuid(id);

                return View("Edit", productionOrderMaterialsPackagePresenter.Edit(materialsPackageId, backURL, UserSession.CurrentUserInfo));
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
                var materialsPackageId = ValidationUtils.TryGetGuid(id);

                return View(productionOrderMaterialsPackagePresenter.Details(materialsPackageId, backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProductionOrderMaterialsPackageDocumentGrid(GridState state)
        {
            try
            {
                ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

                return PartialView("ProductionOrderMaterialsPackageDocumentGrid", 
                    productionOrderMaterialsPackagePresenter.GetMaterialsPackageDocumentGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult DeleteMaterialsPackage(string id)
        {
            try
            {
                var materialsPackageId = ValidationUtils.TryGetGuid(id);

                productionOrderMaterialsPackagePresenter.DeleteMaterialsPackage(materialsPackageId, UserSession.CurrentUserInfo);
                
                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Редактирование документа пакета материалов

        public ActionResult ProductionOrderMaterialsPackageDocumentCreate(string id)
        {
            try
            {
                var packageId = ValidationUtils.TryGetGuid(id);

                return PartialView("ProductionOrderMaterialsPackageDocumentCreate", productionOrderMaterialsPackagePresenter.ProductionOrderMaterialsPackageDocumentCreate(packageId, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public JsonResult ProductionOrderMaterialsPackageDocumentSave(ProductionOrderMaterialsPackageDocumentEditViewModel model)
        {
            try
            {
                HttpPostedFileBase file = null;

                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");
                
                if (Request.Files.Count == 1)
                {
                    file = Request.Files[0];
                    model.FileName = file.FileName;
                    model.FileUpload = file;
                }
                var obj = productionOrderMaterialsPackagePresenter.ProductionOrderMaterialsPackageDocumentSave(model, UserSession.CurrentUserInfo);

                return new JsonResult
                {
                    ContentType = "text/html",
                    Data = new { obj }
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.None)]
        public FileResult DownloadProductionOrderMaterialsPackageDocument(string id)
        {
            try
            {
                var model = productionOrderMaterialsPackagePresenter.DownloadProductionOrderMaterialsPackageDocument(ValidationUtils.TryGetGuid(id), UserSession.CurrentUserInfo);

                // это прежняя версия кода, которая не работает для Firefox v7
                //var e = HttpUtility.UrlPathEncode(model.FileName);
                //Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", (Request.Browser.Browser == "IE") ? e : model.FileName));

                /* Метод File автоматически добавляет Header c ключом "Content-Disposition". И если мы добавим еще один такой header, то Firefox 
                 * заблокирует страницу и скажет о подмене контента. Если не добавлять явно этот header, то IE не увидит название файла. Chrom и Opera 
                 * в обоих случаях работают нормально. Поэтому принято решение добавлять header только для IE. */

                if (Request.Browser.Browser == "IE")
                {
                    Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", HttpUtility.UrlPathEncode(model.FileName)));
                }

                return File(model.FilePath, System.Net.Mime.MediaTypeNames.Application.Octet, model.FileName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult ProductionOrderMaterialsPackageDocumentEdit(string id)
        {
            try
            {
                var docId = ValidationUtils.TryGetGuid(id);

                return PartialView("ProductionOrderMaterialsPackageDocumentEdit", productionOrderMaterialsPackagePresenter.ProductionOrderMaterialsPackageDocumentEdit(docId, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ProductionOrderMaterialsPackageDocumentDelete(string id)
        {
            try
            {
                var docId = ValidationUtils.TryGetGuid(id);

                var obj = productionOrderMaterialsPackagePresenter.ProductionOrderMaterialsPackageDocumentDelete(docId, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
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
