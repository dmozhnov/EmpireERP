BEGIN TRY
    BEGIN TRAN

	EXEC sp_rename 'Region', 'ClientRegion';
	
    PRINT '���������� ��������� �������'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT '��������� ������!!!'
    RETURN
END CATCH
