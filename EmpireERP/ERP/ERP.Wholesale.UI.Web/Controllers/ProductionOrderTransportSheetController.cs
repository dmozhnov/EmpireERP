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
    public class ProductionOrderTransportSheetController : WholesaleController
    {
        #region Поля

        private readonly IProductionOrderTransportSheetPresenter productionOrderTransportSheetPresenter;

        #endregion

        #region Конструкторы

        public ProductionOrderTransportSheetController(IProductionOrderTransportSheetPresenter productionOrderTransportSheetPresenter)
        {
            this.productionOrderTransportSheetPresenter = productionOrderTransportSheetPresenter;
        }

        #endregion

        #region Методы

        public ActionResult List()
        {
            try
            {
                return View(productionOrderTransportSheetPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProductionOrderTransportSheetGrid(GridState state)
        {
            try
            {
                ValidationUtils.NotNull(state,"Неверное значение входного параметра.");

                return PartialView("ProductionOrderTransportSheetGrid", productionOrderTransportSheetPresenter.GetProductionOrderTransportSheetGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
    }
}
