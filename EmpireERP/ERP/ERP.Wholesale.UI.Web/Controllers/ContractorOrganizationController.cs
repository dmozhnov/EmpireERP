using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ContractorOrganizationController : WholesaleController
    {
        #region Поля

        private readonly IContractorOrganizationPresenter contractorOrganizationPresenter;

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        public ContractorOrganizationController(IContractorOrganizationPresenter contractorOrganizationPresenter) : base()
        {            
            this.contractorOrganizationPresenter = contractorOrganizationPresenter;
        }

        #endregion

        #region Список организаций

        /// <summary>
        /// Список организаций
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult List()
        {
            try
            {
                var model = contractorOrganizationPresenter.List(UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }        

        /// <summary>
        /// Получение грида организаций
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShowContractorOrganizationGrid(GridState state)
        {
            try
            {
                var grid = contractorOrganizationPresenter.GetContractorOrganizationGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ContractorOrganizationGrid", grid);
            }
            catch(Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

    }
}
