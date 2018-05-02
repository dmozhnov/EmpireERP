using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ClientContract;
using ERP.Wholesale.UI.ViewModels.Deal;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class DealController : WholesaleController
    {
        #region Поля

        private readonly IDealPresenter dealPresenter;
        
        #endregion

        #region Конструкторы

        public DealController(IDealPresenter dealPresenter)
        {
            this.dealPresenter = dealPresenter;           
        }

        #endregion

        #region Список

        public ActionResult List()
        {
            try
            {
                var model = dealPresenter.List(UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowActiveDealGrid(GridState state)
        {
            try
            {
                var grid = dealPresenter.GetActiveDealGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ActiveDealGrid", grid);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowClosedDealGrid(GridState state)
        {
            try
            {
                var grid = dealPresenter.GetClosedDealGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ClosedDealGrid", grid);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Детали сделки

        #region Детали общие

        [HttpGet]
        public ActionResult Details(int? id = null, string backURL = "")
        {
            try
            {
                ValidationUtils.NotNull(id, "Неверное значение входного параметра.");

                var model = dealPresenter.Details(id.Value, backURL, UserSession.CurrentUserInfo);
                model.TaskGrid.GridPartialViewAction = "/Deal/ShowTaskGrid/";
                model.TaskGrid.HelpContentUrl = "/Help/GetHelp_Deal_Details_TaskGrid";

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Гриды

        #region Квоты по сделке

        [HttpPost]
        public ActionResult ShowQuotaGrid(GridState state)
        {
            try
            {
                var model = dealPresenter.GetQuotaGrid(state, UserSession.CurrentUserInfo);

                return PartialView("DealQuotaGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Оплаты по сделке

        [HttpPost]
        public ActionResult ShowDealPaymentGrid(GridState state)
        {
            try
            {
                GridData data = dealPresenter.GetDealPaymentGrid(state, UserSession.CurrentUserInfo);

                return PartialView("DealPaymentGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Реализации по сделке

        [HttpPost]
        public ActionResult ShowSalesGrid(GridState state)
        {
            try
            {                
                var model = dealPresenter.GetSalesGrid(state, UserSession.CurrentUserInfo);

                return PartialView("DealSalesGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Возвраты по сделке

        public ActionResult ShowReturnFromClientGrid(GridState state)
        {
            try
            {
                ValidationUtils.NotNull(state, "Неверное значение входного параметра.");
                var model = dealPresenter.GetReturnFromClientGrid(state, UserSession.CurrentUserInfo);

                return PartialView("DealReturnFromClientGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Корректировки сальдо

        #region Грид

        [HttpPost]
        public ActionResult ShowDealInitialBalanceCorrectionGrid(GridState state)
        {
            try
            {
                var model = dealPresenter.GetDealInitialBalanceCorrectionGrid(state, UserSession.CurrentUserInfo);

                return PartialView("DealInitialBalanceCorrectionGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Создание

        [HttpGet]
        public ActionResult CreateDealCreditInitialBalanceCorrection(string dealId)
        {
            try
            {
                var model = dealPresenter.CreateDealCreditInitialBalanceCorrection(ValidationUtils.TryGetInt(dealId), UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealInitialBalanceCorrection/DealCreditInitialBalanceCorrectionEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult CreateDealDebitInitialBalanceCorrection(string dealId)
        {
            try
            {
                var model = dealPresenter.CreateDealDebitInitialBalanceCorrection(ValidationUtils.TryGetInt(dealId), UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealInitialBalanceCorrection/DealDebitInitialBalanceCorrectionEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Задачи

        [HttpPost]
        public ActionResult ShowTaskGrid(GridState state)
        {
            try
            {
                var model = dealPresenter.GetTaskGrid(state, UserSession.CurrentUserInfo);
                model.GridPartialViewAction = "/Deal/ShowTaskGrid/";

                return PartialView("~/Views/Task/NewTaskGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #endregion

        #region Добавление / редактирование сделки

        [HttpGet]
        public ActionResult Create(string backURL, string clientId)
        {
            try
            {
                int? id = null;
                if (!String.IsNullOrEmpty(clientId))
                {
                    id = ValidationUtils.TryGetInt(clientId);
                }

                return View("Edit", dealPresenter.Create(id, backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id, string backURL)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(id, "Неверное значение входного параметра.");

                var model = dealPresenter.Edit(id.Value, backURL, UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Edit(DealEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Content(dealPresenter.Save(model, UserSession.CurrentUserInfo).ToString());
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Изменение этапа сделки

        [HttpGet]
        public ActionResult ChangeStage(string dealId)
        {
            try
            {
                var model = dealPresenter.ChangeStage(ValidationUtils.TryGetInt(dealId), UserSession.CurrentUserInfo);

                return PartialView("DealChangeStage", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult MoveToNextStage(string dealId, string currentStageId)
        {
            try
            {
                var result = dealPresenter.MoveToNextStage(ValidationUtils.TryGetInt(dealId), ValidationUtils.TryGetByte(currentStageId), UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult MoveToPreviousStage(string dealId, string currentStageId)
        {
            try
            {
                var result = dealPresenter.MoveToPreviousStage(ValidationUtils.TryGetInt(dealId), ValidationUtils.TryGetByte(currentStageId), UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult MoveToUnsuccessfulClosingStage(string dealId, string currentStageId)
        {
            try
            {
                var result = dealPresenter.MoveToUnsuccessfulClosingStage(ValidationUtils.TryGetInt(dealId), ValidationUtils.TryGetByte(currentStageId), UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult MoveToDecisionMakerSearchStage(string dealId, string currentStageId)
        {
            try
            {
                var result = dealPresenter.MoveToDecisionMakerSearchStage(ValidationUtils.TryGetInt(dealId), ValidationUtils.TryGetByte(currentStageId), UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
        
        #region Добавление / Удаление квоты

        [HttpPost]
        public ActionResult RemoveQuota(string dealId, string quotaId)
        {
            try
            {
                var result = dealPresenter.RemoveQuota(ValidationUtils.TryGetInt(dealId), ValidationUtils.TryGetInt(quotaId), UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Добавление квоты
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        public ActionResult AddQuota(string dealId, string dealQuotaId)
        {
            try
            {
                var result = dealPresenter.AddQuota(ValidationUtils.TryGetInt(dealId), ValidationUtils.TryGetInt(dealQuotaId), UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Добавление всех квот
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        public ActionResult AddAllQuotas(string dealId)
        {
            try
            {
                var result = dealPresenter.AddAllQuotas(ValidationUtils.TryGetInt(dealId), UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / редактирование договора

        /// <summary>
        /// Создание договора
        /// </summary>
        /// <param name="dealId"></param>
        [HttpGet]
        public ActionResult CreateContract(string dealId)
        {
            try
            {
                ValidationUtils.NotNull(dealId, "Неверное значение входного параметра.");

                var model = dealPresenter.CreateContract(ValidationUtils.TryGetInt(dealId), UserSession.CurrentUserInfo);

                return PartialView("~/Views/ClientContract/ClientContractEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Создание / редактирование договора
        /// </summary>
        [HttpPost]
        public ActionResult EditContract(ClientContractEditViewModel model)
        {
            try
            {
                var result = dealPresenter.SaveContract(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SetContract(string dealId, string clientContractId)
        {
            try
            {
                dealPresenter.SetContract(ValidationUtils.TryGetInt(dealId), ValidationUtils.TryGetShort(clientContractId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Проверка возможности установить (или сменить) договор.
        /// </summary>
        /// <param name="dealId">Идентификатор сделки.</param>
        [HttpGet]
        public ActionResult CheckPossibilityToSetContract(string dealId)
        {
            try
            {
                dealPresenter.CheckPossibilityToSetContract(ValidationUtils.TryGetInt(dealId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / удаление оплаты

        #region Оплаты от клиента
        
        /// <summary>
        /// Добавление новой оплаты
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        [HttpGet]
        public ActionResult CreateDealPaymentFromClient(int? dealId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(dealId, "Неверное значение входного параметра.");

                var model = dealPresenter.CreateDealPaymentFromClient(dealId.Value, UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealPayment/DealPaymentFromClientEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Сохранение оплаты
        /// </summary>
        /// <param name="model">Модель оплаты</param>
        [HttpPost]
        public ActionResult SaveDealPaymentFromClient(DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                var result = dealPresenter.SaveDealPaymentFromClient(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Удаление оплаты от клиента
        /// </summary>
        /// <param name="paymentId">Код оплаты</param>
        [HttpPost]
        public ActionResult DeleteDealPaymentFromClient(string paymentId)
        {
            try
            {
                return Json(dealPresenter.DeleteDealPaymentFromClient(ValidationUtils.TryGetGuid(paymentId), UserSession.CurrentUserInfo),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Возврат оплаты клиенту

        /// <summary>
        /// Добавление новой оплаты
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        [HttpGet]
        public ActionResult CreateDealPaymentToClient(int? dealId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(dealId, "Неверное значение входного параметра.");

                var model = dealPresenter.CreateDealPaymentToClient(dealId.Value, UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealPayment/DealPaymentToClientEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Сохранение оплаты
        /// </summary>
        /// <param name="model">Модель оплаты</param>
        [HttpPost]
        public ActionResult SaveDealPaymentToClient(DealPaymentToClientEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                var result = dealPresenter.SaveDealPaymentToClient(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Удаление возврата оплаты клиенту
        /// </summary>
        /// <param name="paymentId">Код оплаты</param>
        [HttpPost]
        public ActionResult DeleteDealPaymentToClient(string paymentId)
        {
            try
            {
                return Json(dealPresenter.DeleteDealPaymentToClient(ValidationUtils.TryGetGuid(paymentId), UserSession.CurrentUserInfo),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
        
        #endregion

        #region Корректировки сальдо

        #region Сохранение

        [HttpPost]
        public ActionResult SaveDealCreditInitialBalanceCorrection(DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(dealPresenter.SaveDealCreditInitialBalanceCorrection(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveDealDebitInitialBalanceCorrection(DealDebitInitialBalanceCorrectionEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(dealPresenter.SaveDealDebitInitialBalanceCorrection(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление

        /// <summary>
        /// Удаление кредитовой корректировки
        /// </summary>
        /// <param name="correctionId">Код корректировки</param>
        [HttpPost]
        public ActionResult DeleteDealCreditInitialBalanceCorrection(string correctionId)
        {
            try
            {
                return Json(dealPresenter.DeleteDealCreditInitialBalanceCorrection(ValidationUtils.TryGetGuid(correctionId), UserSession.CurrentUserInfo),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Удаление дебетовой корректировки
        /// </summary>
        /// <param name="correctionId">Код корректировки</param>
        [HttpPost]
        public ActionResult DeleteDealDebitInitialBalanceCorrection(string correctionId)
        {
            try
            {
                return Json(dealPresenter.DeleteDealDebitInitialBalanceCorrection(ValidationUtils.TryGetGuid(correctionId), UserSession.CurrentUserInfo),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Выбор сделки

        public ActionResult SelectDealByClient(string clientId, string mode = "")
        {
            try
            {
                return PartialView("DealSelector", dealPresenter.SelectDealByClient(ValidationUtils.TryGetInt(clientId), mode, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult SelectDealByTeam(string teamId)
        {
            try
            {
                return PartialView("DealSelector", dealPresenter.SelectDealByTeam(ValidationUtils.TryGetShort(teamId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult SelectDeal(bool? activeOnly)
        {
            try
            {
                var active = activeOnly == null || activeOnly == true;

                return PartialView("DealSelector", dealPresenter.SelectDeal(active, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowDealSelectGrid(GridState state)
        {
            try
            {
                return PartialView("DealSelectGrid", dealPresenter.GetSelectDealGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult SelectDealByClientOrganization(string clientOrganizationId, string mode = "")
        {
            try
            {
                return PartialView("DealSelector", dealPresenter.SelectDealByClientOrganization(
                    ValidationUtils.TryGetInt(clientOrganizationId, "Не указана организация клиента."),
                    mode, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult GetDealInfo(string dealId)
        {
            try
            {
                var id = ValidationUtils.TryGetInt(dealId);
                var obj = dealPresenter.GetDealInfo(id, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion        

        #region Получение списка команд для документов сделки

        /// <summary>
        /// Получение списка команд для документов сделки
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <returns></returns>
        public ActionResult GetTeamListForDealDocument(string dealId)
        {
            try
            {
                return Json(dealPresenter.GetTeamListForDealDocument(ValidationUtils.TryGetInt(dealId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
    }
}
