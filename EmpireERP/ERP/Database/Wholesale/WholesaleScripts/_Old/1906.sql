/* ������� ��� ����������, �.�. � ������ ����� UPDATE �� ����� ����������� ������� */

/* ��������� ������� � ������� [Producer] */
BEGIN TRY
	BEGIN TRAN		
	
	ALTER TABLE dbo.[Producer] ADD ManufacturerId SMALLINT NULL
	ALTER TABLE dbo.[Producer] DROP COLUMN IsManufacturer
	
	alter table dbo.[Producer] 
        add constraint FK_Producer_Manufacturer 
        foreign key (ManufacturerId) 
        references dbo.Manufacturer
   		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT '��������� ������!!!'
	RETURN
END CATCH

GO

/* ������� �������� ������� � ������������ ������ */
BEGIN TRY
	BEGIN TRAN		

	UPDATE P
	SET P.ManufacturerId = M.Id
	FROM dbo.[Producer] P
		JOIN [Manufacturer] M ON P.Id = M.ProducerId
	
	ALTER TABLE dbo.[Manufacturer]  DROP CONSTRAINT FK_Manufacturer_Producer
	
	ALTER TABLE dbo.[Manufacturer] DROP COLUMN ProducerId

	PRINT '���������� ��������� �������'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT '��������� ������!!!'
	RETURN
END CATCH