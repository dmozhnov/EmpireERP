/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.X

  Что нового:
	+ права "Проводка накладной списания" и "Отмена проводки накладной списания"
	+ Поле ArticleId в позиции всех накладных
	
	* В таблицах ExactArticlePriceChangeIndicator, AcceptedArticlePriceChangeIndicator, ArticleMovementFactualFinancialIndicator, 
	ArticleMovementOperationCountIndicator, ReturnFromClientIndicator и SaleIndicator поле Id стало иметь тип Guid
	+ поле ReturnedCount в таблицу SaleWaybillRow
	+ Поле TeamId в таблицу SaleWaybill
	
	* удалена связь внешнего ключа FK_AccountingPriceDeterminationRule_Storage
	* поле OrdinalNumber таблицы ArticleAccountingPrice становится nullable и перестает быть Identity
	
	* обратно добавлена связь внешнего ключа FK_AccountingPriceDeterminationRule_Storage	
	
	* изменен маппинг поля Provider в приходной накладной
	+ Добавлено новое право роли "Администратор"
	
	- Удалены столбцы с показателями из таблицы реестров цен

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

SET @PreviousVersion = '0.9.15' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.16'			-- номер новой версии

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

INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1507, 1)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1508, 1)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

---------------------------------------------------------------------------------

ALTER TABLE [MovementWaybillRow] Add ArticleId INT

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE [ChangeOwnerWaybillRow] Add ArticleId INT

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE [WriteoffWaybillRow] Add ArticleId INT

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE [SaleWaybillRow] Add ArticleId INT

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE [ReturnFromClientWaybillRow] Add ArticleId INT

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

update M
set ArticleId = R.ArticleId
from MovementWaybillRow M
join ReceiptWaybillRow R on R.Id = M.ReceiptWaybillRowId

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

update C
set ArticleId = R.ArticleId
from ChangeOwnerWaybillRow C
join ReceiptWaybillRow R on R.Id = C.ChangeOwnerWaybillRowReceiptWaybillRowId

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

update W
set ArticleId = R.ArticleId
from WriteoffWaybillRow W
join ReceiptWaybillRow R on R.Id = W.WriteoffReceiptWaybillRowId

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

update S
set ArticleId = R.ArticleId
from SaleWaybillRow S
join ExpenditureWaybillRow E on E.Id = S.Id
join ReceiptWaybillRow R on R.Id = E.ExpenditureWaybillRowReceiptWaybillRowId

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

update RR
set ArticleId = R.ArticleId
from ReturnFromClientWaybillRow RR
join SaleWaybillRow S on S.Id = RR.SaleWaybillRowId
join ExpenditureWaybillRow E on E.Id = S.Id
join ReceiptWaybillRow R on R.Id = E.ExpenditureWaybillRowReceiptWaybillRowId

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE [MovementWaybillRow] ALTER COLUMN ArticleId INT not null

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE [ChangeOwnerWaybillRow] ALTER COLUMN ArticleId INT not null

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE [WriteoffWaybillRow] ALTER COLUMN ArticleId INT not null

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE [SaleWaybillRow] ALTER COLUMN ArticleId INT not null

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE [ReturnFromClientWaybillRow] ALTER COLUMN ArticleId INT not null

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[MovementWaybillRow] 
    add constraint FK_MovementWaybillRow_Article 
    foreign key (ArticleId) 
    references dbo.[Article]

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[ReturnFromClientWaybillRow] 
    add constraint FK_ReturnFromClientWaybillRow_Article 
    foreign key (ArticleId) 
    references dbo.[Article]

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[ChangeOwnerWaybillRow] 
    add constraint FK_ChangeOwnerWaybillRow_Article 
    foreign key (ArticleId) 
    references dbo.[Article]

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[SaleWaybillRow] 
    add constraint FK_SaleWaybillRow_Article 
    foreign key (ArticleId) 
    references dbo.[Article]

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
        
alter table dbo.[WriteoffWaybillRow] 
    add constraint FK_WriteoffWaybillRow_Article 
    foreign key (ArticleId) 
    references dbo.[Article]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-------------------------------------------------------------------------------------------


IF EXISTS(select * from ReturnFromClientWaybill where ReturnFromClientWaybillStateId = 2)
BEGIN
	RAISERROR('Ошибка! В БД не должно быть проведенных и не принятых окончательно накладных возврата от клиента.',16,1)
END

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

select *, New_id = NEWID()
into #ExactArticlePriceChangeIndicator
from ExactArticlePriceChangeIndicator

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table ExactArticlePriceChangeIndicator

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

