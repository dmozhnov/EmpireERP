using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ArticleAccountingPriceMap : ClassMap<ArticleAccountingPrice>
    {
        public ArticleAccountingPriceMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.AccountingPrice).Precision(18).Scale(2).Access.CamelCaseField().Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate).Access.CamelCaseField();
            Map(x => x.ErrorAccountingPriceCalculation);
            Map(x => x.ErrorLastDigitCalculation);
            Map(x => x.IsOverlappedOnEnd).Not.Nullable();

            References(x => x.AccountingPriceList).Column("AccountingPriceListId").Not.Nullable();
            References(x => x.Article).Column("ArticleId").Access.CamelCaseField().Not.Nullable();

            Map(x => x.OrdinalNumber).Not.Nullable();
            Where("DeletionDate is null");

            BatchSize(50);
        }
    }
}
