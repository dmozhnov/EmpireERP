﻿using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Indicators
{
    public class ShippedSaleIndicatorMap : ClassMap<ShippedSaleIndicator>
    {
        public ShippedSaleIndicatorMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.EndDate);
            Map(x => x.StorageId).Not.Nullable();
            Map(x => x.UserId).Not.Nullable();
            Map(x => x.ContractorId).Not.Nullable();
            Map(x => x.ClientOrganizationId).Not.Nullable();
            Map(x => x.TeamId).Not.Nullable();
            Map(x => x.ClientId).Not.Nullable();
            Map(x => x.AccountOrganizationId).Not.Nullable();
            Map(x => x.ArticleId).Not.Nullable();

            Map(x => x.BatchId).Not.Nullable();
            Map(x => x.DealId).Not.Nullable();
            Map(x => x.PreviousId);

            Map(x => x.PurchaseCostSum).Not.Nullable().Precision(18).Scale(6);
            Map(x => x.AccountingPriceSum).Not.Nullable().Precision(18).Scale(2);
            Map(x => x.SalePriceSum).Not.Nullable().Precision(18).Scale(2);
            Map(x => x.SoldCount).Not.Nullable().Precision(18).Scale(6);
        }
    }
}