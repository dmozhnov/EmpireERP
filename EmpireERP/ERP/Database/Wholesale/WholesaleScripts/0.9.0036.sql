/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.36
  Что нового:
	* Исправлен баг. Сбрасывается в null контрагент у удаленных задач.
	* Изменена струтура БД
	* Переименованны стобцы в таблицах
	* Восстановлены данные истории задач по исполнениям
	
---------------------------------------------------------------------------------------*/
--SET NOEXEC OFF	-- выполнить данную команду в случае неуспешного обновления
SET DATEFORMAT DMY
SET NOCOUNT ON
SET ARITHABORT ON
SET XACT_ABORT ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

DECLARE @PreviousVersion varchar(15),	-- номер предыдущей версии
		@CurrentVersion varchar(15),	-- номер текущей версии базы данных
		@NewVersion varchar(15),		-- номер новой версии
		@DataBaseName varchar(256),		-- текущая база данных
		@CurrentDate nvarchar(10),		-- текущая дата
		@CurrentTime nvarchar(10),		-- текущее время
		@BackupTarget nvarchar(100)		-- куда делать бэкап базы данных

SET @PreviousVersion = '0.9.35' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.36'			-- номер новой версии

SELECT @CurrentVersion = DataBaseVersion FROM Setting
IF @@ERROR <> 0
BEGIN
	PRINT 'Неверная база данных'
END
ELSE
BEGIN
	-- СОЗДАЕМ БЭКАП БАЗЫ ДАННЫХ
	-- Получаем текущую дату
	SET @CurrentDate = CONVERT(nvarchar(20), GETDATE(), 104)	--	dd.mm.yyyy
	SET @CurrentTime = REPLACE(CONVERT(nvarchar(20), GETDATE(), 108) , ':', '.') --	hh:mm:ss
	SET @DataBaseName = DB_NAME()

	SET @BackupTarget = N'D:\Bizpulse\Backup\Update\' + @DataBaseName + '(' + CAST(@CurrentVersion as nvarchar(20)) + ') ' + 
		@CurrentDate + ' ' + @CurrentTime + N'.bak'

	BACKUP DATABASE @DataBaseName TO DISK = @BackupTarget WITH  INIT,  NOUNLOAD,  NAME = N'wholesale', NOSKIP, DESCRIPTION = 
		N'Обновление версии', NOFORMAT

	IF @@ERROR <> 0
	BEGIN
		PRINT 'Ошибка создания backup''а. Продолжение выполнения невозможно.'
	END
	ELSE
		BEGIN

		IF (@CurrentVersion <> @PreviousVersion)
		BEGIN
			PRINT 'Обновить базу данных ' + @DataBaseName + ' до версии ' + @NewVersion + 
				' можно только из версии  ' + @PreviousVersion +
				'. Текущая версия: ' + @CurrentVersion
		END
		ELSE
		BEGIN
			--Начинаем транзакцию
			BEGIN TRAN

			--Обновляем версию базы данных
			UPDATE Setting 
			SET DataBaseVersion = @NewVersion		
		END
	END
END
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг установки версии окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

UPDATE 
	Task
SET 
	ContractorId = null,
	DealId = null,
	ProductionOrderId = null
WHERE
	not (DeletionDate is null)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

UPDATE 
	Task	
SET 
	ContractorId = null,
	DealId = null,
	ProductionOrderId = null
WHERE
	exists
	(
		SELECT *
		FROM
			Task t
			inner join Contractor c on c.Id = t.ContractorId
		WHERE
			not (c.DeletionDate is null) and
			Task.Id = t.Id
	)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


 if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_BaseTaskHistoryItem_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[BaseTaskHistoryItem]'))
alter table dbo.[BaseTaskHistoryItem]  drop constraint FK_BaseTaskHistoryItem_CreatedBy

    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskExecutionHistoryItem_Task]') AND parent_object_id = OBJECT_ID('dbo.[TaskExecutionHistoryItem]'))
