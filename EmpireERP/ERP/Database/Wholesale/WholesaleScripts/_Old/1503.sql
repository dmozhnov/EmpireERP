BEGIN TRY
	BEGIN TRAN		
	
	ALTER TABLE dbo.ProductionOrderBatch ADD
		IsClosed bit NOT NULL CONSTRAINT DF_ProductionOrderBatch_IsClosed DEFAULT 0
	ALTER TABLE dbo.ProductionOrderBatch DROP CONSTRAINT DF_ProductionOrderBatch_IsClosed

	ALTER TABLE dbo.ProductionOrder ADD
		IsClosed bit NOT NULL CONSTRAINT DF_ProductionOrder_IsClosed DEFAULT 0
	ALTER TABLE dbo.ProductionOrder DROP CONSTRAINT DF_ProductionOrder_IsClosed

	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
