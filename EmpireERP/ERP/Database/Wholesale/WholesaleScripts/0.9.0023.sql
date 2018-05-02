/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.0023

  Что нового:
	+ добавлены таблицы [DealPaymentDocumentDistribution], [DealPaymentDocumentDistributionToDealPaymentDocument], [DealPaymentDocumentDistributionToReturnFromClientWaybill],
		[DealPaymentDocumentDistributionToSaleWaybill], [DealPaymentDocument], [DealCreditInitialBalanceCorrection], [DealDebitInitialBalanceCorrection],
		[DealPaymentFromClient], [DealPaymentToClient]
	* новые таблицы набиты ретроспективными данными. При этом те оплаты, номер которых начинается с '*' или "возврат"
	 забиты в таблицы корректировок первоначального сальдо.
	* в PermissionDistribution для админа добавлены права работы с корректировками первоначального сальдо
	- удалены таблицы PaymentDistributionToPayment, PaymentDistributionToSale, PaymentDistributionToSalesReturn, PaymentDistribution, Payment
	* из PermissionDistribution удалены старые, ставшие лишними, права: ("Ввод положительного сальдо в сделку", "Редактирование перенесенного сальдо", 
		"Ввод и редактирование положительного сальдо", "Ввод и редактирование отрицательного сальдо"
	- из таблицы Deal и Client удален столбец InitialBalance
	
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

SET @PreviousVersion = '0.9.22' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.23'			-- номер новой версии

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

declare @InitialBalanceCount int;

	set @InitialBalanceCount = (select COUNT(*) from Deal
		where InitialBalance > 0)

IF @InitialBalanceCount > 0
RAISERROR('Существуют сделки, у которых первоначальное сальдо отлично от нуля.', 16, 1)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

declare @WrongTypePaymentCount int;

	set @WrongTypePaymentCount = (select COUNT(*) from Payment
		where [PaymentTypeId] <> 1 AND [PaymentTypeId] <> 2)

IF @WrongTypePaymentCount > 0
RAISERROR('Существуют оплаты с типом, отличным от "Оплата от клиента" и "Возврат оплаты клиенту".', 16, 1)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    create table dbo.[DealPaymentDocumentDistribution] (
        Id UNIQUEIDENTIFIER not null,
       Sum DECIMAL(18, 2) not null,
       CreationDate DATETIME not null,
       OrdinalNumber int IDENTITY(1,1) not null,
       SourceDealPaymentDocumentId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    create table dbo.[DealPaymentDocumentDistributionToDealPaymentDocument] (
        Id UNIQUEIDENTIFIER not null,
       DestinationDealPaymentDocumentId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    create table dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill] (
        Id UNIQUEIDENTIFIER not null,
       SaleWaybillId UNIQUEIDENTIFIER not null,
       ReturnFromClientWaybillId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    create table dbo.[DealPaymentDocumentDistributionToSaleWaybill] (
        Id UNIQUEIDENTIFIER not null,
       SaleWaybillId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    create table dbo.[DealPaymentDocument] (
        Id UNIQUEIDENTIFIER not null,
       Date DATETIME not null,
       Sum DECIMAL(18, 2) not null,
       IsFullyDistributed BIT not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       DealPaymentDocumentTypeId TINYINT not null,
       DealId INT not null,
       primary key (Id)
    )

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    create table dbo.[DealCreditInitialBalanceCorrection] (
        Id UNIQUEIDENTIFIER not null,
       CorrectionReason VARCHAR(140) not null,
       primary key (Id)
    )

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    create table dbo.[DealDebitInitialBalanceCorrection] (
        Id UNIQUEIDENTIFIER not null,
       CorrectionReason VARCHAR(140) not null,
       primary key (Id)
    )

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    create table dbo.[DealPaymentFromClient] (
        Id UNIQUEIDENTIFIER not null,
       PaymentDocumentNumber VARCHAR(50) not null,
       DealPaymentFormId TINYINT not null,
       primary key (Id)
    )

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    create table dbo.[DealPaymentToClient] (
        Id UNIQUEIDENTIFIER not null,
       PaymentDocumentNumber VARCHAR(50) not null,
       DealPaymentFormId TINYINT not null,
       primary key (Id)
    )

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    alter table dbo.[DealPaymentDocumentDistribution] 
        add constraint FK_DealPaymentFromClient_DealPaymentDocumentDistribution_SourceDealPaymentDocumentId 
        foreign key (SourceDealPaymentDocumentId) 
        references dbo.[DealPaymentDocument]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    alter table dbo.[DealPaymentDocumentDistributionToDealPaymentDocument] 
        add constraint PFK_DealPaymentDocumentDistributionToDealPaymentDocument 
        foreign key (Id) 
        references dbo.[DealPaymentDocumentDistribution]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    alter table dbo.[DealPaymentDocumentDistributionToDealPaymentDocument] 
        add constraint FK_DealPaymentToClient_DealPaymentDocumentDistributionToDealPaymentDocument_DestinationDealPaymentDocumentId 
        foreign key (DestinationDealPaymentDocumentId) 
        references dbo.[DealPaymentDocument]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    alter table dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill] 
        add constraint PFK_DealPaymentDocumentDistributionToReturnFromClientWaybill 
        foreign key (Id) 
        references dbo.[DealPaymentDocumentDistribution]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    alter table dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill] 
        add constraint FK_DealPaymentDocumentDistributionToReturnFromClientWaybill_SaleWaybill 
        foreign key (SaleWaybillId) 
        references dbo.[SaleWaybill]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    alter table dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill] 
        add constraint FK_ReturnFromClientWaybill_DealPaymentDocumentDistributionToReturnFromClientWaybill_ReturnFromClientWaybillId 
        foreign key (ReturnFromClientWaybillId) 
        references dbo.[ReturnFromClientWaybill]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    alter table dbo.[DealPaymentDocumentDistributionToSaleWaybill] 
        add constraint PFK_DealPaymentDocumentDistributionToSaleWaybill 
        foreign key (Id) 
        references dbo.[DealPaymentDocumentDistribution]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    alter table dbo.[DealPaymentDocumentDistributionToSaleWaybill] 
        add constraint FK_SaleWaybill_DealPaymentDocumentDistributionToSaleWaybill_SaleWaybillId 
        foreign key (SaleWaybillId) 
        references dbo.[SaleWaybill]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    alter table dbo.[DealPaymentDocument] 
        add constraint FK_Deal_DealPaymentDocument_DealId 
        foreign key (DealId) 
        references dbo.[Deal]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    alter table dbo.[DealCreditInitialBalanceCorrection] 
        add constraint PFK_DealCreditInitialBalanceCorrection 
        foreign key (Id) 
        references dbo.[DealPaymentDocument]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    alter table dbo.[DealDebitInitialBalanceCorrection] 
        add constraint PFK_DealDebitInitialBalanceCorrection 
        foreign key (Id) 
        references dbo.[DealPaymentDocument]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    alter table dbo.[DealPaymentFromClient] 
        add constraint PFK_DealPaymentFromClient 
        foreign key (Id) 
        references dbo.[DealPaymentDocument]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    alter table dbo.[DealPaymentToClient] 
        add constraint PFK_DealPaymentToClient 
        foreign key (Id) 
        references dbo.[DealPaymentDocument]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4101, 1)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4102, 1)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4103, 1)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4104, 1)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4105, 1)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_SaleWaybill_DeletionDate_DealId_PaymentFormId_Id')
