BEGIN TRY
    BEGIN TRAN

	ALTER TABLE ReturnFromClientIndicator
	ADD BatchId uniqueidentifier NOT NULL

    PRINT '���������� ��������� �������'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT '��������� ������!!!'
    RETURN
END CATCH
