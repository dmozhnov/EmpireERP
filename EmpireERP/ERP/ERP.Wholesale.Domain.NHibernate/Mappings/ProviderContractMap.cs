using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProviderContractMap : SubclassMap<ProviderContract>
    {
        public ProviderContractMap()
        {
            KeyColumn("Id");

            BatchSize(50);
        }
    }
}
