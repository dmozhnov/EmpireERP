BEGIN TRY
	BEGIN TRAN		
	
	DELETE PermissionDistribution WHERE PermissionId = 3609
	
	PRINT '���������� ��������� �������'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT '��������� ������!!!'
	RETURN
END CATCH
