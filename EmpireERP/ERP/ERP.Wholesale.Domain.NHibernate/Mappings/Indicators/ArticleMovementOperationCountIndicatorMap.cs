using ERP.Wholesale.Domain.Indicators;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Indicators
{
    public class ArticleMovementOperationCountIndicatorMap : ClassMap<ArticleMovementOperationCountIndicator>
    {
        public ArticleMovementOperationCountIndicatorMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.EndDate);
            Map(x => x.PreviousId);
            Map(x => x.ArticleMovementOperationType).Not.Nullable();
            Map(x => x.StorageId).Not.Nullable();
            Map(x => x.Count).Not.Nullable().Precision(18).Scale(6);
        }
    }
}