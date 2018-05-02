BEGIN TRY
    BEGIN TRAN

    ALTER TABLE dbo.[ChangeOwnerWaybill]
        DROP COLUMN [FinallyMovementDate]

    PRINT 'Обновление выполнено успешно'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT 'Произошла ошибка!!!'
    RETURN
END CATCH
