BEGIN TRY
	BEGIN TRAN		
	
	ALTER TABLE dbo.ProductionOrder ALTER COLUMN ProductionOrderPlannedProductionExpensesInCurrency decimal(14,2)
	ALTER TABLE dbo.ProductionOrder ALTER COLUMN ProductionOrderPlannedTransportationExpensesInCurrency decimal(14,2)
	ALTER TABLE dbo.ProductionOrder ALTER COLUMN ProductionOrderPlannedExtraExpensesInCurrency decimal(14,2)
	ALTER TABLE dbo.ProductionOrder ALTER COLUMN ProductionOrderPlannedCustomsExpensesInCurrency decimal(14,2)
	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
