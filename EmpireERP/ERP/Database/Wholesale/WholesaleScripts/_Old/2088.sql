BEGIN TRY
	BEGIN TRAN		
	
	update AcceptedArticlePriceChangeIndicator set AccountOrganizationId = 0
	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH
