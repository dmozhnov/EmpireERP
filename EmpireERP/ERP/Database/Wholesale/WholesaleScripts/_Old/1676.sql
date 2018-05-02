BEGIN TRY
	BEGIN TRAN		
	
	ALTER TABLE dbo.Payment ADD
		IsFromClient bit NOT NULL CONSTRAINT DF_Payment_IsFromClient DEFAULT 1
	ALTER TABLE dbo.Payment DROP CONSTRAINT  DF_Payment_IsFromClient
	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH