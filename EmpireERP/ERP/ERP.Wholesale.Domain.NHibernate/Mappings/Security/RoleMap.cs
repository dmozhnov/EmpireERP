using ERP.Wholesale.Domain.Entities.Security;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Security
{
    public class RoleMap : ClassMap<Role>
    {
        public RoleMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Length(200).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);
            Map(x => x.Comment).Length(4000).Not.Nullable();
            Map(x => x.IsSystemAdmin).Not.Nullable();

            HasMany(x => x.PermissionDistributions)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("RoleId").Inverse().Cascade.AllDeleteOrphan();
                
            HasManyToMany(x => x.Users)
                .AsSet().Access.CamelCaseField()
                .Table("UserRole")
                .ParentKeyColumn("RoleId")
                .ChildKeyColumn("UserId").Inverse()
                .BatchSize(10);

            Where("DeletionDate is null");

            Cache.ReadWrite();
        }
    }
}
