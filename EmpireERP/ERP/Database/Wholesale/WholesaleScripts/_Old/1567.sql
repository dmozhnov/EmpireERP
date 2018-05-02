BEGIN TRY
	BEGIN TRAN		
	
    ALTER TABLE dbo.[ProductionOrderBatch] ADD
        MovedToApprovementStateById INT null
    ALTER TABLE dbo.[ProductionOrderBatch] ADD
        MovedToApprovedStateById INT null
    ALTER TABLE dbo.[ProductionOrderBatch] ADD
        ApprovedLineManagerId INT null
    ALTER TABLE dbo.[ProductionOrderBatch] ADD
        FinancialDepartmentApproverId INT null
    ALTER TABLE dbo.[ProductionOrderBatch] ADD
        SalesDepartmentApproverId INT null
    ALTER TABLE dbo.[ProductionOrderBatch] ADD
        AnalyticalDepartmentApproverId INT null
    ALTER TABLE dbo.[ProductionOrderBatch] ADD
        ApprovedProjectManagerId INT null

    ALTER TABLE dbo.[ProductionOrderBatch] ADD
        MovementToApprovementStateDate DATETIME null
    ALTER TABLE dbo.[ProductionOrderBatch] ADD
        MovementToApprovedStateDate DATETIME null
    ALTER TABLE dbo.[ProductionOrderBatch] ADD
        ApprovedByLineManagerDate DATETIME null
    ALTER TABLE dbo.[ProductionOrderBatch] ADD
        ApprovedByFinancialDepartmentDate DATETIME null
    ALTER TABLE dbo.[ProductionOrderBatch] ADD
        ApprovedBySalesDepartmentDate DATETIME null
    ALTER TABLE dbo.[ProductionOrderBatch] ADD
        ApprovedByAnalyticalDepartmentDate DATETIME null
    ALTER TABLE dbo.[ProductionOrderBatch] ADD
        ApprovedByProjectManagerDate DATETIME null



    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_MovedToApprovementStateBy 
        foreign key (MovedToApprovementStateById) 
        references dbo.[User]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_MovedToApprovedStateBy 
        foreign key (MovedToApprovedStateById) 
        references dbo.[User]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_ApprovedLineManager 
        foreign key (ApprovedLineManagerId) 
        references dbo.[User]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_FinancialDepartmentApprover 
        foreign key (FinancialDepartmentApproverId) 
        references dbo.[User]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_SalesDepartmentApprover 
        foreign key (SalesDepartmentApproverId) 
        references dbo.[User]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_AnalyticalDepartmentApprover 
        foreign key (AnalyticalDepartmentApproverId) 
        references dbo.[User]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_ApprovedProjectManager 
        foreign key (ApprovedProjectManagerId) 
        references dbo.[User]


	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
