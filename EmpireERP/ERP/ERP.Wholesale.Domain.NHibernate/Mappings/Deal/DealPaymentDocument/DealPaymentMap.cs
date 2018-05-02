using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class DealPaymentMap : SubclassMap<DealPayment>
    {
        public DealPaymentMap()
        {
            KeyColumn("Id");

            Map(x => x.PaymentDocumentNumber).Length(50).Not.Nullable();
            Map(x => x.DealPaymentForm).CustomType<DealPaymentForm>().Column("DealPaymentFormId").Access.CamelCaseField().Not.Nullable();

            BatchSize(10);
        }
    }
}
