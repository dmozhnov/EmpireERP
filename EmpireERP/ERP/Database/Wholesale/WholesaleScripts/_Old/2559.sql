BEGIN TRY
	BEGIN TRAN		
	
	CREATE TABLE dbo.Setting (
		DataBaseVersion varchar(15) not null 
	)

	INSERT INTO dbo.Setting(DataBaseVersion)
	VALUES('0.9.1')
	
	PRINT 'Обновление выполнено успешно'	
		
	--ROLLBACK TRAN -- Очень помогает при отладке, а COMMIT TRAN при отладке комментируем
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH