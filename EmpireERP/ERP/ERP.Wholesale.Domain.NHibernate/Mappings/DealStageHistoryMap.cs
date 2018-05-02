using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class DealStageHistoryMap : ClassMap<DealStageHistory>
    {
        public DealStageHistoryMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.EndDate);
            Map(x => x.DealStage).CustomType<DealStage>().Column("DealStageId").Not.Nullable();

            References(x => x.Deal).Column("DealId").Not.Nullable();
        }
    }
}
