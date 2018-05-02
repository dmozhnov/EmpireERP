using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Client;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ClientController : WholesaleController
    {
        #region Поля

        private readonly IClientPresenter clientPresenter;

        #endregion

        #region Конструкторы

        public ClientController(IClientPresenter clientPresenter)
        {
            this.clientPresenter = clientPresenter;
        }

        #endregion

        #region Список

        public ActionResult List()
        {
            try
            {
                var model = clientPresenter.List(UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowClientGrid(GridState state)
        {
            try
            {
                var data = clientPresenter.GetClientGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ClientGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / Редактирование клиента

        [HttpGet]
        public ActionResult Create(string backURL)
        {
            try
            {
                var model = clientPresenter.Create(backURL, UserSession.CurrentUserInfo);

                return View("Edit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult Edit(int id, string backURL)
        {
            try
            {
                var model = clientPresenter.Edit(id, backURL, UserSession.CurrentUserInfo);

                return View("Edit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(ClientEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                var result = clientPresenter.Save(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение списка регионов
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetClientRegionList()
        {
            var model = clientPresenter.GetClientRegionList(UserSession.CurrentUserInfo);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Получение списка программ обслуживания
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetClientServiceProgramList()
        {
            var model = clientPresenter.GetClientServiceProgramList(UserSession.CurrentUserInfo);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Получение списка типов клиентов
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetClientTypeList()
        {
            var model = clientPresenter.GetClientTypeList(UserSession.CurrentUserInfo);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Детали клиента

        #region Детали общие

        /// <summary>
        /// Детали клиента
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="backURL">Обратный адрес</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Details(int? id, string backURL)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(id, "Неверное значение входного параметра.");

                var model = clientPresenter.Details(id.Value, backURL, UserSession.CurrentUserInfo);
                model.TaskGrid.GridPartialViewAction = "/Client/ShowTaskGrid/";
                model.TaskGrid.HelpContentUrl = "/Help/GetHelp_Client_Details_TaskGrid";

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Сделки

        #region Список сделок

        [HttpPost]
        public ActionResult ShowDealGrid(GridState state)
        {
            try
            {
                GridData data = clientPresenter.GetDealGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ClientDealGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Организации клиента

        #region Грид организаций клиента в деталях клиента

        [HttpPost]
        public ActionResult ShowOrganizationGrid(GridState state)
        {
            try
            {
                var model = clientPresenter.GetOrganizationGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ClientOrganizationGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Выбор существующей организации клиента

        /// <summary>
        /// Возвращает модальную форму для выбора организации клиента
        /// Имеет 2 режима:
        /// - (mode == "excludeclient") выбрать организацию клиента из списка существующих, но не входящих в организации конкретного клиента
        /// - (mode == "includeclient") выбрать организацию клиента только среди организаций данного клиента
        /// </summary>
        [HttpGet]
        public ActionResult SelectClientOrganization(int? clientId, string mode)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(clientId, "Неверное значение входного параметра.");

                if (String.IsNullOrWhiteSpace(mode))
                {
                    throw new Exception("Неверное значение входного параметра.");
                }

                var model = clientPresenter.SelectClientOrganization(clientId.Value, mode, UserSession.CurrentUserInfo);

                return PartialView("~/Views/ContractorOrganization/ContractorOrganizationSelector.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Выбор организации клиента из списка всех имеющихся
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectAllClientOrganization()
        {
            try
            {
                var model = clientPresenter.SelectClientOrganization(null, null, UserSession.CurrentUserInfo);

                return PartialView("~/Views/ContractorOrganization/ContractorOrganizationSelector.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение грида доступных организаций контрагента
        /// </summary>
        /// <param name="state">состояние грида</param>
        [HttpPost]
        public ActionResult ShowClientOrganizationSelectGrid(GridState state)
        {
            try
            {
                GridData data = clientPresenter.GetClientOrganizationSelectGrid(state, UserSession.CurrentUserInfo);

                return PartialView("~/Views/ContractorOrganization/ContractorOrganizationSelectGrid.ascx", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Добавить организацию контрагента с данным Id к клиенту.
        /// </summary>
        /// <param name="clientId">код клиента</param>
        /// <param name="organizationId">код организации</param>
        [HttpPost]
        public ActionResult AddClientOrganization(int? clientId, int? organizationId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(clientId, "Неверное значение входного параметра.");
                ValidationUtils.NotNullOrDefault(organizationId, "Неверное значение входного параметра.");

                clientPresenter.AddClientOrganization(clientId.Value, organizationId.Value, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Модальная форма для создания новой организации

        /// <summary>
        /// Возвращает модальную форму для создания новой организации
        /// </summary>
        [HttpGet]
        public ActionResult CreateContractorOrganization(int? contractorId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(contractorId, "Неверное значение входного параметра.");

                var model = clientPresenter.CreateContractorOrganization(contractorId.Value);

                return PartialView("~/Views/EconomicAgent/EconomicAgentTypeSelector.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Создание организации клиента
        /// </summary>
        [HttpPost]
        public ActionResult EditJuridicalPerson(JuridicalPersonEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(model.LegalFormId, "Не задана правовая форма.");
                ValidationUtils.NotNullOrDefault(model.ContractorId, "Неверно указан контрагент.");

                var result = clientPresenter.SaveJuridicalPerson(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Создание организации клиента
        /// </summary>
        [HttpPost]
        public ActionResult EditPhysicalPerson(PhysicalPersonEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(model.LegalFormId, "Не задана правовая форма.");
                ValidationUtils.NotNullOrDefault(model.ContractorId, "Неверно указан контрагент.");

                var result = clientPresenter.SavePhysicalPerson(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Реализации

        [HttpPost]
        public ActionResult ShowSalesGrid(GridState state)
        {
            try
            {
                var model = clientPresenter.GetSalesGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ClientSalesGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Оплаты

        [HttpPost]
        public ActionResult ShowDealPaymentGrid(GridState state)
        {
            try
            {
                var model = clientPresenter.GetDealPaymentGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ClientPaymentGrid", model);
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
                var model = clientPresenter.GetInitialBalanceCorrectionGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ClientDealInitialBalanceCorrectionGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Создание

        [HttpGet]
        public ActionResult CreateDealCreditInitialBalanceCorrection(string clientId)
        {
            try
            {
                var model = clientPresenter.CreateDealCreditInitialBalanceCorrection(ValidationUtils.TryGetInt(clientId), UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealInitialBalanceCorrection/DealCreditInitialBalanceCorrectionEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult CreateDealDebitInitialBalanceCorrection(string clientId)
        {
            try
            {
                var model = clientPresenter.CreateDealDebitInitialBalanceCorrection(ValidationUtils.TryGetInt(clientId), UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealInitialBalanceCorrection/DealDebitInitialBalanceCorrectionEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion        

        #region Сохранение

        [HttpPost]
        public ActionResult SaveDealCreditInitialBalanceCorrection(DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(clientPresenter.SaveDealCreditInitialBalanceCorrection(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
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

                return Json(clientPresenter.SaveDealDebitInitialBalanceCorrection(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
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
                return Json(clientPresenter.DeleteDealCreditInitialBalanceCorrection(ValidationUtils.TryGetGuid(correctionId), UserSession.CurrentUserInfo),
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
                return Json(clientPresenter.DeleteDealDebitInitialBalanceCorrection(ValidationUtils.TryGetGuid(correctionId), UserSession.CurrentUserInfo),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Возвраты

        public ActionResult ShowReturnFromClientGrid(GridState state)
        {
            try
            {
                ValidationUtils.NotNull(state, "Неверное значение входного параметра.");
                var model = clientPresenter.GetReturnFromClientGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ReturnFromClientGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Задачи

        [HttpPost]
        public ActionResult ShowTaskGrid(GridState state)
        {
            try
            {
                var model = clientPresenter.GetTaskGrid(state, UserSession.CurrentUserInfo);
                model.GridPartialViewAction = "/Client/ShowTaskGrid/";

                return PartialView("~/Views/Task/NewTaskGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Блокировка клиента

        /// <summary>
        /// Установить новое значение признака ручной блокировки клиента
        /// </summary>
        /// <param name="id">Код клиента</param>
        /// <param name="isBlockedManually">Новое значение признака блокировки (1 - заблокирован, 0 - нет)</param>
        [HttpPost]
        public ActionResult SetClientBlockingValue(string id, string isBlockedManually)
        {
            try
            {
                byte blockingValue = ValidationUtils.TryGetByte(isBlockedManually);

                var clientId = ValidationUtils.TryGetInt(id);

                clientPresenter.SetClientBlockingValue(clientId, blockingValue, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление клиента

        /// <summary>
        /// Удаление клиента
        /// </summary>
        /// <param name="clientId">Код клиента</param>
        [HttpPost]
        public ActionResult Delete(int? clientId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(clientId, "Неверное значение входного параметра.");

                clientPresenter.Delete(clientId.Value, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление организации клиента

        /// <summary>
        /// Удаление организации клиента из списка
        /// </summary>
        /// <param name="providerId">Код клиента</param>
        /// <param name="providerOrganizationId">Код организации клиента (ClientOrganization)</param>
        [HttpPost]
        public ActionResult RemoveClientOrganization(int? clientId, int? clientOrganizationId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(clientId, "Неверное значение входного параметра.");
                ValidationUtils.NotNullOrDefault(clientOrganizationId, "Неверное значение входного параметра.");

                clientPresenter.RemoveClientOrganization(clientId.Value, clientOrganizationId.Value, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Оплаты от клиента

        public ActionResult CreateDealPaymentFromClient(string clientId)
        {
            try
            {
                var id = ValidationUtils.TryGetInt(clientId);

                var model = clientPresenter.CreateDealPaymentFromClient(id, UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealPayment/DealPaymentFromClientEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveDealPaymentFromClient(DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(clientPresenter.SaveDealPaymentFromClient(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
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
                return Json(clientPresenter.DeleteDealPaymentFromClient(ValidationUtils.TryGetGuid(paymentId), UserSession.CurrentUserInfo),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Возврат оплат клиенту

        public ActionResult CreateDealPaymentToClient(string clientId)
        {
            try
            {
                var id = ValidationUtils.TryGetInt(clientId);

                var model = clientPresenter.CreateDealPaymentToClient(id, UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealPayment/DealPaymentToClientEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveDealPaymentToClient(DealPaymentToClientEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(clientPresenter.SaveDealPaymentToClient(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
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
                return Json(clientPresenter.DeleteDealPaymentToClient(ValidationUtils.TryGetGuid(paymentId), UserSession.CurrentUserInfo),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Выбор клиента

        public ActionResult SelectClient()
        {
            try
            {
                var model = clientPresenter.SelectClient(UserSession.CurrentUserInfo);

                return PartialView("ClientSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowClientSelectGrid(GridState state)
        {
            try
            {
                return PartialView("ClientSelectGrid", clientPresenter.GetSelectClientGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult GetFactualAddress(string clientId)
        {
            try
            {
                var id = ValidationUtils.TryGetInt(clientId);
                var obj = clientPresenter.GetFactualAddress(id, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
    }
}