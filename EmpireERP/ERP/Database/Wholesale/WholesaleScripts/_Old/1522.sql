BEGIN TRY
    BEGIN TRAN

    EXEC sp_rename 'ProductionOrderBatchRow.RowProductionCostSumInCurrency', 'ProductionOrderBatchRowCostInCurrency';

    ALTER TABLE dbo.[ProductionOrderBatch]
        DROP COLUMN [Comment]

    PRINT '���������� ��������� �������'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT '��������� ������!!!'
    RETURN
END CATCH