DROP INDEX IX_SaleWaybill_DeletionDate_DealId_PaymentFormId_Id ON [dbo].SaleWaybill

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

EXEC sp_rename 'SaleWaybill.PaymentFormId', 'DealPaymentFormId';

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

CREATE INDEX IX_SaleWaybill_DeletionDate_DealId_DealPaymentFormId_Id ON [SaleWaybill] ([DeletionDate], [DealId], [DealPaymentFormId], [Id])

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into DealPaymentDocument([Id],[Date],[Sum],[IsFullyDistributed],[CreationDate],[DeletionDate],[DealPaymentDocumentTypeId],[DealId])
select
	Id,
	[Date],
	[Sum],
	IsFullyDistributed,
	CreationDate,
	DeletionDate,
	DealPaymentDocumentTypeId = case
		when ((PaymentDocumentNumber LIKE '*%' or PaymentDocumentNumber LIKE 'возврат%') AND PaymentTypeId = 1) then 4
		when ((PaymentDocumentNumber LIKE '*%' or PaymentDocumentNumber LIKE 'возврат%') AND PaymentTypeId = 2) then 3
		else PaymentTypeId end,
	DealId
from Payment

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into DealPaymentFromClient(Id, PaymentDocumentNumber, DealPaymentFormId)
select 
	Id,
	PaymentDocumentNumber,
	PaymentFormId
