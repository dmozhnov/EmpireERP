using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class DefaultProductionOrderBatchStageMap : ClassMap<DefaultProductionOrderBatchStage>
    {
        public DefaultProductionOrderBatchStageMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Length(200).Access.CamelCaseField().Not.Nullable();
            Map(x => x.Type).CustomType<ProductionOrderBatchStageType>().Column("ProductionOrderBatchStageTypeId").Access.CamelCaseField().Not.Nullable();
        }
    }
}
