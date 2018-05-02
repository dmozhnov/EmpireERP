using ERP.Wholesale.Domain.Indicators.PurchaseIndicators;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Indicators.PurchaseIndicators
{
    public class AcceptedPurchaseIndicatorMap : ClassMap<AcceptedPurchaseIndicator> 
    {
        public AcceptedPurchaseIndicatorMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.EndDate);
            Map(x => x.StorageId).Not.Nullable();
            Map(x => x.UserId).Not.Nullable();
            Map(x => x.ContractorId).Not.Nullable();
            Map(x => x.ContractorOrganizationId).Not.Nullable();
            Map(x => x.ContractId);            
            Map(x => x.AccountOrganizationId).Not.Nullable();
            Map(x => x.ArticleId).Not.Nullable();            
            
            Map(x => x.PreviousId);

            Map(x => x.PurchaseCostSum).Not.Nullable().Precision(18).Scale(6);
            Map(x => x.Count).Not.Nullable().Precision(18).Scale(6);

        }
    }
}
