using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class TaskHistoryItemMap: SubclassMap<TaskHistoryItem>
    {
        public TaskHistoryItemMap()
        {
            KeyColumn("Id");

            References(x => x.Task).Column("TaskId").Not.Nullable();

            Map(x => x.IsContractorChanged).Not.Nullable();
            Map(x => x.IsDeadLineChanged).Not.Nullable();
            Map(x => x.IsDealChanged).Not.Nullable();
            Map(x => x.IsDeletionDateChanged).Not.Nullable();
            Map(x => x.IsDescriptionChanged).Not.Nullable();
            Map(x => x.IsExecutedByChanged).Not.Nullable();
            Map(x => x.IsFactualCompletionDateChanged).Not.Nullable();
            Map(x => x.IsFactualSpentTimeChanged).Not.Nullable();
            Map(x => x.IsTaskPriorityChanged).Not.Nullable();
            Map(x => x.IsProductionOrderChanged).Not.Nullable();
            Map(x => x.IsStartDateChanged).Not.Nullable();
            Map(x => x.IsTopicChanged).Not.Nullable();
            Map(x => x.IsTaskTypeChanged).Not.Nullable();
            Map(x => x.IsTaskExecutionStateChanged).Not.Nullable();

            Map(x => x.Deadline);
            Map(x => x.DeletionDate);
            Map(x => x.Description).Length(8000).Not.Nullable();
            Map(x => x.FactualCompletionDate);
            Map(x => x.FactualSpentTime);
            Map(x => x.StartDate);
            Map(x => x.Topic).Length(200).Not.Nullable();

            References(x => x.Contractor).Column("ContractorId");
            References(x => x.Deal).Column("DealId");
            References(x => x.ExecutedBy).Column("ExecutedById");
            References(x => x.TaskPriority).Column("TaskPriorityId");
            References(x => x.ProductionOrder).Column("ProductionOrderId");
            References(x => x.TaskType).Column("TaskTypeId");
            References(x => x.TaskExecutionState).Column("TaskExecutionStateId");
            References(x => x.TaskExecutionItem).Column("TaskExecutionItemId");
        }
    }
}
