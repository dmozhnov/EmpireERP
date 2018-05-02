using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class DealMap : ClassMap<Deal>
    {
        public DealMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Name).Length(200).Not.Nullable();
            Map(x => x.Stage).CustomType<DealStage>().Column("DealStageId").Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.StageDate).Not.Nullable();
            Map(x => x.ExpectedBudget).Precision(18).Scale(2);
            Map(x => x.IsClosed).Not.Nullable();
            Map(x => x.Comment).Length(4000).Not.Nullable();

            References(x => x.Client).Column("ClientId").Not.Nullable();
            References(x => x.Contract).Column("ClientContractId").Access.CamelCaseField();
            References(x => x.Curator).Column("CuratorId").Not.Nullable();

            HasMany(x => x.StageHistory).AsSet().Access.CamelCaseField().KeyColumn("DealId").Inverse().Cascade.SaveUpdate();
            HasManyToMany<DealQuota>(x => x.Quotas)
                .AsSet().Access.CamelCaseField()
                .Table("DealDealQuota")
                .ParentKeyColumn("DealId")
                .ChildKeyColumn("DealQuotaId").Cascade.All();            
            HasMany(x => x.DealPaymentDocuments).AsSet().Access.CamelCaseField().KeyColumn("DealId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null")
                .BatchSize(50);

            BatchSize(50);
        }
    }
}