alter table dbo.[TaskExecutionHistoryItem]  drop constraint FK_TaskExecutionHistoryItem_Task


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskExecutionHistoryItem_TaskExecutionState]') AND parent_object_id = OBJECT_ID('dbo.[TaskExecutionHistoryItem]'))
alter table dbo.[TaskExecutionHistoryItem]  drop constraint FK_TaskExecutionHistoryItem_TaskExecutionState


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskExecutionHistoryItem_TaskType]') AND parent_object_id = OBJECT_ID('dbo.[TaskExecutionHistoryItem]'))
alter table dbo.[TaskExecutionHistoryItem]  drop constraint FK_TaskExecutionHistoryItem_TaskType

    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_Contractor]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_Contractor


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_Deal]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_Deal


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_ExecutedBy]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_ExecutedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_TaskPriority]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_TaskPriority


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_ProductionOrder]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_ProductionOrder


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_TaskType]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_TaskType


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_TaskExecutionState]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_TaskExecutionState


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_TaskExecutionItem]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_TaskExecutionItem

    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Task_ExecutionBy]') AND parent_object_id = OBJECT_ID('dbo.[Task]'))
alter table dbo.[Task]  drop constraint FK_Task_ExecutionBy


-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

DELETE FROM [dbo].[TaskExecutionHistoryItem]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
 
 DELETE FROM [dbo].[TaskHistoryItem]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
 
 DELETE FROM [dbo].[BaseTaskHistoryItem]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[TaskExecutionHistoryItem] add IsDateChanged BIT not null;
GO
alter table dbo.[TaskExecutionHistoryItem] add IsDeletionDateChanged BIT not null;
GO
alter table dbo.[TaskExecutionHistoryItem] add IsResultDescriptionChanged BIT not null;
GO
alter table dbo.[TaskExecutionHistoryItem] add IsSpentTimeChanged BIT not null;
GO
alter table dbo.[TaskExecutionHistoryItem] add IsCompletionPercentageChanged BIT not null;
GO
alter table dbo.[TaskExecutionHistoryItem] add IsTaskExecutionStateChanged BIT not null;
GO
alter table dbo.[TaskExecutionHistoryItem] add IsTaskTypeChanged BIT not null;
GO
alter table dbo.[TaskExecutionHistoryItem] add Date DATETIME null;
GO
alter table dbo.[TaskExecutionHistoryItem] add DeletionDate DATETIME null;
GO
alter table dbo.[TaskExecutionHistoryItem] add ResultDescription VARCHAR(4000) not null;
GO
alter table dbo.[TaskExecutionHistoryItem] add SpentTime INT null;
GO
alter table dbo.[TaskExecutionHistoryItem] add CompletionPercentage TINYINT null;
GO
alter table dbo.[TaskExecutionHistoryItem] add TaskExecutionStateId SMALLINT null;
GO
alter table dbo.[TaskExecutionHistoryItem] add TaskTypeId SMALLINT null;
GO
alter table dbo.[TaskExecutionHistoryItem] drop column TaskExecutionDiff;
GO 
 
 -- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


