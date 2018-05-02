using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class CurrencyMap : ClassMap<Currency>
    {
        public CurrencyMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Length(20).Not.Nullable();
            Map(x => x.NumericCode).Length(3).Not.Nullable();
            Map(x => x.LiteralCode).Length(3).Not.Nullable();
            Map(x => x.DeletionDate);

            HasMany(x => x.Rates).AsSet().Access.CamelCaseField().KeyColumn("CurrencyId").Inverse().Cascade.AllDeleteOrphan();

            Cache.ReadWrite();
        }
    }
}
