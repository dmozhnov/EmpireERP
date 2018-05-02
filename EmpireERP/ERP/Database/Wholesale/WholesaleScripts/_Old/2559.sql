BEGIN TRY
	BEGIN TRAN		
	
	CREATE TABLE dbo.Setting (
		DataBaseVersion varchar(15) not null 
	)

	INSERT INTO dbo.Setting(DataBaseVersion)
	VALUES('0.9.1')
	
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