BEGIN TRY
	BEGIN TRAN	
	
	EXEC sp_rename 'ReceiptWaybillRow.ApprovedCount', 'AcceptedCount';
	
	PRINT '���������� ��������� �������'	
		
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
	
	alter table dbo.ReceiptWaybill add AcceptanceDate DATETIME null	

	EXEC sp_rename 'ReceiptWaybillRow.FinalCount', 'ApprovedCount';
	EXEC sp_rename 'ReceiptWaybillRow.FinalSum', 'ApprovedSum';
	EXEC sp_rename 'ReceiptWaybill.FinalSum', 'ApprovedSum';
	EXEC sp_rename 'ReceiptWaybill.FinalValueAddedTaxId', 'ApprovedValueAddedTaxId';
	
	PRINT '���������� ��������� �������'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT '��������� ������!!!'
	RETURN
END CATCH
