/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.53

  Что нового:
	* Переcчет показателей проведенного исходящего из проведенного и точного наличия
	
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

SET @PreviousVersion = '0.9.52' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.53'			-- номер новой версии

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

	BACKUP DATABASE @DataBaseName TO DISK = @BackupTarget WITH  INIT,  NOUNLOAD,  NAME = N'wholesale', NOSKIP, DESCRIPTION = N'Обновление версии', NOFORMAT

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

CREATE FUNCTION GetSourceReceiptDate
(
  @sourceWaybillRowId uniqueidentifier,	-- код источника позиции накладной
  @sourceWaybillTypeId tinyint	-- код типа накладной источника
)
RETURNS datetime
WITH SCHEMABINDING
AS
BEGIN
	declare @sourceReceiptDate DateTime -- дата "приемки" накладной
		
	-- Получаем данные о дате поступления источника в точное наличие
	IF(@sourceWaybillTypeId = 1)	-- приходная накладная
		SELECT TOP(1) 
			@sourceReceiptDate = 
			CASE
				WHEN rwr.AreCountDivergencesAfterReceipt = 1 OR rwr.AreSumDivergencesAfterReceipt = 1 
					THEN rw.ApprovementDate
					ELSE rw.ReceiptDate
			END
		FROM
			dbo.ReceiptWaybill rw
			join dbo.ReceiptWaybillRow rwr on rwr.ReceiptWaybillId = rw.Id
		WHERE
			rwr.Id = @sourceWaybillRowId
	ELSE IF (@sourceWaybillTypeId = 2)	-- накладная перемещения
		SELECT TOP(1) 
			@sourceReceiptDate = mw.ReceiptDate
		FROM
			dbo.MovementWaybill mw
			join dbo.MovementWaybillRow mwr on mwr.MovementWaybillId = mw.Id
		WHERE
			mwr.Id = @sourceWaybillRowId
	ELSE IF (@sourceWaybillTypeId = 5)	-- накладная смены собственника
		SELECT TOP(1) 
			@sourceReceiptDate = cow.ChangeOwnerDate
		FROM
			dbo.ChangeOwnerWaybill cow
			join dbo.ChangeOwnerWaybillRow cowr on cowr.ChangeOwnerWaybillId = cow.Id
		WHERE
			cowr.Id = @sourceWaybillRowId
	ELSE IF (@sourceWaybillTypeId = 6)	-- накладная возврата товара
		SELECT TOP(1) 
			@sourceReceiptDate = rfcw.ReceiptDate
		FROM
			dbo.ReturnFromClientWaybill rfcw
			join dbo.ReturnFromClientWaybillRow rfcwr on rfcwr.ReturnFromClientWaybillId = rfcw.Id
		WHERE
			rfcwr.Id = @sourceWaybillRowId

	RETURN @sourceReceiptDate
END

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Добавление индикатора в таблицу проведенного
CREATE PROCEDURE AddValueToAcceptedIndicator 
(
  @date DateTime, 
  @storageId smallint,
  @accountOrganizationId int,
  @articleId int,
  @batchId uniqueidentifier,
  @purchaseCost decimal (18,6),
  @count decimal (18,6)
)
AS
BEGIN
	declare @exists int -- пирзнак существования индикатора с тем же ключом и на туже дату
	declare @keyHash varchar(255) -- ключ
	
	SET @keyHash = HashBytes('SHA1', 
			CAST(@storageId as varchar(255)) + '_' +
			CAST(@accountOrganizationId as varchar(255)) + '_' +
			CAST(@articleId as varchar(255)) + '_' +
			CAST(@batchId as varchar(255)))
	
	SET @exists = (
		SELECT 
			COUNT(*)
		FROM 
			#T_accepted t
		WHERE
			t.KeyHash = @keyHash
			AND t.StartDate = @date
	)
	
	IF (@exists = 0)
		-- Вставляем изменение в проведенном
		INSERT INTO #T_accepted(
			Id,
			StartDate,
			EndDate,
			StorageId,
			AccountOrganizationId,
			ArticleId,
			BatchId,
			PurchaseCost,
			Count,
			KeyHash,
			PreviousId
		)
		VALUES(	
			NEWID(),	--Id,
			@date,	--StartDate,
			null,	--EndDate,
			@storageId,	--StorageId,
			@accountOrganizationId,	--AccountOrganizationId,
			@articleId,	--ArticleId,
			@batchId,	--BatchId,
			@purchaseCost,	--PurchaseCost,
			@count,	--Count,
			@keyHash,
			null	--PreviousId			
		)
	ELSE
		-- Инкрементируем количество
		UPDATE #T_accepted
			SET Count = Count + @count
		WHERE
			KeyHash = @keyHash
			AND StartDate = @date

