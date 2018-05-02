using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class MeasureUnitMap : ClassMap<MeasureUnit>
    {
        public MeasureUnitMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.NumericCode).Length(3).Not.Nullable();
            Map(x => x.FullName).Length(20).Unique().Not.Nullable();
            Map(x => x.ShortName).Length(7).Unique().Not.Nullable();
            Map(x => x.Comment).Length(500).Not.Nullable();
            Map(x => x.Scale).Not.Nullable();

            Cache.ReadWrite();
        }
    }
}
