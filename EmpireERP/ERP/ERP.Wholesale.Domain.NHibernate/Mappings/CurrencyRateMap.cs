using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class CurrencyRateMap : ClassMap<CurrencyRate>
    {
        public CurrencyRateMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.EndDate);
            Map(x => x.Rate).Precision(18).Scale(6).Access.CamelCaseField().Not.Nullable();

            References(x => x.PreviousCurrencyRate).Access.CamelCaseField().Column("PreviousCurrencyRateId");
            References(x => x.Currency).Column("CurrencyId").Not.Nullable();
            References(x => x.BaseCurrency).Column("BaseCurrencyId").Not.Nullable();
        }
    }
}
