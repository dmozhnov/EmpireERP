BEGIN TRY
    BEGIN TRAN

	ALTER TABLE dbo.AccountingPriceList ADD AccountingPriceListCuratorId INT NOT NULL
		CONSTRAINT DF_AccountingPriceList_AccountingPriceListCuratorId DEFAULT 1

	ALTER TABLE dbo.AccountingPriceList DROP CONSTRAINT DF_AccountingPriceList_AccountingPriceListCuratorId

    ALTER TABLE dbo.AccountingPriceList 
        ADD CONSTRAINT FK_AccountingPriceList_Curator 
        FOREIGN KEY (AccountingPriceListCuratorId) 
        REFERENCES dbo.[User]

    PRINT '���������� ��������� �������'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT '��������� ������!!!'
    RETURN
END CATCH
