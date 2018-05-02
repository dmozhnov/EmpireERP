BEGIN TRY
	BEGIN TRAN		
	
	DELETE PermissionDistribution WHERE PermissionId = 3609
	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