END

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Добавление инликатора в таблицу проведенного
CREATE PROCEDURE AddValueToExectIndicator 
(
  @date DateTime, 
  @storageId smallint,
  @accountOrganizationId int,
  @articleId int,
  @batchId uniqueidentifier,
  @purchaseCost decimal (18,6),
  @count decimal (18,6)
)
AS
BEGIN
	declare @exists int -- пирзнак существования индикатора с темже ключом и на туже дату
	declare @keyHash varchar(255) -- ключ
	
	SET @keyHash = HashBytes('SHA1', 
			CAST(@storageId as varchar(255)) + '_' +
			CAST(@accountOrganizationId as varchar(255)) + '_' +
			CAST(@articleId as varchar(255)) + '_' +
			CAST(@batchId as varchar(255)))
	
	--SET @exists = 0
	SET @exists = (
		SELECT 
			COUNT(*)
		FROM 
			#T_exact t
		WHERE
			t.KeyHash = @keyHash
			AND t.StartDate = @date
	)
		
	IF (@exists = 0)
		-- Вставляем изменение в точном
		INSERT INTO #T_exact(
			Id,
			StartDate,
			EndDate,
			StorageId,
			AccountOrganizationId,
			ArticleId,
			BatchId,
			PurchaseCost,
			Count,
			KeyHash,
			PreviousId
		)
		VALUES(	
			NEWID(),	--Id,
			@date,	--StartDate,
			null,	--EndDate,
			@storageId,	--StorageId,
			@accountOrganizationId,	--AccountOrganizationId,
			@articleId,	--ArticleId,
			@batchId,	--BatchId,
			@purchaseCost,	--PurchaseCost,
			@count,	--Count,
			@keyHash,
			null	--PreviousId			
		)
	ELSE
		-- Инкрементируем количество
		UPDATE #T_exact
			SET Count = Count + @count
		WHERE
			KeyHash = @keyHash
			AND StartDate = @date
END

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

