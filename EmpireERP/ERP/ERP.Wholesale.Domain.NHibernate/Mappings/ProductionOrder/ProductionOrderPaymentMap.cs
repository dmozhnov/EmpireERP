using ERP.Wholesale.Domain.Entities;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProductionOrderPaymentMap : ClassMap<ProductionOrderPayment>
    {
        public ProductionOrderPaymentMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.Type).CustomType<ProductionOrderPaymentType>().Column("ProductionOrderPaymentTypeId").Access.CamelCaseField().Not.Nullable();
            Map(x => x.PaymentDocumentNumber).Not.Nullable();
            Map(x => x.Date).Access.CamelCaseField().Not.Nullable();
            Map(x => x.SumInCurrency).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.Form).CustomType<ProductionOrderPaymentForm>().Column("ProductionOrderPaymentFormId").Access.CamelCaseField().Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate).Access.CamelCaseField();
            Map(x => x.Comment).Length(4000).Not.Nullable();

            References(x => x.ProductionOrder).Column("ProductionOrderId").Not.Nullable();
            References(x => x.ProductionOrderPlannedPayment).Column("ProductionOrderPlannedPaymentId").Access.CamelCaseField();
            References(x => x.CurrencyRate).Column("CurrencyRateId").Not.Nullable();
        }
    }
}
