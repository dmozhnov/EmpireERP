BEGIN TRY
	BEGIN TRAN			
	
	ALTER TABLE Deal Add CuratorId INT NOT NULL
		CONSTRAINT DF_Deal_Curator DEFAULT 1
	
	alter table dbo.[Deal] 
        add constraint FK_Deal_Curator 
        foreign key (CuratorId) 
        references dbo.[User]
	
	ALTER TABLE dbo.Deal DROP CONSTRAINT DF_Deal_Curator
	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
