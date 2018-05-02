using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProductionOrderBatchLifeCycleTemplateStageMap : ClassMap<ProductionOrderBatchLifeCycleTemplateStage>
    {
        public ProductionOrderBatchLifeCycleTemplateStageMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Length(200).Access.CamelCaseField().Not.Nullable();
            Map(x => x.Type).CustomType<ProductionOrderBatchStageType>().Column("ProductionOrderBatchStageTypeId").Access.CamelCaseField().Not.Nullable();
            Map(x => x.OrdinalNumber).Not.Nullable();
            Map(x => x.PlannedDuration).Access.CamelCaseField();
            Map(x => x.InWorkDays).Not.Nullable();
            Map(x => x.IsDefault).Not.Nullable();

            References(x => x.Template).Column("TemplateId").Not.Nullable();
        }
    }
}
