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
	SET [Name] = 'Создание заказа'
	WHERE [OrdinalNumber] = 1

	UPDATE [DefaultProductionOrderBatchStage]
	SET [Name] = 'Создание заказа'
	WHERE [Id] = 1

	UPDATE D
	SET [Name] = 'Создание заказа'
	FROM [ProductionOrderBatchLifeCycleTemplateStage] T
	JOIN [DefaultProductionOrderBatchStage] D on D.id = T.id	
	WHERE T.OrdinalNumber = 1

	PRINT 'Обновление выполнено успешно'
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
