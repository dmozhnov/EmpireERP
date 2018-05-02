using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ReceiptWaybillRowMap : ClassMap<ReceiptWaybillRow>
    {
        public ReceiptWaybillRowMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.ArticleMeasureUnitScale).Not.Nullable();

            Map(x => x.PendingCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.PendingSum).Precision(18).Scale(2).Access.CamelCaseField().Not.Nullable();

            Map(x => x.ReceiptedCount).Precision(18).Scale(6).Access.CamelCaseField();
            Map(x => x.ProviderCount).Precision(18).Scale(6).Access.CamelCaseField();
            Map(x => x.ProviderSum).Precision(18).Scale(2).Access.CamelCaseField();

            Map(x => x.UsageAsManualSourceCount).Access.CamelCaseField().Not.Nullable();
            Map(x => x.AreCountDivergencesAfterReceipt).Access.CamelCaseField().Not.Nullable();
            Map(x => x.AreSumDivergencesAfterReceipt).Access.CamelCaseField().Not.Nullable();

            Map(x => x.ApprovedCount).Precision(18).Scale(6).Access.CamelCaseField();
            Map(x => x.ApprovedSum).Precision(18).Scale(2).Access.CamelCaseField();
            Map(x => x.InitialPurchaseCost).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.PurchaseCost).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.ApprovedPurchaseCost).Precision(18).Scale(6).Access.CamelCaseField();
            Map(x => x.CustomsDeclarationNumber).Length(33).Access.CamelCaseField().Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate).Access.CamelCaseField();
            
            Map(x => x.AcceptedCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.ShippedCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.FinallyMovedCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.AvailableToReserveCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();

            References(x => x.RecipientArticleAccountingPrice).Column("RecipientArticleAccountingPriceId").Access.CamelCaseField();
            References(x => x.ReceiptWaybill).Column("ReceiptWaybillId").Not.Nullable();
            References(x => x.Article).Column("ArticleId").Access.CamelCaseField().Not.Nullable();
            References(x => x.PendingValueAddedTax).Column("PendingValueAddedTaxId").Not.Nullable();
            References(x => x.ApprovedValueAddedTax).Column("ApprovedValueAddedTaxId");
            References(x => x.ProductionCountry).Column("CountryId").Not.Nullable();
            References(x => x.Manufacturer).Column("ManufacturerId").Not.Nullable();

            Map(x => x.OrdinalNumber).Not.Nullable();
            Where("DeletionDate is null");

            BatchSize(50);
        }
    }
}
