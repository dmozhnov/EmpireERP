using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class WriteoffWaybillMap : ClassMap<WriteoffWaybill>
    {
        public WriteoffWaybillMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();
            
            Map(x => x.Number).Length(25).Not.Nullable();
            Map(x => x.Date).Access.CamelCaseField().Not.Nullable();
            Map(x => x.Year).Access.CamelCaseField().Not.Nullable();

            Map(x => x.Comment).Length(4000).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);
            Map(x => x.WriteoffDate);
            Map(x => x.AcceptanceDate);
            Map(x => x.SenderAccountingPriceSum).Precision(18).Scale(2);
            Map(x => x.State).CustomType<WriteoffWaybillState>().Column("WriteoffWaybillStateId").Not.Nullable();

            References(x => x.Curator).Access.CamelCaseField().Column("WriteoffWaybillCuratorId").Not.Nullable();
            References(x => x.Sender).Column("WriteoffWaybillSenderId").Not.Nullable();
            References(x => x.SenderStorage).Column("WriteoffWaybillSenderStorageId").Not.Nullable();
            References(x => x.WriteoffReason).Column("WriteoffWaybillReasonId").Not.Nullable();
            HasMany(x => x.Rows).AsSet().Access.CamelCaseField().KeyColumn("WriteoffWaybillId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null")
                .BatchSize(50);
            References(x => x.CreatedBy).Column("WriteoffWaybillCreatedById").Not.Nullable();
            References(x => x.AcceptedBy).Column("WriteoffWaybillAcceptedById");
            References(x => x.WrittenoffBy).Column("WriteoffWaybillWrittenoffById");
        }
    }
}
