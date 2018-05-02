BEGIN TRY
	BEGIN TRAN		
	
	ALTER TABLE dbo.Client ADD
		InitialBalance DECIMAL(18, 6) NOT NULL CONSTRAINT DF_Client_InitialBalance DEFAULT 0
	ALTER TABLE dbo.Client DROP CONSTRAINT DF_Client_InitialBalance
	
	ALTER TABLE dbo.Deal ADD
		InitialBalance DECIMAL(18, 6) NOT NULL CONSTRAINT DF_Deal_InitialBalance DEFAULT 0
	ALTER TABLE dbo.Deal DROP CONSTRAINT DF_Deal_InitialBalance
	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
