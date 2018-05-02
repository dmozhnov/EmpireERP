using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ReturnFromClientWaybillMap : ClassMap<ReturnFromClientWaybill>
    {
        public ReturnFromClientWaybillMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();
            Map(x => x.Number).Length(25).Not.Nullable();
            Map(x => x.Date).Access.CamelCaseField().Not.Nullable();
            Map(x => x.Year).Access.CamelCaseField().Not.Nullable();

            Map(x => x.State).CustomType<ReturnFromClientWaybillState>().Column("ReturnFromClientWaybillStateId").Not.Nullable();

            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);
            Map(x => x.AcceptanceDate);
            Map(x => x.ReceiptDate);
            Map(x => x.RecipientAccountingPriceSum).Precision(18).Scale(2).Access.CamelCaseField();
            Map(x => x.SalePriceSum).Precision(18).Scale(2);
            Map(x => x.Comment).Length(4000).Not.Nullable();

            References(x => x.Team).Access.CamelCaseField().Column("TeamId").Not.Nullable();
            References(x => x.Curator).Access.CamelCaseField().Column("ReturnFromClientWaybillCuratorId").Not.Nullable();
            References(x => x.CreatedBy).Column("ReturnFromClientWaybillCreatedById").Not.Nullable();
            References(x => x.AcceptedBy).Column("ReturnFromClientWaybillAcceptedById");
            References(x => x.ReceiptedBy).Column("ReturnFromClientWaybillReceiptedById");
            References(x => x.Recipient).Column("ReturnFromClientWaybillRecipientId").Not.Nullable();
            References(x => x.Deal).Column("ReturnFromClientWaybillDealId").Not.Nullable();
            References(x => x.RecipientStorage).Column("ReturnFromClientWaybillRecipientStorageId").Not.Nullable();
            References(x => x.ReturnFromClientReason).Column("ReturnFromClientReasonId").Not.Nullable();

            HasMany(x => x.Rows).AsSet().Access.CamelCaseField().KeyColumn("ReturnFromClientWaybillId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null")
                .BatchSize(50);
            HasMany(x => x.Distributions).AsSet().Access.CamelCaseField().KeyColumn("ReturnFromClientWaybillId").Inverse().Cascade.SaveUpdate();
        }
    }
}