CREATE PROCEDURE InsertIndicator 
(
  @waybillRowId uniqueidentifier,
  @acceptDate DateTime, 
  @receiptDate DateTime,
  @storageId smallint,
  @accountOrganizationId int,
  @articleId int,
  @batchId uniqueidentifier
)
AS
BEGIN
	declare @waybillRowArticleMovementId uniqueidentifier -- код строки связи движения товара
	declare @sourceReceiptDate DateTime -- дата "приемки" накладной
	declare @purchaseCost decimal (18,6) -- закупочная цена
	declare @minusCount decimal (18,6)
	declare @count decimal (18,6)
	declare @sourceWaybillRowId uniqueidentifier;	-- код источника позиции накладной
	declare @sourceWaybillTypeId tinyint;	-- код типа накладной источника
	
	-- Получаем закупочную цену
	SELECT
		@purchaseCost = rwr.PurchaseCost
	FROM
		ReceiptWaybillRow rwr
	WHERE
		rwr.Id = @batchId
			
	-- Получаем данные о первом источнике позиции		 	
	SET @waybillRowArticleMovementId = (
	SELECT TOP(1)
		wram.Id		
	FROM
		dbo.WaybillRowArticleMovement wram
	WHERE
		wram.DestinationWaybillRowId = @waybillRowId
		ORDER BY wram.Id)
	
	-- получаем данные по источникам		
	SELECT
		@sourceWaybillRowId = wram.SourceWaybillRowId,
		@sourceWaybillTypeId = wram.SourceWaybillTypeId,
		@count = wram.MovingCount		
	FROM
		dbo.WaybillRowArticleMovement wram
	WHERE
		wram.Id = @waybillRowArticleMovementId
	
	-- Цикл прохода всех источников
	while(@waybillRowArticleMovementId is not null)
	begin
	
		SET @minusCount = @count * -1
		 
		SET @sourceReceiptDate = dbo.GetSourceReceiptDate(@sourceWaybillRowId, @sourceWaybillTypeId)	-- Получаем дату приемки источника

		-- Если источник не принят на момент проводки исходящей, то
		IF(@sourceReceiptDate is null OR @sourceReceiptDate > @acceptDate)
		begin
			-- Вставляем изменение в проведенном
			exec AddValueToAcceptedIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
			-- если источник был принят позже проводки
			IF (@sourceReceiptDate is not null)
			begin
				-- переводим резерв из проведенного в точное
				-- именьшаем проведенное
				exec AddValueToAcceptedIndicator @sourceReceiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost, @minusCount
				
				-- увеличиваем точное
				exec AddValueToExectIndicator @sourceReceiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count		
			end
		end
		ELSE
			-- иначе вставляем изменение в точном
			exec AddValueToExectIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
		
		-- Если исходящая накладная принята, то 
		IF(@receiptDate is not null)
			-- уменьшаем точное
			exec AddValueToExectIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost, @minusCount
			
			
		-- Получаем данные о первом источнике позиции		 	
		SET @waybillRowArticleMovementId = (
		SELECT TOP(1)
			wram.Id		
		FROM
			dbo.WaybillRowArticleMovement wram
		WHERE
			wram.DestinationWaybillRowId = @waybillRowId
			AND wram.Id > @waybillRowArticleMovementId
			ORDER BY wram.Id)
		
		-- получаем данные по источникам		
		SELECT
			@sourceWaybillRowId = wram.SourceWaybillRowId,
			@sourceWaybillTypeId = wram.SourceWaybillTypeId,
			@count = wram.MovingCount		
		FROM
			dbo.WaybillRowArticleMovement wram
		WHERE
			wram.Id = @waybillRowArticleMovementId
	end
END

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- таблица исходной информации для исходящего из точного
create table #T_exact (
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
   KeyHash varchar(255) not null,
   RowNumber int null,
   primary key (Id)
)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

create table #T_accepted (
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
   KeyHash varchar(255) not null,
   RowNumber int null,
   primary key (Id)
)


-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

CREATE INDEX IX_#T_accepted ON #T_accepted (KeyHash, RowNumber, Id);

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

CREATE INDEX IX_#T_exact ON #T_exact (KeyHash, RowNumber, Id);

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

declare @waybillId uniqueidentifier;	-- код текущей накладной
declare @waybillRowId uniqueidentifier;	-- код текущей позиции накладной

declare @startDate DateTime -- дата события накладной
declare @storageId smallint -- код МХ
declare @accountOrganizationId int -- код организации аккаунта
declare @articleId int -- код товара
declare @batchId uniqueidentifier -- код партии

declare @acceptDate DateTime -- дата проводки накладной
declare @receiptDate DateTime -- дата "приемки" накладной
declare @sourceReceiptDate DateTime -- дата "приемки" накладной

-- Накладная перемещения

-- Первая накладаня
SET @waybillId = (
SELECT TOP(1) 
	mw.Id		
FROM 
	MovementWaybill mw
WHERE
	mw.DeletionDate is null
	AND mw.AcceptanceDate is not null
	ORDER BY mw.Id)
	
SELECT TOP(1) 
	@storageId = mw.SenderStorageId,
	@accountOrganizationId = mw.SenderId,
	@acceptDate = mw.AcceptanceDate,
	@receiptDate = mw.ReceiptDate
FROM 
	MovementWaybill mw
WHERE
	mw.Id = @waybillId

