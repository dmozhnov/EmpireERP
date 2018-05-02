using Bizpulse.Admin.Domain.Entities;
using FluentNHibernate.Mapping;

namespace Bizpulse.Admin.Domain.NHibernate.Mappings
{
    public class CityMap : ClassMap<City>
    {
        public CityMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Length(100).Not.Nullable();
            Map(x => x.SortOrder).Not.Nullable();

            References(x => x.Region).Column("RegionId").Not.Nullable();
        }
    }
}
