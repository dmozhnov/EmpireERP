using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class SaleWaybillMap : ClassMap<SaleWaybill>
    {
        public SaleWaybillMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.Number).Length(25).Not.Nullable();
            Map(x => x.Date).Access.CamelCaseField().Not.Nullable();
            Map(x => x.Year).Access.CamelCaseField().Not.Nullable();

            Map(x => x.Comment).Length(4000).Not.Nullable();
            Map(x => x.DeliveryAddress).Length(250).Not.Nullable();
            Map(x => x.DeliveryAddressType).CustomType<DeliveryAddressType>().Column("DeliveryAddressTypeId").Not.Nullable();
            Map(x => x.IsFullyPaid).Not.Nullable();

            Map(x => x.IsPrepayment).Not.Nullable().Access.CamelCaseField();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate).Column("DeletionDate").Access.CamelCaseField();

            References(x => x.Curator).Access.CamelCaseField().Column("SaleWaybillCuratorId").Not.Nullable();
            References(x => x.CreatedBy).Column("SaleWaybillCreatedById").Not.Nullable();
            References(x => x.AcceptedBy).Column("SaleWaybillAcceptedById");
            
            References(x => x.Quota).Access.CamelCaseField().Column("QuotaId").Not.Nullable();
            References(x => x.Deal).Column("DealId").Not.Nullable();
            References(x => x.ValueAddedTax).Column("ValueAddedTaxId").Not.Nullable();
            References(x => x.Team).Access.CamelCaseField().Column("TeamId").Not.Nullable();

            HasMany(x => x.Rows).AsSet().Access.CamelCaseField().KeyColumn("SaleWaybillId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null")
                .BatchSize(50);
            HasMany(x => x.Distributions).AsSet().Access.CamelCaseField().KeyColumn("SaleWaybillId").Inverse().Cascade.SaveUpdate()
                .BatchSize(50);

            BatchSize(10);
        }
    }
}
