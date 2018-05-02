/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.32

  Что нового:
	+ Добавлены права по задачам
	+ Добавлены справочники типа задач, их статусов и приоритетов.
	
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

SET @PreviousVersion = '0.9.31' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.32'			-- номер новой версии

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

create table dbo.[BaseTaskHistoryItem] (
        Id INT IDENTITY NOT NULL,
       CreationDate DATETIME not null,
       CreatedById INT not null,
       primary key (Id)
    )
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


create table dbo.[TaskExecutionHistoryItem] (
    Id INT not null,
   TaskExecutionDiff VARBINARY(8000) not null,
   TaskExecutionItemId INT not null,
   TaskId INT not null,
   primary key (Id)
)
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

create table dbo.[TaskHistoryItem] (
    Id INT not null,
   TaskDiff VARBINARY(8000) not null,
   TaskId INT not null,
   primary key (Id)
)
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

create table dbo.[TaskExecutionItem] (
    Id INT IDENTITY NOT NULL,
   CreationDate DATETIME not null,
   Date DATETIME not null,
   DeletionDate DATETIME null,
   ResultDescription VARCHAR(4000) not null,
   SpentTime INT not null,
   CompletionPercentage TINYINT not null,
   IsCreateByUser BIT not null,
   ExecutionStateId SMALLINT not null,
   CreatedById INT not null,
   TaskId INT not null,
   TaskTypeId SMALLINT not null,
   primary key (Id)
)
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

create table dbo.[TaskExecutionState] (
    Id SMALLINT IDENTITY NOT NULL,
   Name VARCHAR(100) not null,
   OrdinalNumber SMALLINT not null,
   ExecutionStateTypeId TINYINT not null,
   TaskTypeId SMALLINT null,
   primary key (Id)
)
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

create table dbo.[Task] (
    Id INT IDENTITY NOT NULL,
   CompletionPercentage TINYINT not null,
   CreationDate DATETIME not null,
   DeadLine DATETIME null,
   DeletionDate DATETIME null,
   Description VARCHAR(8000) not null,
   FactualCompletionDate DATETIME null,
   FactualSpentTime INT not null,
   StartDate DATETIME null,
   Topic VARCHAR(200) not null,
   ContractorId INT null,
   CreatedById INT not null,
   DealId INT null,
   ExecutionById INT null,
   PriorityId SMALLINT null,
   ProductionOrderId UNIQUEIDENTIFIER null,
   StateId SMALLINT not null,
   TypeId SMALLINT not null,
   primary key (Id)
)
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

create table dbo.TaskTask (
    MainTaskId INT not null,
   RelatedTaskId INT not null,
   primary key (MainTaskId, RelatedTaskId)
)
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

create table dbo.[TaskPriority] (
    Id SMALLINT IDENTITY NOT NULL,
   Name VARCHAR(100) not null,
   OrdinalNumber SMALLINT not null unique,
   primary key (Id)
)
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

create table dbo.[TaskType] (
    Id SMALLINT IDENTITY NOT NULL,
   Name VARCHAR(100) not null unique,
   primary key (Id)
)
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[TaskExecutionHistoryItem] 
    add constraint PFK_TaskExecutionHistoryItem 
    foreign key (Id) 
    references dbo.[BaseTaskHistoryItem]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[TaskExecutionHistoryItem] 
    add constraint FK_TaskExecutionItem_TaskExecutionHistoryItem_TaskExecutionItemId 
    foreign key (TaskExecutionItemId) 
    references dbo.[TaskExecutionItem]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[TaskHistoryItem] 
    add constraint PFK_TaskHistoryItem 
    foreign key (Id) 
    references dbo.[BaseTaskHistoryItem]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[TaskHistoryItem] 
    add constraint FK_Task_TaskHistoryItem_TaskId 
    foreign key (TaskId) 
    references dbo.[Task]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[TaskExecutionItem] 
    add constraint FK_TaskExecutionItem_ExecutionState 
    foreign key (ExecutionStateId) 
    references dbo.[TaskExecutionState]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[TaskExecutionItem] 
    add constraint FK_TaskExecutionItem_CreatedBy 
    foreign key (CreatedById) 
    references dbo.[User]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[TaskExecutionItem] 
    add constraint FK_Task_TaskExecutionItem_TaskId 
    foreign key (TaskId) 
    references dbo.[Task]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[TaskExecutionItem] 
    add constraint FK_TaskExecutionItem_TaskType 
    foreign key (TaskTypeId) 
    references dbo.[TaskType]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[TaskExecutionState] 
    add constraint FK_TaskType_TaskExecutionState_TaskTypeId 
    foreign key (TaskTypeId) 
    references dbo.[TaskType]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[Task] 
    add constraint FK_Task_Contractor 
    foreign key (ContractorId) 
    references dbo.[Contractor]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[Task] 
    add constraint FK_Task_CreatedBy 
    foreign key (CreatedById) 
    references dbo.[User]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[Task] 
    add constraint FK_Task_Deal 
    foreign key (DealId) 
    references dbo.[Deal]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[Task] 
    add constraint FK_Task_ExecutionBy 
    foreign key (ExecutionById) 
    references dbo.[User]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[Task] 
    add constraint FK_Task_Priority 
    foreign key (PriorityId) 
    references dbo.[TaskPriority]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[Task] 
    add constraint FK_Task_ProductionOrder 
    foreign key (ProductionOrderId) 
    references dbo.[ProductionOrder]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[Task] 
    add constraint FK_Task_ExecutionState 
    foreign key (StateId) 
    references dbo.[TaskExecutionState]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[Task] 
    add constraint FK_Task_Type 
    foreign key (TypeId) 
    references dbo.[TaskType]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.TaskTask 
    add constraint FK1AD7AD9A960D29D1 
    foreign key (RelatedTaskId) 
    references dbo.[Task]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.TaskTask 
    add constraint PFK_Task_Task 
    foreign key (MainTaskId) 
    references dbo.[Task]
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

