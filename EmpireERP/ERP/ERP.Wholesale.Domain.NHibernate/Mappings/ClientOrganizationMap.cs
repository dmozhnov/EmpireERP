using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ClientOrganizationMap : SubclassMap<ClientOrganization>
    {
        public ClientOrganizationMap()
        {            
            KeyColumn("Id");

            BatchSize(25);
        }
    }
}
