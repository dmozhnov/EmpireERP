BEGIN TRY
	BEGIN TRAN		

	alter table dbo.[AccountingPriceList] drop column [DistributionId]	
	
	PRINT '���������� ��������� �������'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT '��������� ������!!!'
	RETURN
END CATCH
