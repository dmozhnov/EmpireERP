BEGIN TRY
	BEGIN TRAN		
		
	declare @index_name varchar(255)
		
	select @index_name = i.name
	from sys.tables t
	join sys.indexes i on i.object_id = t.object_id
	where t.name = 'Deal' and i.is_unique_constraint = 1	
		
	exec ('ALTER TABLE [dbo].[Deal] DROP CONSTRAINT ' + @index_name)
	
	PRINT 'Обновление выполнено успешно'	
			
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH

