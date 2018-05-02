using ERP.Wholesale.Domain.Entities.Security;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Security
{
    public class PermissionDistributionMap : ClassMap<PermissionDistribution>
    {
        public PermissionDistributionMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Type).CustomType<PermissionDistributionType>().Column("PermissionDistributionTypeId").Not.Nullable();
            Map(x => x.Permission).CustomType<Permission>().Column("PermissionId").Not.Nullable();
            
            References(x => x.Role).Column("RoleId").Not.Nullable();

            //Cache.ReadWrite();
        }
    }
}
