using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class CountryMap : ClassMap<Country>
    {
        public CountryMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Name).Length(200).Unique().Not.Nullable();
            Map(x => x.NumericCode).Length(3).Not.Nullable();

            BatchSize(50);
        }
    }
}
