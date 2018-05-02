using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ReturnFromClientWaybill;
using ERP.Wholesale.UI.Web.Infrastructure;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ReturnFromClientWaybillController : BaseWaybillController<ReturnFromClientWaybill>
    {
        #region Поля

        private readonly IReturnFromClientWaybillPresenter returnFromClientWaybillPresenter;

        #endregion

        #region Конструкторы

        public ReturnFromClientWaybillController(IReturnFromClientWaybillPresenter returnFromClientWaybillPresenter)
            :base(returnFromClientWaybillPresenter)
        {
            this.returnFromClientWaybillPresenter = returnFromClientWaybillPresenter;
        }

        #endregion

        #region Методы

        #region Список

        public ActionResult List()
        {
            try
            {
                return View(returnFromClientWaybillPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #region Новые и проведенные

        [HttpPost]
        public ActionResult ShowNewAndAcceptedReturnFromClientWaybillGrid(GridState state)
        {
            try
            {
                return PartialView("NewAndAcceptedReturnFromClientWaybillGrid", 
                    returnFromClientWaybillPresenter.GetNewAndAcceptedReturnFromClientWaybillGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Отгруженные

        [HttpPost]
        public ActionResult ShowReceiptedReturnFromClientWaybillGrid(GridState state)
        {
            try
            {
                return PartialView("ReceiptedReturnFromClientWaybillGrid", 
                    returnFromClientWaybillPresenter.GetReceiptedReturnFromClientWaybillGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion 

        #endregion

        #region Создание, редактирование, удаление

        [HttpGet]
        public ActionResult Create(string clientId, string dealId, string backURL = "")
        {
            try
            {
                int? _clientId = null;
                if (!String.IsNullOrEmpty(clientId))
                {
                    _clientId = ValidationUtils.TryGetInt(clientId);
                }

                int? _dealId = null;
                if (!String.IsNullOrEmpty(dealId))
                {
                    _dealId = ValidationUtils.TryGetInt(dealId);
                }

                var model = returnFromClientWaybillPresenter.Create(_clientId, _dealId, backURL, UserSession.CurrentUserInfo);

                return View("Edit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(ReturnFromClientWaybillEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Content(returnFromClientWaybillPresenter.Save(model, UserSession.CurrentUserInfo).ToString());
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

                return View(returnFromClientWaybillPresenter.Edit(waybillId, backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult GetStorageListForAccountOrganization(string accountOrganizationId)
        {
            try
            {
                var id = Utils.ValidationUtils.TryGetInt(accountOrganizationId);
                var organizationList = returnFromClientWaybillPresenter.GetStorageListForAccountOrganization(id, UserSession.CurrentUserInfo);

                return Json(organizationList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение списка основание для возврата
        /// </summary>
        [HttpGet]
        public ActionResult GetReturnFromClientReasonList()
        {
            var model = returnFromClientWaybillPresenter.GetReturnFromClientReasonList(UserSession.CurrentUserInfo);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Получение списка команд для возврата товара по сделке
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <returns></returns>
        public ActionResult GetTeamList(string dealId)
        {
            try
            {
                return Json(returnFromClientWaybillPresenter.GetTeamListForReturnFromClientWaybill(ValidationUtils.TryGetInt(dealId), UserSession.CurrentUserInfo),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #region Удаление

        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(id);

                returnFromClientWaybillPresenter.Delete(waybillId, UserSession.CurrentUserInfo);

                TempData["Message"] = "Накладная удалена.";

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Детали накладной

        [HttpGet]
        public ActionResult Details(string id, string backURL)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(id);

                return View(returnFromClientWaybillPresenter.Details(waybillId, backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #region Список позиций

        [HttpPost]
        public ActionResult ShowReturnFromClientWaybillRowGrid(GridState state)
        {
            try
            {
                return PartialView("ReturnFromClientWaybillRowGrid", returnFromClientWaybillPresenter.GetReturnFromClientWaybillRowGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowReturnFromClientWaybillArticleGroupGrid(GridState state)
        {
            try
            {
                return PartialView("ReturnFromClientWaybillArticleGroupGrid", returnFromClientWaybillPresenter.GetReturnFromClientWaybillArticleGroupGrid(state, UserSession.CurrentUserInfo));
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
                return PartialView("ReturnFromClientWaybillDocumentGrid", returnFromClientWaybillPresenter.GetDocumentGrid(state));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion                

        #region Добавление / редактирование позиций

        [HttpGet]
        public ActionResult AddRow(string returnFromClientWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(returnFromClientWaybillId);

                return PartialView("ReturnFromClientWaybillRowEdit", returnFromClientWaybillPresenter.AddRow(waybillId, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult EditRow(string returnFromClientWaybillId, string returnFromClientWaybillRowId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(returnFromClientWaybillId);
                Guid rowId = ValidationUtils.TryGetGuid(returnFromClientWaybillRowId);

                return PartialView("ReturnFromClientWaybillRowEdit", returnFromClientWaybillPresenter.EditRow(waybillId, rowId, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveRow(ReturnFromClientWaybillRowEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(returnFromClientWaybillPresenter.SaveRow(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Удаление позиции

        [HttpPost]
        public ActionResult DeleteRow(string returnFromClientWaybillId, string returnFromClientWaybillRowId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(returnFromClientWaybillId);
                Guid rowId = ValidationUtils.TryGetGuid(returnFromClientWaybillRowId);

                return Json(returnFromClientWaybillPresenter.DeleteRow(waybillId, rowId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Подготовка / Отменить готовность к проводке

        [HttpPost]
        public ActionResult PrepareToAccept(string returnFromClientWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(returnFromClientWaybillId);

                return Json(returnFromClientWaybillPresenter.PrepareToAccept(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelReadinessToAccept(string returnFromClientWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(returnFromClientWaybillId);

                return Json(returnFromClientWaybillPresenter.CancelReadinessToAccept(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Проводка / отмена проводки

        [HttpPost]
        public ActionResult Accept(string returnFromClientWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(returnFromClientWaybillId);

                return Json(returnFromClientWaybillPresenter.Accept(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelAcceptance(string returnFromClientWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(returnFromClientWaybillId);

                return Json(returnFromClientWaybillPresenter.CancelAcceptance(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Приемка / отмена приемки

        [HttpPost]
        public ActionResult Receipt(string returnFromClientWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(returnFromClientWaybillId);

                return Json(returnFromClientWaybillPresenter.Receipt(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelReceipt(string returnFromClientWaybillId)
        {
            try
            {
                Guid waybillId = ValidationUtils.TryGetGuid(returnFromClientWaybillId);

                return Json(returnFromClientWaybillPresenter.CancelReceipt(waybillId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Печатные формы

        #region ТОРГ 12

        /// <summary>
        /// Получение окна параметров печатной формы
        /// </summary>
        public ActionResult ShowTORG12PrintingFormSettings(string waybillId)
        {
            try
            {
                var waybillGuid = ValidationUtils.TryGetGuid(waybillId);

                var model = returnFromClientWaybillPresenter.GetTORG12PrintingFormSettings(waybillGuid, UserSession.CurrentUserInfo);
                model.ActionUrl = "/ReturnFromClientWaybill/ShowTORG12PrintingForm";
                model.ExportToExcelUrl = "/ReturnFromClientWaybill/ExportTORG12PrintingFormToExcel";

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
                return ExcelFile(returnFromClientWaybillPresenter.ExportTORG12PrintingFormToExcel(settings, UserSession.CurrentUserInfo), "TORG12.xlsx");
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
                var model = returnFromClientWaybillPresenter.GetTORG12PrintingForm(settings, UserSession.CurrentUserInfo);
                model.RowsContentURL = "/ReturnFromClientWaybill/ShowTORG12PrintingFormRows/";

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
                var model = returnFromClientWaybillPresenter.GetTORG12PrintingFormRows(settings, UserSession.CurrentUserInfo);

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
                var model = returnFromClientWaybillPresenter.SelectWaybill(ValidationUtils.TryGetInt(articleId), UserSession.CurrentUserInfo);

                return PartialView("ReturnFromClientWaybillSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowReturnFromClientWaybillSelectGrid(GridState state)
        {
            try
            {
                GridData data = returnFromClientWaybillPresenter.GetWaybillSelectGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReturnFromClientWaybillSelectGrid", data);
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