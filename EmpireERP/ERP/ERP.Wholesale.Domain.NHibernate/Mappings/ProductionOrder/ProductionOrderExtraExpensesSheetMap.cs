using ERP.Wholesale.Domain.Entities;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProductionOrderExtraExpensesSheetMap : ClassMap<ProductionOrderExtraExpensesSheet>
    {
        public ProductionOrderExtraExpensesSheetMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.ExtraExpensesContractorName).Length(200).Not.Nullable();
            Map(x => x.Date).Access.CamelCaseField().Not.Nullable();
            Map(x => x.CurrencyDeterminationType).CustomType<ProductionOrderCurrencyDeterminationType>()
                .Column("ProductionOrderCurrencyDeterminationTypeId").Access.CamelCaseField().Not.Nullable();
            Map(x => x.CostInCurrency).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.ExtraExpensesPurpose).Length(200).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate).Access.CamelCaseField();
            Map(x => x.Comment).Length(4000).Not.Nullable();

            References(x => x.ProductionOrder).Column("ProductionOrderId").Not.Nullable();
            References(x => x.Currency).Column("CurrencyId").Access.CamelCaseField();
            References(x => x.CurrencyRate).Column("CurrencyRateId").Access.CamelCaseField();

            HasMany(x => x.Payments).AsSet().Access.CamelCaseField().KeyColumn("ExtraExpensesSheetId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");
        }
    }
}
