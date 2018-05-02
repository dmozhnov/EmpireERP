BEGIN TRY
	BEGIN TRAN		
	
	ALTER TABLE dbo.[ExpenditureWaybill] ADD RoundSalePrice BIT not null
		CONSTRAINT DF_ExpenditureWaybill_RoundSalePrice DEFAULT 0
		
	ALTER TABLE dbo.[Article] ALTER COLUMN PackVolume decimal(10,6)	
	ALTER TABLE dbo.[Article] ALTER COLUMN PackLength decimal(10,6)
	ALTER TABLE dbo.[Article] ALTER COLUMN PackHeight decimal(10,6)
	ALTER TABLE dbo.[Article] ALTER COLUMN PackWidth decimal(10,6)
	
	ALTER TABLE dbo.[ProductionOrderBatchRow] ALTER COLUMN PackLength decimal(10,6)
	ALTER TABLE dbo.[ProductionOrderBatchRow] ALTER COLUMN PackHeight decimal(10,6)
	ALTER TABLE dbo.[ProductionOrderBatchRow] ALTER COLUMN PackWidth decimal(10,6)
	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
