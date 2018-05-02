using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class DealPaymentDocumentDistributionToSaleWaybillMap : SubclassMap<DealPaymentDocumentDistributionToSaleWaybill>
    {
        public DealPaymentDocumentDistributionToSaleWaybillMap()
        {
            KeyColumn("Id");

            References(x => x.SaleWaybill).Column("SaleWaybillId").Not.Nullable();
            References(x => x.SourceDistributionToReturnFromClientWaybill).Column("SourceDistributionToReturnFromClientWaybillId");

            BatchSize(10);
        }
    }
}