-- Цикл по накладным 	
WHILE (@waybillId is not null)
begin
	-- Первая позиция накладной
	SET @waybillRowId = (
	SELECT TOP(1) 
		mwr.Id
	FROM 
		MovementWaybillRow mwr
	WHERE
		mwr.DeletionDate is null
		AND mwr.MovementWaybillId = @waybillId
		ORDER BY mwr.Id)

	SELECT TOP(1) 
		@articleId = mwr.ArticleId,
		@batchId = mwr.ReceiptWaybillRowId
	FROM 
		MovementWaybillRow mwr
	WHERE
		mwr.Id = @waybillRowId	

	-- Цикл просмотра всех позиций
	WHILE(@waybillRowId is not null)
	begin
		-- сохраняем данные по позиции			
		exec InsertIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
		
		-- Очередная позиция накладной
		SET @waybillRowId = (
		SELECT TOP(1) 
			mwr.Id
		FROM 
			MovementWaybillRow mwr
		WHERE
			mwr.DeletionDate is null
			AND mwr.MovementWaybillId = @waybillId
			AND mwr.Id > @waybillRowId
			ORDER BY mwr.Id)

		SELECT TOP(1) 
			@articleId = mwr.ArticleId,
			@batchId = mwr.ReceiptWaybillRowId
		FROM 
			MovementWaybillRow mwr
		WHERE
			mwr.Id = @waybillRowId		
	end
	
	-- Очередная накладная
	SET @waybillId = (
	SELECT TOP(1) 
		mw.Id		
	FROM 
		MovementWaybill mw
	WHERE
		mw.DeletionDate is null
		AND mw.AcceptanceDate is not null
		AND mw.Id > @waybillId
		ORDER BY mw.Id)
		
	SELECT TOP(1) 
		@storageId = mw.SenderStorageId,
		@accountOrganizationId = mw.SenderId,
		@acceptDate = mw.AcceptanceDate,
		@receiptDate = mw.ReceiptDate
	FROM 
		MovementWaybill mw
	WHERE
		mw.Id = @waybillId
end


-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

declare @waybillId uniqueidentifier;	-- код текущей накладной
declare @waybillRowId uniqueidentifier;	-- код текущей позиции накладной

declare @startDate DateTime -- дата события накладной
declare @storageId smallint -- код МХ
declare @accountOrganizationId int -- код организации аккаунта
declare @articleId int -- код товара
declare @batchId uniqueidentifier -- код партии

declare @acceptDate DateTime -- дата проводки накладной
declare @receiptDate DateTime -- дата "приемки" накладной
declare @sourceReceiptDate DateTime -- дата "приемки" накладной

-- Накладная смены сосбственника

-- Первая накладаня
SET @waybillId = (
SELECT TOP(1) 
	cow.Id		
FROM 
	ChangeOwnerWaybill cow
WHERE
	cow.DeletionDate is null
	AND cow.AcceptanceDate is not null
	ORDER BY cow.Id)
	
SELECT TOP(1) 
	@storageId = cow.ChangeOwnerWaybillStorageId,
	@accountOrganizationId = cow.ChangeOwnerWaybillSenderId,
	@acceptDate = cow.AcceptanceDate,
	@receiptDate = cow.ChangeOwnerDate
FROM 
	ChangeOwnerWaybill cow
WHERE
	cow.Id = @waybillId

