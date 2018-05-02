using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IProductionOrderBatchLifeCycleTemplateService
    {
        ProductionOrderBatchLifeCycleTemplate GetById(short id);
        short Save(ProductionOrderBatchLifeCycleTemplate productionOrderBatchLifeCycleTemplate);
        void Delete(ProductionOrderBatchLifeCycleTemplate productionOrderBatchLifeCycleTemplate);
        IEnumerable<ProductionOrderBatchLifeCycleTemplate> GetFilteredList(object state);
        IEnumerable<ProductionOrderBatchLifeCycleTemplate> GetFilteredList(object state, ParameterString parameterString);
        ProductionOrderBatchLifeCycleTemplate CheckProductionOrderBatchLifeCycleTemplateExistence(short id, User user);
        bool IsNameUnique(string name, short id);

        bool IsPossibilityToEdit(ProductionOrderBatchLifeCycleTemplate template, User user);
        void CheckPossibilityToEdit(ProductionOrderBatchLifeCycleTemplate template, User user);

        bool IsPossibilityToDelete(ProductionOrderBatchLifeCycleTemplate template, User user);
        void CheckPossibilityToDelete(ProductionOrderBatchLifeCycleTemplate template, User user);

        bool IsPossibilityToCreateStageAfter(ProductionOrderBatchLifeCycleTemplateStage stage, User user);
        void CheckPossibilityToCreateStageAfter(ProductionOrderBatchLifeCycleTemplateStage stage, User user);
        bool IsPossibilityToEditStage(ProductionOrderBatchLifeCycleTemplateStage stage, User user);
        void CheckPossibilityToEditStage(ProductionOrderBatchLifeCycleTemplateStage stage, User user);
        bool IsPossibilityToDeleteStage(ProductionOrderBatchLifeCycleTemplateStage stage, User user);
        void CheckPossibilityToDeleteStage(ProductionOrderBatchLifeCycleTemplateStage stage, User user);
        bool IsPossibilityToMoveStageUp(ProductionOrderBatchLifeCycleTemplateStage stage, User user);
        void CheckPossibilityToMoveStageUp(ProductionOrderBatchLifeCycleTemplateStage stage, User user);
        bool IsPossibilityToMoveStageDown(ProductionOrderBatchLifeCycleTemplateStage stage, User user);
        void CheckPossibilityToMoveStageDown(ProductionOrderBatchLifeCycleTemplateStage stage, User user);
    }
}