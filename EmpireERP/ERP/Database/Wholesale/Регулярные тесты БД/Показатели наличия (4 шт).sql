-- Получение даты прянятия накладной
CREATE FUNCTION GetWaybillReceiptDate
(
  @waybillRowId uniqueidentifier,	-- код источника позиции накладной
  @waybillTypeId tinyint	-- код типа накладной источника
)
RETURNS datetime
WITH SCHEMABINDING
AS
BEGIN
	declare @receiptDate DateTime -- дата "приемки" накладной
		
	-- Получаем данные о дате поступления источника в точное наличие
	IF(@waybillTypeId = 1)	-- приходная накладная
		SELECT TOP(1) 
			@receiptDate = 
			CASE
				WHEN rwr.AreCountDivergencesAfterReceipt = 1 OR rwr.AreSumDivergencesAfterReceipt = 1 
					THEN rw.ApprovementDate
					ELSE rw.ReceiptDate
			END
		FROM
			dbo.ReceiptWaybill rw
			join dbo.ReceiptWaybillRow rwr on rwr.ReceiptWaybillId = rw.Id
		WHERE
			rwr.Id = @waybillRowId
	ELSE IF (@waybillTypeId = 2)	-- накладная перемещения
		SELECT TOP(1) 
			@receiptDate = mw.ReceiptDate
		FROM
			dbo.MovementWaybill mw
			join dbo.MovementWaybillRow mwr on mwr.MovementWaybillId = mw.Id
		WHERE
			mwr.Id = @waybillRowId
	ELSE IF (@waybillTypeId = 5)	-- накладная смены собственника
		SELECT TOP(1) 
			@receiptDate = cow.ChangeOwnerDate
		FROM
			dbo.ChangeOwnerWaybill cow
			join dbo.ChangeOwnerWaybillRow cowr on cowr.ChangeOwnerWaybillId = cow.Id
		WHERE
			cowr.Id = @waybillRowId
	ELSE IF (@waybillTypeId = 6)	-- накладная возврата товара
		SELECT TOP(1) 
			@receiptDate = rfcw.ReceiptDate
		FROM
			dbo.ReturnFromClientWaybill rfcw
			join dbo.ReturnFromClientWaybillRow rfcwr on rfcwr.ReturnFromClientWaybillId = rfcw.Id
		WHERE
			rfcwr.Id = @waybillRowId

	RETURN @receiptDate
END
GO

-- Добавление индикатора в таблицу проведенного исходящего из проведенного
CREATE PROCEDURE AddValueToOutgoingFromAcceptedIndicator 
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
	
	SET @exists = (
		SELECT 
			COUNT(*)
		FROM 
			#T_OutgoingFromAccepted t
		WHERE
			t.KeyHash = @keyHash
			AND t.StartDate = @date
	)
	
	IF (@exists = 0)
		-- Вставляем изменение в проведенном
		INSERT INTO #T_OutgoingFromAccepted(
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
		UPDATE #T_OutgoingFromAccepted
			SET Count = Count + @count
		WHERE
			KeyHash = @keyHash
			AND StartDate = @date

END
GO

-- Добавление индикатора в таблицу исходящего проведенного из точного
CREATE PROCEDURE AddValueToOutgoingFromExectIndicator 
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
	declare @exists int -- пирзнак существования индикатора с тем же ключом и на ту же дату
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
			#T_OutgoingFromExact t
		WHERE
			t.KeyHash = @keyHash
			AND t.StartDate = @date
	)
		
	IF (@exists = 0)
		-- Вставляем изменение в точном
		INSERT INTO #T_OutgoingFromExact(
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
		UPDATE #T_OutgoingFromExact
			SET Count = Count + @count
		WHERE
			KeyHash = @keyHash
			AND StartDate = @date
END
GO


-- Добавление индикатора в таблицу точного наличия
CREATE PROCEDURE AddValueToExactIndicator 
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
	declare @exists int -- пирзнак существования индикатора с тем же ключом и на ту же дату
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
			#T_Exact t
		WHERE
			t.KeyHash = @keyHash
			AND t.StartDate = @date
	)
		
	IF (@exists = 0)
		-- Вставляем изменение в точном наличии
		INSERT INTO #T_Exact(
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
		UPDATE #T_Exact
			SET Count = Count + @count
		WHERE
			KeyHash = @keyHash
			AND StartDate = @date
