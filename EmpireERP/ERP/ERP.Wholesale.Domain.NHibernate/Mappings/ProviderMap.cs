using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProviderMap : SubclassMap<Provider>
    {
        public ProviderMap()
        {
            KeyColumn("Id");

            Map(x => x.Reliability).CustomType<ProviderReliability>().Column("ProviderReliabilityId").Not.Nullable();

            References(x => x.Type).Column("ProviderTypeId").Not.Nullable();

            BatchSize(50);
        }
    }
}
