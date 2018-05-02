/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.15

  Что нового:	
	* Id в таблице [ExactArticleAvailabilityIndicator] теперь имеет тип guid
	* Столбец PreviousWaybillId переименован в PreviousId (guid)
	+ таблицы показателей входящего, исходящего проведенного наличия из ожидаемого наличия и исходящего проведенного наличия из точного наличия
	
	+ во все входящие позиции добавлено поле типа tinyint UsageAsManualSourceCount
	+ во все исходящие позиции добавлено поле типа bit IsUsingManualSource
	+ в WaybillRowArticleMovement добавлено поле типа bit IsManuallyCreated
	
	- столбец ReservedCount у позиций входящих строк
	
	- столбец FinallyMovementDate у позиций входящих строк
	+ столбец AvailableToReserveCount
	
	- столбец AcceptancePendingCount у позиций исходящих накладных
	
	- поле AcceptanceDate из таблицы ChangeOwnerWaybillRow
	
	+ поле AcceptanceDate в таблице WriteoffWaybill
	
	+ поле IsObsolete в таблицу Article
	* поле SalaryPercentFromGroup переименовано в IsSalaryPercentFromGroup
	
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

SET @PreviousVersion = '0.9.14' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.15'			-- номер новой версии

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

-- сохраняем текущие данные во временную таблицу
select *, new_id = NEWID(), PreviousId = CAST(null as uniqueidentifier)
into #T
from [ExactArticleAvailabilityIndicator]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- удаляем старый индекс
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ExactArticleAvailabilityIndicator_StorageId_AccountOrganizationId_ArticleId_BatchId')
DROP INDEX [IX_ExactArticleAvailabilityIndicator_StorageId_AccountOrganizationId_ArticleId_BatchId] ON [dbo].[ExactArticleAvailabilityIndicator]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- удаляем старую таблицу
if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ExactArticleAvailabilityIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ExactArticleAvailabilityIndicator]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- обновляем код предыдущей записи
update T
set PreviousId = 
(
	select new_id
	from #T
	where StorageId = T.StorageId and AccountOrganizationId = T.AccountOrganizationId and BatchId = T.BatchId and WaybillId = T.PreviousWaybillId
)
from #T T
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- создаем новую таблицу
create table dbo.[ExactArticleAvailabilityIndicator] (
    Id UNIQUEIDENTIFIER not null,
   StartDate DATETIME not null,
   EndDate DATETIME null,
   StorageId SMALLINT not null,
   AccountOrganizationId INT not null,
   ArticleId INT not null,
   BatchId UNIQUEIDENTIFIER not null,
   PurchaseCost DECIMAL(18, 6) not null,
   Count DECIMAL(18, 6) not null,
   PreviousId UNIQUEIDENTIFIER null,
   primary key (Id)
)
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- заливаем измененные данные в новую таблицу
insert into [ExactArticleAvailabilityIndicator] (Id, StartDate, EndDate, StorageId, AccountOrganizationId, ArticleId, BatchId, PurchaseCost, Count, PreviousId)
select new_id, StartDate, EndDate, StorageId, AccountOrganizationId, ArticleId, BatchId, PurchaseCost, Count, PreviousId
from #T
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- создаем новый индекс
CREATE INDEX [IX_ExactArticleAvailabilityIndicator_StorageId_AccountOrganizationId_ArticleId_BatchId] ON [ExactArticleAvailabilityIndicator] ([StorageId], [AccountOrganizationId], [ArticleId], [BatchId])
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- удаляем временную таблицу
drop table #T
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- создание таблицы показателей входящего проведенного наличия
create table dbo.[IncomingAcceptedArticleAvailabilityIndicator] (
    Id UNIQUEIDENTIFIER not null,
   StartDate DATETIME not null,
   EndDate DATETIME null,
   StorageId SMALLINT not null,
   AccountOrganizationId INT not null,
   ArticleId INT not null,
   BatchId UNIQUEIDENTIFIER not null,
   PurchaseCost DECIMAL(18, 6) not null,
   Count DECIMAL(18, 6) not null,
   PreviousId UNIQUEIDENTIFIER null,
   primary key (Id)
)
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- создание таблицы показателей исходящего проведенного наличия, которое берется из проведенного входящего
create table dbo.[OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator] (
    Id UNIQUEIDENTIFIER not null,
   StartDate DATETIME not null,
   EndDate DATETIME null,
   StorageId SMALLINT not null,
   AccountOrganizationId INT not null,
   ArticleId INT not null,
   BatchId UNIQUEIDENTIFIER not null,
   PurchaseCost DECIMAL(18, 6) not null,
   Count DECIMAL(18, 6) not null,
   PreviousId UNIQUEIDENTIFIER null,
   primary key (Id)
)
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- создание таблицы показателей исходящего проведенного наличия, которое берется из точного входящего
create table dbo.[OutgoingAcceptedFromExactArticleAvailabilityIndicator] (
    Id UNIQUEIDENTIFIER not null,
   StartDate DATETIME not null,
   EndDate DATETIME null,
   StorageId SMALLINT not null,
   AccountOrganizationId INT not null,
   ArticleId INT not null,
   BatchId UNIQUEIDENTIFIER not null,
   PurchaseCost DECIMAL(18, 6) not null,
   Count DECIMAL(18, 6) not null,
   PreviousId UNIQUEIDENTIFIER null,
   primary key (Id)
)
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


