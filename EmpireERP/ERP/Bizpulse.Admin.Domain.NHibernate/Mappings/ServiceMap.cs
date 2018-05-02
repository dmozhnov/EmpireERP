using Bizpulse.Admin.Domain.Entities;
using FluentNHibernate.Mapping;

namespace Bizpulse.Admin.Domain.NHibernate.Mappings
{
    public class ServiceMap : ClassMap<Service>
    {
        public ServiceMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            References(x => x.ServiceSet).Column("ServiceSetId").Not.Nullable();

            Map(x => x.Name).Length(200).Not.Nullable();
            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.EndDate);
            Map(x => x.FactualCost).Precision(18).Scale(2);
            Map(x => x.DeletionDate);
        }
    }
}