create table dbo.[ExactArticlePriceChangeIndicator] (
    Id UNIQUEIDENTIFIER not null,
   StartDate DATETIME not null,
   EndDate DATETIME null,
   PreviousId UNIQUEIDENTIFIER null,
   AccountingPriceListId UNIQUEIDENTIFIER not null,
   StorageId SMALLINT not null,
   AccountingPriceSum DECIMAL(18, 2) not null,
   primary key (Id)
)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into ExactArticlePriceChangeIndicator (Id, StartDate, EndDate, PreviousId, AccountingPriceListId, StorageId, AccountingPriceSum)
select E.New_id, E.StartDate, E.EndDate, EE.New_id, E.AccountingPriceListId, E.StorageId, E.AccountingPriceSum
from #ExactArticlePriceChangeIndicator E
left join #ExactArticlePriceChangeIndicator EE ON EE.Id = E.PreviousIndicatorId 

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table #ExactArticlePriceChangeIndicator

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-------------------------------

select *, New_id = NEWID()
into #ArticleMovementOperationCountIndicator
from ArticleMovementOperationCountIndicator

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table ArticleMovementOperationCountIndicator
  
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
    
create table dbo.[ArticleMovementOperationCountIndicator] (
    Id UNIQUEIDENTIFIER not null,
   StartDate DATETIME not null,
   EndDate DATETIME null,
   PreviousId UNIQUEIDENTIFIER null,
   ArticleMovementOperationType TINYINT not null,
   StorageId SMALLINT not null,
   Count INT not null,
   primary key (Id)
)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into ArticleMovementOperationCountIndicator (Id, StartDate, EndDate, PreviousId, ArticleMovementOperationType, StorageId, [Count])
select A.new_Id, A.StartDate, A.EndDate, AA.New_id, A.ArticleMovementOperationType, A.StorageId, A.[Count]
from #ArticleMovementOperationCountIndicator A
left join #ArticleMovementOperationCountIndicator AA on AA.Id = A.PreviousIndicatorId

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table #ArticleMovementOperationCountIndicator

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

------------------------------------

select *, New_id = NEWID()
into #AcceptedArticlePriceChangeIndicator
from AcceptedArticlePriceChangeIndicator

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table AcceptedArticlePriceChangeIndicator

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

create table dbo.[AcceptedArticlePriceChangeIndicator] (
    Id UNIQUEIDENTIFIER not null,
   StartDate DATETIME not null,
   EndDate DATETIME null,
   PreviousId UNIQUEIDENTIFIER null,
   AccountingPriceListId UNIQUEIDENTIFIER not null,
   AccountOrganizationId INT not null,
   StorageId SMALLINT not null,
   NeedToRecalculate BIT not null,
   ChangeSum DECIMAL(18, 2) not null,
   primary key (Id)
)    

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into AcceptedArticlePriceChangeIndicator (Id, StartDate, EndDate, PreviousId, AccountingPriceListId, AccountOrganizationId,
   StorageId, NeedToRecalculate, ChangeSum)
select A.new_Id, A.StartDate, A.EndDate, AA.New_id, A.AccountingPriceListId, A.AccountOrganizationId, A.StorageId, A.NeedToRecalculate, A.ChangeSum
from #AcceptedArticlePriceChangeIndicator A    
left join #AcceptedArticlePriceChangeIndicator AA on AA.Id = A.PreviousIndicatorId 

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table #AcceptedArticlePriceChangeIndicator

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

------------------------------------------    
    
select *, New_id = NEWID()
into #ArticleMovementFactualFinancialIndicator
from ArticleMovementFactualFinancialIndicator    

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table ArticleMovementFactualFinancialIndicator
  
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
    
create table dbo.[ArticleMovementFactualFinancialIndicator] (
    Id UNIQUEIDENTIFIER not null,
   StartDate DATETIME not null,
   EndDate DATETIME null,
   RecipientId INT null,
   RecipientStorageId SMALLINT null,
   SenderId INT null,
   SenderStorageId SMALLINT null,
   PreviousId UNIQUEIDENTIFIER null,
   WaybillId UNIQUEIDENTIFIER not null,
   ArticleMovementOperationType TINYINT not null,
   AccountingPriceSum DECIMAL(18, 2) not null,
   PurchaseCostSum DECIMAL(18, 6) not null,
   SalePriceSum DECIMAL(18, 2) not null,
   primary key (Id)
)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into ArticleMovementFactualFinancialIndicator (Id, StartDate, EndDate, RecipientId, RecipientStorageId, SenderId, SenderStorageId,
   PreviousId, WaybillId, ArticleMovementOperationType, AccountingPriceSum, PurchaseCostSum, SalePriceSum)
select A.new_Id, A.StartDate, A.EndDate, A.RecipientId, A.RecipientStorageId, A.SenderId, A.SenderStorageId,
   AA.New_id, A.WaybillId, A.ArticleMovementOperationType, A.AccountingPriceSum, A.PurchaseCostSum, A.SalePriceSum