-- Цикл по накладным 	
WHILE (@waybillId is not null)
begin
	-- Первая позиция накладной
	SET @waybillRowId = (
	SELECT TOP(1) 
		cowr.Id
	FROM 
		ChangeOwnerWaybillRow cowr
	WHERE
		cowr.DeletionDate is null
		AND cowr.ChangeOwnerWaybillId = @waybillId
		ORDER BY cowr.Id)

	SELECT TOP(1) 
		@articleId = cowr.ArticleId,
		@batchId = cowr.ChangeOwnerWaybillRowReceiptWaybillRowId
	FROM 
		ChangeOwnerWaybillRow cowr
	WHERE
		cowr.Id = @waybillRowId	

	-- Цикл просмотра всех позиций
	WHILE(@waybillRowId is not null)
	begin		
		-- сохраняем данные по позиции			
		exec InsertIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
		
		-- Очередная позиция накладной
		SET @waybillRowId = (
		SELECT TOP(1) 
			cowr.Id
		FROM 
			ChangeOwnerWaybillRow cowr
		WHERE
			cowr.DeletionDate is null
			AND cowr.ChangeOwnerWaybillId = @waybillId
			AND cowr.Id > @waybillRowId
			ORDER BY cowr.Id)

		SELECT TOP(1) 
			@articleId = cowr.ArticleId,
			@batchId = cowr.ChangeOwnerWaybillRowReceiptWaybillRowId
		FROM 
			ChangeOwnerWaybillRow cowr
		WHERE
			cowr.Id = @waybillRowId			
	end
	
	-- Очередная накладная
	SET @waybillId = (
	SELECT TOP(1) 
		cow.Id		
	FROM 
		ChangeOwnerWaybill cow
	WHERE
		cow.DeletionDate is null
		AND cow.AcceptanceDate is not null
		AND cow.Id > @waybillId
		ORDER BY cow.Id)
		
	SELECT TOP(1) 
		@storageId = cow.ChangeOwnerWaybillStorageId,
		@accountOrganizationId = cow.ChangeOwnerWaybillSenderId,
		@acceptDate = cow.AcceptanceDate,
		@receiptDate = cow.ChangeOwnerDate
	FROM 
		ChangeOwnerWaybill cow
	WHERE
		cow.Id = @waybillId
end

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


declare @waybillId uniqueidentifier;	-- код текущей накладной
declare @waybillRowId uniqueidentifier;	-- код текущей позиции накладной

declare @startDate DateTime -- дата события накладной
declare @storageId smallint -- код МХ
declare @accountOrganizationId int -- код организации аккаунта
declare @articleId int -- код товара
declare @batchId uniqueidentifier -- код партии

declare @acceptDate DateTime -- дата проводки накладной
declare @receiptDate DateTime -- дата "приемки" накладной
declare @sourceReceiptDate DateTime -- дата "приемки" накладной

-- Накладная списания

-- Первая накладаня
SET @waybillId = (
SELECT TOP(1) 
	ww.Id		
FROM 
	WriteoffWaybill ww
WHERE
	ww.DeletionDate is null
	AND ww.AcceptanceDate is not null
	ORDER BY ww.Id)
	
SELECT TOP(1) 
	@storageId = ww.WriteoffWaybillSenderStorageId,
	@accountOrganizationId = ww.WriteoffWaybillSenderId,
	@acceptDate = ww.AcceptanceDate,
	@receiptDate = ww.WriteoffDate
FROM 
	WriteoffWaybill ww
WHERE
	ww.Id = @waybillId

-- Цикл по накладным 	
WHILE (@waybillId is not null)
begin
	-- Первая позиция накладной
	SET @waybillRowId = (
	SELECT TOP(1) 
		wwr.Id
	FROM 
		WriteoffWaybillRow wwr
	WHERE
		wwr.DeletionDate is null
		AND wwr.WriteoffWaybillId = @waybillId
		ORDER BY wwr.Id)

	SELECT TOP(1) 
		@articleId = wwr.ArticleId,
		@batchId = wwr.WriteoffReceiptWaybillRowId
	FROM 
		WriteoffWaybillRow wwr
	WHERE
		wwr.Id = @waybillRowId	

	-- Цикл просмотра всех позиций
	WHILE(@waybillRowId is not null)
	begin		
		-- сохраняем данные по позиции			
		exec InsertIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
		
		-- Очередная позиция накладной
		SET @waybillRowId = (
		SELECT TOP(1) 
			wwr.Id
		FROM 
			WriteoffWaybillRow wwr
		WHERE
			wwr.DeletionDate is null
			AND wwr.WriteoffWaybillId = @waybillId
			AND wwr.Id > @waybillRowId
			ORDER BY wwr.Id)

		SELECT TOP(1) 
			@articleId = wwr.ArticleId,
			@batchId = wwr.WriteoffReceiptWaybillRowId
		FROM 
			WriteoffWaybillRow wwr
		WHERE
			wwr.Id = @waybillRowId			
	end
	
	-- Очередная накладная
	SET @waybillId = (
	SELECT TOP(1) 
		ww.Id		
	FROM 
		WriteoffWaybill ww
	WHERE
		ww.DeletionDate is null
		AND ww.AcceptanceDate is not null
		AND ww.id > @waybillId
		ORDER BY ww.Id)
		
	SELECT TOP(1) 
		@storageId = ww.WriteoffWaybillSenderStorageId,
		@accountOrganizationId = ww.WriteoffWaybillSenderId,
		@acceptDate = ww.AcceptanceDate,
		@receiptDate = ww.WriteoffDate
	FROM 
		WriteoffWaybill ww
	WHERE
		ww.Id = @waybillId