INSERT INTO PermissionDistribution (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30001, 1);
INSERT INTO PermissionDistribution (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30002, 1);
INSERT INTO PermissionDistribution (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30003, 1);
INSERT INTO PermissionDistribution (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30004, 1);
INSERT INTO PermissionDistribution (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30101, 1);
INSERT INTO PermissionDistribution (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30102, 1);
INSERT INTO PermissionDistribution (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30103, 1);
INSERT INTO PermissionDistribution (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30104, 1);
INSERT INTO PermissionDistribution (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30105, 1);
INSERT INTO PermissionDistribution (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30106, 1);

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

SET IDENTITY_INSERT [dbo].[TaskType] ON

INSERT INTO TaskType (Name,Id) VALUES ('Задача', 1);
INSERT INTO TaskType (Name,Id) VALUES ('Звонок', 2);
INSERT INTO TaskType (Name,Id) VALUES ('Встреча', 3);
INSERT INTO TaskType (Name,Id) VALUES ('Презентация', 4);
INSERT INTO TaskType (Name,Id) VALUES ('Совещание', 5);
INSERT INTO TaskType (Name,Id) VALUES ('Поздравление', 6);
INSERT INTO TaskType (Name,Id) VALUES ('Мероприятие', 7);

GO
SET IDENTITY_INSERT [dbo].[TaskType] OFF

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Новая', 1, 1, 1);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('В работе', 2, 2, 1);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Выполнена', 3, 3, 1);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Закрыта', 4, 3, 1);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Снята', 5, 3, 1);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Новая', 1, 1, 2);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('В работе', 2, 2, 2);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Выполнена', 3, 3, 2);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Закрыта', 4, 3, 2);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Снята', 5, 3, 2);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Новая', 1, 1, 3);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('В работе', 2, 2, 3);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Выполнена', 3, 3, 3);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Закрыта', 4, 3, 3);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Снята', 5, 3, 3);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Новая', 1, 1, 4);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('В работе', 2, 2, 4);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Выполнена', 3, 3, 4);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Закрыта', 4, 3, 4);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Снята', 5, 3, 4);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Новая', 1, 1, 5);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('В работе', 2, 2, 5);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Выполнена', 3, 3, 5);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Закрыта', 4, 3, 5);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Снята', 5, 3, 5);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Новая', 1, 1, 6);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('В работе', 2, 2, 6);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Выполнена', 3, 3, 6);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Закрыта', 4, 3, 6);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Снята', 5, 3, 6);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Новая', 1, 1, 7);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('В работе', 2, 2, 7);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Выполнена', 3, 3, 7);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Закрыта', 4, 3, 7);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Снята', 5, 3, 7);

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

SET IDENTITY_INSERT [dbo].[TaskPriority] ON

INSERT INTO TaskPriority (Name, OrdinalNumber, Id) VALUES('Немедленный', 1, 1);
INSERT INTO TaskPriority (Name, OrdinalNumber, Id) VALUES('Срочный', 2, 2);
INSERT INTO TaskPriority (Name, OrdinalNumber, Id) VALUES('Высокий', 3, 3);
INSERT INTO TaskPriority (Name, OrdinalNumber, Id) VALUES('Нормальный', 4, 4);
INSERT INTO TaskPriority (Name, OrdinalNumber, Id) VALUES('Низкий', 5, 5);

GO
SET IDENTITY_INSERT [dbo].[TaskPriority] OFF

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

