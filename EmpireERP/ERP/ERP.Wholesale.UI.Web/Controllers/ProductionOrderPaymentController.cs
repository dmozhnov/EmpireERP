using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ProductionOrderPaymentController : WholesaleController
    {
        #region Поля

        private readonly IProductionOrderPaymentPresenter productionOrderPaymentPresenter;

        #endregion

        #region Конструкторы

        public ProductionOrderPaymentController(IProductionOrderPaymentPresenter productionOrderPaymentPresenter)
        {
            this.productionOrderPaymentPresenter = productionOrderPaymentPresenter;
        }

        #endregion

        #region Методы

        public ActionResult List()
        {
            try
            {
                return View(productionOrderPaymentPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProductionOrderPaymentGrid(GridState state)
        {
            try
            {
                ValidationUtils.NotNull(state,"Неверное значение входного параметра.");

                return PartialView("ProductionOrderPaymentGrid", productionOrderPaymentPresenter.GetProductionOrderPaymentGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Details(string productionOrderPaymentId)
        {
            try
            {
                var id = ValidationUtils.TryGetGuid(productionOrderPaymentId);

                return PartialView("~/Views/ProductionOrder/ProductionOrderPaymentEdit.ascx", productionOrderPaymentPresenter.Details(id, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ChangeProductionOrderPaymentCurrencyRate(string productionOrderPaymentId, string currencyRateId)
        {
            try
            {
                return Json(productionOrderPaymentPresenter.ChangeProductionOrderPaymentCurrencyRate(ValidationUtils.TryGetNotEmptyGuid(productionOrderPaymentId),
                    !String.IsNullOrEmpty(currencyRateId) ? ValidationUtils.TryGetInt(currencyRateId) : (int?)null, UserSession.CurrentUserInfo),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование планового платежа
        /// </summary>
        /// <param name="productionOrderPaymentId">Код оплаты</param>
        /// <param name="productionOrderPlannedPaymentId">Код планового платежа</param>
        [HttpPost]
        public ActionResult ChangeProductionOrderPaymentPlannedPayment(string productionOrderPaymentId, string productionOrderPlannedPaymentId)
        {
            try
            {
                productionOrderPaymentPresenter.ChangeProductionOrderPaymentPlannedPayment(
                    ValidationUtils.TryGetGuid(productionOrderPaymentId),ValidationUtils.TryGetGuid(productionOrderPlannedPaymentId),
                    UserSession.CurrentUserInfo);

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