END
GO


-- Добавление индикатора в таблицу проведенного входящего наличия
CREATE PROCEDURE AddValueToIncomingAcceptedIndicator 
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
	declare @exists int -- признак существования индикатора с тем же ключом и на ту же дату
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
			#T_IncomingAccepted t
		WHERE
			t.KeyHash = @keyHash
			AND t.StartDate = @date
	)
		
	IF (@exists = 0)
		-- Вставляем изменение в проведенном входящем
		INSERT INTO #T_IncomingAccepted(
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
		UPDATE #T_IncomingAccepted
			SET Count = Count + @count
		WHERE
			KeyHash = @keyHash
			AND StartDate = @date
END
GO

-- Вставка входящих индикаторов
CREATE PROCEDURE InsertIncomingIndicator 
(
  @waybillRowId uniqueidentifier,
  @acceptDate DateTime, 
  @receiptDate DateTime,
  @storageId smallint,
  @accountOrganizationId int,
  @articleId int,
  @count decimal(18,6),
  @batchId uniqueidentifier
)
AS
BEGIN
	declare @waybillRowArticleMovementId uniqueidentifier -- код строки связи движения товара
	declare @destinationReceiptDate DateTime -- дата "приемки" накладной
	declare @purchaseCost decimal (18,6) -- закупочная цена
	declare @destinationCount decimal (18,6)
	declare @destinationWaybillRowId uniqueidentifier;	-- код исходящей позиции
	declare @destinationWaybillTypeId tinyint;	-- код типа исходящей накладной
	declare @minusCount decimal(18,6)	-- отрицательное значение количества товара
	-- Получаем закупочную цену
	SELECT
		@purchaseCost = rwr.PurchaseCost
	FROM
		ReceiptWaybillRow rwr
	WHERE
		rwr.Id = @batchId

	-- Поступление во входящее проведенное наличие
	exec AddValueToIncomingAcceptedIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
	IF (@receiptDate is not null)	-- Если позиция принята
	BEGIN
		SET @minusCount = @count * -1
		-- Переводим из проведенного входящего в точное
		exec AddValueToIncomingAcceptedIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@minusCount
		exec AddValueToExactIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
	END
END
GO

-- Вставка исходящих индикаторов
CREATE PROCEDURE InsertOutgoingIndicator 
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
	
	-- получаем данные по источнику		
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
		 
		SET @sourceReceiptDate = dbo.GetWaybillReceiptDate(@sourceWaybillRowId, @sourceWaybillTypeId)	-- Получаем дату приемки источника

		-- Если источник не принят на момент проводки исходящей, то
		IF(@sourceReceiptDate is null OR @sourceReceiptDate > @acceptDate)
		begin
			-- Вставляем изменение в проведенном
			exec AddValueToOutgoingFromAcceptedIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
			-- если источник был принят позже проводки
			IF (@sourceReceiptDate is not null)
			begin
				-- переводим резерв из проведенного в точное
				-- именьшаем проведенное
				exec AddValueToOutgoingFromAcceptedIndicator @sourceReceiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost, @minusCount
				
				-- увеличиваем точное
				exec AddValueToOutgoingFromExectIndicator @sourceReceiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count		
			end
		end
		ELSE
			-- иначе вставляем изменение в точном
			exec AddValueToOutgoingFromExectIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
		
		-- Если исходящая накладная принята, то 
		IF(@receiptDate is not null)
		BEGIN
			-- уменьшаем точное
			exec AddValueToOutgoingFromExectIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost, @minusCount
			exec AddValueToExactIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost, @minusCount
		END
			
			
		-- Получаем данные о следующем источнике позиции		 	
		SET @waybillRowArticleMovementId = (
		SELECT TOP(1)
			wram.Id		
		FROM
			dbo.WaybillRowArticleMovement wram
		WHERE
			wram.DestinationWaybillRowId = @waybillRowId
			AND wram.Id > @waybillRowArticleMovementId
			ORDER BY wram.Id)
		
		-- получаем данные по источнику		
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
GO

-- таблица исходной информации для исходящего из точного
create table #T_OutgoingFromExact (
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
GO
-- таблица исходной информации для исходящего из проведенного
create table #T_OutgoingFromAccepted (
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
GO

-- таблица исходной информации для точного наличия
create table #T_Exact (
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
GO

