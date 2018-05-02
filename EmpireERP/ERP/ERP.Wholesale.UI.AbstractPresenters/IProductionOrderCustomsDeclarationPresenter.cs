using System;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.ProductionOrder;
using ERP.Wholesale.UI.ViewModels.ProductionOrderCustomsDeclaration;
using ERP.Infrastructure.Security;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IProductionOrderCustomsDeclarationPresenter
    {
       ProductionOrderCustomsDeclarationListViewModel List(UserInfo currentUser);
       GridData GetProductionOrderCustomsDeclarationGrid(GridState state, UserInfo currentUser);
    }
}
