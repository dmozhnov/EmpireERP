using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class AccountingPriceListWaybillTakingMap : ClassMap<AccountingPriceListWaybillTaking>
    {
        public AccountingPriceListWaybillTakingMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();
            Map(x => x.TakingDate).Not.Nullable();
            Map(x => x.IsWaybillRowIncoming).Not.Nullable();
            Map(x => x.ArticleAccountingPriceId).Not.Nullable();
            Map(x => x.WaybillType).CustomType<WaybillType>().Column("WaybillTypeId").Not.Nullable();
            Map(x => x.WaybillRowId).Not.Nullable();
            Map(x => x.ArticleId).Not.Nullable();
            Map(x => x.StorageId).Not.Nullable();
            Map(x => x.AccountOrganizationId).Not.Nullable();
            Map(x => x.AccountingPrice).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.IsOnAccountingPriceListStart).Not.Nullable();
            Map(x => x.Count).Precision(18).Scale(6).Not.Nullable();
            Map(x => x.RevaluationDate);

            BatchSize(30);
        }
    }
}
