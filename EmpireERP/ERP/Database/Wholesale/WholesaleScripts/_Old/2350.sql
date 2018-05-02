BEGIN TRY
	BEGIN TRAN
	
	ALTER TABLE Deal
	ADD IsClosed BIT  
	
	PRINT 'Обновление 1 выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH

GO 
 
BEGIN TRY
	BEGIN TRAN
	
	UPDATE Deal SET IsClosed = 0
		WHERE DealStageId IN (1, 2, 3, 4, 5, 6, 10)
		
	UPDATE Deal SET IsClosed = 1
		WHERE DealStageId IN (7, 8, 9)
	
	ALTER TABLE Deal
	ALTER COLUMN IsClosed BIT NOT NULL
	
	PRINT 'Обновление 2 выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH
