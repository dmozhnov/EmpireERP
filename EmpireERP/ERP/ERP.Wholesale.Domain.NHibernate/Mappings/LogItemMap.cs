using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class LogItemMap : ClassMap<LogItem>
    {
        public LogItemMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Time).Not.Nullable();
            Map(x => x.UserId);
            Map(x => x.Url).Length(4000).Not.Nullable();
            Map(x => x.UserMessage).Length(4000).Not.Nullable();
            Map(x => x.SystemMessage).Length(4000).Not.Nullable();
        }
    }
}
