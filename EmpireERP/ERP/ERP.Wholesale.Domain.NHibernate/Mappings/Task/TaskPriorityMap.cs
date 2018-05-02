using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class TaskPriorityMap: ClassMap<TaskPriority>
    {
        public TaskPriorityMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Length(100).Not.Nullable();
            Map(x => x.OrdinalNumber).Unique().Not.Nullable();

            BatchSize(50);
        }
    }
}
