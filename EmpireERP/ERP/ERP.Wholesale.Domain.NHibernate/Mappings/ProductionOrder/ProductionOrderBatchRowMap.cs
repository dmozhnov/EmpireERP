using ERP.Wholesale.Domain.Entities;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProductionOrderBatchRowMap : ClassMap<ProductionOrderBatchRow>
    {
        public ProductionOrderBatchRowMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.ArticleMeasureUnitScale).Not.Nullable();

            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate).Access.CamelCaseField();
            Map(x => x.Count).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.ProductionOrderBatchRowCostInCurrency).Precision(18).Scale(6).Not.Nullable();
            Map(x => x.PackLength).Not.Nullable().Precision(6).Scale(0);
            Map(x => x.PackHeight).Not.Nullable().Precision(6).Scale(0);
            Map(x => x.PackWidth).Not.Nullable().Precision(6).Scale(0);
            Map(x => x.PackWeight).Not.Nullable().Precision(8).Scale(3);
            Map(x => x.PackVolume).Not.Nullable().Precision(15).Scale(6);
            Map(x => x.PackSize).Not.Nullable().Precision(12).Scale(6);

            References(x => x.Article).Column("ArticleId").Access.CamelCaseField().Not.Nullable();
            References(x => x.Batch).Column("BatchId").Not.Nullable();
            References(x => x.Currency).Column("CurrencyId").Not.Nullable();
            References(x => x.Manufacturer).Column("ManufacturerId").Access.CamelCaseField().Not.Nullable();
            References(x => x.ProductionCountry).Column("ProductionCountryId").Not.Nullable();
            References(x => x.ReceiptWaybillRow).Column("ReceiptWaybillRowId");

            Map(x => x.OrdinalNumber).Not.Nullable();
            Where("DeletionDate is null");
        }
    }
}
