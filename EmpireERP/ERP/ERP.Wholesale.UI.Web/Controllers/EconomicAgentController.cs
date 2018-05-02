using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class EconomicAgentController : WholesaleController
    {
        #region Поля

        private readonly IEconomicAgentPresenter economicAgentPresenter;

        #endregion

        #region Конструктор

        public EconomicAgentController(IEconomicAgentPresenter economicAgentPresenter)             
        {
            this.economicAgentPresenter = economicAgentPresenter;
        }

        #endregion

        #region Создание организации

        [HttpPost]
        public ActionResult SelectType(EconomicAgentTypeSelectorViewModel typeOrg)
        {            
            try
            {
                if (typeOrg.IsJuridicalPerson)
                {
                    var jp = economicAgentPresenter.SelectTypeJuridicalPerson(typeOrg);

                    return PartialView("JuridicalPersonEdit", jp);
                }
                else
                {
                    var pp = economicAgentPresenter.SelectTypePhysicalPerson(typeOrg);

                    return PartialView("PhysicalPersonEdit", pp);
                }
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        #endregion

    }
}