ALTER TABLE dbo.[ReceiptWaybillRow] ADD UsageAsManualSourceCount tinyint not null
	CONSTRAINT DF_ReceiptWaybillRow_UsageAsManualSourceCount DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ReceiptWaybillRow] DROP CONSTRAINT DF_ReceiptWaybillRow_UsageAsManualSourceCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[MovementWaybillRow] ADD UsageAsManualSourceCount tinyint not null
	CONSTRAINT DF_MovementWaybillRow_UsageAsManualSourceCount DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[MovementWaybillRow] DROP CONSTRAINT DF_MovementWaybillRow_UsageAsManualSourceCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ReturnFromClientWaybillRow] ADD UsageAsManualSourceCount tinyint not null
	CONSTRAINT DF_ReturnFromClientWaybillRow_UsageAsManualSourceCount DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ReturnFromClientWaybillRow] DROP CONSTRAINT DF_ReturnFromClientWaybillRow_UsageAsManualSourceCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ChangeOwnerWaybillRow] ADD UsageAsManualSourceCount tinyint not null
	CONSTRAINT DF_ChangeOwnerWaybillRow_UsageAsManualSourceCount DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ChangeOwnerWaybillRow] DROP CONSTRAINT DF_ChangeOwnerWaybillRow_UsageAsManualSourceCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ChangeOwnerWaybillRow] ADD IsUsingManualSource  bit not null
	CONSTRAINT DF_ChangeOwnerWaybillRow_IsUsingManualSource DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ChangeOwnerWaybillRow] DROP CONSTRAINT DF_ChangeOwnerWaybillRow_IsUsingManualSource

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ExpenditureWaybillRow] ADD IsUsingManualSource  bit not null
	CONSTRAINT DF_ExpenditureWaybillRow_IsUsingManualSource DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ExpenditureWaybillRow] DROP CONSTRAINT DF_ExpenditureWaybillRow_IsUsingManualSource

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[WriteoffWaybillRow] ADD IsUsingManualSource  bit not null
	CONSTRAINT DF_WriteoffWaybillRow_IsUsingManualSource DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[WriteoffWaybillRow] DROP CONSTRAINT DF_WriteoffWaybillRow_IsUsingManualSource

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[MovementWaybillRow] ADD IsUsingManualSource  bit not null
	CONSTRAINT DF_MovementWaybillRow_IsUsingManualSource DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[MovementWaybillRow] DROP CONSTRAINT DF_MovementWaybillRow_IsUsingManualSource

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[WaybillRowArticleMovement] ADD IsManuallyCreated  bit not null
	CONSTRAINT DF_WaybillRowArticleMovement_IsManuallyCreated DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[WaybillRowArticleMovement] DROP CONSTRAINT DF_WaybillRowArticleMovement_IsManuallyCreated

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

update ExpenditureWaybill
set ExpenditureWaybillStateId = 1
where ExpenditureWaybillStateId in (2, 3)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

update MovementWaybill
set MovementWaybillStateId = 1
where MovementWaybillStateId in (2, 3, 4)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

