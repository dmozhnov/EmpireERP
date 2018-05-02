BEGIN TRY
	BEGIN TRAN		
	
	ALTER TABLE dbo.[MovementWaybillRow] ADD AcceptancePendingCount decimal(18,6) not null 
	CONSTRAINT DF_MovementWaybillRow_AcceptancePendingCount DEFAULT 0
	ALTER TABLE dbo.[MovementWaybillRow] DROP CONSTRAINT DF_MovementWaybillRow_AcceptancePendingCount
	
	ALTER TABLE dbo.[WriteoffWaybillRow] ADD AcceptancePendingCount decimal(18,6) not null 
	CONSTRAINT DF_WriteoffWaybillRow_AcceptancePendingCount DEFAULT 0
	ALTER TABLE dbo.[WriteoffWaybillRow] DROP CONSTRAINT DF_WriteoffWaybillRow_AcceptancePendingCount
	
	ALTER TABLE dbo.[ChangeOwnerWaybillRow] ADD AcceptancePendingCount decimal(18,6) not null 
	CONSTRAINT DF_ChangeOwnerWaybillRow_AcceptancePendingCount DEFAULT 0
	ALTER TABLE dbo.[ChangeOwnerWaybillRow] DROP CONSTRAINT DF_ChangeOwnerWaybillRow_AcceptancePendingCount
	
	ALTER TABLE dbo.[ExpenditureWaybillRow] ADD AcceptancePendingCount decimal(18,6) not null 
	CONSTRAINT DF_ExpenditureWaybillRow_AcceptancePendingCount DEFAULT 0
	ALTER TABLE dbo.[ExpenditureWaybillRow] DROP CONSTRAINT DF_ExpenditureWaybillRow_AcceptancePendingCount
	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
