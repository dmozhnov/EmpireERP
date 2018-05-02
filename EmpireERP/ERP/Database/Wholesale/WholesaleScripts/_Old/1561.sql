BEGIN TRY
	BEGIN TRAN		

	UPDATE [ProductionOrderBatchStage]
	SET [ProductionOrderBatchStageTypeId] = 5
	WHERE [ProductionOrderBatchStageTypeId] = 4


	UPDATE [ProductionOrderBatchStage]
	SET [ProductionOrderBatchStageTypeId] = 4
	WHERE [ProductionOrderBatchStageTypeId] = 3


	UPDATE [ProductionOrderBatchStage]
	SET [ProductionOrderBatchStageTypeId] = 3
	WHERE [ProductionOrderBatchStageTypeId] = 2

	UPDATE [ProductionOrderBatchStage]
	SET [Name] = '�������� ������'
	WHERE [OrdinalNumber] = 1

	UPDATE [DefaultProductionOrderBatchStage]
	SET [Name] = '�������� ������'
	WHERE [Id] = 1

	UPDATE D
	SET [Name] = '�������� ������'
	FROM [ProductionOrderBatchLifeCycleTemplateStage] T
	JOIN [DefaultProductionOrderBatchStage] D on D.id = T.id	
	WHERE T.OrdinalNumber = 1

	PRINT '���������� ��������� �������'
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT '��������� ������!!!'
	RETURN
END CATCH
