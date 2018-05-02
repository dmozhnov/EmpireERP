BEGIN TRY
	BEGIN TRAN		
	
	alter table ProductionOrderCustomsDeclaration
	add Name varchar(200) not null
	constraint DF_Name default ('') 
	
	alter table ProductionOrderCustomsDeclaration
	drop constraint DF_Name
	
	alter table ProductionOrderBatchRow
	add	PackVolume DECIMAL(15, 6) not null
	constraint DF_PackVolume default (0) 
	
	alter table ProductionOrderBatchRow
	drop constraint DF_PackVolume
	
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
