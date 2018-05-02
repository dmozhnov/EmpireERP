using ERP.Wholesale.Domain.Entities;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProductionOrderPlannedPaymentMap : ClassMap<ProductionOrderPlannedPayment>
    {
        public ProductionOrderPlannedPaymentMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.StartDate).Access.CamelCaseField().Not.Nullable();
            Map(x => x.EndDate).Access.CamelCaseField().Not.Nullable();
            Map(x => x.SumInCurrency).Precision(18).Scale(2).Access.CamelCaseField().Not.Nullable();
            Map(x => x.Purpose).Length(50).Access.CamelCaseField().Not.Nullable();
            Map(x => x.PaymentType).CustomType<ProductionOrderPaymentType>().Column("ProductionOrderPaymentTypeId").Access.CamelCaseField().Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate).Access.CamelCaseField();
            Map(x => x.Comment).Length(4000).Not.Nullable();

            References(x => x.ProductionOrder).Column("ProductionOrderId").Not.Nullable();
            References(x => x.Currency).Access.CamelCaseField().Column("CurrencyId").Not.Nullable();
            References(x => x.CurrencyRate).Access.CamelCaseField().Column("CurrencyRateId");

            HasMany(x => x.Payments).AsSet().Access.CamelCaseField().KeyColumn("ProductionOrderPlannedPaymentId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");
        }
    }
}
