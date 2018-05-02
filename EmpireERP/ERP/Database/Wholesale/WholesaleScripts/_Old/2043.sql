BEGIN TRY
	BEGIN TRAN		
	
	ALTER TABLE dbo.ReceiptWaybillRow ADD ApprovedValueAddedTaxId SMALLINT null

    alter table dbo.ReceiptWaybillRow 
        add constraint FK_ReceiptWaybillRow_ApprovedValueAddedTax 
        foreign key (ApprovedValueAddedTaxId) 
        references dbo.ValueAddedTax

    select * from
    dbo.[ReceiptWaybillRow] RR
    JOIN [ReceiptWaybill] R on R.Id = RR.ReceiptWaybillId
    WHERE R.ReceiptWaybillStateId = 2 OR R.ReceiptWaybillStateId = 6
	
	PRINT '���������� 1 ��������� �������'
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT '��������� ������ � ������' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH

GO

BEGIN TRY
	BEGIN TRAN		
        
	update RR
	set ApprovedValueAddedTaxId = RR.ValueAddedTaxId
	from
    dbo.[ReceiptWaybillRow] RR
    JOIN [ReceiptWaybill] R on R.Id = RR.ReceiptWaybillId
    WHERE R.ReceiptWaybillStateId = 2 OR R.ReceiptWaybillStateId = 6
        
	EXEC sp_rename 'ReceiptWaybillRow.ValueAddedTaxId', 'PendingValueAddedTaxId';
	EXEC sp_rename 'FK_ReceiptWaybillRow_ValueAddedTax', 'FK_ReceiptWaybillRow_PendingValueAddedTax';
	
	PRINT '���������� 2 ��������� �������'
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT '��������� ������ � ������' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH

