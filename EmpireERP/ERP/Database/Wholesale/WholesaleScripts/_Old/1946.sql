BEGIN TRY
    BEGIN TRAN

    EXEC sp_rename 'ArticlePriceChangeIndicator', 'AcceptedArticlePriceChangeIndicator';

    PRINT '���������� ��������� �������'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT '��������� ������!!!'
    RETURN
END CATCH
