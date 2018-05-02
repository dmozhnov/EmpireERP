using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class DealPaymentFromClientMap : SubclassMap<DealPaymentFromClient>
    {
        public DealPaymentFromClientMap()
        {
            KeyColumn("Id");

            HasMany(x => x.Distributions)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("SourceDealPaymentDocumentId")
                .Inverse().Cascade.AllDeleteOrphan()
                .BatchSize(50);

            BatchSize(10);
        }
    }
}
