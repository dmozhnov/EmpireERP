BEGIN TRY
	BEGIN TRAN		
	
	ALTER TABLE dbo.ProductionOrder ADD
		ProductionOrderPlannedProductionExpensesInCurrency DECIMAL(18, 6) NOT NULL CONSTRAINT DF_ProductionOrder_ProductionOrderPlannedProductionExpensesInCurrency DEFAULT 0
	ALTER TABLE dbo.ProductionOrder DROP CONSTRAINT DF_ProductionOrder_ProductionOrderPlannedProductionExpensesInCurrency
	
	ALTER TABLE dbo.ProductionOrder ADD
		ProductionOrderPlannedTransportationExpensesInCurrency DECIMAL(18, 6) NOT NULL CONSTRAINT DF_ProductionOrder_ProductionOrderPlannedTransportationExpensesInCurrency DEFAULT 0
	ALTER TABLE dbo.ProductionOrder DROP CONSTRAINT DF_ProductionOrder_ProductionOrderPlannedTransportationExpensesInCurrency

    ALTER TABLE dbo.ProductionOrder ADD
		ProductionOrderPlannedExtraExpensesInCurrency DECIMAL(18, 6) NOT NULL CONSTRAINT DF_ProductionOrder_ProductionOrderPlannedExtraExpensesInCurrency DEFAULT 0
	ALTER TABLE dbo.ProductionOrder DROP CONSTRAINT DF_ProductionOrder_ProductionOrderPlannedExtraExpensesInCurrency
	
	ALTER TABLE dbo.ProductionOrder ADD
		ProductionOrderPlannedCustomsExpensesInCurrency DECIMAL(18, 6) NOT NULL CONSTRAINT DF_ProductionOrder_ProductionOrderPlannedCustomsExpensesInCurrency DEFAULT 0
	ALTER TABLE dbo.ProductionOrder DROP CONSTRAINT DF_ProductionOrder_ProductionOrderPlannedCustomsExpensesInCurrency
	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
