using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ExpenditureWaybillRowMap : SubclassMap<ExpenditureWaybillRow>
    {
        public ExpenditureWaybillRowMap()
        {
            KeyColumn("Id");
            
            Map(x => x.IsUsingManualSource).Not.Nullable();

            Map(x => x.OutgoingWaybillRowState).CustomType<OutgoingWaybillRowState>().Column("OutgoingWaybillRowStateId").Access.CamelCaseField().Not.Nullable();

            References(x => x.ReceiptWaybillRow).Column("ExpenditureWaybillRowReceiptWaybillRowId").Access.CamelCaseField().Not.Nullable();
            References(x => x.SenderArticleAccountingPrice).Column("ExpenditureWaybillSenderArticleAccountingPriceId").Access.CamelCaseField();

            BatchSize(50);
        }
    }
}
