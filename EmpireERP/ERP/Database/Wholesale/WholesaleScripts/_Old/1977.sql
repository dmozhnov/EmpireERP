BEGIN TRY
	BEGIN TRAN		

	ALTER TABLE dbo.[ReceiptWaybill] ALTER COLUMN PendingSum DECIMAL(18, 2) not null
	ALTER TABLE dbo.[ReceiptWaybill] ALTER COLUMN ReceiptedSum DECIMAL(18, 2) null
	ALTER TABLE dbo.[ReceiptWaybill] ALTER COLUMN ApprovedSum DECIMAL(18, 2) null

	PRINT 'Обновление выполнено успешно'
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) + ':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH
