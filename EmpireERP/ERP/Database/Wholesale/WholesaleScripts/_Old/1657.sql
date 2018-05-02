BEGIN TRY
	BEGIN TRAN		

	ALTER TABLE dbo.[ProductionOrderBatch] ADD
		ProductionOrderCustomsDeclarationId UNIQUEIDENTIFIER null

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_ProductionOrderCustomsDeclaration 
        foreign key (ProductionOrderCustomsDeclarationId) 
        references dbo.[ProductionOrderCustomsDeclaration]
	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
