BEGIN TRY
	BEGIN TRAN		
	
	alter table ProductionOrderCustomsDeclaration
	add Name varchar(200) not null
	constraint DF_Name default ('') 
	
	alter table ProductionOrderCustomsDeclaration
	drop constraint DF_Name
	
	alter table ProductionOrderBatchRow
	add	PackVolume DECIMAL(15, 6) not null
	constraint DF_PackVolume default (0) 
	
	alter table ProductionOrderBatchRow
	drop constraint DF_PackVolume
	
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
