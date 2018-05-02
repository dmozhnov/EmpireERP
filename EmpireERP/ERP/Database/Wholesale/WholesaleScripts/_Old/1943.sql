BEGIN TRY
	BEGIN TRAN		
	
	ALTER TABLE dbo.ReceiptWaybillRow ADD ValueAddedTaxId SMALLINT not null
	CONSTRAINT DF_ReceiptWaybillRow_ValueAddedTaxId DEFAULT 2
	ALTER TABLE dbo.ReceiptWaybillRow DROP CONSTRAINT DF_ReceiptWaybillRow_ValueAddedTaxId

    alter table dbo.ReceiptWaybillRow 
        add constraint FK_ReceiptWaybillRow_ValueAddedTax 
        foreign key (ValueAddedTaxId) 
        references dbo.ValueAddedTax
	
	PRINT '���������� 1 ��������� �������'
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT '��������� ������!!!'
	RETURN
END CATCH

GO

BEGIN TRY
	BEGIN TRAN		

	UPDATE R
	SET ValueAddedTaxId = W.PendingValueAddedTaxId
	FROM dbo.ReceiptWaybillRow R
	JOIN [ReceiptWaybill] W on R.ReceiptWaybillId = W.Id
	
	PRINT '���������� 2 ��������� �������'
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT '��������� ������!!!'
	RETURN
END CATCH
