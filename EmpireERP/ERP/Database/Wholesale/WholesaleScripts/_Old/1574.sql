BEGIN TRY
    BEGIN TRAN

	ALTER TABLE dbo.[ProductionOrder] ADD StorageId SMALLINT not null
		CONSTRAINT DF_ProductionOrder_Storage DEFAULT 1

	ALTER TABLE dbo.[ProductionOrder] DROP CONSTRAINT DF_ProductionOrder_Storage

    alter table dbo.[ProductionOrder] 
        add constraint FK_ProductionOrder_Storage 
        foreign key (StorageId) 
        references dbo.[Storage]

    PRINT '���������� ��������� �������'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT '��������� ������!!!'
    RETURN
END CATCH
