using Bizpulse.Admin.Domain.Entities;
using FluentNHibernate.Mapping;

namespace Bizpulse.Admin.Domain.NHibernate.Mappings
{
    public class RateMap : ClassMap<Rate>
    {
        public RateMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Length(100).Not.Nullable();
            Map(x => x.ActiveUserCountLimit).Not.Nullable();
            Map(x => x.TeamCountLimit).Not.Nullable();
            Map(x => x.StorageCountLimit).Not.Nullable();
            Map(x => x.AccountOrganizationCountLimit).Not.Nullable();
            Map(x => x.GigabyteCountLimit).Not.Nullable();

            Map(x => x.UseExtraActiveUserOption).Not.Nullable();
            Map(x => x.UseExtraTeamOption).Not.Nullable();
            Map(x => x.UseExtraStorageOption).Not.Nullable();
            Map(x => x.UseExtraAccountOrganizationOption).Not.Nullable();
            Map(x => x.UseExtraGigabyteOption).Not.Nullable();

            Map(x => x.ExtraActiveUserOptionCostPerMonth).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.ExtraTeamOptionCostPerMonth).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.ExtraStorageOptionCostPerMonth).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.ExtraAccountOrganizationOptionCostPerMonth).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.ExtraGigabyteOptionCostPerMonth).Precision(18).Scale(2).Not.Nullable();

            Map(x => x.BaseCostPerMonth).Precision(18).Scale(2).Not.Nullable();
        }
    }
}
