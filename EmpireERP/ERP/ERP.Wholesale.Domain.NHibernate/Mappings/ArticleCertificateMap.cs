using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ArticleCertificateMap : ClassMap<ArticleCertificate>
    {
        public ArticleCertificateMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Length(500).Not.Nullable();
            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.EndDate);
        }
    }
}
