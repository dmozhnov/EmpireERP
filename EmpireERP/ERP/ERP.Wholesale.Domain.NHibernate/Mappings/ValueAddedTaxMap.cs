using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ValueAddedTaxMap : ClassMap<ValueAddedTax>
    {
        public ValueAddedTaxMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Name).Length(100).Not.Nullable();
            Map(x => x.Value).Precision(4).Scale(2).Not.Nullable();
            Map(x => x.IsDefault).Not.Nullable();

            Cache.ReadWrite();
        }
    }
}
