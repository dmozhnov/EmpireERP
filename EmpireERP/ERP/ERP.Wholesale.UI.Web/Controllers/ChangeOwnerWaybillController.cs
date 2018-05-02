using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.ChangeOwnerWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;
using ERP.Wholesale.UI.Web.Infrastructure;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.Т1;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ChangeOwnerWaybillController : BaseWaybillController<ChangeOwnerWaybill>
    {
        #region Поля

        private readonly IChangeOwnerWaybillPresenter changeOwnerWaybillPresenter;

        #endregion

        #region Конструкторы

        public ChangeOwnerWaybillController(IChangeOwnerWaybillPresenter changeOwnerWaybillPresenter)
            :base(changeOwnerWaybillPresenter)
        {
            this.changeOwnerWaybillPresenter = changeOwnerWaybillPresenter;
        }

        #endregion

        #region Методы

        #region Список накладных

        /// <summary>
        /// Отображение списка накладных
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            try
            {
                return View(changeOwnerWaybillPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        /// <summary>
        /// Отображение грида новых накладных
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShowChangeOwnerWaybillNewGrid(GridState state)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.GetChangeOwnerWaybillNewGrid(state, UserSession.CurrentUserInfo);
                
                return PartialView("ChangeOwnerWaybillNewGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Отображение грида проведенных накладных
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShowChangeOwnerWaybillAcceptedGrid(GridState state)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.GetChangeOwnerWaybillAcceptedGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ChangeOwnerWaybillAcceptedGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Создание/редактирование накладной
        
        /// <summary>
        /// Создание новой накладной
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create(string backURL)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.Create(backURL, UserSession.CurrentUserInfo);

                return View("Edit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Создание новой накладной
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(Guid id, string backURL)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.Edit(id, backURL, UserSession.CurrentUserInfo);

                return View("Edit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Сохранение накладной
        /// </summary>
        /// <param name="model">Накладная</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(ChangeOwnerWaybillEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Content(changeOwnerWaybillPresenter.Save(model, UserSession.CurrentUserInfo).ToString());
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
                return View(changeOwnerWaybillPresenter.AddRowsByList(ValidationUtils.TryGetGuid(id), backURL, UserSession.CurrentUserInfo));
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
                return PartialView("~/Views/OutgoingWaybill/ArticlesForWaybillRowsAdditionByListGrid.ascx", changeOwnerWaybillPresenter.GetArticlesForWaybillRowsAdditionByListGrid(state, UserSession.CurrentUserInfo));
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
                changeOwnerWaybillPresenter.AddRowSimply(ValidationUtils.TryGetGuid(waybillId), ValidationUtils.TryGetInt(articleId),
                    ValidationUtils.TryGetDecimal(count), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        /// <summary>
        /// Получение списка связанных организаций
        /// </summary>
        /// <param name="stirageId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetOrganizationList(short storageId)
        {
            try
            {
                var list = changeOwnerWaybillPresenter.GetOrganizationList(storageId);

                return Json(new { List = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Удаление накладной
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Delete(string id)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(id);
                changeOwnerWaybillPresenter.DeleteChangeOwnerWaybill(waybillId, UserSession.CurrentUserInfo);

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
                var model = changeOwnerWaybillPresenter.Details(ValidationUtils.TryGetGuid(id), backURL, UserSession.CurrentUserInfo);
                
                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

       
        [HttpPost]
        public ActionResult ShowChangeOwnerWaybillRowGrid(GridState state)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.GetChangeOwnerWaybillRowGrid(state, UserSession.CurrentUserInfo);

                return PartialView("~/Views/ChangeOwnerWaybill/ChangeOwnerWaybillRowGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowChangeOwnerWaybillArticleGroupGrid(GridState state)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.GetChangeOwnerWaybillArticleGroupGrid(state, UserSession.CurrentUserInfo);

                return PartialView("~/Views/ChangeOwnerWaybill/ChangeOwnerWaybillArticleGroupGrid.cshtml", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowChangeOwnerWaybillDocsGrid(GridState state)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.GetChangeOwnerWaybillDocsGrid(state, UserSession.CurrentUserInfo);

                return PartialView("~/Views/ChangeOwnerWaybill/ChangeOwnerWaybillDocsGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Добавление/редактирование строк накладной

        /// <summary>
        /// Вызов формы добавления строки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddRow(string id)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.AddRow(ValidationUtils.TryGetGuid(id), UserSession.CurrentUserInfo);

                return PartialView("ChangeOwnerWaybillRowForEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveRow(ChangeOwnerWaybillRowEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(changeOwnerWaybillPresenter.SaveRow(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult EditRow(string id, string rowId)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.EditRow(
                    ValidationUtils.TryGetGuid(id), ValidationUtils.TryGetGuid(rowId), UserSession.CurrentUserInfo);

                return PartialView("ChangeOwnerWaybillRowForEdit", model);
                
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public object GetRowInfo(string waybillId, string batchId)
        {
            try
            {
                var _waybillId = ValidationUtils.TryGetNotEmptyGuid(waybillId);
                var _batchId = ValidationUtils.TryGetNotEmptyGuid(batchId);

                var result = changeOwnerWaybillPresenter.GetRowInfo(_waybillId, _batchId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult DeleteRow(string id, string rowId)
        {
            try
            {
                return Json(changeOwnerWaybillPresenter.DeleteRow(
                    ValidationUtils.TryGetGuid(id), ValidationUtils.TryGetGuid(rowId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Смена получателя

        /// <summary>
        /// Вызов окна смены получателя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeRecipient(string id)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.ChangeRecipient(ValidationUtils.TryGetGuid(id), UserSession.CurrentUserInfo);

                return PartialView("ChangeOwnerWaybillChangeRecipient", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ChangeRecipient(ChangeOwnerWaybillChangeRecipientViewModel model)
        {
            try
            {
                return Json(changeOwnerWaybillPresenter.SaveNewRecipient(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Подготовка / отмена готовности к проводке

        /// <summary>
        /// Подготовка к проводке
        /// </summary>
        /// <param name="id">Код накладной</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PrepareToAccept(string id)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(id);
                var model = changeOwnerWaybillPresenter.PrepareToAccept(waybillId, UserSession.CurrentUserInfo);

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Отменить готовность к проводке
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CancelReadinessToAccept(string id)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(id);
                var model = changeOwnerWaybillPresenter.CancelReadinessToAccept(waybillId, UserSession.CurrentUserInfo);

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Проводка/отмена проводки накладной

        /// <summary>
        /// Проводка накладной
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Accept(string id)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(id);
                var model = changeOwnerWaybillPresenter.Accept(waybillId, UserSession.CurrentUserInfo);

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Отмена проводки накладной
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CancelAcceptance(string id)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(id);
                var model = changeOwnerWaybillPresenter.CancelAcceptance(waybillId, UserSession.CurrentUserInfo);

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Печатные формы

        /// <summary>
        /// Получение печатной формы товарного счета
        /// </summary>
        /// <param name="waybillId">Идентификатор накладной</param>
        [HttpGet]
        public ActionResult ShowCashMemoPrintingForm(string waybillId)
        {
            try
            {
                var _waybillId = ValidationUtils.TryGetGuid(waybillId);

                var model = changeOwnerWaybillPresenter.GetCashMemoPrintingForm(_waybillId, UserSession.CurrentUserInfo);

                return View("~/Views/PrintingForm/Common/CashMemo/CashMemoPrintingForm.aspx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #region Счет-фактура

        /// <summary>
        /// Получение окна параметров печатной формы
        /// </summary>
        public ActionResult ShowInvoicePrintingFormSettings(string waybillId)
        {
            try
            {
                var _waybillId = ValidationUtils.TryGetGuid(waybillId);

                var model = changeOwnerWaybillPresenter.GetInvoicePrintingFormSettings(_waybillId, UserSession.CurrentUserInfo);
                model.ActionUrl = "/ChangeOwnerWaybill/ShowInvoicePrintingForm/";
                model.ExportToExcelUrl = "/ChangeOwnerWaybill/ExportInvoicePrintingFormToExcel";

                return View("~/Views/PrintingForm/Common/Invoice/InvoicePrintingFormSettings.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }

        }

        /// <summary>
        /// Экспорт печатной формы счет-фактуры в Excel 
        /// </summary>
        public ActionResult ExportInvoicePrintingFormToExcel(InvoicePrintingFormSettingsViewModel settings)
        {
            try
            {
                return ExcelFile(changeOwnerWaybillPresenter.ExportInvoicePrintingFormToExcel(settings, UserSession.CurrentUserInfo), "Invoice.xlsx");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение печатной формы счета-фактуры
        /// </summary>
        [HttpGet]
        public ActionResult ShowInvoicePrintingForm(InvoicePrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.GetInvoicePrintingForm(settings, UserSession.CurrentUserInfo);
                model.RowsContentURL = "/ChangeOwnerWaybill/ShowInvoicePrintingFormRows/";

                return View("~/Views/PrintingForm/Common/Invoice/InvoicePrintingForm.aspx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение строк печатной формы счета-фактуры
        /// </summary>
        public ActionResult ShowInvoicePrintingFormRows(InvoicePrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.GetInvoicePrintingFormRows(settings, UserSession.CurrentUserInfo);

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        } 
        #endregion

        #region ТОРГ 12

        /// <summary>
        /// Получение окна параметров печатной формы
        /// </summary>
        public ActionResult ShowTORG12PrintingFormSettings(string waybillId)
        {
            try
            {
                var _waybillId = ValidationUtils.TryGetGuid(waybillId);

                var model = changeOwnerWaybillPresenter.GetTORG12PrintingFormSettings(_waybillId, UserSession.CurrentUserInfo);
                model.ActionUrl = "/ChangeOwnerWaybill/ShowTORG12PrintingForm";
                model.ExportToExcelUrl = "/ChangeOwnerWaybill/ExportTORG12PrintingFormToExcel";

                return PartialView("~/Views/PrintingForm/Common/TORG12/TORG12PrintingFormSettings.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Экспорт печатной формы ТОРГ12 в Excel 
        /// </summary>
        public ActionResult ExportTORG12PrintingFormToExcel(TORG12PrintingFormSettingsViewModel settings)
        {
            try
            {
                return ExcelFile(changeOwnerWaybillPresenter.ExportTORG12PrintingFormToExcel(settings, UserSession.CurrentUserInfo),"TORG12.xlsx");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение печатной формы ТОРГ12 
        /// </summary>
        public ActionResult ShowTORG12PrintingForm(TORG12PrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.GetTORG12PrintingForm(settings, UserSession.CurrentUserInfo);
                model.RowsContentURL = "/ChangeOwnerWaybill/ShowTORG12PrintingFormRows/";

                return View("~/Views/PrintingForm/Common/TORG12/TORG12PrintingForm.aspx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        /// <summary>
        /// Получение строк печатной формы ТОРГ12 
        /// </summary>
        public ActionResult ShowTORG12PrintingFormRows(TORG12PrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.GetTORG12PrintingFormRows(settings, UserSession.CurrentUserInfo);
                
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Накладная
        
        /// <summary>
        /// Получение окна параметров печатной формы
        /// </summary>
        public ActionResult ShowPrintingFormSettings(string waybillId)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.GetPrintingFormSettings(ValidationUtils.TryGetGuid(waybillId), UserSession.CurrentUserInfo);

                return View("~/Views/PrintingForm/ChangeOwnerWaybill/ChangeOwnerWaybillPrintingFormSettings.ascx", model);
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
        public ActionResult ShowPrintingForm(ChangeOwnerWaybillPrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.GetPrintingForm(settings, UserSession.CurrentUserInfo);

                return View("~/Views/PrintingForm/ChangeOwnerWaybill/ChangeOwnerWaybillPrintingForm.aspx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        #endregion

        #region Форма Т1 (TTH)

        /// <summary>
        /// Настройка печати формы Т1
        /// </summary>
        public ActionResult GetT1PrintingFormSettings(string waybillId)
        {
            try
            {
                var _waybillId = ValidationUtils.TryGetGuid(waybillId);
                var model = changeOwnerWaybillPresenter.GetT1PrintingFormSettings(_waybillId, UserSession.CurrentUserInfo);
                model.ActionUrl = "/ChangeOwnerWaybill/ShowT1ProductSectionPrintingForm";

                return PartialView("~/Views/PrintingForm/Common/T1/T1PrintingFormSettings.cshtml", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение печатной формы Т1
        /// </summary>
        public ActionResult ShowT1ProductSectionPrintingForm(T1PrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.ShowT1ProductSectionPrintingForm(settings, UserSession.CurrentUserInfo);
                model.RowsContentURL = "/ChangeOwnerWaybill/ShowT1ProductSectionPrintingFormRows";

                return PartialView("~/Views/PrintingForm/Common/T1/T1ProductSectionPrintingForm.cshtml", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение позиций печатной формы Т1
        /// </summary>
        public ActionResult ShowT1ProductSectionPrintingFormRows(T1PrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.ShowT1ProductSectionPrintingFormRows(settings, UserSession.CurrentUserInfo);

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #endregion

        #region Выбор накладной

        [HttpGet]
        public ActionResult SelectWaybill(string articleId)
        {
            try
            {
                var model = changeOwnerWaybillPresenter.SelectWaybill(ValidationUtils.TryGetInt(articleId), UserSession.CurrentUserInfo);

                return PartialView("ChangeOwnerWaybillSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowChangeOwnerWaybillSelectGrid(GridState state)
        {
            try
            {
                GridData data = changeOwnerWaybillPresenter.GetWaybillSelectGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ChangeOwnerWaybillSelectGrid", data);
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