end

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


declare @waybillId uniqueidentifier;	-- код текущей накладной
declare @waybillRowId uniqueidentifier;	-- код текущей позиции накладной

declare @startDate DateTime -- дата события накладной
declare @storageId smallint -- код МХ
declare @accountOrganizationId int -- код организации аккаунта
declare @articleId int -- код товара
declare @batchId uniqueidentifier -- код партии

declare @acceptDate DateTime -- дата проводки накладной
declare @receiptDate DateTime -- дата "приемки" накладной
declare @sourceReceiptDate DateTime -- дата "приемки" накладной

-- Накладная реализации

-- Первая накладаня
SET @waybillId = (
SELECT TOP(1) 
	ew.Id		
FROM 
	ExpenditureWaybill ew
	join SaleWaybill sw on sw.Id = ew.Id
WHERE
	sw.DeletionDate is null
	AND ew.AcceptanceDate is not null
	ORDER BY ew.Id)
	
SELECT TOP(1) 
	@storageId = ew.ExpenditureWaybillSenderStorageId,
	@accountOrganizationId = c.AccountOrganizationId,
	@acceptDate = ew.AcceptanceDate,
	@receiptDate = ew.ShippingDate
FROM 
	ExpenditureWaybill ew
	join SaleWaybill sw on sw.Id = ew.Id
	join Deal d on d.Id = sw.DealId
	join Contract c on c.Id = d.ClientContractId
WHERE
	ew.Id = @waybillId

-- Цикл по накладным 	
WHILE (@waybillId is not null)
begin
	-- Первая позиция накладной
	SET @waybillRowId = (
	SELECT TOP(1) 
		ewr.Id
	FROM 
		ExpenditureWaybillRow ewr
		join SalewaybillRow swr on swr.Id = ewr.Id
	WHERE
		swr.DeletionDate is null
		AND swr.SaleWaybillId = @waybillId
		ORDER BY ewr.Id)

	SELECT TOP(1) 
		@articleId = swr.ArticleId,
		@batchId = ewr.ExpenditureWaybillRowReceiptWaybillRowId
	FROM 
		ExpenditureWaybillRow ewr
		join SalewaybillRow swr on swr.Id = ewr.Id
	WHERE
		ewr.Id = @waybillRowId	

	-- Цикл просмотра всех позиций
	WHILE(@waybillRowId is not null)
	begin		
		-- сохраняем данные по позиции			
		exec InsertIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
		
		-- Очередная позиция накладной
		SET @waybillRowId = (
		SELECT TOP(1) 
			ewr.Id
		FROM 
			ExpenditureWaybillRow ewr
			join SalewaybillRow swr on swr.Id = ewr.Id
		WHERE
			swr.DeletionDate is null
			AND swr.SaleWaybillId = @waybillId
			AND ewr.Id > @waybillRowId
			ORDER BY ewr.Id)

		SELECT TOP(1) 
			@articleId = swr.ArticleId,
			@batchId = ewr.ExpenditureWaybillRowReceiptWaybillRowId
		FROM 
			ExpenditureWaybillRow ewr
			join SalewaybillRow swr on swr.Id = ewr.Id
		WHERE
			ewr.Id = @waybillRowId			
	end
	
	-- Очередная накладная
	SET @waybillId = (
	SELECT TOP(1) 
		ew.Id		
	FROM 
		ExpenditureWaybill ew
		join SaleWaybill sw on sw.Id = ew.Id
	WHERE
		sw.DeletionDate is null
		AND ew.AcceptanceDate is not null
		AND ew.Id > @waybillId
		ORDER BY ew.Id)
		
	SELECT TOP(1) 
		@storageId = ew.ExpenditureWaybillSenderStorageId,
		@accountOrganizationId = c.AccountOrganizationId,
		@acceptDate = ew.AcceptanceDate,
		@receiptDate = ew.ShippingDate
	FROM 
		ExpenditureWaybill ew
		join SaleWaybill sw on sw.Id = ew.Id
		join Deal d on d.Id = sw.DealId
		join Contract c on c.Id = d.ClientContractId
	WHERE
		ew.Id = @waybillId
