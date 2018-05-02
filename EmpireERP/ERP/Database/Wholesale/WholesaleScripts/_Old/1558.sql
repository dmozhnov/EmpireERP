BEGIN TRY
	BEGIN TRAN		

	ALTER TABLE dbo.[ProductionOrderBatchRow]
	ADD PackWeight DECIMAL(7, 3) not null CONSTRAINT DF_ProductionOrderBatchRow_PackWeight DEFAULT 0
	ALTER TABLE dbo.[ProductionOrderBatchRow] DROP CONSTRAINT DF_ProductionOrderBatchRow_PackWeight

	PRINT '���������� ��������� �������'
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT '��������� ������!!!'
	RETURN
END CATCH
