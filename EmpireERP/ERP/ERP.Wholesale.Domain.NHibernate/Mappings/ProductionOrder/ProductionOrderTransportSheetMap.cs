using ERP.Wholesale.Domain.Entities;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProductionOrderTransportSheetMap : ClassMap<ProductionOrderTransportSheet>
    {
        public ProductionOrderTransportSheetMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.ForwarderName).Length(200).Not.Nullable();
            Map(x => x.RequestDate).Access.CamelCaseField().Not.Nullable();
            Map(x => x.ShippingDate).Access.CamelCaseField();
            Map(x => x.PendingDeliveryDate).Access.CamelCaseField();
            Map(x => x.ActualDeliveryDate).Access.CamelCaseField();
            Map(x => x.CurrencyDeterminationType).CustomType<ProductionOrderCurrencyDeterminationType>()
                .Column("ProductionOrderCurrencyDeterminationTypeId").Access.CamelCaseField().Not.Nullable();
            Map(x => x.CostInCurrency).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.BillOfLadingNumber).Length(100).Not.Nullable();
            Map(x => x.ShippingLine).Length(100).Not.Nullable();
            Map(x => x.PortDocumentNumber).Length(100).Not.Nullable();
            Map(x => x.PortDocumentDate);
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate).Access.CamelCaseField();
            Map(x => x.Comment).Length(4000).Not.Nullable();

            References(x => x.ProductionOrder).Column("ProductionOrderId").Not.Nullable();
            References(x => x.Currency).Column("CurrencyId").Access.CamelCaseField();
            References(x => x.CurrencyRate).Column("CurrencyRateId").Access.CamelCaseField();

            HasMany(x => x.Payments).AsSet().Access.CamelCaseField().KeyColumn("TransportSheetId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");
        }
    }
}
