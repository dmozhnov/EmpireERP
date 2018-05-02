using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProviderOrganizationMap: SubclassMap<ProviderOrganization>
    {
        public ProviderOrganizationMap()
        {            
            KeyColumn("Id");

            BatchSize(25);
        }
    }
}
