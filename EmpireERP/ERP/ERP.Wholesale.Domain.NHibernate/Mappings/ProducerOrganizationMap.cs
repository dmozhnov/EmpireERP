using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProducerOrganizationMap : SubclassMap<ProducerOrganization>
    {
        public ProducerOrganizationMap()
        {
            KeyColumn("Id");

            Map(x => x.DirectorName).Length(100).Not.Nullable();
            Map(x => x.IsManufacturer).Not.Nullable();

            References(x => x.Manufacturer).Column("ManufacturerId").Cascade.All();

            BatchSize(25);
        }
    }
}
