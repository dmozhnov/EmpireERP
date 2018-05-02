BEGIN TRY
    BEGIN TRAN

	ALTER TABLE dbo.[ProductionOrderPayment] ADD
		[Comment] VARCHAR(4000) not null CONSTRAINT DF_ProductionOrderPayment_Comment DEFAULT ''
	ALTER TABLE dbo.[ProductionOrderPayment] DROP CONSTRAINT DF_ProductionOrderPayment_Comment

    --ALTER TABLE dbo.[ProductionOrderPayment]
    --   ADD COLUMN [Comment] VARCHAR(4000) not null

    PRINT 'Обновление выполнено успешно'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT 'Произошла ошибка!!!'
    RETURN
END CATCH
