using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Security
{
    public class EmployeePostMap : ClassMap<EmployeePost>
    {
        public EmployeePostMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Name).Length(100).Unique().Not.Nullable();

            BatchSize(10);
        }
    }
}