from #ArticleMovementFactualFinancialIndicator A
left join #ArticleMovementFactualFinancialIndicator AA on AA.Id = A.PreviousIndicatorId

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table #ArticleMovementFactualFinancialIndicator

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

---------------------------------------------------------

select *, New_id = NEWID()
into #ReturnFromClientIndicator
from ReturnFromClientIndicator    

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table ReturnFromClientIndicator

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

create table dbo.[ReceiptedReturnFromClientIndicator] (
    Id UNIQUEIDENTIFIER not null,
   StartDate DATETIME not null,
   EndDate DATETIME null,
   StorageId SMALLINT not null,
   SaleWaybillCuratorId INT not null,
   ReturnFromClientWaybillCuratorId INT not null,
   ContractorId INT not null,
   ClientOrganizationId INT not null,
   TeamId SMALLINT null,
   ClientId INT not null,
   DealId INT not null,
   AccountOrganizationId INT not null,
   ArticleId INT not null,
   SaleWaybillId UNIQUEIDENTIFIER not null,
   BatchId UNIQUEIDENTIFIER not null,
   PreviousId UNIQUEIDENTIFIER null,
   PurchaseCostSum DECIMAL(18, 6) not null,
   AccountingPriceSum DECIMAL(18, 2) not null,
   SalePriceSum DECIMAL(18, 2) not null,
   ReturnedCount DECIMAL(18, 6) not null,
   primary key (Id)
)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into ReceiptedReturnFromClientIndicator (Id, StartDate, EndDate, StorageId, SaleWaybillCuratorId, ReturnFromClientWaybillCuratorId, ContractorId, ClientOrganizationId, TeamId, ClientId, DealId, PreviousId, AccountOrganizationId, ArticleId, SaleWaybillId, BatchId, AccountingPriceSum, PurchaseCostSum, SalePriceSum, ReturnedCount)
select R.new_Id, R.StartDate, R.EndDate, R.StorageId, R.UserId, R.UserId, R.ProviderId, R.ClientOrganizationId, R.TeamId, R.ClientId, R.DealId,
   RR.new_Id, R.AccountOrganizationId, R.ArticleId, R.SaleWaybillId, R.BatchId, R.AccountingPriceSum, R.PurchaseCostSum, R.SalePriceSum, R.ReturnedCount
from #ReturnFromClientIndicator R
left join #ReturnFromClientIndicator RR on RR.ReturnFromClientWaybillId = R.PreviousWaybillId and RR.BatchId = R.BatchId

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table #ReturnFromClientIndicator

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

----------------------------------------------------------------

select *, New_id = NEWID()
into #SaleIndicator
from SaleIndicator    
    
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
    
drop table SaleIndicator

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

create table dbo.[SaleIndicator] (
    Id UNIQUEIDENTIFIER not null,
   StartDate DATETIME not null,
   EndDate DATETIME null,
   StorageId SMALLINT not null,
   UserId INT not null,
   ContractorId INT not null,
   ClientOrganizationId INT not null,
   TeamId SMALLINT null,
   ClientId INT not null,
   AccountOrganizationId INT not null,
   ArticleId INT not null,
   BatchId UNIQUEIDENTIFIER not null,
   DealId INT not null,
   PreviousId UNIQUEIDENTIFIER null,
   AccountingPriceSum DECIMAL(18, 2) not null,
   PurchaseCostSum DECIMAL(18, 6) not null,
   SalePriceSum DECIMAL(18, 2) not null,
   SoldCount DECIMAL(18, 6) not null,
   primary key (Id)
)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into SaleIndicator (Id, StartDate, EndDate, StorageId, UserId, ContractorId, ClientOrganizationId, TeamId, ClientId, AccountOrganizationId,
   ArticleId, BatchId, DealId, PreviousId, AccountingPriceSum, PurchaseCostSum, SalePriceSum, SoldCount)
select S.New_id, S.StartDate, S.EndDate, S.StorageId, S.UserId, S.ProviderId, S.ClientOrganizationId, S.TeamId, S.ClientId, S.AccountOrganizationId,
   S.ArticleId, S.BatchId, SW.DealId, SS.new_id, S.AccountingPriceSum, S.PurchaseCostSum, S.SalePriceSum, S.SoldCount
