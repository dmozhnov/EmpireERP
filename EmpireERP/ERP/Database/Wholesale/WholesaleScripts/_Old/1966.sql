BEGIN TRY
    BEGIN TRAN

    EXEC sp_rename 'FK_ReceiptWaybill_FinalValueAddedTax', 'FK_ReceiptWaybill_ApprovedValueAddedTax';

    PRINT 'Обновление выполнено успешно'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
    RETURN
END CATCH
