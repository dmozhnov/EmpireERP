BEGIN TRY
	BEGIN TRAN		
	
	ALTER TABLE dbo.ReceiptWaybillRow ADD ArticleMeasureUnitScale TINYINT not null
	CONSTRAINT DF_ReceiptWaybillRow_ArticleMeasureUnitScale DEFAULT 0
	ALTER TABLE dbo.ReceiptWaybillRow DROP CONSTRAINT DF_ReceiptWaybillRow_ArticleMeasureUnitScale

	PRINT '���������� ��������� �������'	
			
	COMMIT TRAN	
	
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT '��������� ������!!!'
	RETURN
END CATCH
