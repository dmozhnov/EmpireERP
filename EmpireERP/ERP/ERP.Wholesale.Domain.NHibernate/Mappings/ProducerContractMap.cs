using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProducerContractMap : SubclassMap<ProducerContract>
    {
        public ProducerContractMap()
        {
            KeyColumn("Id");            
        }
    }
}
