BEGIN TRY
	BEGIN TRAN		

	ALTER TABLE dbo.[Article] ALTER COLUMN PackLength decimal(6,0)
	ALTER TABLE dbo.[Article] ALTER COLUMN PackWidth decimal(6,0)
	ALTER TABLE dbo.[Article] ALTER COLUMN PackHeight decimal(6,0)

	ALTER TABLE dbo.[Article] ALTER COLUMN PackWeight decimal(8,3)

	ALTER TABLE dbo.[Article] ALTER COLUMN PackVolume decimal(15,6)



	ALTER TABLE dbo.[ProductionOrderBatchRow] ALTER COLUMN PackLength decimal(6,0)
	ALTER TABLE dbo.[ProductionOrderBatchRow] ALTER COLUMN PackWidth decimal(6,0)
	ALTER TABLE dbo.[ProductionOrderBatchRow] ALTER COLUMN PackHeight decimal(6,0)

	ALTER TABLE dbo.[ProductionOrderBatchRow] ALTER COLUMN PackWeight decimal(8,3)

	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
