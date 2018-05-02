using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class DealPaymentToClientMap : SubclassMap<DealPaymentToClient>
    {
        public DealPaymentToClientMap()
        {
            KeyColumn("Id");

            HasMany(x => x.ConcreteDistributions)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("DestinationDealPaymentDocumentId")
                .Inverse().Cascade.AllDeleteOrphan()
                .BatchSize(10);
        }
    }
}
