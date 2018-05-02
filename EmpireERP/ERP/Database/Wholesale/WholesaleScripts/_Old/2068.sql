BEGIN TRY
	BEGIN TRAN		
	
	alter table [ReceiptWaybill] alter column CustomsDeclarationNumber VARCHAR(33) not null
	
	alter table ReceiptWaybillRow alter column CustomsDeclarationNumber VARCHAR(33) not null
	
	alter table [ProductionOrderCustomsDeclaration] alter column Number VARCHAR(33) not null
			
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH
