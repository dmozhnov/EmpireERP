using ERP.Wholesale.Domain.Entities;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProductionOrderBatchStageMap : ClassMap<ProductionOrderBatchStage>
    {
        public ProductionOrderBatchStageMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.Name).Length(200).Access.CamelCaseField().Not.Nullable();
            Map(x => x.PlannedDuration).Access.CamelCaseField();
            Map(x => x.ActualStartDate);
            Map(x => x.ActualEndDate);
            Map(x => x.OrdinalNumber).Not.Nullable();
            Map(x => x.Type).CustomType<ProductionOrderBatchStageType>().Column("ProductionOrderBatchStageTypeId").Access.CamelCaseField().Not.Nullable();
            Map(x => x.InWorkDays).Not.Nullable();
            Map(x => x.IsDefault).Not.Nullable();

            References(x => x.Batch).Column("BatchId");
        }
    }
}
