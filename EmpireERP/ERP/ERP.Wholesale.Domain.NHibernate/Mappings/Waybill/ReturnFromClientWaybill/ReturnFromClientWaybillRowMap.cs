using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ReturnFromClientRowMap : ClassMap<ReturnFromClientWaybillRow>
    {
        public ReturnFromClientRowMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();
            Map(x => x.ReturnCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);

            Map(x => x.AcceptedCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.ShippedCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.FinallyMovedCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.AvailableToReserveCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.UsageAsManualSourceCount).Access.CamelCaseField().Not.Nullable();

            References(x => x.ArticleAccountingPrice).Column("ReturnFromClientArticleAccountingPriceId").Access.CamelCaseField();
            References(x => x.ReturnFromClientWaybill).Column("ReturnFromClientWaybillId").Not.Nullable();
            References(x => x.SaleWaybillRow).Column("SaleWaybillRowId").Access.CamelCaseField().Not.Nullable();
            References(x => x.ReceiptWaybillRow).Column("ReceiptWaybillRowId").Access.CamelCaseField().Not.Nullable();
            References(x => x.Article).Column("ArticleId").Access.CamelCaseField().Not.Nullable();

            Where("DeletionDate is null");
        }
    }
}