alter table dbo.[TaskHistoryItem] add  IsContractorChanged BIT not null;
GO
alter table dbo.[TaskHistoryItem] add  IsDeadLineChanged BIT not null;
GO
alter table dbo.[TaskHistoryItem] add  IsDealChanged BIT not null;
GO
alter table dbo.[TaskHistoryItem] add  IsDeletionDateChanged BIT not null;
GO
alter table dbo.[TaskHistoryItem] add  IsDescriptionChanged BIT not null;
GO
alter table dbo.[TaskHistoryItem] add  IsExecutedByChanged BIT not null;
GO
alter table dbo.[TaskHistoryItem] add  IsFactualCompletionDateChanged BIT not null;
GO
alter table dbo.[TaskHistoryItem] add  IsFactualSpentTimeChanged BIT not null;
GO
alter table dbo.[TaskHistoryItem] add  IsTaskPriorityChanged BIT not null;
GO
alter table dbo.[TaskHistoryItem] add  IsProductionOrderChanged BIT not null;
GO
alter table dbo.[TaskHistoryItem] add  IsStartDateChanged BIT not null;
GO
alter table dbo.[TaskHistoryItem] add  IsTopicChanged BIT not null;
GO
alter table dbo.[TaskHistoryItem] add  IsTaskTypeChanged BIT not null;
GO
alter table dbo.[TaskHistoryItem] add  IsTaskExecutionStateChanged BIT not null;
GO
alter table dbo.[TaskHistoryItem] add  Deadline DATETIME null;
GO
alter table dbo.[TaskHistoryItem] add  DeletionDate DATETIME null;
GO
alter table dbo.[TaskHistoryItem] add  Description VARCHAR(255) not null;
GO
alter table dbo.[TaskHistoryItem] add  FactualCompletionDate DATETIME null;
GO
alter table dbo.[TaskHistoryItem] add  FactualSpentTime INT null;
GO
alter table dbo.[TaskHistoryItem] add  StartDate DATETIME null;
GO
alter table dbo.[TaskHistoryItem] add  Topic VARCHAR(255) null;
GO
alter table dbo.[TaskHistoryItem] add  ContractorId INT null;
GO
alter table dbo.[TaskHistoryItem] add  DealId INT null;
GO
alter table dbo.[TaskHistoryItem] add  ExecutedById INT null;
GO
alter table dbo.[TaskHistoryItem] add  TaskPriorityId SMALLINT null;
GO
alter table dbo.[TaskHistoryItem] add  ProductionOrderId UNIQUEIDENTIFIER null;
GO
alter table dbo.[TaskHistoryItem] add  TaskTypeId SMALLINT null;
GO
alter table dbo.[TaskHistoryItem] add  TaskExecutionStateId SMALLINT null;
GO
alter table dbo.[TaskHistoryItem] add  TaskExecutionItemId INT null;
GO
alter table dbo.[TaskHistoryItem] drop column TaskDiff;
GO

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

EXEC sp_rename
    @objname = 'TaskExecutionItem.IsCreateByUser',
    @newname = 'IsCreatedByUser',
    @objtype = 'COLUMN'

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

EXEC sp_rename
    @objname = 'Task.ExecutionById',
    @newname = 'ExecutedById',
    @objtype = 'COLUMN'

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

EXEC sp_rename
    @objname = 'Task.StateId',
    @newname = 'ExecutionStateId',
    @objtype = 'COLUMN'

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[BaseTaskHistoryItem] 
    add constraint FK_BaseTaskHistoryItem_CreatedBy 
    foreign key (CreatedById) 
    references dbo.[User]
    
alter table dbo.[TaskExecutionHistoryItem] 
    add constraint FK_TaskExecutionHistoryItem_Task 
    foreign key (TaskId) 
    references dbo.[Task]

alter table dbo.[TaskExecutionHistoryItem] 
    add constraint FK_TaskExecutionHistoryItem_TaskExecutionState 
    foreign key (TaskExecutionStateId) 
    references dbo.[TaskExecutionState]

alter table dbo.[TaskExecutionHistoryItem] 
    add constraint FK_TaskExecutionHistoryItem_TaskType 
    foreign key (TaskTypeId) 
    references dbo.[TaskType]
alter table dbo.[TaskHistoryItem] 
    add constraint FK_TaskHistoryItem_Contractor 
    foreign key (ContractorId) 
    references dbo.[Contractor]

alter table dbo.[TaskHistoryItem] 
    add constraint FK_TaskHistoryItem_Deal 
    foreign key (DealId) 
    references dbo.[Deal]

alter table dbo.[TaskHistoryItem] 
    add constraint FK_TaskHistoryItem_ExecutedBy 
    foreign key (ExecutedById) 
    references dbo.[User]

alter table dbo.[TaskHistoryItem] 
    add constraint FK_TaskHistoryItem_TaskPriority 
    foreign key (TaskPriorityId) 
    references dbo.[TaskPriority]

alter table dbo.[TaskHistoryItem] 
    add constraint FK_TaskHistoryItem_ProductionOrder 
    foreign key (ProductionOrderId) 
    references dbo.[ProductionOrder]

