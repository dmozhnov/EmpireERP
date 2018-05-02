using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ArticleMap : ClassMap<Article>
    {
        public ArticleMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.FullName).Length(200).Not.Nullable();
            Map(x => x.ShortName).Length(200).Not.Nullable();
            Map(x => x.Number).Length(30).Unique().Not.Nullable();
            Map(x => x.ManufacturerNumber).Length(30).Not.Nullable();

            References(x => x.ArticleGroup).Column("ArticleGroupId").Not.Nullable();
            References(x => x.Trademark).Column("TradeMarkId");
            References(x => x.Manufacturer).Column("ManufacturerId");
            References(x => x.ProductionCountry).Column("ProductionCountryId");
            References(x => x.MeasureUnit).Column("MeasureUnitId").Not.Nullable();
            References(x => x.Certificate).Column("CertificateId");

            Map(x => x.PackSize).Access.CamelCaseField().Precision(12).Scale(6).Not.Nullable();
            Map(x => x.PackWeight).Not.Nullable().Precision(8).Scale(3);
            Map(x => x.PackHeight).Access.CamelCaseField().Not.Nullable();
            Map(x => x.PackLength).Access.CamelCaseField().Not.Nullable();
            Map(x => x.PackWidth).Access.CamelCaseField().Not.Nullable();
            Map(x => x.PackVolume).Access.CamelCaseField().Precision(15).Scale(6).Not.Nullable();
            Map(x => x.IsObsolete).Not.Nullable();
            //TODO: переделать на верблюжий горб, и нет проблем
            Map(x => x.IsSalaryPercentFromGroup).Not.Nullable();//IsSalaryPercentFromGroup должен стоять ПЕРЕД SalaryPercent
            Map(x => x.SalaryPercent).Precision(4).Scale(2).Not.Nullable();//иначе не будет работать получение SalaryPercent при IsSalaryPercentFromGroup = false
            Map(x => x.MarkupPercent).Not.Nullable().Precision(6).Scale(2);
            Map(x => x.Comment).Length(4000).Not.Nullable();

            BatchSize(50);
        }
    }
}
