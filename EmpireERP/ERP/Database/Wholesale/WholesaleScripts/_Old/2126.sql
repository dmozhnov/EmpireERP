BEGIN TRY
	BEGIN TRAN		

	ALTER TABLE dbo.[ProducerOrganization] ADD IsManufacturer BIT not null CONSTRAINT DF_ProducerOrganization_IsManufacturer DEFAULT 0;
	ALTER TABLE dbo.[ProducerOrganization] DROP CONSTRAINT DF_ProducerOrganization_IsManufacturer;

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
