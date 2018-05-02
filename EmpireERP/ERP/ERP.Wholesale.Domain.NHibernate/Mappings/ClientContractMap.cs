using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ClientContractMap : SubclassMap<ClientContract>
    {
        public ClientContractMap()
        {
            KeyColumn("Id");

            BatchSize(50);
        }
    }
}
