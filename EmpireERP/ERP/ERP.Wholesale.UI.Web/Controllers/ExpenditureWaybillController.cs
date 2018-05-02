using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ExpenditureWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.Т1;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ExpenditureWaybillController : BaseWaybillController<ExpenditureWaybill>
    {
        #region Поля

        private readonly IExpenditureWaybillPresenter expenditureWaybillPresenter;

        #endregion

        #region Конструкторы

        public ExpenditureWaybillController(IExpenditureWaybillPresenter expenditureWaybillPresenter)
            : base(expenditureWaybillPresenter)
        {
            this.expenditureWaybillPresenter = expenditureWaybillPresenter;
        }

        #endregion

        #region Список
        
        public ActionResult List()
        {
            try
            {
                return View(expenditureWaybillPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #region Новые и проведенные
        
        [HttpPost]
        public ActionResult ShowNewAndAcceptedExpenditureWaybillGrid(GridState state)
        {
            try
            {
                return PartialView("NewAndAcceptedExpenditureWaybillGrid", expenditureWaybillPresenter.GetNewAndAcceptedExpenditureWaybillGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Отгруженные

        [HttpPost]
        public ActionResult ShowShippedExpenditureWaybillGrid(GridState state)
        {
            try
            {                
                return PartialView("ShippedExpenditureWaybillGrid", expenditureWaybillPresenter.GetShippedExpenditureWaybillGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #endregion

        #region Добавление / редактирование

        /// <summary>
        /// Проверка возможности создания накладной реализации товаров по данной сделке
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CheckPosibilityToCreateExpenditureWaybill(string id)
        {
            try
            {
                expenditureWaybillPresenter.CheckPosibilityToCreate(ValidationUtils.TryGetInt(id), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Create(string backURL, int? dealId, int? clientId, int? clientOrganizationId)
        {
            try
            {
                return View("Edit", expenditureWaybillPresenter.Create(backURL, dealId, clientId, clientOrganizationId, UserSession.CurrentUserInfo));
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
                Guid waybillId = ValidationUtils.TryGetGuid(id);

                return View(expenditureWaybillPresenter.Edit(waybillId, backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        [HttpPost]
        public ActionResult Save(ExpenditureWaybillEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");
                
                return Content(expenditureWaybillPresenter.Save(model, UserSession.CurrentUserInfo).ToString());
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
        
        #region Удаление

        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(id);

                expenditureWaybillPresenter.Delete(waybillId, UserSession.CurrentUserInfo);

                TempData["Message"] = "Накладная удалена.";

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }               

        #endregion

        #region Детали накладной

        [HttpGet]
        public ActionResult Details(string id, string backURL)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(id);

                return View(expenditureWaybillPresenter.Details(waybillId, backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }                               

        #region Список позиций

        [HttpPost]
        public ActionResult ShowExpenditureWaybillRowGrid(GridState state)
        {
            try
            {                
                return PartialView("ExpenditureWaybillRowGrid", expenditureWaybillPresenter.GetExpenditureWaybillRowGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowExpenditureWaybillArticleGroupGrid(GridState state)
        {
            try
            {
                return PartialView("ExpenditureWaybillArticleGroupGrid", expenditureWaybillPresenter.GetExpenditureWaybillArticleGroupGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        #endregion

        #region Добавление / редактирование позиций

        [HttpGet]
        public ActionResult AddRow(string expenditureWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(expenditureWaybillId);

                return PartialView("ExpenditureWaybillRowEdit", expenditureWaybillPresenter.CreateRow(waybillId, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult EditRow(string expenditureWaybillId, string expenditureWaybillRowId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(expenditureWaybillId);
                Guid rowId = ValidationUtils.TryGetGuid(expenditureWaybillRowId);

                return PartialView("ExpenditureWaybillRowEdit", expenditureWaybillPresenter.EditRow(waybillId, rowId, UserSession.CurrentUserInfo));
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

                var result = expenditureWaybillPresenter.GetRowInfo(_waybillId, _batchId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveRow(ExpenditureWaybillRowEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(expenditureWaybillPresenter.SaveRow(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Удаление позиции

        [HttpPost]
        public ActionResult DeleteRow(string expenditureWaybillId, string expenditureWaybillRowId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(expenditureWaybillId);
                Guid rowId = ValidationUtils.TryGetGuid(expenditureWaybillRowId);

                return Json(expenditureWaybillPresenter.DeleteRow(waybillId, rowId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
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
                return PartialView("ExpenditureWaybillDocumentGrid", expenditureWaybillPresenter.GetDocumentGrid(state));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Подготовка / отмена готовности накладной к проводке

        [HttpPost]
        public ActionResult PrepareToAccept (string expenditureWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(expenditureWaybillId);

                return Json(expenditureWaybillPresenter.PrepareToAccept(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelReadinessToAccept(string expenditureWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(expenditureWaybillId);

                return Json(expenditureWaybillPresenter.CancelReadinessToAccept(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Проводка / отмена проводки

        [HttpPost]
        public ActionResult Accept(string expenditureWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(expenditureWaybillId);

                return Json(expenditureWaybillPresenter.Accept(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelAcceptance(string expenditureWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(expenditureWaybillId);

                return Json(expenditureWaybillPresenter.CancelAcceptance(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #region Проводка задним числом 
        
        public ActionResult AcceptRetroactively()
        {
            try
            {
                return View("~/Views/Common/DateTimeSelector.cshtml", expenditureWaybillPresenter.GetRetroactivelyAcceptanceViewModel(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult AcceptRetroactively(string expenditureWaybillId, string acceptanceDate)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(expenditureWaybillId);
                var _acceptanceDate = ValidationUtils.TryGetDate(acceptanceDate);

                return Json(expenditureWaybillPresenter.AcceptRetroactively(waybillId, _acceptanceDate, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        } 
        #endregion

        #endregion

        #region Отгрузка / отмена отгрузки

        [HttpPost]
        public ActionResult Ship(string expenditureWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(expenditureWaybillId);

                return Json(expenditureWaybillPresenter.Ship(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelShipping(string expenditureWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(expenditureWaybillId);

                return Json(expenditureWaybillPresenter.CancelShipping(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #region Отгрузка задним числом

        public ActionResult ShipRetroactively()
        {
            try
            {
                return View("~/Views/Common/DateTimeSelector.cshtml", expenditureWaybillPresenter.GetRetroactivelyShippingViewModel(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShipRetroactively(string expenditureWaybillId, string shippingDate)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(expenditureWaybillId);
                var _shippingDate = ValidationUtils.TryGetDate(shippingDate);

                return Json(expenditureWaybillPresenter.ShipRetroactively(waybillId, _shippingDate, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #endregion        

        #region Добавление позиций списком

        public ActionResult AddRowsByList(string id, string backURL)
        {
            try
            {
                return View(expenditureWaybillPresenter.AddRowsByList(ValidationUtils.TryGetGuid(id), backURL, UserSession.CurrentUserInfo));
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
                return PartialView("~/Views/OutgoingWaybill/ArticlesForWaybillRowsAdditionByListGrid.ascx", expenditureWaybillPresenter.GetArticlesForWaybillRowsAdditionByListGrid(state, UserSession.CurrentUserInfo));
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
                expenditureWaybillPresenter.AddRowSimply(ValidationUtils.TryGetGuid(waybillId), ValidationUtils.TryGetInt(articleId), 
                    ValidationUtils.TryGetDecimal(count), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Печатные формы

        #region Товарный чек
        [HttpGet]
        public ActionResult ShowCashMemoPrintingForm(string waybillId)
        {
            try
            {
                var _waybillId = ValidationUtils.TryGetGuid(waybillId);

                var model = expenditureWaybillPresenter.GetCashMemoPrintingForm(_waybillId, UserSession.CurrentUserInfo);

                return View("~/Views/PrintingForm/Common/CashMemo/CashMemoPrintingForm.aspx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        } 
        #endregion

        #region Расходная накладная

        /// <summary>
        /// Получение окна параметров печатной формы
        /// </summary>
        /// <param name="waybillId">Идентификатор накладной</param>
        public ActionResult ShowExpenditureWaybillPrintingFormSettings(string waybillId)
        {
            try
            {
                var _waybillId = ValidationUtils.TryGetGuid(waybillId);

                var model = expenditureWaybillPresenter.GetExpenditureWaybillPrintingFormSettings(_waybillId, UserSession.CurrentUserInfo);
                model.ActionUrl = "/ExpenditureWaybill/ShowExpenditureWaybillPrintingForm";

                return PartialView("~/Views/PrintingForm/ExpenditureWaybill/ExpenditureWaybillPrintingFormSettings.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение печатной формы расходной накладной
        /// </summary>
        /// <param name="settings">Параметры печатных форм</param>
        public ActionResult ShowExpenditureWaybillPrintingForm(ExpenditureWaybillPrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = expenditureWaybillPresenter.GetExpenditureWaybillPrintingForm(settings, UserSession.CurrentUserInfo);

                return View("~/Views/PrintingForm/ExpenditureWaybill/ExpenditureWaybillPrintingForm.aspx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        } 

        #endregion

        #region Счет-фактура
        
        /// <summary>
        /// Получение параметров печатной формы счета-фактуры
        /// </summary>
        /// <param name="waybillId">Идентификатор накладной</param>
        public ActionResult ShowInvoicePrintingFormSettings(string waybillId)
        {
            try
            {
                var waybillGuid = ValidationUtils.TryGetGuid(waybillId);

                var model = expenditureWaybillPresenter.GetInvoicePrintingFormSettings(waybillGuid, UserSession.CurrentUserInfo);
                model.ActionUrl = "/ExpenditureWaybill/ShowInvoicePrintingForm/";
                model.ExportToExcelUrl = "/ExpenditureWaybill/ExportInvoicePrintingFormToExcel";

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
                return ExcelFile(expenditureWaybillPresenter.ExportInvoicePrintingFormToExcel(settings, UserSession.CurrentUserInfo), "Invoice.xlsx");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение печатной формы счета-фактуры
        /// </summary>
        public ActionResult ShowInvoicePrintingForm(InvoicePrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = expenditureWaybillPresenter.GetInvoicePrintingForm(settings, UserSession.CurrentUserInfo);
                model.RowsContentURL = "/ExpenditureWaybill/ShowInvoicePrintingFormRows/";

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
                var model = expenditureWaybillPresenter.GetInvoicePrintingFormRows(settings, UserSession.CurrentUserInfo);

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

                var model = expenditureWaybillPresenter.GetTORG12PrintingFormSettings(waybillGuid, UserSession.CurrentUserInfo);
                model.ActionUrl = "/ExpenditureWaybill/ShowTORG12PrintingForm";
                model.ExportToExcelUrl = "/ExpenditureWaybill/ExportTORG12PrintingFormToExcel";

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
                return ExcelFile(expenditureWaybillPresenter.ExportTORG12PrintingFormToExcel(settings, UserSession.CurrentUserInfo), "TORG12.xlsx");
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
                var model = expenditureWaybillPresenter.GetTORG12PrintingForm(settings, UserSession.CurrentUserInfo);
                model.RowsContentURL = "/ExpenditureWaybill/ShowTORG12PrintingFormRows/";

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
                var model = expenditureWaybillPresenter.GetTORG12PrintingFormRows(settings, UserSession.CurrentUserInfo);

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Счет на оплату
        
        /// <summary>
        /// Получение окна параметров печатной формы
        /// </summary>
        /// <param name="waybillId">Идентификатор накладной</param>
        public ActionResult ShowPaymentInvoicePrintingFormSettings(string waybillId)
        {
            try
            {
                var _waybillId = ValidationUtils.TryGetGuid(waybillId);

                var model = expenditureWaybillPresenter.GetPaymentInvoicePrintingFormSettings(_waybillId, UserSession.CurrentUserInfo);
                model.ActionUrl = "/ExpenditureWaybill/ShowPaymentInvoicePrintingForm";

                return PartialView("~/Views/PrintingForm/Common/PaymentInvoice/PaymentInvoicePrintingFormSettings.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение печатной формы счета на оплату
        /// </summary>
        /// <param name="waybillId">Идентификатор накладной</param>
        [HttpGet]
        public ActionResult ShowPaymentInvoicePrintingForm(PaymentInvoicePrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = expenditureWaybillPresenter.GetPaymentInvoicePrintingForm(settings, UserSession.CurrentUserInfo);

                return View("~/Views/PrintingForm/Common/PaymentInvoice/PaymentInvoicePrintingForm.aspx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        } 

        #endregion

        #region Квоты

        /// <summary>
        /// Установка квоты для указанной накладной реализации товаров.
        /// </summary>
        /// <param name="expenditureWaybillId">Идентификатор накладной реализации товаров.</param>
        /// <param name="dealQuotaId">Идентификатор квоты.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetDealQuota(string expenditureWaybillId, string dealQuotaId)
        {
            try
            {
                var result = expenditureWaybillPresenter.SetDealQuota(ValidationUtils.TryGetGuid(expenditureWaybillId), ValidationUtils.TryGetShort(dealQuotaId), UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
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
                var model = expenditureWaybillPresenter.GetT1PrintingFormSettings(_waybillId, UserSession.CurrentUserInfo);
                model.ActionUrl = "/ExpenditureWaybill/ShowT1ProductSectionPrintingForm";

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
                var model = expenditureWaybillPresenter.ShowT1ProductSectionPrintingForm(settings, UserSession.CurrentUserInfo);
                model.RowsContentURL = "/ExpenditureWaybill/ShowT1ProductSectionPrintingFormRows";

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
                var model = expenditureWaybillPresenter.ShowT1ProductSectionPrintingFormRows(settings, UserSession.CurrentUserInfo);

                return Json(model, JsonRequestBehavior.AllowGet);
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