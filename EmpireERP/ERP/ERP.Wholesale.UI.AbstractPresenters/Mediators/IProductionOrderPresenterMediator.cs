using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.ViewModels.ProductionOrder;

namespace ERP.Wholesale.UI.AbstractPresenters.Mediators
{
    public interface IProductionOrderPresenterMediator
    {
        ProductionOrderBatchMainDetailsViewModel GetProductionOrderBatchMainDetails(ProductionOrderBatch productionOrderBatch,
            User user);
    }
}
