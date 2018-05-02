BEGIN TRY
	BEGIN TRAN		

	ALTER TABLE dbo.[ProducerOrganization] ADD IsManufacturer BIT not null CONSTRAINT DF_ProducerOrganization_IsManufacturer DEFAULT 0;
	ALTER TABLE dbo.[ProducerOrganization] DROP CONSTRAINT DF_ProducerOrganization_IsManufacturer;

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