update ChangeOwnerWaybill
set ChangeOwnerWaybillStateId = 1
where ChangeOwnerWaybillStateId in (2, 3, 4)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId')
DROP INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [dbo].[ReceiptWaybillRow] 

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ReceiptWaybillRow] DROP COLUMN ReservedCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[MovementWaybillRow] DROP COLUMN ReservedCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ReturnFromClientWaybillRow] DROP COLUMN ReservedCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ChangeOwnerWaybillRow] DROP COLUMN ReservedCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ReceiptWaybillRow] DROP COLUMN FinallyMovementDate

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ReceiptWaybillRow] ADD AvailableToReserveCount numeric(18,6) not null CONSTRAINT DF_AvailableToReserveCount DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ReceiptWaybillRow] DROP CONSTRAINT DF_AvailableToReserveCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

UPDATE RR
SET AvailableToReserveCount = PendingCount - AcceptedCount - ShippedCount - FinallyMovedCount
FROM [ReceiptWaybillRow] RR
JOIN [ReceiptWaybill] R ON R.Id = RR.ReceiptWaybillId AND R.ReceiptWaybillStateId > 1

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

CREATE INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [ReceiptWaybillRow] ([DeletionDate], [ReceiptWaybillId]) 
INCLUDE ([Id], [ArticleMeasureUnitScale], [PendingCount], [PendingSum], [ReceiptedCount], [ProviderCount], [ApprovedCount], [ApprovedSum], [PurchaseCost], [CustomsDeclarationNumber], [CreationDate], [AvailableToReserveCount], [AcceptedCount], [ShippedCount], [FinallyMovedCount], [RecipientArticleAccountingPriceId], [ArticleId], [PendingValueAddedTaxId], [ApprovedValueAddedTaxId], [CountryId], [ManufacturerId], [InitialPurchaseCost], [ProviderSum], [ApprovedPurchaseCost])

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[MovementWaybillRow] DROP COLUMN FinallyMovementDate

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[MovementWaybillRow] ADD AvailableToReserveCount numeric(18,6) not null CONSTRAINT DF_AvailableToReserveCount DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[MovementWaybillRow] DROP CONSTRAINT DF_AvailableToReserveCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

UPDATE MR
SET AvailableToReserveCount = MovingCount - AcceptedCount - ShippedCount - FinallyMovedCount
FROM [MovementWaybillRow] MR
JOIN [MovementWaybill] R ON R.Id = MR.MovementWaybillId AND R.MovementWaybillStateId > 1

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ReturnFromClientWaybillRow] DROP COLUMN FinallyMovementDate

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ReturnFromClientWaybillRow] ADD AvailableToReserveCount numeric(18,6) not null CONSTRAINT DF_AvailableToReserveCount DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ReturnFromClientWaybillRow] DROP CONSTRAINT DF_AvailableToReserveCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

UPDATE RR
SET AvailableToReserveCount = ReturnCount - AcceptedCount - ShippedCount - FinallyMovedCount
FROM [ReturnFromClientWaybillRow] RR
JOIN [ReturnFromClientWaybill] R ON R.Id = RR.ReturnFromClientWaybillId AND R.ReturnFromClientWaybillStateId > 1

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ChangeOwnerWaybillRow] DROP COLUMN FinallyMovementDate

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ChangeOwnerWaybillRow] ADD AvailableToReserveCount numeric(18,6) not null CONSTRAINT DF_AvailableToReserveCount DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ChangeOwnerWaybillRow] DROP CONSTRAINT DF_AvailableToReserveCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

UPDATE CR
SET AvailableToReserveCount = MovingCount - AcceptedCount - ShippedCount - FinallyMovedCount
FROM [ChangeOwnerWaybillRow] CR
JOIN [ChangeOwnerWaybill] R ON R.Id = CR.ChangeOwnerWaybillId AND R.ChangeOwnerWaybillStateId > 1

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


alter table ChangeOwnerWaybillRow drop column AcceptancePendingCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table MovementWaybillRow drop column AcceptancePendingCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table ExpenditureWaybillRow drop column AcceptancePendingCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table WriteoffWaybillRow drop column AcceptancePendingCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table ChangeOwnerWaybillRow drop column AcceptanceDate


-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table WriteoffWaybill add AcceptanceDate datetime 


-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


EXEC sp_rename 'Article.SalaryPercentFromGroup', 'IsSalaryPercentFromGroup';

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 2 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[Article] ADD
	[IsObsolete] BIT not null CONSTRAINT DF_Article_IsObsolete DEFAULT 0

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 3 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[Article] DROP CONSTRAINT DF_Article_IsObsolete

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

