using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class TaskTypeMap: ClassMap<TaskType>
    {
        public TaskTypeMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Length(100).Unique().Not.Nullable();
            
            HasMany(x=>x.States)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("TaskTypeId").Inverse().Cascade.DeleteOrphan();  //Удаляем статусы, которые не связаны с типом задачи
        }
    }
}
