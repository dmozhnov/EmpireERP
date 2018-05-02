using ERP.Wholesale.Domain.Entities;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProductionOrderBatchMap : ClassMap<ProductionOrderBatch>
    {
        public ProductionOrderBatchMap()    
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();
            Map(x => x.Name).Not.Nullable().Length(200);
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate).Access.CamelCaseField();
            Map(x => x.State).CustomType<ProductionOrderBatchState>().Column("ProductionOrderBatchStateId").Access.CamelCaseField().Not.Nullable();
            Map(x => x.Date).Not.Nullable();
            Map(x => x.IsClosed).Access.CamelCaseField().Not.Nullable();
            Map(x => x.MovementToApprovementStateDate);
            Map(x => x.MovementToApprovedStateDate);
            Map(x => x.ApprovedByLineManagerDate);
            Map(x => x.ApprovedByFinancialDepartmentDate);
            Map(x => x.ApprovedBySalesDepartmentDate);
            Map(x => x.ApprovedByAnalyticalDepartmentDate);
            Map(x => x.ApprovedByProjectManagerDate);

            References(x => x.ProductionOrder).Column("ProductionOrderId").Not.Nullable();
            References(x => x.ReceiptWaybill).Column("ReceiptWaybillId");
            References(x => x.CreatedBy).Column("CreatedById").Not.Nullable();
            References(x => x.Curator).Column("CuratorId").Not.Nullable();
            References(x => x.CurrentStage).Column("CurrentStageId").Cascade.All().Access.CamelCaseField().Not.Nullable();
            References(x => x.MovedToApprovementStateBy).Column("MovedToApprovementStateById");
            References(x => x.MovedToApprovedStateBy).Column("MovedToApprovedStateById");
            References(x => x.ApprovedLineManager).Column("ApprovedLineManagerId");
            References(x => x.FinancialDepartmentApprover).Column("FinancialDepartmentApproverId");
            References(x => x.SalesDepartmentApprover).Column("SalesDepartmentApproverId");
            References(x => x.AnalyticalDepartmentApprover).Column("AnalyticalDepartmentApproverId");
            References(x => x.ApprovedProjectManager).Column("ApprovedProjectManagerId");

            HasMany(x => x.Rows).AsSet().Access.CamelCaseField().KeyColumn("BatchId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");
            HasMany(x => x.Stages).AsSet().Access.CamelCaseField().KeyColumn("BatchId").Inverse().Cascade.AllDeleteOrphan();

            Where("DeletionDate is null");
        }
    }
}