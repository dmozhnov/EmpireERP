using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ChangeOwnerWaybillMap : ClassMap<ChangeOwnerWaybill>
    {
        public ChangeOwnerWaybillMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.Number).Length(25).Not.Nullable();
            Map(x => x.Date).Access.CamelCaseField().Not.Nullable();
            Map(x => x.Year).Access.CamelCaseField().Not.Nullable();

            Map(x => x.Comment).Length(4000).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);
            Map(x => x.AcceptanceDate);
            Map(x => x.ChangeOwnerDate);

            Map(x => x.State).CustomType<ChangeOwnerWaybillState>().Column("ChangeOwnerWaybillStateId").Not.Nullable();
            Map(x => x.AccountingPriceSum).Precision(18).Scale(2).Not.Nullable();

            References(x => x.Curator).Access.CamelCaseField().Column("ChangeOwnerWaybillCuratorId").Not.Nullable();
            References(x => x.Storage).Column("ChangeOwnerWaybillStorageId").Not.Nullable();
            References(x => x.Sender).Column("ChangeOwnerWaybillSenderId").Not.Nullable();
            References(x => x.Recipient).Column("ChangeOwnerWaybillRecipientId").Not.Nullable();
            References(x => x.ValueAddedTax).Column("ChangeOwnerWaybillValueAddedTaxId").Not.Nullable();

            References(x => x.CreatedBy).Column("ChangeOwnerWaybillCreatedById").Not.Nullable();
            References(x => x.AcceptedBy).Column("ChangeOwnerWaybillAcceptedById");
            References(x => x.ChangedOwnerBy).Column("ChangeOwnerWaybillChangedOwnerById");

            HasMany(x => x.Rows).AsSet().Access.CamelCaseField().KeyColumn("ChangeOwnerWaybillId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null")
                .BatchSize(50);
        }
    }
}
