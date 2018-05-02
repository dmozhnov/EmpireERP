using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProductionOrderBatchLifeCycleTemplateMap : ClassMap<ProductionOrderBatchLifeCycleTemplate>
    {
        public ProductionOrderBatchLifeCycleTemplateMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Length(200).Not.Nullable();

            HasMany(x => x.Stages)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("TemplateId").Inverse().Cascade.AllDeleteOrphan();
        }
    }
}
