BEGIN TRY
	BEGIN TRAN		
	
	EXEC sp_rename 'ProductionOrderMaterialDocument', 'ProductionOrderMaterialsPackageDocument';
	
	PRINT '���������� ��������� �������'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT '��������� ������!!!'
	RETURN
END CATCH