from Payment 
where (PaymentTypeId = 1)
and (PaymentDocumentNumber not like '*%')
and (PaymentDocumentNumber not like 'возврат%');

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into DealPaymentToClient(Id, PaymentDocumentNumber, DealPaymentFormId)
select 
	Id,
	PaymentDocumentNumber,
	PaymentFormId
from Payment 
where (PaymentTypeId = 2)
and (PaymentDocumentNumber not like '*%')
and (PaymentDocumentNumber not like 'возврат%');

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into DealDebitInitialBalanceCorrection(Id, CorrectionReason)
select 
	Id,
	'Первоначальный долг'
from Payment 
where (PaymentTypeId = 1)
and (PaymentDocumentNumber LIKE '*%');

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into DealDebitInitialBalanceCorrection(Id, CorrectionReason)
select 
	Id,
	'ВОЗВРАТ'
from Payment 
where (PaymentTypeId = 1)
and (PaymentDocumentNumber LIKE 'возврат%');

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into DealCreditInitialBalanceCorrection(Id, CorrectionReason)
select 
	Id,
	'Первоначальный долг'
from Payment 
where (PaymentTypeId = 2)
and (PaymentDocumentNumber LIKE '*%');

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into DealCreditInitialBalanceCorrection(Id, CorrectionReason)
select 
	Id,
	'ВОЗВРАТ'
from Payment
where (PaymentTypeId = 2)
and (PaymentDocumentNumber LIKE 'возврат%');

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

set IDENTITY_INSERT DealPaymentDocumentDistribution ON

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into DealPaymentDocumentDistribution([Id],[Sum],[CreationDate],[OrdinalNumber],[SourceDealPaymentDocumentId])
select
	Id,
	[Sum],
	CreationDate,
	OrdinalNumber,
	PaymentId	
from PaymentDistribution

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

set IDENTITY_INSERT DealPaymentDocumentDistribution OFF

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into DealPaymentDocumentDistributionToDealPaymentDocument([Id],[DestinationDealPaymentDocumentId])
select
	Id,
	PaidPaymentId
from PaymentDistributionToPayment

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into DealPaymentDocumentDistributionToSaleWaybill([Id],SaleWaybillId)
select
	Id,
	SaleWaybillId
from PaymentDistributionToSale

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

insert into [DealPaymentDocumentDistributionToReturnFromClientWaybill]([Id],SaleWaybillId,[ReturnFromClientWaybillId])
select
	Id,
	SaleWaybillId,
	ReturnWaybillId
from [PaymentDistributionToSalesReturn]

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- ПРОВЕРКИ НА КОРРЕКТНОСТЬ БД

-- 1. Нет ли сущностей с одинаковыми Id в разных таблицах, отображающих разные классы-потомки

	-- платежные документы

declare @WrongEntityCount int;

set @WrongEntityCount = (select COUNT(*) from
	DealPaymentFromClient T1
	left outer join DealPaymentToClient T2 on T1.Id=T2.Id
	where (T1.Id is not null) AND (T2.Id is not null))

IF @WrongEntityCount > 0
RAISERROR('Нарушена корректность: существуют документы с одним Id в разных таблицах.', 16, 1)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

declare @WrongEntityCount int;

set @WrongEntityCount = (select COUNT(*) from
	DealPaymentFromClient T1
	left outer join DealDebitInitialBalanceCorrection T2 on T1.Id=T2.Id
	where (T1.Id is not null) AND (T2.Id is not null))

IF @WrongEntityCount > 0
RAISERROR('Нарушена корректность: существуют документы с одним Id в разных таблицах.', 16, 1)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

declare @WrongEntityCount int;

set @WrongEntityCount = (select COUNT(*) from
	DealPaymentFromClient T1
	left outer join DealCreditInitialBalanceCorrection T2 on T1.Id=T2.Id
	where (T1.Id is not null) AND (T2.Id is not null))

IF @WrongEntityCount > 0
RAISERROR('Нарушена корректность: существуют документы с одним Id в разных таблицах.', 16, 1)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

declare @WrongEntityCount int;

set @WrongEntityCount = (select COUNT(*) from
	DealPaymentToClient T1
	left outer join DealDebitInitialBalanceCorrection T2 on T1.Id=T2.Id
	where (T1.Id is not null) AND (T2.Id is not null))

IF @WrongEntityCount > 0
RAISERROR('Нарушена корректность: существуют документы с одним Id в разных таблицах.', 16, 1)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

declare @WrongEntityCount int;

