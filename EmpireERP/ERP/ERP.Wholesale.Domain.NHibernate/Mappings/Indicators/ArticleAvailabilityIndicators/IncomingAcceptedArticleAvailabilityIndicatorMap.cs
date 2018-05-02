using ERP.Wholesale.Domain.Indicators;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Indicators
{
    public class IncomingAcceptedArticleAvailabilityIndicatorMap : ClassMap<IncomingAcceptedArticleAvailabilityIndicator>
    {
        public IncomingAcceptedArticleAvailabilityIndicatorMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();
            
            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.EndDate);
            Map(x => x.StorageId).Not.Nullable();
            Map(x => x.AccountOrganizationId).Not.Nullable();
            Map(x => x.ArticleId).Not.Nullable();
            Map(x => x.BatchId).Not.Nullable();
            Map(x => x.PurchaseCost).Not.Nullable().Precision(18).Scale(6);
            Map(x => x.Count).Not.Nullable().Precision(18).Scale(6);
            Map(x => x.PreviousId);
        }
    }
}
