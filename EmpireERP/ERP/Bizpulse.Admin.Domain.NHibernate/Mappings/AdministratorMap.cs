using Bizpulse.Admin.Domain.Entities;
using FluentNHibernate.Mapping;

namespace Bizpulse.Admin.Domain.NHibernate.Mappings
{
    public class AdministratorMap : ClassMap<Administrator>
    {
        public AdministratorMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.LastName).Length(100).Not.Nullable();
            Map(x => x.FirstName).Length(100).Not.Nullable();

            Map(x => x.Login).Length(30).Unique().Not.Nullable();
            Map(x => x.PasswordHash).Length(1024).Not.Nullable();

            Map(x => x.CreationDate).Not.Nullable();
        }
    }
}
