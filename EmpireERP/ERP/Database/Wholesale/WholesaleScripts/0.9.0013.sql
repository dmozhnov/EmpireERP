/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.13

  Что нового:
	* Исправлены ошибки именования внешних ключей
	* Исправлен тип данных NVARCHAR для полей PhysicalPerson на VARCHAR
	
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

SET @PreviousVersion = '0.9.12' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.13'			-- номер новой версии

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

ALTER TABLE PhysicalPerson ALTER COLUMN PassportSeries VARCHAR(10) NOT NULL;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE PhysicalPerson ALTER COLUMN PassportNumber VARCHAR(10) NOT NULL;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE PhysicalPerson ALTER COLUMN PassportIssuedBy VARCHAR(200) NOT NULL;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE PhysicalPerson ALTER COLUMN PassportDepartmentCode VARCHAR(10) NOT NULL;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKACD7BCB0C92CF08D]') AND parent_object_id = OBJECT_ID('[AccountingPriceList]'))
alter table dbo.[AccountingPriceList]  drop constraint FKACD7BCB0C92CF08D

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKACD7BCB0DE953766]') AND parent_object_id = OBJECT_ID('[AccountingPriceList]'))
alter table dbo.[AccountingPriceList]  drop constraint FKACD7BCB0DE953766

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK7D2386B8AD9E17BD]') AND parent_object_id = OBJECT_ID('AccountingPriceListStorage'))
alter table dbo.AccountingPriceListStorage  drop constraint FK7D2386B8AD9E17BD

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK54157A86FFB99E35]') AND parent_object_id = OBJECT_ID('ProducerManufacturer'))
alter table dbo.ProducerManufacturer  drop constraint FK54157A86FFB99E35

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK5CE03E9C5BBBD660]') AND parent_object_id = OBJECT_ID('DealDealQuota'))
alter table dbo.DealDealQuota  drop constraint FK5CE03E9C5BBBD660

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK8AF9985FAD9E17BD]') AND parent_object_id = OBJECT_ID('TeamStorage'))
alter table dbo.TeamStorage  drop constraint FK8AF9985FAD9E17BD

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKC5CAA47E3E21E430]') AND parent_object_id = OBJECT_ID('TeamDeal'))
alter table dbo.TeamDeal  drop constraint FKC5CAA47E3E21E430

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK63944DB72B40A237]') AND parent_object_id = OBJECT_ID('TeamProductionOrder'))
alter table dbo.TeamProductionOrder  drop constraint FK63944DB72B40A237

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_AccountingPriceList_ArticleAccountingPrice]') AND parent_object_id = OBJECT_ID('[ArticleAccountingPrice]'))
alter table dbo.[ArticleAccountingPrice]  drop constraint FK_AccountingPriceList_ArticleAccountingPrice

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_MovementWaybill_MovementWaybillRow]') AND parent_object_id = OBJECT_ID('[MovementWaybillRow]'))
alter table dbo.[MovementWaybillRow]  drop constraint FK_MovementWaybill_MovementWaybillRow

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[AccountingPriceList] 
	add constraint FK_AccountingPriceDeterminationRule_Storage 
	foreign key (AccountingPriceCalcRuleByCurrentAccountingPriceStorageId) 
	references dbo.[Storage]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[AccountingPriceList] 
    add constraint FK_LastDigitCalcRule_Storage 
    foreign key (LastDigitCalcRuleStorageId) 
    references dbo.[Storage]
    
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[ArticleAccountingPrice] 
    add constraint FK_AccountingPriceList_ArticleAccountingPrice_AccountingPriceListId 
    foreign key (AccountingPriceListId) 
    references dbo.[AccountingPriceList]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.AccountingPriceListStorage 
    add constraint PFK_AccountingPriceList_Storage 
    foreign key (StorageId) 
    references dbo.[Storage]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.ProducerManufacturer 
	add constraint PFK_Producer_Manufacturer 
	foreign key (ManufacturerId) 
	references dbo.[Manufacturer]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
	
alter table dbo.DealDealQuota 
    add constraint PFK_Deal_DealQuota 
    foreign key (DealQuotaId) 
    references dbo.[DealQuota]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
    
alter table dbo.[MovementWaybillRow] 
    add constraint FK_MovementWaybill_MovementWaybillRow_MovementWaybillId 
    foreign key (MovementWaybillId) 
    references dbo.[MovementWaybill]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
    
alter table dbo.TeamStorage 
    add constraint PFK_Team_Storage 
    foreign key (StorageId) 
    references dbo.[Storage]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
    
alter table dbo.TeamDeal 
    add constraint PFK_Team_Deal 
    foreign key (DealId) 
    references dbo.[Deal]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
    
alter table dbo.TeamProductionOrder 
    add constraint PFK_Team_ProductionOrder 
    foreign key (ProductionOrderId) 
    references dbo.[ProductionOrder]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
    
-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

