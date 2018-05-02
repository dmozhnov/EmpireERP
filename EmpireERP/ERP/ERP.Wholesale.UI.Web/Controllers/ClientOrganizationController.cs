using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;
using ERP.Wholesale.UI.ViewModels.Organization;
using ERP.Wholesale.UI.Web.Infrastructure;


namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ClientOrganizationController : WholesaleController
    {
        #region Поля

        private readonly IClientOrganizationPresenter clientOrganizationPresenter;

        #endregion

        #region Конструкторы

        public ClientOrganizationController(IClientOrganizationPresenter clientOrganizationPresenter)            
        {
            this.clientOrganizationPresenter = clientOrganizationPresenter;
        }

        #endregion

        #region Детали

        #region Детали общие

        [HttpGet]
        public ActionResult Details(int? id, string backURL)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(id, "Неверное значение входного параметра.");

                var model = clientOrganizationPresenter.Details(id.Value, Server.UrlEncode(backURL), UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult ShowMainDetails(int? organizationId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(organizationId, "Неверное значение входного параметра.");

                var model = clientOrganizationPresenter.GetMainDetails(organizationId.Value, UserSession.CurrentUserInfo);

                return PartialView("ClientOrganizationMainDetails", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление организации

        /// <summary>
        /// Удаление организации клиента
        /// </summary>
        /// <param name="clientOrganizationId">Код организации клиента</param>
        [HttpPost]
        public ActionResult Delete(int? clientOrganizationId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(clientOrganizationId, "Неверное значение входного параметра.");

                clientOrganizationPresenter.Delete(clientOrganizationId.Value, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Грид расчетных счетов

        [HttpPost]
        public ActionResult ShowRussianBankAccountGrid(GridState state)
        {
            try
            {
                var model = clientOrganizationPresenter.GetRussianBankAccountGrid(state, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/RussianBankAccountGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowForeignBankAccountGrid(GridState state)
        {
            try
            {
                var model = clientOrganizationPresenter.GetForeignBankAccountGrid(state, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/ForeignBankAccountGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Работа с расчетным счетом

        /// <summary>
        /// Создание нового счета
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddRussianBankAccount(int organizationId)
        {
            try
            {
                var model = clientOrganizationPresenter.AddRussianBankAccount(organizationId, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/RussianBankAccountEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Создание нового счета в иностранном банке
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddForeignBankAccount(int organizationId)
        {
            try
            {
                var model = clientOrganizationPresenter.AddForeignBankAccount(organizationId, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/ForeignBankAccountEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }            
        }

        /// <summary>
        /// Редактирование счета
        /// </summary>
        [HttpGet]
        public ActionResult EditRussianBankAccount(int? organizationId, int? bankAccountId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(organizationId, "Неверное значение входного параметра.");
                ValidationUtils.NotNullOrDefault(bankAccountId, "Неверное значение входного параметра.");

                var model = clientOrganizationPresenter.EditRussianBankAccount(organizationId.Value, bankAccountId.Value, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/RussianBankAccountEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование счета в иностранном банке
        /// </summary>
        [HttpGet]
        public ActionResult EditForeignBankAccount(int? organizationId, int? bankAccountId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(organizationId, "Неверное значение входного параметра.");
                ValidationUtils.NotNullOrDefault(bankAccountId, "Неверное значение входного параметра.");

                var model = clientOrganizationPresenter.EditForeignBankAccount(organizationId.Value, bankAccountId.Value, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/ForeignBankAccountEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование расчетного счета
        /// </summary>        
        [HttpPost]
        public ActionResult EditRussianBankAccount(RussianBankAccountEditViewModel model)
        {
            try
            {
                clientOrganizationPresenter.SaveRussianBankAccount(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование расчетного счета
        /// </summary>        
        [HttpPost]
        public ActionResult EditForeignBankAccount(ForeignBankAccountEditViewModel model)
        {
            try
            {
                clientOrganizationPresenter.SaveForeignBankAccount(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Удаление расчетного счета (TODO добавить проверки!!!)
        /// </summary>
        /// <param name="organizationId">Код организации клиента</param>
        /// <param name="bankAccountId">Код расчетного счета</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveRussianBankAccount(int? organizationId, int? bankAccountId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(organizationId, "Неверное значение входного параметра.");
                ValidationUtils.NotNullOrDefault(bankAccountId, "Неверное значение входного параметра.");

                clientOrganizationPresenter.RemoveRussianBankAccount(organizationId.Value, bankAccountId.Value, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Удаление расчетного счета в иностранном банке
        /// </summary>
        /// <param name="organizationId">Код организации клиента</param>
        /// <param name="bankAccountId">Код расчетного счета</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveForeignBankAccount(int? organizationId, int? bankAccountId)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(organizationId, "Неверное значение входного параметра.");
                ValidationUtils.NotNullOrDefault(bankAccountId, "Неверное значение входного параметра.");

                clientOrganizationPresenter.RemoveForeignBankAccount(organizationId.Value, bankAccountId.Value, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Грид реализаций

        [HttpPost]
        public ActionResult ShowSalesGrid(GridState state)
        {
            try
            {
                var model = clientOrganizationPresenter.GetSalesGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ClientOrganizationSalesGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Грид оплат

        [HttpPost]
        public ActionResult ShowDealPaymentGrid(GridState state)
        {
            try
            {
                var model = clientOrganizationPresenter.GetDealPaymentGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ClientOrganizationPaymentGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion        

        #region Грид договоров

        [HttpPost]
        public ActionResult ShowClientContractGrid(GridState state)
        {
            try
            {
                var model = clientOrganizationPresenter.GetClientContractGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ClientOrganizationClientContractGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion  

        #endregion

        #region Работа с оплатой

        #region Оплаты от клиентов
        
        /// <summary>
        /// Добавление новой оплаты
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        [HttpGet]
        public ActionResult CreateClientOrganizationPaymentFromClient(string clientOrganizationId)
        {
            try
            {
                int id = ValidationUtils.TryGetInt(clientOrganizationId);

                var model = clientOrganizationPresenter.CreateClientOrganizationPaymentFromClient(id, UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealPayment/ClientOrganizationPaymentFromClientEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Сохранение оплаты от клиента по организации. Используется для создания оплаты
        /// </summary>
        /// <param name="model">Модель оплаты</param>
        [HttpPost]
        public ActionResult SaveClientOrganizationPaymentFromClient(DestinationDocumentSelectForClientOrganizationPaymentFromClientDistributionViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                clientOrganizationPresenter.SaveClientOrganizationPaymentFromClient(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Сохранение оплаты от клиента по сделке. Используется для переразнесения оплаты
        /// </summary>
        /// <param name="model">Модель оплаты</param>
        [HttpPost]
        public ActionResult SaveDealPaymentFromClient(DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                clientOrganizationPresenter.SaveDealPaymentFromClient(model, UserSession.CurrentUserInfo);

                return Content("");
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
                clientOrganizationPresenter.DeleteDealPaymentFromClient(ValidationUtils.TryGetGuid(paymentId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Возвраты оплат клиенту

        /// <summary>
        /// Добавление нового возврата оплаты
        /// </summary>        
        [HttpGet]
        public ActionResult CreateDealPaymentToClient(string clientOrganizationId)
        {
            try
            {
                int id = ValidationUtils.TryGetInt(clientOrganizationId, "Не указана организация клиента.");

                var model = clientOrganizationPresenter.CreateDealPaymentToClient(id, UserSession.CurrentUserInfo);

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

                clientOrganizationPresenter.SaveDealPaymentToClient(model, UserSession.CurrentUserInfo);

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
                clientOrganizationPresenter.DeleteDealPaymentToClient(ValidationUtils.TryGetGuid(paymentId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Корректировки сальдо

        #region Грид
        
        [HttpPost]
        public ActionResult ShowDealInitialBalanceCorrectionGrid(GridState state)
        {
            try
            {
                var model = clientOrganizationPresenter.GetInitialBalanceCorrectionGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ClientOrganizationDealInitialBalanceCorrectionGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Создание

        [HttpGet]
        public ActionResult CreateDealCreditInitialBalanceCorrection(string clientOrganizationId)
        {
            try
            {
                int id = ValidationUtils.TryGetInt(clientOrganizationId, "Не указана организация клиента.");

                var model = clientOrganizationPresenter.CreateDealCreditInitialBalanceCorrection(id, UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealInitialBalanceCorrection/DealCreditInitialBalanceCorrectionEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult CreateDealDebitInitialBalanceCorrection(string clientOrganizationId)
        {
            try
            {
                int id = ValidationUtils.TryGetInt(clientOrganizationId, "Не указана организация клиента.");

                var model = clientOrganizationPresenter.CreateDealDebitInitialBalanceCorrection(id, UserSession.CurrentUserInfo);

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

                clientOrganizationPresenter.SaveDealCreditInitialBalanceCorrection(model, UserSession.CurrentUserInfo);

                return Content("");
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

                clientOrganizationPresenter.SaveDealDebitInitialBalanceCorrection(model, UserSession.CurrentUserInfo);

                return Content("");
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
                clientOrganizationPresenter.DeleteDealCreditInitialBalanceCorrection(ValidationUtils.TryGetGuid(correctionId), UserSession.CurrentUserInfo);

                return Content("");
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
                clientOrganizationPresenter.DeleteDealDebitInitialBalanceCorrection(ValidationUtils.TryGetGuid(correctionId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Редактирование организации

        /// <summary>
        /// Редактирование деталей организации
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int? organizationId = null)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(organizationId, "Неверное значение входного параметра.");

                var model = clientOrganizationPresenter.Edit(organizationId.Value, UserSession.CurrentUserInfo);

                if (model is JuridicalPersonEditViewModel)
                {
                    return PartialView("~/Views/EconomicAgent/JuridicalPersonEdit.ascx", model);
                }
                else
                {                   
                    return PartialView("~/Views/EconomicAgent/PhysicalPersonEdit.ascx", model);
                }
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование деталей организации
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditJuridicalPerson(JuridicalPersonEditViewModel model)
        {
            try
            {
                clientOrganizationPresenter.SaveJuridicalPerson(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование деталей организации
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditPhysicalPerson(PhysicalPersonEditViewModel model)
        {
            try
            {
                clientOrganizationPresenter.SavePhysicalPerson(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
    }
}
