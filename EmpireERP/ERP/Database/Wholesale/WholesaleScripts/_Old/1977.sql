BEGIN TRY
	BEGIN TRAN		

	ALTER TABLE dbo.[ReceiptWaybill] ALTER COLUMN PendingSum DECIMAL(18, 2) not null
	ALTER TABLE dbo.[ReceiptWaybill] ALTER COLUMN ReceiptedSum DECIMAL(18, 2) null
	ALTER TABLE dbo.[ReceiptWaybill] ALTER COLUMN ApprovedSum DECIMAL(18, 2) null

	PRINT '���������� ��������� �������'
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT '��������� ������ � ������' + STR(ERROR_LINE()) + ':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH
