using Bizpulse.Admin.Domain.Entities;
using Bizpulse.Admin.Domain.ValueObjects;
using FluentNHibernate.Mapping;

namespace Bizpulse.Admin.Domain.NHibernate.Mappings
{
    public class ServiceSetMap : ClassMap<ServiceSet>
    {
        public ServiceSetMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            References(x => x.Client).Column("ClientId").Not.Nullable();

            Component<ServiceSetConfiguration>(x => x.Configuration, y =>
            {
                y.Map(x => x.ExtraActiveUserCount).Not.Nullable();
                y.Map(x => x.ExtraTeamCount).Not.Nullable();
                y.Map(x => x.ExtraStorageCount).Not.Nullable();
                y.Map(x => x.ExtraAccountOrganizationCount).Not.Nullable();
                y.Map(x => x.ExtraGigabyteCount).Not.Nullable();
                y.References(x => x.Rate).Column("RateId").Not.Nullable();
            });

            Map(x => x.Cost).Precision(18).Scale(2);
            Map(x => x.MonthCount).Not.Nullable();
            Map(x => x.BaseServiceCost).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.FactualServiceCost).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.BaseCostPerDay).Precision(18).Scale(2);
            Map(x => x.CostPerDayWithDiscount).Precision(18).Scale(2);
            Map(x => x.ActivationDate);
            Map(x => x.StartDate);
            Map(x => x.EndDate);
            Map(x => x.InitialDayCount);

            HasMany(x => x.Services).AsSet().Access.CamelCaseField().KeyColumn("ServiceSetId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");

            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);
        }
    }
}
