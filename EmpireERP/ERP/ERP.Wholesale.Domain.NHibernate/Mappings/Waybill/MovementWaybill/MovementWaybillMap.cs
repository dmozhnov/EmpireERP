using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class MovementWaybillMap : ClassMap<MovementWaybill>
    {
        public MovementWaybillMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.Number).Length(25).Not.Nullable();
            Map(x => x.Date).Access.CamelCaseField().Not.Nullable();
            Map(x => x.Year).Access.CamelCaseField().Not.Nullable();

            Map(x => x.State).CustomType<MovementWaybillState>().Column("MovementWaybillStateId").Not.Nullable();
            Map(x => x.SenderAccountingPriceSum).Precision(18).Scale(2);
            Map(x => x.RecipientAccountingPriceSum).Precision(18).Scale(2);
            Map(x => x.Comment).Length(4000).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate).Access.CamelCaseField();
            Map(x => x.ShippingDate);
            Map(x => x.ReceiptDate);
            Map(x => x.AcceptanceDate);

            References(x => x.Curator).Access.CamelCaseField().Column("CuratorId").Not.Nullable();
            References(x => x.SenderStorage).Column("SenderStorageId").Not.Nullable();
            References(x => x.Sender).Column("SenderId");
            References(x => x.RecipientStorage).Column("RecipientStorageId").Not.Nullable();
            References(x => x.Recipient).Column("RecipientId").Not.Nullable();
            References(x => x.ValueAddedTax).Column("ValueAddedTaxId").Not.Nullable();

            References(x => x.CreatedBy).Column("MovementWaybillCreatedById").Not.Nullable();
            References(x => x.AcceptedBy).Column("MovementWaybillAcceptedById");
            References(x => x.ShippedBy).Column("MovementWaybillShippedById");
            References(x => x.ReceiptedBy).Column("MovementWaybillReceiptedById");
            
            HasMany(x => x.Rows).AsSet().Access.CamelCaseField().KeyColumn("MovementWaybillId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null")
                .BatchSize(50);
        }
    }
}
