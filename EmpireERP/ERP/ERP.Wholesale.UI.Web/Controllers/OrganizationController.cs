using System;
using System.Web.Mvc;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.Web.Infrastructure;
using System.Web.UI;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class OrganizationController : WholesaleController
    {
        #region Поля

        private readonly IOrganizationPresenter organizationPresenter;

        #endregion

        #region Конструкторы

        public OrganizationController(IOrganizationPresenter organizationPresenter)
        {
            this.organizationPresenter = organizationPresenter;
        }

        #endregion

        /// <summary>
        /// Получение данных банка по БИК
        /// </summary>
        /// <param name="bic">БИК</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetBankByBIC(string bic)
        {
            try
            {
                var result = organizationPresenter.GetBankByBIC(bic);
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }


        public ActionResult GetForeignBankBySWIFT(string swift)
        {
            try
            {
                var result = organizationPresenter.GetBankBySWIFT(swift);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
    }
}
