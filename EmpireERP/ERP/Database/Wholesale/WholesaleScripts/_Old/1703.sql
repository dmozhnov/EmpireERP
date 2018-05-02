BEGIN TRY
	BEGIN TRAN		
	
	ALTER TABLE dbo.Client ADD
		FactualAddress varchar(250) NOT NULL CONSTRAINT DF_Client_FactualAddress DEFAULT ''
	ALTER TABLE dbo.Client DROP CONSTRAINT  DF_Client_FactualAddress
	
	ALTER TABLE dbo.Client ADD
		ContactPhone varchar(20) NOT NULL CONSTRAINT DF_Client_ContactPhone DEFAULT ''
	ALTER TABLE dbo.Client DROP CONSTRAINT  DF_Client_ContactPhone
	
	PRINT '���������� ��������� �������'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT '��������� ������!!!'
	RETURN
END CATCH