set @WrongEntityCount = (select COUNT(*) from
	DealPaymentToClient T1
	left outer join DealCreditInitialBalanceCorrection T2 on T1.Id=T2.Id
	where (T1.Id is not null) AND (T2.Id is not null))

IF @WrongEntityCount > 0
RAISERROR('Нарушена корректность: существуют документы с одним Id в разных таблицах.', 16, 1)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

declare @WrongEntityCount int;

set @WrongEntityCount = (select COUNT(*) from
	DealDebitInitialBalanceCorrection T1
	left outer join DealCreditInitialBalanceCorrection T2 on T1.Id=T2.Id
	where (T1.Id is not null) AND (T2.Id is not null))

IF @WrongEntityCount > 0
RAISERROR('Нарушена корректность: существуют документы с одним Id в разных таблицах.', 16, 1)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	-- разнесения платежных документов

declare @WrongEntityCount int;

set @WrongEntityCount = (select COUNT(*) from
	DealPaymentDocumentDistributionToDealPaymentDocument T1
	left outer join DealPaymentDocumentDistributionToReturnFromClientWaybill T2 on T1.Id=T2.Id
	where (T1.Id is not null) AND (T2.Id is not null))

IF @WrongEntityCount > 0
RAISERROR('Нарушена корректность: существуют документы с одним Id в разных таблицах.', 16, 1)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

declare @WrongEntityCount int;

set @WrongEntityCount = (select COUNT(*) from
	DealPaymentDocumentDistributionToDealPaymentDocument T1
	left outer join DealPaymentDocumentDistributionToSaleWaybill T2 on T1.Id=T2.Id
	where (T1.Id is not null) AND (T2.Id is not null))

IF @WrongEntityCount > 0
RAISERROR('Нарушена корректность: существуют документы с одним Id в разных таблицах.', 16, 1)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

declare @WrongEntityCount int;

set @WrongEntityCount = (select COUNT(*) from
	DealPaymentDocumentDistributionToReturnFromClientWaybill T1
	left outer join DealPaymentDocumentDistributionToSaleWaybill T2 on T1.Id=T2.Id
	where (T1.Id is not null) AND (T2.Id is not null))

IF @WrongEntityCount > 0
RAISERROR('Нарушена корректность: существуют документы с одним Id в разных таблицах.', 16, 1)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- 2. Нет ли сущностей в таблицах с классом-предком, отсутствующих в таблицах всех классов-потомков

	-- платежные документы

declare @WrongEntityCount int;

set @WrongEntityCount = (select COUNT(*) from
	DealPaymentDocument T1
	left outer join DealPaymentFromClient T2 on T1.Id=T2.Id
	left outer join DealPaymentToClient T3 on T1.Id=T3.Id
	left outer join DealDebitInitialBalanceCorrection T4 on T1.Id=T4.Id
	left outer join DealCreditInitialBalanceCorrection T5 on T1.Id=T5.Id
	where (T2.Id is null) AND (T3.Id is null) AND (T4.Id is null) AND (T5.Id is null))

IF @WrongEntityCount > 0
RAISERROR('Нарушена корректность: существуют записи в таблицах с классом-предком, отсутствующих в таблицах всех классов-потомков.', 16, 1)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	-- разнесения платежных документов

declare @WrongEntityCount int;

set @WrongEntityCount = (select COUNT(*) from
	DealPaymentDocumentDistribution T1
	left outer join DealPaymentDocumentDistributionToDealPaymentDocument T2 on T1.Id=T2.Id
	left outer join DealPaymentDocumentDistributionToReturnFromClientWaybill T3 on T1.Id=T3.Id
	left outer join DealPaymentDocumentDistributionToSaleWaybill T4 on T1.Id=T4.Id
	where (T2.Id is null) AND (T3.Id is null) AND (T4.Id is null))

IF @WrongEntityCount > 0
RAISERROR('Нарушена корректность: существуют записи в таблицах с классом-предком, отсутствующих в таблицах всех классов-потомков.', 16, 1)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

delete from PaymentDistributionToPayment

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table PaymentDistributionToPayment

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

delete from PaymentDistributionToSale

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table PaymentDistributionToSale

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

delete from [PaymentDistributionToSalesReturn]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table [PaymentDistributionToSalesReturn]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

delete from PaymentDistribution

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table PaymentDistribution

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

delete from Payment 

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table Payment

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

delete from PermissionDistribution where PermissionId in (3004, 3005, 3503, 3504)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table Deal drop column InitialBalance

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table Client drop column InitialBalance

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

