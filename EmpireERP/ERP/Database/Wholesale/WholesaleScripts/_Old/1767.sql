BEGIN TRY
	BEGIN TRAN		
	
	ALTER TABLE dbo.[ProductionOrderBatch] ADD
       ReceiptWaybillId UNIQUEIDENTIFIER null
    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_ReceiptWaybill 
        foreign key (ReceiptWaybillId) 
        references dbo.[ReceiptWaybill]
	
	ALTER TABLE dbo.[ReceiptWaybill] ADD
       ProductionOrderBatchId UNIQUEIDENTIFIER null
    alter table dbo.[ReceiptWaybill] 
        add constraint FK_ReceiptWaybill_ProductionOrderBatch 
        foreign key (ProductionOrderBatchId) 
        references dbo.[ProductionOrderBatch]

	ALTER TABLE dbo.[ProductionOrderBatchRow] ADD
       ReceiptWaybillRowId UNIQUEIDENTIFIER null
    alter table dbo.[ProductionOrderBatchRow] 
        add constraint FK_ProductionOrderBatchRow_ReceiptWaybillRow 
        foreign key (ReceiptWaybillRowId) 
        references dbo.ReceiptWaybillRow
	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH