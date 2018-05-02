using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class TaskMap : ClassMap<ERP.Wholesale.Domain.Entities.Task>
    {
        public TaskMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.CompletionPercentage).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeadLine).Access.CamelCaseField().Nullable();
            Map(x => x.DeletionDate).Access.CamelCaseField().Access.CamelCaseField();
            Map(x => x.Description).Access.CamelCaseField().Length(8000).Not.Nullable();
            Map(x => x.FactualCompletionDate).Access.CamelCaseField().Nullable();
            Map(x => x.FactualSpentTime).Access.CamelCaseField().Not.Nullable();
            Map(x => x.StartDate).Access.CamelCaseField().Nullable();
            Map(x => x.Topic).Access.CamelCaseField().Length(200).Not.Nullable();

            References(x => x.Contractor).Access.CamelCaseField().Column("ContractorId");
            References(x => x.CreatedBy).Column("CreatedById").Not.Nullable().Fetch.Select();
            References(x => x.Deal).Access.CamelCaseField().Column("DealId");
            References(x => x.ExecutedBy).Access.CamelCaseField().Column("ExecutedById").Fetch.Select();
            References(x => x.Priority).Access.CamelCaseField().Column("PriorityId").Fetch.Join();
            References(x => x.ProductionOrder).Access.CamelCaseField().Column("ProductionOrderId");
            References(x => x.ExecutionState).Access.CamelCaseField().Column("ExecutionStateId").Not.Nullable().Fetch.Join();
            References(x => x.Type).Access.CamelCaseField().Column("TypeId").Not.Nullable().Fetch.Join();

            HasMany(x => x.History)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("TaskId").Inverse().Cascade.SaveUpdate();

            HasMany(x=>x.ExecutionHistory)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("TaskId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");
            
            Where("DeletionDate is null");
        }
    }
}
