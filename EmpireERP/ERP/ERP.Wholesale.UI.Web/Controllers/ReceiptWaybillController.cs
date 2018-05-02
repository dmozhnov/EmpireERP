using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;
using ERP.Wholesale.UI.ViewModels.PrintingForm.ReceiptWaybill;
using ERP.Wholesale.UI.ViewModels.ReceiptWaybill;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ReceiptWaybillController : BaseWaybillController<ReceiptWaybill>
    {
        #region Поля

        private readonly IReceiptWaybillPresenter receiptWaybillPresenter;        
     
        #endregion

        #region Конструктор

        public ReceiptWaybillController(IReceiptWaybillPresenter receiptWaybillPresenter)
            : base(receiptWaybillPresenter)
        {
            this.receiptWaybillPresenter = receiptWaybillPresenter;
        }      

        #endregion

        #region Список

        public ActionResult List()
        {
            try
            {
                return View(receiptWaybillPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowDeliveryPendingGrid(GridState state)
        {
            try
            {
                GridData data = receiptWaybillPresenter.GetDeliveryPendingGrid(state, UserSession.CurrentUserInfo);

                return PartialView("DeliveryPendingGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowDivergenceWaybillGrid(GridState state)
        {
            try
            {
                GridData data = receiptWaybillPresenter.GetDivergenceWaybillGrid(state, UserSession.CurrentUserInfo);

                return PartialView("DivergenceWaybillGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowApprovedWaybillGrid(GridState state)
        {
            try
            {
                GridData data = receiptWaybillPresenter.GetApprovedWaybillGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ApprovedWaybillGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Создание, редактирование, удаление

        [HttpGet]
        public ActionResult Create(string providerId, string productionOrderBatchId, string backURL)
        {
            try
            {
                if (!String.IsNullOrEmpty(productionOrderBatchId))
                {
                    return View("Edit", receiptWaybillPresenter.CreateFromProductionOrderBatch(
                        ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId),
                        backURL, UserSession.CurrentUserInfo));
                }
                else
                {
                    return View("Edit", receiptWaybillPresenter.Create(
                        !String.IsNullOrEmpty(providerId) ? ValidationUtils.TryGetInt(providerId) : (int?)null,
                        backURL, UserSession.CurrentUserInfo));
                }
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

                return View(receiptWaybillPresenter.Edit(waybillId, backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(ReceiptWaybillEditViewModel model)
        {
            try
            {
                string message;
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                var result = receiptWaybillPresenter.Save(model, out message, UserSession.CurrentUserInfo);

                if (!String.IsNullOrEmpty(message))
                {
                    TempData["Message"] = message;
                }

                return Content(result);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Delete(string id, string returnProductionOrderBatchDetails = "0")
        {
            try
            {
                string message;
                Guid waybillId = ValidationUtils.TryGetGuid(id);

                var result = receiptWaybillPresenter.Delete(waybillId,
                    ValidationUtils.TryGetBool(returnProductionOrderBatchDetails),
                    out message,
                    UserSession.CurrentUserInfo);

                if (!String.IsNullOrEmpty(message))
                {
                    TempData["Message"] = message;
                }

                return Json(result, JsonRequestBehavior.AllowGet);
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
                var model = receiptWaybillPresenter.Details(id, Server.UrlEncode(backURL), UserSession.CurrentUserInfo);

                return View("Details", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #region Детали накладной с расхождениями

        [HttpPost]
        public ActionResult ShowAddedRowInReceiptWaybillGrid(GridState state)
        {
            try
            {
                var data = receiptWaybillPresenter.GetAddedRowInReceiptWaybillGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillWithDifDetailsAddedRowGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowAddedArticleGroupInReceiptWaybillGrid(GridState state)
        {
            try
            {
                var data = receiptWaybillPresenter.GetAddedArticleGroupInReceiptWaybillGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillWithDifDetailsAddedArticleGroupGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowDifRowInReceiptWaybillGrid(GridState state)
        {
            try
            {
                var data = receiptWaybillPresenter.GetDifRowInReceiptWaybillGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillWithDifDetailsDifRowGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowDifArticleGroupInReceiptWaybillGrid(GridState state)
        {
            try
            {
                var data = receiptWaybillPresenter.GetDifArticleGroupInReceiptWaybillGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillWithDifDetailsDifArticleGroupGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowMatchRowInReceiptWaybillGrid(GridState state)
        {
            try
            {
                var data = receiptWaybillPresenter.GetMatchRowInReceiptWaybillGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillWithDifDetailsMatchRowGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowMatchArticleGroupInReceiptWaybillGrid(GridState state)
        {
            try
            {
                var data = receiptWaybillPresenter.GetMatchArticleGroupInReceiptWaybillGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillWithDifDetailsMatchArticleGroupGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Детали окончательно согласованной накладной

        [HttpPost]
        public ActionResult ShowApprovementDetailsReceiptWaybillGrid(GridState state)
        {
            try
            {
                var data = receiptWaybillPresenter.GetApproveWaybillRowGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillApprovementDetailsGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowApprovementArticleGroupGrid(GridState state)
        {
            try
            {
                GridData data = receiptWaybillPresenter.GetApproveWaybillArticleGroupGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillApprovementArticleGroupGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Состав приходной накладной

        [HttpPost]
        public ActionResult ShowReceiptWaybillRowGrid(GridState state)
        {
            try
            {
                var data = receiptWaybillPresenter.GetReceiptWaybillRowGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillRowGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowReceiptWaybillArticleGroupGrid(GridState state)
        {
            try
            {
                GridData data = receiptWaybillPresenter.GetReceiptWaybillArticleGroupGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillArticleGroupGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult GetManufacturerList()
        {
            try
            {
                var result = receiptWaybillPresenter.GetManufacturerList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }       

        [HttpGet]
        public ActionResult AddRow(string receiptWaybillId)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(receiptWaybillId);

                var model = receiptWaybillPresenter.AddRow(waybillId, UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillRowForEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult EditRow(string receiptWaybillId, string receiptWaybillRowId)
        {
            try
            {
                var waybillGuid = ValidationUtils.TryGetGuid(receiptWaybillId);
                var rowGuid = ValidationUtils.TryGetGuid(receiptWaybillRowId);

                var model = receiptWaybillPresenter.EditRow(waybillGuid, rowGuid, UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillRowForEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult EditRow(ReceiptWaybillRowEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                var result = receiptWaybillPresenter.SaveRow(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение единиц измерения по товару
        /// </summary>
        /// <param name="articleId">Идентификатор товара</param>
        /// <returns>JSon</returns>
        [HttpPost]
        public ActionResult GetArticleInfo(string articleId)
        {
            var result = receiptWaybillPresenter.GetArticleInfo(ValidationUtils.TryGetInt(articleId));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Получить последнюю по проводке закупочную цену на товар.
        /// </summary>
        [HttpGet]
        public ActionResult GetLastPurchaseCost(string articleId, string waybillId)
        {
            try
            {
                return Content(receiptWaybillPresenter.GetLastPurchaseCost(ValidationUtils.TryGetInt(articleId),
                                ValidationUtils.TryGetGuid(waybillId), UserSession.CurrentUserInfo));
            }
            catch(Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получить номер ГТД для позиции
        /// </summary>
        [HttpGet]
        public ActionResult GetCustomsDeclarationNumberForRow(string articleId, string waybillId)
        {
            try
            {
                var result = receiptWaybillPresenter.GetCustomsDeclarationNumberForRow(ValidationUtils.TryGetInt(articleId),
                                ValidationUtils.TryGetGuid(waybillId), UserSession.CurrentUserInfo);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult DeleteRow(string receiptWaybillId, string receiptWaybillRowId)
        {
            try
            {
                var waybillGuid = ValidationUtils.TryGetGuid(receiptWaybillId);
                var rowGuid = ValidationUtils.TryGetGuid(receiptWaybillRowId);

                var result = receiptWaybillPresenter.DeleteRow(waybillGuid, rowGuid, UserSession.CurrentUserInfo);

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
        public ActionResult ShowDocumentsGrid(GridState state)
        {
            try
            {
                GridData data = receiptWaybillPresenter.GetDocumentsGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillDocumentsGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Проводка

        [HttpPost]
        public ActionResult Accept(string Id, string backURL)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(Id);

                var result = receiptWaybillPresenter.Accept(waybillId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelAcceptance(string receiptWaybillId)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(receiptWaybillId);

                var result = receiptWaybillPresenter.CancelAcceptance(waybillId, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Проводка задним числом

        public ActionResult AcceptRetroactively()
        {
            try
            {
                return View("~/Views/Common/DateTimeSelector.cshtml", receiptWaybillPresenter.GetRetroactivelyAcceptanceViewModel(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult AcceptRetroactively(string receiptWaybillId, string acceptanceDate)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(receiptWaybillId);
                var _acceptanceDate = ValidationUtils.TryGetDate(acceptanceDate);

                return Json(receiptWaybillPresenter.AcceptRetroactively(waybillId, _acceptanceDate, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Приемка

        [HttpGet]
        public ActionResult Receipt(string Id, string backURL)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(Id);

                var model = receiptWaybillPresenter.GetReceiptViewModel(waybillId, backURL, UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult PerformReceiption(string waybillId, string sum)
        {
            try
            {
                string message;
                receiptWaybillPresenter.PerformReceiption(ValidationUtils.TryGetNotEmptyGuid(waybillId),
                    !String.IsNullOrEmpty(sum) ? ValidationUtils.TryGetDecimal(sum) : (decimal?)null, out message,
                    UserSession.CurrentUserInfo);

                if (!String.IsNullOrEmpty(message))
                {
                    TempData["Message"] = message;
                }

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowReceiptArticlesGrid(GridState state)
        {
            try
            {
                GridData data = receiptWaybillPresenter.GetReceiptArticlesGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReceiptedArticlesGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }


        [HttpGet]
        public ActionResult AddWaybillRowFromReceipt(string waybillId)
        {
            try
            {               
                var waybillGuid = ValidationUtils.TryGetGuid(waybillId);

                var model = receiptWaybillPresenter.AddWaybillRowFromReceipt(waybillGuid, UserSession.CurrentUserInfo);

                return PartialView("AddWaybillRowFromReceipt", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }                        
        }

        [HttpPost]
        public ActionResult AddWaybillRowFromReceipt(ReceiptRowAddViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                receiptWaybillPresenter.PerformWaybillRowAdditionFromReceipt(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult EditWaybillRowFromReceipt(string waybillId, string rowId, string receiptedCount, string providerCount, string providerSum)
        {
            try
            {
                var waybillGuid = ValidationUtils.TryGetGuid(waybillId);
                var rowGuid = ValidationUtils.TryGetGuid(rowId);
                var receiptedCountValue = ValidationUtils.TryGetDecimal(receiptedCount, 12, 6,
                    "Введите принимаемое количество.",
                    "Принимаемое количество имеет слишком большое число цифр.",
                    "Принимаемое количество имеет слишком большое число цифр после запятой.");
                var providerCountValue = ValidationUtils.TryGetDecimal(providerCount, 12, 6,
                    "Введите количество по документу.",
                    "Количество по документу имеет слишком большое число цифр.",
                    "Количество по документу имеет слишком большое число цифр после запятой.");
                var providerSumValue = ValidationUtils.TryGetDecimal(providerSum, 16, 2,
                    "Введите сумму по документу.",
                    "Сумма по документу имеет слишком большое число цифр.",
                    "Сумма по документу имеет слишком большое число цифр после запятой.");

                receiptWaybillPresenter.EditWaybillRowFromReceipt(waybillGuid, rowGuid,
                    receiptedCountValue, providerCountValue, providerSumValue, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult DeleteWaybillRowFromReceipt(string waybillId, string rowId)
        {
            try
            {
                var waybillGuid = ValidationUtils.TryGetGuid(waybillId);
                var rowGuid = ValidationUtils.TryGetGuid(rowId);

                receiptWaybillPresenter.DeleteWaybillRowFromReceipt(waybillGuid, rowGuid, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelReceipt(string receiptWaybillId)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(receiptWaybillId);

                var result = receiptWaybillPresenter.CancelReceipt(waybillId, UserSession.CurrentUserInfo);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Приемка задним числом

        public ActionResult ReceiptRetroactively()
        {
            try
            {
                return View("~/Views/Common/DateTimeSelector.cshtml", receiptWaybillPresenter.GetRetroactivelyReceiptViewModel(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ReceiptRetroactively(string receiptWaybillId, string receiptDate, string receiptanceSum)
        {
            try
            {
                string message;
                receiptWaybillPresenter.ReceiptRetroactively(ValidationUtils.TryGetNotEmptyGuid(receiptWaybillId), ValidationUtils.TryGetDate(receiptDate),
                    !String.IsNullOrEmpty(receiptanceSum) ? ValidationUtils.TryGetDecimal(receiptanceSum) : (decimal?)null, out message,
                    UserSession.CurrentUserInfo);

                if (!String.IsNullOrEmpty(message))
                {
                    TempData["Message"] = message;
                }

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Окончательное согласование накладной

        [HttpGet]
        public ActionResult Approve(string Id, string backURL)
        {
            try
            {
                var waybillId = ValidationUtils.TryGetGuid(Id);

                var model = receiptWaybillPresenter.GetApprovementViewModel(waybillId, backURL, UserSession.CurrentUserInfo);

                return View("Approve", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelApprovement(string receiptWaybillId)
        {
            try
            {
                var waybillGuid = ValidationUtils.TryGetGuid(receiptWaybillId);

                var result = receiptWaybillPresenter.CancelApprovement(waybillGuid, UserSession.CurrentUserInfo);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowReceiptWaybillApprovementArticlesGrid(GridState state)
        {
            try
            {
                GridData data = receiptWaybillPresenter.GetApproveArticlesGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillApprovementArticlesGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult EditWaybillRowFromApprovement(string waybillId, string rowId, string approvedCount, string purchaseCost)
        {
            try
            {
                var waybillGuid = ValidationUtils.TryGetGuid(waybillId);
                var rowGuid = ValidationUtils.TryGetGuid(rowId);
                var approvedCountValue = ValidationUtils.TryGetDecimal(approvedCount, 12, 6,
                    "Введите согласованное количество.",
                    "Согласованное количество имеет слишком большое число цифр.",
                    "Согласованное количество имеет слишком большое число цифр после запятой.");
                var purchaseCostValue = ValidationUtils.TryGetDecimal(purchaseCost, 12, 6,
                    "Введите согласованную закупочную цену.",
                    "Согласованная закупочная цена имеет слишком большое число цифр.",
                    "Согласованная закупочная цена имеет слишком большое число цифр после запятой.");

                receiptWaybillPresenter.EditWaybillRowFromApprovement(waybillGuid, rowGuid, approvedCountValue, purchaseCostValue, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult EditWaybillRowValueAddedTaxFromApprovement(string waybillId, string rowId, string valueAddedTaxId)
        {
            try
            {
                var waybillGuid = ValidationUtils.TryGetGuid(waybillId);
                var rowGuid = ValidationUtils.TryGetGuid(rowId);
                var valueAddedTaxIdValue = ValidationUtils.TryGetShort(valueAddedTaxId);

                receiptWaybillPresenter.EditWaybillRowValueAddedTaxFromApprovement(waybillGuid, rowGuid, valueAddedTaxIdValue, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult PerformApprovement(string waybillId, string sum)
        {
            try
            {
                var waybillGuid = ValidationUtils.TryGetGuid(waybillId);
                receiptWaybillPresenter.PerformApprovement(waybillGuid, ValidationUtils.TryGetDecimal(sum), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Согласование задним числом

        public ActionResult ApproveRetroactively()
        {
            try
            {
                return View("~/Views/Common/DateTimeSelector.cshtml", receiptWaybillPresenter.GetRetroactivelyApprovementViewModel(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ApproveRetroactively(string receiptWaybillId, string approvementDate, string approvedSum)
        {
            try
            {
                receiptWaybillPresenter.ApproveRetroactively(ValidationUtils.TryGetGuid(receiptWaybillId), ValidationUtils.TryGetDecimal(approvedSum), ValidationUtils.TryGetDate(approvementDate), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Печатные формы

        /// <summary>
        /// Получение окна параметров печатной формы
        /// </summary>
        public ActionResult ShowPrintingFormSettings(string waybillId)
        {
            try
            {
                var model = receiptWaybillPresenter.GetPrintingFormSettings(ValidationUtils.TryGetNotEmptyGuid(waybillId), UserSession.CurrentUserInfo);

                return View("~/Views/PrintingForm/ReceiptWaybill/ReceiptWaybillPrintingFormSettings.ascx", model);
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
        public ActionResult ShowPrintingForm(ReceiptWaybillPrintingFormSettingsViewModel settings)
        {
            try
            {
                var model = receiptWaybillPresenter.GetPrintingForm(settings, UserSession.CurrentUserInfo);

                return View("~/Views/PrintingForm/ReceiptWaybill/ReceiptWaybillPrintingForm.aspx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #region Акт расхождений

        /// <summary>
        /// Получение печатной формы акта расхождений
        /// </summary>
        /// <param name="settings">Параметры печатных форм</param>
        public ActionResult ShowDivergenceActPrintingForm(string waybillId)
        {
            try
            {
                var waybillGuid = ValidationUtils.TryGetGuid(waybillId);

                var model = receiptWaybillPresenter.GetDivergenceActPrintingForm(waybillGuid, UserSession.CurrentUserInfo);

                return View("~/Views/PrintingForm/ReceiptWaybill/DivergenceAct/DivergenceActPrintingForm.aspx", model);
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

                var model = receiptWaybillPresenter.GetTORG12PrintingFormSettings(waybillGuid, UserSession.CurrentUserInfo);
                model.ActionUrl = "/ReceiptWaybill/ShowTORG12PrintingForm";
                model.ExportToExcelUrl = "/ReceiptWaybill/ExportTORG12PrintingFormToExcel";

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
                return ExcelFile(receiptWaybillPresenter.ExportTORG12PrintingFormToExcel(settings, UserSession.CurrentUserInfo), "TORG12.xlsx");
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
                var model = receiptWaybillPresenter.GetTORG12PrintingForm(settings, UserSession.CurrentUserInfo);
                model.RowsContentURL = "/ReceiptWaybill/ShowTORG12PrintingFormRows/";

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
                var model = receiptWaybillPresenter.GetTORG12PrintingFormRows(settings, UserSession.CurrentUserInfo);

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Работа со связанными ComboBox-ми по выбору поставщика, договора, МХ, организации получателя

        public ActionResult UpdateContractList(string providerId, string receiptStorageId, string accountOrganizationId)
        {
            try
            {
                int _providerId = ConvertToIntOrDefault(providerId);
                int _receiptStorageId = ConvertToIntOrDefault(receiptStorageId);
                int _accountOrganizationId = ConvertToIntOrDefault(accountOrganizationId);

                var obj = receiptWaybillPresenter.GetContractList(_providerId, _receiptStorageId, _accountOrganizationId);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult UpdateReceiptStorageList(string providerId, string contractId, string accountOrganizationId)
        {
            try
            {
                int _providerId = ConvertToIntOrDefault(providerId);
                short _contractId = ConvertToShortOrDefault(contractId);
                int _accountOrganizationId = ConvertToIntOrDefault(accountOrganizationId);

                var obj = receiptWaybillPresenter.GetReceiptStorageList(_providerId, _contractId, _accountOrganizationId, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult UpdateAccountOrganizationList(string providerId, string contractId, string receiptStorageId)
        {
            try
            {
                int _providerId = ConvertToIntOrDefault(providerId);
                short _contractId = ConvertToShortOrDefault(contractId);
                int _receiptStorageId = ConvertToIntOrDefault(receiptStorageId);

                var obj = receiptWaybillPresenter.GetAccountOrganizationList(_providerId, _contractId, _receiptStorageId, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        private int ConvertToIntOrDefault(string value)
        {
            int result = 0;
            if (!(value.Length == 0))
            {
                result = ValidationUtils.TryGetInt(value);
            }

            return result;
        }

        private short ConvertToShortOrDefault(string value)
        {
            short result = 0;
            if (!(value.Length == 0))
            {
                result = ValidationUtils.TryGetShort(value);
            }

            return result;
        }
        #endregion

        #region Выбор накладной

        [HttpGet]
        public ActionResult SelectWaybill(string articleId)
        {
            try
            {
                var model = receiptWaybillPresenter.SelectWaybill(ValidationUtils.TryGetInt(articleId), UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowReceiptWaybillSelectGrid(GridState state)
        {
            try
            {
                GridData data = receiptWaybillPresenter.GetWaybillSelectGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReceiptWaybillSelectGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

    }
}
