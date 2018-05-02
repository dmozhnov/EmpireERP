using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Security
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.LastName).Length(100).Not.Nullable();
            Map(x => x.FirstName).Length(100).Not.Nullable();
            Map(x => x.Patronymic).Length(100).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();

            HasOne(x => x.User).Cascade.All();

            References(x => x.Post).Column("EmployeePostId").Not.Nullable();
            References(x => x.CreatedBy).Column("CreatedById").Not.Nullable();

            BatchSize(10);
        }
    }
}
