BEGIN TRY
    BEGIN TRAN

    EXEC sp_rename 'FK_ReceiptWaybill_FinalValueAddedTax', 'FK_ReceiptWaybill_ApprovedValueAddedTax';

    PRINT '���������� ��������� �������'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT '��������� ������ � ������' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
    RETURN
END CATCH
