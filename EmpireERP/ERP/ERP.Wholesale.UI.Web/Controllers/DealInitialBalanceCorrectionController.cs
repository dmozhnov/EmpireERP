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
    public class DealInitialBalanceCorrectionController : WholesaleController
    {
        #region Поля

        private readonly IDealInitialBalanceCorrectionPresenter dealInitialBalanceCorrectionPresenter;

        #endregion

        #region Конструкторы

        public DealInitialBalanceCorrectionController(IDealInitialBalanceCorrectionPresenter dealInitialBalanceCorrectionPresenter)
        {
            this.dealInitialBalanceCorrectionPresenter = dealInitialBalanceCorrectionPresenter;
        }

        #endregion

        #region Методы

        #region Список

        public ActionResult List()
        {
            try
            {
                return View("~/Views/DealPaymentDocument/DealInitialBalanceCorrection/List.aspx",
                    dealInitialBalanceCorrectionPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowDealInitialBalanceCorrectionGrid(GridState state)
        {
            try
            {
                return PartialView("~/Views/DealPaymentDocument/DealInitialBalanceCorrection/DealInitialBalanceCorrectionGrid.ascx",
                    dealInitialBalanceCorrectionPresenter.GetDealInitialBalanceCorrectionGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Детали

        [HttpGet]
        public ActionResult DealCreditInitialBalanceCorrectionDetails(string correctionId)
        {
            try
            {
                var model = dealInitialBalanceCorrectionPresenter.DealCreditInitialBalanceCorrectionDetails(ValidationUtils.TryGetNotEmptyGuid(correctionId), UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealInitialBalanceCorrection/DealCreditInitialBalanceCorrectionDetails.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult DealDebitInitialBalanceCorrectionDetails(string correctionId)
        {
            try
            {
                var model = dealInitialBalanceCorrectionPresenter.DealDebitInitialBalanceCorrectionDetails(ValidationUtils.TryGetNotEmptyGuid(correctionId), UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealInitialBalanceCorrection/DealDebitInitialBalanceCorrectionDetails.ascx", model);
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
                dealInitialBalanceCorrectionPresenter.DeleteDealCreditInitialBalanceCorrection(ValidationUtils.TryGetGuid(correctionId), UserSession.CurrentUserInfo);

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
                dealInitialBalanceCorrectionPresenter.DeleteDealDebitInitialBalanceCorrection(ValidationUtils.TryGetGuid(correctionId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Создание

        [HttpGet]
        public ActionResult CreateDealCreditInitialBalanceCorrection()
        {
            try
            {
                var model = dealInitialBalanceCorrectionPresenter.CreateDealCreditInitialBalanceCorrection(UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealInitialBalanceCorrection/DealCreditInitialBalanceCorrectionEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult CreateDealDebitInitialBalanceCorrection()
        {
            try
            {
                var model = dealInitialBalanceCorrectionPresenter.CreateDealDebitInitialBalanceCorrection(UserSession.CurrentUserInfo);

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

                dealInitialBalanceCorrectionPresenter.SaveDealCreditInitialBalanceCorrection(model, UserSession.CurrentUserInfo);

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

                dealInitialBalanceCorrectionPresenter.SaveDealDebitInitialBalanceCorrection(model, UserSession.CurrentUserInfo);

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
        /// Модальная форма выбора документов для ручного разнесения кредитовой корректировки сальдо по сделке
        /// </summary>
        /// <param name="model">Модель, заполненная на предыдущем этапе (в форме параметров кредитовой корректировки при создании)</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SelectDestinationDocumentsForDealCreditInitialBalanceCorrectionDistribution(DealCreditInitialBalanceCorrectionEditViewModel model)
        {
            try
            {
                return PartialView("~/Views/DealPaymentDocument/DestinationDocumentSelector/DestinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution.ascx",
                    dealInitialBalanceCorrectionPresenter.SelectDestinationDocumentsForDealCreditInitialBalanceCorrectionDistribution(model, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Грид реализаций для оплаты кредитовой корректировкой
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <returns></returns>
        public ActionResult ShowDestinationSaleGridForDealCreditInitialBalanceCorrectionDistribution(string dealId, string teamId)
        {
            try
            {
                return PartialView("~/Views/DealPaymentDocument/DestinationDocumentSelector/SaleWaybillSelectGrid.ascx",
                    dealInitialBalanceCorrectionPresenter.ShowDestinationSaleGridForDealCreditInitialBalanceCorrectionDistribution(
                    ValidationUtils.TryGetInt(dealId), ValidationUtils.TryGetShort(teamId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Грид документов для оплаты кредитовой корректировкой
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <returns></returns>
        public ActionResult ShowDestinationDocumentGridForDealCreditInitialBalanceCorrectionDistribution(string dealId, string teamId)
        {
            try
            {
                return PartialView("~/Views/DealPaymentDocument/DestinationDocumentSelector/DealDebitInitialBalanceCorrectionSelectGrid.ascx",
                    dealInitialBalanceCorrectionPresenter.ShowDestinationDocumentGridForDealCreditInitialBalanceCorrectionDistribution(
                    ValidationUtils.TryGetInt(dealId), ValidationUtils.TryGetShort(teamId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Модальная форма выбора документов для ручного переразнесения кредитовой корректировки сальдо по сделке
        /// </summary>
        /// <param name="dealCreditInitialBalanceCorrectionId">Код кредитовой корректировки сальдо по сделке</param>
        /// <param name="destinationDocumentSelectorControllerName">Название контроллера, который будет вызываться при submit формы разнесения</param>
        /// <param name="destinationDocumentSelectorActionName">Название метода контроллера, который будет вызываться при submit формы разнесения</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SelectDestinationDocumentsForDealCreditInitialBalanceCorrectionRedistribution(string dealCreditInitialBalanceCorrectionId,
            string destinationDocumentSelectorControllerName, string destinationDocumentSelectorActionName)
        {
            try
            {
                return PartialView("~/Views/DealPaymentDocument/DestinationDocumentSelector/DestinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution.ascx",
                    dealInitialBalanceCorrectionPresenter.SelectDestinationDocumentsForDealCreditInitialBalanceCorrectionRedistribution(ValidationUtils.TryGetGuid(dealCreditInitialBalanceCorrectionId),
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
