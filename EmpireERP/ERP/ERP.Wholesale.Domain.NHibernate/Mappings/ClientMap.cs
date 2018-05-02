using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ClientMap : SubclassMap<Client>
    {
        public ClientMap()
        {
            KeyColumn("Id");

            Map(x => x.Loyalty).CustomType<ClientLoyalty>().Column("LoyaltyId").Not.Nullable();
            Map(x => x.ManualBlockingDate);
            Map(x => x.FactualAddress).Length(250).Not.Nullable();
            Map(x => x.ContactPhone).Length(20).Not.Nullable();

            References(x => x.Type).Column("ClientTypeId").Not.Nullable();
            References(x => x.Region).Column("RegionId").Not.Nullable();
            References(x => x.ServiceProgram).Column("ServiceProgramId").Not.Nullable();
            References(x => x.ManualBlocker).Column("ManualBlockerId");

            HasMany(x => x.Deals).AsSet().Access.CamelCaseField().KeyColumn("ClientId").Inverse().Cascade.SaveUpdate();

            BatchSize(50);
        }
    }
}
