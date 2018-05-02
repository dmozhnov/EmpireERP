using ERP.Wholesale.Domain.Indicators;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Indicators
{
    public class ArticleAccountingPriceIndicatorMap : ClassMap<ArticleAccountingPriceIndicator>
    {
        public ArticleAccountingPriceIndicatorMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.EndDate);
            Map(x => x.StorageId).Not.Nullable();
            Map(x => x.ArticleId).Not.Nullable();
            Map(x => x.AccountingPrice).Not.Nullable().Precision(18).Scale(2);
            Map(x => x.AccountingPriceListId).Not.Nullable();
            Map(x => x.ArticleAccountingPriceId).Not.Nullable();
            
            BatchSize(30);
        }
    }
}
