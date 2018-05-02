using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProductionOrderMaterialsPackageMap : ClassMap<ProductionOrderMaterialsPackage>
    {
        public ProductionOrderMaterialsPackageMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.Name).Length(250).Not.Nullable();
            Map(x => x.Description).Length(250).Not.Nullable();
            Map(x => x.Comment).Length(4000).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.LastChangeDate).Not.Nullable();
            Map(x => x.DeletionDate);
            Map(x => x.ProductionOrderMaterialsPackageSize).Precision(18).Scale(6).Not.Nullable();

            References(x => x.ProductionOrder).Column("ProductionOrderId").Not.Nullable();

            HasMany(x => x.Documents).AsSet().Access.CamelCaseField().KeyColumn("MaterialsPackageId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");

            Where("DeletionDate is null");
        }
    }
}
