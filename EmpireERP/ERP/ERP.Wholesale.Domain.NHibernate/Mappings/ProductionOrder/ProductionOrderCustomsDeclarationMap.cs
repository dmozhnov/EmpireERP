using ERP.Wholesale.Domain.Entities;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProductionOrderCustomsDeclarationMap : ClassMap<ProductionOrderCustomsDeclaration>
    {
        public ProductionOrderCustomsDeclarationMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.CustomsDeclarationNumber).Length(33).Not.Nullable();
            Map(x => x.Name).Length(200).Not.Nullable();
            Map(x => x.Date).Access.CamelCaseField().Not.Nullable();
            Map(x => x.ImportCustomsDutiesSum).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.ExportCustomsDutiesSum).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.ValueAddedTaxSum).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.ExciseSum).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.CustomsFeesSum).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.CustomsValueCorrection).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate).Access.CamelCaseField();
            Map(x => x.Comment).Length(4000).Not.Nullable();

            References(x => x.ProductionOrder).Column("ProductionOrderId").Not.Nullable();

            HasMany(x => x.Payments).AsSet().Access.CamelCaseField().KeyColumn("CustomsDeclarationId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");
        }
    }
}
