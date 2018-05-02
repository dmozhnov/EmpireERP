using System;
using System.Web.Mvc;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    public class BaseWaybillController<T> : WholesaleController where T : BaseWaybill
    {
        private readonly IBaseWaybillPresenter<T> waybillPresenter;

        public BaseWaybillController(IBaseWaybillPresenter<T> waybillPresenter)
        {
            this.waybillPresenter = waybillPresenter;
        }

        /// <summary>
        /// Смена куратора
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        /// <param name="curatorId">Код куратора</param>
        [HttpPost]
        public ActionResult ChangeCurator(string waybillId, string curatorId)
        {
            try
            {
                waybillPresenter.ChangeCurator(waybillId, curatorId, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

    }
}
