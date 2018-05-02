using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.ProductionOrder;
using ERP.Wholesale.UI.ViewModels.ProductionOrderTransportSheet;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IProductionOrderTransportSheetPresenter
    {
       ProductionOrderTransportSheetListViewModel List(UserInfo currentUser);
       GridData GetProductionOrderTransportSheetGrid(GridState state, UserInfo currentUser);
    }
}
