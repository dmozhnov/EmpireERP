BEGIN TRY
    BEGIN TRAN

    --if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ReceiptWaybill_ApprovedValueAddedTax]') AND parent_object_id = OBJECT_ID('[ReceiptWaybill]'))
	alter table dbo.[ReceiptWaybill]  drop constraint FK_ReceiptWaybill_ApprovedValueAddedTax

    ALTER TABLE dbo.[ReceiptWaybill] DROP COLUMN ReceiptedValueAddedTaxSum

	ALTER TABLE dbo.[ReceiptWaybill] DROP COLUMN ApprovedValueAddedTaxId

    PRINT '���������� ��������� �������'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT '��������� ������ � ������' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
    RETURN
END CATCH
