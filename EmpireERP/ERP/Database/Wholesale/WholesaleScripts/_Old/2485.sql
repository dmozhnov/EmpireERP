BEGIN TRY
	BEGIN TRAN
	
	ALTER TABLE AccountingPriceListWaybillTaking
	ADD IsLinkForAccountingPriceListEnding BIT  
	
	ALTER TABLE AccountingPriceListWaybillTaking
	ADD IsReservForReverseRevaluate BIT
	
	ALTER TABLE AccountingPriceListWaybillTaking
	ADD ArticleId INT
	
	ALTER TABLE AccountingPriceListWaybillTaking
	ADD StorageId SMALLINT
	
	ALTER TABLE AccountingPriceList
	ADD IsUsedByAccountingPriceBackChanging BIT 
	
	PRINT 'Обновление 1 выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH

GO 
 
BEGIN TRY
	BEGIN TRAN
	
	UPDATE AccountingPriceListWaybillTaking SET IsLinkForAccountingPriceListEnding = 0
	UPDATE AccountingPriceListWaybillTaking SET IsReservForReverseRevaluate = 0
	UPDATE AccountingPriceListWaybillTaking SET ArticleId = 0
	UPDATE AccountingPriceListWaybillTaking SET StorageId = 0
	UPDATE AccountingPriceList SET IsUsedByAccountingPriceBackChanging = 0

	ALTER TABLE AccountingPriceListWaybillTaking
	ALTER COLUMN IsLinkForAccountingPriceListEnding BIT NOT NULL
	
	ALTER TABLE AccountingPriceListWaybillTaking
	ALTER COLUMN IsReservForReverseRevaluate BIT NOT NULL
	
	ALTER TABLE AccountingPriceListWaybillTaking
	ALTER COLUMN ArticleId INT NOT NULL
	
	ALTER TABLE AccountingPriceListWaybillTaking
	ALTER COLUMN StorageId SMALLINT NOT NULL
	
	ALTER TABLE AccountingPriceList
	ALTER COLUMN IsUsedByAccountingPriceBackChanging BIT NOT NULL
	
	PRINT 'Обновление 2 выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH
