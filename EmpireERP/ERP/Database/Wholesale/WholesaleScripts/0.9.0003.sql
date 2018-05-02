/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.3

  Что нового:
	+ InitialPurchaseCost в позициях прихода
	+ ProviderSum в позициях прихода
	+ ApprovedPurchaseCost в позициях прихода
	* DiscountSum в накладной прихода сделана до 2 знаков и переименована в PendingDiscountSum
	* PendingSum в позиции накладной прихода сделана до 2 знаков
	* ApprovedSum в позиции накладной прихода сделана до 2 знаков
	- убран DiscountPercent из накладной прихода
	- убрана ReceiptedSum из накладной прихода
	
	!!! Надо проверить, какое количество записей выведет SELECT в конце, 0 - значит нет проблем
	
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

SET @PreviousVersion = '0.9.2' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.3'			-- номер новой версии

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

	alter table dbo.[ReceiptWaybill] DROP column DiscountPercent;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 2 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	alter table dbo.[ReceiptWaybill] alter column DiscountSum DECIMAL(18, 2) not null;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 3 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    EXEC sp_rename 'ReceiptWaybill.DiscountSum', 'PendingDiscountSum';

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 4 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	ALTER TABLE dbo.[ReceiptWaybillRow] ADD InitialPurchaseCost DECIMAL(18, 6) not null
	CONSTRAINT DF_ReceiptWaybillRow_InitialPurchaseCost DEFAULT 0

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 5 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	ALTER TABLE dbo.[ReceiptWaybillRow] DROP CONSTRAINT DF_ReceiptWaybillRow_InitialPurchaseCost

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 6 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    -- Поле InitialPurchaseCost равно начальной сумме позиции, деленной на количество (если она не была
    -- добавлена при приемке)
	update RR
	set RR.InitialPurchaseCost =
		CASE WHEN RR.PendingCount <> 0 THEN
			RR.PendingSum / RR.PendingCount
		ELSE
			0
		END
	from
    dbo.[ReceiptWaybillRow] RR;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 7 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	DROP INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [dbo].[ReceiptWaybillRow] WITH ( ONLINE = OFF )

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 8 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	alter table dbo.[ReceiptWaybillRow] alter column PendingSum DECIMAL(18, 2) not null;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 9 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	ALTER TABLE dbo.[ReceiptWaybillRow] ADD ProviderSum DECIMAL(18, 2) null;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 10 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	CREATE NONCLUSTERED INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [dbo].[ReceiptWaybillRow]
	(
		[DeletionDate] ASC,
		[ReceiptWaybillId] ASC
	)
	INCLUDE ( [Id],[ArticleMeasureUnitScale],[PendingCount],[PendingSum],[ReceiptedCount],[ProviderCount],[ProviderSum],
	[ApprovedCount],[ApprovedSum],[PurchaseCost],[CustomsDeclarationNumber],[CreationDate],[FinallyMovementDate],
	[ReservedCount],[AcceptedCount],[ShippedCount],[FinallyMovedCount],[RecipientArticleAccountingPriceId],
	[ArticleId],[PendingValueAddedTaxId],[ApprovedValueAddedTaxId],[CountryId],[ManufacturerId])
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF,
	DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 11 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    -- Поле ProviderSum будет равно ожидаемой сумме позиции, если накладная уже принята на склад
	update RR
	set RR.[ProviderSum] = RR.PendingSum
	from
    dbo.[ReceiptWaybillRow] RR
    JOIN [ReceiptWaybill] R on R.Id = RR.ReceiptWaybillId
    WHERE R.ReceiptWaybillStateId <> 1 AND R.ReceiptWaybillStateId <> 7;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 12 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	DROP INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [dbo].[ReceiptWaybillRow] WITH ( ONLINE = OFF )

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 13 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	alter table dbo.[ReceiptWaybillRow] alter column ApprovedSum DECIMAL(18, 2) null;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 14 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	CREATE NONCLUSTERED INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [dbo].[ReceiptWaybillRow]
	(
		[DeletionDate] ASC,
		[ReceiptWaybillId] ASC
	)
	INCLUDE ( [Id],[ArticleMeasureUnitScale],[PendingCount],[PendingSum],[ReceiptedCount],[ProviderCount],[ProviderSum],
	[ApprovedCount],[ApprovedSum],[PurchaseCost],[CustomsDeclarationNumber],[CreationDate],[FinallyMovementDate],
	[ReservedCount],[AcceptedCount],[ShippedCount],[FinallyMovedCount],[RecipientArticleAccountingPriceId],
	[ArticleId],[PendingValueAddedTaxId],[ApprovedValueAddedTaxId],[CountryId],[ManufacturerId])
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF,
	DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 15 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	alter table dbo.[ReceiptWaybill] DROP column ReceiptedSum;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 16 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    -- Поле ProviderSum у согласованных накладных (с расхождениями и без) будет равно согласованной сумме позиции
    -- Правда, тут минус, что если статус был "После расхождений", а расх.по к-ву нет, и суммы при согл. сошлись с
    -- ожидаемыми суммами (и им будут равны выставленные скриптом "суммы по документу"), то естественным путем
    -- такая накладная появиться бы не могла :)
	update RR
	set RR.[ProviderSum] = RR.[ApprovedSum]
	from
    dbo.[ReceiptWaybillRow] RR
    JOIN [ReceiptWaybill] R on R.Id = RR.ReceiptWaybillId
    WHERE R.ReceiptWaybillStateId = 2 OR R.ReceiptWaybillStateId = 6;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 17 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    -- Поле ProviderSum у накладных, принятых на склад, но не согласованных (с расхождениями "по сумме" или "по сумме и
    -- количеству") будет равно ожидаемой сумме позиции + 1 рубль :) Евгений сказал, что все равно мы всем условиям
    -- не удовлетворим (таким, как сохранение суммы, введенной в поле ReceiptedSum всей накладной в старой базе).
	update RR
	set RR.[ProviderSum] = RR.[PendingSum] + 1
	from
    dbo.[ReceiptWaybillRow] RR
    JOIN [ReceiptWaybill] R on R.Id = RR.ReceiptWaybillId
    WHERE R.ReceiptWaybillStateId = 3 OR R.ReceiptWaybillStateId = 5;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 18 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	ALTER TABLE dbo.[ReceiptWaybillRow] ADD ApprovedPurchaseCost DECIMAL(18, 6) null

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 19 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    -- Поле ApprovedPurchaseCost у согласованных накладных (с расхождениями и без) будет равно согласованной закупочной
    -- цене (PurchaseCost). У прочих накладных оно будет равно null (это приведет к пересчету параметров у тех
    -- накладных, которые уже пытались согласовать, и им выставлены Approved-значения).
    -- Правда, тут минус, что если уже были выставлены некоторые Approved-значения, они потеряются
	update RR
	set RR.[ApprovedPurchaseCost] = RR.[PurchaseCost]
	from
    dbo.[ReceiptWaybillRow] RR
    JOIN [ReceiptWaybill] R on R.Id = RR.ReceiptWaybillId
    WHERE R.ReceiptWaybillStateId = 2 OR R.ReceiptWaybillStateId = 6;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 20 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    -- Также выставляем ApprovedPurchaseCost у принятых позиций частично принятых накладных
	update RR
	set RR.[ApprovedPurchaseCost] = RR.[PurchaseCost]
	from
    dbo.[ReceiptWaybillRow] RR
    JOIN [ReceiptWaybill] R on R.Id = RR.ReceiptWaybillId
    WHERE (R.ReceiptWaybillStateId = 3 OR R.ReceiptWaybillStateId = 4 OR R.ReceiptWaybillStateId = 5)
		AND RR.PendingCount = RR.ReceiptedCount AND RR.PendingCount = RR.ProviderCount AND
		RR.PendingSum = RR.[ProviderSum];

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 21 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    -- В связи с изменением сеттера согласованной зак. цены и согласованной суммы, к-ва, если одно
    -- из значений позиции без расхождений будет null, а прочие не null, произойдет исключение
    -- при попытке начать согласование. Такого не должно быть
	update RR
	set RR.ApprovedCount = null,
		RR.ApprovedPurchaseCost = null,
		RR.ApprovedSum = null
	from
    dbo.[ReceiptWaybillRow] RR
    WHERE RR.ApprovedCount is null OR
		RR.ApprovedPurchaseCost is null OR
		RR.ApprovedSum is null;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 22 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

    PRINT 'Если этот селект выведет больше 0 строк, дело плохо (надо корректировать суммы). Но этого не должно быть'
    DECLARE @count_i INT
	SELECT @count_i = COUNT(*)
		FROM dbo.ReceiptWaybillRow where Round(PendingCount * InitialPurchaseCost, 2) <> PendingSum;
    PRINT @count_i

-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

