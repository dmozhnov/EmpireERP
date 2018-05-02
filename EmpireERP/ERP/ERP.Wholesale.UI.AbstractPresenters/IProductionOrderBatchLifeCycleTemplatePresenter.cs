using System.Collections.Generic;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.ViewModels.ProductionOrderBatchLifeCycleTemplate;
using ERP.Infrastructure.Security;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IProductionOrderBatchLifeCycleTemplatePresenter
    {
        ProductionOrderBatchLifeCycleTemplateListViewModel List(UserInfo currentUser);
        GridData GetProductionOrderBatchLifeCycleTemplateGrid(GridState state, UserInfo currentUser);

        ProductionOrderBatchLifeCycleTemplateEditViewModel Create(UserInfo currentUser);
        ProductionOrderBatchLifeCycleTemplateEditViewModel Edit(short id, UserInfo currentUser);
        object Save(ProductionOrderBatchLifeCycleTemplateEditViewModel model, UserInfo currentUser);
        void Delete(short id, UserInfo currentUser);

        ProductionOrderBatchLifeCycleTemplateDetailsViewModel Details(short id, string backUrl, UserInfo currentUser);
        GridData GetProductionOrderBatchLifeCycleTemplateStageGrid(GridState state, UserInfo currentUser);

        ProductionOrderBatchLifeCycleTemplateStageEditViewModel AddStage(short productionOrderBatchLifeCycleTemplateId, int id, short position, UserInfo currentUser);
        ProductionOrderBatchLifeCycleTemplateStageEditViewModel EditStage(short productionOrderBatchLifeCycleTemplateId, int id, UserInfo currentUser);
        int SaveStage(ProductionOrderBatchLifeCycleTemplateStageEditViewModel model, UserInfo currentUser);
        void DeleteStage(short productionOrderBatchLifeCycleTemplateId, int id, UserInfo currentUser);
        void MoveStageUp(short productionOrderBatchLifeCycleTemplateId, int id, UserInfo currentUser);
        void MoveStageDown(short productionOrderBatchLifeCycleTemplateId, int id, UserInfo currentUser);

        ProductionOrderBatchLifeCycleTemplateSelectViewModel SelectProductionOrderBatchLifeCycleTemplate(UserInfo currentUser);
        GridData GetProductionOrderBatchLifeCycleTemplateSelectGrid(GridState state, UserInfo currentUser);
    }
}
