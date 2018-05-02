using System;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.ProductionOrder;
using ERP.Wholesale.UI.ViewModels.ProductionOrderExtraExpensesSheet;
using ERP.Infrastructure.Security;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IProductionOrderExtraExpensesSheetPresenter
    {
       ProductionOrderExtraExpensesSheetListViewModel List(UserInfo currentUser);
       GridData GetProductionOrderExtraExpensesSheetGrid(GridState state, UserInfo currentUser);
    }
}