end

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Проставляем порядковые номера строк в индикаторах
UPDATE #T_accepted
	SET RowNumber = d.rn
FROM (
	SELECT 
		ROW_NUMBER() over (partition by t.KeyHash order by t.StartDate) rn, 
		t.KeyHash,
		t.Id
	FROM  #T_accepted t) d
WHERE
	#T_accepted.KeyHash = d.KeyHash
	AND #T_accepted.Id = d.Id
	
-- Проставляем порядковые номера строк в индикаторах
UPDATE #T_exact
	SET RowNumber = d.rn
FROM (
	SELECT 
		ROW_NUMBER() over (partition by t.KeyHash order by t.StartDate) rn, 
		t.KeyHash,
		t.Id
	FROM  #T_exact t) d
WHERE
	#T_exact.KeyHash = d.KeyHash
	AND #T_exact.Id = d.Id
	

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Суммируем индикаторы по ключам
declare @indicatorId uniqueidentifier --	код индикатора
declare @startDateNext DateTime	--	дата начала действия следующего
declare @countNext decimal(18,6)	--	приращение индикатора
declare @indicatorNextId uniqueidentifier -- код индикатора
declare @keyHash varchar(255) -- ключ текущего индикатора
declare @rowNumber int -- порядковый номер текущего индикатора
declare @count decimal (18,6) -- количество товара

-- Суммируем индикаторы проведенного
-- Первый ключ
SET @keyHash =(
SELECT TOP(1)
	t.KeyHash
FROM 
	#T_accepted t
	ORDER BY t.KeyHash)
	
WHILE(@keyHash is not null)
begin
	SET @indicatorId = null
	-- первый индикатор
	SELECT TOP(1) 
		@indicatorId = t.Id,
		@count = t.Count,
		@keyHash = t.KeyHash,
		@rowNumber = t.RowNumber
	FROM 
		#T_accepted t
	WHERE 
		t.KeyHash = @keyHash
	ORDER BY t.RowNumber

	WHILE( @indicatorId is not null)
	begin
		-- обнуляем переменные
		SET @startDateNext = null
		SET @countNext = null
		SET @indicatorNextId = null
		
		-- получаем данные по следующему индикатору
		SELECT 
			@startDateNext = t.StartDate,
			@countNext = t.Count,
			@indicatorNextId = t.Id
		FROM 
			#T_accepted t 
		WHERE 
			t.KeyHash = @keyHash 
			AND t.RowNumber = @rowNumber + 1
		
		-- Закрываем предыдущий
		UPDATE #T_accepted
			SET EndDate = @startDateNext
		WHERE Id = @indicatorId
		
		-- Обновляет следующий
		UPDATE #T_accepted
			SET Count = @countNext + @count,
				PreviousId = @indicatorId
		WHERE Id = @indicatorNextId
		
		-- переходим к следующему
		SET @indicatorId = null	-- На случай, когда выборка будет пустой
		-- первый индикатор
		SELECT TOP(1) 
			@indicatorId = t.Id,
			@count = t.Count,
			@keyHash = t.KeyHash,
			@rowNumber = t.RowNumber
		FROM 
			#T_accepted t
		WHERE
			t.KeyHash = @keyHash
			AND t.RowNumber = @rowNumber + 1
	end
	-- очередной ключ
	SET @keyHash =(
	SELECT TOP(1)
		t.KeyHash
	FROM 
		#T_accepted t
	WHERE
		t.KeyHash > @keyHash
		ORDER BY t.KeyHash)
end

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Суммируем индикаторы по ключам
declare @indicatorId uniqueidentifier --	код индикатора
declare @startDateNext DateTime	--	дата начала действия следующего
declare @countNext decimal(18,6)	--	приращение индикатора
declare @indicatorNextId uniqueidentifier -- код индикатора
declare @keyHash varchar(255) -- ключ текущего индикатора
declare @rowNumber int -- порядковый номер текущего индикатора
declare @count decimal (18,6) -- количество товара

-- Суммируем индикаторы точного

-- Первый ключ
SET @keyHash =(
SELECT TOP(1)
	t.KeyHash
FROM 
	#T_exact t
	ORDER BY t.KeyHash)
	
WHILE(@keyHash is not null)
begin
	SET @indicatorId = null
	-- первый индикатор
	SELECT TOP(1) 
		@indicatorId = t.Id,
		@count = t.Count,
		@rowNumber = t.RowNumber
	FROM 
		#T_exact t
	WHERE
		t.KeyHash = @keyHash
	ORDER BY t.RowNumber

	WHILE( @indicatorId is not null)
	begin
		-- обнуляем переменные
		SET @startDateNext = null
		SET @countNext = null
		SET @indicatorNextId = null
		
		-- получаем данные по следующему индикатору
		SELECT 
			@startDateNext = t.StartDate,
			@countNext = t.Count,
			@indicatorNextId = t.Id
		FROM 
			#T_exact t 
		WHERE 
			t.KeyHash = @keyHash 
			AND t.RowNumber = @rowNumber + 1
		
		-- Закрываем предыдущий
		UPDATE #T_exact
			SET EndDate = @startDateNext
		WHERE Id = @indicatorId
		
		-- Обновляет следующий
		UPDATE #T_exact
			SET Count = @countNext + @count,
				PreviousId = @indicatorId
		WHERE Id = @indicatorNextId
		
		-- переходим к следующему
		SET @indicatorId = null	-- На случай, когда выборка будет пустой
		-- первый индикатор
		SELECT TOP(1) 
			@indicatorId = t.Id,
			@count = t.Count,
			@rowNumber = t.RowNumber
		FROM 
			#T_exact t
		WHERE
			t.KeyHash = @keyHash
			AND t.RowNumber = @rowNumber + 1
	end
	
	-- очередной ключ
	SET @keyHash =(
	SELECT TOP(1)
		t.KeyHash
	FROM 
		#T_exact t
	WHERE
		t.KeyHash > @keyHash
		ORDER BY t.KeyHash)
end

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Переносим данные из временных таблиц в основные

DELETE FROM OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

INSERT INTO OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator (
	Id,
	StartDate,
	EndDate,
	StorageId,
	AccountOrganizationId,
	ArticleId,
	BatchId,
	PurchaseCost,
	Count,
	PreviousId
)
SELECT 
	Id,
	StartDate,
	EndDate,
	StorageId,
	AccountOrganizationId,
	ArticleId,
	BatchId,
	PurchaseCost,
	Count,
	PreviousId
FROM #T_accepted
		
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

DELETE FROM OutgoingAcceptedFromExactArticleAvailabilityIndicator

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

INSERT INTO OutgoingAcceptedFromExactArticleAvailabilityIndicator (
	Id,
	StartDate,
	EndDate,
	StorageId,
	AccountOrganizationId,
	ArticleId,
	BatchId,
	PurchaseCost,
	Count,
	PreviousId
)
SELECT 
	Id,
	StartDate,
	EndDate,
	StorageId,
	AccountOrganizationId,
	ArticleId,
	BatchId,
	PurchaseCost,
	Count,
	PreviousId
FROM #T_exact

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop index IX_#T_accepted on #T_accepted

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop index IX_#T_exact on #T_exact

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table #T_exact

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table #T_accepted

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop function dbo.GetSourceReceiptDate

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop procedure dbo.AddValueToAcceptedIndicator

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop procedure dbo.AddValueToExectIndicator

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop procedure dbo.InsertIndicator

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO