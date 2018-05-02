using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class TaskExecutionItemMap: ClassMap<TaskExecutionItem>
    {
        public TaskExecutionItemMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.Date).Access.CamelCaseField().Not.Nullable();
            Map(x => x.DeletionDate).Access.CamelCaseField();
            Map(x => x.ResultDescription).Access.CamelCaseField().Length(4000).Not.Nullable();
            Map(x => x.SpentTime).Access.CamelCaseField().Not.Nullable();
            Map(x => x.CompletionPercentage).Access.CamelCaseField().Not.Nullable();
            Map(x => x.IsCreatedByUser).Not.Nullable();

            References(x => x.ExecutionState).Column("ExecutionStateId").Access.CamelCaseField().Not.Nullable();
            References(x => x.CreatedBy).Column("CreatedById").Not.Nullable();
            References(x => x.Task).Column("TaskId").Not.Nullable();
            References(x => x.TaskType).Column("TaskTypeId").Access.CamelCaseField().Not.Nullable();

            HasMany(x=>x.History)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("TaskExecutionItemId").Inverse().Cascade.SaveUpdate();
        }
    }
}
