using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ClientContract;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ClientContractController : WholesaleController
    {
        #region Поля

        private readonly IClientContractPresenter clientContractPresenter;

        #endregion

        #region Конструкторы

        public ClientContractController(IClientContractPresenter clientContractPresenter)
        {
            this.clientContractPresenter = clientContractPresenter;
        }

        #endregion

        #region Методы

        #region Модальная форма выбора договора с клиентом.

        /// <summary>
        /// Возвращает модальную форму для выбора договора с клиентом.
        /// </summary>
        /// <param name="dealId">Идентификатор сделки, для которой выбирается договор.</param>
        [HttpGet]
        public ActionResult Select(string dealId)
        {
            try
            {
                var model = clientContractPresenter.Select(ValidationUtils.TryGetInt(dealId), UserSession.CurrentUserInfo);

                return PartialView("ClientContractSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение грида доступных для выбора договоров с клиентом.
        /// </summary>
        /// <param name="state">Состояние грида.</param>
        [HttpPost]
        public ActionResult ShowSelectGrid(GridState state)
        {
            try
            {
                var model = clientContractPresenter.GetSelectGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ClientContractSelectGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Создание/редактирование

        /// <summary>
        /// Создание / редактирование договора
        /// </summary>
        [HttpGet]
        public ActionResult EditContract(string contractId)
        {
            try
            {
                var model = clientContractPresenter.EditContract(ValidationUtils.TryGetShort(contractId), UserSession.CurrentUserInfo);

                return PartialView("ClientContractEdit", model);
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
                var result = clientContractPresenter.SaveContract(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        [HttpGet]
        public ActionResult IsUsedBySingleDeal(string clientContractId, string dealId)
        {
            try
            {
                var result = clientContractPresenter.IsUsedBySingleDeal(ValidationUtils.TryGetShort(clientContractId), ValidationUtils.TryGetInt(dealId), UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
    }
}