alter table dbo.[TaskHistoryItem] 
    add constraint FK_TaskHistoryItem_TaskType 
    foreign key (TaskTypeId) 
    references dbo.[TaskType]

alter table dbo.[TaskHistoryItem] 
    add constraint FK_TaskHistoryItem_TaskExecutionState 
    foreign key (TaskExecutionStateId) 
    references dbo.[TaskExecutionState]

alter table dbo.[TaskHistoryItem] 
    add constraint FK_TaskHistoryItem_TaskExecutionItem 
    foreign key (TaskExecutionItemId) 
    references dbo.[TaskExecutionItem]

alter table dbo.[Task] 
    add constraint FK_Task_ExecutedBy 
    foreign key (ExecutedById) 
    references dbo.[User]
        
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

--*****************************************************************************************
--  Удаляем одинаковые исполнения
--*****************************************************************************************
SELECT 
	--t.CreationDate, 
	--t.CreatedById,
	MIN(t.Id) as Id
INTO #T_Id
FROM 
	TaskExecutionItem t
GROUP BY t.CreationDate, t.CreatedById

SELECT t.*
INTO #T
FROM 
	TaskExecutionItem t
	inner join #T_Id tmp on tmp.Id = t.Id
	

DELETE FROM TaskExecutionItem

INSERT INTO TaskExecutionItem (
      [CreationDate]
      ,[Date]
      ,[DeletionDate]
      ,[ResultDescription]
      ,[SpentTime]
      ,[CompletionPercentage]
      ,[IsCreatedByUser]
      ,[ExecutionStateId]
      ,[CreatedById]
      ,[TaskId]
      ,[TaskTypeId])
SELECT 
      [CreationDate]
      ,[Date]
      ,[DeletionDate]
      ,[ResultDescription]
      ,[SpentTime]
      ,[CompletionPercentage]
      ,[IsCreatedByUser]
      ,[ExecutionStateId]
      ,[CreatedById]
      ,[TaskId]
      ,[TaskTypeId]
FROM #T

drop table #T_Id
drop table #T

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

--+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

INSERT INTO [dbo].[BaseTaskHistoryItem](
	  [CreatedById]
      ,[CreationDate])
SELECT
	  d.CreatedById
      ,d.CreationDate      
FROM 
	TaskExecutionItem d

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

 INSERT INTO [dbo].[TaskExecutionHistoryItem](
	  [Id]
	  ,[TaskExecutionItemId]
      ,[TaskId]
      ,[IsDateChanged]
      ,[IsDeletionDateChanged]
      ,[IsResultDescriptionChanged]
      ,[IsSpentTimeChanged]
      ,[IsCompletionPercentageChanged]
      ,[IsTaskExecutionStateChanged]
      ,[IsTaskTypeChanged]
      ,[Date]
      ,[DeletionDate]
      ,[ResultDescription]
      ,[SpentTime]
      ,[TaskExecutionStateId]
      ,[TaskTypeId]
      ,[CompletionPercentage])
SELECT
	  b.Id
	  ,d.Id
      ,d.TaskId
      ,1
      ,1
      ,CASE WHEN LEN(d.ResultDescription) > 0 THEN 1 ELSE 0 END
      ,CASE WHEN d.SpentTime > 0 THEN 1 ELSE 0 END
      ,CASE WHEN d.CompletionPercentage > 0 THEN 1 ELSE 0 END
      ,1	-- Трудно узнать были ли изменения
      ,0
      ,d.Date
      ,null
      ,d.ResultDescription
      ,d.SpentTime
      ,d.ExecutionStateId
      ,d.TaskTypeId
      ,d.CompletionPercentage
FROM 
	TaskExecutionItem d
	inner join BaseTaskHistoryItem b on d.CreationDate = b.CreationDate and d.CreatedById = b.CreatedById
	
 -- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

INSERT INTO [dbo].[BaseTaskHistoryItem](
	  [CreatedById]
      ,[CreationDate])
SELECT
	  d.CreatedById
      ,d.CreationDate      
