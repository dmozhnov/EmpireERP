BEGIN TRY
	BEGIN TRAN		
	--�������� ����������� �� ������������
	PRINT '��� �������� ������ ������� ����� ������������ ������� ����� �60 "������������� Sc-012"'

		ALTER TABLE ForeignBank
		ADD CONSTRAINT SWIFT_Unique  
		UNIQUE (SWIFT)

		ALTER TABLE Article
		ADD CONSTRAINT ArticleFullName_Unique  
		UNIQUE (FullName)

		ALTER TABLE Article
		ADD CONSTRAINT ArticleShortName_Unique  
		UNIQUE (ShortName)

		ALTER TABLE Article
		ADD CONSTRAINT ArticleNumber_Unique 
		UNIQUE (Number)

	PRINT '���������� ��������� �������'
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT '��������� ������ � ������ ' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH

