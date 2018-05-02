BEGIN TRY
	BEGIN TRAN		

	alter table dbo.[AccountingPriceList] drop column [DistributionId]	
	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
