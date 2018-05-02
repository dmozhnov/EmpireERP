BEGIN TRY
	BEGIN TRAN		
	declare @count int
	declare @iter int
	declare @index_name varchar(255)
	declare @TableVar table (id int,
							 name varchar(255))
	insert into @TableVar
	select ROW_NUMBER() over(order by i.name),i.name
	from sys.tables t
		join sys.indexes i on i.object_id = t.object_id
		where t.name = 'Article' and i.is_unique_constraint = 1	
	select @count=COUNT(*) from @TableVar
	
	set @iter = 1
	while(@iter < @count+1)
	begin
		select @index_name=name from @TableVar where id=@iter
		exec ('ALTER TABLE [dbo].[Article] DROP CONSTRAINT ' + @index_name)
		set @iter = @iter + 1
	end
	
	ALTER TABLE Article
			ADD CONSTRAINT ArticleNumber_Unique  
			UNIQUE (Number)
		
	PRINT 'Обновление выполнено успешно'	
			
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH

