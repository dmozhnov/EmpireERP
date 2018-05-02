using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class StorageMap : ClassMap<Storage>
    {
        public StorageMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Name).Length(200).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);
            Map(x => x.Comment).Length(4000).Not.Nullable();
            Map(x => x.Type).CustomType<StorageType>().Column("StorageTypeId").Not.Nullable();
            
            HasMany(x => x.Sections)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("StorageId").Inverse().Cascade.SaveUpdate()
                .Where("DeletionDate is null")
                .BatchSize(10);

            HasManyToMany(x => x.AccountOrganizations)
                .AsSet().Access.CamelCaseField()
                .Table("AccountOrganizationStorage")
                .ParentKeyColumn("StorageId")
                .ChildKeyColumn("AccountOrganizationId").Cascade.All()
                .BatchSize(10);

            Where("DeletionDate is null");

            Cache.ReadWrite();
            BatchSize(50);
        }
    }
}
