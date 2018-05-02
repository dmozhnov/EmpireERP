using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class WaybillRowArticleMovementMap : ClassMap<WaybillRowArticleMovement>
    {
        public WaybillRowArticleMovementMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.DestinationWaybillRowId).Not.Nullable();
            Map(x => x.DestinationWaybillType).CustomType<WaybillType>().Column("DestinationWaybillTypeId").Not.Nullable();

            Map(x => x.SourceWaybillRowId).Not.Nullable();
            Map(x => x.SourceWaybillType).CustomType<WaybillType>().Column("SourceWaybillTypeId").Not.Nullable();

            Map(x => x.MovingCount).Precision(18).Scale(6).Not.Nullable();

            Map(x => x.IsManuallyCreated).Not.Nullable();

            BatchSize(10);
        }
    }
}