-- таблица исходной информации для проведенного входящего
create table #T_IncomingAccepted (
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
GO

-- Таблица, содержащая пары расходящихся индикаторов
CREATE TABLE #Diff(
	OldId UNIQUEIDENTIFIER not null,
	NewId UNIQUEIDENTIFIER not null
)

CREATE INDEX IX_#T_OutgoingFromAccepted ON #T_OutgoingFromAccepted (KeyHash, RowNumber, Id, storageId, accountOrganizationId, articleId, batchId);
GO

CREATE INDEX IX_#T_OutgoingFromExact ON #T_OutgoingFromExact (KeyHash, RowNumber, Id, storageId, accountOrganizationId, articleId, batchId);
GO

CREATE INDEX IX_#T_Exact ON #T_Exact (KeyHash, RowNumber, Id, storageId, accountOrganizationId, articleId, batchId);
GO

CREATE INDEX IX_#T_IncomingAccepted ON #T_IncomingAccepted (KeyHash, RowNumber, Id, storageId, accountOrganizationId, articleId, batchId);
GO

BEGIN TRY
	BEGIN TRAN
	
	SET NOCOUNT ON
					
	declare @waybillId uniqueidentifier;	-- код текущей накладной
	declare @waybillRowId uniqueidentifier;	-- код текущей позиции накладной

	declare @startDate DateTime -- дата события накладной
	declare @storageId smallint -- код МХ
	declare @accountOrganizationId int -- код организации аккаунта
	declare @recipientAccountOrganizationId int -- код организации аккаунта
	declare @articleId int -- код товара
	declare @articleCount decimal(18,6) -- количество товара
	declare @batchId uniqueidentifier -- код партии

	declare @acceptDate DateTime -- дата проводки накладной
	declare @receiptDate DateTime -- дата "приемки" накладной
	declare @approvedDate DateTime -- дата "согласования" накладной прихода
	declare @sourceReceiptDate DateTime -- дата "приемки" накладной
	declare @count decimal (18,6) -- количество товара
	declare @recipientStorageId smallint -- код МХ
	
	PRINT('Начало сбора первички')
	
	-- 1) Накладная перемещения
	PRINT('   1) Накладная перемещения')
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
		@recipientStorageId = mw.RecipientStorageId,
		@accountOrganizationId = mw.SenderId,
		@recipientAccountOrganizationId = mw.RecipientId,
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
			@articleCount = mwr.MovingCount,
			@batchId = mwr.ReceiptWaybillRowId
		FROM 
			MovementWaybillRow mwr
		WHERE
			mwr.Id = @waybillRowId	

		-- Цикл просмотра всех позиций
		WHILE(@waybillRowId is not null)
		begin
			-- сохраняем данные по позиции			
			exec InsertOutgoingIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
			exec InsertIncomingIndicator @waybillRowId,@acceptDate,@receiptDate,@recipientStorageId,@recipientAccountOrganizationId,@articleId,@articleCount,@batchId

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
				@articleCount = mwr.MovingCount,
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
			@recipientStorageId = mw.RecipientStorageId,
			@accountOrganizationId = mw.SenderId,
			@recipientAccountOrganizationId = mw.RecipientId,
			@acceptDate = mw.AcceptanceDate,
			@receiptDate = mw.ReceiptDate
		FROM 
			MovementWaybill mw
		WHERE
			mw.Id = @waybillId
	end
	
	-- 2) Накладная смены собственника
	PRINT('   2) Накладная смены собственника')
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
		@recipientAccountOrganizationId = cow.ChangeOwnerWaybillRecipientId,
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
			@articleCount = cowr.MovingCount,
			@batchId = cowr.ChangeOwnerWaybillRowReceiptWaybillRowId,
			@count = cowr.MovingCount
		FROM 
			ChangeOwnerWaybillRow cowr
		WHERE
			cowr.Id = @waybillRowId	

		-- Цикл просмотра всех позиций
		WHILE(@waybillRowId is not null)
		begin		
			-- сохраняем данные по позиции			
			exec InsertOutgoingIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
			exec InsertIncomingIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@recipientAccountOrganizationId,@articleId,@articleCount,@batchId
			
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
				@articleCount = cowr.MovingCount,
				@batchId = cowr.ChangeOwnerWaybillRowReceiptWaybillRowId,
				@count = cowr.MovingCount
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
			@recipientAccountOrganizationId = cow.ChangeOwnerWaybillRecipientId,
			@acceptDate = cow.AcceptanceDate,
			@receiptDate = cow.ChangeOwnerDate
		FROM 
			ChangeOwnerWaybill cow
		WHERE
			cow.Id = @waybillId
	end


	-- 3) Накладная списания
	PRINT('   3) Накладная списания')
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
			exec InsertOutgoingIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
			
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


	-- 4) Накладная реализации
	PRINT('   4) Накладная реализации')
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
			exec InsertOutgoingIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
			
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

	-- 5) Накладная возврата товара
	PRINT('   5) Накладная возврата товара')
	-- Первая накладаня
	SET @waybillId = (
	SELECT TOP(1) 
		Id
	FROM 
		ReturnFromClientWaybill
	WHERE
		DeletionDate is null
		AND AcceptanceDate is not null
		ORDER BY Id)
		
	SELECT TOP(1) 
		@storageId = rfcw.ReturnFromClientWaybillRecipientStorageId,
		@accountOrganizationId = c.AccountOrganizationId,
		@acceptDate = rfcw.AcceptanceDate,
		@receiptDate = rfcw.ReceiptDate
	FROM 
		ReturnFromClientWaybill rfcw
		join Deal d on d.Id = rfcw.ReturnFromClientWaybillDealId
		join Contract c on c.Id = d.ClientContractId
	WHERE
		rfcw.Id = @waybillId

	-- Цикл по накладным 	
	WHILE (@waybillId is not null)
	begin
		-- Первая позиция накладной
		SET @waybillRowId = (
		SELECT TOP(1) Id
		FROM ReturnFromClientWaybillRow
		WHERE
			DeletionDate is null
			AND ReturnFromClientWaybillId = @waybillId
			ORDER BY Id)

		SELECT TOP(1) 
			@articleId = ArticleId,
			@articleCount = ReturnCount,
			@batchId = ReceiptWaybillRowId
		FROM 
			ReturnFromClientWaybillRow
		WHERE
			Id = @waybillRowId	

		-- Цикл просмотра всех позиций
		WHILE(@waybillRowId is not null)
		begin		
			-- сохраняем данные по позиции			
			exec InsertIncomingIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@articleCount,@batchId
			
			-- Очередная позиция накладной
			SET @waybillRowId = (
			SELECT TOP(1) 
				Id
			FROM ReturnFromClientWaybillRow
			WHERE
				DeletionDate is null
				AND ReturnFromClientWaybillId = @waybillId
				AND Id > @waybillRowId
				ORDER BY Id)

			SELECT TOP(1) 
				@articleId = ArticleId,
				@articleCount = ReturnCount,
				@batchId = ReceiptWaybillRowId
			FROM 
				ReturnFromClientWaybillRow
			WHERE
				Id = @waybillRowId			
		end
		
		-- Очередная накладная
		SET @waybillId = (
		SELECT TOP(1) 
			Id
		FROM 
			ReturnFromClientWaybill
		WHERE
			DeletionDate is null
			AND AcceptanceDate is not null
			AND Id > @waybillId
			ORDER BY Id)
			
		SELECT TOP(1) 
			@storageId = rfcw.ReturnFromClientWaybillRecipientStorageId,
			@accountOrganizationId = c.AccountOrganizationId,
			@acceptDate = rfcw.AcceptanceDate,
			@receiptDate = rfcw.ReceiptDate
		FROM 
			ReturnFromClientWaybill rfcw
			join Deal d on d.Id = rfcw.ReturnFromClientWaybillDealId
			join Contract c on c.Id = d.ClientContractId
		WHERE
			rfcw.Id = @waybillId
	end
	
	-- 6) Приходная накладная
	PRINT('   6) Приходная накладная')
	declare @areDivergencesAfterReceipt bit -- Признак расхождений после приемки
	declare @added bit -- Признак добавления позиции при принятии
	declare @purchaseCost decimal(18,6)
	declare @pendingCount decimal(18,6)
	declare @receiptedCount decimal(18,6)
	declare @approvedCount decimal(18,6)
	
	-- Первая накладаня
	SET @waybillId = (
	SELECT TOP(1) 
		Id
	FROM 
		ReceiptWaybill
	WHERE
		DeletionDate is null
		AND AcceptanceDate is not null
		ORDER BY Id)
		
	SELECT TOP(1) 
		@storageId = ReceiptWaybillReceiptStorageId,
		@accountOrganizationId = AccountOrganizationId,
		@acceptDate = AcceptanceDate,
		@receiptDate = ReceiptDate,
		@approvedDate = ApprovementDate
	FROM 
		ReceiptWaybill
	WHERE
		Id = @waybillId

	-- Цикл по накладным 	
	WHILE (@waybillId is not null)
	begin
		-- Первая позиция накладной
		SET @waybillRowId = (
		SELECT TOP(1) Id
		FROM ReceiptWaybillRow
		WHERE
			DeletionDate is null AND ReceiptWaybillId = @waybillId
			ORDER BY Id)

		SELECT TOP(1) 
			@articleId = ArticleId,
			@pendingCount = PendingCount,
			@receiptedCount = ReceiptedCount,
			@approvedCount = ApprovedCount,
			@areDivergencesAfterReceipt = CASE WHEN (AreCountDivergencesAfterReceipt = 1 OR AreSumDivergencesAfterReceipt = 1) THEN 1 ELSE 0 END,
			@added = CASE WHEN PendingCount = 0 THEN 1 ELSE 0 END,
			@batchId = Id
		FROM 
			ReceiptWaybillRow
		WHERE
			Id = @waybillRowId	
		
		-- Цикл просмотра всех позиций
		WHILE @waybillRowId is not null
		begin		
			-- сохраняем данные по позиции			
		
			-- Получаем закупочную цену
			SELECT
				@purchaseCost = rwr.PurchaseCost
			FROM
				ReceiptWaybillRow rwr
			WHERE
				rwr.Id = @waybillRowId
				
			IF @approvedDate is not null AND @added = 0 AND @areDivergencesAfterReceipt = 1
			BEGIN
				SET @count = @pendingCount * -1
				-- Добавляем в проведенное
				exec AddValueToIncomingAcceptedIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@pendingCount
				-- Убираем из проведенного входящего, т.к. есть расхождения
				exec AddValueToIncomingAcceptedIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
				-- Добавляем в точное после согласования
				exec AddValueToExactIndicator @approvedDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@approvedCount
			END
			IF @approvedDate is not null AND @added = 1
				-- Добавляем в точное добавленную позицию
				exec AddValueToExactIndicator @approvedDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@approvedCount
			IF @approvedDate is null AND @receiptDate is not null AND @added = 0 AND @areDivergencesAfterReceipt = 1
			BEGIN
				SET @count = @pendingCount * -1
				-- Добавляем в проведенное
				exec AddValueToIncomingAcceptedIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@pendingCount
				-- Убираем из проведенного входящего, т.к. есть расхождения
				exec AddValueToIncomingAcceptedIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
			END
			IF @receiptDate is not null AND @added = 0 AND @areDivergencesAfterReceipt = 0
			BEGIN
				SET @count = @pendingCount * -1
				-- Добавляем в проведенное
				exec AddValueToIncomingAcceptedIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@pendingCount
				-- Убираем из проведенного входящего и перемещаем в точное
				exec AddValueToIncomingAcceptedIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
				exec AddValueToExactIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@pendingCount
			END
			IF @receiptDate is null
				-- Добавляем в проведенное
				exec AddValueToIncomingAcceptedIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@pendingCount				
			
			
			-- Очередная позиция накладной
			SET @waybillRowId = (
			SELECT TOP(1) Id
			FROM ReceiptWaybillRow
			WHERE
				DeletionDate is null
				AND ReceiptWaybillId = @waybillId
				AND Id > @waybillRowId
				ORDER BY Id)

			SELECT TOP(1) 
				@articleId = ArticleId,
				@pendingCount = PendingCount,
				@receiptedCount = ReceiptedCount,
				@approvedCount = ApprovedCount,
				@areDivergencesAfterReceipt = CASE WHEN (AreCountDivergencesAfterReceipt = 1 OR AreSumDivergencesAfterReceipt = 1) THEN 1 ELSE 0 END,
				@added = CASE WHEN PendingCount = 0 THEN 1 ELSE 0 END,
				@batchId = Id
			FROM 
				ReceiptWaybillRow
			WHERE
				Id = @waybillRowId				
		end
		
		-- Очередная накладная
		SET @waybillId = (
		SELECT TOP(1) 
			Id
		FROM 
			ReceiptWaybill
		WHERE
			DeletionDate is null
			AND AcceptanceDate is not null
			AND Id > @waybillId
			ORDER BY Id)
			
		SELECT TOP(1) 
			@storageId = ReceiptWaybillReceiptStorageId,
			@accountOrganizationId = AccountOrganizationId,
			@acceptDate = AcceptanceDate,
			@receiptDate = ReceiptDate,
			@approvedDate = ApprovementDate
		FROM 
			ReceiptWaybill
		WHERE
			Id = @waybillId
	end

