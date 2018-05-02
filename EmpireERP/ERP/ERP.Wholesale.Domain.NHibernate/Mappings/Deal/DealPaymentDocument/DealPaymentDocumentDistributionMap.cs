using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class DealPaymentDocumentDistributionMap : ClassMap<DealPaymentDocumentDistribution>
    {
        public DealPaymentDocumentDistributionMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.Sum).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DistributionDate).Not.Nullable();
            Map(x => x.OrdinalNumber).Not.Nullable();

            References(x => x.SourceDealPaymentDocument).Column("SourceDealPaymentDocumentId").Not.Nullable();

            BatchSize(10);
        }
    }
}
