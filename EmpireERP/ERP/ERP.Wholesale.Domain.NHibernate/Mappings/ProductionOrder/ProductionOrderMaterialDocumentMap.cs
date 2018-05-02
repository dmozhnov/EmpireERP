using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate
{
    public class ProductionOrderMaterialsPackageDocumentMap: ClassMap<ProductionOrderMaterialsPackageDocument>
    {
        public ProductionOrderMaterialsPackageDocumentMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.FileName).Length(250).Not.Nullable();
            Map(x => x.Description).Length(250).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.LastChangeDate).Not.Nullable();
            Map(x => x.DeletionDate);
            Map(x => x.Size).Precision(18).Scale(6).Not.Nullable();

            References(x => x.CreatedBy).Column("CreatedById").Not.Nullable();
            References(x => x.MaterialsPackage).Column("MaterialsPackageId").Not.Nullable();
        }
    }
}
