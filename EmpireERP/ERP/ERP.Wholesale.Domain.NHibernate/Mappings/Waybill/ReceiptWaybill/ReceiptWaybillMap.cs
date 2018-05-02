using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ReceiptWaybillMap : ClassMap<ReceiptWaybill>
    {
        public ReceiptWaybillMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.Number).Length(25).Not.Nullable();
            Map(x => x.Date).Access.CamelCaseField().Not.Nullable();
            Map(x => x.Year).Access.CamelCaseField().Not.Nullable();

            Map(x => x.ProviderNumber).Length(25).Not.Nullable();
            Map(x => x.ProviderDate);
            Map(x => x.ProviderInvoiceNumber).Length(25).Not.Nullable();
            Map(x => x.ProviderInvoiceDate);
            Map(x => x.CustomsDeclarationNumber).Length(33).Not.Nullable();
            Map(x => x.PendingSum).Precision(18).Scale(2).Access.CamelCaseField().Not.Nullable();
            Map(x => x.ApprovedSum).Precision(18).Scale(2);
            Map(x => x.PendingDiscountSum).Precision(18).Scale(2).Access.CamelCaseField().Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate).Access.CamelCaseField();
            Map(x => x.State).CustomType<ReceiptWaybillState>().Column("ReceiptWaybillStateId").Not.Nullable();
            Map(x => x.ReceiptDate);
            Map(x => x.ApprovementDate);
            Map(x => x.AcceptanceDate);
            Map(x => x.Comment).Length(4000).Not.Nullable();
            Map(x => x.IsCustomsDeclarationNumberFromReceiptWaybill).Not.Nullable();

            References(x => x.ProductionOrderBatch).Column("ProductionOrderBatchId");
            References(x => x.Curator).Access.CamelCaseField().Column("CuratorId").Not.Nullable();
            References(x => x.ReceiptStorage).Column("ReceiptWaybillReceiptStorageId").Not.Nullable();
            References(x => x.AccountOrganization).Column("AccountOrganizationId").Not.Nullable();
            References(x => x.Provider).Column("ProviderId");
            References(x => x.ProviderContract).Column("ProviderContractId");
            References(x => x.PendingValueAddedTax).Column("PendingValueAddedTaxId").Not.Nullable();
            References(x => x.CreatedBy).Column("ReceiptWaybillCreatedById").Not.Nullable();
            References(x => x.AcceptedBy).Column("ReceiptWaybillAcceptedById");
            References(x => x.ReceiptedBy).Column("ReceiptWaybillReceiptedById");
            References(x => x.ApprovedBy).Column("ReceiptWaybillApprovedById");

            HasMany(x => x.Rows).AsSet().Access.CamelCaseField().KeyColumn("ReceiptWaybillId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null")
                .BatchSize(50);

            BatchSize(50);
        }
    }
}
