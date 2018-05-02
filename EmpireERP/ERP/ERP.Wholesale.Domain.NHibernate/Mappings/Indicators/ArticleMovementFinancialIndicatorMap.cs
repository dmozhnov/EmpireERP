using ERP.Wholesale.Domain.Indicators;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Indicators
{
    public class ArticleMovementFactualFinancialIndicatorMap : ClassMap<ArticleMovementFactualFinancialIndicator>
    {
        public ArticleMovementFactualFinancialIndicatorMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.EndDate);
            Map(x => x.RecipientId);
            Map(x => x.RecipientStorageId);
            Map(x => x.SenderId);
            Map(x => x.SenderStorageId);
            Map(x => x.PreviousId);
            Map(x => x.WaybillId).Not.Nullable();
            Map(x => x.ArticleMovementOperationType).Not.Nullable();

            Map(x => x.PurchaseCostSum).Not.Nullable().Precision(18).Scale(6);
            Map(x => x.AccountingPriceSum).Not.Nullable().Precision(18).Scale(2);
            Map(x => x.SalePriceSum).Not.Nullable().Precision(18).Scale(2);
        }
    }
}