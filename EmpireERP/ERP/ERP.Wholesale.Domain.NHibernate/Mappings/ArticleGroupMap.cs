using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ArticleGroupMap: ClassMap<ArticleGroup>
    {
        public ArticleGroupMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Length(200).Not.Nullable();
            Map(x => x.Comment).Length(4000).Not.Nullable();
            Map(x => x.MarkupPercent).Not.Nullable().Precision(6).Scale(2);
            Map(x => x.SalaryPercent).Not.Nullable().Precision(4).Scale(2);
            Map(x => x.NameFor1C).Length(200).Not.Nullable();

            References(x => x.Parent).Column("ParentId");
            HasMany(x => x.Childs).AsSet()
                .Access.CamelCaseField().Inverse()
                .KeyColumn("ParentId").Cascade.Delete()
                .BatchSize(30);
            
            BatchSize(30);
        }
    }
}
