using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Waybill.ChangeOwnerWaybill
{
    public class ChangeOwnerWaybillRowMap : ClassMap<ChangeOwnerWaybillRow>
    {
        public ChangeOwnerWaybillRowMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.MovingCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);

            Map(x => x.UsageAsManualSourceCount).Access.CamelCaseField().Not.Nullable();
            Map(x => x.IsUsingManualSource).Not.Nullable();

            Map(x => x.AcceptedCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.ShippedCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();            
            Map(x => x.FinallyMovedCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.AvailableToReserveCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.OutgoingWaybillRowState).CustomType<OutgoingWaybillRowState>().Column("OutgoingWaybillRowStateId").Access.CamelCaseField().Not.Nullable();

            References(x => x.ChangeOwnerWaybill).Column("ChangeOwnerWaybillId").Not.Nullable();
            References(x => x.ReceiptWaybillRow).Column("ChangeOwnerWaybillRowReceiptWaybillRowId").Access.CamelCaseField().Not.Nullable();
            References(x => x.ArticleAccountingPrice).Column("ChangeOwnerWaybillRowArticleAccountingPriceId").Access.CamelCaseField();
            References(x => x.ValueAddedTax).Column("ChangeOwnerWaybillRowValueAddedTaxId").Not.Nullable();
            References(x => x.Article).Column("ArticleId").Access.CamelCaseField().Not.Nullable();
            
            Where("DeletionDate is null");
        }
    }
}
