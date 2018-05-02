using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Waybill.SaleWaybill
{
    public class SaleWaybillRowMap : ClassMap<SaleWaybillRow>
    {
        public SaleWaybillRowMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);
            Map(x => x.SellingCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.SalePrice).Precision(18).Scale(2).Access.CamelCaseField();
            
            Map(x => x.AcceptedReturnCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.ReceiptedReturnCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();
            Map(x => x.AvailableToReturnCount).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();

            References(x => x.SaleWaybill).Column("SaleWaybillId").Not.Nullable();
            References(x => x.ValueAddedTax).Column("ValueAddedTaxId").Not.Nullable();
            References(x => x.Article).Column("ArticleId").Access.CamelCaseField().Not.Nullable();

            BatchSize(50);
        }
    }
}
