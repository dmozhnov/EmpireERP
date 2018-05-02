using ERP.Wholesale.Domain.Entities;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProductionOrderTransportSheetPaymentMap : SubclassMap<ProductionOrderTransportSheetPayment>
    {
        public ProductionOrderTransportSheetPaymentMap()
        {
            KeyColumn("Id");

            Map(x => x.DeletionDate).Access.CamelCaseField();

            References(x => x.TransportSheet).Column("TransportSheetId").Not.Nullable();
        }
    }
}
