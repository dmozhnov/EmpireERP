using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.MovementWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;
using ERP.Wholesale.UI.ViewModels.PrintingForm.MovementWaybill;
using ERP.Wholesale.UI.Web.Infrastructure;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.Т1;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class MovementWaybillController : BaseWaybillController<MovementWaybill>
    {
        #region Поля

        private readonly IMovementWaybillPresenter movementWaybillPresenter;

        #endregion

        #region Конструктор
              
        public MovementWaybillController(IMovementWaybillPresenter movementWaybillPresenter) 
            :base(movementWaybillPresenter)
        {
            this.movementWaybillPresenter = movementWaybillPresenter;
        }

        #endregion

        #region Список
        public ActionResult List()
        {
            try
            {
                var model = movementWaybillPresenter.List(UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #region Новые (ожидающие отгрузки)

        [HttpPost]
        public ActionResult ShowShippingPendingGrid(GridState state)
        {
            try
            {
                GridData data = movementWaybillPresenter.GetShippingPendingGrid(state, UserSession.CurrentUserInfo);

                return PartialView("MovementWaybillShippingPendingGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowShippedGrid(GridState state)
        {
            try
            {
                GridData data = movementWaybillPresenter.GetShippedGrid(state, UserSession.CurrentUserInfo);

                return PartialView("MovementWaybillShippedGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowReceiptedGrid(GridState state)
        {
            try
            {
                GridData data = movementWaybillPresenter.GetReceiptedGrid(state, UserSession.CurrentUserInfo);

                return PartialView("MovementWaybillReceiptedGrid", data);
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
                var model = movementWaybillPresenter.Create(backURL, UserSession.CurrentUserInfo);

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
                var waybillId = ValidationUtils.TryGetGuid(id);

                var model = movementWaybillPresenter.Edit(waybillId, backURL, UserSession.CurrentUserInfo);

                return View("Edit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult GetAccountOrganizationsForSenderStorage(short storageId)
        {
            try
            {
                var x = movementWaybillPresenter.GetAccountOrganizationsForSenderStorage(storageId, UserSession.CurrentUserInfo);

                return Json(x, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult GetAccountOrganizationsForRecipientStorage(short storageId)
        {
            try
            {
                var x = movementWaybillPresenter.GetAccountOrganizationsForRecipientStorage(storageId, UserSession.CurrentUserInfo);

                return Json(x, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(MovementWaybillEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Значение входного параметра не задано.");

                var id = movementWaybillPresenter.Save(model, UserSession.CurrentUserInfo);

                return Content(id);

            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление позиций списком

        public ActionResult AddRowsByList(string id, string backURL)
        {
            try
            {
                return View(movementWaybillPresenter.AddRowsByList(ValidationUtils.TryGetGuid(id), backURL, UserSession.CurrentUserInfo));
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
                return PartialView("~/Views/OutgoingWaybill/ArticlesForWaybillRowsAdditionByListGrid.ascx", movementWaybillPresenter.GetArticlesForWaybillRowsAdditionByListGrid(state, UserSession.CurrentUserInfo));
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
                movementWaybillPresenter.AddRowSimply(ValidationUtils.TryGetGuid(waybillId), ValidationUtils.TryGetInt(articleId),
                    ValidationUtils.TryGetDecimal(count), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Детали накладной

        public ActionResult Details(string id, string backURL)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(id);

                var model = movementWaybillPresenter.Details(waybillId, backURL, UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }        

        #region Список позиций и групп товаров

        [HttpPost]
        public ActionResult ShowMovementWaybillRowGrid(GridState state)
        {
            try
            {
                GridData data = movementWaybillPresenter.GetMovementWaybillRowGrid(state, UserSession.CurrentUserInfo);

                return PartialView("MovementWaybillRowGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowMovementWaybillArticleGroupGrid(GridState state)
        {
            try
            {
                GridData data = movementWaybillPresenter.GetMovementWaybillArticleGroupGrid(state, UserSession.CurrentUserInfo);

                return PartialView("MovementWaybillArticleGroupGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / редактирование строк накладной

        [HttpGet]
        public ActionResult AddRow(string movementWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetNotEmptyGuid(movementWaybillId);

                var model = movementWaybillPresenter.AddRow(waybillId, UserSession.CurrentUserInfo);

                return PartialView("MovementWaybillRowEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult EditRow(string movementWaybillId, string movementWaybillRowId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetNotEmptyGuid(movementWaybillId);
                Guid rowId = ValidationUtils.TryGetNotEmptyGuid(movementWaybillRowId);

                var model = movementWaybillPresenter.EditRow(waybillId, rowId, UserSession.CurrentUserInfo);

                return PartialView("MovementWaybillRowEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }


        [HttpPost]
        public ActionResult EditRow(MovementWaybillRowEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                ValidationUtils.NotNullOrDefault(model.MovementWaybillId, "Накладная перемещения не найдена. Возможно, она была удалена.");

                ValidationUtils.Assert((model.ReceiptWaybillRowId != null && model.ReceiptWaybillRowId != Guid.Empty) || !String.IsNullOrEmpty(model.ManualSourcesInfo),
                    "Укажите партию товара или источники.");

                var result = movementWaybillPresenter.SaveRow(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
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

                var result = movementWaybillPresenter.GetRowInfo(_waybillId, _batchId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult DeleteRow(Guid movementWaybillId, Guid movementWaybillRowId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(movementWaybillId, "Неверное значение входного параметра.");
                ValidationUtils.NotNullOrDefault(movementWaybillRowId, "Неверное значение входного параметра.");

                var x = movementWaybillPresenter.DeleteRow(movementWaybillId, movementWaybillRowId, UserSession.CurrentUserInfo);

                return Json(x, JsonRequestBehavior.AllowGet);
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
                GridData data = movementWaybillPresenter.GetDocGrid(state, UserSession.CurrentUserInfo);

                return PartialView("MovementWaybillDocGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Подготовка/отмена подготовки к проводке

        [HttpPost]
        public ActionResult PrepareToAccept(string id)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(id);

                var result = movementWaybillPresenter.PrepareToAccept(waybillId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelReadinessToAccept(string id)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(id);

                var result = movementWaybillPresenter.CancelReadinessToAccept(waybillId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Отгрузка, отмена отгрузки, удаление накладной

        [HttpPost]
        public ActionResult Ship(string id)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(id);

                var result = movementWaybillPresenter.Ship(waybillId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }       

        [HttpPost]
        public ActionResult CancelShipping(string id)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(id);

                var result = movementWaybillPresenter.CancelShipping(waybillId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Accept(string id)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(id);

                var result = movementWaybillPresenter.Accept(waybillId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);

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
                var waybillId = ValidationUtils.TryGetGuid(id);

                var result = movementWaybillPresenter.CancelAcceptance(waybillId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Receipt(string id)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(id);

                var result = movementWaybillPresenter.Receipt(waybillId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelReceipt(string id)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(id);

                var result = movementWaybillPresenter.CancelReceipt(waybillId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
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
                var waybillId = ValidationUtils.TryGetGuid(id);

                movementWaybillPresenter.Delete(waybillId, UserSession.CurrentUserInfo);

                    return Content("");
                
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion    

        #region Печатные формы

        #region Накладная перемещения
        
        /// <summary>
        /// Получение окна параметров печатной формы
        /// </summary>
        public ActionResult ShowPrintingFormSettings(bool PrintSenderPrice, bool PrintRecepientPrice, bool PrintMarkup)
        {
            try
            {
                var model = movementWaybillPresenter.GetPrintingFormSettings(PrintSenderPrice, PrintRecepientPrice, PrintMarkup, UserSession.CurrentUserInfo);

                return PartialView("~/Views/PrintingForm/MovementWaybill/MovementWaybillPrintingFormSettings.ascx", model);
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
        public ActionResult ShowPrintingForm(MovementWaybillPrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = movementWaybillPresenter.GetPrintingForm(settings, UserSession.CurrentUserInfo);

                return View("~/Views/PrintingForm/MovementWaybill/MovementWaybillPrintingForm.aspx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        } 
        #endregion

        #region Товарный чек
        
        /// <summary>
        /// Получение окна параметров печатной формы "Товарный чек"
        /// </summary>
        [HttpGet]
        public ActionResult ShowCashMemoPrintingFormSettings(string waybillId)
        {
            try
            {
                var waybillGuid = ValidationUtils.TryGetGuid(waybillId);
                var model = movementWaybillPresenter.GetCashMemoPrintingFormSettings(waybillGuid, UserSession.CurrentUserInfo);

                return PartialView("~/Views/PrintingForm/Common/CashMemo/CashMemoPrintingFormSettings.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение печатной формы "Товарный чек"
        /// </summary>
        /// <param name="waybillId">Идентификатор накладной</param>
        [HttpGet]
        public ActionResult ShowCashMemoPrintingForm(CashMemoPrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = movementWaybillPresenter.GetCashMemoPrintingForm(settings, UserSession.CurrentUserInfo);

                return View("~/Views/PrintingForm/Common/CashMemo/CashMemoPrintingForm.aspx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        } 
        #endregion

        #region Счет-фактура
        
        /// <summary>
        /// Получение окна параметров печатной формы
        /// </summary>
        public ActionResult ShowInvoicePrintingFormSettings(string waybillId)
        {
            try
            {
                var waybillGuid = ValidationUtils.TryGetGuid(waybillId);
                
                var model = movementWaybillPresenter.GetInvoicePrintingFormSettings(waybillGuid, UserSession.CurrentUserInfo);
                model.ActionUrl = "/MovementWaybill/ShowInvoicePrintingForm/";
                model.ExportToExcelUrl = "/MovementWaybill/ExportInvoicePrintingFormToExcel";

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
                return ExcelFile(movementWaybillPresenter.ExportInvoicePrintingFormToExcel(settings, UserSession.CurrentUserInfo), "Invoice.xlsx");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение печатной формы счета-фактуры
        /// </summary>
        /// <param name="waybillId">Идентификатор накладной</param>
        [HttpGet]
        public ActionResult ShowInvoicePrintingForm(InvoicePrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = movementWaybillPresenter.GetInvoicePrintingForm(settings, UserSession.CurrentUserInfo);
                model.RowsContentURL = "/MovementWaybill/ShowInvoicePrintingFormRows/";

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
        [HttpGet]
        public ActionResult ShowInvoicePrintingFormRows(InvoicePrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = movementWaybillPresenter.GetInvoicePrintingFormRows(settings, UserSession.CurrentUserInfo);

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
                var waybillGuid = ValidationUtils.TryGetGuid(waybillId);
                
                var model = movementWaybillPresenter.GetTORG12PrintingFormSettings(waybillGuid, UserSession.CurrentUserInfo);
                model.ActionUrl = "/MovementWaybill/ShowTORG12PrintingForm";
                model.ExportToExcelUrl = "/MovementWaybill/ExportTORG12PrintingFormToExcel";

                return PartialView("~/Views/PrintingForm/Common/TORG12/TORG12PrintingFormSettings.ascx", model);
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
                var model = movementWaybillPresenter.GetTORG12PrintingForm(settings, UserSession.CurrentUserInfo);
                model.RowsContentURL = "/MovementWaybill/ShowTORG12PrintingFormRows/";

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
                var model = movementWaybillPresenter.GetTORG12PrintingFormRows(settings, UserSession.CurrentUserInfo);
                
                return Json(model, JsonRequestBehavior.AllowGet);
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
                return ExcelFile(movementWaybillPresenter.ExportTORG12PrintingFormToExcel(settings, UserSession.CurrentUserInfo), "TORG12.xlsx");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Форма Т1

        /// <summary>
        /// Настройка печати формы Т1
        /// </summary>
        public ActionResult GetT1PrintingFormSettings(string waybillId)
        {
            try
            {
                var _waybillId = ValidationUtils.TryGetGuid(waybillId);
                var model = movementWaybillPresenter.GetT1PrintingFormSettings(_waybillId, UserSession.CurrentUserInfo);
                model.ActionUrl = "/MovementWaybill/ShowT1ProductSectionPrintingForm";

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
                ValidationUtils.NotNull(settings);
                var model = movementWaybillPresenter.ShowT1ProductSectionPrintingForm(settings, UserSession.CurrentUserInfo);
                model.RowsContentURL = "/MovementWaybill/ShowT1ProductSectionPrintingFormRows";

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
                ValidationUtils.NotNull(settings);
                var model = movementWaybillPresenter.ShowT1ProductSectionPrintingFormRows(settings, UserSession.CurrentUserInfo);

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
                var model = movementWaybillPresenter.SelectWaybill(ValidationUtils.TryGetInt(articleId), UserSession.CurrentUserInfo);

                return PartialView("MovementWaybillSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowMovementWaybillSelectGrid(GridState state)
        {
            try
            {
                GridData data = movementWaybillPresenter.GetWaybillSelectGrid(state, UserSession.CurrentUserInfo);

                return PartialView("MovementWaybillSelectGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

    }
}