FROM 
	TaskExecutionItem d
WHERE
	not(d.DeletionDate is null)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

INSERT INTO [dbo].[TaskExecutionHistoryItem](
	  [Id]
	  ,[TaskExecutionItemId]
      ,[TaskId]
      ,[IsDateChanged]
      ,[IsDeletionDateChanged]
      ,[IsResultDescriptionChanged]
      ,[IsSpentTimeChanged]
      ,[IsCompletionPercentageChanged]
      ,[IsTaskExecutionStateChanged]
      ,[IsTaskTypeChanged]
      ,[DeletionDate]
      ,[ResultDescription])
SELECT
	  b.Id	--[Id]
	  ,d.Id	--[TaskExecutionItemId]
	  ,d.TaskId	--[TaskId]
	  ,0	--[IsDateChanged]
	  ,CASE WHEN d.DeletionDate is NULL THEN 0 ELSE 1 END	--[IsDeletionDateChanged]
	  ,0	--[IsResultDescriptionChanged]
      ,0	--[IsSpentTimeChanged]
      ,0	--[IsCompletionPercentageChanged]
      ,0	--[IsTaskExecutionStateChanged]
      ,0	--[IsTaskTypeChanged]
	  ,d.DeletionDate	--[DeletionDate]
	  ,''	--[ResultDescription]
FROM 
	TaskExecutionItem d
	inner join BaseTaskHistoryItem b on 
	d.CreationDate = b.CreationDate and 
	d.CreatedById = b.CreatedById
WHERE
	not(exists(
		SELECT *
		FROM [dbo].[TaskExecutionHistoryItem] tt
		WHERE b.Id = tt.Id))
		

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

--	Добавляем первое изменение, происходящее при создании задачи
INSERT INTO [dbo].[BaseTaskHistoryItem](
	  [CreatedById]
      ,[CreationDate])
SELECT
	  t.CreatedById
      ,t.CreationDate      
FROM 
	Task t
	
-- 
INSERT INTO [dbo].[TaskHistoryItem](
	   [Id]
      ,[TaskId]
      ,[IsDeadLineChanged]
      ,[IsDealChanged]
      ,[IsDeletionDateChanged]
      ,[IsDescriptionChanged]
      ,[IsExecutedByChanged]
      ,[IsFactualCompletionDateChanged]
      ,[IsFactualSpentTimeChanged]
      ,[IsTaskPriorityChanged]
      ,[IsProductionOrderChanged]
      ,[IsStartDateChanged]
      ,[IsTopicChanged]
      ,[IsTaskTypeChanged]
      ,[IsTaskExecutionStateChanged]
      ,[Deadline]
      ,[DeletionDate]
      ,[Description]
      ,[FactualCompletionDate]
      ,[FactualSpentTime]
      ,[StartDate]
      ,[Topic]
      ,[ContractorId]
      ,[DealId]
      ,[ExecutedById]
      ,[TaskPriorityId]
      ,[ProductionOrderId]
      ,[TaskTypeId]
      ,[TaskExecutionStateId]
      ,[TaskExecutionItemId]
      ,[IsContractorChanged])
	SELECT	 b.Id
			,t.Id
			,1--,[IsDeadLineChanged]
			,1 --,[IsDealChanged]
			,1 --,[IsDeletionDateChanged]
			,1 --,[IsDescriptionChanged]
			,1 --,[IsExecutedByChanged]
			,1 --,[IsFactualCompletionDateChanged]
			,1 --,[IsFactualSpentTimeChanged]
			,1 --,[IsTaskPriorityChanged]
			,1 --,[IsProductionOrderChanged]
			,1 --,[IsStartDateChanged]
			,1 --,[IsTopicChanged]
			,1 --,[IsTaskTypeChanged]
			,1 --,[IsTaskExecutionStateChanged]
			,t.Deadline --,[Deadline]
			,null --,[DeletionDate]
			,t.Description --,[Description]
			,null--,[FactualCompletionDate]
			,0 --,[FactualSpentTime]
			,null --,[StartDate]
			,t.Topic --,[Topic]
			,t.ContractorId --,[ContractorId]
			,t.DealId --,[DealId]
			,t.ExecutedById --,[ExecutedById]
			,t.PriorityId --,[TaskPriorityId]
			,t.ProductionOrderId --,[ProductionOrderId]
			,t.TypeId --,[TaskTypeId]
			,t.ExecutionStateId --,[TaskExecutionStateId]
			,null --,[TaskExecutionItemId]
			,1 --,[IsContractorChanged]
