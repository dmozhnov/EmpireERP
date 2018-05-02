BEGIN TRY
	BEGIN TRAN		
	
	ALTER TABLE dbo.ProductionOrder ALTER COLUMN ProductionOrderPlannedProductionExpensesInCurrency decimal(14,2) not null
	ALTER TABLE dbo.ProductionOrder ALTER COLUMN ProductionOrderPlannedTransportationExpensesInCurrency decimal(14,2) not null
	ALTER TABLE dbo.ProductionOrder ALTER COLUMN ProductionOrderPlannedExtraExpensesInCurrency decimal(14,2) not null
	ALTER TABLE dbo.ProductionOrder ALTER COLUMN ProductionOrderPlannedCustomsExpensesInCurrency decimal(14,2) not null
	
	ALTER TABLE dbo.[Role] ADD IsSystemAdmin BIT not null 
	CONSTRAINT DF_Role_IsSystemAdmin DEFAULT 0
	ALTER TABLE dbo.[Role] DROP CONSTRAINT DF_Role_IsSystemAdmin
	
	DECLARE @QUERY VARCHAR(4000)
	SET @QUERY = 'UPDATE [Role] SET IsSystemAdmin = 1 WHERE Id = 1'
	EXEC(@QUERY)
		
	PRINT 'Обновление выполнено успешно'	
			
	COMMIT TRAN	
	
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
