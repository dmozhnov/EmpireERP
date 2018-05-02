using ERP.Wholesale.Domain.Indicators;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Indicators
{
    public class AcceptedArticleRevaluationIndicatorMap : ClassMap<AcceptedArticleRevaluationIndicator>
    {
        public AcceptedArticleRevaluationIndicatorMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.EndDate);

            Map(x => x.StorageId).Not.Nullable();
            Map(x => x.AccountOrganizationId).Not.Nullable();
            Map(x => x.RevaluationSum).Precision(18).Scale(2).Not.Nullable();

            Map(x => x.PreviousId);

        }
    }
}
