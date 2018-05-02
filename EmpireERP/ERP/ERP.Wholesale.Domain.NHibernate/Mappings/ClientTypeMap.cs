using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ClientTypeMap : ClassMap<ClientType>
    {
        public ClientTypeMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Name).Length(200).Unique().Not.Nullable();

            BatchSize(10);
        }
    }
}