FROM 
	Task t
	inner join BaseTaskHistoryItem b on( t.CreationDate = b.CreationDate and t.CreatedById = b.CreatedById)
WHERE not(exists (
	SELECT *
	FROM TaskExecutionHistoryItem tt 
	WHERE b.Id = tt.id))

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

 --	Добавляем остальные изменения
INSERT INTO [dbo].[BaseTaskHistoryItem](
	  [CreatedById]
      ,[CreationDate])
SELECT
	  d.CreatedById
      ,d.CreationDate      
FROM 
	TaskExecutionItem d
-- 

INSERT INTO [dbo].[TaskHistoryItem](
	   [Id]
      ,[TaskId]
      ,[IsDeadLineChanged]
      ,[IsDealChanged]
      ,[IsDeletionDateChanged]
      ,[IsDescriptionChanged]
      ,[IsExecutedByChanged]
      ,[IsFactualCompletionDateChanged]
      ,[IsFactualSpentTimeChanged]
      ,[IsTaskPriorityChanged]
      ,[IsProductionOrderChanged]
      ,[IsStartDateChanged]
      ,[IsTopicChanged]
      ,[IsTaskTypeChanged]
      ,[IsTaskExecutionStateChanged]
      ,[Deadline]
      ,[DeletionDate]
      ,[Description]
      ,[FactualCompletionDate]
      ,[FactualSpentTime]
      ,[StartDate]
      ,[Topic]
      ,[ContractorId]
      ,[DealId]
      ,[ExecutedById]
      ,[TaskPriorityId]
      ,[ProductionOrderId]
      ,[TaskTypeId]
      ,[TaskExecutionStateId]
      ,[TaskExecutionItemId]
      ,[IsContractorChanged])
	SELECT	 b.Id
			,d.TaskId
			,0--,[IsDeadLineChanged]
			,0 --,[IsDealChanged]
			,0 --,[IsDeletionDateChanged]
			,0 --,[IsDescriptionChanged]
			,0 --,[IsExecutedByChanged]
			,0 --,[IsFactualCompletionDateChanged]
			,CASE WHEN d.SpentTime > 0 THEN 1 ELSE 0 END --,[IsFactualSpentTimeChanged]
			,0 --,[IsTaskPriorityChanged]
			,0 --,[IsProductionOrderChanged]
			,0 --,[IsStartDateChanged]
			,0 --,[IsTopicChanged]
			,0 --,[IsTaskTypeChanged]
			,1 --,[IsTaskExecutionStateChanged]
			,null --,[Deadline]
			,null --,[DeletionDate]
			,'' --,[Description]
			,null--,[FactualCompletionDate]
			,d.SpentTime --,[FactualSpentTime]
			,null --,[StartDate]
			,null --,[Topic]
			,null--,[ContractorId]
			,null--,[DealId]
			,null--,[ExecutedById]
			,null--,[TaskPriorityId]
			,null--,[ProductionOrderId]
			,null --,[TaskTypeId]
			,d.ExecutionStateId --,[TaskExecutionStateId]
			,d.Id --,[TaskExecutionItemId]
			,0 --,[IsContractorChanged]
FROM 
	TaskExecutionItem d
	inner join BaseTaskHistoryItem b on d.CreationDate = b.CreationDate and d.CreatedById = b.CreatedById
WHERE
	d.IsCreatedByUser = 0 and
	not(exists(
		SELECT *
		FROM [dbo].[TaskHistoryItem] tt
		WHERE b.Id = tt.Id)) 
		
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

