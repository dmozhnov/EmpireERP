BEGIN TRY
    BEGIN TRAN

    EXEC sp_rename 'ArticlePriceChangeIndicator', 'AcceptedArticlePriceChangeIndicator';

    PRINT 'Обновление выполнено успешно'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT 'Произошла ошибка!!!'
    RETURN
END CATCH
