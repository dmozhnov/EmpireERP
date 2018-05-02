using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class DealPaymentDocumentDistributionToDealPaymentDocumentMap : SubclassMap<DealPaymentDocumentDistributionToDealPaymentDocument>
    {
        public DealPaymentDocumentDistributionToDealPaymentDocumentMap()
        {
            KeyColumn("Id");

            References(x => x.DestinationDealPaymentDocument).Column("DestinationDealPaymentDocumentId").Not.Nullable();
            References(x => x.SourceDistributionToReturnFromClientWaybill).Column("SourceDistributionToReturnFromClientWaybillId");

            BatchSize(10);
        }
    }
}