---------------------------------------------------------------------------------------------------------------------------
	PRINT('Проставляем порядковые номера')
	-- Проставляем порядковые номера строк в индикаторах
	UPDATE #T_OutgoingFromAccepted
		SET RowNumber = d.rn
	FROM (
		SELECT 
			ROW_NUMBER() over (partition by t.KeyHash order by t.StartDate) rn, 
			t.KeyHash,
			t.Id
		FROM  #T_OutgoingFromAccepted t) d
	WHERE
		#T_OutgoingFromAccepted.KeyHash = d.KeyHash
		AND #T_OutgoingFromAccepted.Id = d.Id
		
	-- Проставляем порядковые номера строк в индикаторах
	UPDATE #T_OutgoingFromExact
		SET RowNumber = d.rn
	FROM (
		SELECT 
			ROW_NUMBER() over (partition by t.KeyHash order by t.StartDate) rn, 
			t.KeyHash,
			t.Id
		FROM  #T_OutgoingFromExact t) d
	WHERE
		#T_OutgoingFromExact.KeyHash = d.KeyHash
		AND #T_OutgoingFromExact.Id = d.Id
		
	-- Проставляем порядковые номера строк в индикаторах
	UPDATE #T_Exact
		SET RowNumber = d.rn
	FROM (
		SELECT 
			ROW_NUMBER() over (partition by t.KeyHash order by t.StartDate) rn, 
			t.KeyHash,
			t.Id
		FROM  #T_Exact t) d
	WHERE
		#T_Exact.KeyHash = d.KeyHash
		AND #T_Exact.Id = d.Id
	
	-- Проставляем порядковые номера строк в индикаторах
	UPDATE #T_IncomingAccepted
		SET RowNumber = d.rn
	FROM (
		SELECT 
			ROW_NUMBER() over (partition by t.KeyHash order by t.StartDate) rn, 
			t.KeyHash,
			t.Id
		FROM  #T_IncomingAccepted t) d
	WHERE
		#T_IncomingAccepted.KeyHash = d.KeyHash
		AND #T_IncomingAccepted.Id = d.Id
---------------------------------------------------------------------------------------------------------------------------
	-- Суммируем индикаторы по ключам
	declare @indicatorId uniqueidentifier --	код индикатора
	declare @startDateNext DateTime	--	дата начала действия следующего
	declare @countNext decimal(18,6)	--	приращение индикатора
	declare @indicatorNextId uniqueidentifier -- код индикатора
	declare @keyHash varchar(255) -- ключ текущего индикатора
	declare @rowNumber int -- порядковый номер текущего индикатора

	-- Суммируем индикаторы исходящего проведенного из проведенного
	PRINT('Суммируем данные показателя: исходящего проведенного из проведенного')
	
	-- Первый ключ
	SET @keyHash =(
	SELECT TOP(1)
		t.KeyHash
	FROM 
		#T_OutgoingFromAccepted t
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
			#T_OutgoingFromAccepted t
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
				#T_OutgoingFromAccepted t 
			WHERE 
				t.KeyHash = @keyHash 
				AND t.RowNumber = @rowNumber + 1
			
			-- Закрываем предыдущий
			UPDATE #T_OutgoingFromAccepted
				SET EndDate = @startDateNext
			WHERE Id = @indicatorId
			
			-- Обновляет следующий
			UPDATE #T_OutgoingFromAccepted
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
				#T_OutgoingFromAccepted t
			WHERE
				t.KeyHash = @keyHash
				AND t.RowNumber = @rowNumber + 1
		end
		-- очередной ключ
		SET @keyHash =(
		SELECT TOP(1)
			t.KeyHash
		FROM 
			#T_OutgoingFromAccepted t
		WHERE
			t.KeyHash > @keyHash
			ORDER BY t.KeyHash)
	end
-------------------------------------------------------------------
	-- Суммируем индикаторы исходящего проведенного из точного
	PRINT('Суммируем данные показателя: исходящего проведенного из точного')
	-- Первый ключ
	SET @keyHash =(
	SELECT TOP(1)
		t.KeyHash
	FROM 
		#T_OutgoingFromExact t
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
			#T_OutgoingFromExact t
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
				#T_OutgoingFromExact t 
			WHERE 
				t.KeyHash = @keyHash 
				AND t.RowNumber = @rowNumber + 1
			
			-- Закрываем предыдущий
			UPDATE #T_OutgoingFromExact
				SET EndDate = @startDateNext
			WHERE Id = @indicatorId
			
			-- Обновляет следующий
			UPDATE #T_OutgoingFromExact
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
				#T_OutgoingFromExact t
			WHERE
				t.KeyHash = @keyHash
				AND t.RowNumber = @rowNumber + 1
		end
		
		-- очередной ключ
		SET @keyHash =(
		SELECT TOP(1)
			t.KeyHash
		FROM 
			#T_OutgoingFromExact t
		WHERE
			t.KeyHash > @keyHash
			ORDER BY t.KeyHash)
	end

---------------------------------
	-- Суммируем индикаторы точного
	PRINT('Суммируем данные показателя: точное наличие')
	
	-- Первый ключ
	SET @keyHash =(
	SELECT TOP(1)
		t.KeyHash
	FROM 
		#T_Exact t
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
			#T_Exact t
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
				#T_Exact t 
			WHERE 
				t.KeyHash = @keyHash 
				AND t.RowNumber = @rowNumber + 1
			
			-- Закрываем предыдущий
			UPDATE #T_Exact
				SET EndDate = @startDateNext
			WHERE Id = @indicatorId
			
			-- Обновляет следующий
			UPDATE #T_Exact
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
				#T_Exact t
			WHERE
				t.KeyHash = @keyHash
				AND t.RowNumber = @rowNumber + 1
		end
		
		-- очередной ключ
		SET @keyHash =(
		SELECT TOP(1)
			t.KeyHash
		FROM 
			#T_Exact t
		WHERE
			t.KeyHash > @keyHash
			ORDER BY t.KeyHash)
	end

---------------------------------
	-- Суммируем индикаторы проведенного входящего
	PRINT('Суммируем данные показателя: проведенного входящего')
	
	-- Первый ключ
	SET @keyHash =(
	SELECT TOP(1)
		t.KeyHash
	FROM 
		#T_IncomingAccepted t
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
			#T_IncomingAccepted t
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
				#T_IncomingAccepted t 
			WHERE 
				t.KeyHash = @keyHash 
				AND t.RowNumber = @rowNumber + 1
			
			-- Закрываем предыдущий
			UPDATE #T_IncomingAccepted
				SET EndDate = @startDateNext
			WHERE Id = @indicatorId
			
			-- Обновляет следующий
			UPDATE #T_IncomingAccepted
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
				#T_IncomingAccepted t
			WHERE
				t.KeyHash = @keyHash
				AND t.RowNumber = @rowNumber + 1
		end
		
		-- очередной ключ
		SET @keyHash =(
		SELECT TOP(1)
			t.KeyHash
		FROM 
			#T_IncomingAccepted t
		WHERE
			t.KeyHash > @keyHash
			ORDER BY t.KeyHash)
	end
--*****************************************************************************************
--	Сравнение старых данными с новыми
--*****************************************************************************************
	
	DECLARE @oldId UNIQUEIDENTIFIER	-- код текущего старого индикатора
	DECLARE @newId UNIQUEIDENTIFIER	-- код текущего нового индикатора
	DECLARE @oldCount DECIMAL(18,6) -- старое количество
	DECLARE @newCount DECIMAL(18,6) -- новое количество
	DECLARE @oldStartDate DateTime -- дата начала действия
	
	--------------------------------------------------
	-- Сравниваем исходящие из точного
	PRINT('Сравниваем исходящие из точного')
	
	SELECT Count(*) 'Расхождения в исходящем из точного'
	--SELECT *
	FROM #T_OutgoingFromExact t
	full JOIN  OutgoingAcceptedFromExactArticleAvailabilityIndicator a on 
		a.StartDate >= t.StartDate AND (a.StartDate < t.EndDate OR t.EndDate is null)
		AND a.StorageId = t.StorageId 
		AND a.AccountOrganizationId = t.AccountOrganizationId 
		AND a.ArticleId = t.ArticleId 
		AND a.BatchId = t.BatchId 
		AND a.PurchaseCost = t.PurchaseCost 
		AND a.Count = t.Count
    WHERE (a.Id is null OR t.Id is null) AND (a.Count <> 0 OR t.Count <> 0)

	--------------------------------------------------
	-- Сравниваем исходящие из проведенного
	PRINT('Сравниваем исходящие из проведенного')
	
	SELECT Count(*) 'Расхождения в исходящем из проведенного'
	--SELECT *
	FROM #T_OutgoingFromAccepted t
	full JOIN  OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator a on 
		a.StartDate >= t.StartDate AND (a.StartDate < t.EndDate OR t.EndDate is null)
		AND a.StorageId = t.StorageId 
		AND a.AccountOrganizationId = t.AccountOrganizationId 
		AND a.ArticleId = t.ArticleId 
		AND a.BatchId = t.BatchId 
		AND a.PurchaseCost = t.PurchaseCost 
		AND a.Count = t.Count
    WHERE (a.Id is null OR t.Id is null) AND (a.Count <> 0 OR t.Count <> 0)
    
	--------------------------------------------------
	-- Сравниваем точное наличие
	PRINT('Сравниваем точное наличие')
	
	SELECT Count(*) 'Расхождения в точном наличии'
	--SELECT *
	FROM #T_Exact t
	full JOIN  ExactArticleAvailabilityIndicator a on 
		a.StartDate >= t.StartDate AND (a.StartDate < t.EndDate OR t.EndDate is null)
		AND a.StorageId = t.StorageId 
		AND a.AccountOrganizationId = t.AccountOrganizationId 
		AND a.ArticleId = t.ArticleId 
		AND a.BatchId = t.BatchId 
		AND a.PurchaseCost = t.PurchaseCost 
		AND a.Count = t.Count
    WHERE (a.Id is null OR t.Id is null) AND (a.Count <> 0 OR t.Count <> 0)
    
	--------------------------------------------------
	-- Сравниваем входящее проведенное
	PRINT('Сравниваем входящее проведенное')
	
	SELECT Count(*) 'Расхождения во входящем проведенном'
	--SELECT *
	FROM #T_IncomingAccepted t
	full JOIN  IncomingAcceptedArticleAvailabilityIndicator a on 
		a.StartDate >= t.StartDate AND (a.StartDate < t.EndDate OR t.EndDate is null)
		AND a.StorageId = t.StorageId 
		AND a.AccountOrganizationId = t.AccountOrganizationId 
		AND a.ArticleId = t.ArticleId 
		AND a.BatchId = t.BatchId 
		AND a.PurchaseCost = t.PurchaseCost 
		AND a.Count = t.Count
    WHERE (a.Id is null OR t.Id is null) AND (a.Count <> 0 OR t.Count <> 0)
    
	--COMMIT TRAN
	ROLLBACK TRAN
END TRY
BEGIN CATCH
	IF XACT_STATE() <> 0 ROLLBACK TRAN
		
	RETURN
END CATCH

DROP INDEX IX_#T_OutgoingFromAccepted on #T_OutgoingFromAccepted
DROP INDEX IX_#T_OutgoingFromExact on #T_OutgoingFromExact
DROP INDEX IX_#T_Exact ON #T_Exact
DROP INDEX IX_#T_IncomingAccepted ON #T_IncomingAccepted

DROP TABLE #T_OutgoingFromExact
DROP TABLE #T_OutgoingFromAccepted
DROP TABLE #T_Exact
DROP TABLE #T_IncomingAccepted
DROP TABLE #Diff

DROP FUNCTION dbo.GetWaybillReceiptDate
DROP PROCEDURE dbo.AddValueToOutgoingFromAcceptedIndicator
DROP PROCEDURE dbo.AddValueToOutgoingFromExectIndicator
DROP PROCEDURE dbo.InsertOutgoingIndicator
DROP PROCEDURE dbo.AddValueToExactIndicator
DROP PROCEDURE dbo.AddValueToIncomingAcceptedIndicator
DROP PROCEDURE dbo.InsertIncomingIndicator