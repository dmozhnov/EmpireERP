BEGIN TRY
    BEGIN TRAN

	EXEC sp_rename 'AccountingPriceList.AccountingPriceListCuratorId', 'CuratorId';
	
    PRINT '���������� ��������� �������'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT '��������� ������!!!'
    RETURN
END CATCH
