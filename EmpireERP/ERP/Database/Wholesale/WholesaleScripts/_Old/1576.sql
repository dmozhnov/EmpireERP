BEGIN TRY
    BEGIN TRAN

	ALTER TABLE dbo.[Article] ADD ManufacturerNumber VARCHAR(30) not null
		CONSTRAINT DF_Article_ManufacturerNumber DEFAULT ''

	ALTER TABLE dbo.[Article] DROP CONSTRAINT DF_Article_ManufacturerNumber

	ALTER TABLE dbo.[ProductionOrderBatchRow] DROP COLUMN ManufacturerArticleNumber

    PRINT '���������� ��������� �������'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT '��������� ������!!!'
    RETURN
END CATCH
