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
    public class DealPaymentDocumentController : WholesaleController
    {
        #region Поля

        private readonly IDealPaymentDocumentPresenter dealPaymentDocumentPresenter;

        #endregion

        #region Конструкторы

        public DealPaymentDocumentController(IDealPaymentDocumentPresenter dealPaymentDocumentPresenter)
        {
            this.dealPaymentDocumentPresenter = dealPaymentDocumentPresenter;
        }

        #endregion

        #region Методы

        #region Получение гридов разнесения в деталях платежных документов

        /// <summary>
        /// Получение грида реализаций, на которые был разнесен данный платежный документ
        /// </summary>
        /// <param name="state">Состояние грида</param>
        [HttpPost]
        public ActionResult ShowSaleWaybillGrid(GridState state)
        {
            try
            {
                var model = dealPaymentDocumentPresenter.GetSaleWaybillGrid(state, UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/SaleWaybillGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение грида дебетов корректировок, на которые был разнесен данный платежный документ
        /// </summary>
        /// <param name="state">Состояние грида</param>
        [HttpPost]
        public ActionResult ShowDealDebitInitialBalanceCorrectionGrid(GridState state)
        {
            try
            {
                var model = dealPaymentDocumentPresenter.GetDealDebitInitialBalanceCorrectionGrid(state, UserSession.CurrentUserInfo);

                return PartialView("~/Views/DealPaymentDocument/DealDebitInitialBalanceCorrectionGrid.ascx", model);
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
