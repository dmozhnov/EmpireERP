using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ExpenditureWaybillMap : SubclassMap<ExpenditureWaybill>
    {
        public ExpenditureWaybillMap()
        {
            KeyColumn("Id");

            Map(x => x.State).CustomType<ExpenditureWaybillState>().Column("ExpenditureWaybillStateId").Not.Nullable();            
            Map(x => x.AcceptanceDate);
            Map(x => x.ShippingDate);
            Map(x => x.SenderAccountingPriceSum).Precision(18).Scale(2);
            Map(x => x.SalePriceSum).Precision(18).Scale(2);
            Map(x => x.RoundSalePrice);
                        
            References(x => x.SenderStorage).Column("ExpenditureWaybillSenderStorageId").Not.Nullable();
            References(x => x.ShippedBy).Column("ExpenditureWaybillShippedById");
        }
    }
}
