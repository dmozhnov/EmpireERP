BEGIN TRY
    BEGIN TRAN

    ALTER TABLE dbo.[ChangeOwnerWaybill]
        DROP COLUMN [FinallyMovementDate]

    PRINT '���������� ��������� �������'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT '��������� ������!!!'
    RETURN
END CATCH
