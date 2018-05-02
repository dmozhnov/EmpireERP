using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Misc;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProductionOrderMap : ClassMap<ProductionOrder>
    {
        public ProductionOrderMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.Date).Not.Nullable();
            Map(x => x.Name).Length(200).Unique().Not.Nullable();
            Map(x => x.IsClosed).Not.Nullable();
            Map(x => x.WorkDaysPlanScheme).Not.Nullable();
            Map(x => x.Comment).Length(4000).Not.Nullable();

            Map(x => x.ProductionOrderPlannedProductionExpensesInCurrency).Precision(14).Scale(2).Not.Nullable();
            Map(x => x.ProductionOrderPlannedTransportationExpensesInCurrency).Precision(14).Scale(2).Not.Nullable();
            Map(x => x.ProductionOrderPlannedExtraExpensesInCurrency).Precision(14).Scale(2).Not.Nullable();
            Map(x => x.ProductionOrderPlannedCustomsExpensesInCurrency).Precision(14).Scale(2).Not.Nullable();

            Map(x => x.ArticleTransportingPrimeCostCalculationType).CustomType<ProductionOrderArticleTransportingPrimeCostCalculationType>()
                .Column("ArticleTransportingPrimeCostCalculationTypeId").Access.CamelCaseField().Not.Nullable();

            References(x => x.Storage).Column("StorageId").Not.Nullable();
            References(x => x.Currency).Column("CurrencyId").Access.CamelCaseField().Not.Nullable();
            References(x => x.CurrencyRate).Column("CurrencyRateId").Access.CamelCaseField();
            References(x => x.Producer).Column("ProducerId").Not.Nullable();
            References(x => x.CreatedBy).Column("CreatedById").Not.Nullable();
            References(x => x.Curator).Column("CuratorId").Not.Nullable();
            References(x => x.Contract).Column("ProducerContractId");

            HasMany(x => x.Batches).AsSet().Access.CamelCaseField().KeyColumn("ProductionOrderId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");
            HasMany(x => x.Payments).AsSet().Access.CamelCaseField().KeyColumn("ProductionOrderId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");
            HasMany(x => x.PlannedPayments).AsSet().Access.CamelCaseField().KeyColumn("ProductionOrderId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");
            HasMany(x => x.TransportSheets).AsSet().Access.CamelCaseField().KeyColumn("ProductionOrderId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");
            HasMany(x => x.ExtraExpensesSheets).AsSet().Access.CamelCaseField().KeyColumn("ProductionOrderId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");
            HasMany(x => x.CustomsDeclarations).AsSet().Access.CamelCaseField().KeyColumn("ProductionOrderId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");
            HasMany(x => x.MaterialsPackages).AsSet().Access.CamelCaseField().KeyColumn("ProductionOrderId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");
        }
    }
}
