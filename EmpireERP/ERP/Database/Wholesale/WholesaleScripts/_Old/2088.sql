BEGIN TRY
	BEGIN TRAN		
	
	update AcceptedArticlePriceChangeIndicator set AccountOrganizationId = 0
	
	PRINT '���������� ��������� �������'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT '��������� ������ � ������' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH
