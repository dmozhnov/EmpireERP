using Bizpulse.Admin.Domain.Entities;
using FluentNHibernate.Mapping;

namespace Bizpulse.Admin.Domain.NHibernate.Mappings
{
    public class RegionMap : ClassMap<Region>
    {
        public RegionMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Length(100).Not.Nullable();
            Map(x => x.Code).Length(2).Not.Nullable();
            Map(x => x.SortOrder).Not.Nullable();
        }
    }
}