from #SaleIndicator S
join SaleWaybill SW on SW.Id = S.SaleWaybillId
left join #SaleIndicator SS on SS.SaleWaybillId = S.PreviousWaybillId and SS.BatchId = S.BatchId

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table #SaleIndicator

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE SaleWaybillRow ADD AcceptedReturnCount NUMERIC(18, 6) not null CONSTRAINT DF_AcceptedReturnCount DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE SaleWaybillRow DROP CONSTRAINT DF_AcceptedReturnCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE SaleWaybillRow ADD ReceiptedReturnCount NUMERIC(18, 6) not null CONSTRAINT DF_ReceiptedReturnCount DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE SaleWaybillRow DROP CONSTRAINT DF_ReceiptedReturnCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE SaleWaybillRow ADD AvailableToReturnCount NUMERIC(18, 6) not null CONSTRAINT DF_AvailableToReturnCount DEFAULT 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE SaleWaybillRow DROP CONSTRAINT DF_AvailableToReturnCount

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

update SR
set ReceiptedReturnCount = 
(
	select ISNULL(SUM(RR.ReturnCount), 0)
	from ReturnFromClientWaybillRow RR 
	where RR.SaleWaybillRowId = SR.Id and RR.DeletionDate is null
),
AvailableToReturnCount = SellingCount -
(
	select ISNULL(SUM(RR.ReturnCount), 0)
	from ReturnFromClientWaybillRow RR 
	where RR.SaleWaybillRowId = SR.Id and RR.DeletionDate is null
)
from SaleWaybillRow SR

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table SaleWaybill add TeamId SMALLINT null

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

create table dbo.[AcceptedReturnFromClientIndicator] (
    Id UNIQUEIDENTIFIER not null,
   StartDate DATETIME not null,
   EndDate DATETIME null,
   StorageId SMALLINT not null,
   SaleWaybillCuratorId INT not null,
   ReturnFromClientWaybillCuratorId INT not null,
   ContractorId INT not null,
   ClientOrganizationId INT not null,
   TeamId SMALLINT null,
   ClientId INT not null,
   DealId INT not null,
   AccountOrganizationId INT not null,
   ArticleId INT not null,
   SaleWaybillId UNIQUEIDENTIFIER not null,
   BatchId UNIQUEIDENTIFIER not null,
   PreviousId UNIQUEIDENTIFIER null,
   PurchaseCostSum DECIMAL(18, 6) not null,
   AccountingPriceSum DECIMAL(18, 2) not null,
   SalePriceSum DECIMAL(18, 2) not null,
   ReturnedCount DECIMAL(18, 6) not null,
   primary key (Id)
)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-----------------------------------------------------------------------------------


drop index IX_ArticleAccountingPrice_DeletionDate_AccountingPriceListId_ArticleId ON [dbo].[ArticleAccountingPrice] 

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.AccountingPriceList drop constraint FK_AccountingPriceDeterminationRule_Storage

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.ArticleAccountingPrice drop column OrdinalNumber

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.ArticleAccountingPrice add OrdinalNumber int

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

CREATE NONCLUSTERED INDEX [IX_ArticleAccountingPrice_DeletionDate_AccountingPriceListId_ArticleId] ON [dbo].[ArticleAccountingPrice] 
(
	[DeletionDate] ASC,
	[AccountingPriceListId] ASC,
	[ArticleId] ASC
)
INCLUDE ( [Id],
[AccountingPrice],
[CreationDate],
[UsedDefaultAccountingPriceCalcRule],
[UsedDefaultLastDigitRule],
[OrdinalNumber]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-------------------------------------------------------------------------------------------------


update dbo.AccountingPriceList set AccountingPriceCalcRuleByCurrentAccountingPriceStorageId = null where AccountingPriceCalcRuleByCurrentAccountingPriceStorageId = 0

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


alter table dbo.AccountingPriceList
        add constraint FK_AccountingPriceDeterminationRule_Storage 
        foreign key (AccountingPriceCalcRuleByCurrentAccountingPriceStorageId) 
        references dbo.[Storage]

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-----------------------------------------------------------------------------------------------


ALTER TABLE dbo.[ReceiptWaybill] ALTER COLUMN ProviderId INT null

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 2 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

INSERT INTO dbo.[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId)
VALUES (3, 1314, 1)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 3 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

----------------------------------------------------------------------------------------------


ALTER TABLE dbo.[AccountingPriceList] DROP COLUMN [PurchaseCostSum];

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 2 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[AccountingPriceList] DROP COLUMN [OldAccountingPriceSum];

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 3 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[AccountingPriceList] DROP COLUMN [NewAccountingPriceSum];

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 4 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

--------------------------------------------------------------------------------------------


ALTER TABLE dbo.[WriteoffWaybill] DROP COLUMN [PurchaseCostSum];

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 2 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ReturnFromClientWaybill] DROP COLUMN [PurchaseCostSum];

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 3 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[MovementWaybill] DROP COLUMN [PurchaseCostSum];

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 4 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ExpenditureWaybill] DROP COLUMN [PurchaseCostSum];

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 5 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ChangeOwnerWaybill] DROP COLUMN [PurchaseCostSum];

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 6 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO



-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

