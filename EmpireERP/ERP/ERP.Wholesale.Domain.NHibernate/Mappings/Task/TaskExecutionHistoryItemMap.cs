using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class TaskExecutionHistoryItemMap : SubclassMap<TaskExecutionHistoryItem>
    {
        public TaskExecutionHistoryItemMap()
        {
            KeyColumn("Id");

            References(x => x.TaskExecutionItem).Column("TaskExecutionItemId").Not.Nullable();
            References(x => x.Task).Column("TaskId").Not.Nullable();

            Map(x => x.IsDateChanged).Not.Nullable();
            Map(x => x.IsDeletionDateChanged).Not.Nullable();
            Map(x => x.IsResultDescriptionChanged).Not.Nullable();
            Map(x => x.IsSpentTimeChanged).Not.Nullable();
            Map(x => x.IsCompletionPercentageChanged).Not.Nullable();
            Map(x => x.IsTaskExecutionStateChanged).Not.Nullable();
            Map(x => x.IsTaskTypeChanged).Not.Nullable();

            Map(x => x.Date);
            Map(x => x.DeletionDate);
            Map(x => x.ResultDescription).Length(4000).Not.Nullable();
            Map(x => x.SpentTime);
            Map(x => x.CompletionPercentage);

            References(x => x.TaskExecutionState).Column("TaskExecutionStateId");
            References(x => x.TaskType).Column("TaskTypeId");
        }
    }
}
