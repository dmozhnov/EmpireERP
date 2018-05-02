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
    public class ProductionOrderExtraExpensesSheetController : WholesaleController
    {
        #region Поля

        private readonly IProductionOrderExtraExpensesSheetPresenter productionOrderExtraExpensesSheetPresenter;

        #endregion

        #region Конструкторы

        public ProductionOrderExtraExpensesSheetController(IProductionOrderExtraExpensesSheetPresenter productionOrderExtraExpensesSheetPresenter)
        {
            this.productionOrderExtraExpensesSheetPresenter = productionOrderExtraExpensesSheetPresenter;
        }

        #endregion

        #region Методы

        public ActionResult List()
        {
            try
            {
                return View(productionOrderExtraExpensesSheetPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProductionOrderExtraExpensesSheetGrid(GridState state)
        {
            try
            {
                ValidationUtils.NotNull(state,"Неверное значение входного параметра.");

                return PartialView("ProductionOrderExtraExpensesSheetGrid", productionOrderExtraExpensesSheetPresenter.GetProductionOrderExtraExpensesSheetGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
    }
}
