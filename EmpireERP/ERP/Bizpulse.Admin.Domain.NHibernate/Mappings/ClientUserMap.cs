using Bizpulse.Admin.Domain.Entities;
using FluentNHibernate.Mapping;

namespace Bizpulse.Admin.Domain.NHibernate.Mappings
{
    public class ClientUserMap : ClassMap<ClientUser>
    {
        public ClientUserMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.LastName).Length(100).Not.Nullable();
            Map(x => x.FirstName).Length(100).Not.Nullable();
            Map(x => x.Patronymic).Length(100).Not.Nullable();
            Map(x => x.Login).Length(30).Not.Nullable();
            Map(x => x.PasswordHash).Length(1024).Not.Nullable();
            Map(x => x.IsClientAdmin).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);
            Map(x => x.IsBlocked).Not.Nullable();

            References(x => x.Client).Column("ClientId").Not.Nullable();

            Where("DeletionDate is null");
        }
    }
}
