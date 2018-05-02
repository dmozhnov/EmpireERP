using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class DealPaymentDocumentDistributionToReturnFromClientWaybillMap : SubclassMap<DealPaymentDocumentDistributionToReturnFromClientWaybill>
    {
        public DealPaymentDocumentDistributionToReturnFromClientWaybillMap()
        {
            KeyColumn("Id");

            References(x => x.SaleWaybill).Column("SaleWaybillId").Not.Nullable();
            References(x => x.ReturnFromClientWaybill).Column("ReturnFromClientWaybillId").Not.Nullable();

            BatchSize(10);
        }
    }
}
