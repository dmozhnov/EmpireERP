using ERP.Wholesale.Domain.Entities.Security;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Security
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.Id).GeneratedBy.Foreign("Employee");
            
            Map(x => x.DisplayName).Length(100).Not.Nullable();
            Map(x => x.DisplayNameTemplate).Length(3).Not.Nullable();

            Map(x => x.Login).Length(30).Unique().Not.Nullable();
            Map(x => x.PasswordHash).Length(1024).Not.Nullable();

            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.BlockingDate);

            References(x => x.CreatedBy).Column("CreatedById").Not.Nullable();
            References(x => x.Blocker).Column("BlockerId");

            HasOne(x => x.Employee).Constrained().ForeignKey();

            HasManyToMany(x => x.Roles)
                .AsSet().Access.CamelCaseField()
                .Table("UserRole")
                .ParentKeyColumn("UserId")
                .ChildKeyColumn("RoleId").Cascade.All()
                .BatchSize(10);
            
            HasManyToMany(x => x.Teams)
                .AsSet().Access.CamelCaseField()
                .Table("UserTeam")
                .ParentKeyColumn("UserId")
                .ChildKeyColumn("TeamId").Inverse()
                .BatchSize(10);

            Cache.ReadWrite();
            BatchSize(50);
        }
    }
}
