using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.PrintingForm.WriteoffWaybill;
using ERP.Wholesale.UI.ViewModels.WriteoffWaybill;
using ERP.Wholesale.UI.Web.Infrastructure;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class WriteoffWaybillController : BaseWaybillController<WriteoffWaybill>
    {
        #region Поля
       
        private readonly IWriteoffWaybillPresenter writeoffWaybillPresenter;

        #endregion

        #region Конструкторы

        public WriteoffWaybillController(IWriteoffWaybillPresenter writeoffWaybillPresenter)
            : base(writeoffWaybillPresenter)
        {
            this.writeoffWaybillPresenter = writeoffWaybillPresenter;
        }

        #endregion

        #region Список

        public ActionResult List()
        {
            try
            {
                var model = writeoffWaybillPresenter.List(UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #region Новые (Ожидается списание)

        [HttpPost]
        public ActionResult ShowWriteoffPendingGrid(GridState state)
        {
            try
            {
                GridData data = writeoffWaybillPresenter.GetWriteoffPendingGrid(state, UserSession.CurrentUserInfo);

                return PartialView("WriteoffPendingGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Выполненные списания

        [HttpPost]
        public ActionResult ShowWrittenOffGrid(GridState state)
        {
            try
            {
                GridData data = writeoffWaybillPresenter.GetWrittenOffGrid(state, UserSession.CurrentUserInfo);

                return PartialView("WrittenoffWaybillGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #endregion

        #region Добавление / редактирование

        [HttpGet]
        public ActionResult Create(string backURL)
        {
            try
            {
                var model = writeoffWaybillPresenter.Create(backURL, UserSession.CurrentUserInfo);

                return View("Edit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult Edit(string id, string backURL)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(id);

                var model = writeoffWaybillPresenter.Edit(waybillId, backURL, UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Edit(WriteoffWaybillEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");


                var result = writeoffWaybillPresenter.Save(model, UserSession.CurrentUserInfo);

                return Content(result);

            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult GetAccountOrganizationsForStorage(short storageId)
        {
            try
            {
                var result = writeoffWaybillPresenter.GetAccountOrganizationsForStorage(storageId);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                writeoffWaybillPresenter.Delete(id, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }


        #endregion

        #region Детали накладной

        /// <summary>
        /// Детали накладной списания
        /// </summary>
        [HttpGet]
        public ActionResult Details(Guid? id, string backURL)
        {
            try
            {
                ValidationUtils.NotNull(id, "Неверное значение входного параметра.");

                var model = writeoffWaybillPresenter.Details(id.Value, backURL, UserSession.CurrentUserInfo);


                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }      
                
        #region Список позиций и групп товаров

        [HttpPost]
        public ActionResult ShowWriteoffWaybillRowGrid(GridState state)
        {
            try
            {
                GridData data = writeoffWaybillPresenter.GetWriteoffWaybillRowGrid(state, UserSession.CurrentUserInfo);

                return PartialView("WriteoffWaybillRowGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowWriteoffWaybillArticleGroupGrid(GridState state)
        {
            try
            {
                GridData data = writeoffWaybillPresenter.GetWriteoffWaybillArticleGroupGrid(state, UserSession.CurrentUserInfo);

                return PartialView("WriteoffWaybillArticleGroupGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Добавление / редактирование позиций

        [HttpGet]
        public ActionResult AddRow(string writeoffWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(writeoffWaybillId);

                var model = writeoffWaybillPresenter.AddRow(waybillId, UserSession.CurrentUserInfo);

                return PartialView("WriteoffWaybillRowEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult EditRow(string writeoffWaybillId, string writeoffWaybillRowId)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(writeoffWaybillId);
                var rowId = ValidationUtils.TryGetGuid(writeoffWaybillRowId);

                var model = writeoffWaybillPresenter.EditRow(waybillId, rowId, UserSession.CurrentUserInfo);

                return PartialView("WriteoffWaybillRowEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult EditRow(WriteoffWaybillRowEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");
                ValidationUtils.NotNullOrDefault(model.WriteoffWaybillId, "Неверное значение входного параметра.");
                ValidationUtils.NotNullOrDefault(model.ReceiptWaybillRowId, "Неверное значение входного параметра.");

                var result = writeoffWaybillPresenter.SaveRow(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #region Добавление позиций списком

        public ActionResult AddRowsByList(string id, string backURL)
        {
            try
            {
                return View(writeoffWaybillPresenter.AddRowsByList(ValidationUtils.TryGetGuid(id), backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowArticlesForWaybillRowsAdditionByListGrid(GridState state)
        {
            try
            {
                return PartialView("~/Views/OutgoingWaybill/ArticlesForWaybillRowsAdditionByListGrid.ascx", writeoffWaybillPresenter.GetArticlesForWaybillRowsAdditionByListGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult AddRowSimply(string waybillId, string articleId, string count)
        {
            try
            {
                writeoffWaybillPresenter.AddRowSimply(ValidationUtils.TryGetGuid(waybillId), ValidationUtils.TryGetInt(articleId),
                    ValidationUtils.TryGetDecimal(count), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        [HttpGet]
        public object GetRowInfo(string waybillId, string batchId)
        {
            try
            {
                var _waybillId = ValidationUtils.TryGetNotEmptyGuid(waybillId);
                var _batchId = ValidationUtils.TryGetNotEmptyGuid(batchId);

                var result = writeoffWaybillPresenter.GetRowInfo(_waybillId, _batchId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление позиции

        [HttpPost]
        public ActionResult DeleteRow(Guid writeoffWaybillId, Guid writeoffWaybillRowId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(writeoffWaybillId, "Неверное значение входного параметра.");
                ValidationUtils.NotNullOrDefault(writeoffWaybillRowId, "Неверное значение входного параметра.");

                var result = writeoffWaybillPresenter.DeleteRow(writeoffWaybillId, writeoffWaybillRowId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Документы по накладной

        [HttpPost]
        public ActionResult ShowDocGrid(GridState state)
        {
            try
            {
                GridData data = writeoffWaybillPresenter.GetDocumentGrid(state);

                return PartialView("WriteoffWaybillDocumentGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Подготовка / отмена готовности к проводке

        [HttpPost]
        public ActionResult PrepareToAccept(string writeoffWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(writeoffWaybillId);

                return Json(writeoffWaybillPresenter.PrepareToAccept(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelReadinessToAccept(string writeoffWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(writeoffWaybillId);

                return Json(writeoffWaybillPresenter.CancelReadinessToAccept(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Проводка / отмена проводки

        [HttpPost]
        public ActionResult Accept(string writeoffWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(writeoffWaybillId);

                return Json(writeoffWaybillPresenter.Accept(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelAcceptance(string writeoffWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(writeoffWaybillId);

                return Json(writeoffWaybillPresenter.CancelAcceptance(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Списание

        [HttpPost]
        public ActionResult Writeoff(Guid? writeoffWaybillId = null)
        {
            try
            {
                ValidationUtils.NotNullOrDefault( writeoffWaybillId, "Неверное значение входного параметра.");

                var result = writeoffWaybillPresenter.Writeoff(writeoffWaybillId.Value, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);                
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Отмена списания

        [HttpPost]
        public ActionResult CancelWriteoff(Guid? writeoffWaybillId = null)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(writeoffWaybillId, "Неверное значение входного параметра.");

                var result = writeoffWaybillPresenter.CancelWriteoff(writeoffWaybillId.Value, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }

        }
        #endregion

        #region Печатные формы

        #region Накладная списания

        /// <summary>
        /// Получение окна параметров печатной формы
        /// </summary>
        public ActionResult ShowWriteoffWaybillPrintingFormSettings(string waybillId)
        {
            try
            {
                var model = writeoffWaybillPresenter.GetWriteoffWaybillPrintingFormSettings(ValidationUtils.TryGetGuid(waybillId), UserSession.CurrentUserInfo);

                return View("~/Views/PrintingForm/WriteoffWaybill/WriteoffWaybillPrintingFormSettings.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение печатной формы накладной
        /// </summary>
        /// <param name="settings">Параметры печатных форм</param>
        public ActionResult ShowWriteoffWaybillPrintingForm(WriteoffWaybillPrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = writeoffWaybillPresenter.GetWriteoffWaybillPrintingForm(settings, UserSession.CurrentUserInfo);

                return View("~/Views/PrintingForm/WriteoffWaybill/WriteoffWaybillPrintingForm.aspx", model);
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
