using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class DealPaymentController : WholesaleController
    {
        #region Поля

        private readonly IDealPaymentPresenter dealPaymentPresenter;

        #endregion

        #region Конструкторы

        public DealPaymentController(IDealPaymentPresenter dealPaymentPresenter)
        {
            this.dealPaymentPresenter = dealPaymentPresenter;
        }

        #endregion

        #region Методы

        #region Список

        public ActionResult List()
        {
            try
            {
                return View("~/Views/DealPaymentDocument/DealPayment/List.aspx",
                    dealPaymentPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowDealPaymentGrid(GridState state)
        {
            try
            {
                return PartialView("~/Views/DealPaymentDocument/DealPayment/DealPaymentGrid.ascx",
                    dealPaymentPresenter.GetDealPaymentGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление оплаты

        /// <summary>
        /// Удаление оплаты от клиента
        /// </summary>
        /// <param name="paymentId">Код оплаты</param>
        [HttpPost]
        public ActionResult DeleteDealPaymentFromClient(string paymentId)
        {
            try
            {
                dealPaymentPresenter.DeleteDealPaymentFromClient(ValidationUtils.TryGetGuid(paymentId), UserSession.CurrentUserInfo);

                return Content("");
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
                dealPaymentPresenter.DeleteDealPaymentToClient(ValidationUtils.TryGetGuid(paymentId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Оплаты от клиента

        /// <summary>
        /// Создание оплаты от клиента по сделке
        /// </summary>
        public ActionResult CreateDealPaymentFromClient()
        {
            try
            {
                var model = dealPaymentPresenter.CreateDealPaymentFromClient(UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealPayment/DealPaymentFromClientEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Сохранение оплаты от клиента по сделке
        /// </summary>
        [HttpPost]
        public ActionResult SaveDealPaymentFromClient(DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                dealPaymentPresenter.SaveDealPaymentFromClient(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Создание оплаты от клиента по организации
        /// </summary>
        public ActionResult CreateClientOrganizationPaymentFromClient()
        {
            try
            {
                var model = dealPaymentPresenter.CreateClientOrganizationPaymentFromClient(UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealPayment/ClientOrganizationPaymentFromClientEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Сохранение оплаты от клиента по организации
        /// </summary>
        /// <param name="model">Модель оплаты</param>
        [HttpPost]
        public ActionResult SaveClientOrganizationPaymentFromClient(DestinationDocumentSelectForClientOrganizationPaymentFromClientDistributionViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                dealPaymentPresenter.SaveClientOrganizationPaymentFromClient(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Возвращает модальную форму деталей оплаты (с информацией о разнесении)
        /// </summary>        
        /// <param name="paymentId">Код оплаты</param>
        [HttpGet]
        public ActionResult DealPaymentFromClientDetails(Guid? paymentId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(paymentId, "Неверное значение входного параметра.");

                var model = dealPaymentPresenter.DealPaymentFromClientDetails(paymentId.Value, UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealPayment/DealPaymentFromClientDetails.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Возвраты оплат клиенту

        /// <summary>
        /// Добавление новой оплаты
        /// </summary>        
        [HttpGet]
        public ActionResult CreateDealPaymentToClient()
        {
            try
            {
                var model = dealPaymentPresenter.CreateDealPaymentToClient(UserSession.CurrentUserInfo);

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
        [HttpPost]
        public ActionResult SaveDealPaymentToClient(DealPaymentToClientEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                dealPaymentPresenter.SaveDealPaymentToClient(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Возвращает модальную форму деталей возврата оплаты клиенту
        /// </summary>        
        /// <param name="paymentId">Код оплаты</param>
        [HttpGet]
        public ActionResult DealPaymentToClientDetails(Guid? paymentId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(paymentId, "Оплата не указана.");

                var model = dealPaymentPresenter.DealPaymentToClientDetails(paymentId.Value, UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealPayment/DealPaymentToClientDetails.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Смена пользователя, вернувшего оплату клиенту
        /// </summary>
        [HttpPost]
        public ActionResult ChangeReturnedByInPaymentToClient(string dealPaymentId, string newReturnedById)
        {
            try
            {
                var _dealPaymentId = ValidationUtils.TryGetGuid(dealPaymentId, "Оплата не указана.");
                var _newReturnedById = ValidationUtils.TryGetInt(newReturnedById, "Пользователь не указан.");

                dealPaymentPresenter.ChangeReturnedByInPaymentToClient(_dealPaymentId, _newReturnedById, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Смена пользователя, принявшего оплату клиенту
        /// </summary>
        [HttpPost]
        public ActionResult ChangeTakenByInPaymentFromClient(string dealPaymentId, string newTakenById)
        {
            try
            {
                var _dealPaymentId = ValidationUtils.TryGetGuid(dealPaymentId, "Оплата не указана.");
                var _newTakenById = ValidationUtils.TryGetInt(newTakenById, "Пользователь не указан.");

                dealPaymentPresenter.ChangeTakenByInPaymentFromClient(_dealPaymentId, _newTakenById, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion



        #region Модальная форма выбора документов для ручного разнесения платежных документов

        /// <summary>
        /// Модальная форма выбора документов для ручного разнесения оплаты от клиента по организации клиента
        /// </summary>
        /// <param name="model">Модель, заполненная на предыдущем этапе (в форме параметров оплаты при создании)</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SelectDestinationDocumentsForClientOrganizationPaymentFromClientDistribution(ClientOrganizationPaymentFromClientEditViewModel model)
        {
            try
            {
                return PartialView("~/Views/DealPaymentDocument/DestinationDocumentSelector/DestinationDocumentSelectorForClientOrganizationPaymentFromClientDistribution.ascx",
                    dealPaymentPresenter.SelectDestinationDocumentsForClientOrganizationPaymentFromClientDistribution(model, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Грид реализаций ручного разнесения оплаты от клиента по организации клиента
        /// </summary>
        /// <param name="clientOrganizationId">Код организации клиента</param>
        /// <param name="teamId">Код команды</param>
        /// <returns></returns>
        public ActionResult ShowDestinationSaleGridForClientOrganizationPaymentFromClientDistribution (string clientOrganizationId, string teamId)
        {
            try
            {
                return PartialView("~/Views/DealPaymentDocument/DestinationDocumentSelector/SaleWaybillSelectGrid.ascx",
                    dealPaymentPresenter.ShowDestinationSaleGridForClientOrganizationPaymentFromClientDistribution(
                    ValidationUtils.TryGetInt(clientOrganizationId), ValidationUtils.TryGetShort(teamId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Грид платежных документов для ручного разнесения оплаты от клиента по организации клиента
        /// </summary>
        /// <param name="clientOrganizationId">Код организации клиента</param>
        /// <param name="teamId">Код команды</param>
        /// <returns></returns>
        public ActionResult ShowDestinationPaymentDocumentGridForClientOrganizationPaymentFromClientDistribution(string clientOrganizationId, string teamId)
        {
            try
            {
                return PartialView("~/Views/DealPaymentDocument/DestinationDocumentSelector/DealDebitInitialBalanceCorrectionSelectGrid.ascx",
                    dealPaymentPresenter.ShowDestinationPaymentDocumentGridForClientOrganizationPaymentFromClientDistribution(
                    ValidationUtils.TryGetInt(clientOrganizationId), ValidationUtils.TryGetShort(teamId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Модальная форма выбора документов для ручного разнесения оплаты от клиента по сделке
        /// </summary>
        /// <param name="model">Модель, заполненная на предыдущем этапе (в форме параметров оплаты при создании)</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SelectDestinationDocumentsForDealPaymentFromClientDistribution(DealPaymentFromClientEditViewModel model)
        {
            try
            {
                return PartialView("~/Views/DealPaymentDocument/DestinationDocumentSelector/DestinationDocumentSelectorForDealPaymentFromClientDistribution.ascx",
                    dealPaymentPresenter.SelectDestinationDocumentsForDealPaymentFromClientDistribution(model, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение модели для грида реализаций при ручном разнесении оплаты
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        public ActionResult ShowDestinationSaleGridForDealPaymentFromClientDistribution(string dealId, string teamId)
        {
            try
            {
                return PartialView("~/Views/DealPaymentDocument/DestinationDocumentSelector/SaleWaybillSelectGrid.ascx",
                    dealPaymentPresenter.ShowDestinationSaleGridForDealPaymentFromClientDistribution(ValidationUtils.TryGetInt(dealId),
                    ValidationUtils.TryGetShort(teamId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение модели для грида платежных документов при ручном разнесении оплаты
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        public ActionResult ShowDestinationPaymentDocumentGridForDealPaymentFromClientDistribution(string dealId, string teamId)
        {
            try
            {
                return PartialView("~/Views/DealPaymentDocument/DestinationDocumentSelector/DealDebitInitialBalanceCorrectionSelectGrid.ascx",
                    dealPaymentPresenter.ShowDestinationPaymentDocumentGridForDealPaymentFromClientDistribution(ValidationUtils.TryGetInt(dealId),
                    ValidationUtils.TryGetShort(teamId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        
        /// <summary>
        /// Модальная форма выбора документов для ручного переразнесения оплаты от клиента по сделке
        /// </summary>
        /// <param name="dealPaymentFromClientId">Код оплаты от клиента по сделке</param>
        /// <param name="destinationDocumentSelectorControllerName">Название контроллера, который будет вызываться при submit формы разнесения</param>
        /// <param name="destinationDocumentSelectorActionName">Название метода контроллера, который будет вызываться при submit формы разнесения</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SelectDestinationDocumentsForDealPaymentFromClientRedistribution(string dealPaymentFromClientId,
            string destinationDocumentSelectorControllerName, string destinationDocumentSelectorActionName)
        {
            try
            {
                return PartialView("~/Views/DealPaymentDocument/DestinationDocumentSelector/DestinationDocumentSelectorForDealPaymentFromClientDistribution.ascx",
                    dealPaymentPresenter.SelectDestinationDocumentsForDealPaymentFromClientRedistribution(ValidationUtils.TryGetGuid(dealPaymentFromClientId),
                    destinationDocumentSelectorControllerName, destinationDocumentSelectorActionName, UserSession.CurrentUserInfo));
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
