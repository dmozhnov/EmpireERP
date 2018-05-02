using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class WriteoffWaybillRowMap : ClassMap<WriteoffWaybillRow>
    {
        public WriteoffWaybillRowMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);
            Map(x => x.OutgoingWaybillRowState).CustomType<OutgoingWaybillRowState>().Column("WriteoffOutgoingWaybillRowStateId").Access.CamelCaseField().Not.Nullable();
            Map(x => x.WritingoffCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            
            Map(x => x.IsUsingManualSource).Not.Nullable();

            References(x => x.WriteoffWaybill).Column("WriteoffWaybillId").Not.Nullable();
            References(x => x.SenderArticleAccountingPrice).Column("WriteoffSenderArticleAccountingPriceId").Access.CamelCaseField();
            References(x => x.ReceiptWaybillRow).Column("WriteoffReceiptWaybillRowId").Access.CamelCaseField().Not.Nullable();
            References(x => x.Article).Column("ArticleId").Access.CamelCaseField().Not.Nullable();

            Where("DeletionDate is null");
        }
    }
}
