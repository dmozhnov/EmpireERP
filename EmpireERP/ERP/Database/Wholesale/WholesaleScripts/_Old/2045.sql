BEGIN TRY
	BEGIN TRAN		
	--�������� ����������� �� ������������

		ALTER TABLE RussianBank
		ADD CONSTRAINT BIK_Unique  
		UNIQUE (BIC)

	PRINT '���������� ��������� �������'
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT '��������� ������ � ������ ' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH

