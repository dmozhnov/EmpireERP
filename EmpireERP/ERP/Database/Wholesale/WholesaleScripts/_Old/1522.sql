BEGIN TRY
    BEGIN TRAN

    EXEC sp_rename 'ProductionOrderBatchRow.RowProductionCostSumInCurrency', 'ProductionOrderBatchRowCostInCurrency';

    ALTER TABLE dbo.[ProductionOrderBatch]
        DROP COLUMN [Comment]

    PRINT 'Обновление выполнено успешно'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT 'Произошла ошибка!!!'
    RETURN
END CATCH
