using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class TaskExecutionStateMap: ClassMap<TaskExecutionState>
    {
        public TaskExecutionStateMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Length(100).Not.Nullable();
            Map(x => x.OrdinalNumber).Not.Nullable();
            Map(x => x.ExecutionStateType).CustomType<TaskExecutionStateType>().Column("ExecutionStateTypeId").Not.Nullable();

            BatchSize(50);
        }
    }
}
