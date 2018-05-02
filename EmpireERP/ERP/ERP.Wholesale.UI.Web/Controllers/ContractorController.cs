using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ContractorController : WholesaleController
    {
        #region Поля

        private readonly IContractorPresenter contractorPresenter;

        #endregion

        #region Конструктор

        public ContractorController(IContractorPresenter contractorPresenter)
        {
            this.contractorPresenter = contractorPresenter;
        }

        #endregion

        #region Методы

        public ActionResult SelectContractor()
        {
            try
            {
                var model = contractorPresenter.SelectContractor(UserSession.CurrentUserInfo);

                return View("ContractorSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowContractorSelectGrid(GridState state)
        {
            try
            {
                var model = contractorPresenter.GetContractorGrid(state, UserSession.CurrentUserInfo);

                return View("ContractorSelectGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
    }
}
