/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.20

  Что нового:
	- таблица SaleIndicator
	+ таблицы AcceptedSaleIndicator и ReceiptedSaleIndicator
	
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

SET @PreviousVersion = '0.9.19' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.20'			-- номер новой версии

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

select *
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

create table dbo.[ShippedSaleIndicator] (
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
   PurchaseCostSum DECIMAL(18, 6) not null,
   AccountingPriceSum DECIMAL(18, 2) not null,
   SalePriceSum DECIMAL(18, 2) not null,
   SoldCount DECIMAL(18, 6) not null,
   primary key (Id)
)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into [ShippedSaleIndicator] (Id, StartDate, EndDate, StorageId, UserId, ContractorId, ClientOrganizationId, TeamId, ClientId, AccountOrganizationId,
   ArticleId, BatchId, DealId, PreviousId, PurchaseCostSum, AccountingPriceSum, SalePriceSum, SoldCount)
select Id, StartDate, EndDate, StorageId, UserId, ContractorId, ClientOrganizationId, TeamId, ClientId, AccountOrganizationId,
   ArticleId, BatchId, DealId, PreviousId, PurchaseCostSum, AccountingPriceSum, SalePriceSum, SoldCount
from #SaleIndicator
   
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
  
create table dbo.[AcceptedSaleIndicator] (
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
   PurchaseCostSum DECIMAL(18, 6) not null,
   AccountingPriceSum DECIMAL(18, 2) not null,
   SalePriceSum DECIMAL(18, 2) not null,
   SoldCount DECIMAL(18, 6) not null,
   primary key (Id)
)
   
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

select Id = newid(), EW.AcceptanceDate, EW.ExpenditureWaybillSenderStorageId, SW.SaleWaybillCuratorId, R.ProviderId, C.ContractorOrganizationId, SW.TeamId, D.ClientId,
	C.AccountOrganizationId, RW.ArticleId, ReceiptWaybillRowId = RW.Id, DealId = D.Id, 
	PurchaseCostSum = Round(RW.PurchaseCost * SR.SellingCount, 6),
	AccountingPriceSum = Round(AAP.AccountingPrice * SR.SellingCount,2),
	SalePriceSum = Round(SR.SalePrice * SR.SellingCount,2),
	SR.SellingCount,
	RowNumber = ROW_NUMBER() OVER ( partition by EW.ExpenditureWaybillSenderStorageId, SW.SaleWaybillCuratorId, R.ProviderId, C.ContractorOrganizationId, SW.TeamId, D.ClientId,
		C.AccountOrganizationId, RW.ArticleId, RW.Id, D.Id order by EW.AcceptanceDate),
	-- хэш-ключ для упрощения связывания таблицы самой с собой
	KeyHash = HashBytes('SHA1', 
		CAST(EW.ExpenditureWaybillSenderStorageId as varchar(255)) + '_' +
		CAST(SW.SaleWaybillCuratorId as varchar(255)) + '_' +
		CAST(R.ProviderId as varchar(255)) + '_' +
		CAST(C.ContractorOrganizationId as varchar(255)) + '_' +
		CAST(SW.TeamId as varchar(255)) + '_' +
		CAST(D.ClientId as varchar(255)) + '_' +
		CAST(C.AccountOrganizationId as varchar(255)) + '_' +
		CAST(RW.ArticleId as varchar(255)) + '_' +
		CAST(RW.Id as varchar(255)) + '_' +
		CAST(D.Id as varchar(255))
	)
into #T	
from SaleWaybillRow SR
join ExpenditureWaybillRow ER on ER.Id = SR.Id
join ExpenditureWaybill EW on EW.Id = SR.SaleWaybillId
join SaleWaybill SW on SW.Id = EW.Id
join ReceiptWaybillRow RW on RW.Id = ER.ExpenditureWaybillRowReceiptWaybillRowId
join ReceiptWaybill R on R.Id = RW.ReceiptWaybillId
join Deal D on D.Id = SW.DealId
join Contract C on C.Id = D.ClientContractId
join ArticleAccountingPrice AAP on AAP.Id = ER.ExpenditureWaybillSenderArticleAccountingPriceId
where SR.DeletionDate is null and SW.DeletionDate is null

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into [AcceptedSaleIndicator] (Id, StartDate, EndDate, StorageId, UserId, ContractorId, ClientOrganizationId, TeamId, ClientId, AccountOrganizationId,
   ArticleId, BatchId, DealId, PreviousId, PurchaseCostSum, AccountingPriceSum, SalePriceSum, SoldCount)
select T.Id, T.AcceptanceDate, 
	(select AcceptanceDate from #T where KeyHash = T.KeyHash and RowNumber = T.RowNumber + 1),
	T.ExpenditureWaybillSenderStorageId, T.SaleWaybillCuratorId, T.ProviderId, T.ContractorOrganizationId, T.TeamId, T.ClientId,
	T.AccountOrganizationId, T.ArticleId, T.ReceiptWaybillRowId, T.DealId,
	PreviousId = (select Id from #T where KeyHash = T.KeyHash and RowNumber = T.RowNumber - 1),
	PurchaseCostSum = (select SUM(PurchaseCostSum) from #T where KeyHash = T.KeyHash and RowNumber <= T.RowNumber),
	AccountingPriceSum = (select SUM(AccountingPriceSum) from #T where KeyHash = T.KeyHash and RowNumber <= T.RowNumber),
	SalePriceSum = (select SUM(SalePriceSum) from #T where KeyHash = T.KeyHash and RowNumber <= T.RowNumber),
	SoldCount = (select SUM(SellingCount) from #T where KeyHash = T.KeyHash and RowNumber <= T.RowNumber)
from #T T

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table #T

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

