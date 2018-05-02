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
    public class ProductionOrderCustomsDeclarationController : WholesaleController
    {
        #region Поля

        private readonly IProductionOrderCustomsDeclarationPresenter productionOrderCustomsDeclarationPresenter;

        #endregion

        #region Конструкторы

        public ProductionOrderCustomsDeclarationController(IProductionOrderCustomsDeclarationPresenter productionOrderCustomsDeclarationPresenter)
        {
            this.productionOrderCustomsDeclarationPresenter = productionOrderCustomsDeclarationPresenter;
        }

        #endregion

        #region Методы

        public ActionResult List()
        {
            try
            {
                return View(productionOrderCustomsDeclarationPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProductionOrderCustomsDeclarationGrid(GridState state)
        {
            try
            {
                ValidationUtils.NotNull(state,"Неверное значение входного параметра.");

                return PartialView("ProductionOrderCustomsDeclarationGrid", productionOrderCustomsDeclarationPresenter.GetProductionOrderCustomsDeclarationGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
    }
}
