BEGIN TRY
	BEGIN TRAN		
	
	alter table [ReceiptWaybillRow] alter column CountryId SMALLINT not null
		
	PRINT '���������� ��������� �������'	
		
	--ROLLBACK TRAN -- ����� �������� ��� �������, � COMMIT TRAN ��� ������� ������������
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT '��������� ������ � ������' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH
