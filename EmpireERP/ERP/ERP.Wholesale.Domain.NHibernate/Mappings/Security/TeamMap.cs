using ERP.Wholesale.Domain.Entities.Security;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Security
{
    public class TeamMap : ClassMap<Team>
    {
        public TeamMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Length(200).Not.Nullable();
            Map(x => x.Comment).Length(4000).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);

            References(x => x.CreatedBy).Column("CreatedById").Not.Nullable();

            //Эта связь поднята над остальными, т.к. иначе конвенции ошибаются и неверно генерируют внешние ключи.
            HasManyToMany(x => x.Deals)
                .AsSet().Access.CamelCaseField()
                .Table("TeamDeal")
                .ParentKeyColumn("TeamId")
                .ChildKeyColumn("DealId").Cascade.All()
                .BatchSize(50);

            HasManyToMany(x => x.Users)
                .AsSet().Access.CamelCaseField()
                .Table("UserTeam")
                .ParentKeyColumn("TeamId")
                .ChildKeyColumn("UserId").Cascade.All()
                .BatchSize(10);

            HasManyToMany(x => x.Storages)
                .AsSet().Access.CamelCaseField()
                .Table("TeamStorage")
                .ParentKeyColumn("TeamId")
                .ChildKeyColumn("StorageId").Cascade.All()
                .BatchSize(50);

            HasManyToMany(x => x.ProductionOrders)
                .AsSet().Access.CamelCaseField()
                .Table("TeamProductionOrder")
                .ParentKeyColumn("TeamId")
                .ChildKeyColumn("ProductionOrderId").Cascade.All()
                .BatchSize(50);

            Where("DeletionDate is null");

            Cache.ReadWrite();
            BatchSize(50);
        }
    }
}
