using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class BaseTaskHistoryItemMap: ClassMap<BaseTaskHistoryItem>
    {
        public BaseTaskHistoryItemMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.CreationDate).Not.Nullable();
            References(x => x.CreatedBy).Column("CreatedById").Not.Nullable();
        }
    }
}
