BEGIN TRY
    BEGIN TRAN

	ALTER TABLE ReturnFromClientIndicator
	ADD BatchId uniqueidentifier NOT NULL

    PRINT 'Обновление выполнено успешно'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT 'Произошла ошибка!!!'
    RETURN
END CATCH
